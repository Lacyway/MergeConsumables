using Comfort.Common;
using Fika.Core.Main.HostClasses;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib;
using MergeConsumables.Operations;
using SPT.Reflection.Patching;
using System.Reflection;
using static Fika.Core.Main.HostClasses.FikaHostInventoryController;

namespace MergeConsumablesFika.Patches;

public class HostInventoryController_RunHostOperation_Patch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(FikaHostInventoryController)
            .GetMethod("RunHostOperation", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [PatchPrefix]
    public static bool Prefix(FikaHostInventoryController __instance, BaseInventoryOperationClass operation, Callback callback)
    {
        if (operation is MergeFoodOperation mergeFoodOperation)
        {
            HostInventoryOperationHandler handler = new(__instance, operation, callback);
            if (__instance.vmethod_0(handler.operation))
            {
                handler.operation.method_1(handler.HandleResult);
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

            handler.operation.Dispose();
            handler.callback?.Fail($"Can't execute {handler.operation}", 1);
            return false;
        }

        if (operation is MergeMedsOperation mergeMedsOperation)
        {
            HostInventoryOperationHandler handler = new(__instance, operation, callback);
            if (__instance.vmethod_0(handler.operation))
            {
                handler.operation.method_1(handler.HandleResult);
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

            handler.operation.Dispose();
            handler.callback?.Fail($"Can't execute {handler.operation}", 1);
        }

        return true;
    }
}
