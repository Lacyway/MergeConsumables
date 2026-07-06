using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib.Utils;
using MergeConsumables.Descriptors;

namespace MergeConsumablesFika;

public static class MergeConsumablesSerialization
{
    public static void AddMergeConsumableTypes(ManualLogSource logger)
    {
        logger.LogInfo("Adding MergeFoodsSerialization");
        try
        {
            InjectNewPolymorphicType<MergeFoodDescriptor>(PutMergeFoodsDescriptor, ReadMergeFoodsDescriptor);
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed injecting MergeFoods: {ex}");
        }

        logger.LogInfo("Adding MergeMedsSerialization");
        try
        {
            InjectNewPolymorphicType<MergeMedsDescriptor>(PutMergeMedsDescriptor, ReadMergeMedsDescriptor);
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed injecting MergeMeds: {ex}");
        }
    }

    private static void InjectNewPolymorphicType<T>(
        Action<NetDataWriter, T> serializer,
        Func<NetDataReader, object> deserializer) where T : class
    {
        var logger = MCF_Plugin.MC_Logger;
        var type = typeof(T);
        logger.LogInfo("Processing target type: " + type.Name);

        var targetClass = typeof(EFTSerializationExtensions);

        var indexToType = (List<Type>)targetClass.GetField("_indexToType", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        var typeToByte = (Dictionary<Type, byte>)targetClass.GetField("_typeToByte", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        var serializers = (Dictionary<Type, Action<NetDataWriter, object>>)targetClass.GetField("_serializers", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        var deserializers = (Dictionary<byte, Func<NetDataReader, object>>)targetClass.GetField("_deserializers", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

        var index = indexToType.IndexOf(type);
        if (index == -1)
        {
            throw new InvalidOperationException($"Type {type.Name} was not found inside GClass3695.List_0.");
        }

        var assignedId = (byte)index;
        logger.LogInfo($"Located type {type.Name} at index ID: {assignedId}");

        typeToByte[type] = assignedId;
        serializers[type] = (writer, target) => serializer(writer, (T)target);
        deserializers[assignedId] = deserializer;

        logger.LogInfo($"Successfully finalized dictionary injection for {type.Name}.");
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
