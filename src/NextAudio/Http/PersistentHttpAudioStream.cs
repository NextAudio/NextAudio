// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System.Buffers;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using NextAudio.Semaphore;
using Polly;
using Polly.Retry;

namespace NextAudio.Http;

/// <summary>
/// Represents a persistent http audio stream.
/// </summary>
public class PersistentHttpAudioStream : ReadOnlyAudioStream
{
    private static readonly SocketError[] _errors = new SocketError[]
    {
        SocketError.ConnectionReset,
        SocketError.TimedOut,
        SocketError.NetworkReset,
    };

    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly PersistentHttpAudioStreamOptions _options;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _policy;

    private Stream? _sourceStream;
    private HttpResponseMessage? _response;
    private long _position;
    private long _length;

    /// <summary>
    /// Creates a new instance of <see cref="PersistentHttpAudioStream" />.
    /// </summary>
    /// <param name="options">The options for this stream.</param>
    public PersistentHttpAudioStream(PersistentHttpAudioStreamOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options;
        _length = options.Length;

        _policy = Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .OrResult((response) =>
                    {
                        return (int)response.StatusCode >= 500 || response.StatusCode == HttpStatusCode.RequestTimeout;
                    })
                    .RetryAsync(options.MaxRetryCount);
    }

    /// <inheritdoc />
    public override bool CanSeek => _sourceStream?.CanSeek == true || _length > 0;

    /// <inheritdoc />
    public override long Length => _length;

    /// <inheritdoc />
    public override long Position
    {
        get => _position;
        set => Seek(value, SeekOrigin.Begin);
    }

    /// <inheritdoc />
    public override RecommendedSynchronicity RecommendedSynchronicity => RecommendedSynchronicity.Async;

    /// <inheritdoc />
    public override int Read(Span<byte> buffer)
    {
        var bufferArray = ArrayPool<byte>.Shared.Rent(buffer.Length);

        try
        {
            var memory = bufferArray.AsMemory(0, buffer.Length);

            var read = ReadAsync(memory).AsTask().GetAwaiter().GetResult();

            memory.Span.CopyTo(buffer);

            return read;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bufferArray);
        }
    }

    /// <inheritdoc />
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return InternalReadAsync(buffer, true, cancellationToken);
    }

    private async ValueTask<int> InternalReadAsync(Memory<byte> buffer, bool attemptReconnect, CancellationToken cancellationToken)
    {
        using var semaphoreDisposable = await _semaphore.EnterAsync(cancellationToken).ConfigureAwait(false);


        await CheckConnectedAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var bytesRead = await _sourceStream!.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

            _position += bytesRead;

            return bytesRead;
        }
        catch (Exception ex)
        {
            if (!attemptReconnect)
            {
                throw;
            }

            SocketException? socketEx = null;

            if (ex is SocketException mainEx)
            {
                socketEx = mainEx;
            }
            else if (ex.InnerException is SocketException innerEx)
            {
                socketEx = innerEx;
            }

            if (socketEx == null)
            {
                throw;
            }

            if (!_errors.Contains(socketEx.SocketErrorCode))
            {
                throw;
            }

            await ClearAsync().ConfigureAwait(false);

            semaphoreDisposable.Dispose();

            return await InternalReadAsync(buffer, false, cancellationToken).ConfigureAwait(false);
        }
    }

    private async ValueTask CheckConnectedAsync(CancellationToken cancellationToken)
    {
        if (_sourceStream != null)
        {
            return;
        }

        var response = await MakeRequestAsync(cancellationToken).ConfigureAwait(false);

        _response = response.EnsureSuccessStatusCode();

        if (_length == 0 && _response.Content.Headers.ContentLength.HasValue)
        {
            _length = _response.Content.Headers.ContentLength.Value;
        }

        var stream = await _response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        _sourceStream = _options.BufferSize is 0 or 1 ? stream : new BufferedStream(stream, _options.BufferSize);
    }

    private Task<HttpResponseMessage> MakeRequestAsync(CancellationToken cancellationToken)
    {
        return _policy.ExecuteAsync(() =>
        {
            using var request = CreateRangeRequest();

            return _options.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        });
    }

    /// <summary>
    /// Creates a range request to retrive the audio stream.
    /// </summary>
    /// <returns>A range request to retrive the audio stream</returns>
    protected virtual HttpRequestMessage CreateRangeRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _options.RequestUri);

        if (_position > 0)
        {
            request.Headers.Range = new RangeHeaderValue(_position, null)
            {
                Unit = "bytes",
            };
        }

        return request;
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        return SeekAsync(offset, origin).AsTask().GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public override async ValueTask<long> SeekAsync(long offset, SeekOrigin origin, CancellationToken cancellationToken = default)
    {
        using var _ = await _semaphore.EnterAsync(cancellationToken).ConfigureAwait(false);

        // Probably never
        if (_sourceStream?.CanSeek == true)
        {
            _position = _sourceStream.Seek(offset, origin);
            return _position;
        }

        if (_response != null && !CanSeek)
        {
            throw new NotSupportedException("Seek is not supported for infinite streams.");
        }

        var pos = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.End => _length + offset,
            SeekOrigin.Current => _position + offset,
            _ => throw new InvalidOperationException($"Unknown SeekOrigin enum value: '{origin}'.")
        };

        if (pos > int.MaxValue)
        {
            throw new InvalidOperationException("Cannot seek more than the Int32 limit.");
        }

        if (pos == _position)
        {
            return _position;
        }

        if (_length > 0 && pos > _length)
        {
            pos = _length;
        }

        if (pos < 0)
        {
            pos = 0;
        }

        await ClearAsync().ConfigureAwait(false);

        _position = pos;

        return pos;
    }

    /// <inheritdoc />
    public override PersistentHttpAudioStream Clone()
    {
        return new PersistentHttpAudioStream(_options.Clone());
    }

    private async ValueTask ClearAsync()
    {
        if (_sourceStream != null)
        {
            await _sourceStream.DisposeAsync().ConfigureAwait(false);
            _sourceStream = null;
        }

        if (_response != null)
        {
            _response.Dispose();
            _response = null;
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            _sourceStream?.Dispose();
            _response?.Dispose();
            _semaphore.Dispose();

            if (_options.DisposeHttpClient)
            {
                _options.HttpClient.Dispose();
            }
        }
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        if (IsDisposed)
        {
            return;
        }

        if (_sourceStream != null)
        {
            await _sourceStream.DisposeAsync().ConfigureAwait(false);
        }

        _response?.Dispose();

        _semaphore.Dispose();

        if (_options.DisposeHttpClient)
        {
            _options.HttpClient.Dispose();
        }
    }
}
