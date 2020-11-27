namespace NextAudio
{
    /// <summary>
    /// Options for an <see cref="IAudioDecoder" />
    /// </summary>
    public class DecoderOptions
    {
        /// <summary>
        /// Creates a new instance of <see cref="DecoderOptions"/>.
        /// </summary>
        /// <param name="channels">The channels number of the input audio.</param>
        /// <param name="sampleRate">The sample rate of the input audio.</param>
        public DecoderOptions(int channels, int sampleRate)
        {
            Channels = channels;
            SampleRate = sampleRate;
        }

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