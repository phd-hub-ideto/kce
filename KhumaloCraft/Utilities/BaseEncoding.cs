using System.Text;

namespace KhumaloCraft.Utilities;

public static class BaseEncoding
{
    internal static readonly Encoding DefaultEncoding = Encoding.UTF8;

    private static string ToBaseString(string source, Action<Stream, Stream> action, Encoding encoding)
    {
        using var sourceStream = new MemoryStream((encoding ?? DefaultEncoding).GetBytes(source));

        using var destinationStream = new MemoryStream();

        action(sourceStream, destinationStream);

        return Encoding.Default.GetString(destinationStream.ToArray());
    }

    private const int BYTE_BITS = 8;

    internal static readonly byte[] BASE_16_ALPHABET = [(byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F'];
    internal static readonly int[] BASE_16_INDEXES = [-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1, -1, -1, -1, -1, -1, -1, 10, 11, 12, 13, 14, 15, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, 11, 12, 13, 14, 15];
    internal static readonly byte BASE_16_LOW = BASE_16_ALPHABET[0];
    internal static readonly byte BASE_16_HIGH = BASE_16_ALPHABET[BASE_16_ALPHABET.Length - 1];

    public static string ToBase16(string source, Encoding encoding = null)
    {
        return ToBaseString(source, ToBase16, encoding);
    }

    public static void ToBase16(Stream source, Stream destination)
    {
        var sv = new StreamView(source);
        sv.UseOverrunByte = true;

        ToBase16(sv, destination);
    }

    public static void ToBase16(IByteIndexer source, Stream destination)
    {

        int len = (int)source.Length;
        if (len == 0)
            return;

        for (int i = 0; i < len; i++)
        {
            byte b = source[i];
            destination.WriteByte(BASE_16_ALPHABET[b >> 4]);
            destination.WriteByte(BASE_16_ALPHABET[b & 0xF]);
        }

    }

    public static void FromBase16(Stream source, Stream destination)
    {
        var sv = new StreamView(source);
        sv.UseOverrunByte = true;

        FromBase16(sv, destination);
    }

    public static void FromBase16(IByteIndexer source, Stream destination)
    {

        int len = (int)source.Length;
        if (len == 0)
            return;

        for (int i = 0; i < len; i += 2)
        {

            byte a = source[i];
            byte b = source[i + 1];

            if ((a < BASE_16_LOW) || (a > BASE_16_HIGH) || (b < BASE_16_LOW) || (b > BASE_16_HIGH))
                throw new Exception("Invalid Base16 data.");

            destination.WriteByte((byte)((BASE_16_INDEXES[(int)a] << 4) + BASE_16_INDEXES[(int)b]));

        }

    }

    private const int BASE_32_SIG_BITS_MASK = 224;
    private const int BASE_32_SIG_BITS = 3;
    private const int BASE_32_OTHER_BITS = 5;
    private const int BASE_32_OTHER_BITS_MASK = 31;

    internal static readonly byte[] BASE_32_RFC3548_ALPHABET = [(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G', (byte)'H', (byte)'I', (byte)'J', (byte)'K', (byte)'L', (byte)'M', (byte)'N', (byte)'O', (byte)'P', (byte)'Q', (byte)'R', (byte)'S', (byte)'T', (byte)'U', (byte)'V', (byte)'W', (byte)'X', (byte)'Y', (byte)'Z', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7'];
    internal static readonly byte[] BASE_32_RFC3548_INDEXES = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 26, 27, 28, 29, 30, 31, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

    internal static readonly byte[] BASE_32_LENIENT_ALPHABET = [(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G', (byte)'H', (byte)'I', (byte)'J', (byte)'K', (byte)'L', (byte)'M', (byte)'N', (byte)'O', (byte)'P', (byte)'Q', (byte)'R', (byte)'S', (byte)'T', (byte)'U', (byte)'V', (byte)'W', (byte)'X', (byte)'Y', (byte)'Z', (byte)'2', (byte)'3', (byte)'4', (byte)'6', (byte)'7', (byte)'9'];
    internal static readonly byte[] BASE_32_LENIENT_INDEXES = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 14, 8, 26, 27, 28, 18, 29, 30, 1, 31, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

    private static void ToBase32(IByteIndexer source, Stream destination, byte[] alphabet)
    {

        int size = (int)source.Length;
        if (size == 0)
            return;

        int len = (int)Math.Ceiling(size * BYTE_BITS / (double)BASE_32_OTHER_BITS);

        for (int i = 0; i < len; i++)
        {

            int bitoffset = i * BASE_32_OTHER_BITS;
            int bytepos = bitoffset / BYTE_BITS;
            int offset = bitoffset % BYTE_BITS;

            destination.WriteByte(alphabet[(byte)((((source[bytepos] << offset) | (source[bytepos + 1] >> (BYTE_BITS - offset))) >> BASE_32_SIG_BITS) & BASE_32_OTHER_BITS_MASK)]);
        }

    }

    private static void FromBase32(IByteIndexer source, Stream destination, byte[] indexes)
    {

        int size = (int)source.Length;
        if (size == 0)
            return;

        int len = size * BASE_32_OTHER_BITS / BYTE_BITS;

        for (int i = 0; i < len; i++)
        {

            int bytepos1 = ((i * BASE_32_SIG_BITS) / BASE_32_OTHER_BITS) + i;
            int bytepos2 = bytepos1 + 1;
            int bytepos3 = bytepos2 + 1;

            int offset1 = (((bytepos2) * BASE_32_SIG_BITS) % BYTE_BITS);
            int offset2 = offset1 - BASE_32_OTHER_BITS;
            int offset3 = offset2 - BASE_32_OTHER_BITS;

            destination.WriteByte((byte)((indexes[source[bytepos1]] << offset1) | (offset2 < 0 ? indexes[source[bytepos2]] >> -offset2 : indexes[source[bytepos2]] << offset2) | (indexes[source[bytepos3]] >> -offset3)));

        }
    }

    public enum Base32CharacterSet
    {
        RFC3548,
        Lenient
    }

    public static string ToBase32(string source, Base32CharacterSet base32CharacterSet, Encoding encoding = null)
    {
        return ToBaseString(source, (s, d) => ToBase32(s, d, base32CharacterSet), encoding);
    }

    public static void ToBase32(Stream source, Stream destination, Base32CharacterSet base32CharacterSet)
    {

        var sv = new StreamView(source);
        sv.UseOverrunByte = true;

        ToBase32(sv, destination, base32CharacterSet);
    }

    public static void ToBase32(IByteIndexer source, Stream destination, Base32CharacterSet base32CharacterSet)
    {
        byte[] alphabet = null;

        switch (base32CharacterSet)
        {
            case Base32CharacterSet.RFC3548:
                alphabet = BASE_32_RFC3548_ALPHABET;
                break;
            case Base32CharacterSet.Lenient:
                alphabet = BASE_32_LENIENT_ALPHABET;
                break;
        }

        ToBase32(source, destination, alphabet);
    }

    public static void FromBase32(Stream source, Stream destination, Base32CharacterSet base32CharacterSet)
    {

        var sv = new StreamView(source);
        sv.UseOverrunByte = true;

        FromBase32(sv, destination, base32CharacterSet);
    }

    public static void FromBase32(IByteIndexer source, Stream destination, Base32CharacterSet base32CharacterSet)
    {

        byte[] indexes = null;

        switch (base32CharacterSet)
        {
            case Base32CharacterSet.RFC3548:
                indexes = BASE_32_RFC3548_INDEXES;
                break;
            case Base32CharacterSet.Lenient:
                indexes = BASE_32_LENIENT_INDEXES;
                break;
        }

        FromBase32(source, destination, indexes);

    }

    internal static readonly byte[] XX_ALPHABET = [(byte)'+', (byte)'-', (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G', (byte)'H', (byte)'I', (byte)'J', (byte)'K', (byte)'L', (byte)'M', (byte)'N', (byte)'O', (byte)'P', (byte)'Q', (byte)'R', (byte)'S', (byte)'T', (byte)'U', (byte)'V', (byte)'W', (byte)'X', (byte)'Y', (byte)'Z', (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f', (byte)'g', (byte)'h', (byte)'i', (byte)'j', (byte)'k', (byte)'l', (byte)'m', (byte)'n', (byte)'o', (byte)'p', (byte)'q', (byte)'r', (byte)'s', (byte)'t', (byte)'u', (byte)'v', (byte)'w', (byte)'x', (byte)'y', (byte)'z'];
    internal static readonly byte[] XX_INDEXES = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 0, 0, 0, 0, 0, 0, 0, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 0, 0, 0, 0, 0, 0, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 0, 0, 0, 0, 0];

    public static string ToXX(string source, Encoding encoding = null)
    {
        return ToBaseString(source, ToXX, encoding);
    }
    public static void ToXX(Stream source, Stream destination)
    {
        StreamView sv = new StreamView(source);
        sv.UseOverrunByte = true;

        ToXX(sv, destination);
    }
    public static void ToXX(IByteIndexer source, Stream destination)
    {
        int len = (int)source.Length;
        if (len == 0)
            return;

        int padlen = 3 - (len % 3);
        len += padlen;

        int i = 0;
        while (true)
        {
            int x = (source[i] & 255) | ((source[i + 1] & 255) << 8) | ((source[i + 2] & 255) << 16);

            i += 3;
            bool last = i >= len;

            destination.WriteByte(XX_ALPHABET[x & 63]);
            destination.WriteByte(XX_ALPHABET[(x >> 6) & 63]);
            destination.WriteByte(XX_ALPHABET[(x >> 12) & 63]);
            destination.WriteByte(XX_ALPHABET[(x >> 18) & 63]);

            if (last && (padlen != 0))
                destination.WriteByte((byte)padlen.ToString()[0]);

            if (last)
                return;
        }

    }

    public static void FromXX(Stream source, Stream destination)
    {
        var sv = new StreamView(source)
        {
            UseOverrunByte = true
        };

        FromXX(sv, destination);
    }

    public static void FromXX(IByteIndexer source, Stream destination)
    {
        int trim = 0;

        int len = (int)source.Length;
        if (len == 0)
            return;

        int i = 0;

        while (true)
        {
            int x = (XX_INDEXES[((int)source[i])] & 255) | ((XX_INDEXES[((int)source[i + 1])] & 255) << 6) | ((XX_INDEXES[((int)source[i + 2])] & 255) << 12) | ((XX_INDEXES[((int)source[i + 3])] & 255) << 18);

            i += 4;

            bool last = (i + 4) >= len;

            if (last)
            {
                if (((len % 4) != 0) && (source[len - 1] >= '0' && source[len - 1] <= '9'))
                    trim = Convert.ToInt32(((char)source[len - 1]).ToString());
            }

            if (!last || trim < 3)
                destination.WriteByte((byte)(x & 255));

            if (!last || trim < 2)
                destination.WriteByte((byte)((x >> 8) & 255));

            if (!last || trim < 1)
                destination.WriteByte((byte)((x >> 16) & 255));

            if (last)
                return;
        }
    }

    internal static readonly byte[] BASE_64_ALPHABET = [(byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F', (byte)'G', (byte)'H', (byte)'I', (byte)'J', (byte)'K', (byte)'L', (byte)'M', (byte)'N', (byte)'O', (byte)'P', (byte)'Q', (byte)'R', (byte)'S', (byte)'T', (byte)'U', (byte)'V', (byte)'W', (byte)'X', (byte)'Y', (byte)'Z', (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f', (byte)'g', (byte)'h', (byte)'i', (byte)'j', (byte)'k', (byte)'l', (byte)'m', (byte)'n', (byte)'o', (byte)'p', (byte)'q', (byte)'r', (byte)'s', (byte)'t', (byte)'u', (byte)'v', (byte)'w', (byte)'x', (byte)'y', (byte)'z', (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'+', (byte)'/'];
    internal static readonly byte[] BASE_64_INDEXES = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 62, 0, 0, 0, 63, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 0, 0, 0, 0, 0, 0, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 0, 0, 0, 0, 0];
    internal static readonly byte BASE_64_PADDING = (byte)'=';

    public static string ToBase64(string source, Encoding encoding = null)
    {
        return ToBaseString(source, ToBase64, encoding);
    }

    public static void ToBase64(Stream source, Stream destination)
    {
        var sv = new StreamView(source);
        sv.UseOverrunByte = true;

        ToBase64(sv, destination);
    }

    public static void ToBase64(IByteIndexer source, Stream destination)
    {

        int len = (int)source.Length;

        if (len == 0)
            return;

        int padlen = (3 - (len % 3)) % 3;

        int i = 0;

        while (true)
        {

            byte x = source[i];
            byte y = source[i + 1];
            byte z = source[i + 2];

            i += 3;

            bool last = i >= len;

            destination.WriteByte(BASE_64_ALPHABET[x >> 0x2]);
            destination.WriteByte(BASE_64_ALPHABET[((x & 0x3) << 0x4) | (y >> 0x4)]);

            if (last && (padlen == 2))
                destination.WriteByte(BASE_64_PADDING);
            else
                destination.WriteByte(BASE_64_ALPHABET[((y & 0xF) << 0x2) | (z >> 0x6)]);
            if (last && (padlen != 0))
                destination.WriteByte(BASE_64_PADDING);
            else
                destination.WriteByte(BASE_64_ALPHABET[z & 0x3F]);

            if (last)
                return;

        }
    }

    public static void FromBase64(Stream source, Stream destination)
    {

        var sv = new StreamView(source);

        sv.UseOverrunByte = true;

        FromBase64(sv, destination);

    }
    public static void FromBase64(IByteIndexer source, Stream destination)
    {

        int len = (int)source.Length;

        if (len == 0)
            return;

        int i = 0;

        while (true)
        {

            byte c = source[i + 2];
            byte d = source[i + 3];

            byte aa = BASE_64_INDEXES[source[i]];
            byte bb = BASE_64_INDEXES[source[i + 1]];
            byte cc = BASE_64_INDEXES[c];
            byte dd = BASE_64_INDEXES[d];

            i += 4;

            bool last = i >= len;

            destination.WriteByte((byte)((aa << 0x2) | (bb >> 0x4)));
            if (last && (c == BASE_64_PADDING))
                return;
            destination.WriteByte((byte)((bb << 0x4) | (cc >> 0x2)));
            if (last && (d == BASE_64_PADDING))
                return;
            destination.WriteByte((byte)((cc << 0x6) | dd));

            if (last)
                return;

        }
    }

    private const int BASE_128_SIG_BITS_MASK = 128;
    private const int BASE_128_SIG_BITS = 1;
    private const int BASE_128_OTHER_BITS = 7;
    private const int BASE_128_OTHER_BITS_MASK = 127;

    public enum Base128CharacterSet
    {
        LowPage,
        HighPage
    }

    public static string ToBase128(string source, Base128CharacterSet base128CharacterSet, Encoding encoding = null)
    {
        return ToBaseString(source, (s, d) => ToBase128(s, d, base128CharacterSet), encoding);
    }

    public static void ToBase128(Stream source, Stream destination, Base128CharacterSet base128CharacterSet)
    {

        StreamView sv = new StreamView(source);
        sv.UseOverrunByte = true;

        ToBase128(sv, destination, base128CharacterSet);

    }

    public static void ToBase128(IByteIndexer source, Stream destination, Base128CharacterSet base128CharacterSet)
    {

        int size = (int)source.Length;
        if (size == 0)
            return;

        int len = size + (int)Math.Ceiling(size / (double)BASE_128_OTHER_BITS);

        int shift = 0;
        if (base128CharacterSet == Base128CharacterSet.HighPage)
            shift = BASE_128_SIG_BITS_MASK;

        for (int i = 0; i < len; i++)
        {
            int bitoffset = i * BASE_128_OTHER_BITS;
            int bytepos = bitoffset / 8;
            int offset = bitoffset % 8;

            destination.WriteByte((byte)(((((source[bytepos] << offset) | (source[bytepos + 1] >> (BYTE_BITS - offset))) >> BASE_128_SIG_BITS) | BASE_128_SIG_BITS_MASK) - shift));
        }
    }

    public static void FromBase128(Stream source, Stream destination)
    {
        var sv = new StreamView(source);

        sv.UseOverrunByte = true;

        FromBase128(sv, destination);
    }

    public static void FromBase128(IByteIndexer source, Stream destination)
    {

        int size = (int)source.Length;
        if (size == 0)
            return;

        int len = (size * BASE_128_OTHER_BITS) / BYTE_BITS;

        for (int i = 0; i < len; i++)
        {
            int bytepos = (i / BASE_128_OTHER_BITS) + i;

            int offset = (i % BASE_128_OTHER_BITS) + BASE_128_SIG_BITS;

            destination.WriteByte((byte)(((source[bytepos] & BASE_128_OTHER_BITS_MASK) << offset) | ((source[bytepos + 1] & BASE_128_OTHER_BITS_MASK) >> (BASE_128_OTHER_BITS - offset))));
        }

    }
}