using EFT.InventoryLogic;

namespace MergeConsumables
{
	public class MC_Food_Operation : IExecute, IRaiseEvents, GInterface339, GInterface343, GInterface347
	{
		public MC_Food_Operation(FoodClass item, FoodClass targetItem, float count, GStruct414<GClass2799> discard, TraderControllerClass itemController)
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

		private readonly FoodClass item;
		private readonly FoodClass targetItem;
		private readonly float count;
		private readonly TraderControllerClass itemController;
		private readonly GStruct414<GClass2799> discard;

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

		public GStruct413 Execute()
		{
			return InteractionsHandlerClassExtensions.MergeFood(item, targetItem, count, itemController, false);
		}

		public void RaiseEvents(TraderControllerClass controller, CommandStatus status)
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
