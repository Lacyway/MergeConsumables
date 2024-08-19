using EFT.InventoryLogic;
using System;

namespace MergeConsumables
{
	public class MC_Food_Operation : IExecute, IRaiseEvents, GInterface339, GInterface343, GInterface347
	{
		public MC_Food_Operation(Item item, Item targetItem, TraderControllerClass itemController)
		{
			this.item = item;
			this.targetItem = targetItem;
			this.itemController = itemController;
		}

		public Item Item
		{
			get
			{
				return item;
			}
		}

		private readonly Item item;

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

		private readonly Item targetItem;
		private readonly TraderControllerClass itemController;

		public bool CanExecute(TraderControllerClass itemController)
		{
			if (item is FoodClass && targetItem is FoodClass)
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
			return InteractionsHandlerClassExtensions.MergeFood((FoodClass)item, (FoodClass)targetItem, itemController, false);
		}

		public void RaiseEvents(TraderControllerClass controller, CommandStatus status)
		{
			item.RaiseRefreshEvent(false, true);
			targetItem.RaiseRefreshEvent(false, true);
		}

		public void RollBack()
		{
			throw new NotImplementedException();
		}
	}
}
