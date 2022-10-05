<p align="center">
  <img src="banner.png" />
	</br>
	<a href="https://github.com/NextAudio/NextAudio/actions">
	<img src="https://img.shields.io/endpoint?label=BUILD%20STATUS&logo=github&logoWidth=20&labelColor=0d0d0d&style=for-the-badge&url=https://github-workflow-status-badge.vercel.app/api/NextAudio/NextAudio/build-test-lint.yml" />
	</a>
	<a href="https://dotnet.microsoft.com/download">
		<img src="https://img.shields.io/badge/dotnet-6+-blueviolet?label=.NET&logo=.net&style=for-the-badge&logoWidth=20&labelColor=0d0d0d" />
	</a>
	<a href="https://www.nuget.org/packages/NextAudio/">
		<img src="https://img.shields.io/nuget/v/NextAudio.svg?label=Version&logo=nuget&style=for-the-badge&logoWidth=20&labelColor=0d0d0d" />
	</a>
	<a href="https://www.nuget.org/packages/NextAudio/">
		<img src="https://img.shields.io/nuget/dt/NextAudio.svg?label=Downloads&logo=nuget&style=for-the-badge&logoWidth=20&labelColor=0d0d0d" />
	</a>
    <a href="https://github.com/renanrcp/NextAudio/NextAudio/blob/main/LICENSE">
		<img src="https://img.shields.io/badge/License-MIT-yellow.svg?label=License&style=for-the-badge&logoWidth=20&labelColor=0d0d0d" />
	</a>
    <p align="center">
        Create audio apps without headache in the .NET ecosystem.
    </p>
	<p align="center">
	This library allows you to create audio apps without a lot of headache existing in the "audio topic"
	</p>
</p>

# Documentation
See [Documentation](https://nextaudio.github.io/)

# Installation
You can add this libray via nuget package manager.

dotnet cli example:

```bash
dotnet add package NextAudio
```
OBS: This install only the core lib. The individual components may also be installed from [NuGet](https://www.nuget.org/profiles/NextAudio).

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
#### Basic features
- High performance, low memory allocation with `Span<T>`, `Memory<T>`, `ArrayPool` and lower GC pressure with `ValueTask`.
- Fully Sync and Async support.
- Per audio frame reading/writing.
- Logging with `Microsoft.Extensions.Logging`.
- Symbols debugging (`.snupkg`).

#### Containers demuxing support:
- Matroska (.mkv/.webm)

# Goals
OBS: Goals will change as new lib versions come out.

#### Audio stream providers support:
- File (current supported by `AudioStream.CreateFromFile`)
- Http (current supported by `AudioStream.CreateFromStream` but is not persistent)
- Youtube

#### Containers demuxing support:
- Ogg (.ogg/.ogv/.oga/.ogx)
- Wav (.wav/.wave)
- MPEG-4 Part 14 (.mp4)
- Flac (.flac)

#### Decoders support:
- Opus (.opus)
- Vorbis (.ogg/.oga)
- AAC (.m4a/.m4b/m4p/m4v/.m4r/.aac/.3gp/.mp4)
- MPEG-1/2 Audio Layer 3 (.mp3/.mp4)

#### Encoders support:
- Opus (.opus)

#### Containers probe support:
- Matroska (.mkv/.webm)
- Ogg (.ogg/.ogv/.oga/.ogx)
- Wav (.wav/.wave)
- MPEG-4 Part 14 (.mp4)
- Flac (.flac)

#### Others audio operations
- Seeking (by timestamp)
- Volume
- Resampling
- Mono to Stereo

#### Others
- Package with all lib individual components
- Audio Pipelines (less complexity when wanna a specific output format)
- Support for `Microsoft.Extensions.DependencyInjection`
- Writing to output formats (`PipelineWriter`/`Stream`)
- An Audio Player structure to control play/queue/pause/seek/volume operations

# Contributing

To contribute to this library follow the [Contributing guideline](https://github.com/NextAudio/NextAudio/blob/main/CONTRIBUTING.md)

# License

This project uses the [MIT License](https://github.com/NextAudio/NextAudio/blob/main/LICENSE).

# Code of Conduct
See [Code of Conduct](https://github.com/NextAudio/NextAudio/blob/main/CODE-OF-CONDUCT.MD)
