using FFMpegCore;
using FFMpegCore.Pipes;
using NextAudio.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NextAudio.FFMpegCore
{
    /// <summary>
    /// Represents 
    /// </summary>
    public class FFMpegCorePlayer : IAudioPlayer
    {
        private readonly Pipe _trackPipe;
        private readonly SemaphoreSlim _playSemaphore;
        private readonly CancellationTokenSource _cts;

        private bool _isDisposed;
        private bool _isPlaying;
        private bool _isPaused;
        private int _volume = 100;
        private int _bufferSize = 200;
        private AudioTrack? _currentTrack;
        private TaskCompletionSource<bool>? _pauseTsc;
        private MemoryStream? _currentStream;
        private long? _oldPosition;
        private bool _writeTaskStarted;

        /// <summary>
        /// Creates a new instance of <see cref="FFMpegCorePlayer" />.
        /// </summary>
        /// <param name="options">The options for the player.</param>
        public FFMpegCorePlayer(FFmpegCorePlayerOptions options)
        {
            options.NotNull(nameof(options));
            options.PipeOptions.NotNull(nameof(options.PipeOptions));
            options.OutputCodec.NotNull(nameof(options.OutputCodec));
            ValidateVolumeValue(options.DefaultVolume, nameof(options.DefaultVolume));

            _trackPipe = new Pipe(options.PipeOptions);
            _playSemaphore = new SemaphoreSlim(1, 1);
            _cts = new CancellationTokenSource();

            OutputCodec = options.OutputCodec;
            _volume = options.DefaultVolume;
            _bufferSize = options.DefaultBufferSize;
        }

        /// <summary>
        /// The requested codec output.
        /// </summary>
        public AudioCodec OutputCodec { get; }

        /// <inheritdoc />
        public TimeSpan Position { get; }

        /// <inheritdoc />
        public AudioTrackInfo? CurrentTrack => _currentTrack?.TrackInfo;

        /// <inheritdoc />
        public PipeReader TrackReader => _trackPipe.Reader;

        private PipeWriter TrackWriter => _trackPipe.Writer;

        /// <inheritdoc />
        public bool IsPlaying
        {
            get => Volatile.Read(ref _isPlaying) && !Volatile.Read(ref _isPaused);
            private set => Volatile.Write(ref _isPlaying, value);
        }

        /// <inheritdoc />
        public bool IsPaused
        {
            get => Volatile.Read(ref _isPaused);
            private set => Volatile.Write(ref _isPaused, value);
        }

        // TODO: Seek support.
        /// <inheritdoc />
        public bool SeekSupported => false;

        /// <inheritdoc />
        public bool VolumeSupported => true;

        /// <inheritdoc />
        public async Task PlayAsync(AudioTrack audioTrack, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            audioTrack.NotNull(nameof(audioTrack));
            audioTrack.TrackInfo.NotNull(nameof(audioTrack.TrackInfo));

            // TODO: Audio analyzer.
            audioTrack.Codec.NotNull(nameof(audioTrack.Codec));

            var cts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);

            await _playSemaphore.WaitAsync(cts.Token);

            try
            {
                if (IsPlaying)
                    await PauseAsync(cts.Token);

                if (_currentTrack.IsNotNull())
                    await _currentTrack!.DisposeAsync();

                if (_currentStream.IsNotNull())
                    await _currentStream!.DisposeAsync();

                _currentTrack = audioTrack;
                _currentStream = new MemoryStream();

                await FFMpegArguments
                        .FromPipeInput(new StreamPipeSource(_currentTrack!))
                        .OutputToPipe(new StreamPipeSink(_currentStream!), options =>
                        {
                            var args = new List<string>();

                            var currentFormat = _currentTrack.Codec.Name;
                            var outputFormat = OutputCodec.Name;

                            if (!currentFormat.Equals(outputFormat))
                                args.Add($"-f {outputFormat}");

                            var currentChannels = _currentTrack.Codec.Channels;
                            var outputChannels = OutputCodec.Channels;

                            if (currentChannels != outputChannels)
                                args.Add($"-ac {outputChannels}");

                            var currentSampleRate = _currentTrack.Codec.SampleRate;
                            var outputSampleRate = OutputCodec.SampleRate;

                            if (currentSampleRate != outputSampleRate)
                                args.Add($"-ar {outputSampleRate}");

                            // TODO: see bit depth conversion?

                            var volume = GetVolume();

                            if (volume != 100)
                                args.Add($"-af {volume / 100f}");

                            if (args.Any())
                                options.WithCustomArgument(string.Join(' ', args));
                        })
                        .ProcessAsynchronously();

                if (_oldPosition.HasValue)
                    _currentStream.Position = _oldPosition.Value;
                else
                    _currentStream.Position = 0;

                _oldPosition = null;

                await ResumeAsync(cts.Token);

                if (!_writeTaskStarted)
                    _ = Task.Run(ProcessTrackAsync, cts.Token);
            }
            finally
            {
                if (!_isDisposed)
                    _playSemaphore.Release();
            }
        }

        private Task ProcessTrackAsync()
        {
            if (_writeTaskStarted)
                return Task.CompletedTask;

            _writeTaskStarted = true;

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public ValueTask PauseAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return default;

            if (IsPaused)
                return default;

            _pauseTsc = new TaskCompletionSource<bool>();
            IsPaused = true;

            return default;
        }

        /// <inheritdoc />
        public ValueTask ResumeAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return default;

            if (!IsPaused || _pauseTsc.IsNull())
                return default;

            _pauseTsc!.TrySetResult(true);
            IsPaused = false;

            return default;
        }

        /// <inheritdoc />
        public ValueTask SeekAsync(TimeSpan time, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        /// <inheritdoc />
        public ValueTask SetVolumeAsync(int volume, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return default;

            if (GetVolume() == volume)
                return default;

            Volatile.Write(ref volume, volume);

            if (!_writeTaskStarted || _currentTrack.IsNull() || _currentStream.IsNull())
                return default;

            _oldPosition = _currentStream!.Position;
            var currentTrackCopy = _currentTrack!.Clone();

            var cts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);

            return new ValueTask(PlayAsync(currentTrackCopy, cts.Token));
        }

        /// <inheritdoc />
        public ValueTask StopAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return default;

            return DisposeAsync();
        }

        /// <inheritdoc />
        public int GetBufferSize()
            => Volatile.Read(ref _bufferSize);

        /// <inheritdoc />
        public void SetBufferSize(int bufferSize)
            => Volatile.Write(ref _bufferSize, bufferSize);

        /// <inheritdoc />
        public int GetVolume()
            => Volatile.Read(ref _volume);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateVolumeValue(int volume, string paramName)
        {
            if (volume < 0)
                throw new ArgumentOutOfRangeException(paramName, "The minimum volume value must be greater than or equal to zero.");

            if (volume > 150)
                throw new ArgumentOutOfRangeException(paramName, "The maximum volume value must be less than or equal to 150.");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="FFMpegCorePlayer" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                _cts.Cancel(false);
                _cts.Dispose();

                TrackWriter.Complete();
                TrackReader.Complete();

                _currentTrack?.Dispose();
                _currentStream?.Dispose();

                _playSemaphore.Dispose();
            }

            IsPlaying = false;
            _isDisposed = true;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(false);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed resources asynchronously.
        /// </summary>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous dispose operation.</returns>
        protected async ValueTask DisposeAsyncCore()
        {
            if (_isDisposed)
                return;

            _cts.Cancel(false);
            _cts.Dispose();

            _playSemaphore.Dispose();

            await TrackWriter.CompleteAsync();
            await TrackReader.CompleteAsync();

            if (_currentTrack.IsNotNull())
                await _currentTrack!.DisposeAsync();

            if (_currentStream.IsNotNull())
                await _currentStream!.DisposeAsync();
        }

        /// <inheritdoc />
        ~FFMpegCorePlayer()
        {
            Dispose(false);
        }
    }
}