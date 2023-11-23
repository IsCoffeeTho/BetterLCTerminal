using HarmonyLib;
using BetterLCTerminal;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace BetterLCTerminal.cmd
{
	public class fileDescriptor {
		private string buffer = "";
		public bool raw = false;

		public void Write(string text) {
			if (!raw) {
				char[] charByChar = text.ToCharArray();
				for (int i = 0; i < charByChar.Length; i++)
					addToBuffer(charByChar[i]);
			} else {
				OnData(text);
			}
		}

		void addToBuffer(char c) {
			buffer.Append(c);
			if (c != '\n') {
				OnData(buffer);
				buffer = "";
			}
			return;
		}

		virtual public void OnData(string text) {

		}
	}

	public class Env {
		public Dictionary<string, string> Vars = new Dictionary<string, string>();
		public Dictionary<string, ICommand> Cmds = new Dictionary<string, ICommand>();
	}
	public class stdpcs {
		protected fileDescriptor stdin = new fileDescriptor();
		protected fileDescriptor stdout = new fileDescriptor();
		protected fileDescriptor stderr = new fileDescriptor();
		public void Print(string text) {
			stdout.Write(text);
		}
		public Env environment;
	}
	public interface ICommand {
		public string Name {get;}
		public string Description {get;}
		int Run(stdpcs process, string[] args);
	}
}