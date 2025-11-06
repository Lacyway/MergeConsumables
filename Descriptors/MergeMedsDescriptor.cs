using EFT;
using MergeConsumables.Errors;
using MergeConsumables.Operations;

namespace MergeConsumables.Descriptors;

public class MergeMedsDescriptor : BaseDescriptorClass
{
    public string SourceItem;
    public string TargetItem;
    public float Count;

    public override GStruct152<BaseInventoryOperationClass> ToInventoryOperation(IPlayer player)
    {
        var sourceItemResult = player.FindItemById(SourceItem);
        if (sourceItemResult.Failed)
        {
            return sourceItemResult.Error;
        }
        if (sourceItemResult.Value is not MedsItemClass sourceMeds)
        {
            return new WrongTypeError(sourceItemResult.Value);
        }

        var targetItemResult = player.FindItemById(TargetItem);
        if (targetItemResult.Failed)
        {
            return targetItemResult.Error;
        }
        if (targetItemResult.Value is not MedsItemClass targetMeds)
        {
            return new WrongTypeError(targetItemResult.Value);
        }

        var result = InteractionsHandlerClassExtensions.MergeMeds(sourceMeds, targetMeds, Count, player.InventoryController, true);
        if (result.Failed)
        {
            return result.Error;
        }

        return new MergeMedsOperation(OperationId, player.InventoryController, result.Value);
    }

    public override string ToString()
    {
        return $"Source: {SourceItem}, Target: {TargetItem}, Count: {Count}";
    }
}
