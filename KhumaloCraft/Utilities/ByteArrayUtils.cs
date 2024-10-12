using System.Text;

namespace KhumaloCraft.Utilities;

/// <summary>
/// A container class for utility methods for dealing with byte arrays
/// </summary>
public static class ByteArrayUtils
{

    /// <summary>
    /// The size of the buffer to use when dealing with byte arrays
    /// </summary>
    public const int BUFFER_SIZE = 64 * 1024; //must be divisible by two

    /// <summary>
    /// Compares two byte arrays and determines if their contents are equal
    /// </summary>
    /// <param name="Source">The source byte array</param>
    /// <param name="Target">The target byte array</param>
    /// <param name="StartIndex">The starting index of both arrays</param>
    /// <param name="Count">The length to examine</param>
    /// <returns><code>true</code>if both arrays contents are equal to one another, <code>false</code> if they are not</returns>
    public static bool Compare(byte[] Source, byte[] Target, int StartIndex, int Count)
    {
        int EndIndex = StartIndex + Count - 1;
        if ((StartIndex < 0) || (EndIndex >= Source.Length) || (EndIndex >= Target.Length))
            return false;

        for (int count = StartIndex; count < EndIndex; count++)
        {
            if (Source[count] != Target[count])
                return false;
        }

        return true;

    }

    /// <summary>
    /// Compares two byte arrays and determines if their contents are equal
    /// </summary>
    /// <param name="Source">The source byte array</param>
    /// <param name="Target">The target byte array</param>
    /// <param name="Count">The length to examine</param>
    /// <returns><code>true</code>if both arrays contents are equal to one another, <code>false</code> if they are not</returns>
    public static bool Compare(byte[] Source, byte[] Target, int Count)
    {
        return Compare(Source, Target, 0, Count);
    }

    /// <summary>
    /// Compares two byte arrays and determines if their contents are equal
    /// </summary>
    /// <param name="Source">The source byte array</param>
    /// <param name="Target">The target byte array</param>
    /// <returns><code>true</code>if both arrays contents are equal to one another, <code>false</code> if they are not</returns>
    public static bool Compare(byte[] Source, byte[] Target)
    {
        return Compare(Source, Target, 0, Math.Max(Source.Length, Target.Length));
    }

    /// <summary>
    /// Copies a range of elements in one byte array to another byte
    /// </summary>
    /// <param name="Source">The byte array that contains the data to copy.</param>
    /// <param name="SourceIndex">A 32-bit integer that represents the index in the source byte array at which copying begins.</param>
    /// <param name="Target">The byte array that receives the data.</param>
    /// <param name="TargetIndex">A 32-bit integer that represents the index in the destination byte array at which storing begins.</param>
    /// <param name="Count">A 32-bit integer that represents the number of elements to copy.</param>
    [Obsolete("This function is implemented in System.Array.Copy")]
    public static void Copy(byte[] Source, int SourceIndex, byte[] Target, int TargetIndex, int Count)
    {
        Array.Copy(Source, SourceIndex, Target, TargetIndex, Count);
    }

    /// <summary>
    /// Copies a range of elements in one byte array to another byte
    /// </summary>
    /// <param name="Source">The byte array that contains the data to copy.</param>
    /// <param name="SourceIndex">A 32-bit integer that represents the index in the source byte array at which copying begins.</param>
    /// <param name="Target">The byte array that receives the data.</param>
    /// <param name="TargetIndex">A 32-bit integer that represents the index in the destination byte array at which storing begins.</param>
    [Obsolete("This function is implemented in System.Array.Copy")]
    public static void Copy(byte[] Source, int SourceIndex, byte[] Target, int TargetIndex)
    {
        ByteArrayUtils.Copy(Source, SourceIndex, Target, TargetIndex, Source.Length);
    }

    /// <summary>
    /// Clones an entire array to the target
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">Length is less than zero.</exception>
    /// <param name="Source">The source of byte array to copy from</param>
    /// <param name="Target">The target byte array to copy to</param>
    public static void Clone(byte[] Source, byte[] Target)
    {
        Array.Copy(Source, Target, Source.Length);
    }

