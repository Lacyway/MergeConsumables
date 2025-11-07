using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib.Utils;
using MergeConsumables.Descriptors;

namespace MergeConsumablesFika.Extensions;

public static class NetDataWriterExtensions
{
    public static void WriteMergeFoodsDescriptor(this NetDataWriter writer, MergeFoodDescriptor descriptor)
    {
        writer.Put(descriptor.OperationId);
        writer.PutMongoID(descriptor.OwnerId);
        writer.Put(descriptor.SourceItem);
        writer.Put(descriptor.TargetItem);
        writer.Put(descriptor.Count);
    }

    public static void WriteMergeMedsDescriptor(this NetDataWriter writer, MergeMedsDescriptor descriptor)
    {
        writer.Put(descriptor.OperationId);
        writer.PutMongoID(descriptor.OwnerId);
        writer.Put(descriptor.SourceItem);
        writer.Put(descriptor.TargetItem);
        writer.Put(descriptor.Count);
    }
}
