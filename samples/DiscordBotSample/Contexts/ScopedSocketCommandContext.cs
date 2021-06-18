using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DiscordBotSample.Contexts
{
    public class ScopedSocketCommandContext : SocketCommandContext, IAsyncDisposable
    {
        private readonly IServiceScope _scope;

        public ScopedSocketCommandContext(IServiceScope scope, DiscordSocketClient client, SocketUserMessage msg) : base(client, msg)
        {
            _scope = scope;
        }

        public IServiceProvider CommandServices => _scope.ServiceProvider;

        public async ValueTask DisposeAsync()
        {
            switch (_scope)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync();
                    break;

                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
    }
}