// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System.Buffers;
using Microsoft.Extensions.Logging;
using NextAudio.Matroska.Models;

namespace NextAudio.Matroska;

/// <summary>
/// Represents a demuxer that demux Matroska files.
/// </summary>
public sealed class MatroskaDemuxer : AudioDemuxer
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

    private bool CurrentBlockHasFrames => _currentBlock.HasValue && (_currentBlockIndex + 1) < _currentBlock.Value.FrameCount;

    /// <inheritdoc/>
    public override int Demux(Span<byte> buffer)
    {
        _logger.LogReadBufferSize(buffer);

        if (SelectedTrack == null)
        {
            StartMatroskaReading(buffer);
        }

        int result;

        if (_currentBlock.HasValue && CurrentBlockHasFrames)
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
                    return result;
                }
            }

            SkipElement(childElement.Value);
        }

        return 0;
    }

    private int ReadNextFrameFromCluster(Span<byte> buffer, MatroskaElement clusterElement)
    {
        MatroskaElement? childElement;

        while ((childElement = ReadNextElement(buffer, clusterElement)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.BlockGroup)
            {
                var result = ReadNextFrameFromGroup(buffer, childElement.Value);

                if (result > 0)
                {
                    _currentBlockGroupElement = childElement;
                    return result;
                }
            }

            if (childElement.Value.Type == MatroskaElementType.SimpleBlock)
            {
                var result = ReadFrameFromBlockElement(buffer, childElement.Value);

                if (result > 0)
                {
                    return result;
                }
            }

            SkipElement(childElement.Value);
        }

        return 0;
    }

    private int ReadNextFrameFromGroup(Span<byte> buffer, MatroskaElement blockGroupElement)
    {
        MatroskaElement? childElement;

        while ((childElement = ReadNextElement(buffer, blockGroupElement)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.Block)
            {
                var result = ReadFrameFromBlockElement(buffer, childElement.Value);

                if (result > 0)
                {
                    return result;
                }
            }

            SkipElement(childElement.Value);
        }

        return 0;
    }

    private int ReadFrameFromBlockElement(Span<byte> buffer, MatroskaElement blockElement)
    {
        var block = ParseBlock(buffer, blockElement);

        return !block.HasValue ? 0 : ReadFrameFromBlock(buffer, block.Value);
    }

    private int ReadFrameFromBlock(Span<byte> buffer, MatroskaBlock block)
    {
        while (CurrentBlockHasFrames)
        {
            var frameSize = block.GetFrameSizeByIndex(_currentBlockIndex);
            var result = ReadSourceStream(buffer[..frameSize]);

            _currentBlockIndex++;

            if (result > 0)
            {
                return result;
            }
        }

        return 0;
    }

    private MatroskaBlock? ParseBlock(Span<byte> buffer, MatroskaElement blockElement)
    {
        _ = ReadSourceStream(buffer[..2]);

        var trackNumber = EbmlReader.ReadUnsignedInteger(buffer[..2]);

        if (trackNumber != SelectedTrack?.TrackNumber)
        {
            return null;
        }

        _ = ReadSourceStream(buffer[2..3]);

        var flags = buffer[2];
        var lacingType = (MatroskaBlockLacingType)(flags & 0b0000110);

        int frameCount;
        int[] frameSizes;

        if (lacingType != MatroskaBlockLacingType.No)
        {
            _ = ReadSourceStream(buffer[3..4]);
            frameCount = (buffer[3] & 0xff) + 1;
        }
        else
        {
            frameCount = 1;
        }

        frameSizes = new int[frameCount];

        switch (lacingType)
        {
            default:
            case MatroskaBlockLacingType.No:
                frameSizes[0] = (int)blockElement.GetRemaining(_position);
                break;

            case MatroskaBlockLacingType.Xiph:
                ParseXiphLacing(buffer[4..], blockElement, frameSizes);
                break;
            case MatroskaBlockLacingType.FixedSize:
                ParseFixedSizeLacing(blockElement, frameSizes);
                break;
            case MatroskaBlockLacingType.Ebml:
                ParseEbmlLacing(buffer[4..], blockElement, frameSizes);
                break;
        }

        return new MatroskaBlock(trackNumber, lacingType, frameCount, frameSizes);
    }

    private void ParseXiphLacing(Span<byte> buffer, MatroskaElement blockElement, Span<int> frameSizes)
    {
        var totalSize = 0;
        var bufferIndex = 0;

        for (var i = 0; i < frameSizes.Length; i++)
        {
            var value = 0;

            do
            {
                _ = ReadSourceStream(buffer[bufferIndex..(bufferIndex + 1)]);
                value += buffer[bufferIndex] & 0xff;
            } while (value == 255);

            frameSizes[i] = value;
            totalSize += value;
        }

        var remaining = blockElement.GetRemaining(_position);
        frameSizes[^1] = (int)remaining - totalSize;
    }

    private void ParseFixedSizeLacing(MatroskaElement blockElement, Span<int> frameSizes)
    {
        var size = (int)(blockElement.GetRemaining(_position) / frameSizes.Length);

        for (var i = 0; i < frameSizes.Length; i++)
        {
            frameSizes[i] = size;
        }
    }

    private void ParseEbmlLacing(Span<byte> buffer, MatroskaElement blockElement, Span<int> frameSizes)
    {
        var vInt = ReadVInt(buffer);

        frameSizes[0] = (int)vInt.Value;

        var totalVIntLength = vInt.Length;
        var totalSize = frameSizes[0];

        for (var i = 0; i < frameSizes.Length; i++)
        {
            vInt = ReadVInt(buffer[totalVIntLength..]);

            frameSizes[i] = frameSizes[i - 1] + (int)ParseEbmlLaceSignedInteger(vInt);

            totalVIntLength += vInt.Length;
            totalSize += frameSizes[i];
        }

        frameSizes[^1] = (int)blockElement.GetRemaining(_position) - totalSize;
    }

    private static ulong ParseEbmlLaceSignedInteger(VInt vInt)
    {
        return vInt.Length switch
        {
            1 => vInt.Value - 63,
            2 => vInt.Value - 8191,
            3 => vInt.Value - 1048575,
            4 => vInt.Value - 134217727,
            _ => throw new InvalidOperationException("An Ebml Lace Signed Integer cannot have length higher than 4 bytes."),
        };
    }

    private void StartMatroskaReading(Span<byte> buffer)
    {
        var ebmlElement = ReadNextElement(buffer);

        if (!ebmlElement.HasValue)
        {
            throw new InvalidOperationException("Cannot read EBML elements.");
        }

        if (ebmlElement.Value.Type != MatroskaElementType.Ebml)
        {
            throw new InvalidOperationException("EBML Header not the first element in the file.");
        }

        SkipElement(ebmlElement.Value);

        var segmentElement = ReadNextElement(buffer);

        if (!segmentElement.HasValue)
        {
            throw new InvalidOperationException("Cannot read segment element.");
        }

        _segmentElement = segmentElement.Value;

        if (_segmentElement.Type != MatroskaElementType.Segment)
        {
            throw new InvalidOperationException("Segment not the second element in the file.");
        }

        _segmentElementLogScope = _logger.ProcessingMasterElementScope(_segmentElement, _position);

        ParseSegmentElement(buffer, _segmentElement);
    }

    private void ParseSegmentElement(Span<byte> buffer, MatroskaElement segmentElement)
    {
        MatroskaElement? childElement;

        while ((childElement = ReadNextElement(buffer, segmentElement)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.Tracks)
            {
                ParseTracksElement(buffer, childElement.Value);
            }
            if (childElement.Value.Type == MatroskaElementType.Cluster)
            {
                _currentClusterElement = childElement.Value;
                break;
            }

            SkipElement(childElement.Value);
        }
    }

    private void ParseTracksElement(Span<byte> buffer, MatroskaElement tracksElement)
    {
        using var _ = _logger.ProcessingMasterElementScope(tracksElement, _position);

        MatroskaElement? childElement;

        var tracks = new List<MatroskaTrack>();

        while ((childElement = ReadNextElement(buffer, tracksElement)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.TrackEntry)
            {
                tracks.Add(ParseTrackElement(buffer, childElement.Value));
            }

            SkipElement(childElement.Value);
        }

        var selectedTrackNumber = _options.TrackSelector(tracks);

        _logger.LogTrackSelected(selectedTrackNumber);

        SelectedTrack = tracks.First(track => track.TrackNumber == selectedTrackNumber);
    }

    private MatroskaTrack ParseTrackElement(Span<byte> buffer, MatroskaElement trackElement)
    {
        using var _ = _logger.ProcessingMasterElementScope(trackElement, _position);

        MatroskaElement? childElement;

        ulong trackNumber = 0;
        ulong trackUid = 0;
        string? name = null;
        var codecID = string.Empty;
        var type = MatroskaTrackType.Unknown;
        var codecPrivate = Array.Empty<byte>();
        MatroskaAudioSettings? audioSettings = null;

        while ((childElement = ReadNextElement(buffer, trackElement)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.TrackNumber)
            {
                trackNumber = ReadUlong(childElement.Value, buffer);
            }
            if (childElement.Value.Type == MatroskaElementType.TrackUid)
            {
                trackUid = ReadUlong(childElement.Value, buffer);
            }
            if (childElement.Value.Type == MatroskaElementType.Name)
            {
                name = ReadUtf8String(childElement.Value, buffer);
            }
            if (childElement.Value.Type == MatroskaElementType.CodecId)
            {
                codecID = ReadAsciiString(childElement.Value, buffer);
            }
            if (childElement.Value.Type == MatroskaElementType.TrackType)
            {
                type = (MatroskaTrackType)ReadUlong(childElement.Value, buffer);
            }
            if (childElement.Value.Type == MatroskaElementType.CodecPrivate)
            {
                var slicedBuffer = ReadBytes(childElement.Value, buffer);

                codecPrivate = new byte[slicedBuffer.Length];

                slicedBuffer.CopyTo(codecPrivate);
            }
            if (childElement.Value.Type == MatroskaElementType.Audio)
            {
                audioSettings = ParseAudioElement(buffer, childElement.Value);
            }

            SkipElement(childElement.Value);
        }

        return new MatroskaTrack(codecID)
        {
            TrackNumber = trackNumber,
            TrackUID = trackUid,
            Name = name,
            Type = type,
            CodecPrivate = Array.AsReadOnly(codecPrivate),
            Audio = audioSettings,
        };
    }

    private MatroskaAudioSettings ParseAudioElement(Span<byte> buffer, MatroskaElement audioElement)
    {
        using var _ = _logger.ProcessingMasterElementScope(audioElement, _position);

        MatroskaElement? childElement;

        var samplingFrequency = 0f;
        float? outputSamplingFrequency = null;
        ulong channels = 0;
        ulong? bitDepth = null;

        while ((childElement = ReadNextElement(buffer, audioElement)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.SamplingFrequency)
            {
                samplingFrequency = (float)ReadFloat(childElement.Value, buffer);
            }
            if (childElement.Value.Type == MatroskaElementType.OutputSamplingFrequency)
            {
                outputSamplingFrequency = (float)ReadFloat(childElement.Value, buffer);
            }
            if (childElement.Value.Type == MatroskaElementType.Channels)
            {
                channels = ReadUlong(childElement.Value, buffer);
            }
            if (childElement.Value.Type == MatroskaElementType.BitDepth)
            {
                bitDepth = ReadUlong(childElement.Value, buffer);
            }

            SkipElement(childElement.Value);
        }

        return new MatroskaAudioSettings
        {
            SamplingFrequency = samplingFrequency,
            OutputSamplingFrequency = outputSamplingFrequency,
            Channels = channels,
            BitDepth = bitDepth,
        };
    }

    private MatroskaElement? ReadNextElement(Span<byte> buffer, MatroskaElement? parent = null)
    {
        if (parent.HasValue && parent.Value.GetRemaining(_position) == 0)
        {
            return null;
        }

        var position = _position;
        var elementId = ReadVInt(buffer);
        var elementSize = ReadVInt(buffer[elementId.Length..]);
        var headerSize = _position - position;
        var depth = parent.HasValue ? parent.Value.Depth + 1 : 0;

        var element = new MatroskaElement(elementId.EncodedValue, depth, position, (int)headerSize, (int)elementSize.Value);

        _logger.LogElementReaded(element);

        return element;
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

    private VInt ReadVInt(Span<byte> buffer)
    {
        _ = ReadSourceStream(buffer[..1]);

        var length = EbmlReader.ReadVariableSizeIntegerLength(buffer[0]);

        _ = ReadSourceStream(buffer[1..length]);

        var vInt = EbmlReader.ReadVariableSizeInteger(buffer[..length], length);

        _logger.LogVIntReaded(vInt, _position);

        return vInt;
    }

    private int ReadSourceStream(Span<byte> buffer, bool throwIfEnd = true)
    {
        var bytesReaded = _sourceStream.Read(buffer);

        _position += bytesReaded;

        return bytesReaded <= 0 && throwIfEnd ? throw new EndOfStreamException() : bytesReaded;
    }

    /// <inheritdoc/>
    public override ValueTask<int> DemuxAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private void SkipElement(MatroskaElement element)
    {
        _ = Seek(element.EndPosition, SeekOrigin.Current);
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        if (CanSeek)
        {
            var newPos = _sourceStream.Seek(offset, origin);

            _position = Position;

            return newPos;
        }

        var pos = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.End => Length + offset, // Should throw NotSupportedException if not we can continue.
            SeekOrigin.Current => _position + offset,
            _ => throw new InvalidOperationException($"Unknown SeekOrigin enum value: {origin}")
        };

        InternalSeek(pos);

        return _position;
    }

    /// <summary>
    /// Seek if the source stream cannot be seeked.
    /// </summary>
    /// <remarks>
    /// Only can seek forward.
    /// </remarks>
    /// <param name="position">The position to seek.</param>
    private void InternalSeek(long position)
    {
        if (position < _position)
        {
            throw new NotSupportedException("The source stream cannot be seeked, you only can seek forward.");
        }

        if (position > int.MaxValue)
        {
            throw new InvalidOperationException("Cannot seek more than the Int32 limit.");
        }

        if (position == _position)
        {
            return;
        }

        var numberOfBytesToSkip = (int)(position - _position);

        var skipBuffer = ArrayPool<byte>.Shared.Rent(numberOfBytesToSkip);

        try
        {
            var bytesRead = _sourceStream.Read(skipBuffer, 0, numberOfBytesToSkip);

            _position += bytesRead;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(skipBuffer, true);
        }
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
