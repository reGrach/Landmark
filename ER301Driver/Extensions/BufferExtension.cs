using System.Text;
using Landmark.ER301Driver.Enums;

namespace Landmark.ER301Driver.Extensions
{
    public static class BufferExtension
    {
        public static ushort GetLength(this byte[] bytes) => bytes.Subset(2, 2).ToUInt16();
        public static ushort GetNodeId(this byte[] bytes) => bytes.Subset(4, 2).ToUInt16();
        public static CommandReader GetCommand(this byte[] bytes) => (CommandReader)bytes.Subset(6, 2).ToUInt16();
        public static ResponseReader GetResponseReader(this byte[] bytes) => (ResponseReader)bytes[8];
        public static byte[] GetSerialNo(this byte[] bytes) => bytes.Subset(9, 4);
        public static byte GetSAKByte(this byte[] bytes) => bytes[9];
        public static TagType GetNFCTagType(this byte[] bytes) => (TagType)BitConverter.ToUInt16(bytes.Subset(9, 2), 0);
        public static byte[] GetReaderData(this byte[] bytes) => bytes.Subset(9, 8);
        public static string GetDeviceName(this byte[] bytes, ushort len) => Encoding.ASCII.GetString(bytes.Subset(10, 7 + len));
    }
}