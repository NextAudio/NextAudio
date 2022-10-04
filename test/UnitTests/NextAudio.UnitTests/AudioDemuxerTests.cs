// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NextAudio.UnitTests;

public class AudioDemuxerTests
{
    [Fact]
    public void CanSeekReturnsFalse()
    {
        // Arrange
        AudioDemuxer demuxer = new AudioDemuxerMock((_) => { });

        // Act
        var result = demuxer.CanSeek;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void SeekThrowsNotSupportedException()
    {
        // Arrange
        AudioDemuxer demuxer = new AudioDemuxerMock((_) => { });

        // Act + Assert
        _ = Assert.Throws<NotSupportedException>(() =>
        {
            _ = demuxer.Seek(0, SeekOrigin.Begin);
        });
    }

    [Fact]
    public void ReadCallsDemux()
    {
        // Arrange
        var expectedBuffer = new byte[]
        {
            1,
            2,
            3
        };

        var receivedBufferCalls = 0;
        byte[]? receivedBuffer = null;

        AudioDemuxer demuxer = new AudioDemuxerMock((buffer) =>
        {
            receivedBuffer = buffer;
            receivedBufferCalls++;
        });

        // Act
        _ = demuxer.Read(expectedBuffer);

        // Assert
        Assert.NotNull(receivedBuffer);
        Assert.Equal(1, receivedBufferCalls);
        Assert.Equal(expectedBuffer, receivedBuffer);
    }

    [Fact]
    public async Task ReadAsyncCallsDemux()
    {
        // Arrange
        var expectedBuffer = new byte[]
        {
            1,
            2,
            3
        };

        var receivedBufferCalls = 0;
        byte[]? receivedBuffer = null;

        AudioDemuxer demuxer = new AudioDemuxerMock((buffer) =>
        {
            receivedBuffer = buffer;
            receivedBufferCalls++;
        });

        // Act
        _ = await demuxer.ReadAsync(expectedBuffer);

        // Assert
        Assert.NotNull(receivedBuffer);
        Assert.Equal(1, receivedBufferCalls);
        Assert.Equal(expectedBuffer, receivedBuffer);
    }

    [Fact]
    public void DemuxByteArrayCallsSpanDemux()
    {
        // Arrange
        var expectedBuffer = new byte[]
        {
            1,
            2,
            3
        };

        var receivedBufferCalls = 0;
        byte[]? receivedBuffer = null;

        AudioDemuxer demuxer = new AudioDemuxerMock((buffer) =>
        {
            receivedBuffer = buffer;
            receivedBufferCalls++;
        });

        // Act
        _ = demuxer.Demux(expectedBuffer, 0, 3);

        // Assert
        Assert.NotNull(receivedBuffer);
        Assert.Equal(1, receivedBufferCalls);
        Assert.Equal(expectedBuffer, receivedBuffer);
    }

    [Fact]
    public async Task DemuxByteArrayAsyncCallsAsyncMemoryDemux()
    {
        // Arrange
        var expectedBuffer = new byte[]
        {
            1,
            2,
            3
        };

        var receivedBufferCalls = 0;
        byte[]? receivedBuffer = null;

        AudioDemuxer demuxer = new AudioDemuxerMock((buffer) =>
        {
            receivedBuffer = buffer;
            receivedBufferCalls++;
        });

        // Act
        _ = await demuxer.DemuxAsync(expectedBuffer, 0, 3);

        // Assert
        Assert.NotNull(receivedBuffer);
        Assert.Equal(1, receivedBufferCalls);
        Assert.Equal(expectedBuffer, receivedBuffer);
    }

    private sealed class AudioDemuxerMock : AudioDemuxer
    {
        private readonly Action<byte[]> _callback;

        public AudioDemuxerMock(Action<byte[]> callback)
        {
            _callback = callback;
        }

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override AudioStream Clone()
        {
            throw new NotImplementedException();
        }

        public override int Demux(Span<byte> buffer)
        {
            _callback(buffer.ToArray());
            return default;
        }

        public override ValueTask<int> DemuxAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            _callback(buffer.ToArray());
            return default;
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
