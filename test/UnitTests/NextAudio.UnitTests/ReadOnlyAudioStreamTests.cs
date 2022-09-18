// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace NextAudio.UnitTests;

public class ReadOnlyAudioStreamTests
{
    [Fact]
    public void CanWriteReturnsFalse()
    {
        // Act
        var stream = new ReadOnlyAudioStreamMock();

        // Arrange
        var result = stream.CanWrite;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanReadReturnsTrue()
    {
        // Act
        var stream = new ReadOnlyAudioStreamMock();

        // Arrange
        var result = stream.CanRead;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void WriteThrowsNotSupportedException()
    {
        // Act
        var stream = new ReadOnlyAudioStreamMock();

        // Arrange + Assert
        _ = Assert.Throws<NotSupportedException>(() =>
        {
            stream.Write(ReadOnlySpan<byte>.Empty);
        });
    }

    private sealed class ReadOnlyAudioStreamMock : ReadOnlyAudioStream
    {
        public override bool CanSeek => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override AudioStream Clone()
        {
            throw new NotImplementedException();
        }

        public override int Read(Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }

        protected override ValueTask DisposeAsyncCore()
        {
            throw new NotImplementedException();
        }
    }
}
