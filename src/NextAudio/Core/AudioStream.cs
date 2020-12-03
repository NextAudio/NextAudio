using System.IO;

namespace NextAudio
{
    /// <summary>
    /// Represents an audio stream.
    /// </summary>
    public abstract class AudioStream : Stream
    {
        /// <summary>
        /// The sample rate of the audio stream.
        /// </summary>
        public abstract int SampleRate { get; }

        /// <summary>
        /// The number of channels of the audio stream.
        /// </summary>
        public abstract int Channels { get; }

        /// <summary>
        /// The bit depth of the audio stream.
        /// </summary>
        public abstract int BitDepth { get; }
    }
}