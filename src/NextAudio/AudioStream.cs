// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NextAudio.Internal;

namespace NextAudio;

/// <summary>
/// Represents an audio stream.
/// </summary>
public abstract class AudioStream : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// A logger factory to log audio streaming info.
    /// </summary>
    protected readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Creates an instance of <see cref="AudioStream" />
    /// </summary>
    /// <param name="loggerFactory">A logger factory to log audio streaming info.</param>
    protected AudioStream(ILoggerFactory? loggerFactory = null)
    {
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
    }

    ///
    ~AudioStream()
    {
        Dispose(false);
    }

    /// <summary>
    /// Indicates if this stream was disposed.
    /// </summary>
    protected bool IsDisposed { get; private set; }

    /// <inheritdoc cref="Stream.CanRead" />
    public abstract bool CanRead { get; }

    /// <inheritdoc cref="Stream.CanSeek" />
    public abstract bool CanSeek { get; }

    /// <inheritdoc cref="Stream.CanWrite" />
    public abstract bool CanWrite { get; }

    /// <inheritdoc cref="Stream.Length" />
    public abstract long Length { get; }

    /// <inheritdoc cref="Stream.Position" />
    public abstract long Position { get; set; }

    /// <summary>
    /// Creates a clone of this stream.
    /// </summary>
    /// <returns>A new cloned stream.</returns>
    public abstract AudioStream Clone();

    /// <inheritdoc cref="Stream.Seek(long, SeekOrigin)" />
    public abstract long Seek(long offset, SeekOrigin origin);

    /// <inheritdoc cref="Stream.Read(Span{byte})" />
    public abstract int Read(Span<byte> buffer);

    /// <inheritdoc cref="Stream.Write(ReadOnlySpan{byte})" />
    public abstract void Write(ReadOnlySpan<byte> buffer);

    /// <inheritdoc cref="Stream.Read(byte[], int, int)" />
    public virtual int Read(byte[] buffer, int offset, int count)
    {
        return Read(new(buffer, offset, count));
    }

    /// <inheritdoc cref="Stream.Write(byte[], int, int)" />
    public virtual void Write(byte[] buffer, int offset, int count)
    {
        Write(new(buffer, offset, count));
    }

    /// <inheritdoc cref="Stream.ReadAsync(Memory{byte}, CancellationToken)" />
    public abstract ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously reads a sequence of bytes from the current stream, advances the
    /// position within the stream by the number of bytes read, and monitors cancellation
    /// requests.
    /// </summary>
    /// <param name="buffer">The buffer to write the data into.</param>
    /// <param name="offset">The byte offset in buffer at which to begin writing data from the stream.</param>
    /// <param name="count">The maximum number of bytes to read.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None" />.</param>
    /// <returns>
    /// A <see cref="Task" /> that represents the asynchronous read operation. The value of the TResult
    /// parameter contains the total number of bytes read into the buffer. The result
    /// value can be less than the number of bytes requested if the number of bytes currently
    /// available is less than the requested number, or it can be 0 (zero) if the end
    /// of the stream has been reached.
    /// </returns>
    /// <exception cref="ArgumentNullException">buffer is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">offset or count is negative.</exception>
    /// <exception cref="ArgumentException">The sum of offset and count is larger than the buffer length.</exception>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
    /// <exception cref="InvalidOperationException">The stream is currently in use by a previous read operation.</exception>
    public virtual ValueTask<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
    {
        return ReadAsync(new(buffer, offset, count), cancellationToken);
    }

    /// <inheritdoc cref="Stream.WriteAsync(ReadOnlyMemory{byte}, CancellationToken)" />
    public abstract ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously writes a sequence of bytes to the current stream, advances the
    /// current position within this stream by the number of bytes written, and monitors
    /// cancellation requests.
    /// </summary>
    /// <param name="buffer">The buffer to write data from.</param>
    /// <param name="offset">The zero-based byte offset in buffer from which to begin copying bytes to the stream.</param>
    /// <param name="count">The maximum number of bytes to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None" />.</param>
    /// <returns>A <see cref="ValueTask" /> that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentNullException">buffer is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">offset or count is negative.</exception>
    /// <exception cref="ArgumentException">The sum of offset and count is larger than the buffer length.</exception>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
    /// <exception cref="InvalidOperationException">The stream is currently in use by a previous read operation.</exception>
    public virtual ValueTask WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
    {
        return WriteAsync(new(buffer, offset, count), cancellationToken);
    }

    /// <inheritdoc cref="Stream.Dispose(bool)" />
    protected abstract void Dispose(bool disposing);

    /// <summary>
    /// Asynchronously releases the unmanaged resources.
    /// </summary>
    /// <returns>A <see cref="ValueTask" /> that represents the asynchronous dispose operation.</returns>
    protected abstract ValueTask DisposeAsyncCore();

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(false);

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Implicit cast a <see cref="Stream" /> in an <see cref="AudioStream "/>
    /// </summary>
    /// <param name="audioStream">The <see cref="AudioStream" /> to be cast.</param>
    public static implicit operator Stream(AudioStream audioStream)
    {
        return new AudioStreamToStream(audioStream);
    }
}
