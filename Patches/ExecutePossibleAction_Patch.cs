using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace MergeConsumables.Patches;

public class ExecutePossibleAction_Patch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(TraderControllerClass), nameof(TraderControllerClass.ExecutePossibleAction),
            [typeof(ItemContextAbstractClass), typeof(Item), typeof(bool), typeof(bool)]);
    }

    [PatchPrefix]
    public static bool Prefix(TraderControllerClass __instance, ItemContextAbstractClass itemContext, Item targetItem, bool simulate, ref GStruct153 __result)
    {
        if (itemContext.Item is MedsItemClass rootMedItem
            && rootMedItem.MedKitComponent != null
            && targetItem is MedsItemClass targetMedItem
            && targetMedItem.MedKitComponent != null)
        {
            __result = InteractionsHandlerClassExtensions.MergeMeds(rootMedItem, targetMedItem, 0, __instance, simulate);
            return false;
        }

        if (itemContext.Item is FoodDrinkItemClass rootFoodItem
            && rootFoodItem.FoodDrinkComponent != null
            && targetItem is FoodDrinkItemClass targetFoodItem
            && targetFoodItem.FoodDrinkComponent != null)
        {
            __result = InteractionsHandlerClassExtensions.MergeFood(rootFoodItem, targetFoodItem, 0, __instance, simulate);
            return false;
        }

        return true;
    }
}