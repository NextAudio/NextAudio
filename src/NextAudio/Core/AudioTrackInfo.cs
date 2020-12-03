using System;

namespace NextAudio
{
    /// <summary>
    /// Represents some information about an audio track.
    /// </summary>
    public class AudioTrackInfo
    {
        /// <summary>
        /// Creates a new instance of <see cref="AudioTrackInfo" />.
        /// </summary>
        /// <param name="title">The title of the track.</param>
        /// <param name="author">The author of the track if has any.</param>
        /// <param name="length">The length of the track if is not a stream.</param>
        /// <param name="providerIdentifier">The identifier of the track for the provider if has any.</param>
        /// <param name="isStream">Indicates if the track is a stream.</param>
        /// <param name="uri">The uri of the track if has any.</param>
        public AudioTrackInfo(string title, string? author = null, string? providerIdentifier = null, long? length = null, Uri? uri = null, bool isStream = false)
        {
            Title = title;
            Author = author;
            Length = length;
            ProviderIdentifier = providerIdentifier;
            IsStream = isStream;
            Uri = uri;
        }

        /// <summary>
        /// The title of the track.
        /// </summary>
        public virtual string Title { get; }

        /// <summary>
        /// The author of the track if has any.
        /// </summary>
        public virtual string? Author { get; }

        /// <summary>
        /// The length of the track if is not a stream.
        /// </summary>
        /// <value></value>
        public virtual long? Length { get; }

        /// <summary>
        /// The identifier of the track for the provider if has any.
        /// </summary>
        public virtual string? ProviderIdentifier { get; }

        /// <summary>
        /// Indicates if the track is a stream.
        /// </summary>
        public virtual bool IsStream { get; }

        /// <summary>
        /// The uri of the track if has any.
        /// </summary>
        public virtual Uri? Uri { get; }
    }
}