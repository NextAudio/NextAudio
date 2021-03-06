using NextAudio.Utils;
using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace NextAudio
{
    /// <summary>
    /// A pipe based decoder that uses <see cref="IAudioDecoder" />.
    /// </summary>
    public class PipedDecoder : IDisposable, IAsyncDisposable
    {
        private bool _isDisposed;

        private readonly Pipe _inputPipe;
        private readonly Pipe _outputPipe;

        /// <summary>
        /// The decoder to be used for pipes.
        /// </summary>
        protected readonly IAudioDecoder _decoder;

        /// <summary>
        /// Creates a new instance of <see cref="PipedDecoder" />.
        /// </summary>
        /// <param name="decoder">The decoder to be used for pipes.</param>
        /// <param name="options">The pipe options to be used for pipes.</param>
        /// <exception cref="ArgumentNullException"><paramref name="decoder" /> cannot be null.</exception>
        public PipedDecoder(IAudioDecoder decoder, PipeOptions? options = null)
        {
            decoder.NotNull(nameof(decoder));

            _decoder = decoder;
            _inputPipe = new Pipe(options ?? PipeOptions.Default);
            _outputPipe = new Pipe(options ?? PipeOptions.Default);
        }

        /// <summary>
        /// The channels number of the input audio.
        /// </summary>
        public virtual int Channels => _decoder.Channels;

        /// <summary>
        /// The sample rate of the input audio.
        /// </summary>
        public virtual int SampleRate => _decoder.SampleRate;

        /// <summary>
        /// Reader for the decoded audio data.
        /// </summary>
        public PipeReader Reader => _outputPipe.Reader;

        /// <summary>
        /// Write for the encoded audio data.
        /// </summary>
        public PipeWriter Writer => _inputPipe.Writer;

        /// <summary>
        /// Reader for encoded audio data.
        /// </summary>
        protected PipeReader InputReader => _inputPipe.Reader;

        /// <summary>
        /// Writer for decoded audio data.
        /// </summary>
        protected PipeWriter OutputWriter => _outputPipe.Writer;

        /// <summary>
        /// Runs the decoder pipeline reading from <see cref="Writer" /> and writing to the <see cref="Reader" />.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous operation.</returns>
        public virtual async Task DecodeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                ReadResult inputReadResult = default;
                FlushResult outputFlushResult = default;

                while (
                    !inputReadResult.IsCompleted &&
                    !inputReadResult.IsCanceled &&
                    !outputFlushResult.IsCompleted &&
                    !outputFlushResult.IsCanceled &&
                    !cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    inputReadResult = await InputReader.ReadAsync(cancellationToken);

                    if (inputReadResult.IsCompleted || inputReadResult.IsCanceled)
                        return;

                    var inputBuffer = inputReadResult.Buffer;
                    var startPosition = inputBuffer.Start;

                    if (!inputBuffer.TryGet(ref startPosition, out var inputMemory))
                        return;

                    InputReader.AdvanceTo(inputBuffer.End);

                    var outputBuffer = OutputWriter.GetMemory();

                    var bytesReaded = _decoder.Decode(inputMemory.Span, outputBuffer.Span);

                    if (bytesReaded <= 0 || cancellationToken.IsCancellationRequested)
                        return;

                    OutputWriter.Advance(bytesReaded);

                    outputFlushResult = await OutputWriter.FlushAsync(cancellationToken);
                }
            }
            finally
            {
                await InputReader.CompleteAsync();
                await OutputWriter.CompleteAsync();
            }
        }

        /// <summary>
        /// Resets the decoder pipeline.
        /// </summary>
        public virtual void Reset()
        {
            _inputPipe.Reset();
            _outputPipe.Reset();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="PipedDecoder" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                Reader.Complete();
                Writer.Complete();
                InputReader.Complete();
                OutputWriter.Complete();
            }

            _isDisposed = true;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (_isDisposed)
                return;

            await DisposeAsyncCore();

            Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting managed resources asynchronously.
        /// </summary>
        /// <returns>A <see cref="ValueTask" /> that represents the asynchronous dispose operation.</returns>
        protected async ValueTask DisposeAsyncCore()
        {
            await Reader.CompleteAsync();
            await Writer.CompleteAsync();
            await InputReader.CompleteAsync();
            await OutputWriter.CompleteAsync();
        }

        /// <inheritdoc />
        ~PipedDecoder()
        {
            Dispose(false);
        }
    }
}