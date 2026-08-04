using System.Threading.Tasks;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using EFT.InventoryLogic.Operations;
using MergeConsumables.Descriptors;
using MergeConsumables.Results;

namespace MergeConsumables.Operations;

public sealed class MergeMedsOperation : AbstractAsyncOperation<MergeMedsResult>
{
    public Item SourceItem;
    public ItemAddress SourceAddress;
    public Item TargetItem;
    public ItemAddress TargetAddress;
    public float Count;
    public MergeFoodResult Result;

    public MergeMedsOperation(ushort id, ItemController controller, MergeMedsResult result) : base(id, controller, result)
    {
        SourceItem = result.Item;
        SourceAddress = SourceItem.Parent;
        TargetItem = result.TargetItem;
        TargetAddress = TargetItem.Parent;
        Count = result.Count;
    }

    public override async Task<IResult> ExecuteInternal()
    {
        await OutProcess(SourceItem, SourceAddress, TargetAddress, new AddSuboperation(SourceItem, this));
        Execute();
        await InProcess(TargetItem, TargetAddress, new RemoveSuboperation(TargetItem, TargetAddress, this));
        return FinishExecution();
    }

    public override BaseInventoryCommand ToBaseInventoryCommand(string ownerId)
    {
        return _executableResult.Value.ToCombineItemsModel();
    }

    public override string ToString()
    {
        return $"Merging {SourceItem.ToFullString()} with {TargetItem.ToFullString()}, Count: {Count}";
    }

    public override InventoryOperationDescriptor ToDescriptor()
    {
        return new MergeMedsDescriptor()
        {
            Operation = this,
            OwnerId = OwnerId,
            OperationId = Id,
            SourceItem = SourceItem.Id,
            TargetItem = TargetItem.Id,
            Count = Count
        };
    }
}