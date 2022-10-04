// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NextAudio.Internal;
using NSubstitute;
using Xunit;

namespace NextAudio.UnitTests.Internal;

public class AudioStreamToStreamTests
{
    [Fact]
    public void CanReadCallsSourceAudioStreamCanRead()
    {
        // Arrange
        var audioStream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        var castStream = new AudioStreamToStream(audioStream);

        // Act
        _ = castStream.CanRead;

        // Assert
        _ = audioStream.Received(1).CanRead;
    }

    [Fact]
    public void CanSeekCallsSourceAudioStreamCanSeek()
    {
        // Arrange
        var audioStream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        var castStream = new AudioStreamToStream(audioStream);

        // Act
        _ = castStream.CanSeek;

        // Assert
        _ = audioStream.Received(1).CanSeek;
    }

    [Fact]
    public void CanWriteCallsSourceAudioStreamCanWrite()
    {
        // Arrange
        var audioStream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        var castStream = new AudioStreamToStream(audioStream);

        // Act
        _ = castStream.CanWrite;

        // Assert
        _ = audioStream.Received(1).CanWrite;
    }

    [Fact]
    public void LengthCallsSourceAudioStreamLength()
    {
        // Arrange
        var audioStream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        var castStream = new AudioStreamToStream(audioStream);

        // Act
        _ = castStream.Length;

        // Assert
        _ = audioStream.Received(1).Length;
    }

    [Fact]
    public void PositionCallsSourceAudioStreamPosition()
    {
        // Arrange
        var audioStream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        var castStream = new AudioStreamToStream(audioStream)
        {
            Position = 1
        };

        // Act
        _ = castStream.Position;

        // Assert
        audioStream.Received(1).Position = 1;
        _ = audioStream.Received(1).Position;
    }

    [Fact]
    public void SetLengthCallsSourceAudioStreamSetLength()
    {
        // Arrange
        var audioStream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);
        var castStream = new AudioStreamToStream(audioStream);

        // Act
        castStream.SetLength(0);

