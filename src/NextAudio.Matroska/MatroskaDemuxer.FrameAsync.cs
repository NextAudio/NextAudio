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
            await StartMatroskaReadingAsync(buffer);
        }

        int result;

        if (_currentBlock.HasValue && BlockHasFrames(_currentBlock.Value))
        {
            result = await ReadFrameFromBlockAsync(buffer, _currentBlock.Value);

            if (result > 0)
            {
                return result;
            }

            _currentBlock = null;
        }

        _currentBlockIndex = 0;

        if (_currentBlockGroupElement.HasValue)
        {
            result = await ReadNextFrameFromGroupAsync(buffer, _currentBlockGroupElement.Value);

            if (result > 0)
            {
                return result;
            }

            _currentBlockGroupElement = null;
        }

        if (_currentClusterElement.HasValue)
        {
            result = await ReadNextFrameFromClusterAsync(buffer, _currentClusterElement.Value);

            if (result > 0)
            {
                return result;
            }
        }

        return await ReadNextFrameFromSegmentAsync(buffer);
    }

    private async ValueTask<int> ReadNextFrameFromSegmentAsync(Memory<byte> buffer)
    {
        MatroskaElement? childElement;

        while ((childElement = await ReadNextElementAsync(buffer, _segmentElement)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.Cluster)
            {
                var result = await ReadNextFrameFromClusterAsync(buffer, childElement.Value);

                if (result > 0)
                {
                    _currentClusterElement = childElement;
                    return result;
                }
            }

            await SkipElementAsync(childElement.Value);
        }

        return 0;
    }

    private async ValueTask<int> ReadNextFrameFromClusterAsync(Memory<byte> buffer, MatroskaElement clusterElement)
    {
        MatroskaElement? childElement;

        while ((childElement = await ReadNextElementAsync(buffer, clusterElement)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.BlockGroup)
            {
                var result = await ReadNextFrameFromGroupAsync(buffer, childElement.Value);

                if (result > 0)
                {
                    _currentBlockGroupElement = childElement;
                    return result;
                }
            }

            if (childElement.Value.Type == MatroskaElementType.SimpleBlock)
            {
                var result = await ReadFrameFromBlockElementAsync(buffer, childElement.Value);

                if (result > 0)
                {
                    return result;
                }
            }

            await SkipElementAsync(childElement.Value);
        }

        return 0;
    }

    private async ValueTask<int> ReadNextFrameFromGroupAsync(Memory<byte> buffer, MatroskaElement blockGroupElement)
    {
        MatroskaElement? childElement;

        while ((childElement = await ReadNextElementAsync(buffer, blockGroupElement)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.Block)
            {
                var result = await ReadFrameFromBlockElementAsync(buffer, childElement.Value);

                if (result > 0)
                {
                    return result;
                }
            }

            await SkipElementAsync(childElement.Value);
        }

        return 0;
    }

    private async ValueTask<int> ReadFrameFromBlockElementAsync(Memory<byte> buffer, MatroskaElement blockElement)
    {
        var block = await ParseBlockAsync(buffer, blockElement);

        return !block.HasValue ? 0 : await ReadFrameFromBlockAsync(buffer, block.Value);
    }

    private async ValueTask<MatroskaBlock?> ParseBlockAsync(Memory<byte> buffer, MatroskaElement blockElement)
    {
        var result = await BlockReader.ReadBlockAsync(_sourceStream, buffer, blockElement, _position, SelectedTrack!.TrackNumber, _logger);

        _position = result.NewPosition;

        return result.Block;
    }

    private async ValueTask<int> ReadFrameFromBlockAsync(Memory<byte> buffer, MatroskaBlock block)
    {
        while (BlockHasFrames(block))
        {
            var frameSize = block.GetFrameSizeByIndex(_currentBlockIndex);
            var result = await ReadSourceStreamAsync(buffer[..frameSize]);

            _currentBlockIndex++;

            _currentBlock = block;
            return result;
        }

        return 0;
    }
}
