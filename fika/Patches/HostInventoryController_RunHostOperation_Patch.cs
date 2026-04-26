using Comfort.Common;
using Fika.Core.Main.HostClasses;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib;
using MergeConsumables.Operations;
using SPT.Reflection.Patching;
using System.Reflection;

namespace MergeConsumablesFika.Patches;

public class HostInventoryController_RunHostOperation_Patch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(HostInventoryController)
            .GetMethod("RunHostOperation", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [PatchPrefix]
    public static bool Prefix(HostInventoryController __instance, BaseInventoryOperationClass operation, Callback callback)
    {
        if (operation is MergeFoodOperation mergeFoodOperation)
        {
            var handler = __instance.GetHandler();
            handler.Set(__instance, mergeFoodOperation, callback);
            try
            {
                if (__instance.vmethod_0(handler.Operation))
                {
                    handler.Operation.method_1(handler.HandleResult);
                    var mergePacket = new MergePacket()
                    {
                        NetId = __instance.FikaPlayer.NetId,
                        CallbackId = operation.Id,
                        Type = 0,
                        MergeFoodOperation = mergeFoodOperation
                    };
                    Singleton<FikaServer>.Instance.SendData(ref mergePacket, DeliveryMethod.ReliableOrdered, true);
                    return false;
                }

                handler.Operation.Dispose();
                handler.Callback?.Fail($"Can't execute {handler.Operation}", 1);
                return false;
            }
            finally
            {
                __instance.ReturnHandler(handler);
            }
        }

        if (operation is MergeMedsOperation mergeMedsOperation)
        {
            var handler = __instance.GetHandler();
            handler.Set(__instance, mergeMedsOperation, callback);
            try
            {
                if (__instance.vmethod_0(handler.Operation))
                {
                    handler.Operation.method_1(handler.HandleResult);
                    var mergePacket = new MergePacket()
                    {
                        NetId = __instance.FikaPlayer.NetId,
                        CallbackId = operation.Id,
                        Type = 1,
                        MergeMedsOperation = mergeMedsOperation
                    };
                    Singleton<FikaServer>.Instance.SendData(ref mergePacket, DeliveryMethod.ReliableOrdered, true);
                    return false;
                }

                handler.Operation.Dispose();
                handler.Callback?.Fail($"Can't execute {handler.Operation}", 1);
            }
            finally
            {
                __instance.ReturnHandler(handler);
            }
        }

        return true;
    }
}
