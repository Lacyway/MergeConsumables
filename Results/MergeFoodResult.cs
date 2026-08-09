using Diz.LanguageExtensions;
using EFT;
using EFT.InventoryLogic;
using MergeConsumables.Models;

namespace MergeConsumables.Results;

public class MergeFoodResult : ITransferOrMergeResult, ISyncOperationResult
{
    public MergeFoodResult(FoodDrink item, ItemAddress from, FoodDrink targetItem, float count, OperationResult<DiscardResult> discard, ItemController itemController)
    {
        _item = item;
        From = from;
        _targetItem = targetItem;
        Count = count;
        _discard = discard;
        ItemController = itemController;
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

    public float Count { get; }

    public ItemController ItemController { get; }

    private readonly FoodDrink _item;
    private readonly FoodDrink _targetItem;
    private readonly OperationResult<DiscardResult> _discard;

    public bool CanExecute(ItemController itemController)
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

    public OperationResult Execute()
    {
        return ItemManipulatorExtensions.MergeFood(_item, _targetItem, Count, ItemController, false);
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

        ItemManipulatorExtensions.MergeFood(_targetItem, _item, Count, ItemController, false);
    }

    public CombineItemsModel ToCombineItemsModel()
    {
        return new CombineItemsModel(_item.Id, _targetItem.Id,
            _item.FoodDrinkComponent.HpPercent, _targetItem.FoodDrinkComponent.HpPercent, Count, "food");
    }
}
