// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System;
using NextAudio.Formats.Codings.Mpeg;
using Xunit;

namespace NextAudio.UnitTests.Formats.Codings.Mpeg;

public class MpegAudioCodingTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public void CtorThrowsArgumentOutOfRangeExceptionIfLayerIsLowerThan1OrHigherThan3(int layer)
    {
        // Act + Assert
        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = new MpegAudioCoding(layer, 4800, 2);
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public void CtorThrowsArgumentOutOfRangeExceptionIfVersionIsDifferentThan1Or2Or25(int version)
    {
        // Act + Assert
        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = new MpegAudioCoding(1, 4800, 2, 192, version);
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public void GetNameThrowsArgumentOutOfRangeExceptionIfLayerIsLowerThan1OrHigherThan3(int layer)
    {
        // Act + Assert
        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = MpegAudioCoding.GetName(layer, 2);
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public void GetNameThrowsArgumentOutOfRangeExceptionIfVersionIsDifferentThan1Or2Or25(int version)
    {
        // Act + Assert
        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = MpegAudioCoding.GetName(1, version);
        });
    }
}
