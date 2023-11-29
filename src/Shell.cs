using BetterLCTerminal.fs;

namespace BetterLCTerminal {
	public class Shell {
		RamFS FileSystem;

		public Shell() {
			FileSystem = new();
			FileSystem.MkDir("/etc");
			FileSystem.Touch("/etc/hostname").Data = "";
			FileSystem.Touch("/etc/username").Data = GameNetworkManager.Instance.localPlayerController.playerUsername;

		}
	}
}