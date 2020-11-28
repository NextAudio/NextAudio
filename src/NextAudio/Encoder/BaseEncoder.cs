using NextAudio.Utils;
using System;
using System.Buffers;

namespace NextAudio
{
    /// <summary>
    /// Base class for audio encoders.
    /// </summary>
    public abstract class BaseEncoder : IAudioEncoder
    {
        /// <summary>
        /// Creates a new instance of <see cref="BaseEncoder" />.
        /// </summary>
        /// <param name="options">The options for this encoder.</param>
        protected BaseEncoder(EncoderOptions options)
        {
            options.NotNull(nameof(options));

            Channels = options.Channels;
            SampleRate = options.SampleRate;
            BitRate = options.BitRate;
        }

        /// <inheritdoc />
        public virtual int Channels { get; }

        /// <inheritdoc />
        public virtual int SampleRate { get; }

        /// <inheritdoc />
        public virtual int BitRate { get; }

        /// <inheritdoc />
        public abstract int Encode(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset, int outputCount);

        /// <inheritdoc />
        public virtual int Encode(ReadOnlySpan<byte> inputBuffer, Span<byte> outputBuffer)
        {
            var sharedInputBuffer = ArrayPool<byte>.Shared.Rent(inputBuffer.Length);
            var sharedOutputBuffer = ArrayPool<byte>.Shared.Rent(outputBuffer.Length);

            try
            {
                inputBuffer.CopyTo(sharedInputBuffer);

                var read = Encode(sharedInputBuffer, 0, inputBuffer.Length, sharedOutputBuffer, 0, outputBuffer.Length);

                new ReadOnlySpan<byte>(sharedOutputBuffer, 0, read).CopyTo(outputBuffer);

                return read;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedInputBuffer);
                ArrayPool<byte>.Shared.Return(sharedOutputBuffer);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="BaseEncoder" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected abstract void Dispose(bool disposing);

        /// <inheritdoc />
        ~BaseEncoder()
        {
            Dispose(false);
        }
    }
}