// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Formats.Codings.Opus;

/// <summary>
/// Represents an opus audio coding.
/// </summary>
public record class OpusAudioCoding : AudioCoding
{
    /// <summary>
    /// The opus audio coding name.
    /// </summary>
    public const string OpusName = "Opus";

    /// <summary>
    /// Creates a new instance of <see cref="OpusAudioCoding" />.
    /// </summary>
    /// <param name="sampleRate">The sample rate of the audio stream.</param>
    /// <param name="channels">The number of channels of the audio stream.</param>
    /// <param name="bitDepth">The bit depth of the audio stream.</param>
    public OpusAudioCoding(int sampleRate, int channels, int? bitDepth = null)
        : base(OpusName, AudioCodingType.Opus, sampleRate, channels, bitDepth)
    {

    }
}
