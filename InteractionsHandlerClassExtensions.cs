using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using UnityEngine;

namespace MergeConsumables
{
	public static class InteractionsHandlerClassExtensions
	{
		public static GStruct455<MC_Meds_Operation> MergeMeds(MedsItemClass item, MedsItemClass targetItem, float count, TraderControllerClass itemController, bool simulate)
		{
			if (item.TemplateId != targetItem.TemplateId)
			{
				return new GClass3854("Not same item");
			}

			if (item.Id == targetItem.Id)
			{
				return new GClass3854("Same item?");
			}

			if (targetItem.MedKitComponent.HpResource >= targetItem.MedKitComponent.MaxHpResource)
			{
				return new GClass3854("Already max");
			}

			MedKitComponent rootComponent = item.MedKitComponent;
			MedKitComponent targetComponent = targetItem.MedKitComponent;

			float originalRootHp = rootComponent.HpResource;
			float originalTargetHp = targetComponent.HpResource;

			float maxTransferable = Mathf.Min(targetComponent.MaxHpResource - targetComponent.HpResource, rootComponent.HpResource);
			float transferAmount = count > 0 ? Mathf.Min(count, maxTransferable) : maxTransferable;

			rootComponent.HpResource -= transferAmount;
			targetComponent.HpResource += transferAmount;

			GStruct455<GClass3200> discard = default;
			if (rootComponent.HpResource <= 0)
			{
				discard = InteractionsHandlerClass.Discard(item, itemController, false);
				if (!discard.Succeeded)
				{
					MC_Plugin.MC_Logger.LogError(discard.Error);
					return discard.Error;
				}
			}

			if (simulate)
			{
				discard.Value?.RollBack();

				rootComponent.HpResource = originalRootHp;
				targetComponent.HpResource = originalTargetHp;
			}

			return new MC_Meds_Operation(item, item.Parent, targetItem, transferAmount, discard, itemController);
		}

		public static GStruct455<MC_Food_Operation> MergeFood(FoodDrinkItemClass item, FoodDrinkItemClass targetItem, float count, TraderControllerClass itemController, bool simulate)
		{
			if (item.TemplateId != targetItem.TemplateId)
			{
				return new GClass3854("Not same item");
			}

			if (item.Id == targetItem.Id)
			{
				return new GClass3854("Same item?");
			}

			if (targetItem.FoodDrinkComponent.HpPercent >= targetItem.FoodDrinkComponent.MaxResource)
			{
				return new GClass3854("Already max");
			}

			FoodDrinkComponent rootComponent = item.FoodDrinkComponent;
			FoodDrinkComponent targetComponent = targetItem.FoodDrinkComponent;

			float originalRootHp = rootComponent.HpPercent;
			float originalTargetHp = targetComponent.HpPercent;

			float maxTransferable = Mathf.Min(targetComponent.MaxResource - targetComponent.HpPercent, rootComponent.HpPercent);
			float transferAmount = count > 0 ? Mathf.Min(count, maxTransferable) : maxTransferable;

			rootComponent.HpPercent -= transferAmount;
			targetComponent.HpPercent += transferAmount;

			GStruct455<GClass3200> discard = default;
			if (rootComponent.HpPercent <= 0)
			{
				discard = InteractionsHandlerClass.Discard(item, itemController, false);
				if (!discard.Succeeded)
				{
					MC_Plugin.MC_Logger.LogError(discard.Error);
					return discard.Error;
				}
			}

			if (simulate)
			{
				discard.Value?.RollBack();

				rootComponent.HpPercent = originalRootHp;
				targetComponent.HpPercent = originalTargetHp;
			}

			return new MC_Food_Operation(item, item.Parent, targetItem, transferAmount, discard, itemController);
		}

		private static void SendOperation(CombineItemsModel model)
		{
			AbstractGame instance = Singleton<AbstractGame>.Instance;
			if (instance != null && instance is not HideoutGame)
			{
				return;
			}

			Singleton<ClientApplication<ISession>>.Instance.Session.SendOperationRightNow(model, (ar) => { });
		}
	}
}
