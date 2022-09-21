// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Text;

namespace NextAudio.Matroska;

/// <summary>
/// A reader for the Ebml notation.
/// </summary>
public static class EbmlReader
{
    internal static readonly DateTime MilleniumStart = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

#pragma warning disable CS0675
    /// <summary>
    /// Read an Ebml signed integer from the <paramref name="buffer" />.
    /// </summary>
    /// <param name="buffer">The input buffer to read the Ebml signed integer.</param>
    /// <returns>The parsed Ebml signed integer.</returns>
    public static long ReadSignedInteger(ReadOnlySpan<byte> buffer)
    {
        long result = (sbyte)buffer[0]; // The signal (positive/negative using sign extension)

        for (var i = 1; i < buffer.Length; i++)
        {
            result = (result << 8) | (buffer[i] & 0xff);
        }

        return result;
    }
#pragma warning restore CS0675

    /// <summary>
    /// Read an Ebml unsigned integer from the <paramref name="buffer" />.
    /// </summary>
    /// <param name="buffer">The input buffer to read the Ebml unsigned integer.</param>
    /// <returns>The parsed Ebml ynsigned integer.</returns>
    public static ulong ReadUnsignedInteger(ReadOnlySpan<byte> buffer)
    {
        ulong result = 0;
        for (var i = 0; i < buffer.Length; i++)
        {
            result = (result << 8) | buffer[i];
        }
        return result;
    }

    /// <summary>
    /// Read an Ebml float from the <paramref name="buffer" />.
    /// </summary>
    /// <param name="buffer">The input buffer to read the Ebml float.</param>
    /// <returns>The parsed Ebml float.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="buffer" /> length is different than 4 or 8.</exception>
    public static double ReadFloat(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length is not 4 and not 8)
        {
            throw new ArgumentOutOfRangeException(nameof(buffer), $"{nameof(buffer)}.Length is different than 4 or 8.");
        }

        var union = new FloatUnion
        {
            UnsignedIntegerValue = ReadUnsignedInteger(buffer),
        };

        return buffer.Length == 4
            ? union.FloatValue
            : union.DoubleValue;
    }

    // WOOWWWW how this works??
    // This is an union structure the same as the "C lang union structure":
    // https://www.tutorialspoint.com/cprogramming/c_unions.htm
    // Basically all fields have the same position in the memory,
    // That implicts cast these value types (float/double), because
    // after all, the value is the same just your value type representation
    // is different here.
    [StructLayout(LayoutKind.Explicit)]
    private ref struct FloatUnion
    {
        [FieldOffset(0)]
        public ulong UnsignedIntegerValue;

        [FieldOffset(0)]
        public readonly float FloatValue;

        [FieldOffset(0)]
        public readonly double DoubleValue;
    }

    /// <summary>
    /// Read an Ebml date from the <paramref name="buffer" />.
    /// </summary>
    /// <param name="buffer">The input buffer to read the Ebml date.</param>
    /// <returns>The parsed Ebml date.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="buffer" /> length is different than 8.</exception>
    public static DateTime ReadDate(ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length != 8)
        {
            throw new ArgumentOutOfRangeException(nameof(buffer), $"{nameof(buffer)}.Length is different than 8.");
        }

        var number = ReadSignedInteger(buffer);

        return MilleniumStart.AddTicks(number / 100);
    }

    /// <summary>
    /// Read an Ebml ascii string from the <paramref name="buffer" />.
    /// </summary>
    /// <param name="buffer">The input buffer to read the Ebml ascii string.</param>
    /// <returns>The parsed Ebml ascii string.</returns>
    public static ReadOnlySpan<char> ReadAsciiString(ReadOnlySpan<byte> buffer)
    {
        return ReadString(buffer, Encoding.ASCII);
    }

    /// <summary>
    /// Read an Ebml utf8 string from the <paramref name="buffer" />.
    /// </summary>
    /// <param name="buffer">The input buffer to read the Ebml utf8 string.</param>
    /// <returns>The parsed Ebml utf8 string.</returns>
    public static ReadOnlySpan<char> ReadUtf8String(ReadOnlySpan<byte> buffer)
    {
        return ReadString(buffer, Encoding.UTF8);
    }

    private static ReadOnlySpan<char> ReadString(ReadOnlySpan<byte> inputBuffer, Encoding encoding)
    {
        var zeros = 0;

        for (var i = inputBuffer.Length - 1; i >= 0; i--)
        {
            if (inputBuffer[i] != 0)
            {
                break;
            }

            zeros++;
        }

        var buffer = inputBuffer[..^zeros];

        var charCount = encoding.GetCharCount(buffer);

        Span<char> outputBuffer = new char[charCount];

        var bytesWritten = encoding.GetChars(buffer, outputBuffer[..charCount]);

        return outputBuffer[..bytesWritten];
    }
}
