// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Xunit;

namespace NextAudio.Matroska.FunctionalTests;

public class MatroskaDemuxerTests
{
    public static IEnumerable<object[]> MatroskaFileData()
    {
        // This file has 2 audio files, we will choose the last because it has some xiph block lacings.
        TrackSelector file5Selector = (tracks) => 10;

        yield return new object[] { "assets/test1.mkv", MatroskaDemuxerOptions.Default.TrackSelector, 576 };
        yield return new object[] { "assets/test2.mkv", MatroskaDemuxerOptions.Default.TrackSelector, 512 };
        yield return new object[] { "assets/test3.mkv", MatroskaDemuxerOptions.Default.TrackSelector, 574 };

        // This file has elements with undefined size in this case we can't demux.
        yield return new object[] { "assets/test4.mkv", MatroskaDemuxerOptions.Default.TrackSelector, 0 };

        yield return new object[] { "assets/test5.mkv", file5Selector, 134 };
        yield return new object[] { "assets/test6.mkv", MatroskaDemuxerOptions.Default.TrackSelector, 576 };

        // This file has a broken element but we can demux some frames before.
        yield return new object[] { "assets/test7.mkv", MatroskaDemuxerOptions.Default.TrackSelector, 485 };

        yield return new object[] { "assets/test8.mkv", MatroskaDemuxerOptions.Default.TrackSelector, 519 };
    }

    [Theory]
    [MemberData(nameof(MatroskaFileData))]
    public void CanSyncDemuxAnyMatroskaFile(string path, TrackSelector trackSelector, int expectedLastFrameSize)
    {
        using var file = File.Open(path, FileMode.Open, FileAccess.Read);
        using var demuxer = new MatroskaDemuxer(file, new()
        {
            TrackSelector = trackSelector,
        });

        var buffer = new byte[1024];

        var bytesReaded = 0;
        var lastFrameSize = 0;

        while ((bytesReaded = demuxer.Demux(buffer)) > 0)
        {
            lastFrameSize = bytesReaded;
        }

        // It's basically impossible to check if all frames are readed correct
        // We will just check the last because if the last are correct,
        // the others are probably correct too.
        // Also the elements, blocks and VInt reading are covering in unit tests.
        Assert.Equal(expectedLastFrameSize, lastFrameSize);
    }

    [Theory]
    [MemberData(nameof(MatroskaFileData))]
    public async Task CanAsyncDemuxAnyMatroskaFile(string path, TrackSelector trackSelector, int expectedLastFrameSize)
    {
        using var file = File.Open(path, FileMode.Open, FileAccess.Read);
        using var demuxer = new MatroskaDemuxer(file, new()
        {
            TrackSelector = trackSelector,
        });

        var buffer = new byte[1024];

        var bytesReaded = 0;
        var lastFrameSize = 0;

        while ((bytesReaded = await demuxer.DemuxAsync(buffer)) > 0)
        {
            lastFrameSize = bytesReaded;
        }

        // It's basically impossible to check if all frames are readed correct
        // We will just check the last because if the last are correct,
        // the others are probably correct too.
        // Also the elements, blocks and VInt reading are covering in unit tests.
        Assert.Equal(expectedLastFrameSize, lastFrameSize);
    }
}
