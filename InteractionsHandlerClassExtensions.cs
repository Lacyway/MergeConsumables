using Comfort.Common;
using EFT.InventoryLogic;
using EFT.UI;

namespace MergeConsumables
{
	public static class InteractionsHandlerClassExtensions
	{
		public static GStruct414<MC_Meds_Operation> MergeMeds(MedsClass item, MedsClass targetItem, TraderControllerClass itemController, bool simulate)
		{
			if (item.TemplateId != targetItem.TemplateId)
			{
				return new GClass3370("Not same item");
			}

			if (item.Id == targetItem.Id)
			{
				return new GClass3370("Same item?");
			}

			if (targetItem.MedKitComponent.HpResource >= targetItem.MedKitComponent.MaxHpResource)
			{
				return new GClass3370("Already max");
			}

			if (!simulate)
			{
				MedKitComponent rootComponent = item.MedKitComponent;
				MedKitComponent targetComponent = targetItem.MedKitComponent;

				while (rootComponent.HpResource > 0 && targetComponent.HpResource < targetComponent.MaxHpResource)
				{
					rootComponent.HpResource--;
					targetComponent.HpResource++;
				}

				if (rootComponent.HpResource <= 0)
				{
					GStruct414<GClass2799> discard = InteractionsHandlerClass.Discard(item, itemController, false, false);
					if (!discard.Succeeded)
					{
						MC_Plugin.MC_Logger.LogError(discard.Error);
					}
					else
					{
						discard.Value.RaiseEvents(itemController, CommandStatus.Begin);
						discard.Value.RaiseEvents(itemController, CommandStatus.Succeed);
					}
				}

				Singleton<GUISounds>.Instance.PlayItemSound(item.ItemSound, EInventorySoundType.drop);
			}

			return new MC_Meds_Operation(item, targetItem, itemController);
		}

		public static GStruct414<MC_Food_Operation> MergeFood(FoodClass item, FoodClass targetItem, TraderControllerClass itemController, bool simulate)
		{
			if (item.TemplateId != targetItem.TemplateId)
			{
				return new GClass3370("Not same item");
			}

			if (item.Id == targetItem.Id)
			{
				return new GClass3370("Same item?");
			}

			if (targetItem.FoodDrinkComponent.HpPercent >= targetItem.FoodDrinkComponent.MaxResource)
			{
				return new GClass3370("Already max");
			}

			if (!simulate)
			{
				FoodDrinkComponent rootComponent = item.FoodDrinkComponent;
				FoodDrinkComponent targetComponent = targetItem.FoodDrinkComponent;

				while (rootComponent.HpPercent > 0 && targetComponent.HpPercent < targetComponent.MaxResource)
				{
					rootComponent.HpPercent--;
					targetComponent.HpPercent++;
				}

				if (rootComponent.HpPercent <= 0)
				{
					GStruct414<GClass2799> discard = InteractionsHandlerClass.Discard(item, itemController, false, false);
					if (!discard.Succeeded)
					{
						MC_Plugin.MC_Logger.LogError(discard.Error);
					}
					else
					{
						discard.Value.RaiseEvents(itemController, CommandStatus.Begin);
						discard.Value.RaiseEvents(itemController, CommandStatus.Succeed);
					}
				}

				Singleton<GUISounds>.Instance.PlayItemSound(item.ItemSound, EInventorySoundType.drop);
			}

			return new MC_Food_Operation(item, targetItem, itemController);
		}
	}
}
