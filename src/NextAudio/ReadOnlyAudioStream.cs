// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace NextAudio;

/// <summary>
/// Represents a readonly audio stream.
/// </summary>
public abstract class ReadOnlyAudioStream : AudioStream
{
    /// <summary>
    /// Creates an instance of <see cref="ReadOnlyAudioStream" />.
    /// </summary>
    /// <param name="loggerFactory">A logger factory to log audio streaming info.</param>
    protected ReadOnlyAudioStream(ILoggerFactory? loggerFactory = null) : base(loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override bool CanRead => true;

    /// <inheritdoc/>
    public override bool CanWrite => false;

    /// <inheritdoc/>
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public override void Write(ReadOnlySpan<byte> buffer)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }
}
