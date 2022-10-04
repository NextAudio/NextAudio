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
        byte[]? codecPrivate = null;
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
            CodecPrivate = codecPrivate != null ? Array.AsReadOnly(codecPrivate) : null,
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
