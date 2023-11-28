using HarmonyLib;
using BetterLCTerminal;
using TMPro;
using System;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine.UIElements.Collections;

namespace BetterLCTerminal.Patches
{
	[HarmonyPatch(typeof(Terminal))]
	internal class TerminalPatch
	{
		// Speculation:
		// inputFieldText // where text goes to be displayed
		// screenText // where text comes in from presummably

		public static TerminalPatch instance;
		public Shell shell;
		public TemrinalMod BaseMod;

		public TerminalPatch()
		{
			instance.BaseMod = TemrinalMod.Instance;
			instance.shell = new();
		}

		[HarmonyPatch(typeof(Terminal), "Start")]
		[HarmonyPostfix]
		static void CreateInstance()
		{
			instance ??= new TerminalPatch();
		}

		[HarmonyPatch(typeof(Terminal), "BeginUsingTerminal")]
		[HarmonyPostfix]
		static void ConfigureTerminalObject(ref TMP_InputField __screenText) {
			float scale = instance.BaseMod.CFG_textsize.Value; // retrieves saved value
			__screenText.pointSize = 11 + (2 * (scale - 1));
			__screenText.caretWidth = 5;
			__screenText.caretBlinkRate = 0;
		}

		[HarmonyPatch(typeof(Terminal), "TextChanged")]
		[HarmonyPrefix]
		static bool GetText()
		{
			
			return true;
		}
	}
}