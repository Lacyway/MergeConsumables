using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Extensions;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Eft.ItemEvent;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Routers;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Utils;

namespace MergeConsumables;

[Injectable]
public class CombineItemController(ISptLogger<CombineItemController> logger, EventOutputHolder eventOutputHolder,
    InventoryHelper inventoryHelper, HttpResponseUtil httpResponseUtil, SaveServer saveServer)
{
    public async ValueTask<ItemEventRouterResponse> CombineItems(PmcData pmcData, CombineItemsModel body, string sessionId)
    {
        var output = eventOutputHolder.GetOutput(sessionId);

        if (body is null || body.SourceItem is null || body.TargetItem is null || body.Type is null)
        {
            return httpResponseUtil.AppendErrorToOutput(output, "Missing data in body");
        }

#if DEBUG
        logger.Info($"Received request to merge: SourceItem: {body.SourceItem}, TargetItem: {body.TargetItem}, SourceAmount: {body.SourceAmount}, TargetAmount: {body.TargetAmount}"); 
#endif

        var sourceItem = pmcData.Inventory?.Items?
            .FirstOrDefault(i => i.Id == body.SourceItem);
        var targetItem = pmcData.Inventory?.Items?
            .FirstOrDefault(i => i.Id == body.TargetItem);

        if (sourceItem == null || targetItem == null)
        {
            return httpResponseUtil.AppendErrorToOutput(output, $"Could not find source or target item! Soruce: {body.SourceItem}, Target; {body.TargetItem}");
        }

        sourceItem.AddUpd();
        targetItem.AddUpd();

        switch (body.Type)
        {
            case "medical":
                if (sourceItem.Upd!.MedKit is UpdMedKit sourceMedKit && targetItem.Upd!.MedKit is UpdMedKit targetMedKit)
                {
                    sourceMedKit.HpResource -= body.TransferAmount;
                    targetMedKit.HpResource += body.TransferAmount;
                }
                else if (sourceItem.Template == targetItem.Template)
                {
                    var newSourceMedKit = sourceItem.Upd!.MedKit ??= new();
                    newSourceMedKit.HpResource = body.SourceAmount;

                    var newTargetMedKit = targetItem.Upd!.MedKit ??= new();
                    newTargetMedKit.HpResource = body.TargetAmount;

                    newSourceMedKit.HpResource -= body.TransferAmount;
                    newTargetMedKit.HpResource += body.TransferAmount;

                    logger.Warning("MedKit was missing on source or target item - attempted to resolve with Template");
                }
                else
                {
                    const string errorMessage = "MedKit was missing on source or target item!";
                    logger.Error(errorMessage);
                    return httpResponseUtil.AppendErrorToOutput(output, errorMessage);
                }
                break;
            case "food":
                if (sourceItem.Upd!.FoodDrink is UpdFoodDrink sourceFoodDrink && targetItem.Upd!.FoodDrink is UpdFoodDrink targetFoodDrink)
                {
                    sourceFoodDrink.HpPercent -= body.TransferAmount;
                    targetFoodDrink.HpPercent += body.TransferAmount;
                }
                else if (sourceItem.Template == targetItem.Template)
                {
                    var newSourceFoodDrink = sourceItem.Upd!.FoodDrink ??= new();
                    newSourceFoodDrink.HpPercent = body.SourceAmount;

                    var newTargetFoodDrink = targetItem.Upd!.FoodDrink ??= new();
                    newTargetFoodDrink.HpPercent = body.TargetAmount;

                    newSourceFoodDrink.HpPercent -= body.TransferAmount;
                    newTargetFoodDrink.HpPercent += body.TransferAmount;

                    logger.Warning("FoodDrink was missing on source or target item - attempted to resolve with Template");
                }
                else
                {
                    const string errorMessage = "FoodDrink was missing on source or target item!";
                    logger.Error(errorMessage);
                    return httpResponseUtil.AppendErrorToOutput(output, errorMessage);
                }
                break;
        }

        if (body.SourceAmount <= body.TransferAmount)
        {
            inventoryHelper.RemoveItem(pmcData, body.SourceItem.Value, sessionId);
        }

        await saveServer.SaveProfileAsync(sessionId);

        return output;
    }
}