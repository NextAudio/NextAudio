// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using NextAudio.Matroska.Models;
using NextAudio.Utils;

namespace NextAudio.Matroska.Internal;

internal static class BlockReader
{
    public static MatroskaBlockResult ReadBlock(AudioStream stream, Span<byte> buffer, MatroskaElement blockElement, long position, ulong selectedTrackNumber, ILogger logger)
    {
        var vInt = VIntReader.ReadVInt(stream, buffer, position, logger);

        position += vInt.Length;

        var trackNumber = vInt.Value;

        if (trackNumber != selectedTrackNumber)
        {
            return new MatroskaBlockResult(null, position);
        }

        position = AudioStreamUtils.Seek(stream, 2, SeekOrigin.Current, position);

        position += AudioStreamUtils.ReadFullyAudioStream(stream, buffer[vInt.Length..(vInt.Length + 1)]);

        var flags = buffer[vInt.Length];
        var lacingType = (MatroskaBlockLacingType)(flags & 0b0000110);

        int frameCount;
        int[] frameSizes;

        if (lacingType != MatroskaBlockLacingType.No)
        {
            position += AudioStreamUtils.ReadFullyAudioStream(stream, buffer[(vInt.Length + 1)..(vInt.Length + 2)]);
            frameCount = (buffer[vInt.Length + 1] & 0xff) + 1;
        }
        else
        {
            frameCount = 1;
        }

        frameSizes = new int[frameCount];

        switch (lacingType)
        {
            default:
            case MatroskaBlockLacingType.No:
                frameSizes[0] = (int)blockElement.GetRemaining(position);
                break;

            case MatroskaBlockLacingType.Xiph:
                position = ParseXiphLacing(stream, buffer[(vInt.Length + 2)..], position, blockElement, frameSizes);
                break;
            case MatroskaBlockLacingType.FixedSize:
                ParseFixedSizeLacing(position, blockElement, frameSizes);
                break;
            case MatroskaBlockLacingType.Ebml:
                position = ParseEbmlLacing(stream, buffer[(vInt.Length + 2)..], position, blockElement, frameSizes, logger);
                break;
        }

        var block = new MatroskaBlock(blockElement, trackNumber, lacingType, frameCount, frameSizes);

        logger.LogBlockParsed(block);

        return new MatroskaBlockResult(block, position);
    }

    public static async ValueTask<MatroskaBlockResult> ReadBlockAsync(AudioStream stream, Memory<byte> buffer, MatroskaElement blockElement, long position, ulong selectedTrackNumber, ILogger logger)
    {
        var vInt = await VIntReader.ReadVIntAsync(stream, buffer, position, logger).ConfigureAwait(false);

        position += vInt.Length;

        var trackNumber = vInt.Value;

        if (trackNumber != selectedTrackNumber)
        {
            return new MatroskaBlockResult(null, position);
        }

        position = await AudioStreamUtils.SeekAsync(stream, 2, SeekOrigin.Current, position).ConfigureAwait(false);

        position += await AudioStreamUtils.ReadFullyAudioStreamAsync(stream, buffer[vInt.Length..(vInt.Length + 1)]).ConfigureAwait(false);

        var flags = buffer.Span[vInt.Length];
        var lacingType = (MatroskaBlockLacingType)(flags & 0b0000110);

        int frameCount;
        int[] frameSizes;

        if (lacingType != MatroskaBlockLacingType.No)
        {
            position += await AudioStreamUtils.ReadFullyAudioStreamAsync(stream, buffer[(vInt.Length + 1)..(vInt.Length + 2)]).ConfigureAwait(false);
            frameCount = (buffer.Span[vInt.Length + 1] & 0xff) + 1;
        }
        else
        {
            frameCount = 1;
        }

        frameSizes = new int[frameCount];

        switch (lacingType)
        {
            default:
            case MatroskaBlockLacingType.No:
                frameSizes[0] = (int)blockElement.GetRemaining(position);
                break;

            case MatroskaBlockLacingType.Xiph:
                position = await ParseXiphLacingAsync(stream, buffer[(vInt.Length + 2)..], position, blockElement, frameSizes).ConfigureAwait(false);
                break;
            case MatroskaBlockLacingType.FixedSize:
                ParseFixedSizeLacing(position, blockElement, frameSizes);
                break;
            case MatroskaBlockLacingType.Ebml:
                position = await ParseEbmlLacingAsync(stream, buffer[(vInt.Length + 2)..], position, blockElement, frameSizes, logger).ConfigureAwait(false);
                break;
        }

        var block = new MatroskaBlock(blockElement, trackNumber, lacingType, frameCount, frameSizes);

        logger.LogBlockParsed(block);

        return new MatroskaBlockResult(block, position);
    }

