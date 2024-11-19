using EFT.InventoryLogic;

namespace MergeConsumables
{
	public class MC_Meds_Operation : IExecute, IRaiseEvents, GInterface385, GInterface390, GInterface394
	{
		public MC_Meds_Operation(MedsItemClass item, MedsItemClass targetItem, float count, GStruct446<GClass3129> discard, TraderControllerClass itemController)
		{
			this.item = item;
			this.targetItem = targetItem;
			this.count = count;
			this.discard = discard;
			this.itemController = itemController;
		}

		public Item Item
		{
			get
			{
				return item;
			}
		}

		public Item ResultItem
		{
			get
			{
				return targetItem;
			}
		}

		public Item TargetItem
		{
			get
			{
				return targetItem;
			}
		}

		public TraderControllerClass ItemController
		{
			get
			{
				return itemController;
			}
		}

		private readonly MedsItemClass item;
		private readonly MedsItemClass targetItem;
		private readonly float count;
		private readonly TraderControllerClass itemController;
		private readonly GStruct446<GClass3129> discard;

		public bool CanExecute(TraderControllerClass itemController)
		{
			if (item != null && targetItem != null)
			{
				if (item.TemplateId == targetItem.TemplateId)
				{
					return true;
				}
			}

			return false;
		}

		public GStruct445 Execute()
		{
			return InteractionsHandlerClassExtensions.MergeMeds(item, targetItem, count, itemController, false);
		}

		public void RaiseEvents(IItemOwner controller, CommandStatus status)
		{
			if (discard.Succeeded && discard.Value != null)
			{
				discard.Value.RaiseEvents(controller, status);
			}
			else
			{
				item.RaiseRefreshEvent(false, true);
			}

			targetItem.RaiseRefreshEvent(false, true);
		}

		public void RollBack()
		{
			if (discard.Succeeded && discard.Value != null)
			{
				discard.Value.RollBack();
			}

			InteractionsHandlerClassExtensions.MergeMeds(targetItem, item, count, itemController, false);
		}

		public CombineItemsModel ToCombineItemsModel()
		{
			return new CombineItemsModel(item.Id, targetItem.Id, item.MedKitComponent.HpResource, targetItem.MedKitComponent.HpResource, "medical");
		}
	}
}
