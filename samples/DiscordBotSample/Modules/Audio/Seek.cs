using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DiscordBotSample.Modules.Audio
{
    public partial class AudioModule
    {
        [Command("seek")]
        public async Task SeekAsync([Remainder] TimeSpan time)
        {
            await Player.SeekAsync(time);
        }
    }
}