// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;

namespace NextAudio.Http;

internal static partial class PersistentHttpAudioStreamLogging
{
    [LoggerMessage(1, LogLevel.Debug, "Exception throwed during reading.")]
    public static partial void LogExceptionWhenRead(this ILogger logger, Exception exception);
}
