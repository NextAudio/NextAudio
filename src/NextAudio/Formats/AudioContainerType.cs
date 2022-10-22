// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Formats;

/// <summary>
/// Represents an audio container type identifier
/// </summary>
public enum AudioContainerType
{
    /// <summary>
    /// The audio container type is unknown,
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The audio container type is Matroska (.webm/.mkv),
    /// </summary>
    Matroska = 1,
}
