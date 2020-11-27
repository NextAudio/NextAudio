using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NextAudio
{
    /// <summary>
    /// A base class for audio decoders.
    /// </summary>
    public abstract class BaseDecoder : IAudioDecoder
    {
        /// <summary>
        /// Creates a new instance of <see cref="BaseDecoder" />.
        /// </summary>
        /// <param name="options">The options for this decoder.</param>
        protected BaseDecoder(DecoderOptions options)
        {
            Channels = options.Channels;
            SampleRate = options.SampleRate;
        }

        /// <inheritdoc />
        public virtual int Channels { get; }

        /// <inheritdoc />
        public virtual int SampleRate { get; }

        /// <inheritdoc />
        public abstract Task<int> DecodeAsync(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset, int outputCount, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public virtual ValueTask<int> DecodeAsync(ReadOnlyMemory<byte> inputBuffer, Memory<byte> outputBuffer, CancellationToken cancellationToken = default)
        {
            if (MemoryMarshal.TryGetArray(inputBuffer, out ArraySegment<byte> inputArray) && MemoryMarshal.TryGetArray(outputBuffer, out ArraySegment<byte> outputArray))
                return new ValueTask<int>(DecodeAsync(inputArray.Array!, inputArray.Offset, inputArray.Count, outputArray.Array!, outputArray.Offset, outputArray.Count, cancellationToken));

            var sharedInputBuffer = ArrayPool<byte>.Shared.Rent(inputBuffer.Length);

            inputBuffer.Span.CopyTo(sharedInputBuffer);

            var sharedOutputBuffer = ArrayPool<byte>.Shared.Rent(outputBuffer.Length);

            return FinishDecodeAsync(DecodeAsync(sharedInputBuffer, 0, inputBuffer.Length, sharedOutputBuffer, 0, outputBuffer.Length, cancellationToken), sharedInputBuffer, sharedOutputBuffer, outputBuffer);
        }

        private static async ValueTask<int> FinishDecodeAsync(Task<int> decodeTask, byte[] localInputBuffer, byte[] localOutputBuffer, Memory<byte> localDestination)
        {
            try
            {
                var result = await decodeTask.ConfigureAwait(false);

                new ReadOnlySpan<byte>(localOutputBuffer, 0, result).CopyTo(localDestination.Span);

                return result;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(localInputBuffer);
                ArrayPool<byte>.Shared.Return(localOutputBuffer);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="BaseDecoder" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed resources asynchronously.
        /// </summary>
        /// <returns> A <see cref="ValueTask" /> that represents the asynchronous dispose operation.</returns>
        protected abstract ValueTask DisposeAsyncCore();

        /// <inheritdoc />
        ~BaseDecoder()
        {
            Dispose(false);
        }
    }
}