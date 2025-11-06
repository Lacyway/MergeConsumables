using EFT.InventoryLogic;

namespace MergeConsumables;

public class MC_Food_Operation : IExecute, IRaiseEvents, GInterface424, GInterface429, GInterface433
{
    public MC_Food_Operation(FoodDrinkItemClass item, ItemAddress from, FoodDrinkItemClass targetItem, float count, GStruct154<GClass3408> discard, TraderControllerClass itemController)
    {
        _item = item;
        From = from;
        _targetItem = targetItem;
        _count = count;
        _discard = discard;
        _itemController = itemController;
    }

    public Item Item
    {
        get
        {
            return _item;
        }
    }

    public Item ResultItem
    {
        get
        {
            return _targetItem;
        }
    }

    public ItemAddress From { get; }

    public Item TargetItem
    {
        get
        {
            return _targetItem;
        }
    }

    public TraderControllerClass ItemController
    {
        get
        {
            return _itemController;
        }
    }

    private readonly FoodDrinkItemClass _item;
    private readonly FoodDrinkItemClass _targetItem;
    private readonly float _count;
    private readonly TraderControllerClass _itemController;
    private readonly GStruct154<GClass3408> _discard;

    public bool CanExecute(TraderControllerClass itemController)
    {
        if (_item != null && _targetItem != null)
        {
            if (_item.TemplateId == _targetItem.TemplateId)
            {
                return true;
            }
        }

        return false;
    }

    public GStruct153 Execute()
    {
        return InteractionsHandlerClassExtensions.MergeFood(_item, _targetItem, _count, _itemController, false);
    }

    public void RaiseEvents(IItemOwner controller, CommandStatus status)
    {
        if (_discard.Succeeded && _discard.Value != null)
        {
            _discard.Value.RaiseEvents(controller, status);
        }
        else
        {
            _item.RaiseRefreshEvent(false, true);
        }

        _targetItem.RaiseRefreshEvent(false, true);
    }

    public void RollBack()
    {
        if (_discard.Succeeded && _discard.Value != null)
        {
            _discard.Value.RollBack();
        }

        InteractionsHandlerClassExtensions.MergeFood(_targetItem, _item, _count, _itemController, false);
    }

    public CombineItemsModel ToCombineItemsModel()
    {
        return new CombineItemsModel(_item.Id, _targetItem.Id, _item.FoodDrinkComponent.HpPercent, _targetItem.FoodDrinkComponent.HpPercent, "food");
    }
}
