using NextAudio.Utils;
using System;
using System.IO.Pipelines;
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
        private readonly CancellationTokenSource _cts;

        private bool _isDisposed;
        private bool _isPlaying;
        private bool _isPaused;
        private int _volume = 100;
        private int _bufferSize = 200;
        private AudioTrack? _currentTrack;

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
        public Task PlayAsync(AudioTrackInfo audioTrackInfo, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ValueTask ResumeAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ValueTask PauseAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ValueTask SeekAsync(TimeSpan time, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ValueTask SetVolumeAsync(int volume, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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
                TrackWriter.Complete();
                TrackReader.Complete();
                _currentTrack?.Dispose();
            }

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

            await TrackWriter.CompleteAsync();
            await TrackReader.CompleteAsync();

            if (_currentTrack.IsNotNull())
                await _currentTrack!.DisposeAsync();
        }

        /// <inheritdoc />
        ~FFMpegCorePlayer()
        {
            Dispose(false);
        }
    }
}