// Licensed to the NextAudio under one or more agreements.
// NextAudio licenses this file to you under the MIT license.

using System.ComponentModel;

namespace NextAudio;

/// <summary>
/// Represents an audio stream.
/// </summary>
public abstract class AudioStream : Stream
{
    /// <summary>
    /// Creates a clone of this stream.
    /// </summary>
    /// <returns>A new cloned stream.</returns>
    public abstract AudioStream Clone();

    #region Disable Browse of unused stream methods

    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return base.BeginRead(buffer, offset, count, callback, state);
    }

    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return base.BeginWrite(buffer, offset, count, callback, state);
    }

    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override void Close()
    {
        base.Close();
    }

    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override int EndRead(IAsyncResult asyncResult)
    {
        return base.EndRead(asyncResult);
    }

    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override void EndWrite(IAsyncResult asyncResult)
    {
        base.EndWrite(asyncResult);
    }

    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override void Flush()
    {
        // Deprecated member.
    }

    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        // Deprecated member.

        return cancellationToken.IsCancellationRequested ? Task.FromCanceled(cancellationToken) : Task.CompletedTask;
    }

    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override bool CanTimeout => base.CanTimeout; // Deprecated member.

    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override int WriteTimeout // Deprecated member.
    {
        get => base.WriteTimeout;
        set => base.WriteTimeout = value;
    }

    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override int ReadTimeout // Deprecated member.
    {
        get => base.ReadTimeout;
        set => base.ReadTimeout = value;
    }

    #endregion Disable Browse of unused stream methods
}
