using System.Diagnostics;
using System.Security.Cryptography;

namespace KhumaloCraft.Application.Session;

// taken from http://referencesource.microsoft.com/#System.Web/State/SessionIDManager.cs,48c8139a6df88e00
// Microsoft's implementation of session ID generation is subject to change at any time,
// which we really don't want, hence why we have "frozen" their code here in this manner
internal class UniqueUserIdGenerator
{
    internal const int NUM_CHARS_IN_ENCODING = 32;
    internal const int ENCODING_BITS_PER_CHAR = 5;
    internal const int ID_LENGTH_BITS = 120;
    internal const int ID_LENGTH_BYTES = (ID_LENGTH_BITS / 8);                      // 15
    internal const int ID_LENGTH_CHARS = (ID_LENGTH_BITS / ENCODING_BITS_PER_CHAR); // 24

    private static readonly char[] _encoding = {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                '0', '1', '2', '3', '4', '5'
    };

    private RandomNumberGenerator _randgen;

    public String Create()
    {
        if (_randgen == null)
        {
            _randgen = RandomNumberGenerator.Create();
        }

        var buffer = new byte[15];

        _randgen.GetBytes(buffer);

        return Encode(buffer);
    }

    private static String Encode(byte[] buffer)
    {
        int i, j, k, n;
        char[] chars = new char[ID_LENGTH_CHARS];

        Debug.Assert(buffer.Length == ID_LENGTH_BYTES);

        j = 0;
        for (i = 0; i < ID_LENGTH_BYTES; i += 5)
        {
            n = (int)buffer[i] |
                 ((int)buffer[i + 1] << 8) |
                 ((int)buffer[i + 2] << 16) |
                 ((int)buffer[i + 3] << 24);

            k = (n & 0x0000001F);
            chars[j++] = _encoding[k];

            k = ((n >> 5) & 0x0000001F);
            chars[j++] = _encoding[k];

            k = ((n >> 10) & 0x0000001F);
            chars[j++] = _encoding[k];

            k = ((n >> 15) & 0x0000001F);
            chars[j++] = _encoding[k];

            k = ((n >> 20) & 0x0000001F);
            chars[j++] = _encoding[k];

            k = ((n >> 25) & 0x0000001F);
            chars[j++] = _encoding[k];

            n = ((n >> 30) & 0x00000003) | ((int)buffer[i + 4] << 2);

            k = (n & 0x0000001F);
            chars[j++] = _encoding[k];

            k = ((n >> 5) & 0x0000001F);
            chars[j++] = _encoding[k];
        }

        Debug.Assert(j == ID_LENGTH_CHARS);

        return new String(chars);
    }
}