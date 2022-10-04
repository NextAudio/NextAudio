// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using NextAudio.Matroska.Models;

namespace NextAudio.Matroska;

/// <summary>
/// Represents a demuxer that demux Matroska files.
/// </summary>
public sealed partial class MatroskaDemuxer : AudioDemuxer
{
    private readonly AudioStream _sourceStream;
    private readonly ILogger<MatroskaDemuxer> _logger;
    private readonly MatroskaDemuxerOptions _options;

    private long _position;

    private MatroskaElement _segmentElement;
    private MatroskaElement? _currentClusterElement;
    private MatroskaElement? _currentBlockGroupElement;
    private MatroskaBlock? _currentBlock;
    private int _currentBlockIndex;

    private IDisposable? _segmentElementLogScope;


    /// <summary>
    /// Creates a new instance of <see cref="MatroskaDemuxer" />.
    /// </summary>
    /// <param name="sourceStream">The source stream with Matroska data to be demuxed.</param>
    /// <param name="options">The options for this demuxer.</param>
    /// <param name="loggerFactory">A logger factory to log audio streaming info.</param>
    public MatroskaDemuxer(AudioStream sourceStream, MatroskaDemuxerOptions? options = null, ILoggerFactory? loggerFactory = null)
        : base(loggerFactory)
    {
        _sourceStream = sourceStream ?? throw new ArgumentNullException(nameof(sourceStream));
        _options = options ?? MatroskaDemuxerOptions.Default;
        _logger = _loggerFactory.CreateLogger<MatroskaDemuxer>();
    }

    /// <summary>
    /// The Matroska track selected using the <see cref="MatroskaDemuxerOptions.TrackSelector" />.
    /// </summary>
    public MatroskaTrack? SelectedTrack { get; private set; }

    /// <inheritdoc/>
    public override bool CanSeek => _sourceStream.CanSeek;

    /// <inheritdoc/>
    public override long Length => _sourceStream.Length;

    /// <inheritdoc/>
    public override long Position
    {
        get => CanSeek ? _sourceStream.Position : _position;
        set => Seek(value, SeekOrigin.Begin);
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        var result = InternalSeek(offset, origin);

        CheckCurrentStatePostSeek();

        return result;
    }

    private bool BlockHasFrames(MatroskaBlock block)
    {
        return _currentBlockIndex < block.FrameCount;
    }

    private void CheckCurrentStatePostSeek()
    {
        if (_position < _segmentElement.Position)
        {
            _segmentElement = default;
        }


        if (_currentClusterElement.HasValue)
        {
            if (_position < _currentClusterElement.Value.Position || _position > _currentClusterElement.Value.EndPosition)
            {
                _currentClusterElement = null;
            }
        }

        if (_currentBlockGroupElement.HasValue)
        {
            if (_position < _currentBlockGroupElement.Value.Position || _position > _currentBlockGroupElement.Value.EndPosition)
            {
                _currentBlockGroupElement = null;
            }
        }

        if (_currentBlock.HasValue)
        {
            if (_position < _currentBlock.Value.Element.Position || _position > _currentBlock.Value.Element.EndPosition)
            {
                _currentBlock = null;
                _currentBlockIndex = 0;
            }
        }

        if (!_currentBlock.HasValue)
        {
            return;
        }

        // If block was survived we will change the index relative to position
        // and seek to the start of the frame.

        var totalFrameSize = 0;
        var blockStartPosition = _currentBlock.Value.Element.Position + _currentBlock.Value.Element.HeaderSize;

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

    /// <inheritdoc/>
    public override ValueTask<int> DemuxAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }


        if (disposing && _options.DisposeSourceStream)
        {
            if (_segmentElementLogScope != null)
            {
                _segmentElementLogScope.Dispose();
                _segmentElementLogScope = null;
            }

            _sourceStream.Dispose();
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsyncCore()
    {
        if (IsDisposed)
        {
            return;
        }

        if (_options.DisposeSourceStream)
        {
            if (_segmentElementLogScope != null)
            {
                _segmentElementLogScope.Dispose();
                _segmentElementLogScope = null;
            }

            await _sourceStream.DisposeAsync();
        }
    }

    /// <inheritdoc/>
    public override MatroskaDemuxer Clone()
    {
        var optionsClone = _options.Clone();

        var copy = new MatroskaDemuxer(_sourceStream, optionsClone, _loggerFactory);

        _options.DisposeSourceStream = false;

        return copy;
    }
}
