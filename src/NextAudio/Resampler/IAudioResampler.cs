using System;

namespace NextAudio
{
    /// <summary>
    /// Represents an audio resampler.
    /// </summary>
    public interface IAudioResampler
    {
        /// <summary>
        /// The input sample rate.
        /// </summary>
        int InputSampleRate { get; }

        /// <summary>
        /// The output sample rate.
        /// </summary>
        int OutputSampleRate { get; }

        /// <summary>
        /// Read and resample the <paramref name="inputBuffer" /> and write to the <paramref name="outputBuffer" />.
        /// </summary>
        /// <param name="inputBuffer">A buffer containing the input audio.</param>
        /// <param name="inputOffset">The input offset of the <paramref name="inputBuffer" />.</param>
        /// <param name="inputCount">The input count of the <paramref name="inputBuffer" />.</param>
        /// <param name="outputBuffer">A buffer containing the output audio.</param>
        /// <param name="outputOffset">The output offset of the <paramref name="outputBuffer" />.</param>
        /// <param name="outputCount">The output count of the <paramref name="outputBuffer" />.</param>
        /// <returns>The number of bytes written to the <paramref name="outputBuffer" />.</returns>
        int Resample(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset, int outputCount);

        /// <summary>
        /// Read and resample the <paramref name="inputBuffer" /> and write to the <paramref name="outputBuffer" />.
        /// </summary>
        /// <param name="inputBuffer">A buffer containing the input audio.</param>
        /// <param name="outputBuffer">A buffer containing the output audio.</param>
        /// <returns>The number of bytes written to the <paramref name="outputBuffer" />.</returns>
        int Resample(ReadOnlySpan<byte> inputBuffer, Span<byte> outputBuffer);
    }
}