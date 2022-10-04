// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using NextAudio.Matroska.Internal;
using NextAudio.Matroska.Models;
using NextAudio.Utils;

namespace NextAudio.Matroska;

public partial class MatroskaDemuxer
{
    private MatroskaElement? ReadNextElement(Span<byte> buffer, MatroskaElement? parent = null)
    {
        var result = ElementReader.ReadNextElement(_sourceStream, _position, buffer, _logger, parent);

        _position = result.NewPosition;

        return result.Element;
    }

    private ReadOnlySpan<byte> ReadBytes(MatroskaElement element, Span<byte> buffer)
    {
        _ = ReadSourceStream(buffer[..element.DataSize]);

        var value = buffer[..element.DataSize];

        _logger.LogElementValueReaded(element, value);

        return value;
    }

    private double ReadFloat(MatroskaElement element, Span<byte> buffer)
    {
        _ = ReadSourceStream(buffer[..element.DataSize]);

        var value = EbmlReader.ReadFloat(buffer[..element.DataSize]);

        _logger.LogElementValueReaded(element, value);

        return value;
    }

    private string ReadAsciiString(MatroskaElement element, Span<byte> buffer)
    {
        _ = ReadSourceStream(buffer[..element.DataSize]);

        var value = EbmlReader.ReadAsciiString(buffer[..element.DataSize]).ToString();

        _logger.LogElementValueReaded(element, value);

        return value;
    }

    private string ReadUtf8String(MatroskaElement element, Span<byte> buffer)
    {
        _ = ReadSourceStream(buffer[..element.DataSize]);

        var value = EbmlReader.ReadUtf8String(buffer[..element.DataSize]).ToString();

        _logger.LogElementValueReaded(element, value);

        return value;
    }

    private ulong ReadUlong(MatroskaElement element, Span<byte> buffer)
    {
        _ = ReadSourceStream(buffer[..element.DataSize]);

        var value = EbmlReader.ReadUnsignedInteger(buffer[..element.DataSize]);

        _logger.LogElementValueReaded(element, value);

        return value;
    }

    private void SkipElement(MatroskaElement element)
    {
        _ = InternalSeek(element.GetRemaining(_position), SeekOrigin.Current);
    }

    private int ReadSourceStream(Span<byte> buffer)
    {
        var result = AudioStreamUtils.ReadFullyAudioStream(_sourceStream, buffer);

        _position += result;

        return result;
    }

    private long InternalSeek(long offset, SeekOrigin origin)
    {
        var result = AudioStreamUtils.Seek(_sourceStream, offset, origin, _position);

        _position = result;

        return result;
    }
}
