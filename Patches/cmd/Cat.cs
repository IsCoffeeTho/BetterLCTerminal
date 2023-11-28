using HarmonyLib;
using BetterLCTerminal;
using BetterLCTerminal.stdlib;
using System.Collections.ObjectModel;

namespace BetterLCTerminal.command
{
	public class Cat : IProcess
	{
		string IProcess.Name => "cat";
		string IProcess.Description => "Reads out a file entry";

		public int Run(StdLib p, string[] args)
		{
			
			return 0;
		}
	}
}