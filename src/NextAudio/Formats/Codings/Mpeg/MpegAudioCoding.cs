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
    /// <param name="layer">The Mpeg layer of this coding.</param>
    /// <param name="sampleRate">The sample rate of the audio stream.</param>
    /// <param name="channels">The number of channels of the audio stream.</param>
    /// <param name="bitRate">The average bitrate, in kilobits per second of the audio stream.</param>
    /// <param name="version">The Mpeg version of this coding.</param>
    public MpegAudioCoding(int layer, int sampleRate, int channels, int? bitRate = null, int? version = null)
        : base(GetName(layer, version), AudioCodingType.Mpeg, sampleRate, channels, 16)
    {
        if (layer is < 1 or > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(layer), $"Invalid MPEG layer number '{layer}'.");
        }

        if (version.HasValue && version.Value is not 1 or 2 or 25)
        {
            throw new ArgumentOutOfRangeException(nameof(layer), $"Invalid MPEG version number '{version.Value}'.");
        }

        Version = version;
        Layer = layer;
        BitRate = bitRate;
    }

    /// <summary>
    /// The Mpeg version of this coding.
    /// </summary>
    public virtual int? Version { get; init; }

    /// <summary>
    /// The Mpeg layer of this coding.
    /// </summary>
    public virtual int Layer { get; init; }

    /// <summary>
    /// The average bitrate, in kilobits per second of the audio stream.
    /// </summary>
    public virtual int? BitRate { get; init; }

    /// <summary>
    /// Returns the name of a Mpeg audio coding based by your <paramref name="version" /> and <paramref name="layer" />.
    /// </summary>
    /// <param name="layer">The Mpeg layer of this coding.</param>
    /// <param name="version">The Mpeg version of this coding.</param>
    /// <returns>The name of a mpeg audio coding based by your <paramref name="version" /> and <paramref name="layer" />.</returns>
    public static string GetName(int layer, int? version = null)
    {
        if (layer is < 1 or > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(layer), $"Invalid MPEG layer number '{layer}'.");
        }

        var versionStr = version.HasValue
            ? version.Value switch
            {
                1 => "1",
                2 => "2",
                25 => "2.5",
                _ => throw new ArgumentOutOfRangeException(nameof(layer), $"Invalid MPEG version '{version}'."),
            }
            : layer switch
            {
                3 => "1, 2, 2.5",
                2 or 1 => "1, 2",
                _ => throw new InvalidOperationException(), // Unhit
            };

        var layerStr = new string('I', layer);

        return $"MPEG Audio {versionStr} Layer {layerStr}";
    }
}
