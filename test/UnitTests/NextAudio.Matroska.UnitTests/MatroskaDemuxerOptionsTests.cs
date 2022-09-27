// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using NextAudio.Matroska.Models;
using Xunit;

namespace NextAudio.Matroska.UnitTests;

public class MatroskaDemuxerOptionsTests
{
    public static IEnumerable<object[]> DefaultTrackSelectorChooseSelectFirstAudioTrackWhenHasAudioTracksData()
    {
        yield return new object[]
        {
            new MatroskaTrack[]
            {
                new MatroskaTrack("V_VP9") { TrackNumber = 1, Type = MatroskaTrackType.Video },
                new MatroskaTrack("S_TEXT/UTF8") { TrackNumber = 2, Type = MatroskaTrackType.Subtitle },
                new MatroskaTrack("A_OPUS") { TrackNumber = 3, Type = MatroskaTrackType.Audio },
                new MatroskaTrack("A_MPEG/L3") { TrackNumber = 4, Type = MatroskaTrackType.Audio },
            },
            3
        };
        yield return new object[]
        {
            new MatroskaTrack[]
            {
                new MatroskaTrack("S_TEXT/UTF8") { TrackNumber = 1, Type = MatroskaTrackType.Subtitle },
                new MatroskaTrack("V_VP9") { TrackNumber = 2, Type = MatroskaTrackType.Video },
                new MatroskaTrack("A_MPEG/L3") { TrackNumber = 3, Type = MatroskaTrackType.Audio },
                new MatroskaTrack("A_OPUS") { TrackNumber = 4, Type = MatroskaTrackType.Audio },
            },
            3
        };
    }

    [Theory]
    [MemberData(nameof(DefaultTrackSelectorChooseSelectFirstAudioTrackWhenHasAudioTracksData))]
    public void DefaultTrackSelectorChooseSelectFirstAudioTrackWhenHasAudioTracks(IEnumerable<MatroskaTrack> tracks, ulong expectedTrackNumber)
    {
        var options = new MatroskaDemuxerOptions();

        var result = options.TrackSelector(tracks);

        Assert.Equal(expectedTrackNumber, result);
    }

    public static IEnumerable<object[]> DefaultTrackSelectorChooseSelectFirstTrackWhenDoesntHasAnyAudioTracksData()
    {
        yield return new object[]
        {
            new MatroskaTrack[]
            {
                new MatroskaTrack("V_VP9") { TrackNumber = 1, Type = MatroskaTrackType.Video },
                new MatroskaTrack("S_TEXT/UTF8") { TrackNumber = 2, Type = MatroskaTrackType.Subtitle },
            },
            1
        };
        yield return new object[]
        {
            new MatroskaTrack[]
            {
                new MatroskaTrack("S_TEXT/UTF8") { TrackNumber = 1, Type = MatroskaTrackType.Subtitle },
                new MatroskaTrack("V_VP9") { TrackNumber = 2, Type = MatroskaTrackType.Video },
            },
            1
        };
    }

    [Theory]
    [MemberData(nameof(DefaultTrackSelectorChooseSelectFirstTrackWhenDoesntHasAnyAudioTracksData))]
    public void DefaultTrackSelectorChooseSelectFirstTrackWhenDoesntHasAnyAudioTracks(IEnumerable<MatroskaTrack> tracks, ulong expectedTrackNumber)
    {
        var options = new MatroskaDemuxerOptions();

        var result = options.TrackSelector(tracks);

        Assert.Equal(expectedTrackNumber, result);
    }
}
