// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging.Abstractions;
using NextAudio.Matroska.Internal;
using NextAudio.Matroska.Models;
using Xunit;

namespace NextAudio.Matroska.UnitTests.Internal;

public class ElementReaderTests
{
    public static IEnumerable<object[]> ReadNextElementWithParentWithoutRemainingParentReturnsNullData()
    {
        yield return new object[]
        {
            (AudioStream)new MemoryStream(),
            10,
            Array.Empty<byte>(),
            new MatroskaElement(0, 0, 10, 0, 0),
        };
        yield return new object[]
        {
            (AudioStream)new MemoryStream(),
            100,
            Array.Empty<byte>(),
            new MatroskaElement(0, 0, 100, 0, 0),
        };
    }

    public static IEnumerable<object[]> ReadNextElementWithoutParentReadsNextElementData()
    {
        yield return new object[]
        {
            (AudioStream)new MemoryStream(new byte[] { 26, 69, 223, 163, 147 }),
            0,
            new byte[5],
            new MatroskaElement(440786851, 0, 0, 5, 19)
        };
        yield return new object[]
        {
            (AudioStream)new MemoryStream(new byte[] { 24, 83, 128, 103, 17, 100, 33, 41 }),
            24,
            new byte[8],
            new MatroskaElement(408125543, 0, 24, 8, 23339305)
        };
    }

    public static IEnumerable<object[]> ReadNextElementWithParentReadsNextElementData()
    {
        yield return new object[]
        {
            (AudioStream)new MemoryStream(new byte[] { 21, 73, 169, 102, 64, 205 }),
            96,
            new byte[6],
            new MatroskaElement(408125543, 0, 24, 8, 23339305),
            new MatroskaElement(357149030, 1, 96, 6, 205),
        };
        yield return new object[]
        {
            (AudioStream)new MemoryStream(new byte[] { 22, 84, 174, 107, 64, 159, }),
            307,
            new byte[6],
            new MatroskaElement(408125543, 0, 24, 8, 23339305),
            new MatroskaElement(374648427, 1, 307, 6, 159),
        };
    }

    [Theory]
    [MemberData(nameof(ReadNextElementWithParentWithoutRemainingParentReturnsNullData))]
    public void ReadNextElementWithParentWithoutRemainingParentReturnsNull(AudioStream stream, long position, byte[] buffer, MatroskaElement parent)
    {
        // Act
        var result = ElementReader.ReadNextElement(stream, position, buffer, NullLogger.Instance, parent);

        // Assert
        Assert.False(result.Element.HasValue);
        Assert.Equal(position, result.NewPosition);
    }

    [Theory]
    [MemberData(nameof(ReadNextElementWithoutParentReadsNextElementData))]
    public void ReadNextElementWithoutParentReadsNextElement(AudioStream stream, long position, byte[] buffer, MatroskaElement expectedElement)
    {
        // Arrange
        var expectedPosition = position + expectedElement.HeaderSize;

        // Act
        var result = ElementReader.ReadNextElement(stream, position, buffer, NullLogger.Instance);

        // Assert
        Assert.True(result.Element.HasValue);
        Assert.Equal(expectedElement, result.Element.Value);
        Assert.Equal(result.NewPosition, expectedPosition);
    }

    [Theory]
    [MemberData(nameof(ReadNextElementWithParentReadsNextElementData))]
    public void ReadNextElementWithParentReadsNextElement(AudioStream stream, long position, byte[] buffer, MatroskaElement parent, MatroskaElement expectedElement)
    {
        // Arrange
        var expectedPosition = position + expectedElement.HeaderSize;

        // Act
        var result = ElementReader.ReadNextElement(stream, position, buffer, NullLogger.Instance, parent);

        // Assert
        Assert.True(result.Element.HasValue);
        Assert.Equal(expectedElement, result.Element.Value);
        Assert.Equal(result.NewPosition, expectedPosition);
    }

    [Theory]
    [MemberData(nameof(ReadNextElementWithParentWithoutRemainingParentReturnsNullData))]
    public async Task ReadNextElementAsyncWithParentWithoutRemainingParentReturnsNull(AudioStream stream, long position, byte[] buffer, MatroskaElement parent)
    {
        // Act
        var result = await ElementReader.ReadNextElementAsync(stream, position, buffer, NullLogger.Instance, parent);

        // Assert
        Assert.False(result.Element.HasValue);
        Assert.Equal(position, result.NewPosition);
    }

    [Theory]
    [MemberData(nameof(ReadNextElementWithoutParentReadsNextElementData))]
    public async Task ReadNextElementAsyncWithoutParentReadsNextElement(AudioStream stream, long position, byte[] buffer, MatroskaElement expectedElement)
    {
        // Arrange
        var expectedPosition = position + expectedElement.HeaderSize;

        // Act
        var result = await ElementReader.ReadNextElementAsync(stream, position, buffer, NullLogger.Instance);

        // Assert
        Assert.True(result.Element.HasValue);
        Assert.Equal(expectedElement, result.Element.Value);
        Assert.Equal(result.NewPosition, expectedPosition);
    }

    [Theory]
    [MemberData(nameof(ReadNextElementWithParentReadsNextElementData))]
    public async Task ReadNextElementAsyncWithParentReadsNextElement(AudioStream stream, long position, byte[] buffer, MatroskaElement parent, MatroskaElement expectedElement)
    {
        // Arrange
        var expectedPosition = position + expectedElement.HeaderSize;

        // Act
        var result = await ElementReader.ReadNextElementAsync(stream, position, buffer, NullLogger.Instance, parent);

        // Assert
        Assert.True(result.Element.HasValue);
        Assert.Equal(expectedElement, result.Element.Value);
        Assert.Equal(result.NewPosition, expectedPosition);
    }
}
