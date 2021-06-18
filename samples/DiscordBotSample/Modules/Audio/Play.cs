using Discord.Commands;
using DiscordBotSample.Contexts;
using System.Threading.Tasks;

namespace DiscordBotSample.Modules.Audio
{
    public partial class AudioModule : ModuleBase<ScopedSocketCommandContext>
    {
        [Command("play")]
        public async Task PlayAsync([Remainder] string query)
        {
            await ReplyAsync("Test");
        }
    }
}