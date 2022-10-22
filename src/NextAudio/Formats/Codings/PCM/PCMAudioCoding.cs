// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Formats.Codings.PCM;

/// <summary>
/// Represents a PCM audio coding.
/// </summary>
public record class PCMAudioCoding : AudioCoding
{
    /// <summary>
    /// The audio coding PCM name.
    /// </summary>
    public const string PCMName = "PCM";

    /// <summary>
    /// Creates a new instance of <see cref="PCMAudioCoding" />.
    /// </summary>
    /// <param name="endianness">The endianness of each sample in the audio stream.</param>
    /// <param name="format">The format of each sample in the audio stream.</param>
    /// <param name="sampleRate">The sample rate of the audio stream.</param>
    /// <param name="channels">The number of channels of the audio stream.</param>
    /// <param name="bitDepth">The bit depth of the audio stream.</param>
    public PCMAudioCoding(PCMEndianness endianness, PCMFormat format, int sampleRate, int channels, int bitDepth)
       : base(PCMName, AudioCodingType.PCM, sampleRate, channels, bitDepth)
    {
        Endiannes = endianness;
        Format = format;

        if ((endianness != PCMEndianness.Indeterminate) && (format == PCMFormat.FloatingPoint || format == PCMFormat.FixedPoint))
        {
            throw new ArgumentException("Fixed or floating point samples cannot have endianness", nameof(endianness));
        }

        if (bitDepth == 8 && endianness != PCMEndianness.Indeterminate)
        {
            throw new ArgumentException("8-bit signed or unsigned samples cannot have endianness", nameof(endianness));
        }
    }

    /// <summary>
    /// The endianness of each sample in the audio stream.
    /// </summary>
    public virtual PCMEndianness Endiannes { get; init; }

    /// <summary>
    /// The format of each sample in the audio stream.
    /// </summary>
    public virtual PCMFormat Format { get; init; }
}
