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
    private IDisposable? _clusterElementLogScope;
    private IDisposable? _blockGroupElementLogScope;
    private IDisposable? _blockElementLogScope;


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
    public override bool CanSeek => false;

    /// <inheritdoc/>
    public override long Length => _sourceStream.Length;

    /// <inheritdoc/>
    public override long Position
    {
        get => _position;
        set => throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    private void PreventSourceSeek()
    {
        if (_sourceStream.CanSeek && _sourceStream.Position != _position)
        {
            // Seek by position can break the demuxer state.
            // With this method we can prevent the source stream seeking.
            _ = _sourceStream.Seek(_position, SeekOrigin.Begin);
        }
    }

    private bool BlockHasFrames(MatroskaBlock block)
    {
        return _currentBlockIndex < block.FrameCount;
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
            DisposeSegmentElementLogScope();
            DisposeClusterElementLogScope();
            DisposeBlockGroupElementLogScope();
            DisposeBlockElementLogScope();

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
            DisposeSegmentElementLogScope();
            DisposeClusterElementLogScope();
            DisposeBlockGroupElementLogScope();
            DisposeBlockElementLogScope();

            await _sourceStream.DisposeAsync().ConfigureAwait(false);
        }
    }

    private void DisposeSegmentElementLogScope()
    {
        if (_segmentElementLogScope == null)
        {
            return;
        }

        _segmentElementLogScope.Dispose();
        _segmentElementLogScope = null;
    }

    private void DisposeClusterElementLogScope()
    {
        if (_clusterElementLogScope == null)
        {
            return;
        }

        _clusterElementLogScope.Dispose();
        _clusterElementLogScope = null;
    }

    private void DisposeBlockGroupElementLogScope()
    {
        if (_blockGroupElementLogScope == null)
        {
            return;
        }

        _blockGroupElementLogScope.Dispose();
        _blockGroupElementLogScope = null;
    }

    private void DisposeBlockElementLogScope()
    {
        if (_blockElementLogScope == null)
        {
            return;
        }

        _blockElementLogScope.Dispose();
        _blockElementLogScope = null;
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
