using System.Text;
using Landmark.ER301Driver.Extensions;

namespace Landmark.ER301Driver.Models;

public class AnticollisionResponse : NFCResponseModel
{
    public uint SerialNo { get; set; }

    public string SerialNoHex { get; set; }

    protected AnticollisionResponse(byte[] readBuffer)
        : base(readBuffer)
    {
        var serialBytes = readBuffer.GetSerialNo();

        SerialNo = BitConverter.ToUInt32(serialBytes, 0);
        SerialNoHex = serialBytes.ToHex(":");
    }

    public override string ToString() => $"{{Command={Command}, NodeId={NodeId}, ResponseCode={Response}, SerialNo={SerialNo}}}";

    public static new AnticollisionResponse GetFromBuffer(byte[] readBuffer) => new(readBuffer);

}
