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

    private void CheckCurrentStatePostSeek()
    {
        // We don't need to check the current state if
        // the demuxer doesn't started yet.
        if (SelectedTrack == null)
        {
            return;
        }

        // We doesn't wanna go to before the segment element.
        if (_position < _segmentElement.Position || _position < _segmentElement.DataPosition)
        {
            _ = InternalSeek(_segmentElement.DataPosition, SeekOrigin.Begin);

            _currentClusterElement = null;
            _currentBlockGroupElement = null;
            _currentBlock = null;
            _currentBlockIndex = 0;
            return;
        }

        CheckCurrentStateForCluster();
        CheckCurrentStateForBlockGroup();
        CheckCurrentStateForBlock();
    }

    private void CheckCurrentStateForCluster()
    {
        var shouldDeleteClusterElement = _currentClusterElement.HasValue &&
            (
                _position < _currentClusterElement.Value.Position ||
                _position > _currentClusterElement.Value.EndPosition
            );

        if (shouldDeleteClusterElement)
        {
            _currentClusterElement = null;
            return;
        }

        var positionIsInsideClusterElement = _currentClusterElement.HasValue &&

            _position >= _currentClusterElement.Value.Position &&
            _position <= _currentClusterElement.Value.DataPosition
        ;

        if (positionIsInsideClusterElement)
        {
            _ = InternalSeek(_currentClusterElement!.Value.DataPosition, SeekOrigin.Begin);
        }
    }

    private void CheckCurrentStateForBlockGroup()
    {
        var shouldDeleteClusterElement = _currentBlockGroupElement.HasValue &&
            (
                _position < _currentBlockGroupElement.Value.Position ||
                _position > _currentBlockGroupElement.Value.EndPosition
            );

        if (shouldDeleteClusterElement)
        {
            _currentBlockGroupElement = null;
            return;
        }

        var positionIsInsideClusterElement = _currentBlockGroupElement.HasValue &&

            _position >= _currentBlockGroupElement.Value.Position &&
            _position <= _currentBlockGroupElement.Value.DataPosition
        ;

        if (positionIsInsideClusterElement)
        {
            _ = InternalSeek(_currentBlockGroupElement!.Value.DataPosition, SeekOrigin.Begin);
        }
    }

    private void CheckCurrentStateForBlock()
    {
        var shouldDeleteClusterElement = _currentBlock.HasValue &&
            (
                _position < _currentBlock.Value.Element.Position ||
                _position > _currentBlock.Value.Element.EndPosition
            );

        if (shouldDeleteClusterElement)
        {
            _currentBlock = null;
            _currentBlockIndex = 0;
            return;
        }

        var positionIsInsideClusterElement = _currentBlock.HasValue &&

            _position >= _currentBlock.Value.Element.Position &&
            _position <= _currentBlock.Value.Element.DataPosition
        ;

        if (positionIsInsideClusterElement)
        {
            _ = InternalSeek(_currentBlock!.Value.Element.DataPosition, SeekOrigin.Begin);
            _currentBlockIndex = 0;
        }

        if (!_currentBlock.HasValue)
        {
            return;
        }

        // If block was survived we will change the index relative to position
        // and seek to the start of the frame.

        var totalFrameSize = 0;
        var blockStartPosition = _currentBlock.Value.Element.DataPosition;

        for (var i = 0; i < _currentBlock.Value.FrameCount; i++)
        {
            var frameSize = _currentBlock.Value.GetFrameSizeByIndex(i);

            var frameStartPosition = totalFrameSize + blockStartPosition;

            totalFrameSize += frameSize;

            var frameEndPosition = totalFrameSize + blockStartPosition;

            if (frameEndPosition < _position)
            {
                continue;
            }

            _currentBlockIndex = i;

            if (frameStartPosition != _position)
            {
                _ = InternalSeek(frameStartPosition, SeekOrigin.Begin);
            }
        }
    }
}
