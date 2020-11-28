using System.IO.Pipelines;

namespace NextAudio
{
    /// <summary>
    /// Extension methods and utilities for decoders.
    /// </summary>
    public static class DecoderExtensions
    {
        /// <summary>
        /// Creates a <see cref="PipedDecoder" /> from an <see cref="IAudioDecoder" />.
        /// </summary>
        /// <param name="decoder">The decoder to be used for the pipes.</param>
        /// <param name="pipeOptions">The pipe options for the <see cref="PipedDecoder" />.</param>
        /// <returns>A new instance of <see cref="PipedDecoder" />.</returns>
        public static PipedDecoder CreatePipedDecoder(this IAudioDecoder decoder, PipeOptions? pipeOptions = null)
            => new PipedDecoder(decoder, pipeOptions);
    }
}