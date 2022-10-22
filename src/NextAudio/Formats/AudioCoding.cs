// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Formats;

/// <summary>
/// Represents an audio coding.
/// </summary>
public record class AudioCoding
{
    /// <summary>
    /// An unknown instance of an <see cref="AudioCoding" />.
    /// </summary>
    public static readonly AudioCoding Unknown = new("Unknown", AudioCodingType.Unknown, 0, 0, null);

    /// <summary>
    /// Creates a new instance of <see cref="AudioCoding" />.
    /// </summary>
    /// <param name="name">The full name of the coding format.</param>
    /// <param name="type">The coding type identifier.</param>
    /// <param name="sampleRate">The sample rate of the audio stream.</param>
    /// <param name="channels">The number of channels of the audio stream.</param>
    /// <param name="bitDepth">The bit depth of the audio stream.</param>
    public AudioCoding(string name, AudioCodingType type, int sampleRate, int channels, int? bitDepth = null)
    {
        Name = name;
        Type = type;
        SampleRate = sampleRate;
        Channels = channels;
        BitDepth = bitDepth;
    }

    /// <summary>
    /// The full name of the coding format.
    /// </summary>
    public virtual string Name { get; init; }

    /// <summary>
    /// The coding type identifier.
    /// </summary>
    public virtual AudioCodingType Type { get; init; }

    /// <summary>
    /// The sample rate of the audio stream.
    /// </summary>
    public virtual int SampleRate { get; init; }

    /// <summary>
    /// The number of channels of the audio stream.
    /// </summary>
    public virtual int Channels { get; init; }

    /// <summary>
    /// The bit depth of the audio stream.
    /// </summary>
    public virtual int? BitDepth { get; init; }
}
