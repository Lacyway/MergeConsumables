using Diz.LanguageExtensions;
using EFT;
using EFT.InventoryLogic;
using EFT.InventoryLogic.Operations;
using MergeConsumables.Errors;
using MergeConsumables.Operations;

namespace MergeConsumables.Descriptors;

public sealed class MergeMedsDescriptor : InventoryOperationDescriptor
{
    public string SourceItem;
    public string TargetItem;
    public float Count;

    public override OperationCreationResult<AbstractOperation> ToInventoryOperation(IPlayer player)
    {
        var sourceItemResult = player.FindItemById(SourceItem);
        if (sourceItemResult.Failed)
        {
            return sourceItemResult.Error;
        }
        if (sourceItemResult.Value is not Meds sourceMeds)
        {
            return new WrongTypeError(sourceItemResult.Value);
        }

        var targetItemResult = player.FindItemById(TargetItem);
        if (targetItemResult.Failed)
        {
            return targetItemResult.Error;
        }
        if (targetItemResult.Value is not Meds targetMeds)
        {
            return new WrongTypeError(targetItemResult.Value);
        }

        var result = ItemManipulatorExtensions.MergeMeds(sourceMeds, targetMeds,
            Count, player.InventoryController, true);
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
