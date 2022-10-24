// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System.ComponentModel;

namespace NextAudio.Semaphore;

/// <summary>
/// Some extensions methods for <see cref="SemaphoreSlim" />.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class SemaphoreSlimExtensions
{
    /// <summary>
    /// Waits to enter in this <paramref name="semaphore" />,
    /// the returned disposable will release the <paramref name="semaphore" /> if disposed.
    /// </summary>
    /// <param name="semaphore">The <see cref="SemaphoreSlim" /> to enter.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A disposable which release <paramref name="semaphore" /> when disposed.</returns>
    public static IDisposable Enter(this SemaphoreSlim semaphore, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(semaphore);

        semaphore.Wait(cancellationToken);

        return new SemaphoreLockDisposable(semaphore);
    }

    /// <summary>
    /// Waits to enter in this <paramref name="semaphore" /> until the specified <paramref name="millisecondsTimeout" />,
    /// the returned disposable will release the <paramref name="semaphore" /> if disposed.
    /// </summary>
    /// <param name="semaphore">The <see cref="SemaphoreSlim" /> to enter.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite" />(-1) to wait indefinitely.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A disposable which release <paramref name="semaphore" /> when disposed.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecondsTimeout" /> is equals to 0.</exception>
    public static IDisposable Enter(this SemaphoreSlim semaphore, int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(semaphore);

        if (millisecondsTimeout == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout), $"The '{nameof(millisecondsTimeout)}' cannot be equals to 0.");
        }

        _ = semaphore.Wait(millisecondsTimeout, cancellationToken);

        return new SemaphoreLockDisposable(semaphore);
    }

    /// <summary>
    /// Waits to enter in this <paramref name="semaphore" /> until the specified <paramref name="timeout" />,
    /// the returned disposable will release the <paramref name="semaphore" /> if disposed.
    /// </summary>
    /// <param name="semaphore">The <see cref="SemaphoreSlim" /> to enter.</param>
    /// <param name="timeout">A <see cref="TimeSpan" /> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan" /> that represents -1 milliseconds to wait indefinitely</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A disposable which release <paramref name="semaphore" /> when disposed.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout" /> is equals to 0.</exception>
    public static IDisposable Enter(this SemaphoreSlim semaphore, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(semaphore);

        if (timeout.TotalMilliseconds == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), $"The '{nameof(timeout)}' cannot be equals to 0.");
        }

        _ = semaphore.Wait(timeout, cancellationToken);

        return new SemaphoreLockDisposable(semaphore);
    }

    /// <summary>
    /// Asynchronously waits to enter in this <paramref name="semaphore" />,
    /// the returned disposable will release the <paramref name="semaphore" /> if disposed.
    /// </summary>
    /// <param name="semaphore">The <see cref="SemaphoreSlim" /> to enter.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>
    /// A <see cref="ValueTask" /> that represents an asynchronous operation where the result is
    /// a disposable which release <paramref name="semaphore" /> when disposed.
    /// </returns>
    public static async ValueTask<IDisposable> EnterAsync(this SemaphoreSlim semaphore, CancellationToken cancellationToken = default)
    {
        if (semaphore.Wait(0, cancellationToken))
        {
            return new SemaphoreLockDisposable(semaphore);
        }

        ArgumentNullException.ThrowIfNull(semaphore);

        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        return new SemaphoreLockDisposable(semaphore);
    }

    /// <summary>
    /// Asynchronously waits to enter in this <paramref name="semaphore" /> until the specified <paramref name="millisecondsTimeout" />,
    /// the returned disposable will release the <paramref name="semaphore" /> if disposed.
    /// </summary>
    /// <param name="semaphore">The <see cref="SemaphoreSlim" /> to enter.</param>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait,
    /// <see cref="Timeout.Infinite" />(-1) to wait indefinitely.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>
    /// A <see cref="ValueTask" /> that represents an asynchronous operation where the result is
    /// a disposable which release <paramref name="semaphore" /> when disposed.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecondsTimeout" /> is equals to 0.</exception>
    public static async ValueTask<IDisposable> EnterAsync(this SemaphoreSlim semaphore, int millisecondsTimeout, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(semaphore);

        if (millisecondsTimeout == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout), $"The '{nameof(millisecondsTimeout)}' cannot be equals to 0.");
        }

        if (semaphore.Wait(0, cancellationToken))
        {
            return new SemaphoreLockDisposable(semaphore);
        }

        _ = await semaphore.WaitAsync(millisecondsTimeout, cancellationToken).ConfigureAwait(false);

        return new SemaphoreLockDisposable(semaphore);
    }

    /// <summary>
    /// Asynchronously waits to enter in this <paramref name="semaphore" /> until the specified <paramref name="timeout" />,
    /// the returned disposable will release the <paramref name="semaphore" /> if disposed.
    /// </summary>
    /// <param name="semaphore">The <see cref="SemaphoreSlim" /> to enter.</param>
    /// <param name="timeout">A <see cref="TimeSpan" /> that represents the number of milliseconds to wait,
    /// a <see cref="TimeSpan" /> that represents -1 milliseconds to wait indefinitely</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>
    /// A <see cref="ValueTask" /> that represents an asynchronous operation where the result is
    /// a disposable which release <paramref name="semaphore" /> when disposed.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout" /> is equals to 0.</exception>
    public static async ValueTask<IDisposable> EnterAsync(this SemaphoreSlim semaphore, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(semaphore);

        if (timeout.TotalMilliseconds == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), $"The '{nameof(timeout)}' cannot be equals to 0.");
        }

        if (semaphore.Wait(0, cancellationToken))
        {
            return new SemaphoreLockDisposable(semaphore);
        }

        _ = await semaphore.WaitAsync(timeout, cancellationToken).ConfigureAwait(false);

        return new SemaphoreLockDisposable(semaphore);
    }

    private class SemaphoreLockDisposable : IDisposable
    {
        private SemaphoreSlim? _semaphore;

        public SemaphoreLockDisposable(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public void Dispose()
        {
            _ = (_semaphore?.Release());
            _semaphore = null;
        }
    }
}
