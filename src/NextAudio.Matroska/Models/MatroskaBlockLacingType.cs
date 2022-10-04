// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Matroska.Models;

/// <summary>
/// Represents wich lacing type a <see cref="MatroskaBlock" /> is using.
/// </summary>
public enum MatroskaBlockLacingType : byte
{
    /// <summary>
    /// The block don't uses any lacing type.
    /// </summary>
    No = 0,

    /// <summary>
    /// The block uses xiph lacing type.
    /// </summary>
    Xiph = 2,

    /// <summary>
    /// The block uses fixed size lacing type.
    /// </summary>
    FixedSize = 4,

    /// <summary>
    /// The block uses ebml lacing type.
    /// </summary>
    Ebml = 6,
}
