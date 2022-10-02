// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System.Buffers;

namespace NextAudio.Internal;

internal sealed class AudioStreamToStream : Stream
{
    private readonly AudioStream _sourceStream;

    public AudioStreamToStream(AudioStream sourceStream)
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

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _sourceStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _sourceStream.SetLength(value);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return Read(buffer.AsSpan(offset, count));
    }

    public override int Read(Span<byte> buffer)
    {
        return _sourceStream.Read(buffer);
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _sourceStream.ReadAsync(buffer, cancellationToken);
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _sourceStream.ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
    }

    public override int ReadByte()
    {
        var buffer = ArrayPool<byte>.Shared.Rent(1);

        try
        {
            var result = Read(buffer, 0, 1);

            return result <= 0
                ? -1
                : buffer[0];
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return new TaskAsyncResult(ReadAsync(buffer, offset, count, CancellationToken.None), state, callback);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
        return ((Task<int>)asyncResult).GetAwaiter().GetResult();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        Write(buffer.AsSpan(offset, count));
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        _sourceStream.Write(buffer);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _sourceStream.WriteAsync(buffer, cancellationToken);
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return WriteAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
    }

    public override void WriteByte(byte value)
    {
        ReadOnlySpan<byte> buffer = new byte[]
        {
            value
        };

        Write(buffer);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return new TaskAsyncResult(WriteAsync(buffer, offset, count, CancellationToken.None), state, callback);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
        ((Task)asyncResult).GetAwaiter().GetResult();
    }

    public override void Flush()
    {
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _sourceStream.Dispose();
        }
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync().ConfigureAwait(false);
        await _sourceStream.DisposeAsync().ConfigureAwait(false);
    }

    // Extracted from https://github.com/dotnet/runtime/blob/fb5f07f9c580bdcd5d0726d1391c2a52a01030f8/src/libraries/Common/src/System/Threading/Tasks/TaskToApm.cs#L79
    internal sealed class TaskAsyncResult : IAsyncResult
    {
        internal readonly Task _task;
        private readonly AsyncCallback? _callback;

        internal TaskAsyncResult(Task task, object? state, AsyncCallback? callback)
        {
            _task = task;
            AsyncState = state;

            if (task.IsCompleted)
            {
                CompletedSynchronously = true;
                callback?.Invoke(this);
            }
            else if (callback != null)
            {
                _callback = callback;
                _task.ConfigureAwait(false)
                        .GetAwaiter()
                        .OnCompleted(InvokeCallback);
            }
        }

        private void InvokeCallback()
        {
            _callback!.Invoke(this);
        }

        public object? AsyncState { get; }

        public bool CompletedSynchronously { get; }

        public bool IsCompleted => _task.IsCompleted;

        public WaitHandle AsyncWaitHandle => ((IAsyncResult)_task).AsyncWaitHandle;
    }
}
