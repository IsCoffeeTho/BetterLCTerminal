using HarmonyLib;
using BetterLCTerminal;
using TMPro;
using System;
using BepInEx;
using BepInEx.Configuration;

namespace BetterLCTerminal.Patches
{
	[HarmonyPatch(typeof(Terminal))]
	internal class TerminalPatch
	{
		// Speculation:
		// inputFieldText // where text goes to be displayed
		// screenText // where text comes in from presummably

		public static TerminalPatch instance;
		public cmd.Env Environment = new();

		public TemrinalMod BaseMod;
		
		public TerminalPatch()
		{
			instance.BaseMod = TemrinalMod.Instance;
			// TODO: add more commands
			Environment.Cmds.AddItem(new cmd.Help());

			Environment.calculateLuT();
			// Environment.Cmds.Add("ls", new cmd.Bestiary());
		}

		[HarmonyPatch("Start")]
		[HarmonyPostfix]
		static void ConfigureTerminalObject(ref TextMeshProUGUI ___inputFieldText)
		{
			if (instance == null)
				instance = new TerminalPatch();
			// ___inputFieldText.fontSize = instance.BaseMod.CFG_textsize.Value;

		}

		[HarmonyPatch("selectTextFieldDelayed")]
		[HarmonyPrefix]
		static bool selectTextFieldDelayed(ref TMP_InputField ___screenText)
		{
			___screenText.ActivateInputField();
			___screenText.Select();
			return false;
		}

		[HarmonyPatch("BeginUsingTerminal")]
		[HarmonyPostfix]
		static void ReplaceText(ref TerminalNode ___currentNode, ref string ___currentText)
		{
			___currentText = "TERM";
			___currentNode.displayText = "TERM";
		}

	}
}