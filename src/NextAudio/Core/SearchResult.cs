using System.Collections.Generic;

namespace NextAudio
{
    /// <summary>
    /// Represents a search result.
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="SearchResult" />.
        /// </summary>
        /// <param name="status">The status of the search result.</param>
        /// <param name="tracks">All tracks informations obtained by search.</param>
        /// <param name="selectedTrack">The selected track obtained by search if exists.</param>
        public SearchResult(SearchStatus status, IEnumerable<AudioTrackInfo> tracks, AudioTrackInfo? selectedTrack = null)
        {
            Status = status;
            Tracks = tracks;
            SelectedTrack = selectedTrack;
        }

        /// <summary>
        /// The status of the search result.
        /// </summary>
        public SearchStatus Status { get; }

        /// <summary>
        /// All tracks informations obtained by search.
        /// </summary>
        public IEnumerable<AudioTrackInfo> Tracks { get; }

        /// <summary>
        /// The selected track obtained by search if exists.
        /// </summary>
        public AudioTrackInfo? SelectedTrack { get; }
    }
}