using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using BetterLCTerminal.stdlib;
using HarmonyLib;

namespace BetterLCTerminal
{
	public class Shell
	{
		private string ANSI_BUFFER = "";
		private bool ANSI_TOGGLE = false;

		public List<IProcess> ListOfCommands = new();

		public Dictionary<string, IProcess> invokeStringLUT = new();

		public string Hostname => "DeadMan1";

		public Shell()
		{
			ANSI_BUFFER = "";
			ANSI_TOGGLE = false;

			var commands = Assembly.GetExecutingAssembly().GetTypes().Where(p =>
				p.Namespace == "BetterLCTerminal.command"
			).ToArray();

			for (int i = 0; i < commands.Length; i++)
			{
				ConstructorInfo command = commands[i].GetConstructors()[0];
				ListOfCommands.Append(command.Invoke(null));
			}

			for (int i = 0; i < ListOfCommands.Count; i++)
			{
				IProcess command = ListOfCommands[i];
				invokeStringLUT.Add(command.Name, command);
			}
		}

		public int ExecuteCommand(string[] v)
		{
			if (invokeStringLUT.ContainsKey(v[0]))
			{
				StdLib p = new();

				p.stdout.OnData += ANSI_translate;
				p.stderr.OnData += ANSI_translate;

				return invokeStringLUT.GetValueSafe(v[0]).Run(p, v);
			}
			return 1;
		}

		public void ANSI_translate(object sender, stdlib.FD_OnData_args args)
		{
			char[] char_vec = args.Data.ToCharArray();
			string Translated = "";
			for (int i = 0; i < char_vec.Length; i++)
			{
				if (ANSI_TOGGLE)
				{
					ANSI_BUFFER += char_vec[i];
					// if ("".Contains(char_vec[i])) {
						
					// }
				}
				else
				{
					if (char_vec[i] == '\x1b')
					{
						ANSI_BUFFER = "";
						Translated += "^[";
						// ANSI_TOGGLE = true;
					}
					else
					{
						Translated += char_vec[i];
						// place character on screen
					}
				}
			}
		}
	}
}