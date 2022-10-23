// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace NextAudio.Http;

/// <summary>
/// Options when creating a <see cref="PersistentHttpAudioStream" /> from an <see cref="Uri" />.
/// </summary>
public class PersistentHttpAudioStreamOptions
{
    /// <summary>
    /// Creates a new instance of <see cref="PersistentHttpAudioStreamOptions" />.
    /// </summary>
    /// <param name="requestUri">The request uri to get the stream.</param>
    /// <param name="length">The length of the stream.</param>
    /// <param name="httpClient">A http client do retrieve the audio stream.</param>
    /// <param name="loggerFactory">A logger factory to log audio streaming info.</param>
    public PersistentHttpAudioStreamOptions(Uri requestUri, long? length = null, HttpClient? httpClient = null, ILoggerFactory? loggerFactory = null)
    {
        ArgumentNullException.ThrowIfNull(requestUri);

        RequestUri = requestUri;
        Length = length ?? 0;
        HttpClient = httpClient ?? new();
        LoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
    }

    /// <summary>
    /// The request uri to retrive the audio stream.
    /// </summary>
    public virtual Uri RequestUri { get; set; }

    /// <summary>
    /// The length of the uri.
    /// </summary>
    public virtual long Length { get; set; }

    /// <summary>
    /// The max retry count if the request fails, the default value is 0.
    /// </summary>
    public virtual int MaxRetryCount { get; set; } = 2;

    /// <summary>
    /// A http client do retrieve the audio stream.
    /// </summary>
    public virtual HttpClient HttpClient { get; set; }

    /// <summary>
    /// The size of the buffer used by <see cref="BufferedStream" /> for buffering. The default
    /// buffer size is 4096. 0 or 1 means that buffering should be disabled. Negative
    /// values are not allowed.
    /// </summary>
    public virtual int BufferSize { get; set; } = 4096;

    /// <summary>
    /// A logger factory to log audio streaming info.
    /// </summary>
    public virtual ILoggerFactory LoggerFactory { get; set; }

    /// <summary>
    /// Creates a clone of the current <see cref="PersistentHttpAudioStreamOptions" />.
    /// </summary>
    /// <returns>A clone of the current <see cref="PersistentHttpAudioStreamOptions" />.</returns>
    public virtual PersistentHttpAudioStreamOptions Clone()
    {
        return new PersistentHttpAudioStreamOptions(RequestUri, Length, HttpClient, LoggerFactory)
        {
            MaxRetryCount = MaxRetryCount,
            BufferSize = BufferSize,
        };
    }
}
