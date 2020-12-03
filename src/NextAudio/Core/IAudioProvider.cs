using System;
using System.Threading.Tasks;

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

        /// <summary>
        /// Search for track for the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="query">The query to be used for search.</param>
        /// <returns>The result of the search.</returns>
        ValueTask<SearchResult> SearchAsync(string query);
    }
}