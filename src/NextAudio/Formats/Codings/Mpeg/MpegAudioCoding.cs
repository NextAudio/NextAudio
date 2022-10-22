// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Formats.Codings.Mpeg;

/// <summary>
/// Represents an mpeg audio coding.
/// </summary>
public record class MpegAudioCoding : AudioCoding
{
    /// <summary>
    /// Creates a new instance of <see cref="MpegAudioCoding" />.
    /// </summary>
    /// <param name="version">The MPEG version of this coding.</param>
    /// <param name="layer">The MPEG layer of this coding.</param>
    /// <param name="bitRate">The average bitrate, in kilobits per second of the audio stream.</param>
    /// <param name="sampleRate">The sample rate of the audio stream.</param>
    /// <param name="channels">The number of channels of the audio stream.</param>
    public MpegAudioCoding(int version, int layer, int bitRate, int sampleRate, int channels)
        : base(GetName(version, layer), AudioCodingType.Mpeg, sampleRate, channels, 16)
    {
        Version = version;
        Layer = layer;
        BitRate = bitRate;
    }

    /// <summary>
    /// The MPEG version of this coding.
    /// </summary>
    public virtual int Version { get; init; }

    /// <summary>
    /// The MPEG layer of this coding.
    /// </summary>
    public virtual int Layer { get; init; }

    /// <summary>
    /// The average bitrate, in kilobits per second of the audio stream.
    /// </summary>
    public virtual int BitRate { get; init; }

    /// <summary>
    /// Returns the name of a mpeg audio coding based by your <paramref name="version" /> and <paramref name="layer" />.
    /// </summary>
    /// <param name="version">The MPEG version of this coding.</param>
    /// <param name="layer">The MPEG layer of this coding.</param>
    /// <returns>The name of a mpeg audio coding based by your <paramref name="version" /> and <paramref name="layer" />.</returns>
    public static string GetName(int version, int layer)
    {
        return $"MPEG-{version} Audio Layer {layer}";
    }
}
