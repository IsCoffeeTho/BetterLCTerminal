using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BetterLCTerminal.stdlib
{
	public class StdLib
	{
		public FileDescriptor stdin = new();
		public FileDescriptor stdout = new();
		public FileDescriptor stderr = new();
		public void Print(string text)
		{
			stdout.Write(text);
			stdout.Write("\n");
		}
	}
	public interface IProcess
	{
		public string Name { get; }
		public string Description { get; }
		int Run(StdLib p, string[] args);
	}
}