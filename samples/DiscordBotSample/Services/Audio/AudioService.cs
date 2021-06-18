using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DiscordBotSample.Services.Audio
{
    public class AudioService : IAsyncDisposable
    {
        private readonly ConcurrentDictionary<ulong, AudioPlayer> _players;
        private readonly DiscordSocketClient _client;

        public AudioService(DiscordSocketClient client)
        {
            _client = client;

            _players = new();
        }

        public AudioPlayer CreatePlayer(SocketGuild guild)
        {
            return _players.GetOrAdd(guild.Id, (_) =>
            {
                return new AudioPlayer(guild);
            });
        }

        public AudioPlayer GetPlayer(SocketGuild guild)
            => GetPlayer(guild.Id);

        public AudioPlayer GetPlayer(ulong guildId)
        {
            _ = _players.TryGetValue(guildId, out var player);

            return player;
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var player in _players.Values)
                await player.DisposeAsync();
        }
    }
}