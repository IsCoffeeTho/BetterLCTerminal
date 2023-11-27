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

		[HarmonyPatch("Start")]
		[HarmonyPostfix]
		static void ConfigureTerminalObject(ref TMP_InputField __screenText)
		{
			instance ??= new TerminalPatch();
			__screenText.pointSize = 11 + (2 * (instance.BaseMod.CFG_textsize.Value - 1));
			__screenText.caretWidth = 5;
			__screenText.caretBlinkRate = 0;
		}

		[HarmonyPatch("TextChanged")]
		[HarmonyPrefix]
		static bool GetText()
		{
			
			return true;
		}
	}
}