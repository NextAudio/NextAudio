// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.
using NextAudio.Matroska.Utils;

namespace NextAudio.Matroska.Models;

/// <summary>
/// Represents a Matroska Element
/// </summary>
public readonly ref struct MatroskaElement
{
    /// <summary>
    /// Creates a new instance of <see cref="MatroskaElement" />.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="position"></param>
    /// <param name="headerSize"></param>
    /// <param name="dataSize"></param>
    public MatroskaElement(ulong id, long position, int headerSize, int dataSize)
    {
        Id = id;
        Position = position;
        HeaderSize = headerSize;
        DataSize = dataSize;

        var type = (MatroskaElementType)id;

        Type = type;
        ValueType = type.GetEbmlValueType();
    }

    /// <summary>
    /// The Ebml id of the element.
    /// </summary>
    public ulong Id { get; }

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
    /// The end position of this element in the <see cref="AudioStream" />.
    /// </summary>
    public long EndPosition => Position + HeaderSize + DataSize;
}