    /// <summary>
    /// Returns part of a byte array
    /// </summary>
    /// <param name="Source">The source byte array</param>
    /// <param name="StartIndex">The index to start from in the source array</param>
    /// <exception cref="System.ArgumentOutOfRangeException">Length is less than zero.</exception>
    /// <remarks>This creates a new array and copies a section of bytes from the source array into the new array</remarks>
    /// <returns>The slice of the source array</returns>
    public static byte[] Slice(byte[] Source, int StartIndex)
    {
        byte[] result = new byte[Source.Length - StartIndex];
        Array.Copy(Source, StartIndex, result, 0, Source.Length - StartIndex);
        return result;
    }

    /// <summary>
    /// Returns part of a byte array
    /// </summary>
    /// <param name="Source">The source byte array</param>
    /// <param name="StartIndex">The index to start from in the source array</param>
    /// <param name="Length">The amount of bytes to slice from the source array </param>
    /// <exception cref="System.ArgumentOutOfRangeException">Length is less than zero.</exception>
    /// <remarks>This creates a new array and copies a section of bytes from the source array into the new array</remarks>
    /// <returns>The slice of the source array</returns>
    public static byte[] Slice(byte[] Source, int StartIndex, int Length)
    {
        byte[] result = new byte[Length];
        Array.Copy(Source, StartIndex, result, 0, Length);
        return result;
    }

    /// <summary>
    /// Converts a byte array to a string in the <see cref="Encoding.Default"/> encoding
    /// </summary>
    /// <param name="Data">The byte array to convert</param>
    /// <returns>The string represented by the bytes</returns>
    [Obsolete("This functionality is already provided by Encoding.Default.GetString")]
    public static string ToString(byte[] Data)
    {
        return ToString(Data, Encoding.Default);
    }

    /// <summary>
    /// Converts a byte array to string in a specific encoding
    /// </summary>
    /// <param name="Data">The byte array to convert</param>
    /// <param name="Encoding">The encoding to use in the conversion</param>
    /// <returns>The string represented by the bytes</returns>
    [Obsolete("This functionality is already provided by Encoding.GetString")]
    public static string ToString(byte[] Data, Encoding Encoding)
    {
        return Encoding.GetString(Data);
    }

    /// <summary>
    /// Transforms a set of bytes from one format to another
    /// </summary>
    /// <param name="Data">The bytes to transform</param>
    /// <param name="Transform">An array of transforms to perform on the bytes</param>
    /// <returns>The transformed array of bytes</returns>
    public static byte[] Transform(byte[] Data, params DataTransformType[] Transform)
    {
        using (MemoryStream from = new MemoryStream(Data))
        {
            using (MemoryStream to = new MemoryStream())
            {
                StreamUtils.CopyStream(from, to, Transform);
                to.Position = 0;
                return to.ToArray();
            }
        }
    }

    /// <summary>
    /// Performs transforms on file while reading it in
    /// </summary>
    /// <param name="FilePath">The full path to the file to read in</param>
    /// <param name="Transform">An array of transforms to apply</param>
    /// <returns>A byte array after the transforms have been applied</returns>
    public static byte[] Load(string FilePath, params DataTransformType[] Transform)
    {
        using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
        {
            using (MemoryStream ms = new MemoryStream())
            {
                StreamUtils.CopyStream(fs, ms, Transform);
                ms.Position = 0;
                return ms.ToArray();
            }
        }
    }

    /// <summary>
    /// Performs transforms on a byte array while saving it to a file
    /// </summary>
    /// <param name="FilePath">The full path to the file to save to</param>
    /// <param name="Data">The byte array to save</param>
    /// <param name="Transform">An array of transforms to apply to the data as it is being saved</param>
    /// <remarks>If the file already exists, it will be overwritten</remarks>
    public static void Save(string FilePath, byte[] Data, params DataTransformType[] Transform)
    {
        using (FileStream fs = new FileStream(FilePath, FileMode.Create))
        {
            using (MemoryStream ms = new MemoryStream(Data))
            {
                StreamUtils.CopyStream(ms, fs, Transform);
            }
        }
    }

