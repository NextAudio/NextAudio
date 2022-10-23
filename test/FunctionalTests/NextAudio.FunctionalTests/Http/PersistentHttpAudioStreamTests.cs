// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using NextAudio.Http;
using RichardSzalay.MockHttp;
using Xunit;

namespace NextAudio.FunctionalTests.Http;

public class PersistentHttpAudioStreamTests
{
    [Fact]
    public async Task CanReconnectIfConnectionDrops()
    {
        // Arrange
        using var file = File.Open("assets/test1.mkv", FileMode.Open, FileAccess.Read);
        using var stream = new InterruptibleStream(file, 5000);

        using var mockHttp = new MockHttpMessageHandler();

        var requestUri = "http://test.com/test1.mkv";

        _ = mockHttp.When(HttpMethod.Head, requestUri).Respond(req => stream.GetResponseMessage(req));
        _ = mockHttp.When(HttpMethod.Get, requestUri).Respond(req => stream.GetResponseMessage(req));

        using var client = mockHttp.ToHttpClient();

        var options = new PersistentHttpAudioStreamOptions(new(requestUri), null, client);

        using var persistentHttpAudioStream = new PersistentHttpAudioStream(options);

        // Act
        var buffer = new byte[1024];
        var bytesRead = 0;

        while ((bytesRead = await persistentHttpAudioStream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
        {
        }

        // Assert
        Assert.Equal(persistentHttpAudioStream.Position, file.Length);
    }

    [Fact]
    public async Task CantContinueIfInterruptibleFail2TimesInARow()
    {
        // Arrange
        using var file = File.Open("assets/test1.mkv", FileMode.Open, FileAccess.Read);
        using var stream = new InterruptibleStream(file, 5000, 2);

        using var mockHttp = new MockHttpMessageHandler();

        var requestUri = "http://test.com/test1.mkv";

        _ = mockHttp.When(HttpMethod.Head, requestUri).Respond(req => stream.GetResponseMessage(req));
        _ = mockHttp.When(HttpMethod.Get, requestUri).Respond(req => stream.GetResponseMessage(req));

        using var client = mockHttp.ToHttpClient();

        var options = new PersistentHttpAudioStreamOptions(new(requestUri), null, client);

        using var persistentHttpAudioStream = new PersistentHttpAudioStream(options);

        // Act + Assert
        await Assert.ThrowsAsync<SocketException>(async () =>
        {
            var buffer = new byte[1024];
            var bytesRead = 0;

            while ((bytesRead = await persistentHttpAudioStream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
            {
            }
        }).ConfigureAwait(false);
    }

    [Fact]
    public async Task CanContinueIfRequestFailsLessThanMaxRetryCount()
    {
        // Arrange
        using var file = File.Open("assets/test1.mkv", FileMode.Open, FileAccess.Read);
        using var stream = new InterruptibleStream(file, 5000, 1, 2);

        using var mockHttp = new MockHttpMessageHandler();

        var requestUri = "http://test.com/test1.mkv";

        _ = mockHttp.When(HttpMethod.Head, requestUri).Respond(req => stream.GetResponseMessage(req));
        _ = mockHttp.When(HttpMethod.Get, requestUri).Respond(req => stream.GetResponseMessage(req));

        using var client = mockHttp.ToHttpClient();

        var options = new PersistentHttpAudioStreamOptions(new(requestUri), null, client)
        {
            MaxRetryCount = 2,
        };

        using var persistentHttpAudioStream = new PersistentHttpAudioStream(options);

        // Act
        var buffer = new byte[1024];
        var bytesRead = 0;

        while ((bytesRead = await persistentHttpAudioStream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
        {
        }

        // Assert
        Assert.Equal(persistentHttpAudioStream.Position, file.Length);
    }


    [Fact]
    public async Task CantContinueIfRequestFailsMoreThanMaxRetryCount()
    {
        // Arrange
        using var file = File.Open("assets/test1.mkv", FileMode.Open, FileAccess.Read);
        using var stream = new InterruptibleStream(file, 5000, 1, 3);

        using var mockHttp = new MockHttpMessageHandler();

        var requestUri = "http://test.com/test1.mkv";

        _ = mockHttp.When(HttpMethod.Head, requestUri).Respond(req => stream.GetResponseMessage(req));
        _ = mockHttp.When(HttpMethod.Get, requestUri).Respond(req => stream.GetResponseMessage(req));

        using var client = mockHttp.ToHttpClient();

        var options = new PersistentHttpAudioStreamOptions(new(requestUri), null, client)
        {
            MaxRetryCount = 2,
        };

        using var persistentHttpAudioStream = new PersistentHttpAudioStream(options);

        // Act + Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            var buffer = new byte[1024];
            var bytesRead = 0;

            while ((bytesRead = await persistentHttpAudioStream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
            {
            }
        }).ConfigureAwait(false);
    }

    [Fact]
    public async Task CannotSeekIfResponseDoesntHasLength()
    {
        // Arrange
        using var file = File.Open("assets/test1.mkv", FileMode.Open, FileAccess.Read);
        using var stream = new InterruptibleStream(file, 5000);

        using var mockHttp = new MockHttpMessageHandler();

        var requestUri = "http://test.com/test1.mkv";

        _ = mockHttp.When(HttpMethod.Head, requestUri).Respond(req => stream.GetResponseMessage(req));
        _ = mockHttp.When(HttpMethod.Get, requestUri).Respond(req => stream.GetResponseMessage(req));

        using var client = mockHttp.ToHttpClient();

        var options = new PersistentHttpAudioStreamOptions(new(requestUri), null, client)
        {
            MaxRetryCount = 2,
        };

        using var persistentHttpAudioStream = new PersistentHttpAudioStream(options);

        // Initialize the stream.
        _ = await persistentHttpAudioStream.ReadAsync(new byte[1024]).ConfigureAwait(false);

        // Act + Assert
        await Assert.ThrowsAsync<NotSupportedException>(async () =>
        {
            await persistentHttpAudioStream.SeekAsync(1000, SeekOrigin.Current).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    [Fact]
    public async Task CanSeekIfResponseHasLength()
    {
        // Arrange
        using var file = File.Open("assets/test1.mkv", FileMode.Open, FileAccess.Read);
        using var stream = new InterruptibleStream(file, 5000, 1, 0, true);

        using var mockHttp = new MockHttpMessageHandler();

        var requestUri = "http://test.com/test1.mkv";

        _ = mockHttp.When(HttpMethod.Head, requestUri).Respond(req => stream.GetResponseMessage(req));
        _ = mockHttp.When(HttpMethod.Get, requestUri).Respond(req => stream.GetResponseMessage(req));

        using var client = mockHttp.ToHttpClient();

        var options = new PersistentHttpAudioStreamOptions(new(requestUri), null, client)
        {
            MaxRetryCount = 2,
        };

        using var persistentHttpAudioStream = new PersistentHttpAudioStream(options);

        // Initialize the stream.
        _ = await persistentHttpAudioStream.ReadAsync(new byte[1024]).ConfigureAwait(false);

        // Act
        var result = await persistentHttpAudioStream.SeekAsync(1000, SeekOrigin.Current).ConfigureAwait(false);

        // Assert
        Assert.Equal(1024 + 1000, result);
    }

    [Fact]
    public async Task CanSeekIfOptionsHasLength()
    {
        // Arrange
        using var file = File.Open("assets/test1.mkv", FileMode.Open, FileAccess.Read);
        using var stream = new InterruptibleStream(file, 5000);

        using var mockHttp = new MockHttpMessageHandler();

        var requestUri = "http://test.com/test1.mkv";

        _ = mockHttp.When(HttpMethod.Head, requestUri).Respond(req => stream.GetResponseMessage(req));
        _ = mockHttp.When(HttpMethod.Get, requestUri).Respond(req => stream.GetResponseMessage(req));

        using var client = mockHttp.ToHttpClient();

        var options = new PersistentHttpAudioStreamOptions(new(requestUri), file.Length, client)
        {
            MaxRetryCount = 2,
        };

        using var persistentHttpAudioStream = new PersistentHttpAudioStream(options);

        // Initialize the stream.
        _ = await persistentHttpAudioStream.ReadAsync(new byte[1024]).ConfigureAwait(false);

        // Act
        var result = await persistentHttpAudioStream.SeekAsync(1000, SeekOrigin.Current).ConfigureAwait(false);

        // Assert
        Assert.Equal(1024 + 1000, result);
    }

    private sealed class InterruptibleStream : Stream
    {
        private readonly Stream _sourceStream;
        private readonly long _interruptibleMark;
        private readonly int _maxInterruptionCount;
        private readonly int _maxRequestErrorCount;
        private readonly bool _enableSeek;

        public InterruptibleStream(Stream sourceStream, long interruptibleMark, int maxInterruptionCount = 1, int maxRequestErrorCount = 0, bool enableSeek = false)
        {
            _sourceStream = sourceStream;
            _interruptibleMark = interruptibleMark;
            _maxInterruptionCount = maxInterruptionCount;
            _maxRequestErrorCount = maxRequestErrorCount;
            _enableSeek = enableSeek;
        }

        private int Interruptions { get; set; }

        private int RequestErrors { get; set; }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new System.NotImplementedException();

        public override long Position { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override void Flush()
        {
            throw new System.NotImplementedException();
        }

        public Task<HttpResponseMessage> GetResponseMessage(HttpRequestMessage requestMessage)
        {
            var response = new HttpResponseMessage();

            if (RequestErrors < _maxRequestErrorCount)
            {
                RequestErrors++;

                response.StatusCode = HttpStatusCode.InternalServerError;

                return Task.FromResult(response);
            }

            if (requestMessage.Headers.Range != null)
            {
                response.StatusCode = HttpStatusCode.PartialContent;
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
            }

            response.Content = new StreamContent(this);

            if (_enableSeek)
            {
                response.Content.Headers.ContentLength = _sourceStream.Length;
            }

            return Task.FromResult(response);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (Interruptions < _maxInterruptionCount && _sourceStream.Position >= _interruptibleMark)
            {
                Interruptions++;
                throw new SocketException((int)SocketError.NetworkReset);
            }

            return _sourceStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }
    }
}
