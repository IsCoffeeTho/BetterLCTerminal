using HarmonyLib;
using BetterLCTerminal;
using BetterLCTerminal.stdlib;
using System.Collections.ObjectModel;
using System;

namespace BetterLCTerminal.command
{
	public class Cam : IProcess
	{
		string IProcess.Name => "cam";
		string IProcess.Description => "Runs the Camera Client";

		public int Run(StdLib p, string[] args)
		{
			p.Print("\x1b[H\x1b[3J");
			return 0;
		}
	}
}