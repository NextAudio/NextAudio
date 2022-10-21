// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System.IO;
using Xunit;

namespace NextAudio.UnitTests;

public class FileAudioStreamOptionsTests
{
    [Fact]
    public void GetStreamToAudioStreamOptionsReturnsAllStreamToAudioStreamOptionsFields()
    {
        // Arrange
        var options = new FileAudioStreamOptions
        {
            DisposeSourceStream = false,
            LoggerFactory = null!,
            RecommendedSynchronicity = RecommendedSynchronicity.Async,
        };

        // Act
        var result = options.GetStreamToAudioStreamOptions();

        // Assert
        Assert.Equal(options.DisposeSourceStream, result.DisposeSourceStream);
        Assert.Equal(options.LoggerFactory, result.LoggerFactory);
        Assert.Equal(options.RecommendedSynchronicity, result.RecommendedSynchronicity);
    }

    [Fact]
    public void GetFileStreamOptionsReturnsAllFileStreamOptions()
    {
        // Arrange
        var options = new FileAudioStreamOptions
        {
            Mode = FileMode.OpenOrCreate,
            Access = FileAccess.Write,
            Share = FileShare.Inheritable,
            FileOptions = FileOptions.Encrypted | FileOptions.RandomAccess,
            BufferSize = 1024,
            PreallocationSize = 1024 * 8,
        };

        // Act
        var result = options.GetFileStreamOptions();

        // Assert
        Assert.Equal(options.Mode, result.Mode);
        Assert.Equal(options.Access, result.Access);
        Assert.Equal(options.Share, result.Share);
        Assert.Equal(options.FileOptions, result.Options);
        Assert.Equal(options.BufferSize, result.BufferSize);
        Assert.Equal(options.PreallocationSize, result.PreallocationSize);
    }

    [Fact]
    public void RecommendedSynchronicityReturnsAsyncIfFileOptionsHasAsyncFlag()
    {
        // Arrange
        var options = new FileAudioStreamOptions
        {
            FileOptions = FileOptions.Asynchronous,
        };

        // Act
        var result = options.RecommendedSynchronicity;

        // Assert
        Assert.Equal(RecommendedSynchronicity.Async, result);
    }

    [Fact]
    public void RecommendedSynchronicityReturnsSyncIfFileOptionsDoesntHasAsyncFlag()
    {
        // Arrange
        var options = new FileAudioStreamOptions
        {
            FileOptions = FileOptions.None,
        };

        // Act
        var result = options.RecommendedSynchronicity;

        // Assert
        Assert.Equal(RecommendedSynchronicity.Sync, result);
    }

    [Fact]
    public void FileOptionsHasAsyncFlagIfRecommendedSynchronicityIsAsync()
    {
        // Arrange
        var options = new FileAudioStreamOptions
        {
            RecommendedSynchronicity = RecommendedSynchronicity.Async,
        };

        // Act
        var result = options.FileOptions;

        // Assert
        Assert.True(result.HasFlag(FileOptions.Asynchronous));
    }

    [Fact]
    public void FileOptionsDoesntHasAsyncFlagIfRecommendedSynchronicityIsNotAsync()
    {
        // Arrange
        var options = new FileAudioStreamOptions
        {
            RecommendedSynchronicity = RecommendedSynchronicity.Sync,
        };

        // Act
        var result = options.FileOptions;

        // Assert
        Assert.False(result.HasFlag(FileOptions.Asynchronous));
    }
}
