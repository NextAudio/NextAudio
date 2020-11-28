namespace NextAudio
{
    /// <summary>
    /// Options for an <see cref="IAudioResampler" />.
    /// </summary>
    public class ResamplerOptions
    {
        /// <summary>
        /// Creates a new instance of <see cref="ResamplerOptions" />.
        /// </summary>
        /// <param name="inputSampleRate">The input sample rate.</param>
        /// <param name="outputSampleRate">The output sample rate.</param>
        public ResamplerOptions(int inputSampleRate, int outputSampleRate)
        {
            InputSampleRate = inputSampleRate;
            OutputSampleRate = outputSampleRate;
        }

        /// <summary>
        /// The input sample rate.
        /// </summary>
        public virtual int InputSampleRate { get; }

        /// <summary>
        /// The output sample rate.
        /// </summary>
        public virtual int OutputSampleRate { get; }
    }
}