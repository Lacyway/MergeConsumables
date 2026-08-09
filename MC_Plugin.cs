using BepInEx;
using BepInEx.Logging;
using EFT.BinarySerialization;
using HarmonyLib;
using MergeConsumables.Descriptors;
using MergeConsumables.Patches;

namespace MergeConsumables;

[BepInPlugin("com.lacyway.mc", "MergeConsumables", "1.6.1")]
internal sealed class MC_Plugin : BaseUnityPlugin
{
    internal static ManualLogSource MC_Logger;

    private void Awake()
    {
        MC_Logger = Logger;

        MC_Logger.LogInfo($"{nameof(MC_Plugin)} has been loaded.");
        new ConvertOperationResultToOperation_Patch().Enable();
        new ExecutePossibleAction_Patch().Enable();
        var harmony = new Harmony("com.lacyway.mc");
        harmony.PatchAll();
        BinarySerializationMirrorExtensions._types.AddRange([typeof(MergeFoodDescriptor), typeof(MergeMedsDescriptor)]);
    }
}
