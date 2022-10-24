// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using NextAudio.Matroska.Internal;
using NextAudio.Matroska.Models;

namespace NextAudio.Matroska;

public partial class MatroskaDemuxer
{
    /// <inheritdoc/>
    public override int Demux(Span<byte> buffer)
    {
        _logger.LogReadBufferSize(buffer);

        if (!IsInitialized)
        {
            Initialize();
        }

        int result;

        if (_currentBlock.HasValue && BlockHasFrames(_currentBlock.Value))
        {
            result = ReadFrameFromBlock(buffer, _currentBlock.Value);

            if (result > 0)
            {
                return result;
            }

            _currentBlock = null;
        }

        _currentBlockIndex = 0;

        if (_currentBlockGroupElement.HasValue)
        {
            result = ReadNextFrameFromGroup(buffer, _currentBlockGroupElement.Value);

            if (result > 0)
            {
                return result;
            }

            _currentBlockGroupElement = null;
        }

        if (_currentClusterElement.HasValue)
        {
            result = ReadNextFrameFromCluster(buffer, _currentClusterElement.Value);

            if (result > 0)
            {
                return result;
            }

            _currentClusterElement = null;
        }

        return ReadNextFrameFromSegment(buffer);
    }

    private int ReadNextFrameFromSegment(Span<byte> buffer)
    {
        MatroskaElement? childElement;

        while ((childElement = ReadNextElement(buffer, _segmentElement)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.Cluster)
            {
                var result = ReadNextFrameFromCluster(buffer, childElement.Value);

                if (result > 0)
                {
                    _currentClusterElement = childElement;
                    _clusterElementLogScope = _logger.ProcessingMasterElementScope(childElement.Value);
                    return result;
                }
            }

            SkipElement(childElement.Value);
        }

        DisposeSegmentElementLogScope();
        return 0;
    }

    private int ReadNextFrameFromCluster(Span<byte> buffer, MatroskaElement clusterElement)
    {
        MatroskaElement? childElement;

        while ((childElement = ReadNextElement(buffer, clusterElement)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.BlockGroup)
            {
                _blockGroupElementLogScope = _logger.ProcessingMasterElementScope(childElement.Value);

                var result = ReadNextFrameFromGroup(buffer, childElement.Value);

                if (result > 0)
                {
                    _currentBlockGroupElement = childElement;
                    return result;
                }
            }

            if (childElement.Value.Type == MatroskaElementType.SimpleBlock)
            {
                _blockElementLogScope = _logger.ProcessingMasterElementScope(childElement.Value);

                var result = ReadFrameFromBlockElement(buffer, childElement.Value);

                if (result > 0)
                {
                    return result;
                }
            }

            SkipElement(childElement.Value);
        }

        DisposeClusterElementLogScope();
        return 0;
    }

    private int ReadNextFrameFromGroup(Span<byte> buffer, MatroskaElement blockGroupElement)
    {
        MatroskaElement? childElement;

        while ((childElement = ReadNextElement(buffer, blockGroupElement)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.Block)
            {
                _blockElementLogScope = _logger.ProcessingMasterElementScope(childElement.Value);

                var result = ReadFrameFromBlockElement(buffer, childElement.Value);

                if (result > 0)
                {
                    return result;
                }
            }

            SkipElement(childElement.Value);
        }

        DisposeBlockGroupElementLogScope();
        return 0;
    }

    private int ReadFrameFromBlockElement(Span<byte> buffer, MatroskaElement blockElement)
    {
        var block = ParseBlock(buffer, blockElement);

        if (!block.HasValue)
        {
            DisposeBlockElementLogScope();
        }

        return !block.HasValue ? 0 : ReadFrameFromBlock(buffer, block.Value);
    }

    private MatroskaBlock? ParseBlock(Span<byte> buffer, MatroskaElement blockElement)
    {
        var result = BlockReader.ReadBlock(_sourceStream, buffer, blockElement, _position, SelectedTrack!.TrackNumber, _logger);

        _position = result.NewPosition;

        return result.Block;
    }

    private int ReadFrameFromBlock(Span<byte> buffer, MatroskaBlock block)
    {
        while (BlockHasFrames(block))
        {
            var frameSize = block.GetFrameSizeByIndex(_currentBlockIndex);
            var result = ReadSourceStream(buffer[..frameSize]);

            _logger.LogFrameRead(frameSize, _currentBlockIndex, _position);

            _currentBlockIndex++;

            _currentBlock = block;

            if (result <= 0)
            {
                DisposeBlockElementLogScope();
            }

            return result;
        }

        DisposeBlockElementLogScope();
        return 0;
    }
}
