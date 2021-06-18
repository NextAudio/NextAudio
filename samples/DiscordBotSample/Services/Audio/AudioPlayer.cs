using Discord;
using Discord.Audio;
using Discord.WebSocket;
using NextAudio;
using System.Threading.Tasks;

namespace DiscordBotSample.Services.Audio
{
    public class AudioPlayer
    {
        private IAudioClient _audioClient;

        public AudioPlayer(SocketGuild guild)
        {
            Guild = guild;

            Options = new();
        }

        public bool IsPlaying { get; private set; }

        public DefaultAudioQueue<AudioTrackInfo> Queue { get; } = new();

        public PlayerOptions Options { get; }

        public SocketGuild Guild { get; }

        public SocketVoiceChannel VoiceChannel => Guild.CurrentUser.VoiceChannel;

        public async Task ConnectAsync(ulong voiceChannelId)
        {
            if (VoiceChannel != null)
                return;

            var voiceChannel = Guild.GetVoiceChannel(voiceChannelId);
            
            _audioClient = await voiceChannel.ConnectAsync(true, false, false);
        }

        public async Task PlayAsync(AudioTrack audioTrack)
        {
            
        }

        public async Task StopAsync()
        {

        }

        public async Task PauseAsync()
        {
            
        }

        public async Task SetVolumeAsync()
        {

        }

        public async Task DisconnectAsync()
        {
            if (VoiceChannel != null)
            {
                await VoiceChannel.DisconnectAsync();
            }

            if (_audioClient == null)
                return;

            _audioClient.Dispose();
            _audioClient = null;
        }
    }
}