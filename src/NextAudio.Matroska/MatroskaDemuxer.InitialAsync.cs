// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using NextAudio.Matroska.Models;

namespace NextAudio.Matroska;

public partial class MatroskaDemuxer
{
    private async ValueTask StartMatroskaReadingAsync(Memory<byte> buffer)
    {
        var ebmlElement = await ReadNextElementAsync(buffer).ConfigureAwait(false);

        if (!ebmlElement.HasValue)
        {
            throw new InvalidOperationException("Cannot read EBML elements.");
        }

        if (ebmlElement.Value.Type != MatroskaElementType.Ebml)
        {
            throw new InvalidOperationException("EBML Header not the first element in the file.");
        }

        await SkipElementAsync(ebmlElement.Value).ConfigureAwait(false);

        var segmentElement = await ReadNextElementAsync(buffer).ConfigureAwait(false);

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

        await ParseSegmentElementAsync(buffer, _segmentElement).ConfigureAwait(false);
    }

    private async ValueTask ParseSegmentElementAsync(Memory<byte> buffer, MatroskaElement segmentElement)
    {
        MatroskaElement? childElement;

        while ((childElement = await ReadNextElementAsync(buffer, segmentElement).ConfigureAwait(false)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.Tracks)
            {
                await ParseTracksElementAsync(buffer, childElement.Value).ConfigureAwait(false);
            }
            if (childElement.Value.Type == MatroskaElementType.Cluster)
            {
                _currentClusterElement = childElement.Value;
                break;
            }

            await SkipElementAsync(childElement.Value).ConfigureAwait(false);
        }
    }

    private async ValueTask ParseTracksElementAsync(Memory<byte> buffer, MatroskaElement tracksElement)
    {
        using var _ = _logger.ProcessingMasterElementScope(tracksElement, _position);

        MatroskaElement? childElement;

        var tracks = new List<MatroskaTrack>();

        while ((childElement = await ReadNextElementAsync(buffer, tracksElement).ConfigureAwait(false)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.TrackEntry)
            {
                tracks.Add(await ParseTrackElementAsync(buffer, childElement.Value).ConfigureAwait(false));
            }

            await SkipElementAsync(childElement.Value).ConfigureAwait(false);
        }

        var selectedTrackNumber = _options.TrackSelector(tracks);

        _logger.LogTrackSelected(selectedTrackNumber);

        SelectedTrack = tracks.First(track => track.TrackNumber == selectedTrackNumber);
    }

    private async ValueTask<MatroskaTrack> ParseTrackElementAsync(Memory<byte> buffer, MatroskaElement trackElement)
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

        while ((childElement = await ReadNextElementAsync(buffer, trackElement).ConfigureAwait(false)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.TrackNumber)
            {
                trackNumber = await ReadUlongAsync(childElement.Value, buffer).ConfigureAwait(false);
            }
            if (childElement.Value.Type == MatroskaElementType.TrackUid)
            {
                trackUid = await ReadUlongAsync(childElement.Value, buffer).ConfigureAwait(false);
            }
            if (childElement.Value.Type == MatroskaElementType.Name)
            {
                name = await ReadUtf8StringAsync(childElement.Value, buffer).ConfigureAwait(false);
            }
            if (childElement.Value.Type == MatroskaElementType.CodecId)
            {
                codecID = await ReadAsciiStringAsync(childElement.Value, buffer).ConfigureAwait(false);
            }
            if (childElement.Value.Type == MatroskaElementType.TrackType)
            {
                var typeUlong = await ReadUlongAsync(childElement.Value, buffer).ConfigureAwait(false);

                type = (MatroskaTrackType)typeUlong;
            }
            if (childElement.Value.Type == MatroskaElementType.CodecPrivate)
            {
                var slicedBuffer = await ReadBytesAsync(childElement.Value, buffer).ConfigureAwait(false);

                codecPrivate = new byte[slicedBuffer.Length];

                slicedBuffer.CopyTo(codecPrivate);
            }
            if (childElement.Value.Type == MatroskaElementType.Audio)
            {
                audioSettings = await ParseAudioElementAsync(buffer, childElement.Value).ConfigureAwait(false);
            }

            await SkipElementAsync(childElement.Value).ConfigureAwait(false);
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

    private async ValueTask<MatroskaAudioSettings> ParseAudioElementAsync(Memory<byte> buffer, MatroskaElement audioElement)
    {
        using var _ = _logger.ProcessingMasterElementScope(audioElement, _position);

        MatroskaElement? childElement;

        var samplingFrequency = 0f;
        float? outputSamplingFrequency = null;
        ulong channels = 0;
        ulong? bitDepth = null;

        while ((childElement = await ReadNextElementAsync(buffer, audioElement).ConfigureAwait(false)) != null)
        {
            if (childElement.Value.Type == MatroskaElementType.SamplingFrequency)
            {
                var samplingFrequencyDouble = await ReadFloatAsync(childElement.Value, buffer).ConfigureAwait(false);
                samplingFrequency = (float)samplingFrequencyDouble;
            }
            if (childElement.Value.Type == MatroskaElementType.OutputSamplingFrequency)
            {
                var outputSamplingFrequencyDouble = await ReadFloatAsync(childElement.Value, buffer).ConfigureAwait(false);
                outputSamplingFrequency = (float)outputSamplingFrequencyDouble;
            }
            if (childElement.Value.Type == MatroskaElementType.Channels)
            {
                channels = await ReadUlongAsync(childElement.Value, buffer).ConfigureAwait(false);
            }
            if (childElement.Value.Type == MatroskaElementType.BitDepth)
            {
                bitDepth = await ReadUlongAsync(childElement.Value, buffer).ConfigureAwait(false);
            }

            await SkipElementAsync(childElement.Value).ConfigureAwait(false);
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
