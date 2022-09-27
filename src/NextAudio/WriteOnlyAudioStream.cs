// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace NextAudio;

/// <inheritdoc />
public abstract class WriteOnlyAudioStream : AudioStream
{
    /// <summary>
    /// Creates an instance of <see cref="WriteOnlyAudioStream" />.
    /// </summary>
    /// <param name="loggerFactory">A logger factory to log audio streaming info.</param>
    protected WriteOnlyAudioStream(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }

    /// <summary>
    /// Creates an instance of <see cref="WriteOnlyAudioStream" />.
    /// </summary>
    protected WriteOnlyAudioStream()
    {
    }

    /// <inheritdoc />
    public override bool CanRead => false;

    /// <inheritdoc />
    public override bool CanWrite => true;

    /// <inheritdoc />
    public override int Read(Span<byte> buffer)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }
}
