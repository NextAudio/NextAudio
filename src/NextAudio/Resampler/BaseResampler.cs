using System;
using System.Buffers;

namespace NextAudio
{
    /// <summary>
    /// A base class for audio resamplers.
    /// </summary>
    public abstract class BaseResampler : IAudioResampler
    {
        /// <inheritdoc />
        public int InputSampleRate { get; }

        /// <inheritdoc />
        public int OutputSampleRate { get; }

        /// <inheritdoc />
        public abstract int Resample(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset, int outputCount);

        /// <inheritdoc />
        public int Resample(ReadOnlySpan<byte> inputBuffer, Span<byte> outputBuffer)
        {
            var sharedInputBuffer = ArrayPool<byte>.Shared.Rent(inputBuffer.Length);
            var sharedOutputBuffer = ArrayPool<byte>.Shared.Rent(outputBuffer.Length);

            try
            {
                inputBuffer.CopyTo(sharedInputBuffer);

                var read = Resample(sharedInputBuffer, 0, inputBuffer.Length, sharedOutputBuffer, 0, outputBuffer.Length);

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
            Dispose(false);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="BaseResampler" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        public abstract void Dispose(bool disposing);
    }
}