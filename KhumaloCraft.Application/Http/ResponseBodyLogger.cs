using Microsoft.AspNetCore.Http;
using System.Text;

namespace KhumaloCraft.Application.Http
{
    public class ResponseBodyLogger : IDisposable
    {
        private MemoryStream _logStream;

        public ResponseBodyLogger(HttpContext context, int maxLength = 64 * 1024)
        {
            _logStream = new MemoryStream();
            context.Response.Body = new ProxyLogStream(context.Response.Body, _logStream, maxLength);
        }

        public string GetResponseBodyAsString()
        {
            return Encoding.UTF8.GetString(_logStream.ToArray());
        }

        public void Dispose()
        {
            _logStream.Dispose();
        }

        private class ProxyLogStream : Stream
        {
            private Stream _wrappedStream;
            private MemoryStream _logStream;
            private int _maxLogLength;

            public ProxyLogStream(Stream wrappedStream, MemoryStream logStream, int maxLogLength)
            {
                _wrappedStream = wrappedStream;
                _logStream = logStream;
                _maxLogLength = maxLogLength;
            }

            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override void Flush()
            {
                _wrappedStream.Flush();
            }

            public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
            {
                WriteToLogStream(buffer);
                return _wrappedStream.WriteAsync(buffer, cancellationToken);
            }

            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                return _wrappedStream.FlushAsync(cancellationToken);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _wrappedStream.Write(buffer, offset, count);

                WriteToLogStream(buffer.AsMemory(offset, count));
            }

            private void WriteToLogStream(ReadOnlyMemory<byte> buffer)
            {
                if (_logStream.Length < _maxLogLength)
                {
                    var remainingBytes = _maxLogLength - (int)_logStream.Length;
                    _logStream.Write(buffer.Slice(0, Math.Min(remainingBytes, buffer.Length)).Span);
                }
            }

            #region NotSupported
            public override long Length => throw new NotSupportedException();

            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }
            #endregion
        }
    }
}
