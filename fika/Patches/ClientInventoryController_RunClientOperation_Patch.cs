using Comfort.Common;
using Fika.Core.Main.ClientClasses;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib;
using MergeConsumables.Operations;
using SPT.Reflection.Patching;
using System.Reflection;

namespace MergeConsumablesFika.Patches;

public class ClientInventoryController_RunClientOperation_Patch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(ClientInventoryController)
            .GetMethod("RunClientOperation", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [PatchPrefix]
    public static bool Prefix(ClientInventoryController __instance, BaseInventoryOperationClass operation, Callback callback)
    {
        if (operation is MergeFoodOperation mergeFoodOperation)
        {
            if (!__instance.vmethod_0(operation))
            {
                operation.Dispose();
                callback.Fail("LOCAL: hands controller can't perform this operation");
                return false;
            }

            ClientInventoryController.ClientInventoryOperationHandler handler = new()
            {
                Operation = operation,
                Callback = callback,
                InventoryController = __instance
            };

            var operationNum = __instance.AddOperationCallback(operation, handler.ReceiveStatusFromServer);
            var mergePacket = new MergePacket()
            {
                NetId = __instance.FikaPlayer.NetId,
                CallbackId = operationNum,
                Type = 0,
                MergeFoodOperation = mergeFoodOperation
            };
            Singleton<FikaClient>.Instance.SendData(ref mergePacket, DeliveryMethod.ReliableOrdered, true);
            return false;
        }

        if (operation is MergeMedsOperation mergeMedsOperation)
        {
            if (!__instance.vmethod_0(operation))
            {
                operation.Dispose();
                callback.Fail("LOCAL: hands controller can't perform this operation");
                return false;
            }

            ClientInventoryController.ClientInventoryOperationHandler handler = new()
            {
                Operation = operation,
                Callback = callback,
                InventoryController = __instance
            };

            var operationNum = __instance.AddOperationCallback(operation, handler.ReceiveStatusFromServer);
            var mergePacket = new MergePacket()
            {
                NetId = __instance.FikaPlayer.NetId,
                CallbackId = operationNum,
                Type = 1,
                MergeMedsOperation = mergeMedsOperation
            };
            Singleton<FikaClient>.Instance.SendData(ref mergePacket, DeliveryMethod.ReliableOrdered, true);
            return false;
        }

        return true;
    }
}
