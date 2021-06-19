using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBotSample.Modules.Audio
{
    public partial class AudioModule
    {
        [Command("volume")]
        public async Task VolumeAsync(int volume)
        {
            await Player.SetVolumeAsync(volume);
        }
    }
}