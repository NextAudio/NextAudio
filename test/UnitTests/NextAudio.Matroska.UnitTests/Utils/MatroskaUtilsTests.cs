// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

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
}
