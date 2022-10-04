// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging.Abstractions;
using NextAudio.Matroska.Internal;
using NextAudio.Matroska.Models;
using Xunit;

namespace NextAudio.Matroska.UnitTests.Internal;

public class VIntReaderTests
{
    public static IEnumerable<object[]> ReadVIntReadsVIntData()
    {
        yield return new object[]
        {
            (AudioStream)new MemoryStream(new byte[] { 26, 69, 223, 163 }),
            new byte[4],
            new VInt(0x1A45DFA3, 4),
        };
        yield return new object[]
        {
            (AudioStream)new MemoryStream(new byte[] { 159, 66, 134, 129, 1, 66, 247, 29 }),
            new byte[1],
            new VInt(159, 1),
        };
        yield return new object[]
        {
            (AudioStream)new MemoryStream(new byte[] { 26, 69, 223, 163, 100, 100, 100 }),
            new byte[4],
            new VInt(0x1A45DFA3, 4),
        };
        yield return new object[]
        {
            (AudioStream)new MemoryStream(new byte[] { 159, 66, 134, 129, 1, 66, 247, 29, 100, 100, 100 }),
            new byte[1],
            new VInt(159, 1),
        };
    }

    [Theory]
    [MemberData(nameof(ReadVIntReadsVIntData))]
    public void ReadVIntReadsVInt(AudioStream stream, byte[] buffer, VInt expectedVInt)
    {
        // Act
        var result = VIntReader.ReadVInt(stream, buffer, 0, NullLogger.Instance);

        // Assert
        Assert.Equal(expectedVInt, result);
    }

    [Theory]
    [MemberData(nameof(ReadVIntReadsVIntData))]
    public async Task ReadVIntAsyncReadsVInt(AudioStream stream, byte[] buffer, VInt expectedVInt)
    {
        // Act
        var result = await VIntReader.ReadVIntAsync(stream, buffer, 0, NullLogger.Instance);

        // Assert
        Assert.Equal(expectedVInt, result);
    }
}
