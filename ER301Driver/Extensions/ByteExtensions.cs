namespace Landmark.ER301Driver.Extensions;

public static class ByteExtensions
{
    public static string ToHex(this byte value) =>
        "0x" + Convert.ToString(value, 16).PadLeft(2, '0');
}