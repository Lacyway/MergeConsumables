using MergeConsumables.Descriptors;

namespace MergeConsumables.Extensions;

public static class EFTWriterClassExtensions
{
    public static void WriteMergeFoodsDescriptor(this EFTWriterClass writer, MergeFoodDescriptor descriptor)
    {
        writer.WriteUShort(descriptor.OperationId);
        writer.WriteMongoId(descriptor.OwnerId);
        writer.WriteString(descriptor.SourceItem);
        writer.WriteString(descriptor.TargetItem);
        writer.WriteFloat(descriptor.Count);
    }

    public static void WriteMergeMedsDescriptor(this EFTWriterClass writer, MergeMedsDescriptor descriptor)
    {
        writer.WriteUShort(descriptor.OperationId);
        writer.WriteMongoId(descriptor.OwnerId);
        writer.WriteString(descriptor.SourceItem);
        writer.WriteString(descriptor.TargetItem);
        writer.WriteFloat(descriptor.Count);
    }
}