    /// <summary>
    /// Creates a new random byte array.
    /// </summary>
    /// <param name="size">The size of the array to be returned.</param>
    /// <returns>A new random byte array.</returns>
    public static byte[] Random(int size)
    {
        byte[] bytes = new byte[size];
        new Random().NextBytes(bytes);
        return bytes;
    }

    public static byte CalcParityNibble(byte[] data, bool includeLastNibble)
    {
        int value = 0;
        int to = (data.Length * 2);

        if (!includeLastNibble)
            to--;

        for (int counter = 0; counter < to; counter++)
        {
            byte b = data[counter / 2];
            if ((counter % 2) == 0)
            {
                value ^= (b & 0xF0) >> 4;
            }
            else
            {
                value ^= (b & 0xF);
            }
        }
        return (byte)value;
    }

    public static byte CalcParityNibble(byte[] data)
    {
        return CalcParityNibble(data, false);
    }

    public static void SetParityNibble(byte[] data)
    {
        byte parity = CalcParityNibble(data);
        int last = data.Length - 1;
        data[last] = (byte)((data[last] & 0xF0) | parity);
    }

    public static bool TestParityNibble(byte[] data)
    {
        byte parity = CalcParityNibble(data);
        int last = data.Length - 1;
        return (data[last] & 0xF) == parity;
    }

    public static byte CalcParityByte(byte[] data, bool includeLastByte)
    {
        int value = 0;
        int to = data.Length;
        if (!includeLastByte)
            to--;
        for (int counter = 0; counter < to; counter++)
        {
            value ^= data[counter];
        }
        return (byte)value;
    }

    public static byte CalcParityByte(byte[] data)
    {
        return CalcParityNibble(data, false);
    }

    public static void SetParityByte(byte[] data)
    {
        byte parity = CalcParityByte(data);
        int last = data.Length - 1;
        data[last] = parity;
    }

    public static bool TestParityByte(byte[] data)
    {
        byte parity = CalcParityByte(data);
        int last = data.Length - 1;
        return data[last] == parity;
    }

    public static int ByteArrayToInt(byte[] data)
    {
        if (data.Length != 4)
            throw new ArgumentException("Data length must be 4 bytes.", nameof(data));
        return ((data[0] << 24) | (data[1] << 16) | (data[2] << 8) | (data[3]));
    }
    public static byte[] IntToByteArray(int data)
    {
        return [(byte)(data >> 24), (byte)(data >> 16), (byte)(data >> 8), (byte)data];
    }

    /// <summary>
    /// Computes a 32-bit hash code from a byte array, suitable for Object::GetHashCode().
    /// </summary>
    public static int ByteArrayToHashCode(byte[] data)
    {
        if (data.Length % 4 != 0)
            throw new ArgumentException("Data length must be a multiple of 4 bytes.", nameof(data));

        int h = 0;

        for (int i = 0; i < data.Length; i += 4)
        {
            h ^= (data[i + 0] << 24) | (data[i + 1] << 16) | (data[i + 2] << 8) | (data[i + 3] << 0);
        }

        return h;
    }

    public static bool GetBit(byte[] data, int position)
    {
        int index = position / 8;
        int shift = 7 - (position % 8);
        return ((data[index] >> shift) & 1) == 1;
    }
    public static void SetBit(byte[] data, int position, bool value)
    {
        int index = position / 8;
        int shift = 7 - (position % 8);
        if (value)
        {
            data[index] = (byte)(data[index] | (1 << shift));
        }
        else
        {
            data[index] = (byte)(data[index] & ~(1 << shift));
        }
    }

