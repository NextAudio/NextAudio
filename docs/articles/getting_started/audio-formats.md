## Audio Formats
Every `AudioTrack` has your own `AudioFormat`, let's understand what this means.

### AudioFormat
The `AudioFormat` class has two properties: `AudioContainer` and `AudioCoding`.

## AudioContainer
The `AudioContainer` represents which container this format is using eg. (`Matroska`/`Ogg`).

If you don't understand what is an audio container is, see the [glossary](/glossary.html).

The `AudioContainer` can also be other inherited class for this specific container with additional info eg. (`MatroskaAudioContainer` for `Matroska`).

If the container is unmapped in the NextAudio it will return an `AudioContainerType.Unknown` in the `Type` property.

## AudioCoding
The `AudioCoding` represents which coding this format is using eg. (`PCM`/`Opus`/`Vorbis`).

If you don't understand what is an audio coding is, see the [glossary](/glossary.html).

The `AudioCoding` can also be other inherited class for this specific container with additional info eg. (`PCMAudioCoding` for `PCM`).

If the coding is unmapped in the NextAudio it will return an `AudioCodingType.Unknown` in the `Type` property.
