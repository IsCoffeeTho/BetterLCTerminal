using HarmonyLib;
using BetterLCTerminal;
using BetterLCTerminal.stdlib;
using System.Collections.ObjectModel;

namespace BetterLCTerminal.command
{
	public class Help : IProcess
	{
		string IProcess.Name => "help";
		string IProcess.Description => "Displays commands for user to use";

		public int Run(StdLib p, string[] args)
		{
			
			return 0;
		}
	}
}