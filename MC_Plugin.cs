using BepInEx;
using BepInEx.Logging;

namespace MergeConsumables
{
	[BepInPlugin("com.lacyway.mc", "MergeConsumables", "1.3.0")]
	internal class MC_Plugin : BaseUnityPlugin
	{
		internal static ManualLogSource MC_Logger;

		protected void Awake()
		{
			MC_Logger = Logger;

			MC_Logger.LogInfo($"{nameof(MC_Plugin)} has been loaded.");
			new MC_Patches().Enable();
		}
	}
}
