using Landmark.ER301Driver.Enums;
using Landmark.ER301Driver.Extensions;

namespace Landmark.ER301Driver.Models;

public class NFCResponseModel
{
    public ushort Length { get; set; }

    public ResponseReader Response { get; set; }

    public ushort NodeId { get; set; }

    public CommandReader Command { get; set; }

    protected NFCResponseModel(byte[] readBuffer)
    {
        Length = readBuffer.GetLength();
        NodeId = readBuffer.GetNodeId();
        Command = readBuffer.GetCommand();
        Response = (ResponseReader)readBuffer[8];
    }

    public override string ToString() => $"{{Command={Command}, NodeId={NodeId}, ResponseCode={Response}}}";

    public virtual byte GetCheckSum(byte[] readBuffer) => readBuffer[4 + Length];

    public static NFCResponseModel GetFromBuffer(byte[] readBuffer) => new(readBuffer);
}
