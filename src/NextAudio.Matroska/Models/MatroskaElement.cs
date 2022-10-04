// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.
using System.Diagnostics;
using NextAudio.Matroska.Utils;

namespace NextAudio.Matroska.Models;

/// <summary>
/// Represents a Matroska Element
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public readonly struct MatroskaElement
{
    /// <summary>
    /// Creates a new instance of <see cref="MatroskaElement" />.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="depth"></param>
    /// <param name="position"></param>
    /// <param name="headerSize"></param>
    /// <param name="dataSize"></param>
    public MatroskaElement(ulong id, int depth, long position, int headerSize, int dataSize)
    {
        Id = id;
        Depth = depth;
        Position = position;
        HeaderSize = headerSize;
        DataSize = dataSize;
        DataPosition = Position + HeaderSize;
        EndPosition = Position + HeaderSize + DataSize;

        Type = MatroskaUtils.GetMatroskaElementType(Id);
        ValueType = MatroskaUtils.GetEbmlValueType(Type);
    }

    /// <summary>
    /// The Ebml id of the element.
    /// </summary>
    public ulong Id { get; }

    /// <summary>
    /// The depth of this element (elements can be inside each other).
    /// </summary>
    public int Depth { get; }

    /// <summary>
    /// The mapped matroska element type according with the <see cref="Id" />.
    /// </summary>
    public MatroskaElementType Type { get; }

    /// <summary>
    /// The Ebml value type of this element.
    /// </summary>
    public EbmlValueType ValueType { get; }

    /// <summary>
    /// The position of this element in the <see cref="AudioStream" />.
    /// </summary>
    public long Position { get; }

    /// <summary>
    /// The size of the header containing in this element.
    /// </summary>
    public int HeaderSize { get; }

    /// <summary>
    /// The size of the data containing in this element.
    /// </summary>
    public int DataSize { get; }

    /// <summary>
    /// The position of this element data in the <see cref="AudioStream" />.
    /// </summary>
    public long DataPosition { get; }

    /// <summary>
    /// The end position of this element in the <see cref="AudioStream" />.
    /// </summary>
    public long EndPosition { get; }

    /// <summary>
    /// Get the remaining number of bytes from the <paramref name="position" /> to the end of this element.
    /// </summary>
    /// <param name="position">The position of the stream to be checked.</param>
    /// <returns>The remaining number of bytes from the <paramref name="position" /> to the end of this element.</returns>
    public long GetRemaining(long position)
    {
        return Position + HeaderSize + DataSize - position;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"0x{Id:X} {Type} - {ValueType} [{DataSize} bytes]";
}
