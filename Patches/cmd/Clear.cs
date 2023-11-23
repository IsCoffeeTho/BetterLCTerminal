using HarmonyLib;
using BetterLCTerminal;
using BetterLCTerminal.cmd;

namespace BetterLCTerminal.cmd
{
	public class Clear : ICommand
	{
		string ICommand.Name => "clear";

		public string[] Aliases = new string[10];

		string ICommand.Description => "Clears the screen";

		public int Run(stdpcs process, string[] args)
		{
			process.Print(args.Join(null, " "));
			return 0;
		}

		public Clear() {
			Aliases[0] = "cls";
		} 
	}
}