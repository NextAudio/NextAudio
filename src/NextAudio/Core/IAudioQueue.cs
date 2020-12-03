using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NextAudio
{
    /// <summary>
    /// Represents a queue for audio tracks.
    /// </summary>
    /// <typeparam name="TTrackInfo"></typeparam>
    public interface IAudioQueue<TTrackInfo> : IEnumerable<TTrackInfo>
        where TTrackInfo : AudioTrackInfo
    {
        /// <summary>
        /// Try adds an item to the end of the queue.
        /// </summary>
        /// <param name="trackInfo">The item to be added.</param>
        /// <returns><c>true</c> if the item was added successfully otherwise <c>false</c>.</returns>
        bool TryEnqueue(TTrackInfo trackInfo);

        /// <summary>
        /// Try dequeue an item from the
        /// </summary>
        /// <param name="trackInfo">The item dequeued.</param>
        /// <returns><c>true</c> if the item was dequeued successfully otherwise <c>false</c>.</returns>
        bool TryDequeue([MaybeNullWhen(false)] out TTrackInfo? trackInfo);

        /// <summary>
        /// The total count of the items in the queue.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Try peek an item from the queue.
        /// </summary>
        /// <remarks>
        /// The item will not be removed from the queue.
        /// </remarks>
        /// <param name="trackInfo">The item peeked.</param>
        /// <returns><c>true</c> if the item was peeked successfully otherwise <c>false</c>.</returns>
        bool TryPeek([MaybeNullWhen(false)] out TTrackInfo? trackInfo);

        /// <summary>
        /// Remove the specified item from the queue.
        /// </summary>
        /// <param name="trackInfo">The item to be removed.</param>
        void Remove(TTrackInfo trackInfo);

        /// <summary>
        /// Clear the queue.
        /// </summary>
        void Clear();

        /// <summary>
        /// Shuffle the queue.
        /// </summary>
        void Shuffle();

        /// <summary>
        /// Remove an item from the queue with the specified index.
        /// </summary>
        /// <param name="index">The index of the item to be removed.</param>
        /// <returns>The item removed.</returns>
        TTrackInfo RemoveAt(int index);

        /// <summary>
        /// Remove all items from the queue with the specified indexes.
        /// </summary>
        /// <param name="startIndex">The start index of the items to be removed.</param>
        /// <param name="endIndex">The end index of the items to be removed.</param>
        /// <returns>The items removed.</returns>
        IEnumerable<TTrackInfo> RemoveRange(int startIndex, int endIndex);
    }
}