// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio;

/// <summary>
/// Options when creating a <see cref="Stream" /> from an <see cref="AudioStream" />.
/// </summary>
public sealed class AudioStreamToStreamOptions
{
    /// <summary>
    /// The default options <see cref="AudioStreamToStreamOptions" /> instance.
    /// </summary>
    public static readonly AudioStreamToStreamOptions Default = new();

    /// <summary>
    /// If the source audio stream should be disposed when the stream disposes.
    /// The default value is <see langword="true" />.
    /// </summary>
    public bool DisposeSourceStream { get; set; } = true;

    /// <summary>
    /// Creates a clone of the current <see cref="AudioStreamToStreamOptions" />.
    /// </summary>
    /// <returns>A clone of the current <see cref="AudioStreamToStreamOptions" />.</returns>
    public AudioStreamToStreamOptions Clone()
    {
        return new()
        {
            DisposeSourceStream = DisposeSourceStream,
        };
    }
}
