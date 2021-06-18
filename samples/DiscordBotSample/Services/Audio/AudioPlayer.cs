using Discord;
using Discord.Audio;
using Discord.WebSocket;
using NextAudio;
using NextAudio.FFMpegCore;
using System;
using System.Threading.Tasks;

namespace DiscordBotSample.Services.Audio
{
    public class AudioPlayer : IAsyncDisposable
    {
        public static readonly AudioCodec OPUS_CODEC = new("Opus", "libopus", "libopus", 48000, 2, 16);

        private readonly FFMpegCorePlayer _underlyingPlayer;
        private IAudioClient _audioClient;

        public AudioPlayer(SocketGuild guild)
        {
            Guild = guild;

            Options = new();

            var options = new FFmpegCorePlayerOptions
            {
                OutputCodec = OPUS_CODEC,
            };

            _underlyingPlayer = new(options);
        }

        public bool IsPlaying { get; private set; }

        public DefaultAudioQueue<AudioTrackInfo> Queue { get; } = new();

        public AudioTrackInfo CurrentTrack { get; }

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

        public Task PlayAsync(AudioTrack audioTrack)
            => _underlyingPlayer.PlayAsync(audioTrack);

        public async Task StopAsync()
        {
            await _underlyingPlayer.StopAsync();

            await DisconnectAsync();
        }

        public ValueTask PauseAsync()
            => _underlyingPlayer.PauseAsync();

        public ValueTask ResumeAsync()
            => _underlyingPlayer.ResumeAsync();

        public ValueTask SetVolumeAsync(int volume)
            => _underlyingPlayer.SetVolumeAsync(volume);

        public Task DisconnectAsync()
            => DisposeAsync().AsTask();

        public async ValueTask DisposeAsync()
        {
            if (VoiceChannel != null)
            {
                await VoiceChannel.DisconnectAsync();
            }

            if (_audioClient == null)
                return;

            _audioClient.Dispose();
            _audioClient = null;

            await _underlyingPlayer.DisposeAsync();
        }
    }
}