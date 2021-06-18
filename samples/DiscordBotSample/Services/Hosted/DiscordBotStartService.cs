using Discord;
using Discord.WebSocket;
using DiscordBotSample.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotSample.Services.Hosted
{
    public class DiscordBotStartService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly DiscordSocketClient _client;

        public DiscordBotStartService(IConfiguration configuration, DiscordSocketClient client)
        {
            _configuration = configuration;
            _client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var options = _configuration.GetSection("Bot").Get<BotOptions>();

            await _client.LoginAsync(TokenType.Bot, options.Token);
            await _client.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await _client.SetStatusAsync(UserStatus.Invisible);
            await _client.StopAsync();
        }
    }
}