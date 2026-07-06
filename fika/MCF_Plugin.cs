using BepInEx;
using BepInEx.Logging;

namespace MergeConsumablesFika;

[BepInDependency("com.fika.core", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.lacyway.mc", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin("com.lacyway.mcf", "MergeConsumablesFika", "1.1.0")]
internal class MCF_Plugin : BaseUnityPlugin
{
    internal static ManualLogSource MC_Logger;

    protected void Awake()
    {
        MC_Logger = Logger;
        MC_Logger.LogInfo($"{nameof(MCF_Plugin)} has been loaded.");
        MergeConsumablesSerialization.AddMergeConsumableTypes(MC_Logger);
    }
}
