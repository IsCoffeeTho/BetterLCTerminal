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
			while (pathname.EndsWith("/") && pathname.Length != 1)
			{
				pathname = pathname.Substring(0, pathname.Length - 1);
			}
			return pathname;
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
			pathname = SanitizePathName(pathname);
			if (!pathname.StartsWith("/"))
				throw new Exception($"ENOENT: {pathname}");

			string[] pathTree = pathname[1..].Split("/");
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
			pathname = SanitizePathName(pathname);
			if (!pathname.StartsWith("/"))
				throw new Exception($"ENOENT: {pathname}");

			string[] pathTree = pathname[1..].Split("/");
			if (pathTree.Length == 0)
				pathTree = new string[0];

			return LookupPathTree(pathTree);
		}

		public Dir MkDir(string pathname)
		{
			TerminalMod.Instance.mls.LogDebug($"MKDIR {pathname}");
			pathname = SanitizePathName(pathname);
			if (!pathname.StartsWith("/"))
				throw new Exception($"ENOENT: {pathname}");

			string[] pathTree = pathname[1..].Split("/");
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
			TerminalMod.Instance.mls.LogDebug($"TOUCH {pathname}");
			pathname = SanitizePathName(pathname);
			if (!pathname.StartsWith("/"))
				throw new Exception($"ENOENT: {pathname}");

			string[] pathTree = pathname[1..].Split("/");
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
			TerminalMod.Instance.mls.LogDebug($"Link {LinkFrom}->{LinkTo}");
			LinkFrom = SanitizePathName(LinkFrom);
			if (!LinkFrom.StartsWith("/"))
				throw new Exception($"ENOENT: {LinkFrom}");
			LinkTo = SanitizePathName(LinkTo);
			if (!LinkTo.StartsWith("/"))
				throw new Exception($"EINVLD: {LinkTo}");

			string[] pathTree = LinkFrom[1..].Split("/");
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

			Stat LinkedNode = LookupPathTree(LinkTo[1..].Split("/"));
			symbolicLink.Refer = LinkedNode;

			return symbolicLink;
		}

		public Pipe FIFO(string pathname)
		{
			pathname = SanitizePathName(pathname);
			if (!pathname.StartsWith("/"))
				throw new Exception($"ENOENT: {pathname}");
			pathname = SanitizePathName(pathname);

			string[] pathTree = pathname[1..].Split("/");
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
			pathname = SanitizePathName(pathname);
			if (!pathname.StartsWith("/"))
				throw new Exception($"ENOENT: {pathname}");
			pathname = SanitizePathName(pathname);

			string[] pathTree = pathname[1..].Split("/");
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
		public string Buffer = "";
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
				return this.GetType().Name;
			}
		}
		public Dir Parent = null;
	}
}