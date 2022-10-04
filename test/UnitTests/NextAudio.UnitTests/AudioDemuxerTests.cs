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
    public void ReadCallsDemux()
    {
        // Act
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

        // Arrange
        _ = demuxer.Read(expectedBuffer);

        // Assert
        Assert.NotNull(receivedBuffer);
        Assert.Equal(1, receivedBufferCalls);
        Assert.Equal(expectedBuffer, receivedBuffer);
    }

    [Fact]
    public async Task ReadAsyncCallsDemux()
    {
        // Act
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

        // Arrange
        _ = await demuxer.ReadAsync(expectedBuffer);

        // Assert
        Assert.NotNull(receivedBuffer);
        Assert.Equal(1, receivedBufferCalls);
        Assert.Equal(expectedBuffer, receivedBuffer);
    }

    [Fact]
    public void DemuxByteArrayCallsSpanDemux()
    {
        // Act
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

        // Arrange
        _ = demuxer.Demux(expectedBuffer, 0, 3);

        // Assert
        Assert.NotNull(receivedBuffer);
        Assert.Equal(1, receivedBufferCalls);
        Assert.Equal(expectedBuffer, receivedBuffer);
    }

    [Fact]
    public async Task DemuxByteArrayAsyncCallsAsyncMemoryDemux()
    {
        // Act
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

        // Arrange
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

        public override bool CanSeek => throw new NotImplementedException();

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
