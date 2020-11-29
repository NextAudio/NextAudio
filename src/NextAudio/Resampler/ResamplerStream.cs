using NextAudio.Utils;
using System.Buffers;
using System.IO;

namespace NextAudio
{
    /// <summary>
    /// A stream based resampler that uses <see cref="IAudioResampler" />.
    /// </summary>
    public class ResamplerStream : Stream
    {
        private bool _isDisposed;

        private readonly bool _disposeSourceStream;

        /// <summary>
        /// The resampler to be used on <see cref="Read"/>.
        /// </summary>
        protected readonly IAudioResampler _resampler;

        /// <summary>
        /// The source <see cref="Stream" /> to be used to read input audio.
        /// </summary>
        protected readonly Stream _sourceStream;

        /// <summary>
        /// Creates a new instance of <see cref="ResamplerStream" />.
        /// </summary>
        /// <param name="resampler">The resampler to be used on <see cref="Read"/>.</param>
        /// <param name="sourceStream">The source <see cref="Stream" /> to be used to read input audio.</param>
        public ResamplerStream(IAudioResampler resampler, Stream sourceStream)
        {
            resampler.NotNull(nameof(resampler));
            sourceStream.NotNull(nameof(sourceStream));

            _resampler = resampler;
            _sourceStream = sourceStream;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ResamplerStream" />.
        /// </summary>
        /// <param name="resampler">The resampler to be used on <see cref="Read" />.</param>
        public ResamplerStream(IAudioResampler resampler) : this(resampler, new MemoryStream())
        {
            _disposeSourceStream = true;
        }

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

                return _resampler.Resample(inputBuffer, 0, inputBuffer.Length, buffer, offset, count);
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

        /// <inheritdoc />
        ~ResamplerStream()
        {
            Dispose(false);
        }
    }
}