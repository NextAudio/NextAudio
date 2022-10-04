// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using NextAudio.Matroska.Models;

namespace NextAudio.Matroska;

internal static partial class MatroskaLogging
{
    private static readonly Func<ILogger, string, long, IDisposable> _processingMasterElementScope
        = LoggerMessage.DefineScope<string, long>("Processing master element '{string}' in position '{position}'.");

    public static IDisposable ProcessingMasterElementScope(this ILogger logger, MatroskaElement element, long position)
    {
        return _processingMasterElementScope(logger, element.Type.ToString(), position);
    }

    public static void LogReadBufferSize(this ILogger logger, ReadOnlySpan<byte> buffer)
    {
        if (logger.IsEnabled(LogLevel.Warning) && buffer.Length < 1024)
        {
            LogReadBufferSize(logger);
        }
    }

    public static void LogBlockParsed(this ILogger logger, MatroskaBlock block)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            LogBlockParsed(logger, block.TrackNumber, block.FrameCount, block.LacingType, block.Element.Position);
        }
    }

    public static void LogElementReaded(this ILogger logger, MatroskaElement element)
    {
        LogElementReaded(logger, element.Id, element.DataSize);
    }

    public static void LogElementValueReaded<T>(this ILogger logger, MatroskaElement element, T value)
    {
        if (logger.IsEnabled(LogLevel.Trace) && value != null)
        {
            var strValue = value.ToString();

            if (strValue == null)
            {
                return;
            }

            LogElementValueReaded(logger, element.Id, strValue);
        }
    }

    public static void LogElementValueReaded(this ILogger logger, MatroskaElement element, ReadOnlySpan<byte> value)
    {
        if (logger.IsEnabled(LogLevel.Trace) && !value.IsEmpty)
        {
            var strValue = $"{value.Length} bytes";

            LogElementValueReaded(logger, element.Id, strValue);
        }
    }

    public static void LogVIntReaded(this ILogger logger, VInt vInt, long position)
    {
        LogVIntReaded(logger, vInt.EncodedValue, vInt.Value, position);
    }


    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, SkipEnabledCheck = true, Message = "Buffer's length used to read is lower than recomended (1024).")]
    private static partial void LogReadBufferSize(this ILogger logger);

    [LoggerMessage(EventId = 2, Level = LogLevel.Trace, Message = "Readed matroska element of ID 'Ox{Id:X}' with '{Length}' bytes of length.")]
    public static partial void LogElementReaded(this ILogger logger, ulong id, int length);

    [LoggerMessage(EventId = 3, Level = LogLevel.Trace, SkipEnabledCheck = true, Message = "Readed value for element of ID '0x{Id:X}' and value '{Value}'.")]
    private static partial void LogElementValueReaded(this ILogger logger, ulong id, string value);

    [LoggerMessage(EventId = 4, Level = LogLevel.Trace, Message = "Readed EBML variable size integer of ID 'Ox{Id:X}' and value '{Value}' at position '{Position}'.")]
    public static partial void LogVIntReaded(this ILogger logger, ulong id, ulong value, long position);

    [LoggerMessage(EventId = 5, Level = LogLevel.Debug, Message = "Selected matroska track of number '{TrackNumber}'.")]
    public static partial void LogTrackSelected(this ILogger logger, ulong trackNumber);

    [LoggerMessage(EventId = 6, Level = LogLevel.Debug, SkipEnabledCheck = true, Message = "Parsed matroska block of track number '{TrackNumber}' and with lacing type '{LacingType}' and frame count '{FrameCount}' at position '{Position}'.")]
    public static partial void LogBlockParsed(this ILogger logger, ulong trackNumber, int frameCount, MatroskaBlockLacingType lacingType, long position);

    [LoggerMessage(EventId = 7, Level = LogLevel.Debug, Message = "Readed audio frame of size '{FrameSize}' in index '{Index}' at position '{Position}'.")]
    public static partial void LogFrameReaded(this ILogger logger, int frameSize, int index, long position);
}
