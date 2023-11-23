using HarmonyLib;
using BetterLCTerminal;
using BetterLCTerminal.cmd;

namespace BetterLCTerminal.cmd
{
	public class Echo : ICommand {

		string ICommand.Name => "echo";

		string ICommand.Description => "Prints text back into terminal";

		public int Run(stdpcs process, string[] args)
		{
			process.Print(args.Join(null, " "));
			return 0;
		}
	}
}