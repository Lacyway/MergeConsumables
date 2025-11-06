using MergeConsumables.Descriptors;

namespace MergeConsumables.Extensions;

public static class EFTReaderClassExtensions
{
    public static MergeFoodDescriptor ReadMergeFoodsDescriptor(this EFTReaderClass reader)
    {
        return new()
        {
            OperationId = reader.ReadUShort(),
            OwnerId = reader.ReadMongoId(),
            SourceItem = reader.ReadString(),
            TargetItem = reader.ReadString(),
            Count = reader.ReadFloat()
        };
    }

    public static MergeMedsDescriptor ReadMergeMedsDescriptor(this EFTReaderClass reader)
    {
        return new()
        {
            OperationId = reader.ReadUShort(),
            OwnerId = reader.ReadMongoId(),
            SourceItem = reader.ReadString(),
            TargetItem = reader.ReadString(),
            Count = reader.ReadFloat()
        };
    }
}
