// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using NextAudio.Matroska.Models;

namespace NextAudio.Matroska;

public partial class MatroskaDemuxer
{
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
            CodecPrivate = codecPrivate ?? ReadOnlyMemory<byte>.Empty,
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
}
