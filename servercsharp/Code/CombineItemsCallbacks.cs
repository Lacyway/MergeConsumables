using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.ItemEvent;

namespace MergeConsumables;

[Injectable]
public sealed class CombineItemCallbacks(CombineItemController combineItemController)
{
    public async ValueTask<ItemEventRouterResponse> HandleCombineItems(PmcData pmcData, CombineItemsModel body, string sessionID)
    {
        return await combineItemController.CombineItems(pmcData, body, sessionID);
    }
}
