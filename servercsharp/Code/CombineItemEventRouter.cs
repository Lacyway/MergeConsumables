using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.DI.Routing;

namespace MergeConsumables;

[Injectable(TypePriority = OnLoadOrder.Routers)]
public sealed class CombineItemEventR(CombineItemCallbacks combineItemCallbacks) : ItemEventRouter([new ItemRouteAction<CombineItemsModel>(MergeConsumables.CombineRouter,
    async (url, pmcData, body, sessionID, output, cancellationToken) => await combineItemCallbacks.HandleCombineItems(pmcData, body, sessionID, cancellationToken))]);