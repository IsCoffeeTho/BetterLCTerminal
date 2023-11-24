using HarmonyLib;
using BetterLCTerminal;
using BetterLCTerminal.cmd;
using System.Collections.ObjectModel;

namespace BetterLCTerminal.cmd
{
	public class Help : ICommand
	{
		string ICommand.Name => "help";

		public static readonly ReadOnlyCollection<string> Aliases = new(
			new string[] {
			}
		);

		string ICommand.Description => "Displays commands for user to use";

		public int Run(stdpcs process, string[] args)
		{
			
			return 0;
		}
	}
}