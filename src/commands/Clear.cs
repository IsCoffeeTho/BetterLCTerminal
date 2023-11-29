using BetterLCTerminal;

namespace BetterLCTerminal.command
{
	public class Clear : IProcess
	{
		int IProcess.Main(StdLib p, string[] args)
		{
			p.Write(1, "\x1b[H\x1b[3J");
			return 0;
		}
	}
}