// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Utils;

internal static class StreamUtils
{
    public static int ReadStream(AudioStream stream, Span<byte> buffer)
    {
        var bytesReaded = 0;

        do
        {
            bytesReaded += stream.Read(buffer[bytesReaded..]);
        } while (bytesReaded < buffer.Length);

        return bytesReaded;
    }

    public static async ValueTask<int> ReadStreamAsync(AudioStream stream, Memory<byte> buffer)
    {
        var bytesReaded = 0;

        do
        {
            bytesReaded += await stream.ReadAsync(buffer[bytesReaded..]).ConfigureAwait(false);
        } while (bytesReaded < buffer.Length);

        return bytesReaded;
    }

    public static long ComputePosition(long position, int bytesReaded)
    {
        return position + bytesReaded;
    }
}
