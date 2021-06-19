using Discord;
using Discord.Audio;
using Discord.WebSocket;
using NextAudio;
using NextAudio.FFMpegCore;
using System;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace DiscordBotSample.Services.Audio
{
    public class AudioPlayer : IAsyncDisposable
    {
        public static readonly AudioCodec OPUS_CODEC = new("Opus", "libopus", "libopus", 48000, 2, 16);

        private readonly FFMpegCorePlayer _underlyingPlayer;
        private IAudioClient _audioClient;
        private bool _transmitTaskStarted;

        public AudioPlayer(SocketGuild guild)
        {
            Guild = guild;

            Options = new();

            var options = new FFmpegCorePlayerOptions();

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

        public async Task PlayAsync(AudioTrack audioTrack)
        {
            await _underlyingPlayer.PlayAsync(audioTrack);

            if (!_transmitTaskStarted)
                _ = Task.Run(TransmitTask);
        }

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

        private async Task TransmitTask()
        {
            if (_transmitTaskStarted)
                return;

            _transmitTaskStarted = true;

            await Task.Delay(10);

            using var outStream = _audioClient.CreatePCMStream(AudioApplication.Music);

            ReadResult result = default;

            while (!result.IsCompleted && !result.IsCanceled)
            {
                result = await _underlyingPlayer.TrackReader.ReadAsync();

                var position = result.Buffer.Start;
                var consumed = position;

                try
                {
                    while (result.Buffer.TryGet(ref position, out var memory))
                    {
                        await outStream.WriteAsync(memory);

                        consumed = position;
                    }

                    consumed = result.Buffer.End;
                }
                finally
                {
                    _underlyingPlayer.TrackReader.AdvanceTo(consumed);
                }
            }

            await _underlyingPlayer.TrackReader.CompleteAsync();

            _transmitTaskStarted = false;
        }

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