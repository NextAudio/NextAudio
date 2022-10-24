// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Formats.Codings.PCM;

/// <summary>
/// Represents an PCM audio coding endiannes.
/// </summary>
public enum PCMEndianness
{
    /// <summary>
    /// Each sample in the audio stream is neither big endian nor little
    /// endian.
    /// </summary>
    Indeterminate = 0,

    /// <summary>
    /// Each sample in the audio stream uses big-endian byte order.
    /// </summary>
    BigEndian = 1,

    /// <summary>
    /// Each sample in the audio stream uses little-endian byte order.
    /// </summary>
    LittleEndian = 2,
}
