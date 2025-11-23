using BepInEx;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
#if DEBUG
using EFT.UI;
#endif
using Fika.Core.Modding;
using Fika.Core.Modding.Events;
using Fika.Core.Networking;
using Fika.Core.Networking.LiteNetLib;
using Fika.Core.Networking.LiteNetLib.Utils;
using Fika.Core.Networking.Packets.Communication;
using Fika.Core.Networking.Packets.Generic;
using Fika.Core.Networking.Packets.Generic.SubPackets;
using MergeConsumables.Operations;
using MergeConsumablesFika.Patches;
using System;

namespace MergeConsumablesFika;

[BepInPlugin("com.lacyway.mcf", "MergeConsumablesFika", "1.0.0")]
internal class MCF_Plugin : BaseUnityPlugin
{
    internal static ManualLogSource MC_Logger;

    protected void Awake()
    {
        MC_Logger = Logger;

        MC_Logger.LogInfo($"{nameof(MCF_Plugin)} has been loaded.");

        new ClientInventoryController_RunClientOperation_Patch()
            .Enable();
        new HostInventoryController_RunHostOperation_Patch()
            .Enable();

        FikaEventDispatcher.SubscribeEvent<FikaNetworkManagerCreatedEvent>(OnNetworkManagerCreated);
    }

    private void OnNetworkManagerCreated(FikaNetworkManagerCreatedEvent createdEvent)
    {
        if (createdEvent.Manager is FikaServer fikaServer)
        {
            fikaServer.RegisterPacket<MergePacket, NetPeer>(OnServerMergePacketReceived);
        }
        else if (createdEvent.Manager is FikaClient fikaClient)
        {
            fikaClient.RegisterPacket<MergePacket>(OnClientMergePacketReceived);
        }
        else
        {
            Logger.LogError($"The manager was of incorrect type: {createdEvent.Manager.GetType().Name}");
        }
    }

    private void OnClientMergePacketReceived(MergePacket packet)
    {
        var client = Singleton<FikaClient>.Instance;
        if (client.CoopHandler.Players.TryGetValue(packet.NetId, out var player))
        {
            var controller = player.InventoryController;
            if (controller != null)
            {
                try
                {
                    if (controller is Interface18 networkController)
                    {
                        var result = networkController.CreateOperationFromDescriptor(packet.Type == 0 ? packet.MergeFoodDescriptor : packet.MergeMedsDescriptor);
                        if (!result.Succeeded)
                        {
                            Logger.LogError($"ConvertInventoryPacket::Unable to process descriptor from netId {packet.NetId}, error: {result.Error}");
                            return;
                        }

                        client.InventoryOperations.Enqueue(result.Value);
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogError($"ConvertInventoryPacket::Exception thrown: {exception}");
                }
            }
            else
            {
                Logger.LogError("ConvertInventoryPacket: inventory was null!");
            }
        }
    }

    private void OnServerMergePacketReceived(MergePacket packet, NetPeer peer)
    {
        var server = Singleton<FikaServer>.Instance;
        if (server.CoopHandler.Players.TryGetValue(packet.NetId, out var playerToApply))
        {
            try
            {
                if (playerToApply.InventoryController is Interface18 inventoryController)
                {
                    var result = inventoryController.CreateOperationFromDescriptor(packet.Type == 0 ? packet.MergeFoodDescriptor : packet.MergeMedsDescriptor);
#if DEBUG
                    if (result.Succeeded)
                    {
                        ConsoleScreen.Log($"Received InvOperation: {result.Value.GetType().Name}, Id: {result.Value.Id}");
                    }
#endif
                    if (result.Failed)
                    {
                        Logger.LogError($"ItemControllerExecutePacket::Operation conversion failed: {result.Error}");
                        server.SendGenericPacketToPeer(EGenericSubPacketType.OperationCallback,
                            OperationCallbackPacket.FromValue(packet.NetId, packet.CallbackId, EOperationStatus.Failed, result.Error.ToString()), peer);

                        ResyncInventoryIdPacket resyncPacket = new(playerToApply.NetId);
                        server.SendDataToPeer(ref resyncPacket, DeliveryMethod.ReliableOrdered, peer);
                        return;
                    }

                    FikaServer.InventoryOperationHandler handler = new(result, packet.CallbackId, packet.NetId, peer, server);
                    server.SendGenericPacketToPeer(EGenericSubPacketType.OperationCallback,
                            OperationCallbackPacket.FromValue(packet.NetId, packet.CallbackId, EOperationStatus.Started), peer);

                    if (packet.Type == 0)
                    {
                        packet.MergeFoodOperation = (MergeFoodOperation)result.Value;
                        packet.MergeFoodDescriptor = null;
                    }
                    else
                    {
                        packet.MergeMedsOperation = (MergeMedsOperation)result.Value;
                        packet.MergeMedsDescriptor = null;
                    }
                    server.SendData(ref packet, DeliveryMethod.ReliableOrdered, peer);
                    handler.OperationResult.Value.method_1(handler.HandleResult);
                }
                else
                {
                    throw new InvalidTypeException($"Inventory controller was not of type {nameof(Interface18)}!");
                }
            }
            catch (Exception exception)
            {
                Logger.LogError($"ItemControllerExecutePacket::Exception thrown: {exception}");
                server.SendGenericPacketToPeer(EGenericSubPacketType.OperationCallback,
                            OperationCallbackPacket.FromValue(packet.NetId, packet.CallbackId, EOperationStatus.Failed, exception.Message), peer);

                ResyncInventoryIdPacket resyncPacket = new(playerToApply.NetId);
                server.SendDataToPeer(ref resyncPacket, DeliveryMethod.ReliableOrdered, peer);
            }
        }
    }
}
