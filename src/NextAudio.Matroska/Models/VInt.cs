// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Matroska.Models;

/// <summary>
/// Represents an Ebml Variable Signed Integer.
/// /// </summary>
public readonly struct VInt
{
    private static readonly ulong[] BitsMasks =
    {
        (1L << 0) - 1,
        (1L << 7) - 1,
        (1L << 14) - 1,
        (1L << 21) - 1,
        (1L << 28) - 1,
        (1L << 35) - 1,
        (1L << 42) - 1,
        (1L << 49) - 1,
        (1L << 56) - 1
    };

    /// <summary>
    /// Creates a new instance of <see cref="VInt" />.
    /// </summary>
    /// <param name="encodedValue">The encoded value.</param>
    /// <param name="length">The length of this integer.</param>
    public VInt(ulong encodedValue, int length)
    {
        EncodedValue = encodedValue;
        Length = length;
        Value = encodedValue & BitsMasks[Length];
    }

    /// <summary>
    /// The parsed value.
    /// </summary>
    public ulong Value { get; }

    /// <summary>
    /// The encoded value.
    /// </summary>
    public ulong EncodedValue { get; }

    /// <summary>
    /// The length.
    /// </summary>
    public int Length { get; }
}
