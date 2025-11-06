using HarmonyLib;
using MergeConsumables.Descriptors;
using MergeConsumables.Extensions;
using SPT.Reflection.Patching;
using System.Reflection;

namespace MergeConsumables.Patches;

[HarmonyPatch]
public static class WritePolymorphPatch
{
    // Target method: GClass3695.WritePolymorph<BaseDescriptorClass>
    static MethodBase TargetMethod()
    {
        var openMethod = typeof(GClass3695)
            .GetMethod(nameof(GClass3695.WritePolymorph),
                       BindingFlags.Public | BindingFlags.Static);

        // Construct generic method for BaseDescriptorClass
        return openMethod.MakeGenericMethod(typeof(BaseDescriptorClass));
    }

    // Prefix
    static bool Prefix(EFTWriterClass writer, BaseDescriptorClass target)
    {
        if (target is MergeFoodDescriptor mergeFoodDescriptor)
        {
            var num = GClass3695.List_0.IndexOf(target.GetType());
            writer.WriteByte((byte)num);
            writer.WriteMergeFoodsDescriptor(mergeFoodDescriptor);
            return false; // skip original
        }

        if (target is MergeMedsDescriptor mergeMedsDescriptor)
        {
            var num = GClass3695.List_0.IndexOf(target.GetType());
            writer.WriteByte((byte)num);
            writer.WriteMergeMedsDescriptor(mergeMedsDescriptor);
            return false; // skip original
        }

        return true; // let original run
    }
}