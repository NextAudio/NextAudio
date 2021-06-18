using Discord.Commands;
using DiscordBotSample.Contexts;
using DiscordBotSample.Services.Audio;

namespace DiscordBotSample.Modules.Audio
{
    [RequireContext(ContextType.Guild)]
    public partial class AudioModule : ModuleBase<ScopedSocketCommandContext>
    {
        private readonly AudioService _audioService;

        public AudioModule(AudioService audioService)
        {
            _audioService = audioService;
        }
    }
}