    public static void PackSixBytesExNibble(byte[] data, int twentyFiveBits, int nineteenBits)
    {

        if (data.Length < 6)
            throw new ArgumentException("Data argument must be at least 6 bytes long", nameof(data));

        byte[] sourceTwentyFiveBits = IntToByteArray(twentyFiveBits);
        byte[] sourceNineteenBits = IntToByteArray(nineteenBits);

        SetBit(data, 0, GetBit(sourceTwentyFiveBits, 31));
        SetBit(data, 42, GetBit(sourceNineteenBits, 13));
        SetBit(data, 2, GetBit(sourceTwentyFiveBits, 30));
        SetBit(data, 40, GetBit(sourceTwentyFiveBits, 29));
        SetBit(data, 4, GetBit(sourceNineteenBits, 14));
        SetBit(data, 38, GetBit(sourceTwentyFiveBits, 28));
        SetBit(data, 6, GetBit(sourceNineteenBits, 15));
        SetBit(data, 36, GetBit(sourceTwentyFiveBits, 27));
        SetBit(data, 8, GetBit(sourceNineteenBits, 16));
        SetBit(data, 34, GetBit(sourceTwentyFiveBits, 26));
        SetBit(data, 10, GetBit(sourceNineteenBits, 17));
        SetBit(data, 32, GetBit(sourceTwentyFiveBits, 25));
        SetBit(data, 12, GetBit(sourceTwentyFiveBits, 24));
        SetBit(data, 30, GetBit(sourceNineteenBits, 18));
        SetBit(data, 14, GetBit(sourceTwentyFiveBits, 23));
        SetBit(data, 28, GetBit(sourceTwentyFiveBits, 22));
        SetBit(data, 16, GetBit(sourceNineteenBits, 19));
        SetBit(data, 26, GetBit(sourceTwentyFiveBits, 21));
        SetBit(data, 18, GetBit(sourceNineteenBits, 20));
        SetBit(data, 24, GetBit(sourceTwentyFiveBits, 20));
        SetBit(data, 20, GetBit(sourceNineteenBits, 21));
        SetBit(data, 22, GetBit(sourceTwentyFiveBits, 19));
        SetBit(data, 21, GetBit(sourceNineteenBits, 22));
        SetBit(data, 23, GetBit(sourceTwentyFiveBits, 18));
        SetBit(data, 19, GetBit(sourceNineteenBits, 23));
        SetBit(data, 25, GetBit(sourceTwentyFiveBits, 17));
        SetBit(data, 17, GetBit(sourceTwentyFiveBits, 16));
        SetBit(data, 27, GetBit(sourceNineteenBits, 24));
        SetBit(data, 15, GetBit(sourceTwentyFiveBits, 15));
        SetBit(data, 29, GetBit(sourceNineteenBits, 25));
        SetBit(data, 13, GetBit(sourceTwentyFiveBits, 14));
        SetBit(data, 31, GetBit(sourceNineteenBits, 26));
        SetBit(data, 11, GetBit(sourceTwentyFiveBits, 13));
        SetBit(data, 33, GetBit(sourceNineteenBits, 27));
        SetBit(data, 9, GetBit(sourceTwentyFiveBits, 12));
        SetBit(data, 35, GetBit(sourceNineteenBits, 28));
        SetBit(data, 7, GetBit(sourceTwentyFiveBits, 11));
        SetBit(data, 37, GetBit(sourceNineteenBits, 29));
        SetBit(data, 5, GetBit(sourceTwentyFiveBits, 10));
        SetBit(data, 39, GetBit(sourceNineteenBits, 30));
        SetBit(data, 3, GetBit(sourceTwentyFiveBits, 9));
        SetBit(data, 41, GetBit(sourceTwentyFiveBits, 8));
        SetBit(data, 1, GetBit(sourceNineteenBits, 31));
        SetBit(data, 43, GetBit(sourceTwentyFiveBits, 7));

    }
    public static void UnPackSixBytesExNibble(byte[] data, out int twentyFiveBits, out int nineteenBits)
    {

        byte[] targetTwentyFiveBits = new byte[4];
        byte[] targetNineteenBits = new byte[4];

        SetBit(targetTwentyFiveBits, 31, GetBit(data, 0));
        SetBit(targetNineteenBits, 13, GetBit(data, 42));
        SetBit(targetTwentyFiveBits, 30, GetBit(data, 2));
        SetBit(targetTwentyFiveBits, 29, GetBit(data, 40));
        SetBit(targetNineteenBits, 14, GetBit(data, 4));
        SetBit(targetTwentyFiveBits, 28, GetBit(data, 38));
        SetBit(targetNineteenBits, 15, GetBit(data, 6));
        SetBit(targetTwentyFiveBits, 27, GetBit(data, 36));
        SetBit(targetNineteenBits, 16, GetBit(data, 8));
        SetBit(targetTwentyFiveBits, 26, GetBit(data, 34));
        SetBit(targetNineteenBits, 17, GetBit(data, 10));
        SetBit(targetTwentyFiveBits, 25, GetBit(data, 32));
        SetBit(targetTwentyFiveBits, 24, GetBit(data, 12));
        SetBit(targetNineteenBits, 18, GetBit(data, 30));
        SetBit(targetTwentyFiveBits, 23, GetBit(data, 14));
        SetBit(targetTwentyFiveBits, 22, GetBit(data, 28));
        SetBit(targetNineteenBits, 19, GetBit(data, 16));
        SetBit(targetTwentyFiveBits, 21, GetBit(data, 26));
        SetBit(targetNineteenBits, 20, GetBit(data, 18));
        SetBit(targetTwentyFiveBits, 20, GetBit(data, 24));
        SetBit(targetNineteenBits, 21, GetBit(data, 20));
        SetBit(targetTwentyFiveBits, 19, GetBit(data, 22));
        SetBit(targetNineteenBits, 22, GetBit(data, 21));
        SetBit(targetTwentyFiveBits, 18, GetBit(data, 23));
        SetBit(targetNineteenBits, 23, GetBit(data, 19));
        SetBit(targetTwentyFiveBits, 17, GetBit(data, 25));
        SetBit(targetTwentyFiveBits, 16, GetBit(data, 17));
        SetBit(targetNineteenBits, 24, GetBit(data, 27));
        SetBit(targetTwentyFiveBits, 15, GetBit(data, 15));
        SetBit(targetNineteenBits, 25, GetBit(data, 29));
        SetBit(targetTwentyFiveBits, 14, GetBit(data, 13));
        SetBit(targetNineteenBits, 26, GetBit(data, 31));
        SetBit(targetTwentyFiveBits, 13, GetBit(data, 11));
        SetBit(targetNineteenBits, 27, GetBit(data, 33));
        SetBit(targetTwentyFiveBits, 12, GetBit(data, 9));
        SetBit(targetNineteenBits, 28, GetBit(data, 35));
        SetBit(targetTwentyFiveBits, 11, GetBit(data, 7));
        SetBit(targetNineteenBits, 29, GetBit(data, 37));
        SetBit(targetTwentyFiveBits, 10, GetBit(data, 5));
        SetBit(targetNineteenBits, 30, GetBit(data, 39));
        SetBit(targetTwentyFiveBits, 9, GetBit(data, 3));
        SetBit(targetTwentyFiveBits, 8, GetBit(data, 41));
        SetBit(targetNineteenBits, 31, GetBit(data, 1));
        SetBit(targetTwentyFiveBits, 7, GetBit(data, 43));

        twentyFiveBits = ByteArrayToInt(targetTwentyFiveBits);
        nineteenBits = ByteArrayToInt(targetNineteenBits);

    }

