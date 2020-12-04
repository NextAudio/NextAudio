using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace NextAudio.FFMpegCore
{
    public class FFMpegCorePlayer : IAudioPlayer
    {
        private readonly Pipe _trackPipe;
        private readonly CancellationTokenSource _cts;

        private bool _isDisposed;
        private bool _isPlaying;
        private bool _isPaused;
        private int _volume = 100;

        public FFMpegCorePlayer()
        {
            _trackPipe = new Pipe();
            _cts = new CancellationTokenSource();
        }

        /// <summary>
        /// The requested codec output.
        /// </summary>
        public AudioCodec CodecOutput { get; }

        /// <inheritdoc />
        public TimeSpan Position { get; }

        /// <inheritdoc />
        public AudioTrackInfo? CurrentTrack { get; }

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

        /// <inheritdoc />
        public bool SeekSupported { get; }

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
        public void SetBufferSize(int bufferSize)
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
            throw new NotImplementedException();
        }

        /// <inheritdoc />

        public int GetBufferSize()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />

        public int GetVolume()
            => Volatile.Read(ref _volume);

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
        }

        /// <inheritdoc />
        ~FFMpegCorePlayer()
        {
            Dispose(false);
        }
    }
}