// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

namespace NextAudio;

/// <summary>
/// The recomended synchronicity operation to use when read/write.
/// </summary>
/// <remarks>
/// This doesn't define the synchronicity support, all are supported.
/// </remarks>
public enum RecommendedSynchronicity
{
    /// <summary>
    /// Any synchronicity can be used the source stream supports any very well.
    /// </summary>
    Any = 0,

    /// <summary>
    /// The recommended synchronicity is synchronous.
    /// </summary>
    Sync = 1,

    /// <summary>
    /// The recommended synchronicity is asynchronous.
    /// </summary>
    Async = 2,
}
