// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Xunit;

namespace NextAudio.Matroska.UnitTests;

public class MatroskaDemuxerTests
{
    [Fact]
    public void SourceSeekIsReversedWhenDemuxNextFrame()
    {
        using var file = File.Open("assets/test1.mkv", FileMode.Open);
        using var demuxer = new MatroskaDemuxer(file);

        var buffer = new byte[1024];

        // Starts the demuxer and read the first frame
        _ = demuxer.Demux(buffer);
        var oldPosition = file.Position;

        // Seek the source file
        _ = file.Seek(0, SeekOrigin.Begin);

        // Read the second frame
        _ = demuxer.Demux(buffer);
        var newPosition = file.Position;

        Assert.NotEqual(0, file.Position);
        Assert.NotEqual(oldPosition, newPosition);
        Assert.True(newPosition > oldPosition);
    }

    [Fact]
    public void CanSeekReturnsFalse()
    {
        using var stream = new MemoryStream();
        using var demuxer = new MatroskaDemuxer(stream);

        var result = demuxer.CanSeek;

        Assert.False(result);
    }

    [Fact]
    public void SeekThrowsNotSupportedException()
    {
        using var stream = new MemoryStream();
        using var demuxer = new MatroskaDemuxer(stream);

        _ = Assert.Throws<NotSupportedException>(() =>
        {
            _ = demuxer.Seek(1000, SeekOrigin.Begin);
        });
    }

    [Fact]
    public void DisposeDisposesSourceStreamIfOptionsConfigureToNotDisposeSourceStream()
    {
        var stream = new MemoryStream();
        var demuxer = new MatroskaDemuxer(stream, new()
        {
            DisposeSourceStream = true,
        });

        demuxer.Dispose();

        _ = Assert.Throws<ObjectDisposedException>(() =>
        {
            _ = stream.ReadByte();
        });
    }

    [Fact]
    public async Task DisposeAsyncDisposesSourceStreamIfOptionsConfigureToNotDisposeSourceStream()
    {
        var stream = new MemoryStream();
        var demuxer = new MatroskaDemuxer(stream, new()
        {
            DisposeSourceStream = true,
        });

        await demuxer.DisposeAsync();

        _ = Assert.Throws<ObjectDisposedException>(() =>
        {
            _ = stream.ReadByte();
        });
    }

    [Fact]
    public void CloneReturnsANewDemuxerAndChangeTheCurrentOptionsToNotDisposeSourceStream()
    {
        var stream = new MemoryStream(new byte[] { 1, 2 });
        var options = new MatroskaDemuxerOptions()
        {
            DisposeSourceStream = true,
        };
        var demuxer = new MatroskaDemuxer(stream, options);

        var demuxerResult = demuxer.Clone();

        // Don't dispose the source stream.
        demuxer.Dispose();

        // If source stream disposed will throw exception.
        var byteResult = stream.ReadByte();

        demuxerResult.Dispose();

        Assert.NotNull(demuxerResult);
        Assert.Equal(1, byteResult);
        _ = Assert.Throws<ObjectDisposedException>(() =>
        {
            _ = stream.ReadByte();
        });
    }
}
