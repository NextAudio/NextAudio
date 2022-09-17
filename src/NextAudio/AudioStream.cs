// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio;

/// <summary>
/// Represents a basic audio stream.
/// </summary>
public abstract class AudioStream : Stream
{
    /// <summary>
    /// Creates a clone of this stream.
    /// </summary>
    /// <returns>A new cloned stream.</returns>
    public abstract AudioStream Clone();
}
