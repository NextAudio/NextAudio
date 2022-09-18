// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace NextAudio.UnitTests;

public class WriteOnlyAudioStreamTests
{
    [Fact]
    public void CanWriteReturnsTrue()
    {
        // Act
        var stream = new WriteOnlyAudioStreamMock();

        // Arrange
        var result = stream.CanWrite;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanReadReturnsFalse()
    {
        // Act
        var stream = new WriteOnlyAudioStreamMock();

        // Arrange
        var result = stream.CanRead;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ReadThrowsNotSupportedException()
    {
        // Act
        var stream = new WriteOnlyAudioStreamMock();

        // Arrange + Assert
        _ = Assert.Throws<NotSupportedException>(() =>
        {
            _ = stream.Read(Span<byte>.Empty);
        });
    }

    private sealed class WriteOnlyAudioStreamMock : WriteOnlyAudioStream
    {
        public override bool CanSeek => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override AudioStream Clone()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void Write(ReadOnlySpan<byte> buffer)
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
