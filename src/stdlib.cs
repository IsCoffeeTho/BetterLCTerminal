/* ========================================================================== */
/*                                                                            */
/*                                                             /   /   \      */
/*   Made By IsCoffeeTho                                     /    |      \    */
/*                                                          |     |       |   */
/*   stdlib.cs                                              |      \      |   */
/*                                                          |       |     |   */
/*   Last Edited: 11:41PM 06/12/2023                         \      |    /    */
/*                                                             \   /   /      */
/*                                                                            */
/* ========================================================================== */

using System;
using UnityEngine.UI;

namespace BetterLCTerminal
{
	public class StdLib
	{
		private IFileDescriptor[] FileDescriptorTable = new IFileDescriptor[256];
		public StdLib()
		{
			
		}

		public int Write(int fd, string text)
		{
			if (FileDescriptorTable[fd] == null)
				throw new Exception("Invalid File Descriptor");
			FileDescriptorTable[fd].Write(text);
			return text.Length;
		}

		public string Read(int fd)
		{
			if (FileDescriptorTable[fd] == null)
				throw new Exception("Invalid File Descriptor");
			return FileDescriptorTable[fd].Read();
		}

		public void Exit(int exitCode = 0) {
			
		}

		// public int Open(string pathname) // relative
		// {
			
		// }
	}

	interface IFileDescriptor
	{
		bool Available { get; set; }
		bool Awaiting { get; set; }
		string Read();
		void Write(string text);
	}

	public interface IProcess
	{
		int Main(StdLib p, string[] args);
	}
}