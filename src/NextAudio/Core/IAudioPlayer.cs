using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace NextAudio
{
    /// <summary>
    /// Represents an audio player that can play tracks.
    /// </summary>
    public interface IAudioPlayer : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// The position of the playing track.
        /// </summary>
        TimeSpan Position { get; }

        /// <summary>
        /// The current playing track if has any.
        /// </summary>
        AudioTrackInfo? CurrentTrack { get; }

        /// <summary>
        /// The pipe reader for read data from the current playing audio track.
        /// </summary>
        PipeReader TrackReader { get; }

        /// <summary>
        /// If has any playing track and if is not paused.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// If the playback is paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// If the playback supports seeking.
        /// </summary>
        bool SeekSupported { get; }

        /// <summary>
        /// If the playback supports custom volume levels.
        /// </summary>
        bool VolumeSupported { get; }

        /// <summary>
        /// Play the requested audio track.
        /// </summary>
        /// <param name="audioTrack">The requested audio track to be played.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        Task PlayAsync(AudioTrack audioTrack, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop the playback and destroy the player.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> that represents an asynchronous operation.</returns>
        ValueTask StopAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Pause the playback.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> that represents an asynchronous operation.</returns>
        ValueTask PauseAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Resume the playback.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask" /> that represents an asynchronous operation.</returns>
        ValueTask ResumeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Seeks the current audio track to the specified position.
        /// </summary>
        /// <param name="time">The position to be used for seeking.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> that represents an asynchronous operation.</returns>
        ValueTask SeekAsync(TimeSpan time, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the current audio track volume.
        /// </summary>
        /// <param name="volume">The volume to set.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask"/> that represents an asynchronous operation.</returns>
        ValueTask SetVolumeAsync(int volume, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current audio track volume.
        /// </summary>
        /// <returns>The current audio track volume.</returns>
        int GetVolume();

        /// <summary>
        /// Gets the playback buffer size.
        /// </summary>
        /// <returns>The playback buffer size.</returns>
        int GetBufferSize();

        /// <summary>
        /// Sets the playback buffer size.
        /// </summary>
        /// <param name="bufferSize">The playback buffer size to set.</param>
        void SetBufferSize(int bufferSize);
    }
}