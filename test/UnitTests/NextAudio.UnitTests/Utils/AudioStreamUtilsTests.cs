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
        var result = AudioStreamUtils.ReadFullyAudioStream(stream, buffer);

        Assert.Equal(buffer.Length, result);
    }

    [Theory]
    [MemberData(nameof(ReadFullyAudioStreamReadsCurrentAudioStreamData))]
    public async Task ReadFullyAudioStreamAsyncReadsCurrentAudioStream(AudioStream stream, byte[] buffer)
    {
        var result = await AudioStreamUtils.ReadFullyAudioStreamAsync(stream, buffer);

        Assert.Equal(buffer.Length, result);
    }

    [Fact]
    public void ReadFullyAudioStreamNotReadsIfAudioStreamEnded()
    {
        AudioStream stream = new MemoryStream(Array.Empty<byte>());

        var result = AudioStreamUtils.ReadFullyAudioStream(stream, new byte[10]);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task ReadFullyAudioStreamAsyncNotReadsIfAudioStreamEnded()
    {
        AudioStream stream = new MemoryStream(Array.Empty<byte>());

        var result = await AudioStreamUtils.ReadFullyAudioStreamAsync(stream, new byte[10]);

        Assert.Equal(0, result);
    }

    [Fact]
    public void SeekCallsSourceStreamSeekIfCanSeek()
    {
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(true);

        _ = AudioStreamUtils.Seek(stream, 1000, SeekOrigin.Current, 0);

        _ = stream.Received(1).Seek(1000, SeekOrigin.Current);
    }

    [Fact]
    public async Task SeekAsyncCallsSourceStreamSeekIfCanSeek()
    {
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(true);

        _ = await AudioStreamUtils.SeekAsync(stream, 1000, SeekOrigin.Current, 0);

        _ = stream.Received(1).Seek(1000, SeekOrigin.Current);
    }

    [Fact]
    public void SeekThrowsNotSupportedExceptionIfSourceCantSeekAndSeekOriginIsEndAndLengthThrows()
    {
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);
        _ = stream.Length.Returns((x) => throw new NotSupportedException());

        _ = Assert.Throws<NotSupportedException>(() =>
        {
            _ = AudioStreamUtils.Seek(stream, 1000, SeekOrigin.End, 0);
        });
    }

    [Fact]
    public async Task SeekAsyncThrowsNotSupportedExceptionIfSourceCantSeekAndSeekOriginIsEndAndLengthThrows()
    {
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);
        _ = stream.Length.Returns((x) => throw new NotSupportedException());

        _ = await Assert.ThrowsAsync<NotSupportedException>(async () =>
        {
            _ = await AudioStreamUtils.SeekAsync(stream, 1000, SeekOrigin.End, 0);
        });
    }

    [Fact]
    public void SeekThrowsIfSeekOriginIsUnknownAndSourceCantSeek()
    {
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);

        _ = Assert.Throws<InvalidOperationException>(() =>
        {
            _ = AudioStreamUtils.Seek(stream, 1000, (SeekOrigin)10, 0);
        });
    }

    [Fact]
    public async Task SeekAsyncThrowsIfSeekOriginIsUnknownAndSourceCantSeek()
    {
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);

        _ = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            _ = await AudioStreamUtils.SeekAsync(stream, 1000, (SeekOrigin)10, 0);
        });
    }

    [Fact]
    public void SeekThrowsIfSourceCantSeekAndNewPositionIsLowerThanCurrentPosition()
    {
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);

        _ = Assert.Throws<NotSupportedException>(() =>
        {
            _ = AudioStreamUtils.Seek(stream, 500, SeekOrigin.Begin, 1000);
        });
    }

    [Fact]
    public async Task SeekAsyncThrowsIfSourceCantSeekAndNewPositionIsLowerThanCurrentPosition()
    {
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);

        _ = await Assert.ThrowsAsync<NotSupportedException>(async () =>
        {
            _ = await AudioStreamUtils.SeekAsync(stream, 500, SeekOrigin.Begin, 1000);
        });
    }

    [Fact]
    public void SeekThrowsIfSourceCantSeekAndNewPositionIsHigherThanIntMaxValue()
    {
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);

        _ = Assert.Throws<InvalidOperationException>(() =>
        {
            _ = AudioStreamUtils.Seek(stream, ((long)int.MaxValue) + 1, SeekOrigin.Current, 0);
        });
    }

    [Fact]
    public async Task SeekAsyncThrowsIfSourceCantSeekAndNewPositionIsHigherThanIntMaxValue()
    {
        var stream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        _ = stream.CanSeek.Returns(false);

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
        var stream = new PartialReadAudioStreamMock(false, length);

        var result = AudioStreamUtils.Seek(stream, offset, origin, currentPosition);

        Assert.Equal(expectedPosition, result);
    }

    [Theory]
    [InlineData(100, SeekOrigin.Begin, 10, 0, 100)]
    [InlineData(100, SeekOrigin.Current, 10, 0, 110)]
    [InlineData(-100, SeekOrigin.End, 500, 1000, 900)]
    public async Task SeekAsyncForcellySeekIfSourceCantSeek(long offset, SeekOrigin origin, long currentPosition, long length, long expectedPosition)
    {
        var stream = new PartialReadAudioStreamMock(false, length);

        var result = await AudioStreamUtils.SeekAsync(stream, offset, origin, currentPosition);

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
