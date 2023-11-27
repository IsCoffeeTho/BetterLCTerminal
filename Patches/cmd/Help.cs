using HarmonyLib;
using BetterLCTerminal;
using BetterLCTerminal.stdlib;
using System.Collections.ObjectModel;

namespace BetterLCTerminal.cmd
{
	public class Help : IProcess
	{
		string IProcess.Name => "help";

		public static readonly ReadOnlyCollection<string> Aliases = new(
			new string[] {
				"man"
			}
		);

		string IProcess.Description => "Displays commands for user to use";

		public int Run(process p, string[] args)
		{
			
			return 0;
		}
	}
}