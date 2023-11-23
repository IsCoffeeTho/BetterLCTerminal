using HarmonyLib;
using BetterLCTerminal;
using BetterLCTerminal.cmd;

namespace BetterLCTerminal.cmd
{
	public class Help : ICommand {

		string ICommand.Name => "help";

		string ICommand.Description => "Displays commands for user to use";

		public int Run(stdpcs process, string[] args)
		{
			
			return 0;
		}
	}
}