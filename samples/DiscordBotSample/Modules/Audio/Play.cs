using Discord.Commands;
using Discord.WebSocket;
using DiscordBotSample.Contexts;
using DiscordBotSample.Services.Audio;
using NextAudio;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBotSample.Modules.Audio
{
    public partial class AudioModule
    {
        [Command("play")]
        public async Task PlayAsync([Remainder] string query)
        {
            var fileStrem = File.OpenRead(query);

            var trackInfo = new AudioTrackInfo("Example", null, null, null, null, false);

            var audioTrack = new AudioTrack(fileStrem, trackInfo, AudioPlayer.OPUS_CODEC, true, null);

            await Player.ConnectAsync(((SocketGuildUser)Context.User).VoiceChannel.Id);

            await Player.PlayAsync(audioTrack);
        }
    }
}