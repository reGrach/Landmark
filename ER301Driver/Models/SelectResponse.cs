using Landmark.ER301Driver.Extensions;

namespace Landmark.ER301Driver.Models;

public class SelectResponse : NFCResponseModel
{
    // WARNING: There's something fishy about this SAK byte. According to
    // specifications it ought to hold the Select AcKnowledge byte which
    // helps to identy the card. However, with the ER301 reader, this byte
    // is always 9!?
    public byte SAKByte { get; set; }

    protected SelectResponse(byte[] readBuffer)
        : base(readBuffer)
    {
        SAKByte = readBuffer.GetSAKByte();
    }

    public override string ToString() => $"{{Command={Command}, NodeId={NodeId}, ResponseCode={Response}, SAK={SAKByte}}}";

    public static new SelectResponse GetFromBuffer(byte[] readBuffer) => new(readBuffer);
}