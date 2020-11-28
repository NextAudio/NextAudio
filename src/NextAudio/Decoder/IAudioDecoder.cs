using System;
using System.Threading;
using System.Threading.Tasks;

namespace NextAudio
{
    /// <summary>
    /// Represents an audio decoder.
    /// </summary>
    public interface IAudioDecoder : IDisposable
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
        /// Read and decode the <paramref name="inputBuffer" /> and write to the <paramref name="outputBuffer" />.
        /// </summary>
        /// <param name="inputBuffer">A buffer containing the input audio.</param>
        /// <param name="inputOffset">The input offset of the <paramref name="inputBuffer" />.</param>
        /// <param name="inputCount">The input count of the <paramref name="inputBuffer" />.</param>
        /// <param name="outputBuffer">A buffer containing the output audio.</param>
        /// <param name="outputOffset">The output offset of the <paramref name="outputBuffer" />.</param>
        /// <param name="outputCount">The output count of the <paramref name="outputBuffer" />.</param>
        /// <returns>The number of bytes written to the <paramref name="outputBuffer" />.</returns>
        int Decode(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset, int outputCount);

        /// <summary>
        /// Read and decode the <paramref name="inputBuffer" /> and write to the <paramref name="outputBuffer" />.
        /// </summary>
        /// <param name="inputBuffer">A buffer containing the input audio.</param>
        /// <param name="outputBuffer">A buffer containing the output audio.</param>
        /// <returns>The number of bytes written to the <paramref name="outputBuffer" />.</returns>
        int Decode(ReadOnlySpan<byte> inputBuffer, Span<byte> outputBuffer);
    }
}