// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using NextAudio.Matroska.Models;

namespace NextAudio.Matroska.Utils;

/// <summary>
/// Some matroska function utilities.
/// </summary>
public static class MatroskaUtils
{

    /// <summary>
    /// Cast <see cref="MatroskaElementType" /> in an <see cref="EbmlValueType" />.
    /// </summary>
    /// <param name="type">The <see cref="MatroskaElementType" /> to be cast.</param>
    /// <returns>The casted <see cref="EbmlValueType" />.</returns>
    public static EbmlValueType GetEbmlValueType(this MatroskaElementType type)
    {
        return type switch
        {
            MatroskaElementType.Ebml => EbmlValueType.MasterElement,
            MatroskaElementType.DocType => EbmlValueType.AsciiString,
            MatroskaElementType.Segment => EbmlValueType.MasterElement,
            MatroskaElementType.SeekHead => EbmlValueType.MasterElement,
            MatroskaElementType.Seek => EbmlValueType.MasterElement,
            MatroskaElementType.SeekId => EbmlValueType.Binary,
            MatroskaElementType.SeekPosition => EbmlValueType.UnsignedInteger,
            MatroskaElementType.Info => EbmlValueType.MasterElement,
            MatroskaElementType.Duration => EbmlValueType.Float,
            MatroskaElementType.TimecodeScale => EbmlValueType.UnsignedInteger,
            MatroskaElementType.Cluster => EbmlValueType.MasterElement,
            MatroskaElementType.Timecode => EbmlValueType.UnsignedInteger,
            MatroskaElementType.SimpleBlock => EbmlValueType.Binary,
            MatroskaElementType.BlockGroup => EbmlValueType.MasterElement,
            MatroskaElementType.Block => EbmlValueType.Binary,
            MatroskaElementType.BlockDuration => EbmlValueType.UnsignedInteger,
            MatroskaElementType.ReferenceBlock => EbmlValueType.SignedInteger,
            MatroskaElementType.Tracks => EbmlValueType.MasterElement,
            MatroskaElementType.TrackEntry => EbmlValueType.MasterElement,
            MatroskaElementType.TrackNumber => EbmlValueType.UnsignedInteger,
            MatroskaElementType.TrackUid => EbmlValueType.UnsignedInteger,
            MatroskaElementType.TrackType => EbmlValueType.UnsignedInteger,
            MatroskaElementType.Name => EbmlValueType.Utf8String,
            MatroskaElementType.CodecId => EbmlValueType.AsciiString,
            MatroskaElementType.CodecPrivate => EbmlValueType.Binary,
            MatroskaElementType.Audio => EbmlValueType.MasterElement,
            MatroskaElementType.SamplingFrequency => EbmlValueType.Float,
            MatroskaElementType.OutputSamplingFrequency => EbmlValueType.Float,
            MatroskaElementType.Channels => EbmlValueType.UnsignedInteger,
            MatroskaElementType.BitDepth => EbmlValueType.UnsignedInteger,
            MatroskaElementType.Cues => EbmlValueType.MasterElement,
            MatroskaElementType.CuePoint => EbmlValueType.MasterElement,
            MatroskaElementType.CueTime => EbmlValueType.UnsignedInteger,
            MatroskaElementType.CueTrackPositions => EbmlValueType.MasterElement,
            MatroskaElementType.CueTrack => EbmlValueType.UnsignedInteger,
            MatroskaElementType.CueClusterPosition => EbmlValueType.UnsignedInteger,
            MatroskaElementType.Unknown => throw new NotImplementedException(),
            _ => EbmlValueType.None,
        };
    }
}
