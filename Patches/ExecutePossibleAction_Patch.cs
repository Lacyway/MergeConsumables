using Diz.LanguageExtensions;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace MergeConsumables.Patches;

public sealed class ExecutePossibleAction_Patch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return AccessTools.Method(typeof(ItemController), nameof(ItemController.ExecutePossibleAction),
            [typeof(ItemContext), typeof(Item), typeof(bool), typeof(bool)]);
    }

    [PatchPrefix]
    public static bool Prefix(ItemController __instance, ItemContext itemContext, Item targetItem, bool simulate, ref OperationResult __result)
    {
        if (itemContext.Item is Meds rootMedItem
            && rootMedItem.MedKitComponent != null
            && targetItem is Meds targetMedItem
            && targetMedItem.MedKitComponent != null)
        {
            __result = ItemManipulatorExtensions.MergeMeds(rootMedItem, targetMedItem, 0, __instance, simulate);
            return false;
        }

        if (itemContext.Item is Food rootFoodItem
            && rootFoodItem.FoodDrinkComponent != null
            && targetItem is Food targetFoodItem
            && targetFoodItem.FoodDrinkComponent != null)
        {
            __result = ItemManipulatorExtensions.MergeFood(rootFoodItem, targetFoodItem, 0, __instance, simulate);
            return false;
        }

        return true;
    }
}