        // Assert
        audioStream.Received(1).SetLength(0);
    }

    [Fact]
    public void SeekCallsSourceAudioStreamSeek()
    {
        // Arrange
        var audioStream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        var castStream = new AudioStreamToStream(audioStream);

        // Act
        _ = castStream.Seek(0, SeekOrigin.Begin);

        // Assert
        _ = audioStream.Received(1).Seek(0, SeekOrigin.Begin);
    }

    [Fact]
    public void ReadByteCallsAudioStreamReadWithASpanWithLengthEqualsTo1()
    {
        // Arrange
        var totalReadCalls = 0;
        var isValidReadCall = false;

        var audioStream = new AudioStreamMock((buffer) =>
        {
            isValidReadCall = buffer.Length == 1;
            totalReadCalls++;
        });

        var castStream = new AudioStreamToStream(audioStream);

        // Act
        _ = castStream.ReadByte();

        // Assert
        Assert.True(isValidReadCall);
        Assert.Equal(1, totalReadCalls);
    }

    [Fact]
    public void WriteByteCallsAudioStreamReadWithASpanWithLengthEqualsTo1AndSameByteValue()
    {
        // Arrange
        byte expectedByteValue = 123;

        var totalWriteCalls = 0;
        var isValidWriteCall = false;
        byte byteValue = 0;

        var audioStream = new AudioStreamMock((buffer) =>
        {
            isValidWriteCall = buffer.Length == 1;
            byteValue = buffer[0];
            totalWriteCalls++;
        });

        var castStream = new AudioStreamToStream(audioStream);

        // Act
        castStream.WriteByte(expectedByteValue);

        // Assert
        Assert.True(isValidWriteCall);
        Assert.Equal(expectedByteValue, byteValue);
        Assert.Equal(1, totalWriteCalls);
    }

    [Fact]
    public async Task ReadAsyncValueTaskCallsAudioStreamReadAsyncValueTask()
    {
        // Arrange
        var audioStream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        Stream castStream = new AudioStreamToStream(audioStream);

        Memory<byte> expectedMemory = new byte[]
        {
            1,
            2,
            3
        };
        var expectedCancellationToken = CancellationToken.None;

        // Act
        _ = await castStream.ReadAsync(expectedMemory, expectedCancellationToken);

        // Assert
        _ = await audioStream.Received(1).ReadAsync(expectedMemory, expectedCancellationToken);
    }

    [Fact]
    public async Task WriteAsyncValueTaskCallsAudioStreamWriteAsyncValueTask()
    {
        // Arrange
        var audioStream = Substitute.For<AudioStream>(NullLoggerFactory.Instance);

        Stream castStream = new AudioStreamToStream(audioStream);

        Memory<byte> expectedMemory = new byte[]
        {
            1,
            2,
            3
        };
        var expectedCancellationToken = CancellationToken.None;

        // Act
        await castStream.WriteAsync(expectedMemory, expectedCancellationToken);

        // Assert
        await audioStream.Received(1).WriteAsync(expectedMemory, expectedCancellationToken);
    }

    [Fact]
    public void SpanReadCallsAudioStreamSpanRead()
    {
        // Arrange
        var expectedByteArray = new byte[]
        {
            1,
            2,
            3,
        };
        byte[]? byteArray = null;

        var audioStream = new AudioStreamMock((buffer) =>
        {
            byteArray = buffer;
        });

        Stream castStream = new AudioStreamToStream(audioStream);

        Span<byte> spanRead = expectedByteArray;

        // Act
        _ = castStream.Read(spanRead);

        // Assert
        Assert.NotNull(byteArray);
        Assert.Equal(expectedByteArray, byteArray);
    }

    [Fact]
    public void ReadOnlySpanWriteCallsAudioStreamReadOnlySpanWrite()
    {
        // Arrange
        var expectedByteArray = new byte[]
        {
            1,
            2,
            3,
        };
        byte[]? byteArray = null;

        var audioStream = new AudioStreamMock((buffer) =>
        {
            byteArray = buffer;
        });

        Stream castStream = new AudioStreamToStream(audioStream);

        ReadOnlySpan<byte> spanRead = expectedByteArray;

        // Act
        castStream.Write(spanRead);

        // Assert
        Assert.NotNull(byteArray);
        Assert.Equal(expectedByteArray, byteArray);
    }

    [Fact]
    public void DisposeCallsAudioStreamDispose()
    {
        // Arrange
        var disposeCalled = false;
        var disposeCallsCount = 0;

        var audioStream = new AudioStreamMock(() =>
        {
            disposeCalled = true;
            disposeCallsCount++;
        });

        Stream castStream = new AudioStreamToStream(audioStream);

        // Act
        castStream.Dispose();

        // Assert
        Assert.True(disposeCalled);
        Assert.Equal(1, disposeCallsCount);
    }

    [Fact]
    public async Task DisposeAsyncCallsAudioStreamDisposeAsync()
    {
        // Arrange
        var disposeAsyncCalled = false;
        var disposeAsyncCallsCount = 0;

        var audioStream = new AudioStreamMock(() =>
        {
            disposeAsyncCalled = true;
            disposeAsyncCallsCount++;
        });

        Stream castStream = new AudioStreamToStream(audioStream);

        // Act
        await castStream.DisposeAsync();

        // Assert
        Assert.True(disposeAsyncCalled);

        // the count will be equals 3 because DisposeAsync also calls Dispose and the base DisposeAsync
        Assert.Equal(3, disposeAsyncCallsCount);
    }

    private sealed class AudioStreamMock : AudioStream
    {
        private readonly Action<byte[]>? _readWriteCallback;
        private readonly Action? _disposeCallback;

        public AudioStreamMock(Action<byte[]> readWriteCallback)
        {
            _readWriteCallback = readWriteCallback;
        }

        public AudioStreamMock(Action? disposeCallback)
        {
            _disposeCallback = disposeCallback;
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
            _readWriteCallback!(buffer.ToArray());

            return default;
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
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

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            _readWriteCallback!(buffer.ToArray());
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            _disposeCallback?.Invoke();
        }

        protected override ValueTask DisposeAsyncCore()
        {
            _disposeCallback?.Invoke();

            return ValueTask.CompletedTask;
        }
    }
}

