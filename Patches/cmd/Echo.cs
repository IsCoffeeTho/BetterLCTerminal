using HarmonyLib;
using BetterLCTerminal;
using BetterLCTerminal.stdlib;
using System.Collections.ObjectModel;

namespace BetterLCTerminal.cmd
{
	public class Echo : IProcess {

		string IProcess.Name => "echo";
		string IProcess.Description => "Prints text back into terminal";

		public static readonly ReadOnlyCollection<string> Aliases = new(
			new string[] {
				"say"
			}
		);

		public int Run(process p, string[] args)
		{
			p.Print(args.Join(null, " "));
			return 0;
		}
	}
}