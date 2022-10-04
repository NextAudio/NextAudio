// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging.Abstractions;
using NextAudio.Matroska.Internal;
using NextAudio.Matroska.Models;
using Xunit;

namespace NextAudio.Matroska.UnitTests.Internal;

public class BlockReaderTests
{
    public static IEnumerable<object[]> ReadBlockWithDifferentTrackNumberReturnsNullData()
    {
        yield return new object[]
        {
            new MockAudioStream(new byte[] { 130 }, 816),
            new byte[1],
            new MatroskaElement(163, 2, 813, 3, 5008),
            816,
            1,
            817
        };
        yield return new object[]
        {
            new MockAudioStream(new byte[] { 129 }, 5827),
            new byte[1],
            new MatroskaElement(163, 2, 5824, 3, 4464),
            5827,
            2,
            5828
        };
    }

    [Theory]
    [MemberData(nameof(ReadBlockWithDifferentTrackNumberReturnsNullData))]
    public void ReadBlockWithDifferentTrackNumberReturnsNull(
        AudioStream stream,
        byte[] buffer,
        MatroskaElement blockElement,
        long position,
        ulong selectedTrackNumber,
        long expectedPosition)
    {
        // Act
        var result = BlockReader.ReadBlock(stream, buffer, blockElement, position, selectedTrackNumber, NullLogger.Instance);

        // Assert
        Assert.False(result.Block.HasValue);
        Assert.Equal(expectedPosition, result.NewPosition);
    }

    [Theory]
    [MemberData(nameof(ReadBlockWithDifferentTrackNumberReturnsNullData))]
    public async Task ReadBlockAsyncWithDifferentTrackNumberReturnsNull(
        AudioStream stream,
        byte[] buffer,
        MatroskaElement blockElement,
        long position,
        ulong selectedTrackNumber,
        long expectedPosition)
    {
        // Act
        var result = await BlockReader.ReadBlockAsync(stream, buffer, blockElement, position, selectedTrackNumber, NullLogger.Instance);

        // Assert
        Assert.False(result.Block.HasValue);
        Assert.Equal(expectedPosition, result.NewPosition);
    }

    public static IEnumerable<object[]> ReadBlockReadsBlockWithNoLacingData()
    {
        yield return new object[]
        {
            new MockAudioStream(new byte[] { 130, 0, 0, 128 }, 11117),
            new byte[4],
            new MatroskaElement(163, 2, 11114, 3, 676),
            11117,
            2,
            11121,
            new MatroskaBlock(
                new MatroskaElement(163, 2, 11114, 3, 676),
                2,
                MatroskaBlockLacingType.No,
                1,
                new int[] { 672 }
            ),
        };
        yield return new object[]
        {
            new MockAudioStream(new byte[] { 130, 0, 0, 128 }, 26519),
            new byte[4],
            new MatroskaElement(163, 2, 26516, 3, 580),
            26519,
            2,
            26523,
            new MatroskaBlock(
                new MatroskaElement(163, 2, 26516, 3, 580),
                2,
                MatroskaBlockLacingType.No,
                1,
                new int[] { 576 }
            ),
        };
    }

    [Theory]
    [MemberData(nameof(ReadBlockReadsBlockWithNoLacingData))]
    public void ReadBlockReadsBlockWithNoLacing(
        AudioStream stream,
        byte[] buffer,
        MatroskaElement blockElement,
        long position,
        ulong selectedTrackNumber,
        long expectedPosition,
        MatroskaBlock expectedBlock)
    {
        // Act
        var result = BlockReader.ReadBlock(stream, buffer, blockElement, position, selectedTrackNumber, NullLogger.Instance);

        // Assert
        Assert.True(result.Block.HasValue);
        Assert.Equal(expectedPosition, result.NewPosition);
        Assert.Equal(expectedBlock, result.Block.Value, BlockEqualityComparer.Instance);
    }

    [Theory]
    [MemberData(nameof(ReadBlockReadsBlockWithNoLacingData))]
    public async Task ReadBlockAsyncReadsBlockWithNoLacing(
        AudioStream stream,
        byte[] buffer,
        MatroskaElement blockElement,
        long position,
        ulong selectedTrackNumber,
        long expectedPosition,
        MatroskaBlock expectedBlock)
    {
        // Act
        var result = await BlockReader.ReadBlockAsync(stream, buffer, blockElement, position, selectedTrackNumber, NullLogger.Instance);

        // Assert
        Assert.True(result.Block.HasValue);
        Assert.Equal(expectedPosition, result.NewPosition);
        Assert.Equal(expectedBlock, result.Block.Value, BlockEqualityComparer.Instance);
    }

