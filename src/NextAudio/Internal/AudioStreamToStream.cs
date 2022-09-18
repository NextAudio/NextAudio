// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System.Buffers;

namespace NextAudio.Internal;

internal sealed class AudioStreamToStream : Stream
{
    private readonly AudioStream _sourceAudioStream;

    public AudioStreamToStream(AudioStream sourceAudioStream)
    {
        _sourceAudioStream = sourceAudioStream;
    }

    public override bool CanRead => _sourceAudioStream.CanRead;

    public override bool CanSeek => _sourceAudioStream.CanSeek;

    public override bool CanWrite => _sourceAudioStream.CanWrite;

    public override long Length => _sourceAudioStream.Length;

    public override long Position
    {
        get => _sourceAudioStream.Position;
        set => _sourceAudioStream.Position = value;
    }

    public override void Flush()
    {
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _sourceAudioStream.Seek(offset, origin);
    }

    public override int ReadByte()
    {
        var arrayBuffer = ArrayPool<byte>.Shared.Rent(1);

        try
        {
            Span<byte> buffer = arrayBuffer;

            var result = Read(buffer[..1]);

            return result <= 0
                ? -1
                : buffer[0];
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(arrayBuffer, true);
        }
    }

    public override void WriteByte(byte value)
    {
        ReadOnlySpan<byte> buffer = new byte[]
        {
            value
        };

        Write(buffer);
    }

    public override int Read(Span<byte> buffer)
    {
        return _sourceAudioStream.Read(buffer);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _sourceAudioStream.Read(buffer, offset, count);
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        return _sourceAudioStream.ReadAsync(buffer, cancellationToken);
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _sourceAudioStream.ReadAsync(buffer, offset, count, cancellationToken).AsTask();
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        _sourceAudioStream.Write(buffer);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _sourceAudioStream.Write(buffer, offset, count);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _sourceAudioStream.WriteAsync(buffer, cancellationToken);
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
    {
        return _sourceAudioStream.WriteAsync(buffer, offset, count, cancellationToken).AsTask();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _sourceAudioStream.Dispose();
        }
    }

    public override ValueTask DisposeAsync()
    {
        return _sourceAudioStream.DisposeAsync();
    }
}