    public static void PackFourBytes(byte[] data, int thirtyTwoBits)
    {

        if (data.Length < 4)
            throw new ArgumentException("Data argument must be at least 4 bytes long", nameof(data));

        byte[] sourceThirtyTwoBits = IntToByteArray(thirtyTwoBits);

        SetBit(data, 0, GetBit(sourceThirtyTwoBits, 31));
        SetBit(data, 1, GetBit(sourceThirtyTwoBits, 15));
        SetBit(data, 2, GetBit(sourceThirtyTwoBits, 29));
        SetBit(data, 3, GetBit(sourceThirtyTwoBits, 13));
        SetBit(data, 4, GetBit(sourceThirtyTwoBits, 27));
        SetBit(data, 5, GetBit(sourceThirtyTwoBits, 11));
        SetBit(data, 6, GetBit(sourceThirtyTwoBits, 25));
        SetBit(data, 7, GetBit(sourceThirtyTwoBits, 9));
        SetBit(data, 8, GetBit(sourceThirtyTwoBits, 23));
        SetBit(data, 9, GetBit(sourceThirtyTwoBits, 7));
        SetBit(data, 10, GetBit(sourceThirtyTwoBits, 21));
        SetBit(data, 11, GetBit(sourceThirtyTwoBits, 5));
        SetBit(data, 12, GetBit(sourceThirtyTwoBits, 19));
        SetBit(data, 13, GetBit(sourceThirtyTwoBits, 3));
        SetBit(data, 14, GetBit(sourceThirtyTwoBits, 17));
        SetBit(data, 15, GetBit(sourceThirtyTwoBits, 1));
        SetBit(data, 16, GetBit(sourceThirtyTwoBits, 0));
        SetBit(data, 17, GetBit(sourceThirtyTwoBits, 16));
        SetBit(data, 18, GetBit(sourceThirtyTwoBits, 2));
        SetBit(data, 19, GetBit(sourceThirtyTwoBits, 18));
        SetBit(data, 20, GetBit(sourceThirtyTwoBits, 4));
        SetBit(data, 21, GetBit(sourceThirtyTwoBits, 20));
        SetBit(data, 22, GetBit(sourceThirtyTwoBits, 6));
        SetBit(data, 23, GetBit(sourceThirtyTwoBits, 22));
        SetBit(data, 24, GetBit(sourceThirtyTwoBits, 8));
        SetBit(data, 25, GetBit(sourceThirtyTwoBits, 24));
        SetBit(data, 26, GetBit(sourceThirtyTwoBits, 10));
        SetBit(data, 27, GetBit(sourceThirtyTwoBits, 26));
        SetBit(data, 28, GetBit(sourceThirtyTwoBits, 12));
        SetBit(data, 29, GetBit(sourceThirtyTwoBits, 28));
        SetBit(data, 30, GetBit(sourceThirtyTwoBits, 14));
        SetBit(data, 31, GetBit(sourceThirtyTwoBits, 30));
    }

