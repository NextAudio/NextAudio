using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotSample.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBotSample.Services.Hosted
{
    public class DiscordCommandHandlerService : IHostedService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;

        public DiscordCommandHandlerService(DiscordSocketClient client, CommandService commands, IServiceProvider provider)
        {
            _client = client;
            _commands = commands;
            _provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            await _commands.AddModulesAsync(typeof(DiscordCommandHandlerService).Assembly, _provider);

            _client.MessageReceived += MessageReceivedAsync;
            _commands.CommandExecuted += CommandExecutedAsync;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

            _client.MessageReceived -= MessageReceivedAsync;
            _commands.CommandExecuted -= CommandExecutedAsync;

            return Task.CompletedTask;
        }

        private Task CommandExecutedAsync(Optional<CommandInfo> arg1, ICommandContext arg2, IResult arg3)
        {
            if (arg2 is not ScopedSocketCommandContext context)
                return Task.CompletedTask;

            return context.DisposeAsync().AsTask();
        }

        private Task MessageReceivedAsync(SocketMessage arg)
        {
            if (arg is not SocketUserMessage message)
                return Task.CompletedTask;

            var argPos = 0;

            if (message.Author.IsBot || !message.HasStringPrefix("x!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
                return Task.CompletedTask;

            var scopeFactory = _provider.GetRequiredService<IServiceScopeFactory>();
            var scope = scopeFactory.CreateScope();

            var context = new ScopedSocketCommandContext(scope, _client, message);

            return _commands.ExecuteAsync(context, argPos, scope.ServiceProvider, MultiMatchHandling.Best);
        }
    }
}