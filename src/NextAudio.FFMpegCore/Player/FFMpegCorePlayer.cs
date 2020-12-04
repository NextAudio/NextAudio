using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace NextAudio.FFMpegCore
{
    public class FFMpegCorePlayer : IAudioPlayer
    {
        public AudioCodec CodecOutput { get; }

        public TimeSpan Position { get; }

        public AudioTrackInfo? CurrentTrack { get; }

        public PipeReader TrackReader { get; }

        public bool IsPlaying { get; }

        public bool IsPaused { get; }

        public bool SeekSupported { get; }

        public bool VolumeSupported { get; }

        public ValueTask PauseAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task PlayAsync(AudioTrackInfo audioTrackInfo, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask ResumeAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask SeekAsync(TimeSpan time, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void SetBufferSize(int bufferSize)
        {
            throw new NotImplementedException();
        }

        public ValueTask SetVolumeAsync(int volume, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask StopAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public int GetBufferSize()
        {
            throw new NotImplementedException();
        }

        public int GetVolume()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}