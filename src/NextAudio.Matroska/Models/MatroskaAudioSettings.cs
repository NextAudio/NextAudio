// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio.Matroska.Models;

/// <summary>
/// Represents some audio details for a Matroska Audio Track.
/// </summary>
public record class MatroskaAudioSettings
{
    /// <summary>
    /// The sampling frequency in Hz.
    /// </summary>
    public float SamplingFrequency { get; init; }

    /// <summary>
    /// The real output sampling frequency in Hz (used for SBR techniques).
    /// </summary>
    public float? OutputSamplingFrequency { get; init; }

    /// <summary>
    /// The number of channels in the track.
    /// </summary>
    public ulong Channels { get; init; }

    /// <summary>
    /// The bits per sample, mostly used for PCM.
    /// </summary>
    public ulong? BitDepth { get; init; }
}
