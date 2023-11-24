using HarmonyLib;
using BetterLCTerminal;
using BetterLCTerminal.cmd;
using System.Collections.ObjectModel;

namespace BetterLCTerminal.cmd
{
	public class Clear : ICommand
	{
		string ICommand.Name => "clear";
		string ICommand.Description => "Clears the screen";

		public static readonly ReadOnlyCollection<string> Aliases = new(
			new string[] {
				"cls"
			}
		);

		public int Run(stdpcs process, string[] args)
		{
			process.Print("\x1b[H\x1b[3J");
			return 0;
		}
	}
}