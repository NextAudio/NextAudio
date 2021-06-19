using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBotSample.Modules.Audio
{
    public partial class AudioModule
    {
        [Command("resume")]
        public async Task ResumeAsync()
        {
            await Player.ResumeAsync();
        }
    }
}