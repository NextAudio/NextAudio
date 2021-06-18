using System.Collections.Concurrent;

namespace DiscordBotSample.Services.Audio
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, AudioPlayer> _players;
    }
}