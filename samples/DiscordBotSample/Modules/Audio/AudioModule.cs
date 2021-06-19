using Discord.Commands;
using DiscordBotSample.Contexts;
using DiscordBotSample.Services.Audio;

namespace DiscordBotSample.Modules.Audio
{
    [RequireContext(ContextType.Guild)]
    public sealed partial class AudioModule : ModuleBase<ScopedSocketCommandContext>
    {
        private readonly AudioService _audioService;

        public AudioModule(AudioService audioService)
        {
            _audioService = audioService;
        }

        private AudioPlayer Player => _audioService.GetPlayer(Context.Guild) ?? _audioService.CreatePlayer(Context.Guild);
    }
}