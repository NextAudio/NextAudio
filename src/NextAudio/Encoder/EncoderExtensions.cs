using System.IO;
using System.IO.Pipelines;

namespace NextAudio
{
    /// <summary>
    /// Extension methods and utilities for encoders.
    /// </summary>
    public static class EncoderExtensions
    {
        /// <summary>
        /// Creates a <see cref="PipedEncoder" /> from an <see cref="IAudioEncoder" />.
        /// </summary>
        /// <param name="encoder">The encoder to be used for the pipes.</param>
        /// <param name="pipeOptions">The pipe options for the <see cref="PipedEncoder" />.</param>
        /// <returns>A new instance of <see cref="PipedEncoder" />.</returns>
        public static PipedEncoder CreatePipedEncoder(this IAudioEncoder encoder, PipeOptions? pipeOptions = null)
            => new PipedEncoder(encoder, pipeOptions);

        /// <summary>
        /// Creates a new <see cref="EncoderStream" /> from an <see cref="IAudioEncoder" /> and a <see cref="Stream" /> containing input audio.
        /// </summary>
        /// <param name="encoder">The encoder to be used on <see cref="EncoderStream.Read"/>.</param>
        /// <param name="sourceStream">The source <see cref="Stream" /> containing input audio.</param>
        /// <returns>A new instance of <see cref="EncoderStream" />.</returns>
        public static EncoderStream CreateEncoderStream(this IAudioEncoder encoder, Stream sourceStream)
            => new EncoderStream(encoder, sourceStream);

        /// <summary>
        /// Creates a <see cref="EncoderStream" /> from an <see cref="IAudioEncoder" />.
        /// </summary>
        /// <remarks>
        /// This overload does not contains a source stream, for use that you'll need to use <see cref="EncoderStream.Write" />
        /// for write input audio.
        /// </remarks>
        /// <param name="encoder">The encoder to be used on <see cref="EncoderStream.Read" />.</param>
        /// <returns>A new instance of <see cref="EncoderStream" />.</returns>
        public static EncoderStream CreateEncoderStream(this IAudioEncoder encoder)
            => new EncoderStream(encoder);
    }
}