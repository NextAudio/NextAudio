// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Matroska.Models;

/// <summary>
/// Represents a Matroska block of data with 1 or more frames.
/// </summary>
public readonly struct MatroskaBlock
{
    private readonly int[] _frameSizes;

    /// <summary>
    /// Creates a new instance of <see cref="MatroskaBlock" />.
    /// </summary>
    /// <param name="trackNumber">The track number which this block is for.</param>
    /// <param name="lacingType">The lacing type that block uses.</param>
    /// <param name="frameCount">The number of frames in this block.</param>
    /// <param name="frameSizes">All frames sizes in this block.</param>
    public MatroskaBlock(ulong trackNumber, MatroskaBlockLacingType lacingType, int frameCount, int[] frameSizes)
    {
        TrackNumber = trackNumber;
        LacingType = lacingType;
        FrameCount = frameCount;
        _frameSizes = frameSizes;
    }

    /// <summary>
    /// The track number which this block is for.
    /// </summary>
    public ulong TrackNumber { get; }

    /// <summary>
    /// The lacing type that block uses.
    /// </summary>
    public MatroskaBlockLacingType LacingType { get; }

    /// <summary>
    /// The number of frames in this block.
    /// </summary>
    public int FrameCount { get; }

    /// <summary>
    /// Get the frame size by the especified <paramref name="index" />.
    /// </summary>
    /// <param name="index">The index of the frame to be retained.</param>
    /// <exception cref="ArgumentException">The <paramref name="index" /> cannot be equals or higher than the <see cref="FrameCount" />.</exception>
    /// <returns>The frame size of the especified <paramref name="index" />.</returns>
    public int GetFrameSizeByIndex(int index)
    {
        return index >= FrameCount
            ? throw new ArgumentException($"The '{nameof(index)}' cannot be equals or higher than the '{nameof(FrameCount)}'.", nameof(index))
            : _frameSizes[index];
    }
}
