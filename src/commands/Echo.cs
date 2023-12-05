/* ========================================================================== */
/*                                                                            */
/*                                                             /   /   \      */
/*   Made By IsCoffeeTho                                     /    |      \    */
/*                                                          |     |       |   */
/*   Echo.cs                                                |      \      |   */
/*                                                          |       |     |   */
/*   Last Edited: 01:33AM 06/12/2023                         \      |    /    */
/*                                                             \   /   /      */
/*                                                                            */
/* ========================================================================== */

using HarmonyLib;
using BetterLCTerminal;
using System.Linq;

namespace BLCT.command
{
	public class Echo : IProcess {

		int IProcess.Main(StdLib p, string[] args)
		{
			p.Write(1, args.Join(null, " "));
			return 0;
		}
	}
}