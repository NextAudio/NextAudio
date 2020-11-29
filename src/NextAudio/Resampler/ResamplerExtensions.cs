using System.IO;
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

        /// <summary>
        /// Creates a new <see cref="ResamplerStream" /> from an <see cref="IAudioResampler" /> and a <see cref="Stream" /> containing input audio.
        /// </summary>
        /// <param name="resampler">The resampler to be used on <see cref="ResamplerStream.Read"/>.</param>
        /// <param name="sourceStream">The source <see cref="Stream" /> containing input audio.</param>
        /// <returns>A new instance of <see cref="ResamplerStream" />.</returns>
        public static ResamplerStream CreateResamplerStream(this IAudioResampler resampler, Stream sourceStream)
            => new ResamplerStream(resampler, sourceStream);

        /// <summary>
        /// Creates a <see cref="ResamplerStream" /> from an <see cref="IAudioResampler" />.
        /// </summary>
        /// <remarks>
        /// This overload does not contains a source stream, for use that you'll need to use <see cref="ResamplerStream.Write" />
        /// for write input audio.
        /// </remarks>
        /// <param name="resampler">The resampler to be used on <see cref="ResamplerStream.Read" />.</param>
        /// <returns>A new instance of <see cref="ResamplerStream" />.</returns>
        public static ResamplerStream CreateResamplerStream(this IAudioResampler resampler)
            => new ResamplerStream(resampler);
    }
}