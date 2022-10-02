// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Utils;

/// <summary>
/// Some <see cref="AudioStream" /> function utilities.
/// </summary>
public static class AudioStreamUtils
{
    /// <summary>
    /// Read the <paramref name="stream" /> until the total bytes reach the <paramref name="buffer" /> length.
    /// </summary>
    /// <param name="stream">The <see cref="AudioStream" /> to be readed.</param>
    /// <param name="buffer">The buffer to be used to read the <paramref name="stream" />.</param>
    /// <returns>The total of bytes readed.</returns>
    public static int ReadFullyAudioStream(AudioStream stream, Span<byte> buffer)
    {
        var totalBytesReaded = 0;
        var bytesReaded = 0;

        do
        {
            bytesReaded = stream.Read(buffer[bytesReaded..]);
            totalBytesReaded += bytesReaded;
        } while (bytesReaded > 0 && totalBytesReaded < buffer.Length);

        return totalBytesReaded;
    }

    /// <summary>
    /// Read the <paramref name="stream" /> until the total bytes reach the <paramref name="buffer" /> length.
    /// </summary>
    /// <param name="stream">The <see cref="AudioStream" /> to be readed.</param>
    /// <param name="buffer">The buffer to be used to read the <paramref name="stream" />.</param>
    /// <returns>A <see cref="ValueTask" /> that represents an that represents an asynchronous operation
    /// where the result is the total of bytes readed.</returns>
    public static async ValueTask<int> ReadFullyAudioStreamAsync(AudioStream stream, Memory<byte> buffer)
    {
        var totalBytesReaded = 0;
        var bytesReaded = 0;

        do
        {
            bytesReaded = await stream.ReadAsync(buffer[bytesReaded..]).ConfigureAwait(false);
            totalBytesReaded += bytesReaded;
        } while (bytesReaded > 0 && totalBytesReaded < buffer.Length);

        return totalBytesReaded;
    }

    /// <summary>
    /// Sums the <paramref name="position" /> and the <paramref name="bytesReaded" />.
    /// </summary>
    /// <param name="position">The position to be summed.</param>
    /// <param name="bytesReaded">The total of bytes readed to be summed.</param>
    /// <returns>The sum of <paramref name="position" /> and <paramref name="bytesReaded" />.</returns>
    public static long ComputePosition(long position, int bytesReaded)
    {
        return position + bytesReaded;
    }
}
