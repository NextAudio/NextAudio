// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Matroska.Models;

/// <summary>
/// Enum which maps all Ebml value types.
/// </summary>
public enum EbmlValueType
{
    /// <summary>
    /// None element, mapped because enum require default value, this type shouldn't exists.
    /// </summary>
    None = 0,

    /// <summary>
    /// This element value is a signed integer.
    /// </summary>
    SignedInteger,

    /// <summary>
    /// This element value is a unsigned integer.
    /// </summary>
    UnsignedInteger,

    /// <summary>
    /// This element value is a floating point number.
    /// </summary>
    Float,

    /// <summary>
    /// This element value is a string in the ASCII encoding.
    /// </summary>
    AsciiString,

    /// <summary>
    /// This element value is a string in the UTF-8 encoding.
    /// </summary>
    Utf8String,

    /// <summary>
    /// This element value is a date.
    /// </summary>
    Date,

    /// <summary>
    /// This element value is binary data.
    /// </summary>
    Binary,

    /// <summary>
    /// This element value is a container for other EBML sub-elements.
    /// </summary>
    MasterElement,
}