    public static void UnPackFourBytes(byte[] data, out int thirtyTwoBits)
    {
        byte[] targetThirtyTwoBits = new byte[4];

        SetBit(targetThirtyTwoBits, 31, GetBit(data, 0));
        SetBit(targetThirtyTwoBits, 15, GetBit(data, 1));
        SetBit(targetThirtyTwoBits, 29, GetBit(data, 2));
        SetBit(targetThirtyTwoBits, 13, GetBit(data, 3));
        SetBit(targetThirtyTwoBits, 27, GetBit(data, 4));
        SetBit(targetThirtyTwoBits, 11, GetBit(data, 5));
        SetBit(targetThirtyTwoBits, 25, GetBit(data, 6));
        SetBit(targetThirtyTwoBits, 9, GetBit(data, 7));
        SetBit(targetThirtyTwoBits, 23, GetBit(data, 8));
        SetBit(targetThirtyTwoBits, 7, GetBit(data, 9));
        SetBit(targetThirtyTwoBits, 21, GetBit(data, 10));
        SetBit(targetThirtyTwoBits, 5, GetBit(data, 11));
        SetBit(targetThirtyTwoBits, 19, GetBit(data, 12));
        SetBit(targetThirtyTwoBits, 3, GetBit(data, 13));
        SetBit(targetThirtyTwoBits, 17, GetBit(data, 14));
        SetBit(targetThirtyTwoBits, 1, GetBit(data, 15));
        SetBit(targetThirtyTwoBits, 0, GetBit(data, 16));
        SetBit(targetThirtyTwoBits, 16, GetBit(data, 17));
        SetBit(targetThirtyTwoBits, 2, GetBit(data, 18));
        SetBit(targetThirtyTwoBits, 18, GetBit(data, 19));
        SetBit(targetThirtyTwoBits, 4, GetBit(data, 20));
        SetBit(targetThirtyTwoBits, 20, GetBit(data, 21));
        SetBit(targetThirtyTwoBits, 6, GetBit(data, 22));
        SetBit(targetThirtyTwoBits, 22, GetBit(data, 23));
        SetBit(targetThirtyTwoBits, 8, GetBit(data, 24));
        SetBit(targetThirtyTwoBits, 24, GetBit(data, 25));
        SetBit(targetThirtyTwoBits, 10, GetBit(data, 26));
        SetBit(targetThirtyTwoBits, 26, GetBit(data, 27));
        SetBit(targetThirtyTwoBits, 12, GetBit(data, 28));
        SetBit(targetThirtyTwoBits, 28, GetBit(data, 29));
        SetBit(targetThirtyTwoBits, 14, GetBit(data, 30));
        SetBit(targetThirtyTwoBits, 30, GetBit(data, 31));

        thirtyTwoBits = ByteArrayToInt(targetThirtyTwoBits);

    }

