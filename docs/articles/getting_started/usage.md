## Usage

#### AudioStream
NextAudio uses `AudioStream` instead raw `Stream` class because `Stream` has a lot of unnecessary methods to coverage.

The `AudioStream` also have an additional `SeekAsync` method for streams which support asynchronous seeking.

#### Stream

The `AudioStream` class has all implict casts if you need an `AudioStream` from a `Stream`, or you can do this manually with:

```csharp
AudioStream.CreateFromStream(stream);
```

You can also pass some options for this `AudioStream` with the `StreamToAudioStreamOptions` class:
```csharp
AudioStream.CreateFromStream(stream, new StreamToAudioStreamOptions
{
  DisposeSourceStream = true,
  RecommendedSynchronicity = RecommendedSynchronicity.Any,
  LoggerFactory = NullLoggerFactory.Instance,
});
```

The default options will be used if you use any implicit cast.

The inverse is true too, and you can use any implicit cast if you wanna a `Stream` from an `AudioStream` or you can do this manually with:
```csharp
audioStream.CastToStream();

// Or statically:
AudioStream.CastToStream(audioStream);
```

You can also pass some options for this `Stream` with the `AudioStreamToStreamOptions` class:
```csharp
audioStream.CastToStream(new AudioStreamToStreamOptions
{
  DisposeSourceStream = true,
});
```

The default options will be used if you use any implicit cast.

#### File
You can also create an `AudioStream` from a file path with:

```csharp
AudioStream.CreateFromFile("filePath.mkv");
```

File audio streams supports the `FileAudioStreamOptions` options or the native class `FileStreamOptions`, we recommend the `FileAudioStreamOptions` because this class already cover all options present in `FileStreamOptions`.

```csharp
AudioStream.CreateFromFile("filePath.mkv", new FileAudioStreamOptions
{
  Mode = FileMode.Open,
  Access = FileAccess.Read,
  Share = FileShare.Read,
  FileOptions = FileOptions.None,
  PreallocationSize = 0,
  BufferSize = 4096,
  DisposeSourceStream = true,
  RecommendedSynchronicity = RecommendedSynchronicity.Sync,
  LoggerFactory = NullLoggerFactory.Instance,
});
```

The default options will be used if you use the create method without any options set.

#### Http
You can also create an `AudioStream` from a url with:

```csharp
AudioStream.CreateFromUrl("https://url.com");
```

Http audio streams is persistent (handles eventual disconnects), you can change some of these behaviors with the `PersistentHttpAudioStreamOptions` class:

```csharp
AudioStream.CreateFromUrl(new PersistentHttpAudioStreamOptions("https://url.com")
{
  MaxRetryCount = 2,
  Length = 0, // Will be extracted from response headers.
  DisposeHttpClient = true,
  HttpClient = new HttpClient(),
  BufferSize = 4096,
  LoggerFactory = NullLoggerFactory.Instance,
});
```

We recommend pass the following params in the constructor to avoid extra allocations: `Length`, `HttpClient` and `LoggerFactory`:
```csharp
AudioStream.CreateFromUrl(new PersistentHttpAudioStreamOptions("https://url.com", null, new HttpClient(), NullLoggerFactory.Instance)
{
  MaxRetryCount = 2,
  DisposeHttpClient = true,
  BufferSize = 4096,
});
```

The default options will be used if you use the create method without any options set.

#### AudioDemuxers
Any container demuxer inherits from the ``AudioDemuxer`` class, this class has `Demux` or `DemuxAsync` methods instead `Read` and `ReadAsync` (yes, you can continue use these methods normally).

Any `Seek/SeekAsync` operation for streams positions is not supported.

#### RecommendedSynchronicity
The property `RecommendedSynchronicity` is present in any `AudioStream` it says to you or for the internal lib what is the recommended synchronicity when reading from the `AudioStream` or writing in.

Obs: The lib supports both synchronicitys but some streams don't recommend some synchronicitys, (e.g an async-only stream will have your sync operations using `.GetAwaiter().GetResult()`).

#### Manually reading/writing

NextAudio will always use audio frame buffers for read/write operations.

This makes the streaming data very easy.
```csharp
using var output = new MemoryStream();
using var file = AudioStream.CreateFromFile("test1.mkv");
using var reader = new SomeAudioReadStreamType(file);

var bytesRead = 0;
Span<byte> buffer = new byte[1024]; // We recommend a buffer size at least 1024 bytes.

while ((bytesRead = reader.Read(buffer)) > 0)
{
    // Always slice the buffer to bytesRead,
    // the lib always will return an audio frame
    // when using a read operation.
    var frame = buffer.Slice(0, bytesRead);
    output.Write(frame);
}
```
