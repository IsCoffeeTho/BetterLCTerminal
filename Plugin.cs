/* ========================================================================== */
/*                                                                            */
/*                                                             /   /   \      */
/*   Made By IsCoffeeTho                                     /    |      \    */
/*                                                          |     |       |   */
/*   Plugin.cs                                              |      \      |   */
/*                                                          |       |     |   */
/*   Last Edited: 01:33AM 06/12/2023                         \      |    /    */
/*                                                             \   /   /      */
/*                                                                            */
/* ========================================================================== */

using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
		static void InstantiateShell(Terminal __instance)
		{
			try
			{
				float scale = Instance.CFG_textsize.Value; // retrieves saved value
				__instance.screenText.pointSize = scale;
				__instance.screenText.caretWidth = (int)(scale / 2f);
				__instance.screenText.caretBlinkRate = 0;
				__instance.inputFieldText.color = Color.white;
				__instance.terminalUIScreen.renderMode = RenderMode.ScreenSpaceOverlay;
				__instance.terminalUIScreen.scaleFactor = 2.25f;
				__instance.topRightText.enabled = false;
				try
				{
					// disables the backdrop of the money indicator
					__instance.terminalUIScreen.gameObject.transform
						.GetChild(0) // container
						.GetChild(5) // money indicator image
						.gameObject.GetComponent<Image>()
							.enabled = false;
					// may fail in the future
				}
				catch (Exception err)
				{
					_ = err; // discard
				}

				__term = null;
			}
			catch (Exception s)
			{
				mls.LogError($"Problem Creating Terminal: {s}");
			}
		}

		[HarmonyPatch(typeof(Terminal), "Update")]
		[HarmonyPrefix]
		static void ReplacedUpdate(Terminal __instance)
		{
			if (HUDManager.Instance == null || GameNetworkManager.Instance == null || GameNetworkManager.Instance.localPlayerController == null)
			{
				return;
			}
			if (__instance.terminalInUse)
			{
				if (Keyboard.current.anyKey.wasPressedThisFrame)
				{

				}
			}
		}

		[HarmonyPatch(typeof(Terminal), "BeginUsingTerminal")]
		[HarmonyPrefix]
		static void InitializeShellSession()
		{
			GameNetworkManager.Instance.localPlayerController.IsInspectingItem = true;
			// this disables camera movement
			try
			{
				if (__term == null)
				{
					mls.LogDebug("Terminal has been instantiated, creating a new shell");
					__term = new();
				}
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
			GameNetworkManager.Instance.localPlayerController.IsInspectingItem = false;
			// this re-enables camera movement
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