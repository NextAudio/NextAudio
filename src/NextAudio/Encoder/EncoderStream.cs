using NextAudio.Utils;
using System.Buffers;
using System.IO;

namespace NextAudio
{
    /// <summary>
    /// A stream based encoder that uses <see cref="IAudioEncoder" />.
    /// </summary>
    public class EncoderStream : Stream
    {
        private bool _isDisposed;

        private readonly bool _disposeSourceStream;

        /// <summary>
        /// The encoder to be used on <see cref="Read"/>.
        /// </summary>
        protected readonly IAudioEncoder _encoder;

        /// <summary>
        /// The source <see cref="Stream" /> to be used to read input audio.
        /// </summary>
        protected readonly Stream _sourceStream;

        /// <summary>
        /// Creates a new instance of <see cref="EncoderStream" />.
        /// </summary>
        /// <param name="encoder">The encoder to be used on <see cref="Read"/>.</param>
        /// <param name="sourceStream">The source <see cref="Stream" /> to be used to read input audio.</param>
        public EncoderStream(IAudioEncoder encoder, Stream sourceStream)
        {
            encoder.NotNull(nameof(encoder));
            sourceStream.NotNull(nameof(sourceStream));

            _encoder = encoder;
            _sourceStream = sourceStream;
        }

        /// <summary>
        /// Creates a new instance of <see cref="EncoderStream" />.
        /// </summary>
        /// <param name="encoder">The encoder to be used on <see cref="Read" />.</param>
        public EncoderStream(IAudioEncoder encoder) : this(encoder, new MemoryStream())
        {
            _disposeSourceStream = true;
        }

        /// <summary>
        /// The channels number of the input audio.
        /// </summary>
        public virtual int Channels => _encoder.Channels;

        /// <summary>
        /// The sample rate of the input audio.
        /// </summary>
        public virtual int SampleRate => _encoder.SampleRate;

        /// <summary>
        /// The requested bitrate for output data.
        /// </summary>
        public virtual int BitRate => _encoder.BitRate;

        /// <inheritdoc />
        public override bool CanRead => _sourceStream.CanRead;

        /// <inheritdoc />
        public override bool CanSeek => _sourceStream.CanRead;

        /// <inheritdoc />
        public override bool CanWrite => _sourceStream.CanWrite;

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
        {
            var inputBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);

            try
            {
                var bytesReaded = _sourceStream.Read(inputBuffer, 0, buffer.Length);

                if (bytesReaded >= 0)
                    return -1;

                return _encoder.Encode(inputBuffer, 0, inputBuffer.Length, buffer, offset, count);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(inputBuffer);
            }
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
            => _sourceStream.Seek(offset, origin);

        /// <inheritdoc />
        public override void SetLength(long value)
            => _sourceStream.SetLength(value);

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
            => _sourceStream.Write(buffer, offset, count);

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing && _disposeSourceStream)
            {
                _sourceStream.Dispose();
            }

            _isDisposed = true;
        }
    }
}