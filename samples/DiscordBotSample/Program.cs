using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
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
                    });

                    services.AddSingleton(sp =>
                    {
                        var config = sp.GetRequiredService<IOptions<DiscordSocketConfig>>();

                        return new DiscordSocketClient(config.Value);
                    });
                    
                    services.AddHostedService<DiscordToMicrosoftLoggingService>();
                    services.AddHostedService<DiscordBotStartService>();
                });
    }
}
