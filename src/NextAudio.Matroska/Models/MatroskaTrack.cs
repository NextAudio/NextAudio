// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using NextAudio.Formats;

namespace NextAudio.Matroska.Models;

/// <summary>
/// Represents a Matroska track entry.
/// </summary>
public record class MatroskaTrack
{
    /// <summary>
    /// Creates a new instance of <see cref="MatroskaTrack" />.
    /// </summary>
    /// <param name="codecID">The ID corresponding to the codec.</param>
    public MatroskaTrack(string codecID)
    {
        CodecID = codecID;
    }

    /// <summary>
    /// The track number as used in the Block Header.
    /// </summary>
    public ulong TrackNumber { get; init; }

    /// <summary>
    /// The trackUID number of the track representing the plane.
    /// </summary>
    public ulong TrackUID { get; init; }

    /// <summary>
    /// The human-readable track name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// The ID corresponding to the codec.
    /// </summary>
    public string CodecID { get; }

    /// <summary>
    /// The Matroska track type.
    /// </summary>
    public MatroskaTrackType Type { get; init; }

    /// <summary>
    /// The private data only known to the audio codec.
    /// </summary>
    public ReadOnlyMemory<byte> CodecPrivate { get; init; }

    /// <summary>
    /// The audio details for this Matroska Audio Track.
    /// </summary>
    public MatroskaAudioSettings? Audio { get; init; }

    /// <summary>
    /// The audio coding for this Matroska Audio Track.
    /// </summary>
    public AudioCoding? AudioCoding { get; init; }
}
