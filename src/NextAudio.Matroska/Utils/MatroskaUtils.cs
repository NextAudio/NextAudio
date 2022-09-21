// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using NextAudio.Matroska.Models;

namespace NextAudio.Matroska.Utils;

/// <summary>
/// Some matroska function utilities.
/// </summary>
public static class MatroskaUtils
{
    private static readonly ulong[] MatroskaElementTypeValues = Enum.GetValues(typeof(MatroskaElementType)).Cast<ulong>().ToArray();

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

    /// <summary>
    /// Check if the <paramref name="id" /> was mapped in the <see cref="MatroskaElementType" /> enum.
    /// </summary>
    /// <param name="id">The Ebml id to be checked.</param>
    /// <returns><see langword="true" /> if the <paramref name="id" /> is mapped, otherwise <see langword="false" />.</returns>
    public static bool IsEbmlIdMappedForMatroskaElementType(ulong id)
    {
        return MatroskaElementTypeValues.Contains(id);
    }

    /// <summary>
    /// Cast an <see cref="ulong" /> in an <see cref="MatroskaElementType" />.
    /// </summary>
    /// <param name="id">The <see cref="ulong" /> to be cast.</param>
    /// <returns>
    /// The casted <see cref="MatroskaElementType" /> (will be <see cref="MatroskaElementType.Unknown" /> if <paramref name="id" /> isn't mapped).
    /// </returns>
    public static MatroskaElementType GetMatroskaElementType(ulong id)
    {
        return IsEbmlIdMappedForMatroskaElementType(id)
            ? (MatroskaElementType)id
            : MatroskaElementType.Unknown;
    }

    /// <summary>
    /// Try cast an <see cref="ulong" /> in an <see cref="MatroskaElementType" />.
    /// </summary>
    /// <param name="id">The <see cref="ulong" /> to be cast.</param>
    /// <param name="matroskaElementType">
    /// The casted <see cref="MatroskaElementType" /> (will be <see cref="MatroskaElementType.Unknown" /> if the return be <see langword="false"/>).
    /// </param>
    /// <returns><see langword="true" /> if casted succesfully, otherwise <see langword="false" />.</returns>
    public static bool TryGetMatroskaElementType(ulong id, out MatroskaElementType matroskaElementType)
    {
        matroskaElementType = GetMatroskaElementType(id);
        return matroskaElementType != MatroskaElementType.Unknown;
    }
}
