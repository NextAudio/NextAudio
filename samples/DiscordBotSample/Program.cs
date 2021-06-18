using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotSample.Services.Audio;
using DiscordBotSample.Services.Hosted;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DiscordBotSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<DiscordSocketConfig>(config =>
                    {
                        config.AlwaysDownloadUsers = false;

                        // ASP.NET Core logging will decide the correct.
                        config.LogLevel = LogSeverity.Debug;
                    });
                    services.Configure<CommandServiceConfig>(config =>
                    {
                        config.DefaultRunMode = RunMode.Async;
                        config.SeparatorChar = ' ';
                        config.CaseSensitiveCommands = false;
                        config.IgnoreExtraArgs = true;

                        // ASP.NET Core logging will decide the correct.
                        config.LogLevel = LogSeverity.Debug;
                    });

                    services.AddSingleton(sp =>
                    {
                        var config = sp.GetRequiredService<IOptions<DiscordSocketConfig>>();

                        return new DiscordSocketClient(config.Value);
                    });
                    services.AddSingleton(sp =>
                    {
                        var config = sp.GetRequiredService<IOptions<CommandServiceConfig>>();

                        return new CommandService(config.Value);
                    });

                    services.AddSingleton<AudioService>();
                    
                    services.AddHostedService<DiscordToMicrosoftLoggingService>();
                    services.AddHostedService<DiscordBotStartService>();
                    services.AddHostedService<DiscordCommandHandlerService>();
                });
    }
}
