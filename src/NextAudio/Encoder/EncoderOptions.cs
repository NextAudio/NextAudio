namespace NextAudio
{
    /// <summary>
    /// Options for an <see cref="IAudioEncoder" />.
    /// </summary>
    public class EncoderOptions
    {
        /// <summary>
        /// Creates a new instance of <see cref="EncoderOptions" />.
        /// </summary>
        /// <param name="bitRate">The requested bitrate for output data.</param>
        /// <param name="channels">The channels number of the input audio.</param>
        /// <param name="sampleRate">The sample rate of the input audio.</param>
        public EncoderOptions(int bitRate, int channels, int sampleRate)
        {
            BitRate = bitRate;
            Channels = channels;
            SampleRate = sampleRate;
        }

        /// <summary>
        /// The requested bitrate for output data.
        /// </summary>
        public virtual int BitRate { get; }

        /// <summary>
        /// The channels number of the input audio.
        /// </summary>
        public virtual int Channels { get; }

        /// <summary>
        /// The sample rate of the input audio.
        /// </summary>
        public virtual int SampleRate { get; }
    }
}