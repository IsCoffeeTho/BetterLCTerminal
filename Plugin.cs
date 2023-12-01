using System;
using System.Collections;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;

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
		public static ManualLogSource mls;
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

			mls = BepInEx.Logging.Logger.CreateLogSource(MOD_GUID);
			mls.LogInfo("Plugin BetterLCTerminal is loaded!");
			mls.LogInfo("MOD_GUID: IsCoffeeTho.Terminal");

			harmony.PatchAll(typeof(TerminalMod));
		}

		[HarmonyPatch(typeof(Terminal), "Start")]
		[HarmonyPostfix]
		static void InstantiateShell(ref TMP_InputField ___screenText)
		{
			mls.LogDebug("Terminal is present, creating a new shell");
			__term = new();
			mls.LogDebug(__term);
			float scale = Instance.CFG_textsize.Value; // retrieves saved value
			___screenText.pointSize = scale;
			___screenText.caretWidth = (int)(scale / 2f);
			___screenText.caretBlinkRate = 0;
		}

		[HarmonyPatch(typeof(Terminal), "BeginUsingTerminal")]
		[HarmonyPrefix]
		static void InitializeShellSession()
		{
			try
			{
				__term.FileSystem.MkDir("/tmp");
				mls.LogDebug("Created Session");
			}
			catch (Exception s)
			{
				mls.LogError($"Problem starting Terminal Session: {s}");
			}
		}

		[HarmonyPatch(typeof(Terminal), "QuitTerminal")]
		[HarmonyPostfix]
		static void ExitShellSession()
		{
			try
			{
				__term.FileSystem.Rm("/tmp");
				mls.LogDebug("Destroyed Session");
			}
			catch (Exception s)
			{
				mls.LogError($"Problem stopping Terminal Session: {s}");
			}
		}
	}
}