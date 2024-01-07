using Landmark.ER301Driver.Extensions;

namespace Landmark.ER301Driver.Models;

public class NFCDeviceResponseModel : NFCResponseModel
{
    public string DeviceName { get; set; }

    protected NFCDeviceResponseModel(byte[] readBuffer)
        : base(readBuffer)
    {
        DeviceName = readBuffer.GetDeviceName(Length);
    }

    public override string ToString() => $"{{Command={Command}, NodeId={NodeId}, ResponseCode={Response}, DeviceName={DeviceName}}}";

    public static new NFCDeviceResponseModel GetFromBuffer(byte[] readBuffer) => new(readBuffer);
}
