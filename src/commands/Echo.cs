using HarmonyLib;
using BetterLCTerminal;
using System.Linq;

namespace Terminal.command
{
	public class Echo : IProcess {

		int IProcess.Main(StdLib p, string[] args)
		{
			p.Write(1, args.Join(null, " "));
			return 0;
		}
	}
}