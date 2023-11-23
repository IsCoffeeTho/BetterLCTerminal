using HarmonyLib;
using BetterLCTerminal;
using TMPro;
using System;

namespace BetterLCTerminal.Patches
{
	[HarmonyPatch(typeof(Terminal))]
	internal class TerminalPatch
	{
		// TODO: better command system
		// 
		// inputFieldText // where text goes to be displayed
		// screenText // where text comes in from presummably

		public static TerminalPatch instance;
		public cmd.Env Environment = new();

		public TerminalPatch()
		{
			Environment.Cmds.Add("help", new cmd.Help());
			// Environment.Cmds.Add("ls", new cmd.Bestiary());
		}

		[HarmonyPatch("Start")]
		[HarmonyPostfix]
		static void ConfigureTerminalObject(ref TextMeshProUGUI ___inputFieldText, ref TerminalNode ___currentNode)
		{
			if (instance == null)
				instance = new TerminalPatch();
			___inputFieldText.richText = true;
			
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