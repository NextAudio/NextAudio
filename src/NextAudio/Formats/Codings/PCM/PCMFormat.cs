// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Formats.Codings.PCM;

/// <summary>
/// Represents an PCM audio coding format.
/// </summary>
public enum PCMFormat
{
    /// <summary>
    /// Each sample in the data stream is a signed integer.
    /// </summary>
    SignedInteger = 0,

    /// <summary>
    /// Each sample in the data stream is an unsigned integer.
    /// </summary>
    UnsignedInteger = 1,

    /// <summary>
    /// Each sample in the data stream is a fixed-point number.
    /// </summary>
    FixedPoint = 2,

    /// <summary>
    /// Each sample in the data stream is a floating-point number.
    /// </summary>
    FloatingPoint = 3,
}
