namespace NextAudio
{
    /// <summary>
    /// Represents an audio codec.
    /// </summary>
    public class AudioCodec
    {
        /// <summary>
        /// Creates a new instance of <see cref="AudioCodec" />.
        /// </summary>
        /// <param name="fullName">The full name of the codec.</param>
        /// <param name="name">The popular name of the codec (same as the file extension);</param>
        /// <param name="sampleRate">The sample rate of the codec.</param>
        /// <param name="channels">The number of channels of codec.</param>
        /// <param name="bitDepth">The bit depth of the codec.</param>
        public AudioCodec(string fullName, string name, int sampleRate, int channels, int bitDepth)
        {
            FullName = fullName;
            Name = name;
            SampleRate = sampleRate;
            Channels = channels;
            BitDepth = bitDepth;
        }

        /// <summary>
        /// The full name of the codec.
        /// </summary>
        public virtual string FullName { get; }

        /// <summary>
        /// The popular name of the codec (same as the file extension);
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// The sample rate of the audio stream.
        /// </summary>
        public virtual int SampleRate { get; }

        /// <summary>
        /// The number of channels of the audio stream.
        /// </summary>
        public virtual int Channels { get; }

        /// <summary>
        /// The bit depth of the audio stream.
        /// </summary>
        public virtual int BitDepth { get; }
    }
}