using Diz.LanguageExtensions;
using EFT.InventoryLogic;
using MergeConsumables.Results;
using UnityEngine;

namespace MergeConsumables;

public static class ItemManipulatorExtensions
{
    public static OperationResult<MergeMedsResult> MergeMeds(Meds item, Meds targetItem, float count, ItemController itemController, bool simulate)
    {
        if (item.TemplateId != targetItem.TemplateId)
        {
            return new StringError("Not same item");
        }

        if (item.Id == targetItem.Id)
        {
            return new StringError("Same item?");
        }

        if (targetItem.MedKitComponent.HpResource >= targetItem.MedKitComponent.MaxHpResource)
        {
            return new StringError("Already max");
        }

        var rootComponent = item.MedKitComponent;
        var targetComponent = targetItem.MedKitComponent;

        var originalRootHp = rootComponent.HpResource;
        var originalTargetHp = targetComponent.HpResource;

        var maxTransferable = Mathf.Min(targetComponent.MaxHpResource - targetComponent.HpResource, rootComponent.HpResource);
        var transferAmount = count > 0 ? Mathf.Min(count, maxTransferable) : maxTransferable;

        rootComponent.HpResource -= transferAmount;
        targetComponent.HpResource += transferAmount;

        OperationResult<DiscardResult> discard = default;
#if DEBUG
        MC_Plugin.MC_Logger.LogInfo($"Resource has {rootComponent.HpResource} units left"); 
#endif
        if (rootComponent.HpResource < 1f)
        {
#if DEBUG
            MC_Plugin.MC_Logger.LogInfo("Destroying component due to less than 0"); 
#endif
            discard = ItemManipulator.Discard(item, itemController, false);
            if (!discard.Succeeded)
            {
                rootComponent.HpResource = originalRootHp;
                targetComponent.HpResource = originalTargetHp;

                MC_Plugin.MC_Logger.LogError(discard.Error);
                return discard.Error;
            }
        }

        if (simulate)
        {
            discard.Value?.RollBack();

            rootComponent.HpResource = originalRootHp;
            targetComponent.HpResource = originalTargetHp;
        }

        return new MergeMedsResult(item, item.CurrentAddress, targetItem, transferAmount, discard, itemController);
    }

    public static OperationResult<MergeFoodResult> MergeFood(FoodDrink item, FoodDrink targetItem, float count, ItemController itemController, bool simulate)
    {
        if (item.TemplateId != targetItem.TemplateId)
        {
            return new StringError("Not same item");
        }

        if (item.Id == targetItem.Id)
        {
            return new StringError("Same item?");
        }

        if (targetItem.FoodDrinkComponent.HpPercent >= targetItem.FoodDrinkComponent.MaxResource)
        {
            return new StringError("Already max");
        }

        var rootComponent = item.FoodDrinkComponent;
        var targetComponent = targetItem.FoodDrinkComponent;

        var originalRootHp = rootComponent.HpPercent;
        var originalTargetHp = targetComponent.HpPercent;

        var maxTransferable = Mathf.Min(targetComponent.MaxResource - targetComponent.HpPercent, rootComponent.HpPercent);
        var transferAmount = count > 0 ? Mathf.Min(count, maxTransferable) : maxTransferable;

        rootComponent.HpPercent -= transferAmount;
        targetComponent.HpPercent += transferAmount;

        OperationResult<DiscardResult> discard = default;
        if (rootComponent.HpPercent < 1f)
        {
            discard = ItemManipulator.Discard(item, itemController, false);
            if (!discard.Succeeded)
            {
                rootComponent.HpPercent = originalRootHp;
                targetComponent.HpPercent = originalTargetHp;

                MC_Plugin.MC_Logger.LogError(discard.Error);
                return discard.Error;
            }
        }

        if (simulate)
        {
            discard.Value?.RollBack();

            rootComponent.HpPercent = originalRootHp;
            targetComponent.HpPercent = originalTargetHp;
        }

        return new MergeFoodResult(item, item.CurrentAddress, targetItem, transferAmount, discard, itemController);
    }
}