    public static string EncryptTenCharacterReference(int twentyFiveBits, int nineteenBits, string password)
    {

        if ((twentyFiveBits < 0) || (twentyFiveBits > 33554431))
            throw new ArgumentException("Valid range is between 0 and 33,554,431", nameof(twentyFiveBits));

        if ((nineteenBits < 0) || (nineteenBits > 524287))
            throw new ArgumentException("Valid range is between 0 and 524,287", "ninteenBits");

        byte[] data = new byte[6];

        //pack the data
        PackSixBytesExNibble(data, twentyFiveBits, nineteenBits);

        //set the parity nibble
        SetParityNibble(data);

        //encrypt with password
        data = EncryptionUtils.RC4Encrypt(data, password);

        //convert to base 32
        data = ByteArrayUtils.Transform(data, DataTransformType.ToBase32Lenient);

        //convert to string
        string result = Encoding.Default.GetString(data);

        //place a dash in the middle
        result = result.Insert(5, "-");

        return result;

    }
    public static bool DecryptTenCharacterReference(string reference, string password, out int twentyFiveBits, out int nineteenBits)
    {

        twentyFiveBits = 0;
        nineteenBits = 0;

        //strip the dashes & clean the text
        reference = reference.Trim().Replace("-", String.Empty);

        if (reference.Length != 10)
            return false;

        //get the string data
        byte[] data = Encoding.Default.GetBytes(reference);

        //convert from base 32
        data = ByteArrayUtils.Transform(data, DataTransformType.FromBase32Lenient);

        //decrypt using password
        data = EncryptionUtils.RC4Decrypt(data, password);

        //test the length, and parity nibble
        if ((data.Length != 6) || (!TestParityNibble(data)))
            return false;

        //unpack the data
        UnPackSixBytesExNibble(data, out twentyFiveBits, out nineteenBits);

        return true;

    }

    public static string EncryptEightCharacterReference(int thirtyTwoBits, string password)
    {

        byte[] data = new byte[5];

        //pack the data
        PackFourBytes(data, thirtyTwoBits);

        //set the parity nibble
        SetParityByte(data);

        //encrypt with password
        data = EncryptionUtils.RC4Encrypt(data, password);

        //convert to base 32
        data = ByteArrayUtils.Transform(data, DataTransformType.ToBase32Lenient);

        //convert to string
        string result = Encoding.Default.GetString(data);

        //place a dash in the middle
        result = result.Insert(4, "-");

        return result;

    }

    public static bool DecryptEightCharacterReference(string reference, string password, out int thirtyTwoBits)
    {
        thirtyTwoBits = 0;

        //strip the dashes & clean the text
        reference = reference.Trim().Replace("-", String.Empty);

        if (reference.Length != 8)
            return false;

        //get the string data
        byte[] data = Encoding.Default.GetBytes(reference);

        //convert from base 32
        data = ByteArrayUtils.Transform(data, DataTransformType.FromBase32Lenient);

        //decrypt using password
        data = EncryptionUtils.RC4Decrypt(data, password);

        //test the length, and parity nibble
        if ((data.Length != 5) || (!TestParityByte(data)))
            return false;

        //unpack the data
        UnPackFourBytes(data, out thirtyTwoBits);

        return true;

    }
}