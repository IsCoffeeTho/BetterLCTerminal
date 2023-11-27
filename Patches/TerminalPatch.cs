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
		public stdlib.Env Environment = new();
		private string ANSI_BUFFER = "";

		private bool ANSI_TOGGLE = false;

		public TemrinalMod BaseMod;

		public TerminalPatch()
		{
			instance.BaseMod = TemrinalMod.Instance;
			// TODO: add more commands
			Environment.Cmds.AddItem(new cmd.Help());
			Environment.Cmds.AddItem(new cmd.Echo());
			Environment.Cmds.AddItem(new cmd.Clear());
			Environment.Cmds.AddItem(new cmd.Cam());
			Environment.Cmds.AddItem(new cmd.Cat());
			Environment.Cmds.AddItem(new cmd.List());
			Environment.calculateLuT();
		}

		public int ExecuteCommand(string[] v)
		{
			if (Environment.CommandLuT.ContainsKey(v[0]))
			{
				stdlib.process p = new();
				p.stdout.OnData += ANSI_translate;
				p.stderr.OnData += ANSI_translate;
				// hook environment to terminal
				return Environment.CommandLuT.Get(v[0]).Run(p, v);
			}
			return -1;
		}

		static void ANSI_translate(object sender, stdlib.FD_OnData_args args)
		{
			char[] char_vec = args.Data.ToCharArray();
			for (int i = 0; i < char_vec.Length; i++) {
				if (instance.ANSI_TOGGLE) {
					instance.ANSI_BUFFER += char_vec[i];
					if ("".Contains(char_vec[i])) {

					}
				} else {
					if (char_vec[i] == '\x1b') {
						instance.ANSI_BUFFER = "";
						instance.ANSI_TOGGLE = true;
					} else {
						// place character on screen
					}
				}
			}
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
		static void TEST_ReplaceText(ref TerminalNode ___currentNode, ref string ___currentText)
		{
			___currentText = "TERM";
			___currentNode.displayText = "TERM";
		}

	}
}