// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using NextAudio.Matroska.Models;
using Xunit;

namespace NextAudio.Matroska.UnitTests;

public class EbmlReaderTests
{
    [Theory]
    [InlineData(new byte[] { 0, 208, 137, 107 }, 13666667)]
    [InlineData(new byte[] { 0, 176, 191, 101 }, 11583333)]
    [InlineData(new byte[] { 0, 192, 164, 104 }, 12625000)]
    [InlineData(new byte[] { 1, 43, 34, 199 }, 19604167)]
    [InlineData(new byte[] { 255, 47, 118, 149 }, -13666667)]
    [InlineData(new byte[] { 255, 79, 64, 155 }, -11583333)]
    [InlineData(new byte[] { 255, 63, 91, 152 }, -12625000)]
    [InlineData(new byte[] { 254, 212, 221, 57 }, -19604167)]
    public void ReadSignedIntegerReadsEbmlSignedInteger(byte[] buffer, long expectedValue)
    {
        // Arrange
        var result = EbmlReader.ReadSignedInteger(buffer);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData(new byte[] { 1, 163 }, 419)]
    [InlineData(new byte[] { 2 }, 2)]
    [InlineData(new byte[] { 15, 66, 64 }, 1000000)]
    [InlineData(new byte[] { 6, 43, 224, 88, 94, 166, 71 }, 1737092415530567)]
    public void ReadUnsignedIntegerReadsEbmlUnsignedInteger(byte[] buffer, ulong expectedValue)
    {
        // Arrange
        var result = EbmlReader.ReadUnsignedInteger(buffer);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData(new byte[] { 71, 59, 128 })] // less than 4
    [InlineData(new byte[] { 71, 59, 128, 167, 234, 242 })] // more than 4 and less than 8
    [InlineData(new byte[] { 71, 59, 128, 167, 234, 242, 12, 23, 13 })] // more than 8
    public void ReadFloatWithBufferSizeDifferentThan4Or8ThrowsArgumentOutOfRangeException(byte[] buffer)
    {
        // Arrange + Assert
        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = EbmlReader.ReadFloat(buffer);
        });
    }

    [Theory]
    [InlineData(new byte[] { 71, 59, 128, 0 }, 48000.0f)] // float
    [InlineData(new byte[] { 199, 59, 128, 0 }, -48000.0f)] // float
    [InlineData(new byte[] { 127, 223, 255, 255, 255, 255, 255, 255 }, 8.988465674311579E+307)] // double
    [InlineData(new byte[] { 255, 223, 255, 255, 255, 255, 255, 255 }, -8.988465674311579E+307)] // double
    public void ReadFloatReadsEbmlFloat(byte[] buffer, double expectedValue)
    {
        // Arrange
        var result = EbmlReader.ReadFloat(buffer);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    public static IEnumerable<object[]> ReadDateReadsEbmlDateData()
    {
        yield return new object[]
        {
            new byte[] { 9, 50, 165, 206, 38, 57, 0, 0 }, new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };
        yield return new object[]
        {
            new byte[] { 9, 96, 255, 111, 219, 210, 0, 0 }, new DateTime(2021, 6, 1, 0, 0, 0, DateTimeKind.Utc),
        };
        yield return new object[]
        {
            new byte[] { 9, 162, 175, 161, 83, 220, 0, 0 }, new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
    }

    [Theory]
    [MemberData(nameof(ReadDateReadsEbmlDateData))]
    public void ReadDateReadsEbmlDate(byte[] buffer, DateTime expectedValue)
    {
        // Arrange
        var result = EbmlReader.ReadDate(buffer);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7 })] // less than 8
    [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 })] // more than 8
    public void ReadDateWithBufferSizeDifferentThan8ThrowsArgumentOutOfRangeException(byte[] buffer)
    {
        // Arrange + Assert
        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = EbmlReader.ReadDate(buffer);
        });
    }

    [Theory]
    [InlineData(new byte[] { 119, 101, 98, 109 }, "webm")]
    [InlineData(new byte[] { 119, 101, 98, 109, 0, 0 }, "webm")] // Check if desconsider additional zeros
    [InlineData(new byte[] { 109, 107, 118, }, "mkv")]
    [InlineData(new byte[] { 101, 110, 103, }, "eng")]
    [InlineData(new byte[] { 112, 111, 114, }, "por")]
    [InlineData(new byte[] { 65, 95, 79, 80, 85, 83 }, "A_OPUS")]
    [InlineData(new byte[] { 65, 95, 77, 80, 69, 71, 47, 76, 51 }, "A_MPEG/L3")]
    public void ReadAsciiStringReadsEbmlAsciiString(byte[] buffer, string expectedValue)
    {
        // Arrange
        var result = EbmlReader.ReadAsciiString(buffer);

        // Assert
        Assert.Equal(expectedValue, result.ToString());
    }

    [Theory]
    [InlineData(new byte[] { 65, 99, 195, 186, 115, 116, 105, 99, 111 }, "Acústico")]
    [InlineData(new byte[] { 87, 104, 101, 110, 32, 73, 39, 109, 32, 87, 105, 116, 104, 32, 89, 111, 117 }, "When I'm With You")]
    [InlineData(new byte[] { 103, 111, 111, 103, 108, 101, 47, 118, 105, 100, 101, 111, 45, 102, 105, 108, 101 }, "google/video-file")]
    [InlineData(new byte[] { 78, 101, 120, 116, 65, 117, 100, 105, 111 }, "NextAudio")]
    [InlineData(new byte[] { 78, 101, 120, 116, 65, 117, 100, 105, 111, 0, 0 }, "NextAudio")] // Check if desconsider additional zeros
    public void ReadUtf8StringReadsEbmlUtf8tring(byte[] buffer, string expectedValue)
    {
        // Arrange
        var result = EbmlReader.ReadUtf8String(buffer);

        // Assert
        Assert.Equal(expectedValue, result.ToString());
    }

    public static IEnumerable<object[]> ReadVariableSizeIntegerReadsEbmlVariableSizeIntegerData()
    {
        yield return new object[] { new byte[] { 26, 69, 223, 163 }, new VInt(0x1A45DFA3, 4) };
        yield return new object[] { new byte[] { 159, 66, 134, 129, 1, 66, 247, 29 }, new VInt(159, 1) };
    }

    [Theory]
    [MemberData(nameof(ReadVariableSizeIntegerReadsEbmlVariableSizeIntegerData))]
    public void ReadVariableSizeIntegerReadsEbmlVariableSizeInteger(byte[] buffer, VInt expectedValue)
    {
        // Arrange
        var result = EbmlReader.ReadVariableSizeInteger(buffer);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData(new byte[] { 0, 69, 223, 163 })] // 0 is the unique byte which gives size higher than 8
    [InlineData(new byte[] { 0, 66, 134, 129, 1, 66, 247, 29 })]
    public void ReadVariableSizeIntegerThrowsInvalidOperationExceptionIfSizeIsHigherThan8(byte[] buffer)
    {
        // Arrange + Assert
        _ = Assert.Throws<InvalidOperationException>(() =>
        {
            _ = EbmlReader.ReadVariableSizeInteger(buffer);
        });
    }
}
