using Comfort.Common;
using EFT.InventoryLogic;
using MergeConsumables.Descriptors;
using MergeConsumables.Results;
using System.Threading.Tasks;

namespace MergeConsumables.Operations;

public class MergeMedsOperation : GClass3475<MergeMedsResult>
{
    public Item SourceItem;
    public ItemAddress SourceAddress;
    public Item TargetItem;
    public ItemAddress TargetAddress;
    public float Count;
    public MergeFoodResult Result;

    public MergeMedsOperation(ushort id, TraderControllerClass controller, MergeMedsResult result) : base(id, controller, result)
    {
        SourceItem = result.Item;
        SourceAddress = SourceItem.Parent;
        TargetItem = result.TargetItem;
        TargetAddress = TargetItem.Parent;
        Count = result.Count;
    }

    public override async Task<IResult> ExecuteInternal()
    {
        await method_3(SourceItem, SourceAddress, TargetAddress, new GClass3397(SourceItem, this));
        Execute();
        await method_4(TargetItem, TargetAddress, new GClass3398(TargetItem, TargetAddress, this));
        return method_5();
    }

    public override GClass3471 ToBaseInventoryCommand(string ownerId)
    {
        return Gstruct154_0.Value.ToCombineItemsModel();
    }

    public override string ToString()
    {
        return $"Merging {SourceItem.ToFullString()} with {TargetItem.ToFullString()}, Count: {Count}";
    }

    public override BaseDescriptorClass ToDescriptor()
    {
        return new MergeMedsDescriptor()
        {
            Operation = this,
            SourceItem = SourceItem.Id,
            TargetItem = TargetItem.Id,
            Count = Count
        };
    }
}