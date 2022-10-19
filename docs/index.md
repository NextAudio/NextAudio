<p>
  <img class="logo" src="images/banner.png" />
</p>

# NextAudio Documentation

NextAudio is a library focused in audio operations like encoding, decoding, muxing, demuxing, resampling, etc.

It is designed to have simple usage but with streamming support with high performance.

# Getting Started
If this is your first time using NextAudio, you should refer to the [Introduction](/articles/introduction.html) in articles section.

If you wanna see all objects provided by this library you can access the [API Documentation](/api).


# Features

## Basic features
- High performance, low memory allocation with `Span<T>`, `Memory<T>`, `ArrayPool` and lower GC pressure with `ValueTask`.
- Fully Sync and Async support.
- Per audio frame reading/writing.
- Logging with `Microsoft.Extensions.Logging`.
- Symbols debugging (`.snupkg`).

## Containers demuxing support:
- Matroska (.mkv/.webm)

# Source License
NextAudio is licensed under [MIT License](https://github.com/NextAudio/NextAudio/blob/main/LICENSE).

The repository containing the source code for this library is in [Github](https://github.com/NextAudio/NextAudio).

NextAudio is built by [renanrcp](https://github.com/renanrcp).
