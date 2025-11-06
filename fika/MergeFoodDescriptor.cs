using Comfort.Common;
using Diz.LanguageExtensions;
using EFT;
using EFT.InventoryLogic;
using System.Threading.Tasks;

namespace MergeConsumables;

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
            return new MergeConsumableError(sourceItemResult.Value);
        }

        var targetItemResult = player.FindItemById(TargetItem);
        if (targetItemResult.Failed)
        {
            return targetItemResult.Error;
        }
        if (targetItemResult.Value is not FoodDrinkItemClass targetFood)
        {
            return new MergeConsumableError(targetItemResult.Value);
        }

        var result = InteractionsHandlerClassExtensions.MergeFood(sourceFood, targetFood, Count, player.InventoryController, true);
        if (result.Failed)
        {
            return result.Error;
        }

        return new MergeFoodOperation(OperationId, player.InventoryController, result.Value);
    }
}

public class MergeConsumableError(Item item) : Error
{
    public Item Item = item;

    public override string ToString()
    {
        return $"Invalid item type: {Item.GetType().Name}";
    }
}

public class MergeFoodOperation : GClass3475<MergeFoodResult>
{
    public Item SourceItem;
    public ItemAddress SourceAddress;
    public Item TargetItem;
    public ItemAddress TargetAddress;
    public float Count;

    public MergeFoodOperation(ushort id, TraderControllerClass controller, MergeFoodResult result) : base(id, controller, result)
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
        return null;
    }

    public override BaseDescriptorClass ToDescriptor()
    {
        return new MergeFoodDescriptor()
        {
            Operation = this,
            SourceItem = SourceItem.Id,
            TargetItem = TargetItem.Id,
            Count = Count
        };
    }
}
