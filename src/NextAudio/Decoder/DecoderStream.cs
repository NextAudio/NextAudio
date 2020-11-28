using NextAudio.Utils;
using System.Buffers;
using System.IO;

namespace NextAudio
{
    /// <summary>
    /// A stream based decoder that uses <see cref="IAudioDecoder" />.
    /// </summary>
    public class DecoderStream : Stream
    {
        private bool _isDisposed;

        private readonly bool _disposeSourceStream;

        /// <summary>
        /// The decoder to be used on <see cref="Read"/>.
        /// </summary>
        protected readonly IAudioDecoder _decoder;

        /// <summary>
        /// The source <see cref="Stream" /> to be used to read input audio.
        /// </summary>
        protected readonly Stream _sourceStream;

        /// <summary>
        /// Creates a new instance of <see cref="DecoderStream" />.
        /// </summary>
        /// <param name="decoder">The decoder to be used on <see cref="Read"/>.</param>
        /// <param name="sourceStream">The source <see cref="Stream" /> to be used to read input audio.</param>
        public DecoderStream(IAudioDecoder decoder, Stream sourceStream)
        {
            decoder.NotNull(nameof(decoder));
            sourceStream.NotNull(nameof(sourceStream));

            _decoder = decoder;
            _sourceStream = sourceStream;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DecoderStream" />.
        /// </summary>
        /// <param name="decoder">The decoder to be used on <see cref="Read" />.</param>
        public DecoderStream(IAudioDecoder decoder) : this(decoder, new MemoryStream())
        {
            _disposeSourceStream = true;
        }

        /// <summary>
        /// The channels number of the input audio.
        /// </summary>
        public virtual int Channels => _decoder.Channels;

        /// <summary>
        /// The sample rate of the input audio.
        /// </summary>
        public virtual int SampleRate => _decoder.SampleRate;

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

                return _decoder.Decode(inputBuffer, 0, inputBuffer.Length, buffer, offset, count);
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