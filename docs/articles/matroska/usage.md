## Usage
#### MatroskaDemuxer
A `MatroskaDemuxer` can be created from any Matroska `AudioStream`:

```csharp
using var file = AudioStream.CreateFromFile("test1.mkv");
using var demuxer = new MatroskaDemuxer(file);
```

Matroska supports many tracks inside the container, NextAudio will always select the first audio track inside the container, you change this behavior in the `MatroskaDemuxerOptions#TrackSelector`.

```csharp
using var file = AudioStream.CreateFromFile("test1.mkv");
using var demuxer = new MatroskaDemuxer(file, new MatroskaDemuxerOptions()
{
  DisposeSourceStream = false,
  TrackSelector = (tracks) =>
  {
    foreach (var track in tracks)
    {
      if (track.Type == MatroskaTrackType.Audio && track.CodecID.Contains("OPUS"))
      {
        return track.TrackNumber;
      }
    }

    return tracks.First().TrackNumber;
  },
  LoggerFactory = NullLoggerFactory.Instance,
});
```

In this example we select the first audio track with the `Opus` codec or return the first track.

`MatroskaDemuxer` has a lot of debugging logging if you need, don't forget to pass the `ILoggerFactory` instance in the `MatroskaDemuxerOptions` options.

#### Manually demuxing
The lib will always use audio frame buffers for read/write operations.

This makes the streaming data very easy.
```csharp
using var output = new MemoryStream();
using var file = AudioStream.CreateFromFile("test1.mkv");
using var demuxer = new MatroskaDemuxer(file);

var bytesRead = 0;
Span<byte> buffer = new byte[1024];

while ((bytesRead = demuxer.Demux(buffer)) > 0)
{
    // Always slice the buffer to bytesRead,
    // the lib always will return an audio frame
    // when using a read operation.
    var frame = buffer.Slice(0, bytesRead);
    output.Write(frame);
}
```
