using System.Reflection;
using EFT.InventoryLogic;
using EFT.InventoryLogic.Operations;
using MergeConsumables.Operations;
using MergeConsumables.Results;
using SPT.Reflection.Patching;

namespace MergeConsumables.Patches;

public sealed class ConvertOperationResultToOperation_Patch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(ItemController)
            .GetMethod(nameof(ItemController.ConvertOperationResultToOperation));
    }

    [PatchPrefix]
    public static bool Prefix(ItemController __instance, IOperationResult operationResult, ref AbstractOperation __result)
    {
        if (operationResult is MergeFoodResult mergeFoodResult)
        {
            __result = new MergeFoodOperation(__instance.GetAndIncrementNextOperationId(), __instance, mergeFoodResult);
            return false;
        }

        if (operationResult is MergeMedsResult mergeMedsResult)
        {
            __result = new MergeMedsOperation(__instance.GetAndIncrementNextOperationId(), __instance, mergeMedsResult);
            return false;
        }

        return true;
    }
}