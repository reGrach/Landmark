using Landmark.ER301Driver.Enums;
using Landmark.ER301Driver.Extensions;

namespace Landmark.ER301Driver.Models;

public class NFCTypeResponseModel : NFCResponseModel
{
    public TagType NFCTagType { get; set; }

    protected NFCTypeResponseModel(byte[] readBuffer)
        : base(readBuffer)
    {
        NFCTagType = readBuffer.GetNFCTagType();
    }

    public override string ToString() => $"{{Command={Command}, NodeId={NodeId}, ResponseCode={Response}, NFCTagType={NFCTagType}}}";

    public static new NFCTypeResponseModel GetFromBuffer(byte[] readBuffer) => new(readBuffer);
}
