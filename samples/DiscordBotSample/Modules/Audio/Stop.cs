using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBotSample.Modules.Audio
{
    public partial class AudioModule
    {
        [Command("stop")]
        public async Task StopAsync()
        {
            await Player.StopAsync();
        }
    }
}