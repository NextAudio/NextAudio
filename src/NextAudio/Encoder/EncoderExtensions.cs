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
    }
}