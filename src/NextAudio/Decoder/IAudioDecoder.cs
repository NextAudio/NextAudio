using System;
using System.Threading;
using System.Threading.Tasks;

namespace NextAudio
{
    /// <summary>
    /// Represents an audio decoder.
    /// </summary>
    public interface IAudioDecoder : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// The channels number of the input audio.
        /// </summary>
        int Channels { get; }

        /// <summary>
        /// The sample rate of the input audio.
        /// </summary>
        int SampleRate { get; }

        /// <summary>
        /// Read and decode the input buffer and write to the output buffer.
        /// </summary>
        /// <param name="inputBuffer">A buffer containing the input audio.</param>
        /// <param name="inputOffset">The input offset in the input buffer.</param>
        /// <param name="inputCount">The input count of the input buffer.</param>
        /// <param name="outputBuffer">A buffer containing the output audio.</param>
        /// <param name="outputOffset">The output offset of the output buffer.</param>
        /// <param name="outputCount">The output count of the output buffer.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>The number of bytes read.</returns>
        Task<int> DecodeAsync(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset, int outputCount, CancellationToken cancellationToken = default);

        /// <summary>
        /// Read and decode the input buffer and write to the output buffer.
        /// </summary>
        /// <param name="inputBuffer">A buffer containing the input audio.</param>
        /// <param name="outputBuffer">A buffer containing the output audio.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>The number of bytes read.</returns>
        ValueTask<int> DecodeAsync(ReadOnlyMemory<byte> inputBuffer, Memory<byte> outputBuffer, CancellationToken cancellationToken = default);
    }
}