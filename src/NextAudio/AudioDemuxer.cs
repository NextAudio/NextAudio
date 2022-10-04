// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace NextAudio;

/// <summary>
/// Represents an Audio demuxer.
/// </summary>
public abstract class AudioDemuxer : ReadOnlyAudioStream
{
    /// <summary>
    /// Creates an instance of <see cref="AudioDemuxer" />.
    /// </summary>
    /// <param name="loggerFactory">A logger factory to log audio streaming info.</param>
    protected AudioDemuxer(ILoggerFactory? loggerFactory = null) : base(loggerFactory)
    {
    }

    // The majority of the demuxers will have broken state if
    // Seek without a specific unit.
    /// <inheritdoc />
    public override bool CanSeek => false;

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    // The majority of the demuxers will have broken state if
    // Seek without a specific unit.
    /// <inheritdoc />
    public override ValueTask<long> SeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override int Read(Span<byte> buffer)
    {
        return Demux(buffer);
    }

    /// <inheritdoc />
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return DemuxAsync(buffer, cancellationToken);
    }

    /// <summary>
    /// Demux the audio writing to the <paramref name="buffer" />.
    /// </summary>
    /// <param name="buffer">The buffer to write the demuxed audio.</param>
    /// <returns>The number of bytes written to the <paramref name="buffer" />.</returns>
    public abstract int Demux(Span<byte> buffer);

    /// <summary>
    /// Demux the audio writing to the <paramref name="buffer" />.
    /// </summary>
    /// <param name="buffer">The input buffer to write the demuxed audio.</param>
    /// <param name="offset">The offset position to use when writing to the <paramref name="buffer" />.</param>
    /// <param name="count">The max number of dumexed audio bytes to write in the <paramref name="buffer" />.</param>
    /// <returns>The number of bytes written to the <paramref name="buffer" />.</returns>
    public virtual int Demux(byte[] buffer, int offset, int count)
    {
        return Demux(new(buffer, offset, count));
    }

    /// <summary>
    /// Asynchronously demux the audio writing to the <paramref name="buffer" />.
    /// </summary>
    /// <param name="buffer">The input buffer to write the demuxed audio.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The number of bytes written to the <paramref name="buffer" />.</returns>
    public abstract ValueTask<int> DemuxAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously demux the audio writing to the <paramref name="buffer" />.
    /// </summary>
    /// <param name="buffer">The input buffer to write the demuxed audio.</param>
    /// <param name="offset">The offset position to use when writing to the <paramref name="buffer" />.</param>
    /// <param name="count">The max number of dumexed audio bytes to write in the <paramref name="buffer" />.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The number of bytes written to the <paramref name="buffer" />.</returns>
    public virtual ValueTask<int> DemuxAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
    {
        return DemuxAsync(new(buffer, offset, count), cancellationToken);
    }
}
