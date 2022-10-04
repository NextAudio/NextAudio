// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace NextAudio;

/// <summary>
/// Options when creating an <see cref="AudioStream" /> from a <see cref="Stream" />.
/// </summary>
public class StreamToAudioStreamOptions
{
    /// <summary>
    /// The default options <see cref="StreamToAudioStreamOptions" /> instance.
    /// </summary>
    public static readonly StreamToAudioStreamOptions Default = new();

    /// <summary>
    /// If the source stream should be disposed when the demuxer disposes.
    /// </summary>
    public bool DisposeSourceStream { get; set; }

    /// <summary>
    /// The recomended synchronicity operation to use when read/write the source <see cref="Stream" />.
    /// </summary>
    public RecommendedSynchronicity RecommendedSynchronicity { get; set; }

    /// <summary>
    /// A logger factory to log audio streaming info.
    /// </summary>
    public ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;

    /// <summary>
    /// Creates a clone of the current <see cref="StreamToAudioStreamOptions" />.
    /// </summary>
    /// <returns>A clone of the current <see cref="StreamToAudioStreamOptions" />.</returns>
    public StreamToAudioStreamOptions Clone()
    {
        return new()
        {
            DisposeSourceStream = DisposeSourceStream,
            RecommendedSynchronicity = RecommendedSynchronicity,
            LoggerFactory = LoggerFactory,
        };
    }
}
