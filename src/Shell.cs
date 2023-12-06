/* ========================================================================== */
/*                                                                            */
/*                                                             /   /   \      */
/*   Made By IsCoffeeTho                                     /    |      \    */
/*                                                          |     |       |   */
/*   Shell.cs                                               |      \      |   */
/*                                                          |       |     |   */
/*   Last Edited: 01:07PM 06/12/2023                         \      |    /    */
/*                                                             \   /   /      */
/*                                                                            */
/* ========================================================================== */

using System;
using System.Linq;
using System.Reflection;
using BetterLCTerminal;
using BetterLCTerminal.fs;

namespace BetterLCTerminal
{
	public class Shell
	{
		public RamFS FileSystem;

		public Shell()
		{
			try
			{
				TerminalMod.mls.LogDebug($"Making FileSystem");
				FileSystem = new();
				TerminalMod.mls.LogDebug($"RAMFS >> {FileSystem}");
				TerminalMod.mls.LogDebug($"making /etc");
				FileSystem.MkDir("/etc");
				FileSystem.Touch("/etc/hostname").Data = "Fortune9";
				TerminalMod.mls.LogDebug($"making /var");
				FileSystem.MkDir("/var");
				FileSystem.Touch("/var/path").Data = "/bin:/sbin:/usr/sbin:/usr/bin";
				FileSystem.MkDir("/var/run");
				TerminalMod.mls.LogDebug($"making /run");
				FileSystem.SymLink("/run", "/var/run");
				FileSystem.Touch("/run/username").Data = GameNetworkManager.Instance.localPlayerController.playerUsername;
				TerminalMod.mls.LogDebug($"making /bin and /usr/bin");
				FileSystem.MkDir("/bin");
				FileSystem.MkDir("/usr");
				FileSystem.MkDir("/usr/bin");

				TerminalMod.mls.LogDebug($"RETRIEVING RUNTIME IPROCESS");

				Type[] potentialCommands = Assembly.GetExecutingAssembly().GetTypes()
					.Where(t => t != typeof(IProcess) && typeof(IProcess).IsAssignableFrom(t) && !t.IsAbstract)
					.ToArray();

				for (int i = 0; i < potentialCommands.Length; i++)
				{
					Type cmd = potentialCommands[i];
					if (cmd == null)
						continue;
					if (cmd.Namespace == "BLCT.command")
					{
						FileSystem.AssignProgram($"/bin/{cmd.Name.ToLower()}", (IProcess)Activator.CreateInstance(cmd));
						continue;
					}
					FileSystem.AssignProgram($"/usr/bin/{cmd.Name.ToLower()}", (IProcess)Activator.CreateInstance(cmd));

				}

				TerminalMod.mls.LogDebug($"making /home");
				FileSystem.MkDir("/home");
				FileSystem.MkDir($"/home/{GameNetworkManager.Instance.localPlayerController.playerUsername}");
				TerminalMod.mls.LogDebug($"Completed FileSystem Creation");
			}
			catch (Exception s)
			{
				throw new Exception($"Couldn't create Shell class", s);
			}
		}
	}
}