    public static IEnumerable<object[]> ReadBlockReadsBlockWithXiphLacingData()
    {
        yield return new object[]
        {
            new MockAudioStream(new byte[] { 138, 0, 0, 130, 7, 145, 145, 143, 150, 140, 150, 156 }, 91706),
            new byte[12],
            new MatroskaElement(163, 2, 91703, 3, 1181),
            91706,
            10,
            91718,
            new MatroskaBlock(
                new MatroskaElement(163, 2, 91703, 3, 1181),
                10,
                MatroskaBlockLacingType.Xiph,
                8,
                new int[] { 145, 145, 143, 150, 140, 150, 156, 140 }
            ),
        };
        yield return new object[]
        {
            new MockAudioStream(new byte[] { 138, 0, 0, 130, 6, 187, 236, 255, 14, 152, 141, 151 }, 4267179),
            new byte[12],
            new MatroskaElement(163, 2, 4267176, 3, 1291),
            4267179,
            10,
            4267191,
            new MatroskaBlock(
                new MatroskaElement(163, 2, 4267176, 3, 1291),
                10,
                MatroskaBlockLacingType.Xiph,
                7,
                new int[] { 187, 236, 269, 152, 141, 151, 143 }
            ),
        };
    }

    [Theory]
    [MemberData(nameof(ReadBlockReadsBlockWithXiphLacingData))]
    public void ReadBlockReadsBlockWithXiphLacing(
        AudioStream stream,
        byte[] buffer,
        MatroskaElement blockElement,
        long position,
        ulong selectedTrackNumber,
        long expectedPosition,
        MatroskaBlock expectedBlock)
    {
        // Act
        var result = BlockReader.ReadBlock(stream, buffer, blockElement, position, selectedTrackNumber, NullLogger.Instance);

        // Assert
        Assert.True(result.Block.HasValue);
        Assert.Equal(expectedPosition, result.NewPosition);
        Assert.Equal(expectedBlock, result.Block.Value, BlockEqualityComparer.Instance);
    }

    [Theory]
    [MemberData(nameof(ReadBlockReadsBlockWithXiphLacingData))]
    public async Task ReadBlockAsyncReadsBlockWithXiphLacing(
        AudioStream stream,
        byte[] buffer,
        MatroskaElement blockElement,
        long position,
        ulong selectedTrackNumber,
        long expectedPosition,
        MatroskaBlock expectedBlock)
    {
        // Act
        var result = await BlockReader.ReadBlockAsync(stream, buffer, blockElement, position, selectedTrackNumber, NullLogger.Instance);

        // Assert
        Assert.True(result.Block.HasValue);
        Assert.Equal(expectedPosition, result.NewPosition);
        Assert.Equal(expectedBlock, result.Block.Value, BlockEqualityComparer.Instance);
    }

    public static IEnumerable<object[]> ReadBlockReadsBlockWithFixedSizeLacingData()
    {
        yield return new object[]
        {
            new MockAudioStream(new byte[] { 130, 0, 0, 132, 3 }, 31581),
            new byte[5],
            new MatroskaElement(163, 2, 31578, 3, 3077),
            31581,
            2,
            31586,
            new MatroskaBlock(
                new MatroskaElement(163, 2, 31578, 3, 3077),
                2,
                MatroskaBlockLacingType.FixedSize,
                4,
                new int[] { 768, 768, 768, 768 }
            ),
        };
        yield return new object[]
        {
            new MockAudioStream(new byte[] { 130, 0, 0, 132, 7 }, 5590759),
            new byte[5],
            new MatroskaElement(163, 2, 5590756, 3, 4613),
            5590759,
            2,
            5590764,
            new MatroskaBlock(
                new MatroskaElement(163, 2, 5590756, 3, 4613),
                2,
                MatroskaBlockLacingType.FixedSize,
                8,
                new int[] { 576, 576, 576, 576, 576, 576, 576, 576 }
            ),
        };
    }

    [Theory]
    [MemberData(nameof(ReadBlockReadsBlockWithFixedSizeLacingData))]
    public void ReadBlockReadsBlockWithFixedSizeLacing(
        AudioStream stream,
        byte[] buffer,
        MatroskaElement blockElement,
        long position,
        ulong selectedTrackNumber,
        long expectedPosition,
        MatroskaBlock expectedBlock)
    {
        // Act
        var result = BlockReader.ReadBlock(stream, buffer, blockElement, position, selectedTrackNumber, NullLogger.Instance);

        // Assert
        Assert.True(result.Block.HasValue);
        Assert.Equal(expectedPosition, result.NewPosition);
        Assert.Equal(expectedBlock, result.Block.Value, BlockEqualityComparer.Instance);
    }

    [Theory]
    [MemberData(nameof(ReadBlockReadsBlockWithFixedSizeLacingData))]
    public async Task ReadBlockAsyncReadsBlockWithFixedSizeLacing(
        AudioStream stream,
        byte[] buffer,
        MatroskaElement blockElement,
        long position,
        ulong selectedTrackNumber,
        long expectedPosition,
        MatroskaBlock expectedBlock)
    {
        // Act
        var result = await BlockReader.ReadBlockAsync(stream, buffer, blockElement, position, selectedTrackNumber, NullLogger.Instance);

        // Assert
        Assert.True(result.Block.HasValue);
        Assert.Equal(expectedPosition, result.NewPosition);
        Assert.Equal(expectedBlock, result.Block.Value, BlockEqualityComparer.Instance);
    }

