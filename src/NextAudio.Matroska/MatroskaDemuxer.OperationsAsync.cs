// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using NextAudio.Matroska.Internal;
using NextAudio.Matroska.Models;
using NextAudio.Utils;

namespace NextAudio.Matroska;

public partial class MatroskaDemuxer
{
    private async ValueTask<MatroskaElement?> ReadNextElementAsync(Memory<byte> buffer, MatroskaElement? parent = null, CancellationToken cancellationToken = default)
    {
        var result = await ElementReader.ReadNextElementAsync(_sourceStream, _position, buffer, _logger, parent, cancellationToken).ConfigureAwait(false);

        _position = result.NewPosition;

        return result.Element;
    }

    private async ValueTask<ReadOnlyMemory<byte>> ReadBytesAsync(MatroskaElement element, Memory<byte> buffer, CancellationToken cancellationToken)
    {
        _ = await ReadSourceStreamAsync(buffer[..element.DataSize], cancellationToken).ConfigureAwait(false);

        var value = buffer[..element.DataSize];

        _logger.LogElementValueRead(element, value);

        return value;
    }

    private async ValueTask<double> ReadFloatAsync(MatroskaElement element, Memory<byte> buffer, CancellationToken cancellationToken)
    {
        _ = await ReadSourceStreamAsync(buffer[..element.DataSize], cancellationToken).ConfigureAwait(false);

        var value = EbmlReader.ReadFloat(buffer.Span[..element.DataSize]);

        _logger.LogElementValueRead(element, value);

        return value;
    }

    private async ValueTask<string> ReadAsciiStringAsync(MatroskaElement element, Memory<byte> buffer, CancellationToken cancellationToken)
    {
        _ = await ReadSourceStreamAsync(buffer[..element.DataSize], cancellationToken).ConfigureAwait(false);

        var value = EbmlReader.ReadAsciiString(buffer.Span[..element.DataSize]).ToString();

        _logger.LogElementValueRead(element, value);

        return value;
    }

    private async ValueTask<string> ReadUtf8StringAsync(MatroskaElement element, Memory<byte> buffer, CancellationToken cancellationToken)
    {
        _ = await ReadSourceStreamAsync(buffer[..element.DataSize], cancellationToken).ConfigureAwait(false);

        var value = EbmlReader.ReadUtf8String(buffer.Span[..element.DataSize]).ToString();

        _logger.LogElementValueRead(element, value);

        return value;
    }

    private async ValueTask<ulong> ReadUlongAsync(MatroskaElement element, Memory<byte> buffer, CancellationToken cancellationToken)
    {
        _ = await ReadSourceStreamAsync(buffer[..element.DataSize], cancellationToken).ConfigureAwait(false);

        var value = EbmlReader.ReadUnsignedInteger(buffer.Span[..element.DataSize]);

        _logger.LogElementValueRead(element, value);

        return value;
    }

    private async ValueTask SkipElementAsync(MatroskaElement element, CancellationToken cancellationToken)
    {
        _ = await InternalSeekAsync(element.GetRemaining(_position), SeekOrigin.Current, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<int> ReadSourceStreamAsync(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        await PreventSourceSeekAsync(cancellationToken).ConfigureAwait(false);

        var result = await AudioStreamUtils.ReadFullyAudioStreamAsync(_sourceStream, buffer, cancellationToken).ConfigureAwait(false);

        _position += result;

        return result;
    }

    private async ValueTask<long> InternalSeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken)
    {
        var result = await AudioStreamUtils.SeekAsync(_sourceStream, offset, origin, _position, cancellationToken).ConfigureAwait(false);

        _position = result;

        return result;
    }

    private async ValueTask PreventSourceSeekAsync(CancellationToken cancellationToken)
    {
        if (_sourceStream.CanSeek && _sourceStream.Position != _position)
        {
            // Seek by position can break the demuxer state.
            // With this method we can prevent the source stream seeking.
            _ = await _sourceStream.SeekAsync(_position, SeekOrigin.Begin, cancellationToken).ConfigureAwait(false);
        }
    }
}
