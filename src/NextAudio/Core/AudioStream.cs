using System.IO;

namespace NextAudio
{
    /// <summary>
    /// Represents an audio stream.
    /// </summary>
    public abstract class AudioStream : Stream
    {
        /// <summary>
        /// The codec of the audio stream.
        /// </summary>
        public abstract AudioCodec Codec { get; }
    }
}