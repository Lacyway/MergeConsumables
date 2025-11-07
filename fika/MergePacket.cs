using Fika.Core.Networking.LiteNetLib.Utils;
using MergeConsumables.Descriptors;
using MergeConsumables.Operations;
using MergeConsumablesFika.Extensions;

namespace MergeConsumablesFika;

public struct MergePacket : INetSerializable
{
    public int NetId;
    public ushort CallbackId;
    public byte Type;

    public MergeFoodOperation MergeFoodOperation;
    public MergeMedsOperation MergeMedsOperation;

    public MergeFoodDescriptor MergeFoodDescriptor;
    public MergeMedsDescriptor MergeMedsDescriptor;

    public void Deserialize(NetDataReader reader)
    {
        NetId = reader.GetInt();
        CallbackId = reader.GetUShort();
        Type = reader.GetByte();
        if (Type == 0)
        {
            MergeFoodDescriptor = reader.ReadMergeFoodsDescriptor();
        }
        else
        {
            MergeMedsDescriptor = reader.ReadMergeMedsDescriptor();
        }
    }

    public readonly void Serialize(NetDataWriter writer)
    {
        writer.Put(NetId);
        writer.Put(CallbackId);
        writer.Put(Type);
        if (Type == 0)
        {
            var descriptor = (MergeFoodDescriptor)MergeFoodOperation.ToDescriptor();
            writer.WriteMergeFoodsDescriptor(descriptor);
        }
        else
        {
            var descriptor = (MergeMedsDescriptor)MergeMedsOperation.ToDescriptor();
            writer.WriteMergeMedsDescriptor(descriptor);
        }
    }
}
