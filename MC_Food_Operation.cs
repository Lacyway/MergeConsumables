using EFT.InventoryLogic;

namespace MergeConsumables
{
	public class MC_Food_Operation : IExecute, IRaiseEvents, GInterface385, GInterface390, GInterface394
	{
		public MC_Food_Operation(FoodDrinkItemClass item, FoodDrinkItemClass targetItem, float count, GStruct446<GClass3129> discard, TraderControllerClass itemController)
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

		private readonly FoodDrinkItemClass item;
		private readonly FoodDrinkItemClass targetItem;
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
			return InteractionsHandlerClassExtensions.MergeFood(item, targetItem, count, itemController, false);
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

			InteractionsHandlerClassExtensions.MergeFood(targetItem, item, count, itemController, false);
		}

		public CombineItemsModel ToCombineItemsModel()
		{
			return new CombineItemsModel(item.Id, targetItem.Id, item.FoodDrinkComponent.HpPercent, targetItem.FoodDrinkComponent.HpPercent, "food");
		}
	}
}
