// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using NextAudio.Formats;
using NextAudio.Formats.Codings.Mpeg;
using NextAudio.Formats.Codings.Opus;
using NextAudio.Formats.Codings.PCM;
using NextAudio.Matroska.Models;
using NextAudio.Matroska.Utils;
using Xunit;

namespace NextAudio.Matroska.UnitTests.Utils;

public class MatroskaUtilsTests
{
    [Theory]
    [InlineData(MatroskaElementType.Ebml, EbmlValueType.MasterElement)]
    [InlineData(MatroskaElementType.Block, EbmlValueType.Binary)]
    public void GetEbmlValueTypeWithMappedMatroskaElementTypeReturnsCorrespondentEbmlValueType(MatroskaElementType elementType, EbmlValueType expectedValue)
    {
        // Act
        var result = MatroskaUtils.GetEbmlValueType(elementType);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void GetEbmlValueTypeWithUnmappedMatroskaElementTypeReturnsNone()
    {
        // Arrange
        var elementType = MatroskaElementType.Unknown;

        // Act
        var result = MatroskaUtils.GetEbmlValueType(elementType);

        // Assert
        Assert.Equal(EbmlValueType.None, result);
    }

    [Theory]
    [InlineData(0x1A45DFA3)]
    [InlineData(0x18538067)]
    [InlineData(0xA3)]
    public void IsEbmlIdMappedForMatroskaElementTypeReturnsTrueForMappedElements(ulong id)
    {
        // Act
        var result = MatroskaUtils.IsEbmlIdMappedForMatroskaElementType(id);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(0x4D80)]
    [InlineData(0x5741)]
    [InlineData(0x4285)]
    public void IsEbmlIdMappedForMatroskaElementTypeReturnsFalseForUnmappedElements(ulong id)
    {
        // Act
        var result = MatroskaUtils.IsEbmlIdMappedForMatroskaElementType(id);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(0x1A45DFA3, MatroskaElementType.Ebml)]
    [InlineData(0x18538067, MatroskaElementType.Segment)]
    [InlineData(0xA3, MatroskaElementType.SimpleBlock)]
    public void GetMatroskaElementTypeReturnsWithMappedElementTypeReturnsCorrespondentElementType(ulong id, MatroskaElementType expectedValue)
    {
        // Act
        var result = MatroskaUtils.GetMatroskaElementType(id);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData(0x4D80)]
    [InlineData(0x5741)]
    [InlineData(0x4285)]
    public void GetMatroskaElementTypeReturnsWithUnmappedElementTypeReturnsUnknow(ulong id)
    {
        // Act
        var result = MatroskaUtils.GetMatroskaElementType(id);

        // Assert
        Assert.Equal(MatroskaElementType.Unknown, result);
    }

    [Theory]
    [InlineData(0x1A45DFA3, MatroskaElementType.Ebml)]
    [InlineData(0x18538067, MatroskaElementType.Segment)]
    [InlineData(0xA3, MatroskaElementType.SimpleBlock)]
    public void TryGetMatroskaElementTypeWithMappedElementTypeReturnsTrueAndTheCorrespondentElementType(ulong id, MatroskaElementType expectedValue)
    {
        // Act
        var result = MatroskaUtils.TryGetMatroskaElementType(id, out var resultValue);

        // Assert
        Assert.True(result);
        Assert.Equal(expectedValue, resultValue);
    }

    [Theory]
    [InlineData(0x4D80)]
    [InlineData(0x5741)]
    [InlineData(0x4285)]
    public void TryGetMatroskaElementTypeWithUnmappedElementTypeReturnsFalseAndTheUnknow(ulong id)
    {
        // Act
        var result = MatroskaUtils.TryGetMatroskaElementType(id, out var resultValue);

        // Assert
        Assert.False(result);
        Assert.Equal(MatroskaElementType.Unknown, resultValue);
    }

    public static IEnumerable<object[]> GetAudioCodingReturnsNonUnknownAudioCodingIfIsMappedData()
    {
        yield return new object[]
        {
            "A_OPUS",
            new MatroskaAudioSettings
            {
                SamplingFrequency = 48000,
                Channels = 2,
                BitDepth = 16
            },
            new OpusAudioCoding(48000, 2, 16),
        };
        yield return new object[]
        {
            "A_PCM/INT/BIG",
            new MatroskaAudioSettings
            {
                SamplingFrequency = 48000,
                Channels = 2,
                BitDepth = 16
            },
            new PCMAudioCoding(PCMEndianness.BigEndian, PCMFormat.SignedInteger, 48000, 2, 16),
        };
        yield return new object[]
        {
            "A_PCM/INT/LIT",
            new MatroskaAudioSettings
            {
                SamplingFrequency = 48000,
                Channels = 2,
                BitDepth = 16
            },
            new PCMAudioCoding(PCMEndianness.LittleEndian, PCMFormat.SignedInteger, 48000, 2, 16),
        };
        yield return new object[]
        {
            "A_PCM/FLOAT/IEEE",
            new MatroskaAudioSettings
            {
                SamplingFrequency = 48000,
                Channels = 2,
                BitDepth = 16
            },
            new PCMAudioCoding(PCMEndianness.Indeterminate, PCMFormat.FloatingPoint, 48000, 2, 16),
        };
        yield return new object[]
        {
            "A_MPEG/L3",
            new MatroskaAudioSettings
            {
                SamplingFrequency = 48000,
                Channels = 2,
            },
            new MpegAudioCoding(3, 48000, 2),
        };
        yield return new object[]
        {
            "A_MPEG/L2",
            new MatroskaAudioSettings
            {
                SamplingFrequency = 48000,
                Channels = 2,
            },
            new MpegAudioCoding(2, 48000, 2),
        };
        yield return new object[]
        {
            "A_MPEG/L1",
            new MatroskaAudioSettings
            {
                SamplingFrequency = 48000,
                Channels = 2,
                BitDepth = 16
            },
            new MpegAudioCoding(1, 48000, 2),
        };
        yield return new object[]
        {
            "A_FLAC",
            new MatroskaAudioSettings
            {
                SamplingFrequency = 48000,
                Channels = 2,
            },
            new AudioCoding("Flac", AudioCodingType.Flac, 48000, 2),
        };
    }

    [Theory]
    [MemberData(nameof(GetAudioCodingReturnsNonUnknownAudioCodingIfIsMappedData))]
    public void GetAudioCodingReturnsNonUnknownAudioCodingIfIsMapped(string codecID, MatroskaAudioSettings audioSettings, AudioCoding expectedCoding)
    {
        // Act
        var result = MatroskaUtils.GetAudioCoding(codecID, audioSettings);

        // Assert
        Assert.NotEqual(AudioCodingType.Unknown, result.Type);
        Assert.StrictEqual(expectedCoding, result);
    }

    [Fact]
    public void GetAudioCodingReturnsUnknownAudioCodingIfIsNotMapped()
    {
        // Arrange
        var codecID = "A_RANDOM/CODEC";
        var audioSettings = new MatroskaAudioSettings
        {
            SamplingFrequency = 48000,
            Channels = 2,
            BitDepth = 16,
        };

        // Act
        var result = MatroskaUtils.GetAudioCoding(codecID, audioSettings);

        // Assert
        Assert.Equal(AudioCodingType.Unknown, result.Type);
    }

    [Fact]
    public void GetAudioCodingThrowsInvalidOperationExceptionIfPCMHaveNullBitDepth()
    {
        // Arrange
        var codecID = "PCM/INT/LIT";
        var audioSettings = new MatroskaAudioSettings
        {
            SamplingFrequency = 48000,
            Channels = 2,
            BitDepth = null,
        };

        // Act + Assert
        _ = Assert.Throws<InvalidOperationException>(() =>
        {
            _ = MatroskaUtils.GetAudioCoding(codecID, audioSettings);
        });
    }

    [Fact]
    public void GetAudioCodingThrowsInvalidOperationExceptionIfPCMHaveInvalidEndianess()
    {
        // Arrange
        var codecID = "PCM/INT/RANDOM";
        var audioSettings = new MatroskaAudioSettings
        {
            SamplingFrequency = 48000,
            Channels = 2,
            BitDepth = 16,
        };

        // Act + Assert
        _ = Assert.Throws<InvalidOperationException>(() =>
        {
            _ = MatroskaUtils.GetAudioCoding(codecID, audioSettings);
        });
    }
}
