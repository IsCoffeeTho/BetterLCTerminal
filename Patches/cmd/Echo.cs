using HarmonyLib;
using BetterLCTerminal;
using BetterLCTerminal.cmd;
using System.Collections.ObjectModel;

namespace BetterLCTerminal.cmd
{
	public class Echo : ICommand {

		string ICommand.Name => "echo";
		string ICommand.Description => "Prints text back into terminal";

		public static readonly ReadOnlyCollection<string> Aliases = new(
			new string[] {
			}
		);

		public int Run(stdpcs process, string[] args)
		{
			process.Print(args.Join(null, " "));
			return 0;
		}
	}
}