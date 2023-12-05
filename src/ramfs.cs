/* ========================================================================== */
/*                                                                            */
/*                                                             /   /   \      */
/*   Made By IsCoffeeTho                                     /    |      \    */
/*                                                          |     |       |   */
/*   ramfs.cs                                               |      \      |   */
/*                                                          |       |     |   */
/*   Last Edited: 01:33AM 06/12/2023                         \      |    /    */
/*                                                             \   /   /      */
/*                                                                            */
/* ========================================================================== */

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine.UI;

namespace BetterLCTerminal.fs
{
	public class RamFS
	{
		public Dir Root;

		public RamFS()
		{
			Root = new();
		}

		private string SanitizePathName(string pathname)
		{
			string[] _ = new string[0];
			return SanitizePathName(pathname, out _);
		}
		private string SanitizePathName(string pathname, out string[] tree)
		{
			while (pathname.EndsWith("/") && pathname.Length != 1)
			{
				pathname = pathname.Substring(0, pathname.Length - 1);
			}
			string[] pathTree = pathname.Split("/");
			List<string> sanitizedTree = new();
			for (int i = 0; i < pathTree.Length; i++)
			{
				if (pathTree[i] == "..")
				{
					if (sanitizedTree.Count > 1)
						sanitizedTree.RemoveAt(sanitizedTree.Count);
				}
				else if (pathTree[i] == "." || pathTree[i] == "")
					continue;
				else
					sanitizedTree.Add(pathTree[i]);
			}
			tree = sanitizedTree.ToArray();
			return $"/{sanitizedTree.Join(null, "/")}";
		}
		private Stat LookupPathTree(string[] pathTree)
		{
			Stat current = Root;

			string resolvedPath = "/";

			for (int i = 0; i < pathTree.Length; i++)
			{
				if (current.Type != "Dir")
					throw new Exception($"ENOENT: {resolvedPath.Substring(0, resolvedPath.Length - 1)}");

				resolvedPath += $"{pathTree[i]}/";

				if (!((Dir)current).Entries.TryGetValue(pathTree[i], out current))
					throw new Exception($"ENOENT: {resolvedPath.Substring(0, resolvedPath.Length - 1)}");

				if (current.Type == "Link")// resolve link
				{
					if (current.Equals(((Link)current).Refer))
						throw new Exception($"ECYCLIC: {resolvedPath.Substring(0, resolvedPath.Length - 1)}");
					if (((Link)current).Refer.Type == "Link")
						throw new Exception($"ELNKV: {resolvedPath.Substring(0, resolvedPath.Length - 1)}");
					current = ((Link)current).Refer;
				}

			}
			return current;
		}
		private Stat SearchPathTree(string[] pathTree)
		{
			Stat current = Root;

			string resolvedPath = "/";

			for (int i = 0; i < pathTree.Length; i++)
			{
				if (current.Type == "Link")// resolve link
				{
					if (current.Equals(((Link)current).Refer))
						throw new Exception($"ECYCLIC: {resolvedPath.Substring(0, resolvedPath.Length - 1)}");
					if (((Link)current).Refer.Type == "Link")
						throw new Exception($"ELNKV: {resolvedPath.Substring(0, resolvedPath.Length - 1)}");
					current = ((Link)current).Refer;
					resolvedPath += $"{pathTree[i]}/";
					continue;
				}

				if (current.Type != "Dir")
					throw new Exception("ENOENT");

				resolvedPath += $"{pathTree[i]}/";

				if (!((Dir)current).Entries.TryGetValue(pathTree[i], out current))
					throw new Exception("ENOENT");
			}
			return current;
		}

		public void Rm(string pathname)
		{
			
			pathname = SanitizePathName(pathname, out string[] pathTree);
			if (!pathname.StartsWith("/"))
				throw new Exception($"EINVLD: {pathname}");

			string NameOfThingToRemove = pathTree.Last();
			if (pathTree.Length == 0)
				pathTree = new string[0];

			Stat ThingToRemove = SearchPathTree(pathTree);

			if (ThingToRemove.Type == "Dir")
			{
				Dir d = (Dir)ThingToRemove;
				string[] entryNames = d.Entries.Keys.ToArray();
				for (int i = 0; i < entryNames.Length; i++)
				{
					Rm($"{pathname}/{entryNames[i]}");
				}
			}
			ThingToRemove.Parent.Entries.Remove(NameOfThingToRemove);
		}

		public Stat StatPath(string pathname)
		{
			pathname = SanitizePathName(pathname, out string[] pathTree);
			if (!pathname.StartsWith("/"))
				throw new Exception($"EINVLD: {pathname}");

			return LookupPathTree(pathTree);
		}

		public Dir MkDir(string pathname)
		{
			pathname = SanitizePathName(pathname, out string[] pathTree);
			if (!pathname.StartsWith("/"))
				throw new Exception($"EINVLD: {pathname}");

			string dirName = pathTree.Last();
			string[] parentTree;
			if (pathTree.Length <= 1)
				parentTree = new string[0];
			else
			{
				parentTree = new string[pathTree.Length - 1];
				Array.Copy(pathTree, 0, parentTree, 0, pathTree.Length - 1);
			}

			Stat directoryToPlaceIn = LookupPathTree(parentTree); if (directoryToPlaceIn.Type != "Dir")
				throw new Exception($"ENOTDIR: /{parentTree.Join(null, "/")}");

			if (((Dir)directoryToPlaceIn).Entries.ContainsKey(dirName))
				throw new Exception($"EEXIST: {pathname}");

			Dir directoryToMake = new()
			{
				Parent = (Dir)directoryToPlaceIn
			};

			((Dir)directoryToPlaceIn).Entries.Add(dirName, directoryToMake);

			return directoryToMake;
		}

