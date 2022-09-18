// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio;

/// <inheritdoc />
public abstract class WriteOnlyAudioStream : AudioStream
{
    /// <inheritdoc />
    public override bool CanRead => false;

    /// <inheritdoc />
    public override bool CanWrite => true;

    /// <inheritdoc />
    public override int Read(Span<byte> buffer)
    {
        throw new NotSupportedException();
    }
}