    public static IEnumerable<object[]> ReadBlockReadsBlockWithEbmlLacingData()
    {
        yield return new object[]
        {
            new MockAudioStream(new byte[] { 130, 0, 0, 134, 7, 65, 224, 96, 95, 96, 95, 191, 191, 191, 95, 159 }, 816),
            new byte[16],
            new MatroskaElement(163, 2, 813, 3, 5008),
            816,
            2,
            832,
            new MatroskaBlock(
                new MatroskaElement(163, 2, 813, 3, 5008),
                2,
                MatroskaBlockLacingType.Ebml,
                8,
                new int[] { 480, 576, 672, 672, 672, 672, 576, 672 }
            ),
        };
        yield return new object[]
        {
            new MockAudioStream(new byte[] { 130, 0, 0, 134, 3, 66, 160, 191, 96, 95 }, 14264),
            new byte[10],
            new MatroskaElement(163, 2, 14261, 3, 2794),
            14264,
            2,
            14274,
            new MatroskaBlock(
                new MatroskaElement(163, 2, 14261, 3, 2794),
                2,
                MatroskaBlockLacingType.Ebml,
                4,
                new int[] { 672, 672, 768, 672 }
            ),
        };
    }

    [Theory]
    [MemberData(nameof(ReadBlockReadsBlockWithEbmlLacingData))]
    public void ReadBlockReadsBlockWithEbmlLacing(
        AudioStream stream,
        byte[] buffer,
        MatroskaElement blockElement,
        long position,
        ulong selectedTrackNumber,
        long expectedPosition,
        MatroskaBlock expectedBlock)
    {
        // Act
        var result = BlockReader.ReadBlock(stream, buffer, blockElement, position, selectedTrackNumber, NullLogger.Instance);

        // Assert
        Assert.True(result.Block.HasValue);
        Assert.Equal(expectedPosition, result.NewPosition);
        Assert.Equal(expectedBlock, result.Block.Value, BlockEqualityComparer.Instance);
    }

    [Theory]
    [MemberData(nameof(ReadBlockReadsBlockWithEbmlLacingData))]
    public async Task ReadBlockAsyncReadsBlockWithEbmlLacing(
        AudioStream stream,
        byte[] buffer,
        MatroskaElement blockElement,
        long position,
        ulong selectedTrackNumber,
        long expectedPosition,
        MatroskaBlock expectedBlock)
    {
        // Act
        var result = await BlockReader.ReadBlockAsync(stream, buffer, blockElement, position, selectedTrackNumber, NullLogger.Instance);

        // Assert
        Assert.True(result.Block.HasValue);
        Assert.Equal(expectedPosition, result.NewPosition);
        Assert.Equal(expectedBlock, result.Block.Value, BlockEqualityComparer.Instance);
    }

    private class MockAudioStream : ReadOnlyAudioStream
    {
        private readonly byte[] _buffer;

        private int _currentIndex;

        public MockAudioStream(byte[] buffer, long position)
        {
            _buffer = buffer;
            Position = position;
        }

        public override long Position { get; set; }

        public override bool CanSeek => false;

        public override long Length => long.MaxValue;

        public override RecommendedSynchronicity RecommendedSynchronicity => throw new NotImplementedException();

        public override AudioStream Clone()
        {
            throw new NotImplementedException();
        }

        public override int Read(Span<byte> buffer)
        {
            var limit = Math.Min(buffer.Length, _buffer.Length);

            var source = _buffer.AsSpan(_currentIndex, limit);

            source.CopyTo(buffer);

            _currentIndex += source.Length;
            Position += source.Length;

            return source.Length;
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(Read(buffer.Span));
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
        }

        protected override ValueTask DisposeAsyncCore()
        {
            return ValueTask.CompletedTask;
        }
    }

    public class BlockEqualityComparer : IEqualityComparer<MatroskaBlock>
    {
        public static readonly BlockEqualityComparer Instance = new();

        public bool Equals(MatroskaBlock x, MatroskaBlock y)
        {
            var baseValid =
                x.FrameCount == y.FrameCount &&
                x.LacingType == y.LacingType &&
                x.TrackNumber == y.TrackNumber;

            if (!baseValid)
            {
                return false;
            }

            for (var i = 0; i < x.FrameCount; i++)
            {
                if (x.GetFrameSizeByIndex(i) != y.GetFrameSizeByIndex(i))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode([DisallowNull] MatroskaBlock obj)
        {
            return obj.GetHashCode();
        }
    }
}
