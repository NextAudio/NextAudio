using Discord.Commands;
using Discord.WebSocket;
using DiscordBotSample.Contexts;
using DiscordBotSample.Services.Audio;
using NextAudio;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBotSample.Modules.Audio
{
    public partial class AudioModule : ModuleBase<ScopedSocketCommandContext>
    {
        [Command("play")]
        public async Task PlayAsync([Remainder] string query)
        {
            var fileStrem = File.OpenRead(query);

            var trackInfo = new AudioTrackInfo("Example", null, null, null, null, false);

            var audioTrack = new AudioTrack(fileStrem, trackInfo, AudioPlayer.OPUS_CODEC, true, null);

            var player = _audioService.GetPlayer(Context.Guild) ?? _audioService.CreatePlayer(Context.Guild);

            await player.ConnectAsync(((SocketGuildUser)Context.User).VoiceChannel.Id);

            await player.PlayAsync(audioTrack);
        }
    }
}