// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio;

/// <summary>
/// Some extensions methods for <see cref="AudioStream" />.
/// </summary>
public static class AudioStreamExtensions
{
    /// <summary>
    /// Casts a <see cref="Stream" /> from an <see cref="AudioStream" />.
    /// </summary>
    /// <param name="audioStream">The <see cref="AudioStream" /> to be cast.</param>
    /// <param name="options">The options for the cast.</param>
    /// <returns>A new <see cref="Stream" /> created from the <paramref name="audioStream" />.</returns>
    public static Stream CastToStream(this AudioStream audioStream, AudioStreamToStreamOptions? options = null)
    {
        return AudioStream.CastToStream(audioStream, options);
    }
}
