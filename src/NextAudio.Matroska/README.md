# Documentation
See [Documentation](https://nextaudio.github.io/)

# Installation
You can add this libray via nuget package manager.

dotnet cli example:

```bash
dotnet add package NextAudio.Matroska
```

## Stable
The stable builds are available from [NuGet](https://www.nuget.org/profiles/NextAudio).

## Unstable
The development builds are availabe from Myget feed `https://www.myget.org/F/next-audio/api/v3/index.json`.
These builds target the `main` branch.

# Usage
The lib will always use audio frame buffers for read/write operations.

This makes the streaming data very easy.
```csharp
using var output = new MemoryStream();
using var file = AudioStream.CreateFromFile("test1.mkv");
using var demuxer = new MatroskaDemuxer(file);

var bytesReaded = 0;
Span<byte> buffer = new byte[1024];

while ((bytesReaded = demuxer.Demux(buffer)) > 0)
{
    // Always slice the buffer to bytesReaded,
    // the lib always will return an audio frame
    // when using a read operation.
    var frame = buffer.Slice(0, bytesReaded);
    output.Write(frame);
}
```
For more usage and guides check the [documentation](https://nextaudio.github.io/)

# Features
- Demuxing
- Ebml values reading (from buffers)

# Thanks and References
[Matroska Website](https://www.matroska.org/index.html)
[OlegZee/NEbml](https://github.com/OlegZee/NEbml)
[sedmelluq/lavaplayer](https://github.com/sedmelluq/lavaplayer)
[hasenbanck/matroska-demuxer](https://github.com/hasenbanck/matroska-demuxer)
[StefH/Matroska](https://github.com/StefH/Matroska)
