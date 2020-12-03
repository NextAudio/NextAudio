namespace NextAudio
{
    /// <summary>
    /// Represents a queueable audio player.
    /// </summary>
    public interface IQueueableAudioPlayer : IAudioPlayer
    {
        /// <summary>
        /// A queue of audio tracks.
        /// </summary>
        IAudioQueue<AudioTrackInfo> Queue { get; }
    }
}