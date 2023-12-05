/* ========================================================================== */
/*                                                                            */
/*                                                             /   /   \      */
/*   Made By IsCoffeeTho                                     /    |      \    */
/*                                                          |     |       |   */
/*   Clear.cs                                               |      \      |   */
/*                                                          |       |     |   */
/*   Last Edited: 01:33AM 06/12/2023                         \      |    /    */
/*                                                             \   /   /      */
/*                                                                            */
/* ========================================================================== */

using BetterLCTerminal;

namespace BLCT.command
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