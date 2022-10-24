// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using NextAudio.Semaphore;
using Xunit;

namespace NextAudio.UnitTests.Semaphore;

public class SemaphoreSlimExtensionsTests
{
    [Fact]
    public void EnterWithMilissecondsTimeoutEqualsTo0ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var semaphore = new SemaphoreSlim(1, 1);

        // Act + Assert
        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = SemaphoreSlimExtensions.Enter(semaphore, 0);
        });
    }

    [Fact]
    public void EnterWithTimeoutEqualsTo0ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var semaphore = new SemaphoreSlim(1, 1);

        // Act + Assert
        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = SemaphoreSlimExtensions.Enter(semaphore, TimeSpan.FromMilliseconds(0));
        });
    }

    [Fact]
    public async Task EnterAsyncWithMilissecondsTimeoutEqualsTo0ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var semaphore = new SemaphoreSlim(1, 1);

        // Act + Assert
        _ = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            _ = await SemaphoreSlimExtensions.EnterAsync(semaphore, 0);
        });
    }

    [Fact]
    public async Task EnterAsyncWithTimeoutEqualsTo0ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var semaphore = new SemaphoreSlim(1, 1);

        // Act + Assert
        _ = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            _ = await SemaphoreSlimExtensions.EnterAsync(semaphore, TimeSpan.FromMilliseconds(0));
        });
    }

    [Fact]
    public void EnterShouldEnterIfSemaphoreIsReleasedAndReleaseIfDisposes()
    {
        // Arrange
        var semaphore = new SemaphoreSlim(1, 1);

        // Act
        var disposable = SemaphoreSlimExtensions.Enter(semaphore);

        var preDisposableResult = semaphore.Wait(0);

        disposable.Dispose();

        var postDisposableResult = semaphore.Wait(0);

        Assert.False(preDisposableResult);
        Assert.True(postDisposableResult);
    }

    [Fact]
    public async Task EnterAsyncShouldEnterIfSemaphoreIsReleasedAndReleaseIfDisposes()
    {
        // Arrange
        var semaphore = new SemaphoreSlim(1, 1);

        // Act
        var disposable = await SemaphoreSlimExtensions.EnterAsync(semaphore);

        var preDisposableResult = semaphore.Wait(0);

        disposable.Dispose();

        var postDisposableResult = semaphore.Wait(0);

        Assert.False(preDisposableResult);
        Assert.True(postDisposableResult);
    }
}
