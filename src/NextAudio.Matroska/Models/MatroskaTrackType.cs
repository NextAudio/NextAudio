// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Matroska.Models;

/// <summary>
/// Represents a Matroska track type.
/// </summary>
public enum MatroskaTrackType : ulong
{
    /// <summary>
    /// The Matroska track type is unknown (shouldn't appear).
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The Matroska track type is a video.
    /// </summary>
    Video = 1,

    /// <summary>
    /// The Matroska track type is an audio.
    /// </summary>
    Audio = 2,

    /// <summary>
    /// The Matroska track type is a complex.
    /// </summary>
    Complex = 3,

    /// <summary>
    /// The Matroska track type is a logo.
    /// </summary>
    Logo = 16,

    /// <summary>
    /// The Matroska track type is a subtitle.
    /// </summary>
    Subtitle = 17,

    /// <summary>
    /// The Matroska track type is buttons.
    /// </summary>
    Buttons = 18,

    /// <summary>
    /// The Matroska track type is a control.
    /// </summary>
    Control = 32,

    /// <summary>
    /// The Matroska track type is a metadata.
    /// </summary>
    Metadata = 33,
}
