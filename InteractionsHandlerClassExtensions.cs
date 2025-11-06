using Comfort.Common;
using EFT;
using UnityEngine;

namespace MergeConsumables;

public static class InteractionsHandlerClassExtensions
{
    public static GStruct154<MC_Meds_Operation> MergeMeds(MedsItemClass item, MedsItemClass targetItem, float count, TraderControllerClass itemController, bool simulate)
    {
        if (item.TemplateId != targetItem.TemplateId)
        {
            return new GClass1522("Not same item");
        }

        if (item.Id == targetItem.Id)
        {
            return new GClass1522("Same item?");
        }

        if (targetItem.MedKitComponent.HpResource >= targetItem.MedKitComponent.MaxHpResource)
        {
            return new GClass1522("Already max");
        }

        var rootComponent = item.MedKitComponent;
        var targetComponent = targetItem.MedKitComponent;

        var originalRootHp = rootComponent.HpResource;
        var originalTargetHp = targetComponent.HpResource;

        var maxTransferable = Mathf.Min(targetComponent.MaxHpResource - targetComponent.HpResource, rootComponent.HpResource);
        var transferAmount = count > 0 ? Mathf.Min(count, maxTransferable) : maxTransferable;

        rootComponent.HpResource -= transferAmount;
        targetComponent.HpResource += transferAmount;

        GStruct154<GClass3408> discard = default;
        if (rootComponent.HpResource <= 0)
        {
            discard = InteractionsHandlerClass.Discard(item, itemController, false);
            if (!discard.Succeeded)
            {
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

        return new MC_Meds_Operation(item, item.CurrentAddress, targetItem, transferAmount, discard, itemController);
    }

    public static GStruct154<MC_Food_Operation> MergeFood(FoodDrinkItemClass item, FoodDrinkItemClass targetItem, float count, TraderControllerClass itemController, bool simulate)
    {
        if (item.TemplateId != targetItem.TemplateId)
        {
            return new GClass1522("Not same item");
        }

        if (item.Id == targetItem.Id)
        {
            return new GClass1522("Same item?");
        }

        if (targetItem.FoodDrinkComponent.HpPercent >= targetItem.FoodDrinkComponent.MaxResource)
        {
            return new GClass1522("Already max");
        }

        var rootComponent = item.FoodDrinkComponent;
        var targetComponent = targetItem.FoodDrinkComponent;

        var originalRootHp = rootComponent.HpPercent;
        var originalTargetHp = targetComponent.HpPercent;

        var maxTransferable = Mathf.Min(targetComponent.MaxResource - targetComponent.HpPercent, rootComponent.HpPercent);
        var transferAmount = count > 0 ? Mathf.Min(count, maxTransferable) : maxTransferable;

        rootComponent.HpPercent -= transferAmount;
        targetComponent.HpPercent += transferAmount;

        GStruct154<GClass3408> discard = default;
        if (rootComponent.HpPercent <= 0)
        {
            discard = InteractionsHandlerClass.Discard(item, itemController, false);
            if (!discard.Succeeded)
            {
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

        return new MC_Food_Operation(item, item.CurrentAddress, targetItem, transferAmount, discard, itemController);
    }

    private static void SendOperation(CombineItemsModel model)
    {
        var instance = Singleton<AbstractGame>.Instance;
        if (instance != null && instance is not HideoutGame)
        {
            return;
        }

        Singleton<ClientApplication<ISession>>.Instance.Session.SendOperationRightNow(model, (ar) => { });
    }
}
