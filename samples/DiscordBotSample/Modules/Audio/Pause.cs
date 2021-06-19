using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBotSample.Modules.Audio
{
    public partial class AudioModule
    {
        [Command("pause")]
        public async Task PauseAsync()
        {
            await Player.PauseAsync();
        }
    }
}