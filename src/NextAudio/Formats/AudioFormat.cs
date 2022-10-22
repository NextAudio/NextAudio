// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Formats;

/// <summary>
/// Represents an audio format.
/// </summary>
public record class AudioFormat
{

    /// <summary>
    /// Creates a new instance of <see cref="AudioFormat" />.
    /// </summary>
    /// <param name="coding">The audio coding of this format.</param>
    /// <param name="container">The audio container of this format.</param>
    public AudioFormat(AudioCoding coding, AudioContainer? container = null)
    {
        Coding = coding;
        Container = container;
    }

    /// <summary>
    /// The audio coding of this format.
    /// </summary>
    public AudioCoding Coding { get; init; }

    /// <summary>
    /// The audio container of this format.
    /// </summary>
    public AudioContainer? Container { get; init; }
}
