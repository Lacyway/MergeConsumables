using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace MergeConsumables
{
	public class MC_Patches
	{
		public void Enable()
		{
			new ExecutePossibleAction_Patch().Enable();
			new RunNetworkTransaction_Patch().Enable();
		}

		public class ExecutePossibleAction_Patch : ModulePatch
		{
			protected override MethodBase GetTargetMethod()
			{
				return AccessTools.Method(typeof(TraderControllerClass), nameof(TraderControllerClass.ExecutePossibleAction),
					[typeof(ItemContextAbstractClass), typeof(Item), typeof(bool), typeof(bool)]);
			}

			[PatchPrefix]
			public static bool Prefix(TraderControllerClass __instance, ItemContextAbstractClass itemContext, Item targetItem, bool simulate, ref GStruct445 __result)
			{
				if (itemContext.Item is MedsItemClass rootMedItem
					&& rootMedItem.MedKitComponent != null
					&& targetItem is MedsItemClass targetMedItem
					&& targetMedItem.MedKitComponent != null)
				{
					__result = InteractionsHandlerClassExtensions.MergeMeds(rootMedItem, targetMedItem, 0, __instance, simulate);
					return false;
				}

				if (itemContext.Item is FoodDrinkItemClass rootFoodItem
					&& rootFoodItem.FoodDrinkComponent != null
					&& targetItem is FoodDrinkItemClass targetFoodItem
					&& targetFoodItem.FoodDrinkComponent != null)
				{
					__result = InteractionsHandlerClassExtensions.MergeFood(rootFoodItem, targetFoodItem, 0, __instance, simulate);
					return false;
				}

				return true;
			}
		}

		public class RunNetworkTransaction_Patch : ModulePatch
		{
			protected override MethodBase GetTargetMethod()
			{
				return typeof(TraderControllerClass).GetMethod(nameof(TraderControllerClass.RunNetworkTransaction));
			}

			[PatchPrefix]
			public static bool Prefix(TraderControllerClass __instance, IRaiseEvents operationResult, Callback callback)
			{
				if (operationResult is MC_Food_Operation foodOperation)
				{
					var result = foodOperation.Execute();
					if (result.Succeeded && result.Value is MC_Food_Operation executedFoodOperation)
					{
						executedFoodOperation.RaiseEvents(executedFoodOperation.ItemController, CommandStatus.Begin);
						SendOperation(executedFoodOperation.ToCombineItemsModel(), callback);
						executedFoodOperation.RaiseEvents(executedFoodOperation.ItemController, CommandStatus.Succeed);
					}

					return false;
				}

				if (operationResult is MC_Meds_Operation medsOperation)
				{
					var result = medsOperation.Execute();
					if (result.Succeeded && result.Value is MC_Meds_Operation executedMedsOperation)
					{
						executedMedsOperation.RaiseEvents(executedMedsOperation.ItemController, CommandStatus.Begin);
						SendOperation(executedMedsOperation.ToCombineItemsModel(), callback);
						executedMedsOperation.RaiseEvents(executedMedsOperation.ItemController, CommandStatus.Succeed);
					}

					return false;
				}

				return true;
			}

			private static void SendOperation(CombineItemsModel model, Callback callback)
			{
				AbstractGame instance = Singleton<AbstractGame>.Instance;
				if (instance != null && instance is not HideoutGame)
				{
					return;
				}

				callback ??= result => { };
				Singleton<ClientApplication<ISession>>.Instance.Session.SendOperationRightNow(model, callback);
			}
		}
	}
}
