using EFT.InventoryLogic;
using MergeConsumables.Models;

namespace MergeConsumables.Results;

public class MergeMedsResult : IExecute, IRaiseEvents, GInterface424, GInterface429, GInterface433
{
    public MergeMedsResult(MedsItemClass item, ItemAddress from, MedsItemClass targetItem, float count, GStruct154<GClass3408> discard, TraderControllerClass itemController)
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

    public TraderControllerClass ItemController { get; }

    private readonly MedsItemClass _item;
    private readonly MedsItemClass _targetItem;
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
        return InteractionsHandlerClassExtensions.MergeMeds(_item, _targetItem, Count, ItemController, false);
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

        InteractionsHandlerClassExtensions.MergeMeds(_targetItem, _item, Count, ItemController, false);
    }

    public CombineItemsModel ToCombineItemsModel()
    {
        return new CombineItemsModel(_item.Id, _targetItem.Id, _item.MedKitComponent.HpResource, _targetItem.MedKitComponent.HpResource, "medical");
    }
}
