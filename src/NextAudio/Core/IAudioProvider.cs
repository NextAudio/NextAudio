using System;
using System.Threading;
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
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>The result of the search.</returns>
        ValueTask<SearchResult> SearchAsync(string query, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the audio track for the specified <see cref="AudioTrackInfo" />.
        /// </summary>
        /// <param name="trackInfo">The track info to get the audio track.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>An <see cref="AudioTrack" /> instance.</returns>
        Task<AudioTrack> GetAudioTrackAsync(AudioTrackInfo trackInfo, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the audio track for the specified <paramref name="identifier" />.
        /// </summary>
        /// <param name="identifier">The identifier to get the audio track.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>An <see cref="AudioTrack" /> instance.</returns>
        Task<AudioTrack> GetAudioTrackAsync(string identifier, CancellationToken cancellationToken = default);
    }
}