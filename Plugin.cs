/* ========================================================================== */
/*                                                                            */
/*                                                             /   /   \      */
/*   Made By IsCoffeeTho                                     /    |      \    */
/*                                                          |     |       |   */
/*   Plugin.cs                                              |      \      |   */
/*                                                          |       |     |   */
/*   Last Edited: 12:17AM 07/12/2023                         \      |    /    */
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
		static void RemovePreviousShell(Terminal __instance)
		{
			try
			{
				__term = null;
			}
			catch (Exception s)
			{
				mls.LogError($"Problem Creating Terminal: {s}");
			}
		}

		public void ChangeUI(Terminal _this)
		{
			float scale = Instance.CFG_textsize.Value; // retrieves saved value
			_this.screenText.pointSize = scale + 4f;
			_this.screenText.caretWidth = (int)(scale / 2f);
			_this.screenText.caretBlinkRate = 0;
			_this.inputFieldText.color = Color.white;
			_this.terminalUIScreen.renderMode = RenderMode.ScreenSpaceOverlay;
			_this.terminalUIScreen.scaleFactor = 1f;
			_this.topRightText.enabled = false;
			_this.screenText.image.color = Color.black;
			float ScreenAspect = (Screen.currentResolution.height - 100f) / 3f;
			RectTransform BackdropRect = _this.screenText.GetComponent<RectTransform>();
			
			// unhappy with result
			BackdropRect.anchorMax.Set(1f, 1f);
			BackdropRect.anchorMin.Set(0f, 0f);
			BackdropRect.sizeDelta.Set(ScreenAspect * 4, ScreenAspect * 3);
			BackdropRect.anchoredPosition.Set(0f, 0f);
			BackdropRect.anchoredPosition3D.Set(0f, 0f, -1f);
			
			_this.terminalUIScreen.gameObject.transform
				.GetChild(0)
				.GetChild(5)
				.gameObject.GetComponent<Image>()
					.enabled = false;
		}

		[HarmonyPatch(typeof(Terminal), "BeginUsingTerminal")]
		[HarmonyPrefix]
		static void InitializeShellSession(Terminal __instance)
		{
			GameNetworkManager.Instance.localPlayerController.IsInspectingItem = true;
			// this disables camera movement
			try
			{
				if (__term == null)
					__term = new();
				Instance.ChangeUI(__instance);
				__term.FileSystem.MkDir("/tmp");
				mls.LogDebug("Created Session");
			}
			catch (Exception s)
			{
				mls.LogError($"Problem starting Terminal Session: {s}");
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