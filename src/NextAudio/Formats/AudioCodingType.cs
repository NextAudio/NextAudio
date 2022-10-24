// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Formats;

/// <summary>
/// Represents an audio coding type identifier
/// </summary>
public enum AudioCodingType
{
    /// <summary>
    /// The audio coding type is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The audio coding type is PCM.
    /// </summary>
    PCM = 1,

    /// <summary>
    /// The audio coding type is opus.
    /// </summary>
    Opus = 2,

    /// <summary>
    /// The audio coding type is vorbis.
    /// </summary>
    Vorbis = 3,

    /// <summary>
    /// The audio coding type is AAC.
    /// </summary>
    AAC = 4,

    /// <summary>
    /// The audio coding type is Mpeg.
    /// </summary>
    Mpeg = 5,

    /// <summary>
    /// The audio coding type is Flac.
    /// </summary>
    Flac = 6,
}
