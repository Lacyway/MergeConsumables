using System;
using BepInEx.Logging;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib.Utils;
using MergeConsumables.Descriptors;

namespace MergeConsumablesFika;

public static class MergeConsumablesSerialization
{
    public static void AddMergeConsumableTypes(ManualLogSource logger)
    {
        Register<MergeFoodDescriptor>(logger, PutMergeFoodsDescriptor, ReadMergeFoodsDescriptor);
        Register<MergeMedsDescriptor>(logger, PutMergeMedsDescriptor, ReadMergeMedsDescriptor);
    }

    private static void Register<T>(ManualLogSource logger, Action<NetDataWriter, T> serializer,
        Func<NetDataReader, T> deserializer) where T : class
    {
        var type = typeof(T);
        try
        {
            EFTSerializationExtensions.RegisterPolymorphicType(serializer, deserializer);
            logger.LogInfo($"Registered {type.Name}.");
        }
        catch (Exception ex)
        {

            logger.LogError($"Failed registering {type.Name}.");
        }
    }

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

    public static void PutMergeFoodsDescriptor(this NetDataWriter writer, MergeFoodDescriptor descriptor)
    {
        writer.Put(descriptor.OperationId);
        writer.PutMongoID(descriptor.OwnerId);
        writer.Put(descriptor.SourceItem);
        writer.Put(descriptor.TargetItem);
        writer.Put(descriptor.Count);
    }

    public static void PutMergeMedsDescriptor(this NetDataWriter writer, MergeMedsDescriptor descriptor)
    {
        writer.Put(descriptor.OperationId);
        writer.PutMongoID(descriptor.OwnerId);
        writer.Put(descriptor.SourceItem);
        writer.Put(descriptor.TargetItem);
        writer.Put(descriptor.Count);
    }
}
