using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Request;
using SPTarkov.Server.Core.Models.Eft.ItemEvent;

namespace MergeConsumables;

[Injectable]
public class CombineItemEventRouter(CombineItemCallbacks combineItemCallbacks) : ItemEventRouterDefinition
{
    

    public override async ValueTask<ItemEventRouterResponse> HandleItemEvent(string url, PmcData pmcData, BaseInteractionRequestData body, MongoId sessionID, ItemEventRouterResponse output)
    {
        return url switch
        {
            MergeConsumables.CombineRouter => await combineItemCallbacks.HandleCombineItems(pmcData, body as CombineItemsModel, sessionID),
            _ => throw new Exception($"CombineItemEventRouter being used when it cant handle route {url}")
        };
    }

    protected override List<HandledRoute> GetHandledRoutes()
    {
        return [new(MergeConsumables.CombineRouter, false)];
    }

    protected override ValueTask<ItemEventRouterResponse> HandleItemEventInternal(string url, PmcData pmcData, BaseInteractionRequestData body, MongoId sessionID, ItemEventRouterResponse output)
    {
        throw new NotImplementedException();
    }
}
