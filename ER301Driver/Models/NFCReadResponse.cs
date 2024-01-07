using Landmark.ER301Driver.Extensions;

namespace Landmark.ER301Driver.Models;

public class NFCReadResponse : NFCResponseModel
{
    public byte[] Data { get; set; }

    protected NFCReadResponse(byte[] readBuffer)
        : base(readBuffer)
    {
        Data = readBuffer.GetReaderData();
    }

    public override string ToString() => $"{{Command={Command}, NodeId={NodeId}, ResponseCode={Response}, Data={Data.ToHex()}}}";

    public static new NFCReadResponse GetFromBuffer(byte[] readBuffer) => new(readBuffer);

}
