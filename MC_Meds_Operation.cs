using EFT.InventoryLogic;

namespace MergeConsumables
{
	public class MC_Meds_Operation : IExecute, IRaiseEvents, GInterface398, GInterface403, GInterface407
    {
		public MC_Meds_Operation(MedsItemClass item, ItemAddress from, MedsItemClass targetItem, float count, GStruct455<GClass3200> discard, TraderControllerClass itemController)
		{
			this.item = item;
            this.from = from;
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

        public ItemAddress From
        {
            get
            {
                return from;
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
        private readonly ItemAddress from;
        private readonly MedsItemClass targetItem;
		private readonly float count;
		private readonly TraderControllerClass itemController;
		private readonly GStruct455<GClass3200> discard;

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

		public GStruct454 Execute()
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
