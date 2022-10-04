// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using NextAudio.Matroska.Internal;
using NextAudio.Matroska.Models;

namespace NextAudio.Matroska;

public partial class MatroskaDemuxer
{
    /// <inheritdoc/>
    public override async ValueTask<int> DemuxAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        _logger.LogReadBufferSize(buffer.Span);

        if (SelectedTrack == null)
        {
            await StartMatroskaReadingAsync(buffer).ConfigureAwait(false);
        }

        int result;

        if (_currentBlock.HasValue && BlockHasFrames(_currentBlock.Value))
        {
            result = await ReadFrameFromBlockAsync(buffer, _currentBlock.Value).ConfigureAwait(false);

            if (result > 0)
            {
                return result;
            }

            _currentBlock = null;
        }

        _currentBlockIndex = 0;

        if (_currentBlockGroupElement.HasValue)
        {
            result = await ReadNextFrameFromGroupAsync(buffer, _currentBlockGroupElement.Value).ConfigureAwait(false);

            if (result > 0)
            {
                return result;
            }

            _currentBlockGroupElement = null;
        }

        if (_currentClusterElement.HasValue)
        {
            result = await ReadNextFrameFromClusterAsync(buffer, _currentClusterElement.Value).ConfigureAwait(false);

            if (result > 0)
            {
                return result;
            }

            _currentClusterElement = null;
        }

        return await ReadNextFrameFromSegmentAsync(buffer).ConfigureAwait(false);
    }

    private async ValueTask<int> ReadNextFrameFromSegmentAsync(Memory<byte> buffer)
    {
        MatroskaElement? childElement;

        while ((childElement = await ReadNextElementAsync(buffer, _segmentElement).ConfigureAwait(false)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.Cluster)
            {
                var result = await ReadNextFrameFromClusterAsync(buffer, childElement.Value).ConfigureAwait(false);

                if (result > 0)
                {
                    _currentClusterElement = childElement;
                    _clusterElementLogScope = _logger.ProcessingMasterElementScope(childElement.Value, _position);
                    return result;
                }
            }

            await SkipElementAsync(childElement.Value).ConfigureAwait(false);
        }

        DisposeSegmentElementLogScope();
        return 0;
    }

    private async ValueTask<int> ReadNextFrameFromClusterAsync(Memory<byte> buffer, MatroskaElement clusterElement)
    {
        MatroskaElement? childElement;

        while ((childElement = await ReadNextElementAsync(buffer, clusterElement).ConfigureAwait(false)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.BlockGroup)
            {
                _blockGroupElementLogScope = _logger.ProcessingMasterElementScope(childElement.Value, _position);

                var result = await ReadNextFrameFromGroupAsync(buffer, childElement.Value).ConfigureAwait(false);

                if (result > 0)
                {
                    _currentBlockGroupElement = childElement;
                    return result;
                }
            }

            if (childElement.Value.Type == MatroskaElementType.SimpleBlock)
            {
                _blockElementLogScope = _logger.ProcessingMasterElementScope(childElement.Value, _position);
                var result = await ReadFrameFromBlockElementAsync(buffer, childElement.Value).ConfigureAwait(false);

                if (result > 0)
                {
                    return result;
                }
            }

            await SkipElementAsync(childElement.Value).ConfigureAwait(false);
        }

        return 0;
    }

    private async ValueTask<int> ReadNextFrameFromGroupAsync(Memory<byte> buffer, MatroskaElement blockGroupElement)
    {
        MatroskaElement? childElement;

        while ((childElement = await ReadNextElementAsync(buffer, blockGroupElement).ConfigureAwait(false)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.Block)
            {
                _blockElementLogScope = _logger.ProcessingMasterElementScope(childElement.Value, _position);
                var result = await ReadFrameFromBlockElementAsync(buffer, childElement.Value).ConfigureAwait(false);

                if (result > 0)
                {
                    return result;
                }
            }

            await SkipElementAsync(childElement.Value).ConfigureAwait(false);
        }

        DisposeBlockGroupElementLogScope();
        return 0;
    }

    private async ValueTask<int> ReadFrameFromBlockElementAsync(Memory<byte> buffer, MatroskaElement blockElement)
    {
        var block = await ParseBlockAsync(buffer, blockElement).ConfigureAwait(false);

        if (!block.HasValue)
        {
            DisposeBlockElementLogScope();
        }

        return !block.HasValue ? 0 : await ReadFrameFromBlockAsync(buffer, block.Value).ConfigureAwait(false);
    }

    private async ValueTask<MatroskaBlock?> ParseBlockAsync(Memory<byte> buffer, MatroskaElement blockElement)
    {
        var result = await BlockReader.ReadBlockAsync(_sourceStream, buffer, blockElement, _position, SelectedTrack!.TrackNumber, _logger).ConfigureAwait(false);

        _position = result.NewPosition;

        return result.Block;
    }

    private async ValueTask<int> ReadFrameFromBlockAsync(Memory<byte> buffer, MatroskaBlock block)
    {
        while (BlockHasFrames(block))
        {
            var frameSize = block.GetFrameSizeByIndex(_currentBlockIndex);
            var result = await ReadSourceStreamAsync(buffer[..frameSize]).ConfigureAwait(false);

            _logger.LogFrameReaded(frameSize, _currentBlockIndex, _position);

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
