// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Formats.Containers.Matroska;

/// <summary>
/// Represents a matroska audio container.
/// </summary>
public record class MatroskaAudioContainer : AudioContainer
{
    /// <summary>
    /// The matroska audio container name.
    /// </summary>
    public const string MatroskaName = "Matroska";

    /// <summary>
    /// Creates a new instance of <see cref="MatroskaAudioContainer" />.
    /// </summary>
    public MatroskaAudioContainer() : base(MatroskaName, AudioContainerType.Matroska)
    {
    }
}
