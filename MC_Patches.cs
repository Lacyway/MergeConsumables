using EFT.InventoryLogic;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using System.Threading.Tasks;

namespace MergeConsumables
{
	public class MC_Patches
	{
		public void Enable()
		{
			new ExecutePossibleAction_Patch().Enable();
			new AcceptItem_Patch().Enable();
		}

		public class ExecutePossibleAction_Patch : ModulePatch
		{
			protected override MethodBase GetTargetMethod()
			{
				return AccessTools.Method(typeof(TraderControllerClass), nameof(TraderControllerClass.ExecutePossibleAction),
					new[] { typeof(ItemContextAbstractClass), typeof(Item), typeof(bool), typeof(bool) });
			}

			[PatchPrefix]
			public static bool Prefix(TraderControllerClass __instance, ItemContextAbstractClass itemContext, Item targetItem, bool simulate, ref GStruct413 __result)
			{
				if (itemContext.Item is MedsClass rootMedItem
					&& rootMedItem.MedKitComponent != null
					&& targetItem is MedsClass targetMedItem
					&& targetMedItem.MedKitComponent != null)
				{
					__result = InteractionsHandlerClassExtensions.MergeMeds(rootMedItem, targetMedItem, __instance, simulate);
					return false;
				}

				if (itemContext.Item is FoodClass rootFoodItem
					&& rootFoodItem.FoodDrinkComponent != null
					&& targetItem is FoodClass targetFoodItem
					&& targetFoodItem.FoodDrinkComponent != null)
				{
					__result = InteractionsHandlerClassExtensions.MergeFood(rootFoodItem, targetFoodItem, __instance, simulate);
					return false;
				}

				return true;
			}
		}

		public class AcceptItem_Patch : ModulePatch
		{
			protected override MethodBase GetTargetMethod()
			{
				return typeof(GridView).GetMethod(nameof(GridView.AcceptItem));
			}

			[PatchPrefix]
			public static bool Prefix(GridView __instance, ItemContextClass itemContext, ItemContextAbstractClass targetItemContext, ref Task __result)
			{
				if (__instance.CanAccept(itemContext, targetItemContext, out GStruct413 result))
				{
					if (result.Value is MC_Meds_Operation medOperation)
					{
						medOperation.Execute();
						medOperation.RaiseEvents(medOperation.ItemController, CommandStatus.Succeed);
						__result = Task.CompletedTask;
						return false;
					}

					if (result.Value is MC_Food_Operation foodOperation)
					{
						foodOperation.Execute();
						foodOperation.RaiseEvents(foodOperation.ItemController, CommandStatus.Succeed);
						__result = Task.CompletedTask;
						return false;
					}
				}
				return true;
			}
		}
	}
}
