// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Internal;

internal sealed class StreamToAudioStream : AudioStream
{
    private readonly Stream _sourceStream;

    public StreamToAudioStream(Stream sourceStream)
    {
        _sourceStream = sourceStream;
    }

    public override bool CanRead => _sourceStream.CanRead;

    public override bool CanSeek => _sourceStream.CanSeek;

    public override bool CanWrite => _sourceStream.CanWrite;

    public override long Length => _sourceStream.Length;

    public override long Position
    {
        get => _sourceStream.Position;
        set => _sourceStream.Position = value;
    }

    public override RecommendedSynchronicity RecommendedSynchronicity => RecommendedSynchronicity.Any;

    public override StreamToAudioStream Clone()
    {
        return new StreamToAudioStream(_sourceStream);
    }

    public override int Read(Span<byte> buffer)
    {
        return _sourceStream.Read(buffer);
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _sourceStream.ReadAsync(buffer, cancellationToken);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _sourceStream.Seek(offset, origin);
    }

    public override ValueTask<long> SeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken = default)
    {
        return cancellationToken.IsCancellationRequested
            ? ValueTask.FromCanceled<long>(cancellationToken)
            : ValueTask.FromResult(Seek(offset, origin));
    }

    public override void SetLength(long value)
    {
        _sourceStream.SetLength(value);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        _sourceStream.Write(buffer);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _sourceStream.WriteAsync(buffer, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            _sourceStream.Dispose();
        }
    }

    protected override ValueTask DisposeAsyncCore()
    {
        return IsDisposed ? ValueTask.CompletedTask : _sourceStream.DisposeAsync();
    }
}
