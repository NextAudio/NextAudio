// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using NextAudio.Matroska.Models;

namespace NextAudio.Matroska.Internal;

internal static class ElementReader
{
    public static ElementReadResult ReadNextElement(AudioStream stream, long position, Span<byte> buffer, ILogger logger, MatroskaElement? parent = null)
    {
        if (parent.HasValue && parent.Value.GetRemaining(position) <= 0)
        {
            return new ElementReadResult(null, position);
        }

        var initialPosition = position;

        var elementId = VIntReader.ReadVInt(stream, buffer, position, logger);
        position += elementId.Length;

        var elementSize = VIntReader.ReadVInt(stream, buffer[elementId.Length..], position, logger);
        position += elementSize.Length;

        var headerSize = position - initialPosition;
        var depth = parent.HasValue ? parent.Value.Depth + 1 : 0;

        var element = new MatroskaElement(elementId.EncodedValue, depth, initialPosition, (int)headerSize, (int)elementSize.Value);

        logger.LogElementReaded(element);

        return new ElementReadResult(element, position);
    }

    public static async ValueTask<ElementReadResult> ReadNextElementAsync(AudioStream stream, long position, Memory<byte> buffer, ILogger logger, MatroskaElement? parent = null)
    {
        if (parent.HasValue && parent.Value.GetRemaining(position) <= 0)
        {
            return new ElementReadResult(null, position);
        }

        var initialPosition = position;

        var elementId = await VIntReader.ReadVIntAsync(stream, buffer, position, logger);
        position += elementId.Length;

        var elementSize = await VIntReader.ReadVIntAsync(stream, buffer[elementId.Length..], position, logger);
        position += elementSize.Length;

        var headerSize = position - initialPosition;
        var depth = parent.HasValue ? parent.Value.Depth + 1 : 0;

        var element = new MatroskaElement(elementId.EncodedValue, depth, initialPosition, (int)headerSize, (int)elementSize.Value);

        logger.LogElementReaded(element);

        return new ElementReadResult(element, position);
    }

    internal readonly struct ElementReadResult
    {
        public ElementReadResult(MatroskaElement? element, long newPosition)
        {
            Element = element;
            NewPosition = newPosition;
        }

        public MatroskaElement? Element { get; }

        public long NewPosition { get; }
    }
}
