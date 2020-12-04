using System.IO.Pipelines;

namespace NextAudio.FFMpegCore
{
    /// <summary>
    /// Options for <see cref="FFMpegCorePlayer" />.
    /// </summary>
    public class FFmpegCorePlayerOptions
    {
        // Yes this container is not correct.
        private static readonly AudioCodec DefaultOutputCodec = new AudioCodec("PCM", "s16le", "WAV", 48000, 2, 16);

        /// <summary>
        /// The requested output codec.
        /// </summary>
        public AudioCodec OutputCodec { get; set; } = DefaultOutputCodec;

        /// <summary>
        /// Options for the pipes audio data.
        /// </summary>
        public PipeOptions PipeOptions { get; set; } = PipeOptions.Default;

        /// <summary>
        /// The default buffer size for read/write.
        /// </summary>
        public int DefaultBufferSize { get; set; } = 200;

        /// <summary>
        /// The default volume for the player.
        /// </summary>
        public int DefaultVolume { get; set; } = 100;
    }
}