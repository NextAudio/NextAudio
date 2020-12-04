using NextAudio.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NextAudio
{
    /// <inheritdoc />
    public class DefaultAudioQueue<TTrackInfo> : IAudioQueue<TTrackInfo>
        where TTrackInfo : AudioTrackInfo
    {
        private readonly LinkedList<TTrackInfo> _source;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultAudioQueue{TTrackInfo}" />.
        /// </summary>
        public DefaultAudioQueue()
        {
            _source = new LinkedList<TTrackInfo>();
        }

        /// <inheritdoc />
        public int Count
        {
            get
            {
                lock (_source)
                    return _source.Count;
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            lock (_source)
                _source.Clear();
        }

        /// <inheritdoc />
        public IEnumerator<TTrackInfo> GetEnumerator()
        {
            lock (_source)
                for (var node = _source.First; node.IsNotNull(); node = node.Next)
                    yield return node!.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <inheritdoc />
        public void Remove(TTrackInfo trackInfo)
        {
            trackInfo.NotNull(nameof(trackInfo));

            lock (_source)
                _source.Remove(trackInfo);
        }

        /// <inheritdoc />
        public TTrackInfo RemoveAt(int index)
        {
            lock (_source)
            {
                var currentNode = _source.First;

                for (var i = 0; i <= index && currentNode.IsNotNull(); i++)
                {
                    if (i != index)
                    {
                        currentNode = currentNode!.Next;
                        continue;
                    }

                    _source.Remove(currentNode!);
                    break;
                }

                if (currentNode.IsNull())
                    throw new ArgumentOutOfRangeException(nameof(index));

                return currentNode!.Value;
            }
        }

        /// <inheritdoc />
        public IEnumerable<TTrackInfo> RemoveRange(int startIndex, int endIndex)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (endIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(endIndex));

            if (endIndex > startIndex)
                throw new ArgumentOutOfRangeException(nameof(endIndex));

            if (endIndex - startIndex > Count)
                throw new ArgumentOutOfRangeException(nameof(endIndex));

            var tempIndex = 0;
            var removed = new TTrackInfo[Count - startIndex];

            lock (_source)
            {
                var currentNode = _source.First;
                while (tempIndex != startIndex && currentNode.IsNotNull())
                {
                    tempIndex++;
                    currentNode = currentNode!.Next;
                }

                var nextNode = currentNode?.Next;
                for (var i = 0; i < endIndex && currentNode.IsNotNull(); i++)
                {
                    var tempValue = currentNode!.Value;
                    removed[i] = tempValue;

                    _source.Remove(currentNode);
                    currentNode = nextNode;
                    nextNode = nextNode?.Next;
                }

                return removed;
            }
        }

        /// <inheritdoc />
        public void Shuffle()
        {
            lock (_source)
            {
                if (_source.Count < 2)
                    return;

                var shadow = new TTrackInfo[_source.Count];
                var i = 0;

                for (var node = _source.First; node.IsNotNull(); node = node.Next)
                {
                    var random = new Random(BitConverter.ToInt32(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())));

                    var j = random.Next(i + 1);

                    if (i != j)
                        shadow[i] = shadow[j];

                    shadow[j] = node!.Value;
                    i++;
                }

                _source.Clear();

                foreach (var value in shadow)
                    _source.AddLast(value);
            }
        }

        /// <inheritdoc />
        public bool TryDequeue([MaybeNullWhen(false)] out TTrackInfo? trackInfo)
        {
            lock (_source)
            {
                if (_source.Count < 1)
                {
                    trackInfo = default;
                    return false;
                }

                if (_source.First.IsNull())
                {
                    trackInfo = default;
                    return true;
                }

                var result = _source.First?.Value;
                if (result.IsNull())
                {
                    trackInfo = default;
                    return false;
                }

                _source.RemoveFirst();

                trackInfo = result!;
                return true;
            }
        }

        /// <inheritdoc />
        public bool TryEnqueue(TTrackInfo trackInfo)
        {
            trackInfo.NotNull(nameof(trackInfo));

            lock (_source)
                _source.AddLast(trackInfo);

            return true;
        }

        /// <inheritdoc />
        public bool TryPeek([MaybeNullWhen(false)] out TTrackInfo? trackInfo)
        {
            lock (_source)
            {
                if (_source.First.IsNull())
                {
                    trackInfo = default;
                    return false;
                }

                trackInfo = _source.First!.Value;
                return true;
            }
        }
    }
}