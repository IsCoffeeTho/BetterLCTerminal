using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace BetterLCTerminal
{
	[BepInPlugin(MOD_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	[BepInProcess("Lethal Company.exe")]
	public class TemrinalMod : BaseUnityPlugin
	{
		private const string MOD_GUID = "IsCoffeeTho.Terminal";
		private readonly Harmony harmony = new Harmony(MOD_GUID);
		public static TemrinalMod Instance;
		internal ManualLogSource mls;
		private void Awake()
		{
			if (Instance == null)
				Instance = this;

			mls = BepInEx.Logging.Logger.CreateLogSource(MOD_GUID);

			mls.LogInfo("Plugin BetterLCTerminal is loaded! MOD_GUID: 'IsCoffeeTho.Terminal'");

			harmony.PatchAll(typeof(TemrinalMod));
		}
	}
}
