using System;
using System.Linq;
using System.Reflection;
using BetterLCTerminal.fs;

namespace BetterLCTerminal
{
	public class Shell
	{
		public RamFS FileSystem;

		public Shell()
		{
			FileSystem = new();
			FileSystem.MkDir("/etc");
			FileSystem.Touch("/etc/hostname").Data = "Fortune9";
			FileSystem.Touch("/etc/username").Data = GameNetworkManager.Instance.localPlayerController.playerUsername;

			FileSystem.MkDir("/bin");

			Type[] potentialCommands = Assembly.GetExecutingAssembly().GetTypes()
				.Where(t => string.Equals(t.Namespace, "BetterLCTerminal.command", StringComparison.Ordinal))
				.ToArray();

			for (int i = 0; i < potentialCommands.Length; i++)
			{
				Type cmd = potentialCommands[i];
				if (cmd == null)
					continue;
				if (typeof(IProcess).IsAssignableFrom(cmd)) // is a valid command
					FileSystem.AssignProgram($"/bin/{cmd.Name.ToLower()}", (IProcess)Activator.CreateInstance(cmd)); // line needs a rewrite maybe
			}
		}
	}
}