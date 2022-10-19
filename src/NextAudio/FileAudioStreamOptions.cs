// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace NextAudio;

/// <summary>
/// Options when creating an <see cref="AudioStream" /> from a file.
/// </summary>
public sealed class FileAudioStreamOptions
{
    private RecommendedSynchronicity? _recommendedSynchronicity;
    private FileOptions? _fileOptions;

    /// <summary>
    /// Creates a new instance of <see cref="FileAudioStreamOptions" />.
    /// </summary>
    public FileAudioStreamOptions()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="FileAudioStreamOptions" />.
    /// </summary>
    /// <param name="fileStreamOptions">The file stream options.</param>
    public FileAudioStreamOptions(FileStreamOptions fileStreamOptions)
    {
        if (fileStreamOptions == null)
        {
            throw new ArgumentNullException(nameof(fileStreamOptions));
        }

        Mode = fileStreamOptions.Mode;
        Access = fileStreamOptions.Access;
        Share = fileStreamOptions.Share;
        FileOptions = fileStreamOptions.Options;
        PreallocationSize = fileStreamOptions.PreallocationSize;
        BufferSize = fileStreamOptions.BufferSize;
    }

    /// <summary>
    /// One of the enumeration values that determines how to open or create the file.
    /// The default value is <see cref="FileMode.Open" />.
    /// </summary>
    public FileMode Mode { get; set; } = FileMode.Open;

    /// <summary>
    /// A bitwise combination of the enumeration values that determines how the file
    /// can be accessed by the <see cref="FileStream" /> object. This also determines the
    /// values returned by the <see cref="FileStream.CanRead" /> and <see cref="FileStream.CanWrite" />
    /// properties of the <see cref="FileStream" /> object.
    /// The default value is <see cref="FileAccess.Read" />.
    /// </summary>
    public FileAccess Access { get; set; } = FileAccess.Read;

    /// <summary>
    /// A bitwise combination of the enumeration values that determines how the file
    /// will be shared by processes. The default value is <see cref="FileShare.Read" />.
    /// </summary>
    public FileShare Share { get; set; } = FileShare.Read;

    /// <summary>
    /// A bitwise combination of the enumeration values that specifies additional file
    /// options. The default value is <see cref="FileOptions.None" />, which indicates synchronous IO.
    /// </summary>
    public FileOptions FileOptions
    {
        get => _fileOptions ?? (
                    !_recommendedSynchronicity.HasValue
                        ? FileOptions.None
                        : _recommendedSynchronicity.Value == RecommendedSynchronicity.Async
                            ? FileOptions.Asynchronous
                            : FileOptions.None
                );
        set => _fileOptions = value;
    }

    /// <summary>
    /// The initial allocation size in bytes for the file. A positive value is effective
    /// only when a regular file is being created or overwritten (<see cref="FileMode.Create" />
    /// or <see cref="FileMode.CreateNew" />). Negative values are not allowed. In other cases
    /// (including the default 0 value), it's ignored. This value is a hint and is not
    /// a strong guarantee. It is not supported on Web Assembly (WASM) and FreeBSD (the
    /// value is ignored). For Windows, Linux and macOS we will try to preallocate the
    /// disk space to fill the requested allocation size. If that turns out to be impossible,
    /// the operation is going to throw an exception. The final file length (EOF) will
    /// be determined by the number of bytes written to the file.
    /// </summary>
    public long PreallocationSize { get; set; }

    /// <summary>
    /// The size of the buffer used by <see cref="FileStream" /> for buffering. The default
    /// buffer size is 4096. 0 or 1 means that buffering should be disabled. Negative
    /// values are not allowed.
    /// </summary>
    public int BufferSize { get; set; } = 4096;

    /// <summary>
    /// The default options <see cref="FileAudioStreamOptions" /> instance.
    /// </summary>
    public static readonly FileAudioStreamOptions Default = new();

    /// <summary>
    /// If the source stream should be disposed when the audio stream disposes.
    /// The default value is <see langword="true" />.
    /// </summary>
    public bool DisposeSourceStream { get; set; } = true;

    /// <summary>
    /// The recommended synchronicity operation to use when read/write the source <see cref="Stream" />.
    /// </summary>
    public RecommendedSynchronicity RecommendedSynchronicity
    {
        get => _recommendedSynchronicity ??
                    (
                        !_fileOptions.HasValue
                            ? RecommendedSynchronicity.Sync
                            : _fileOptions.Value.HasFlag(FileOptions.Asynchronous)
                                ? RecommendedSynchronicity.Async
                                : RecommendedSynchronicity.Sync
                    );
        set => _recommendedSynchronicity = value;
    }

    /// <summary>
    /// A logger factory to log audio streaming info.
    /// </summary>
    public ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;

    /// <summary>
    /// Creates a clone of the current <see cref="FileAudioStreamOptions" />.
    /// </summary>
    /// <returns>A clone of the current <see cref="FileAudioStreamOptions" />.</returns>
    public FileAudioStreamOptions Clone()
    {
        return new()
        {
            DisposeSourceStream = DisposeSourceStream,
            RecommendedSynchronicity = RecommendedSynchronicity,
            LoggerFactory = LoggerFactory,
            Mode = Mode,
            Access = Access,
            Share = Share,
            FileOptions = FileOptions,
            PreallocationSize = PreallocationSize,
            BufferSize = BufferSize,
        };
    }

    /// <summary>
    /// Cast this instance to a <see cref="StreamToAudioStreamOptions" />.
    /// </summary>
    /// <returns>A casted <see cref="StreamToAudioStreamOptions" />.</returns>
    public StreamToAudioStreamOptions GetStreamToAudioStreamOptions()
    {
        return new()
        {
            DisposeSourceStream = DisposeSourceStream,
            RecommendedSynchronicity = RecommendedSynchronicity,
            LoggerFactory = LoggerFactory,
        };
    }

    /// <summary>
    /// Cast this instance to a <see cref="FileStreamOptions" />.
    /// </summary>
    /// <returns>A casted <see cref="FileStreamOptions" />.</returns>
    public FileStreamOptions GetFileStreamOptions()
    {
        return new()
        {
            Mode = Mode,
            Access = Access,
            Share = Share,
            Options = FileOptions,
            PreallocationSize = PreallocationSize,
            BufferSize = BufferSize,
        };
    }
}
