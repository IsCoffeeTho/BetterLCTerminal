using HarmonyLib;
using BetterLCTerminal;
using BetterLCTerminal.stdlib;
using System.Collections.ObjectModel;

namespace BetterLCTerminal.command
{
	public class Echo : IProcess {

		string IProcess.Name => "echo";
		string IProcess.Description => "Prints text back into terminal";

		public int Run(StdLib p, string[] args)
		{
			p.Print(args.Join(null, " "));
			return 0;
		}
	}
}