using HarmonyLib;
using BetterLCTerminal;
using BetterLCTerminal.stdlib;
using System.Collections.ObjectModel;

namespace BetterLCTerminal.command
{
	public class List : IProcess
	{
		string IProcess.Name => "ls";
		string IProcess.Description => "Lists Knowledge Files";
		
		public int Run(StdLib p, string[] args)
		{
			return 0;
		}
	}
}