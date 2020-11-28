using System.IO.Pipelines;

namespace NextAudio.Resampler
{
    /// <summary>
    /// Extension methods and utilities for resamplers.
    /// </summary>
    public static class ResamplerExtensions
    {
        /// <summary>
        /// Creates a <see cref="PipedResampler" /> from an <see cref="IAudioResampler" />.
        /// </summary>
        /// <param name="resampler">The resampler to be used for the pipes.</param>
        /// <param name="pipeOptions">The pipe options for the <see cref="PipedResampler" />.</param>
        /// <returns>A new instance of <see cref="PipedResampler" />.</returns>
        public static PipedResampler CreatePipedResampler(this IAudioResampler resampler, PipeOptions? pipeOptions = null)
            => new PipedResampler(resampler, pipeOptions);
    }
}