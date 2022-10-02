// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NextAudio.Utils;
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

    [Theory]
    [InlineData(new object[] { 10, 10, 20 })]
    [InlineData(new object[] { 82, 8, 90 })]
    public void ComputePositionSumsPositionAndBytesReaded(long position, int bytesReaded, long expectedPosition)
    {
        var result = AudioStreamUtils.ComputePosition(position, bytesReaded);

        Assert.Equal(expectedPosition, result);
    }

    private class PartialReadAudioStreamMock : AudioStream
    {
        private readonly byte[] _buffer;
        private readonly int _maxRead;

        public PartialReadAudioStreamMock(byte[] buffer, int maxRead)
        {
            _buffer = buffer;
            _maxRead = maxRead;
        }

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

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
