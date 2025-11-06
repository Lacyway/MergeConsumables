using EFT;
using MergeConsumables.Errors;
using MergeConsumables.Operations;

namespace MergeConsumables.Descriptors;

public class MergeFoodDescriptor : BaseDescriptorClass
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
        if (sourceItemResult.Value is not FoodDrinkItemClass sourceFood)
        {
            return new WrongTypeError(sourceItemResult.Value);
        }

        var targetItemResult = player.FindItemById(TargetItem);
        if (targetItemResult.Failed)
        {
            return targetItemResult.Error;
        }
        if (targetItemResult.Value is not FoodDrinkItemClass targetFood)
        {
            return new WrongTypeError(targetItemResult.Value);
        }

        var result = InteractionsHandlerClassExtensions.MergeFood(sourceFood, targetFood, Count, player.InventoryController, true);
        if (result.Failed)
        {
            return result.Error;
        }

        return new MergeFoodOperation(OperationId, player.InventoryController, result.Value);
    }

    public override string ToString()
    {
        return $"ID: {OperationId}, OwnerId: {OwnerId}, Source: {SourceItem}, Target: {TargetItem}, Count: {Count}";
    }
}
