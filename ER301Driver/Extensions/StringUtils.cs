namespace Landmark.ER301Driver.Extensions;

public static class StringExtensions
{
    public static string LittleEndianToBigEndian(this string hexString)
    {
        char[] littleEndian = hexString.ToCharArray();
        char[] bigEndian = new char[32];

        for (int i = 0; i < littleEndian.Length; i += 2)
        {
            bigEndian[31 - i] = littleEndian[i + 1];
            bigEndian[30 - i] = littleEndian[i];
        }

        return new string(bigEndian);
    }
}
