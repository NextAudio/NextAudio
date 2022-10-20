// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NextAudio.Matroska.Models;

namespace NextAudio.Matroska;

/// <summary>
/// Represents a delegate which selects a <see cref="MatroskaTrack.TrackNumber" /> to be demuxed.
/// </summary>
/// <param name="tracks">An enumerable of <see cref="MatroskaTrack" /> to be chosen.</param>
/// <returns>The <see cref="MatroskaTrack.TrackNumber" /> to be demuxed.</returns>
public delegate ulong TrackSelector(IEnumerable<MatroskaTrack> tracks);

/// <summary>
/// Represents some options of <see cref="MatroskaDemuxer" />.
/// </summary>
public sealed class MatroskaDemuxerOptions
{
    /// <summary>
    /// The default instance of <see cref="MatroskaDemuxerOptions" />.
    /// </summary>
    public static readonly MatroskaDemuxerOptions Default = new();

    /// <summary>
    /// If the source stream should be disposed when the demuxer disposes.
    /// </summary>
    public bool DisposeSourceStream { get; set; }

    /// <summary>
    /// The track selector to be used to choose which track will be demuxed.
    /// </summary>
    public TrackSelector TrackSelector { get; set; } = DefaultTrackSelector;

    /// <summary>
    /// A logger factory to log audio streaming info.
    /// </summary>
    public ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;

    /// <summary>
    /// Creates a clone of the current <see cref="MatroskaDemuxerOptions" />.
    /// </summary>
    /// <returns>A clone of the current <see cref="MatroskaDemuxerOptions" />.</returns>
    public MatroskaDemuxerOptions Clone()
    {
        return new()
        {
            TrackSelector = TrackSelector,
            DisposeSourceStream = DisposeSourceStream,
            LoggerFactory = LoggerFactory,
        };
    }

    private static ulong DefaultTrackSelector(IEnumerable<MatroskaTrack> tracks)
    {
        foreach (var track in tracks)
        {
            if (track.Type == MatroskaTrackType.Audio)
            {
                return track.TrackNumber;
            }
        }

        return 1;
    }
}
