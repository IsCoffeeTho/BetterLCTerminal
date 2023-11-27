using HarmonyLib;
using BetterLCTerminal;
using BetterLCTerminal.stdlib;
using System.Collections.ObjectModel;

namespace BetterLCTerminal.command
{
	public class Clear : IProcess
	{
		string IProcess.Name => "clear";
		string IProcess.Description => "Clears the screen";
		
		public static readonly ReadOnlyCollection<string> Aliases = new(
			new string[] {
				"cls"
			}
		);

		public int Run(StdLib p, string[] args)
		{
			p.Print("\x1b[H\x1b[3J");
			return 0;
		}
	}
}