// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NextAudio.UnitTests;

public class ReadOnlyAudioStreamTests
{
    [Fact]
    public void CanWriteReturnsFalse()
    {
        // Arrange
        var stream = new ReadOnlyAudioStreamMock();

        // Act
        var result = stream.CanWrite;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanReadReturnsTrue()
    {
        // Arrange
        var stream = new ReadOnlyAudioStreamMock();

        // Act
        var result = stream.CanRead;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void WriteThrowsNotSupportedException()
    {
        // Arrange
        var stream = new ReadOnlyAudioStreamMock();

        // Act + Assert
        _ = Assert.Throws<NotSupportedException>(() =>
        {
            stream.Write(ReadOnlySpan<byte>.Empty);
        });
    }

    [Fact]
    public async Task WriteAsyncThrowsNotSupportedException()
    {
        // Arrange
        var stream = new ReadOnlyAudioStreamMock();

        // Act + Assert
        _ = await Assert.ThrowsAsync<NotSupportedException>(async () =>
        {
            await stream.WriteAsync(ReadOnlyMemory<byte>.Empty);
        });
    }

    [Fact]
    public void SetLengthThrowsNotSupportedException()
    {
        var stream = new ReadOnlyAudioStreamMock();

        _ = Assert.Throws<NotSupportedException>(() =>
        {
            stream.SetLength(0);
        });
    }

    private sealed class ReadOnlyAudioStreamMock : ReadOnlyAudioStream
    {
        public override bool CanSeek => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override RecommendedSynchronicity RecommendedSynchronicity => throw new NotImplementedException();

        public override AudioStream Clone()
        {
            throw new NotImplementedException();
        }

        public override int Read(Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
        }

        protected override ValueTask DisposeAsyncCore()
        {
            return ValueTask.CompletedTask;
        }
    }
}
