// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Utils;

internal static class StreamUtils
{
    public static int ReadFullyStream(AudioStream stream, Span<byte> buffer)
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

    public static async ValueTask<int> ReadFullyStreamAsync(AudioStream stream, Memory<byte> buffer)
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

    public static long ComputePosition(long position, int bytesReaded)
    {
        return position + bytesReaded;
    }
}
