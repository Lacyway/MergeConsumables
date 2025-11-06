using HarmonyLib;
using MergeConsumables.Extensions;
using SPT.Reflection.Patching;
using System.Linq;
using System.Reflection;

namespace MergeConsumables.Patches;

public class ReadPolymorph_Patch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(GClass3695)
            .GetMethod(nameof(GClass3695.ReadPolymorph));
    }

    [PatchPrefix]
    public static bool Prefix(EFTReaderClass reader, ref BaseDescriptorClass __result)
    {
        switch (reader.ReadByte())
        {
            case 132:
                __result = reader.ReadMergeFoodsDescriptor();
                return false;
            case 133:
                __result = reader.ReadMergeMedsDescriptor();
                return false;
        }

        reader.Position--; // not our operations, rewind
        return true;
    }
}
