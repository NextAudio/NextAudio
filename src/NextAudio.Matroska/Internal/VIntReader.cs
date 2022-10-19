// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using NextAudio.Matroska.Models;
using NextAudio.Utils;

namespace NextAudio.Matroska.Internal;

internal static class VIntReader
{
    public static VInt ReadVInt(AudioStream stream, Span<byte> buffer, long position, ILogger logger)
    {
        _ = AudioStreamUtils.ReadFullyAudioStream(stream, buffer[..1]);

        var length = EbmlReader.ReadVariableSizeIntegerLength(buffer[0]);

        if (length > 1)
        {
            _ = AudioStreamUtils.ReadFullyAudioStream(stream, buffer[1..length]);
        }

        var vInt = EbmlReader.ReadVariableSizeInteger(buffer[..length], length);

        logger.LogVIntRead(vInt, position + vInt.Length);

        return vInt;
    }

    public static async ValueTask<VInt> ReadVIntAsync(AudioStream stream, Memory<byte> buffer, long position, ILogger logger)
    {
        _ = await AudioStreamUtils.ReadFullyAudioStreamAsync(stream, buffer[..1]).ConfigureAwait(false);

        var length = EbmlReader.ReadVariableSizeIntegerLength(buffer.Span[0]);

        if (length > 1)
        {
            _ = await AudioStreamUtils.ReadFullyAudioStreamAsync(stream, buffer[1..length]).ConfigureAwait(false);
        }

        var vInt = EbmlReader.ReadVariableSizeInteger(buffer.Span[..length], length);

        logger.LogVIntRead(vInt, position + vInt.Length);

        return vInt;
    }
}
