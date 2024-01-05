using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jworkz.ResonitePowerShellModule.Mocks;

/// <summary>
/// A mock stream mainly used for unit tests. This is based on the NullStream
/// class created by Microsoft.
/// </summary>
[ExcludeFromCodeCoverage]
public class MockStream : Stream
{
    public override bool CanRead => true;
    public override bool CanWrite => true;
    public override bool CanSeek => true;
    public override long Length => 0;
    public override long Position { get => 0; set { } }

    public override void CopyTo(Stream destination, int bufferSize) { }

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
        cancellationToken.IsCancellationRequested ?
            Task.FromCanceled(cancellationToken) :
            Task.CompletedTask;

    protected override void Dispose(bool disposing) { }

    public override void Flush() { }

    public override Task FlushAsync(CancellationToken cancellationToken) =>
        cancellationToken.IsCancellationRequested ?
            Task.FromCanceled(cancellationToken) :
            Task.CompletedTask;

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        TaskToAsyncResult.Begin(Task.CompletedTask, callback, state);

    public override int EndRead(IAsyncResult asyncResult) =>
        TaskToAsyncResult.End<int>(asyncResult);

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) =>
        TaskToAsyncResult.Begin(Task.CompletedTask, callback, state);

    public override void EndWrite(IAsyncResult asyncResult) =>
        TaskToAsyncResult.End(asyncResult);

    public override int Read(byte[] buffer, int offset, int count) => 0;

    public override int Read(Span<byte> buffer) => 0;

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        cancellationToken.IsCancellationRequested ?
            Task.FromCanceled<int>(cancellationToken) :
            Task.FromResult(0);

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken) =>
        cancellationToken.IsCancellationRequested ?
            ValueTask.FromCanceled<int>(cancellationToken) :
            default;

    public override int ReadByte() => -1;

    public override void Write(byte[] buffer, int offset, int count) { }

    public override void Write(ReadOnlySpan<byte> buffer) { }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        cancellationToken.IsCancellationRequested ?
            Task.FromCanceled(cancellationToken) :
            Task.CompletedTask;

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
        cancellationToken.IsCancellationRequested ?
            ValueTask.FromCanceled(cancellationToken) :
            default;

    public override void WriteByte(byte value) { }

    public override long Seek(long offset, SeekOrigin origin) => 0;

    public override void SetLength(long length) { }
}
