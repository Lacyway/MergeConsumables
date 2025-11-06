using MergeConsumables.Operations;
using MergeConsumables.Results;
using SPT.Reflection.Patching;
using System.Reflection;

namespace MergeConsumables.Patches;

public class ConvertOperationResultToOperation_Patch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(TraderControllerClass)
            .GetMethod(nameof(TraderControllerClass.ConvertOperationResultToOperation));
    }

    [PatchPrefix]
    public static bool Prefix(TraderControllerClass __instance, IRaiseEvents operationResult, ref BaseInventoryOperationClass __result)
    {
        if (operationResult is MergeFoodResult mergeFoodResult)
        {
            __result = new MergeFoodOperation(__instance.method_12(), __instance, mergeFoodResult);
            return false;
        }

        if (operationResult is MergeMedsResult mergeMedsResult)
        {
            __result = new MergeMedsOperation(__instance.method_12(), __instance, mergeMedsResult);
            return false;
        }

        return true;
    }
}