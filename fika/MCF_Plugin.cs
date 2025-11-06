using BepInEx;
using BepInEx.Logging;

namespace MergeConsumables;

[BepInPlugin("com.lacyway.mcf", "MergeConsumablesFika", "1.4.0")]
internal class MCF_Plugin : BaseUnityPlugin
{
    internal static ManualLogSource MC_Logger;

    protected void Awake()
    {
        MC_Logger = Logger;

        MC_Logger.LogInfo($"{nameof(MCF_Plugin)} has been loaded.");
    }
}
