// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NextAudio.Utils;
using NSubstitute;
using Xunit;

namespace NextAudio.UnitTests.Utils;

public class AudioStreamUtilsTests
{
    public static IEnumerable<object[]> ReadFullyAudioStreamReadsCurrentAudioStreamData()
    {
        yield return new object[]
        {
            (AudioStream)new MemoryStream(new byte[] { 1, 2, 3, 4, 5 }),
            new byte[5],
        };
        yield return new object[]
        {
            (AudioStream)new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }),
            new byte[7],
        };

        yield return new object[]
        {
            new PartialReadAudioStreamMock(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 5),
            new byte[7],
        };
        yield return new object[]
        {
            new PartialReadAudioStreamMock(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 7),
            new byte[10],
        };
    }

    [Theory]
    [MemberData(nameof(ReadFullyAudioStreamReadsCurrentAudioStreamData))]
    public void ReadFullyAudioStreamReadsCurrentAudioStream(AudioStream stream, byte[] buffer)
    {
        // Act
        var result = AudioStreamUtils.ReadFullyAudioStream(stream, buffer);

        // Assert
        Assert.Equal(buffer.Length, result);
    }

    [Theory]
    [MemberData(nameof(ReadFullyAudioStreamReadsCurrentAudioStreamData))]
    public async Task ReadFullyAudioStreamAsyncReadsCurrentAudioStream(AudioStream stream, byte[] buffer)
    {
        // Act
        var result = await AudioStreamUtils.ReadFullyAudioStreamAsync(stream, buffer);

        // Assert
        Assert.Equal(buffer.Length, result);
    }

    [Fact]
    public void ReadFullyAudioStreamNotReadsIfAudioStreamEnded()
    {
        // Arrange
        AudioStream stream = new MemoryStream(Array.Empty<byte>());

        // Act
        var result = AudioStreamUtils.ReadFullyAudioStream(stream, new byte[10]);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task ReadFullyAudioStreamAsyncNotReadsIfAudioStreamEnded()
    {
        // Arrange
        AudioStream stream = new MemoryStream(Array.Empty<byte>());

        // Act
        var result = await AudioStreamUtils.ReadFullyAudioStreamAsync(stream, new byte[10]);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void SeekCallsSourceStreamSeekIfCanSeek()
    {
        // Arrange
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(true);

        // Act
        _ = AudioStreamUtils.Seek(stream, 1000, SeekOrigin.Current, 0);

        // Assert
        _ = stream.Received(1).Seek(1000, SeekOrigin.Current);
    }

    [Fact]
    public async Task SeekAsyncCallsSourceStreamSeekIfCanSeek()
    {
        // Arrange
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(true);

        // Act
        _ = await AudioStreamUtils.SeekAsync(stream, 1000, SeekOrigin.Current, 0);

        // Assert
        _ = stream.Received(1).Seek(1000, SeekOrigin.Current);
    }

    [Fact]
    public void SeekThrowsNotSupportedExceptionIfSourceCantSeekAndSeekOriginIsEndAndLengthThrows()
    {
        // Arrange
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);
        _ = stream.Length.Returns((x) => throw new NotSupportedException());

        // Act + Assert
        _ = Assert.Throws<NotSupportedException>(() =>
        {
            _ = AudioStreamUtils.Seek(stream, 1000, SeekOrigin.End, 0);
        });
    }

    [Fact]
    public async Task SeekAsyncThrowsNotSupportedExceptionIfSourceCantSeekAndSeekOriginIsEndAndLengthThrows()
    {
        // Arrange
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);
        _ = stream.Length.Returns((x) => throw new NotSupportedException());

        // Act + Assert
        _ = await Assert.ThrowsAsync<NotSupportedException>(async () =>
        {
            _ = await AudioStreamUtils.SeekAsync(stream, 1000, SeekOrigin.End, 0);
        });
    }

    [Fact]
    public void SeekThrowsIfSeekOriginIsUnknownAndSourceCantSeek()
    {
        // Arrange
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);

        // Act + Assert
        _ = Assert.Throws<InvalidOperationException>(() =>
        {
            _ = AudioStreamUtils.Seek(stream, 1000, (SeekOrigin)10, 0);
        });
    }

    [Fact]
    public async Task SeekAsyncThrowsIfSeekOriginIsUnknownAndSourceCantSeek()
    {
        // Arrange
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);

        // Act + Assert
        _ = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            _ = await AudioStreamUtils.SeekAsync(stream, 1000, (SeekOrigin)10, 0);
        });
    }

    [Fact]
    public void SeekThrowsIfSourceCantSeekAndNewPositionIsLowerThanCurrentPosition()
    {
        // Arrange
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);

        // Act + Assert
        _ = Assert.Throws<NotSupportedException>(() =>
        {
            _ = AudioStreamUtils.Seek(stream, 500, SeekOrigin.Begin, 1000);
        });
    }

    [Fact]
    public async Task SeekAsyncThrowsIfSourceCantSeekAndNewPositionIsLowerThanCurrentPosition()
    {
        // Arrange
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);

        // Act + Assert
        _ = await Assert.ThrowsAsync<NotSupportedException>(async () =>
        {
            _ = await AudioStreamUtils.SeekAsync(stream, 500, SeekOrigin.Begin, 1000);
        });
    }

    [Fact]
    public void SeekThrowsIfSourceCantSeekAndNewPositionIsHigherThanIntMaxValue()
    {
        // Arrange
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);

        // Act + Assert
        _ = Assert.Throws<InvalidOperationException>(() =>
        {
            _ = AudioStreamUtils.Seek(stream, ((long)int.MaxValue) + 1, SeekOrigin.Current, 0);
        });
    }

    [Fact]
    public async Task SeekAsyncThrowsIfSourceCantSeekAndNewPositionIsHigherThanIntMaxValue()
    {
        // Arrange
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);

        // Act + Assert
        _ = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            _ = await AudioStreamUtils.SeekAsync(stream, ((long)int.MaxValue) + 1, SeekOrigin.Current, 0);
        });
    }

    [Theory]
    [InlineData(100, SeekOrigin.Begin, 10, 0, 100)]
    [InlineData(100, SeekOrigin.Current, 10, 0, 110)]
    [InlineData(-100, SeekOrigin.End, 500, 1000, 900)]
    public void SeekForcellySeekIfSourceCantSeek(long offset, SeekOrigin origin, long currentPosition, long length, long expectedPosition)
    {
        // Arrange
        var stream = new PartialReadAudioStreamMock(false, length);

        // Act
        var result = AudioStreamUtils.Seek(stream, offset, origin, currentPosition);

        // Assert
        Assert.Equal(expectedPosition, result);
    }

    [Theory]
    [InlineData(100, SeekOrigin.Begin, 10, 0, 100)]
    [InlineData(100, SeekOrigin.Current, 10, 0, 110)]
    [InlineData(-100, SeekOrigin.End, 500, 1000, 900)]
    public async Task SeekAsyncForcellySeekIfSourceCantSeek(long offset, SeekOrigin origin, long currentPosition, long length, long expectedPosition)
    {
        // Arrange
        var stream = new PartialReadAudioStreamMock(false, length);

        // Act
        var result = await AudioStreamUtils.SeekAsync(stream, offset, origin, currentPosition);

        // Assert
        Assert.Equal(expectedPosition, result);
    }

    private class PartialReadAudioStreamMock : AudioStream
    {
        private readonly bool _canSeek;
        private readonly long _length;
        private readonly byte[] _buffer;
        private readonly int _maxRead;

        public PartialReadAudioStreamMock(byte[] buffer, int maxRead)
        {
            _buffer = buffer;
            _maxRead = maxRead;
        }

        public PartialReadAudioStreamMock(bool canSeek, long length)
        {
            _buffer = new byte[8 * 1024];
            _maxRead = 8 * 1024;
            _canSeek = canSeek;
            _length = length;
        }

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => _canSeek;

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => _length;

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override RecommendedSynchronicity RecommendedSynchronicity => throw new NotImplementedException();

        public override AudioStream Clone()
        {
            throw new NotImplementedException();
        }

        public override int Read(Span<byte> buffer)
        {
            var limit = Math.Min(buffer.Length, _maxRead);

            _buffer.AsSpan(0, limit).CopyTo(buffer);

            return limit;
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var limit = Math.Min(buffer.Length, _maxRead);

            _buffer.AsMemory(0, limit).CopyTo(buffer);

            return ValueTask.FromResult(limit);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<long> SeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
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
