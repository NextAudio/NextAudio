using System;
using System.IO;

namespace NextAudio
{
    /// <summary>
    /// Represents an audio track.
    /// </summary>
    public class AudioTrack : AudioStream
    {
        /// <summary>
        /// The source stream for the audio track.
        /// </summary>
        protected readonly Stream _sourceStream;

        /// <summary>
        /// Creates a new instance of <see cref="AudioTrack" />.
        /// </summary>
        /// <param name="sourceStream">The source stream for the audio track.</param>
        /// <param name="trackInfo">Information about the audio track.</param>
        /// <param name="codec">The codec for the audio track.</param>
        /// <param name="provider">The provider for the audio track.</param>
        public AudioTrack(Stream sourceStream, AudioTrackInfo trackInfo, AudioCodec codec, IAudioProvider? provider = null)
        {
            _sourceStream = sourceStream;
            TrackInfo = trackInfo;
            Codec = codec;
            Provider = provider;
        }

        /// <summary>
        /// Information about the audio track.
        /// </summary>
        public virtual AudioTrackInfo TrackInfo { get; }

        /// <inheritdoc />
        public override AudioCodec Codec { get; }

        /// <summary>
        /// The provider of the audio track if has any.
        /// </summary>
        public virtual IAudioProvider? Provider { get; }

        /// <inheritdoc />
        public override bool CanRead => _sourceStream.CanRead;

        /// <inheritdoc />
        public override bool CanSeek => _sourceStream.CanSeek;

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override long Length => _sourceStream.Length;

        /// <inheritdoc />
        public override long Position
        {
            get => _sourceStream.Position;
            set => _sourceStream.Position = value;
        }

        /// <inheritdoc />
        public override void Flush()
            => _sourceStream.Flush();

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
            => _sourceStream.Read(buffer, offset, count);

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
            => _sourceStream.Seek(offset, origin);

        /// <inheritdoc />
        public override void SetLength(long value)
            => _sourceStream.SetLength(value);

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();
    }
}