using System.IO.Compression;
using System.Text;

namespace KhumaloCraft.Utilities;

public static class StreamUtils
{
    internal static void DoCopyStream(Stream From, Stream To, DataTransformType Transform)
    {

        Stream OldTo = null;
        Stream OldFrom = null;

        switch (Transform)
        {
            case DataTransformType.Bits:
                break;
            case DataTransformType.Deflate:
                OldTo = To;
                To = new DeflateStream(To, CompressionMode.Compress, true);
                break;
            case DataTransformType.Inflate:
                OldFrom = From;
                From = new DeflateStream(From, CompressionMode.Decompress, true);
                break;
            case DataTransformType.GZip:
                OldTo = To;
                To = new GZipStream(To, CompressionMode.Compress, true);
                break;
            case DataTransformType.GUnZip:
                OldFrom = From;
                From = new GZipStream(From, CompressionMode.Decompress, true);
                break;
            case DataTransformType.ToBase16:
                BaseEncoding.ToBase16(From, To);
                return;
            case DataTransformType.FromBase16:
                BaseEncoding.FromBase16(From, To);
                return;
            case DataTransformType.ToBase64:
                BaseEncoding.ToBase64(From, To);
                return;
            case DataTransformType.FromBase64:
                BaseEncoding.FromBase64(From, To);
                return;
            case DataTransformType.ToBase128:
                BaseEncoding.ToBase128(From, To, BaseEncoding.Base128CharacterSet.LowPage);
                return;
            case DataTransformType.FromBase128:
                BaseEncoding.FromBase128(From, To);
                return;
            case DataTransformType.ToBase32:
                BaseEncoding.ToBase32(From, To, BaseEncoding.Base32CharacterSet.RFC3548);
                return;
            case DataTransformType.FromBase32:
                BaseEncoding.FromBase32(From, To, BaseEncoding.Base32CharacterSet.RFC3548);
                return;
            case DataTransformType.ToBase32Lenient:
                BaseEncoding.ToBase32(From, To, BaseEncoding.Base32CharacterSet.Lenient);
                return;
            case DataTransformType.FromBase32Lenient:
                BaseEncoding.FromBase32(From, To, BaseEncoding.Base32CharacterSet.Lenient);
                return;
            case DataTransformType.XXEncode:
                BaseEncoding.ToXX(From, To);
                return;
            case DataTransformType.XXDecode:
                BaseEncoding.FromXX(From, To);
                return;
            default:
                break;
        }

        try
        {

            DoCopyStream(From, To);

        }
        finally
        {

            if (OldTo != null)
                To.Dispose(); //kill the one that we created

            if (OldFrom != null)
                From.Dispose(); //kill the one that we created

        }

    }

    private static void DoCopyStream(Stream From, Stream To)
    {

        byte[] buffer = new byte[ByteArrayUtils.BUFFER_SIZE];
        int c;
        do
        {
            c = From.Read(buffer, 0, buffer.Length);
            if (c > 0)
            {
                To.Write(buffer, 0, c);
            }

        } while (c > 0);

        To.Flush();

    }

    public static void CopyStream(Stream From, Stream To, params DataTransformType[] Transform)
    {
        List<DataTransformType> transforms = new List<DataTransformType>(Transform);

        if (transforms.Count == 0)
            transforms.Add(DataTransformType.Bits);

        MemoryStream ms1 = null;
        MemoryStream ms2 = null;

        try
        {

            Stream inp = null;
            Stream outp = null;

            for (int i = 0; i < transforms.Count; i++)
            {
                DataTransformType trans = transforms[i];
                if (i == 0) //the first one 
                {
                    inp = From;
                }
                else
                {
                    if (i % 2 == 0) //even iterations
                    {
                        if (ms2 == null)
                            ms2 = new MemoryStream();
                        inp = ms2;
                    }
                    else //odd iterations
                    {
                        if (ms1 == null)
                            ms1 = new MemoryStream();
                        inp = ms1;
                    }
                    inp.Position = 0; //position our temp stream
                }
                if (i == (transforms.Count - 1)) //last one 
                {
                    outp = To;
                }
                else
                {
                    if (i % 2 == 0) //even iterations
                    {
                        if (ms1 == null)
                            ms1 = new MemoryStream();
                        outp = ms1;
                    }
                    else //odd iterations
                    {
                        if (ms2 == null)
                            ms2 = new MemoryStream();
                        outp = ms2;
                    }
                    outp.SetLength(0); //clear the output stream
                }

                DoCopyStream(inp, outp, trans);

                if ((outp == ms1) || (outp == ms2))
                    outp.Position = 0; //temp memory buffer move to start
            }
        }
        finally
        {
            if (ms1 != null)
                ms1.Dispose();
            if (ms2 != null)
                ms2.Dispose();
        }

    }

    public static void Transform(Stream From, Stream To, params DataTransformType[] Transform)
    {
        CopyStream(From, To, Transform);
    }

    public static Byte[] ToByteArray(Stream From, params DataTransformType[] Transform)
    {
        using var ToStream = new MemoryStream();

        CopyStream(From, ToStream, Transform);

        return ToStream.ToArray();
    }

    public static void FromByteArray(byte[] From, Stream To, params DataTransformType[] Transform)
    {
        using var fromStream = new MemoryStream(From);

        CopyStream(fromStream, To, Transform);
    }

    public static bool CompareStream(Stream Stream1, Stream Stream2)
    {

        if (Stream1.CanSeek)
            Stream1.Position = 0;

        if (Stream2.CanSeek)
            Stream2.Position = 0;

        byte[] buffer1 = new byte[ByteArrayUtils.BUFFER_SIZE];
        byte[] buffer2 = new byte[ByteArrayUtils.BUFFER_SIZE];
        int c1;
        int c2;
        do
        {
            c1 = Stream1.Read(buffer1, 0, buffer1.Length);
            c2 = Stream2.Read(buffer2, 0, buffer2.Length);

            if (c1 != c2)
                return false;

            if (!ByteArrayUtils.Compare(buffer1, buffer2, c1))
                return false;

        } while (c1 > 0);

        return true;

    }

    public static string ToString(Stream Stream)
    {
        return ToString(Stream, Encoding.Default);
    }
    public static string ToString(Stream Stream, Encoding Encoding)
    {
        byte[] bytes = new byte[Stream.Length - Stream.Position];
        Stream.Read(bytes, 0, bytes.Length);
        return Encoding.GetString(bytes);
    }

    public static void FromString(String String, Stream Stream)
    {
        FromString(String, Stream, Encoding.Default);
    }
    public static void FromString(String String, Stream Stream, Encoding Encoding)
    {
        byte[] bytes = Encoding.GetBytes(String);
        Stream.Write(bytes, 0, bytes.Length);
    }

    public static byte[] ToArray(this Stream stream)
    {
        return ToByteArray(stream);
    }
    public static byte[] ToArray(this Stream stream, params DataTransformType[] transform)
    {
        return ToByteArray(stream, transform);
    }

}
