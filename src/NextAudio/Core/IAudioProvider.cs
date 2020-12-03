using System;

namespace NextAudio
{
    /// <summary>
    /// Represents an audio provider that can search and load audio tracks.
    /// </summary>
    public interface IAudioProvider : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// The name of this audio provider.
        /// </summary>
        string Name { get; }
    }
}