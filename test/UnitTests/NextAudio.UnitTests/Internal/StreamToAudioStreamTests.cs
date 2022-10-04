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
    public void RecommendedSynchronicityCallsOptionsRecommendedSynchronicity()
    {
        // Arrange
        var options = new StreamToAudioStreamOptions()
        {
            RecommendedSynchronicity = RecommendedSynchronicity.Async,
        };
        var stream = Substitute.For<Stream>();
        var castStream = new StreamToAudioStream(stream, options);

        // Act
        var result = castStream.RecommendedSynchronicity;

        // Assert
        Assert.Equal(RecommendedSynchronicity.Async, result);
    }

    [Fact]
    public void CanReadCallsSourceStreamCanRead()
    {
        // Arrange
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream, StreamToAudioStreamOptions.Default);

        // Act
        _ = castStream.CanRead;

        // Assert
        _ = stream.Received(1).CanRead;
    }

    [Fact]
    public void CanSeekCallsSourceStreamCanSeek()
    {
        // Arrange
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream, StreamToAudioStreamOptions.Default);

        // Act
        _ = castStream.CanSeek;

        // Assert
        _ = stream.Received(1).CanSeek;
    }

    [Fact]
    public void CanWriteCallsSourceStreamCanWrite()
    {
        // Arrange
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream, StreamToAudioStreamOptions.Default);

        // Act
        _ = castStream.CanWrite;

        // Assert
        _ = stream.Received(1).CanWrite;
    }

    [Fact]
    public void LengthCallsSourceStreamLength()
    {
        // Arrange
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream, StreamToAudioStreamOptions.Default);

        // Act
        _ = castStream.Length;

        // Assert
        _ = stream.Received(1).Length;
    }

    [Fact]
    public void PositionCallsSourceStreamPosition()
    {
        // Arrange
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream, StreamToAudioStreamOptions.Default)
        {
            Position = 1
        };

        // Act
        _ = castStream.Position;

        // Assert
        stream.Received(1).Position = 1;
        _ = stream.Received(1).Position;
    }

    [Fact]
    public void SeekCallsSourceStreamSeek()
    {
        // Arrange
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream, StreamToAudioStreamOptions.Default);

        // Act
        _ = castStream.Seek(0, SeekOrigin.Begin);

        // Assert
        _ = stream.Received(1).Seek(0, SeekOrigin.Begin);
    }

    [Fact]
    public async Task SeekAsyncCallsSourceStreamSeek()
    {
        // Arrange
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream, StreamToAudioStreamOptions.Default);

        // Act
        _ = await castStream.SeekAsync(0, SeekOrigin.Begin);

        // Assert
        _ = stream.Received(1).Seek(0, SeekOrigin.Begin);
    }

    [Fact]
    public void SetLengthCallsSourceStreamSetLength()
    {
        // Arrange
        var stream = Substitute.For<Stream>();

        var castStream = new StreamToAudioStream(stream, StreamToAudioStreamOptions.Default);

        // Act
        castStream.SetLength(0);

        // Assert
        stream.Received(1).SetLength(0);
    }

    [Fact]
    public void ReadCallsSourceStreamRead()
    {
        // Arrange
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

        var castStream = new StreamToAudioStream(stream, StreamToAudioStreamOptions.Default);

        // Act
        _ = castStream.Read(expectedByteArray.AsSpan());

        // Assert
        Assert.NotNull(byteArray);
        Assert.Equal(expectedByteArray, byteArray);
    }

    [Fact]
    public void WriteCallsSourceStreamWrite()
    {
        // Arrange
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

        var castStream = new StreamToAudioStream(stream, StreamToAudioStreamOptions.Default);

        // Act
        castStream.Write(expectedByteArray.AsSpan());

        // Assert
        Assert.NotNull(byteArray);
        Assert.Equal(expectedByteArray, byteArray);
    }

    [Fact]
    public async Task ReadAsyncCallsSourceStreamReadAsync()
    {
        // Arrange
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

        var castStream = new StreamToAudioStream(stream, StreamToAudioStreamOptions.Default);

        // Act
        _ = await castStream.ReadAsync(expectedByteArray.AsMemory());

        // Assert
        Assert.NotNull(byteArray);
        Assert.Equal(expectedByteArray, byteArray);
    }

    [Fact]
    public async Task WriteAsyncCallsSourceStreamWriteAsync()
    {
        // Arrange
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

        var castStream = new StreamToAudioStream(stream, StreamToAudioStreamOptions.Default);

        // Act
        await castStream.WriteAsync(expectedByteArray.AsMemory());

        // Assert
        Assert.NotNull(byteArray);
        Assert.Equal(expectedByteArray, byteArray);
    }

    [Fact]
    public void CloneReturnsANewAudioStreamAndChangeTheCurrentOptionsToNotDisposeSourceStream()
    {
        // Arrange
        var stream = new MemoryStream(new byte[] { 1, 2 });
        var options = new StreamToAudioStreamOptions()
        {
            DisposeSourceStream = true,
        };
        var castStream = new StreamToAudioStream(stream, options);

        // Act

        var castResult = castStream.Clone();

        // Don't dispose the source stream.
        castStream.Dispose();

        // If source stream disposed will throw exception.
        var byteResult = stream.ReadByte();

        castResult.Dispose();

        // Assert

        Assert.NotNull(castResult);
        Assert.Equal(1, byteResult);
        _ = Assert.Throws<ObjectDisposedException>(() =>
        {
            _ = stream.ReadByte();
        });
    }

    [Fact]
    public void DisposeCallsStreamDispose()
    {
        // Arrange
        var disposeCalled = false;
        var disposeCallsCount = 0;

        var stream = new StreamMock(() =>
        {
            disposeCalled = true;
            disposeCallsCount++;
        });

        var castStream = new StreamToAudioStream(stream, StreamToAudioStreamOptions.Default);

        // Act
        castStream.Dispose();

        // Assert
        Assert.True(disposeCalled);
        Assert.Equal(1, disposeCallsCount);
    }

    [Fact]
    public async Task DisposeAsyncCallsStreamDisposeAsync()
    {
        // Arrange
        var disposeAsyncCalled = false;
        var disposeAsyncCallsCount = 0;

        var stream = new StreamMock(() =>
        {
            disposeAsyncCalled = true;
            disposeAsyncCallsCount++;
        });

        var castStream = new StreamToAudioStream(stream, StreamToAudioStreamOptions.Default);

        // Act
        await castStream.DisposeAsync();

        // Assert
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
