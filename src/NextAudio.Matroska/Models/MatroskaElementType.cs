// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Matroska.Models;

/// <summary>
/// Enum which map relevant matroska element types based on the Ebml Id.
/// </summary>
public enum MatroskaElementType : ulong
{
    /// <summary>
    /// Any other unmapped ebml element type.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Represents the Ebml element type.
    /// </summary>
    Ebml = 0x1A45DFA3,

    /// <summary>
    /// Represents the DocType element type.
    /// </summary>
    DocType = 0x4282,

    /// <summary>
    /// Represents the Segment element type.
    /// </summary>
    Segment = 0x18538067,

    /// <summary>
    /// Represents the SeekHead element type.
    /// </summary>
    SeekHead = 0x114D9B74,

    /// <summary>
    /// Represents the Seek element type.
    /// </summary>
    Seek = 0x4DBB,

    /// <summary>
    /// Represents the SeekId element type.
    /// </summary>
    SeekId = 0x53AB,

    /// <summary>
    /// Represents the SeekPosition element type.
    /// </summary>
    SeekPosition = 0x53AC,

    /// <summary>
    /// Represents the Info element type.
    /// </summary>
    Info = 0x1549A966,

    /// <summary>
    /// Represents the Duration element type.
    /// </summary>
    Duration = 0x4489,

    /// <summary>
    /// Represents the TimecodeScale element type.
    /// </summary>
    TimecodeScale = 0x2AD7B1,

    /// <summary>
    /// Represents the Cluster element type.
    /// </summary>
    Cluster = 0x1F43B675,

    /// <summary>
    /// Represents the Timecode element type.
    /// </summary>
    Timecode = 0xE7,

    /// <summary>
    /// Represents the SimpleBlock element type.
    /// </summary>
    SimpleBlock = 0xA3,

    /// <summary>
    /// Represents the BlockGroup element type.
    /// </summary>
    BlockGroup = 0xA0,

    /// <summary>
    /// Represents the Block element type.
    /// </summary>
    Block = 0xA1,

    /// <summary>
    /// Represents the BlockDuration element type.
    /// </summary>
    BlockDuration = 0x9B,

    /// <summary>
    /// Represents the ReferenceBlock element type.
    /// </summary>
    ReferenceBlock = 0xFB,

    /// <summary>
    /// Represents the Tracks element type.
    /// </summary>
    Tracks = 0x1654AE6B,

    /// <summary>
    /// Represents the TrackEntry element type.
    /// </summary>
    TrackEntry = 0xAE,

    /// <summary>
    /// Represents the TrackNumber element type.
    /// </summary>
    TrackNumber = 0xD7,

    /// <summary>
    /// Represents the TrackUid element type.
    /// </summary>
    TrackUid = 0x73C5,

    /// <summary>
    /// Represents the TrackType element type.
    /// </summary>
    TrackType = 0x83,

    /// <summary>
    /// Represents the Name element type.
    /// </summary>
    Name = 0x536E,

    /// <summary>
    /// Represents the CodecId element type.
    /// </summary>
    CodecId = 0x86,

    /// <summary>
    /// Represents the CodecPrivate element type.
    /// </summary>
    CodecPrivate = 0x63A2,

    /// <summary>
    /// Represents the Audio element type.
    /// </summary>
    Audio = 0xE1,

    /// <summary>
    /// Represents the SamplingFrequency element type.
    /// </summary>
    SamplingFrequency = 0xB5,

    /// <summary>
    /// Represents the OutputSamplingFrequency element type.
    /// </summary>
    OutputSamplingFrequency = 0x78B5,

    /// <summary>
    /// Represents the Channels element type.
    /// </summary>
    Channels = 0x9F,

    /// <summary>
    /// Represents the BitDepth element type.
    /// </summary>
    BitDepth = 0x6264,

    /// <summary>
    /// Represents the Cues element type.
    /// </summary>
    Cues = 0x1C53BB6B,

    /// <summary>
    /// Represents the CuePoint element type.
    /// </summary>
    CuePoint = 0xBB,

    /// <summary>
    /// Represents the CueTime element type.
    /// </summary>
    CueTime = 0xB3,

    /// <summary>
    /// Represents the CueTrackPositions element type.
    /// </summary>
    CueTrackPositions = 0xB7,

    /// <summary>
    /// Represents the CueTrack element type.
    /// </summary>
    CueTrack = 0xF7,

    /// <summary>
    /// Represents the CueClusterPosition element type.
    /// </summary>
    CueClusterPosition = 0xF1,
}