    private static long ParseXiphLacing(AudioStream stream, Span<byte> buffer, long position, MatroskaElement blockElement, Span<int> frameSizes)
    {
        var totalSize = 0;
        var bufferIndex = 0;

        for (var i = 0; i < frameSizes.Length - 1; i++)
        {
            var value = 0;

            do
            {
                position += AudioStreamUtils.ReadFullyAudioStream(stream, buffer[bufferIndex..(bufferIndex + 1)]);
                value += buffer[bufferIndex] & 0xff;
            } while (value == 255);

            frameSizes[i] = value;
            totalSize += value;
        }

        var remaining = blockElement.GetRemaining(position);
        frameSizes[^1] = (int)remaining - totalSize;

        return position;
    }

    private static async ValueTask<long> ParseXiphLacingAsync(AudioStream stream, Memory<byte> buffer, long position, MatroskaElement blockElement, Memory<int> frameSizes)
    {
        var totalSize = 0;
        var bufferIndex = 0;

        for (var i = 0; i < frameSizes.Length - 1; i++)
        {
            var value = 0;

            do
            {
                position += await AudioStreamUtils.ReadFullyAudioStreamAsync(stream, buffer[bufferIndex..(bufferIndex + 1)]).ConfigureAwait(false);
                value += buffer.Span[bufferIndex] & 0xff;
            } while (value == 255);

            frameSizes.Span[i] = value;
            totalSize += value;
        }

        var remaining = blockElement.GetRemaining(position);
        frameSizes.Span[^1] = (int)remaining - totalSize;

        return position;
    }

    private static void ParseFixedSizeLacing(long position, MatroskaElement blockElement, Span<int> frameSizes)
    {
        var size = (int)(blockElement.GetRemaining(position) / frameSizes.Length);

        for (var i = 0; i < frameSizes.Length; i++)
        {
            frameSizes[i] = size;
        }
    }

    private static long ParseEbmlLacing(AudioStream stream, Span<byte> buffer, long position, MatroskaElement blockElement, Span<int> frameSizes, ILogger logger)
    {
        var vInt = VIntReader.ReadVInt(stream, buffer, position, logger);

        position += vInt.Length;

        frameSizes[0] = (int)vInt.Value;

        var totalVIntLength = vInt.Length;
        var totalSize = frameSizes[0];

        for (var i = 1; i < frameSizes.Length - 1; i++)
        {
            vInt = VIntReader.ReadVInt(stream, buffer[totalVIntLength..], position, logger);

            position += vInt.Length;

            frameSizes[i] = frameSizes[i - 1] + (int)ParseEbmlLaceSignedInteger(vInt);

            totalVIntLength += vInt.Length;
            totalSize += frameSizes[i];
        }

        frameSizes[^1] = (int)blockElement.GetRemaining(position) - totalSize;

        return position;
    }

    private static async ValueTask<long> ParseEbmlLacingAsync(AudioStream stream, Memory<byte> buffer, long position, MatroskaElement blockElement, Memory<int> frameSizes, ILogger logger)
    {
        var vInt = await VIntReader.ReadVIntAsync(stream, buffer, position, logger).ConfigureAwait(false);

        position += vInt.Length;

        frameSizes.Span[0] = (int)vInt.Value;

        var totalVIntLength = vInt.Length;
        var totalSize = frameSizes.Span[0];

        for (var i = 1; i < frameSizes.Length - 1; i++)
        {
            vInt = await VIntReader.ReadVIntAsync(stream, buffer[totalVIntLength..], position, logger).ConfigureAwait(false);

            position += vInt.Length;

            frameSizes.Span[i] = frameSizes.Span[i - 1] + (int)ParseEbmlLaceSignedInteger(vInt);

            totalVIntLength += vInt.Length;
            totalSize += frameSizes.Span[i];
        }

        frameSizes.Span[^1] = (int)blockElement.GetRemaining(position) - totalSize;

        return position;
    }

    private static ulong ParseEbmlLaceSignedInteger(VInt vInt)
    {
        return vInt.Length switch
        {
            1 => vInt.Value - 63,
            2 => vInt.Value - 8191,
            3 => vInt.Value - 1048575,
            4 => vInt.Value - 134217727,
            _ => throw new InvalidOperationException("An Ebml Lace Signed Integer cannot have length higher than 4 bytes."),
        };
    }

    public readonly struct MatroskaBlockResult
    {
        public MatroskaBlockResult(MatroskaBlock? block, long newPosition)
        {
            Block = block;
            NewPosition = newPosition;
        }

        public MatroskaBlock? Block { get; }

        public long NewPosition { get; }
    }
}
