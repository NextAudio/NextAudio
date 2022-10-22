// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Formats;

/// <summary>
/// Represents an audio container.
/// </summary>
public record class AudioContainer
{
    /// <summary>
    /// An unknown instance of an <see cref="AudioContainer" />.
    /// </summary>
    public static readonly AudioContainer Unknown = new("Unknown", AudioContainerType.Unknown);

    /// <summary>
    /// Creates a new instance of <see cref="AudioContainer" />.
    /// </summary>
    /// <param name="name">The full name of the container.</param>
    /// <param name="type">The container type identifier.</param>
    public AudioContainer(string name, AudioContainerType type)
    {
        Name = name;
        Type = type;
    }

    /// <summary>
    /// The full name of the container.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The container type identifier.
    /// </summary>
    public AudioContainerType Type { get; init; }
}
