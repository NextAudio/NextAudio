// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System;
using NextAudio.Formats.Codings.PCM;
using Xunit;

namespace NextAudio.UnitTests.Formats.Codings.PCM;

public class PCMAudioCodingTests
{
    [Theory]
    [InlineData(PCMEndianness.BigEndian, PCMFormat.FloatingPoint)]
    [InlineData(PCMEndianness.BigEndian, PCMFormat.FixedPoint)]
    [InlineData(PCMEndianness.LittleEndian, PCMFormat.FloatingPoint)]
    [InlineData(PCMEndianness.LittleEndian, PCMFormat.FixedPoint)]
    public void CtorThrowsArgumentExceptionIfEndianessIsDifferentThanIndeterminateAndFormatIsEqualsToFloatingOrFixed(PCMEndianness endianness, PCMFormat format)
    {
        // Act + Assert
        _ = Assert.Throws<ArgumentException>(() =>
        {
            _ = new PCMAudioCoding(endianness, format, 48000, 2, 16);
        });
    }

    [Theory]
    [InlineData(PCMEndianness.BigEndian)]
    [InlineData(PCMEndianness.LittleEndian)]
    public void CtorThrowsArgumentExceptionIfBitDepthIs8AndHaveEndianness(PCMEndianness endianness)
    {
        // Act + Assert
        _ = Assert.Throws<ArgumentException>(() =>
        {
            _ = new PCMAudioCoding(endianness, PCMFormat.SignedInteger, 48000, 2, 8);
        });
    }
}
