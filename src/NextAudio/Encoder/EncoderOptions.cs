namespace NextAudio
{
    /// <summary>
    /// Options for an <see cref="IAudioEncoder" />.
    /// </summary>
    public class EncoderOptions
    {
        /// <summary>
        /// The requested bitrate for output data.
        /// </summary>
        public int BitRate { get; }

        /// <summary>
        /// The channels number of the input audio.
        /// </summary>
        public int Channels { get; }

        /// <summary>
        /// The sample rate of the input audio.
        /// </summary>
        public int SampleRate { get; }
    }
}