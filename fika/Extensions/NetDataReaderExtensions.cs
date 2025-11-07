using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib.Utils;
using MergeConsumables.Descriptors;

namespace MergeConsumablesFika.Extensions;

public static class NetDataReaderExtensions
{
    public static MergeFoodDescriptor ReadMergeFoodsDescriptor(this NetDataReader reader)
    {
        return new()
        {
            OperationId = reader.GetUShort(),
            OwnerId = reader.GetMongoID(),
            SourceItem = reader.GetString(),
            TargetItem = reader.GetString(),
            Count = reader.GetFloat()
        };
    }

    public static MergeMedsDescriptor ReadMergeMedsDescriptor(this NetDataReader reader)
    {
        return new()
        {
            OperationId = reader.GetUShort(),
            OwnerId = reader.GetMongoID(),
            SourceItem = reader.GetString(),
            TargetItem = reader.GetString(),
            Count = reader.GetFloat()
        };
    }
}
