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
			TerminalMod.mls.LogDebug($"Making FileSystem");
			FileSystem = new();

			FileSystem.MkDir("/etc");
			FileSystem.Touch("/etc/hostname").Data = "Fortune9";
			FileSystem.Touch("/etc/username").Data = GameNetworkManager.Instance.localPlayerController.playerUsername;

			FileSystem.MkDir("/bin");

			Type[] potentialCommands = Assembly.GetExecutingAssembly().GetTypes()
				.Where(t => t != typeof(IProcess) && typeof(IProcess).IsAssignableFrom(t) && !t.IsAbstract)
				.ToArray();

			for (int i = 0; i < potentialCommands.Length; i++)
			{
				Type cmd = potentialCommands[i];
				if (cmd == null)
					continue;
				FileSystem.AssignProgram($"/bin/{cmd.Name.ToLower()}", (IProcess)Activator.CreateInstance(cmd));
			}

			TerminalMod.mls.LogDebug($"Completed FileSystem Creation");
		}
	}
}