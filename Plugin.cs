using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;

namespace BetterLCTerminal
{
	[BepInPlugin(MOD_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	[BepInProcess("Lethal Company.exe")]
	public class TerminalMod : BaseUnityPlugin
	{
		private const string MOD_GUID = "IsCoffeeTho.Terminal";
		private readonly Harmony harmony = new(MOD_GUID);
		public static TerminalMod Instance;
		public static Shell __term;
		public ManualLogSource mls;
		public ConfigEntry<int> CFG_textsize;
		public void Awake()
		{
			if (Instance == null)
				Instance = this;

			Instance.CFG_textsize = Config.Bind("Terminal",
				"TextSize", 12,
				"Font Size - min: 10"
			);
			Instance.CFG_textsize.Value = Math.Max(Instance.CFG_textsize.Value, 10);

			Instance.mls = BepInEx.Logging.Logger.CreateLogSource(MOD_GUID);
			Instance.mls.LogInfo("Plugin BetterLCTerminal is loaded!");
			Instance.mls.LogInfo("MOD_GUID: IsCoffeeTho.Terminal");

			harmony.PatchAll(typeof(TerminalMod));
		}

		[HarmonyPatch(typeof(Terminal), "Start")]
		[HarmonyPostfix]
		static void InstantiateShell(ref TMP_InputField ___screenText)
		{
			Instance.mls.LogMessage("Instancing terminal");
			float scale = Instance.CFG_textsize.Value; // retrieves saved value
			___screenText.pointSize = scale;
			___screenText.caretWidth = (int)(scale / 2f);
			___screenText.caretBlinkRate = 0;
			__term = new();
		}

		[HarmonyPatch(typeof(Terminal), "selectTextFieldDelayed")]
		[HarmonyPrefix]
		static bool InitializeShellSession()
		{
			Instance.mls.LogMessage("Opened terminal: creating session");
			if (HUDManager.Instance != null)
				HUDManager.Instance.ChangeControlTip(0, "Quit terminal : [TAB]", clearAllOther: true);
			__term.FileSystem.MkDir("/tmp");
			return true;
		}

		[HarmonyPatch(typeof(Terminal), "QuitTerminal")]
		[HarmonyPostfix]
		static void ExitShellSession()
		{
			Instance.mls.LogMessage("Closed terminal: exiting session");
			__term.FileSystem.Rm("/tmp");
		}
	}
}