// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System.Buffers;

namespace NextAudio.Utils;

/// <summary>
/// Some <see cref="AudioStream" /> function utilities.
/// </summary>
public static class AudioStreamUtils
{
    /// <summary>
    /// Read the <paramref name="stream" /> until the total bytes reach the <paramref name="buffer" /> length.
    /// </summary>
    /// <param name="stream">The <see cref="AudioStream" /> to be read.</param>
    /// <param name="buffer">The buffer to be used to read the <paramref name="stream" />.</param>
    /// <returns>The total of bytes read.</returns>
    public static int ReadFullyAudioStream(AudioStream stream, Span<byte> buffer)
    {
        var totalBytesRead = 0;
        var bytesRead = 0;

        do
        {
            bytesRead = stream.Read(buffer[bytesRead..]);
            totalBytesRead += bytesRead;
        } while (bytesRead > 0 && totalBytesRead < buffer.Length);

        return totalBytesRead;
    }

    /// <summary>
    /// Read the <paramref name="stream" /> until the total bytes reach the <paramref name="buffer" /> length.
    /// </summary>
    /// <param name="stream">The <see cref="AudioStream" /> to be read.</param>
    /// <param name="buffer">The buffer to be used to read the <paramref name="stream" />.</param>
    /// <returns>A <see cref="ValueTask" /> that represents an asynchronous operation
    /// where the result is the total of bytes read.</returns>
    public static async ValueTask<int> ReadFullyAudioStreamAsync(AudioStream stream, Memory<byte> buffer)
    {
        var totalBytesRead = 0;
        var bytesRead = 0;

        do
        {
            bytesRead = await stream.ReadAsync(buffer[bytesRead..]).ConfigureAwait(false);
            totalBytesRead += bytesRead;
        } while (bytesRead > 0 && totalBytesRead < buffer.Length);

        return totalBytesRead;
    }

    /// <summary>
    /// Seek the <see cref="AudioStream" /> if the stream doesn't support seek, try forcefully seek.
    /// </summary>
    /// <param name="stream">The <see cref="AudioStream" /> to be seeked.</param>
    /// <param name="offset">A byte offset relative to the <paramref name="origin" />.</param>
    /// <param name="origin">A value of type <see cref="SeekOrigin" /> indicating the reference point used
    /// to obtain the new position.</param>
    /// <param name="position">The current <paramref name="stream" /> position.</param>
    /// <returns>The new position within the current <paramref name="stream" />.</returns>
    /// <exception cref="InvalidOperationException">Unknown <paramref name="origin" /> value or
    /// the sum of <paramref name="offset" /> and <paramref name="position" /> is higher than
    /// <see cref="int.MaxValue" />.
    /// </exception>
    /// <exception cref="NotSupportedException">Cannot seek the stream even forcefully.</exception>
    public static long Seek(AudioStream stream, long offset, SeekOrigin origin, long position)
    {
        if (stream.CanSeek)
        {
            return stream.Seek(offset, origin);
        }

        var pos = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.End => stream.Length + offset, // Should throw NotSupportedException if not we can continue.
            SeekOrigin.Current => position + offset,
            _ => throw new InvalidOperationException($"Unknown SeekOrigin enum value: '{origin}'.")
        };

        if (pos < position)
        {
            throw new NotSupportedException("The source stream cannot be seeked, you only can seek forward.");
        }

        if (pos > int.MaxValue)
        {
            throw new InvalidOperationException("Cannot seek more than the Int32 limit.");
        }

        if (pos == position)
        {
            return position;
        }

        var numberOfBytesToSkip = (int)(pos - position);

        var skipBuffer = ArrayPool<byte>.Shared.Rent(numberOfBytesToSkip);

        try
        {
            position += ReadFullyAudioStream(stream, skipBuffer.AsSpan(0, numberOfBytesToSkip));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(skipBuffer);
        }

        return position;
    }

    /// <summary>
    /// Seek the <see cref="AudioStream" /> if the stream doesn't support seek, try forcefully seek.
    /// </summary>
    /// <param name="stream">The <see cref="AudioStream" /> to be seeked.</param>
    /// <param name="offset">A byte offset relative to the <paramref name="origin" />.</param>
    /// <param name="origin">A value of type <see cref="SeekOrigin" /> indicating the reference point used
    /// to obtain the new position.</param>
    /// <param name="position">The current <paramref name="stream" /> position.</param>
    /// <returns>
    /// A <see cref="ValueTask" /> that represents an asynchronous operation
    /// where the result is Tthe new position within the current <paramref name="stream" />.
    /// </returns>
    /// <exception cref="InvalidOperationException">Unknown <paramref name="origin" /> value or
    /// the sum of <paramref name="offset" /> and <paramref name="position" /> is higher than
    /// <see cref="int.MaxValue" />.
    /// </exception>
    /// <exception cref="NotSupportedException">Cannot seek the stream even forcefully.</exception>
    public static async ValueTask<long> SeekAsync(AudioStream stream, long offset, SeekOrigin origin, long position)
    {
        if (stream.CanSeek)
        {
            return await stream.SeekAsync(offset, origin).ConfigureAwait(false);
        }

        var pos = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.End => stream.Length + offset, // Should throw NotSupportedException if not we can continue.
            SeekOrigin.Current => position + offset,
            _ => throw new InvalidOperationException($"Unknown SeekOrigin enum value: '{origin}'.")
        };

        if (pos < position)
        {
            throw new NotSupportedException("The source stream cannot be seeked, you only can seek forward.");
        }

        if (pos > int.MaxValue)
        {
            throw new InvalidOperationException("Cannot seek more than the Int32 limit.");
        }

        if (pos == position)
        {
            return position;
        }

        var numberOfBytesToSkip = (int)(pos - position);

        var skipBuffer = ArrayPool<byte>.Shared.Rent(numberOfBytesToSkip);

        try
        {
            position += await ReadFullyAudioStreamAsync(stream, skipBuffer.AsMemory(0, numberOfBytesToSkip)).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(skipBuffer);
        }

        return position;
    }
}
