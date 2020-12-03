namespace NextAudio
{
    /// <summary>
    /// Represents a status of a perfomed search.
    /// </summary>
    public enum SearchStatus
    {
        /// <summary>
        /// The search result is a single track.
        /// </summary>
        TrackLoaded = 0,

        /// <summary>
        /// The search result is a playlist with one or many tracks.
        /// </summary>
        PlaylistLoaded = 1,

        /// <summary>
        /// The search result is a search that can contains a lot of loadable tracks.
        /// </summary>
        SearchResult = 2,
    }
}