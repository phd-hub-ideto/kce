using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KhumaloCraft.Utilities;

public interface IByteIndexer
{
    long Length { get; }
    byte this[long i] { get; }
}

public class StreamView : IByteIndexer
{
    private byte[] _future = null;
    private long _futureStart = -1;
    private long _futureEnd = -1;

    private byte[] _past = null;
    private long _pastStart = -1;
    private long _pastEnd = -1;

    private void Next()
    {
        if (_future == null)
        {
            _future = new byte[_range];
            _futureStart = _stream.Position;
        }
        else
        {
            if (_past == null)
            {
                _past = new byte[_range];
            }

            //swap future and past
            (_past, _future) = (_future, _past);

            //set past indexes
            _pastStart = _futureStart;
            _pastEnd = _futureEnd;

            _futureStart = _futureEnd + 1;
        }

        //map the future
        _futureEnd += _stream.Read(_future, 0, _range);
    }

    private Stream _stream = null;
    private long _streamLength = -1;
    private long _streamPosition = -1;
    private int _range = -1;

    public StreamView(Stream Stream) : this(Stream, ByteArrayUtils.BUFFER_SIZE)
    {
    }

    public StreamView(Stream Stream, int Range)
    {

        _stream = Stream;
        _streamLength = _stream.Length;
        _streamPosition = _stream.Position;
        _range = Range;
    }

    public byte this[long i]
    {
        get
        {
            i += _streamPosition;

            if (_future == null)
            {
                Next(); //map the first view
            }

            if (i >= _streamLength) //overrun
            {
                if (UseOverrunByte)
                    return Overrun;
                else
                    throw new Exception("Stream index overrun.");
            }

            if (((_past == null) && (i < _futureStart)) || (i < _pastStart)) //underrun
            {
                if (UseUnderrunByte)
                    return Underrun;
                else
                    throw new Exception("Stream index underrun.");
            }

            while (i > _futureEnd)
                Next();

            if (i > _pastEnd)
            {
                return _future[i - _futureStart];
            }
            else
            {
                return _past[i - _pastStart];
            }

        }
    }
    public long Length
    {
        get
        {
            return _streamLength - _streamPosition;
        }
    }

    public bool UseOverrunByte = false;
    public byte Overrun = 0;

    public bool UseUnderrunByte = false;
    public byte Underrun = 0;

}