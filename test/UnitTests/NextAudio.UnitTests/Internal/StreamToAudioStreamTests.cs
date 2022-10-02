// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NextAudio.Internal;
using NSubstitute;
using Xunit;

namespace NextAudio.UnitTests.Internal;

public class StreamToAudioStreamTests
{
    [Fact]
    public void CanReadCallsSourceStreamCanRead()
    {
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream);

        _ = castStream.CanRead;

        _ = stream.Received(1).CanRead;
    }

    [Fact]
    public void CanSeekCallsSourceStreamCanSeek()
    {
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream);

        _ = castStream.CanSeek;

        _ = stream.Received(1).CanSeek;
    }

    [Fact]
    public void CanWriteCallsSourceStreamCanWrite()
    {
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream);

        _ = castStream.CanWrite;

        _ = stream.Received(1).CanWrite;
    }

    [Fact]
    public void LengthCallsSourceStreamLength()
    {
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream);

        _ = castStream.Length;

        _ = stream.Received(1).Length;
    }

    [Fact]
    public void PositionCallsSourceStreamPosition()
    {
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream)
        {
            Position = 1
        };

        _ = castStream.Position;

        stream.Received(1).Position = 1;
        _ = stream.Received(1).Position;
    }

    [Fact]
    public void SeekCallsSourceStreamSeek()
    {
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream);

        _ = castStream.Seek(0, SeekOrigin.Begin);

        _ = stream.Received(1).Seek(0, SeekOrigin.Begin);
    }

    [Fact]
    public void SetLengthCallsSourceStreamSetLength()
    {
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream);

        castStream.SetLength(0);

        stream.Received(1).SetLength(0);
    }

    [Fact]
    public void ReadCallsSourceStreamRead()
    {
        var expectedByteArray = new byte[]
        {
            1,
            2,
            3,
        };
        byte[]? byteArray = null;

        var stream = new StreamMock((buffer) =>
        {
            byteArray = buffer;
        });

        var castStream = new StreamToAudioStream(stream);

        _ = castStream.Read(expectedByteArray.AsSpan());

        Assert.NotNull(byteArray);
        Assert.Equal(expectedByteArray, byteArray);
    }

    [Fact]
    public void WriteCallsSourceStreamWrite()
    {
        var expectedByteArray = new byte[]
        {
            1,
            2,
            3,
        };
        byte[]? byteArray = null;

        var stream = new StreamMock((buffer) =>
        {
            byteArray = buffer;
        });

        var castStream = new StreamToAudioStream(stream);

        castStream.Write(expectedByteArray.AsSpan());

        Assert.NotNull(byteArray);
        Assert.Equal(expectedByteArray, byteArray);
    }

    [Fact]
    public async Task ReadAsyncCallsSourceStreamReadAsync()
    {
        var expectedByteArray = new byte[]
        {
            1,
            2,
            3,
        };
        byte[]? byteArray = null;

        var stream = new StreamMock((buffer) =>
        {
            byteArray = buffer;
        });

        var castStream = new StreamToAudioStream(stream);

        _ = await castStream.ReadAsync(expectedByteArray.AsMemory());

        Assert.NotNull(byteArray);
        Assert.Equal(expectedByteArray, byteArray);
    }

    [Fact]
    public async Task WriteAsyncCallsSourceStreamWriteAsync()
    {
        var expectedByteArray = new byte[]
        {
            1,
            2,
            3,
        };
        byte[]? byteArray = null;

        var stream = new StreamMock((buffer) =>
        {
            byteArray = buffer;
        });

        var castStream = new StreamToAudioStream(stream);

        await castStream.WriteAsync(expectedByteArray.AsMemory());

        Assert.NotNull(byteArray);
        Assert.Equal(expectedByteArray, byteArray);
    }

    [Fact]
    public void DisposeCallsStreamDispose()
    {
        var disposeCalled = false;
        var disposeCallsCount = 0;

        var stream = new StreamMock(() =>
        {
            disposeCalled = true;
            disposeCallsCount++;
        });

        var castStream = new StreamToAudioStream(stream);

        castStream.Dispose();

        Assert.True(disposeCalled);
        Assert.Equal(1, disposeCallsCount);
    }

    [Fact]
    public async Task DisposeAsyncCallsStreamDisposeAsync()
    {
        var disposeAsyncCalled = false;
        var disposeAsyncCallsCount = 0;

        var stream = new StreamMock(() =>
        {
            disposeAsyncCalled = true;
            disposeAsyncCallsCount++;
        });

        var castStream = new StreamToAudioStream(stream);

        await castStream.DisposeAsync();

        Assert.True(disposeAsyncCalled);
        Assert.Equal(1, disposeAsyncCallsCount);
    }

    private class StreamMock : Stream
    {
        private readonly Action<byte[]>? _readWriteCallback;
        private readonly Action? _disposeCallback;

        public StreamMock(Action<byte[]> readWriteCallback)
        {
            _readWriteCallback = readWriteCallback;
        }

        public StreamMock(Action? disposeCallback)
        {
            _disposeCallback = disposeCallback;
        }

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            _readWriteCallback!(buffer.ToArray());

            return default;
        }

        public override int Read(Span<byte> buffer)
        {
            _readWriteCallback!(buffer.ToArray());

            return default;
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            _readWriteCallback!(buffer.ToArray());

            return default;
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            _readWriteCallback!(buffer.ToArray());
        }

        protected override void Dispose(bool disposing)
        {
            _disposeCallback!.Invoke();
        }

        public override ValueTask DisposeAsync()
        {
            _disposeCallback!.Invoke();
            return ValueTask.CompletedTask;
        }
    }
}