		public File Touch(string pathname)
		{
			pathname = SanitizePathName(pathname, out string[] pathTree);
			if (!pathname.StartsWith("/"))
				throw new Exception($"EINVLD: {pathname}");

			string fileName = pathTree.Last();
			string[] parentTree;
			if (pathTree.Length <= 1)
				parentTree = new string[0];
			else
			{
				parentTree = new string[pathTree.Length - 1];
				Array.Copy(pathTree, 0, parentTree, 0, pathTree.Length - 1);
			}

			Stat directoryToPlaceIn = LookupPathTree(parentTree);
			if (directoryToPlaceIn.Type != "Dir")
				throw new Exception($"ENOTDIR: /{parentTree.Join(null, "/")}");

			if (((Dir)directoryToPlaceIn).Entries.ContainsKey(fileName))
				throw new Exception($"EEXIST: {pathname}");

			File FileToTouch = new()
			{
				Parent = (Dir)directoryToPlaceIn
			};
			((Dir)directoryToPlaceIn).Entries.Add(fileName, FileToTouch);

			return FileToTouch;
		}

		public Link SymLink(string LinkFrom, string LinkTo)
		{
			LinkFrom = SanitizePathName(LinkFrom, out string[] pathTree);
			if (!LinkFrom.StartsWith("/"))
				throw new Exception($"EINVLD: {LinkFrom}");
			LinkTo = SanitizePathName(LinkTo, out string[] resultTree);
			if (!LinkTo.StartsWith("/"))
				throw new Exception($"EINVLD: {LinkTo}");

			string linkName = pathTree.Last();
			string[] parentTree;
			if (pathTree.Length <= 1)
				parentTree = new string[0];
			else
			{
				parentTree = new string[pathTree.Length - 1];
				Array.Copy(pathTree, 0, parentTree, 0, pathTree.Length - 1);
			}

			Stat directoryToPlaceIn = LookupPathTree(parentTree);
			if (directoryToPlaceIn.Type != "Dir")
				throw new Exception($"ENOTDIR: /{parentTree.Join(null, "/")}");
			if (((Dir)directoryToPlaceIn).Entries.ContainsKey(linkName))
				throw new Exception($"EEXIST: {LinkTo}");

			Link symbolicLink = new()
			{
				Parent = (Dir)directoryToPlaceIn,
			};
			((Dir)directoryToPlaceIn).Entries.Add(linkName, symbolicLink);

			Stat LinkedNode = LookupPathTree(resultTree);
			
			symbolicLink.Refer = LinkedNode;
			if (LinkedNode.Type == "Link")
				symbolicLink.Refer = ((Link)LinkedNode).Refer; // this will attempt to erase ELINKV

			return symbolicLink;
		}

		public Pipe FIFO(string pathname)
		{
			pathname = SanitizePathName(pathname, out string[] pathTree);
			if (!pathname.StartsWith("/"))
				throw new Exception($"EINVLD: {pathname}");

			string PipeName = pathTree.Last();
			string[] parentTree;
			if (pathTree.Length <= 1)
				parentTree = new string[0];
			else
			{
				parentTree = new string[pathTree.Length - 1];
				Array.Copy(pathTree, 0, parentTree, 0, pathTree.Length - 1);
			}

			Stat directoryToPlaceIn = LookupPathTree(parentTree);
			if (directoryToPlaceIn.Type != "Dir")
				throw new Exception($"ENOTDIR: /{parentTree.Join(null, "/")}");

			if (((Dir)directoryToPlaceIn).Entries.ContainsKey(PipeName))
				throw new Exception($"EEXIST: {pathname}");

			Pipe PipeToCreate = new()
			{
				Parent = (Dir)directoryToPlaceIn
			};
			((Dir)directoryToPlaceIn).Entries.Add(PipeName, PipeToCreate);

			return PipeToCreate;
		}
		public Program AssignProgram(string pathname, IProcess program)
		{
			pathname = SanitizePathName(pathname, out string[] pathTree);
			if (!pathname.StartsWith("/"))
				throw new Exception($"EINVLD: {pathname}");
			
			string PipeName = pathTree.Last();
			string[] parentTree;
			if (pathTree.Length <= 1)
				parentTree = new string[0];
			else
			{
				parentTree = new string[pathTree.Length - 1];
				Array.Copy(pathTree, 0, parentTree, 0, pathTree.Length - 1);
			}

			Stat directoryToPlaceIn = LookupPathTree(parentTree);
			if (directoryToPlaceIn.Type != "Dir")
				throw new Exception($"ENOTDIR: /{parentTree.Join(null, "/")}");

			if (((Dir)directoryToPlaceIn).Entries.ContainsKey(PipeName))
				throw new Exception($"EEXIST: {pathname}");

			Program ProgramFile = new()
			{
				Routine = program,
				Parent = (Dir)directoryToPlaceIn
			};
			((Dir)directoryToPlaceIn).Entries.Add(PipeName, ProgramFile);

			return ProgramFile;
		}
	}

	public class Dir : Stat
	{
		public new int Size => Entries.Count;
		public Dictionary<string, Stat> Entries = new();
	}

	public class Link : Stat
	{
		public new int Size = 1;
		public Stat Refer = null;
	}

	public class Pipe : Stat
	{
		public new int Size = 256;
		public char[] Buffer = new char[256];
		public byte write_idx = 0;
		public byte read_idx = 0;
	}

	public class Program : Stat
	{
		public new int Size => 1;

		public IProcess Routine = null;
	}

	public class File : Stat
	{
		public new int Size => Data.Length;

		public string Data = "";
	}

	public class Stat
	{
		public int Size => 0;
		public string Type
		{
			get
			{
				return GetType().Name;
			}
		}
		public Dir Parent = null;
	}
}