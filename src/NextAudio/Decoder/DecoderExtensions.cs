using System.IO;
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

        /// <summary>
        /// Creates a new <see cref="DecoderStream" /> from an <see cref="IAudioDecoder" /> and a <see cref="Stream" /> containing input audio.
        /// </summary>
        /// <param name="decoder">The decoder to be used on <see cref="DecoderStream.Read"/>.</param>
        /// <param name="sourceStream">The source <see cref="Stream" /> containing input audio.</param>
        /// <returns>A new instance of <see cref="DecoderStream" />.</returns>
        public static DecoderStream CreateDecoderStream(this IAudioDecoder decoder, Stream sourceStream)
            => new DecoderStream(decoder, sourceStream);

        /// <summary>
        /// Creates a <see cref="DecoderStream" /> from an <see cref="IAudioDecoder" />.
        /// </summary>
        /// <remarks>
        /// This overload does not contains a source stream, for use that you'll need to use <see cref="DecoderStream.Write" />
        /// for write input audio.
        /// </remarks>
        /// <param name="decoder">The decoder to be used on <see cref="DecoderStream.Read" />.</param>
        /// <returns>A new instance of <see cref="DecoderStream" />.</returns>
        public static DecoderStream CreateDecoderStream(this IAudioDecoder decoder)
            => new DecoderStream(decoder);
    }
}