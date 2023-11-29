using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace BetterLCTerminal.fs
{
	class RamFS
	{
		Dir Root = new();

		private Stat LookupPathTree(string[] pathTree)
		{
			Stat current = Root;
			for (int i = 0; i < pathTree.Length; i++)
			{
				if (current.Type != "Directory")
					throw new Exception("ENOENT");

				if (!((Dir)current).Entries.TryGetValue(pathTree[i], out current))
					throw new Exception("ENOENT");

				if (current.Type == "Link")// resolve link
				{
					if (current.Equals(((Link)current).Refer))
						throw new Exception("ECYCLIC");
					if (((Link)current).Refer.Type == "Link")
						throw new Exception("ELNKV");
					current = ((Link)current).Refer;
				}
			}
			return current;
		}

		public Stat StatPath(string pathname)
		{
			if (!pathname.StartsWith("/"))
				throw new Exception("ENOENT");

			string[] pathTree = pathname[1..].Split("/");

			return LookupPathTree(pathTree);
		}

		public Dir MkDir(string pathname)
		{
			if (!pathname.StartsWith("/"))
				throw new Exception("ENOENT");

			string[] pathTree = pathname[1..].Split("/");
			string dirName = pathTree.Last();
			pathTree = pathTree[0..-1];

			Stat directoryToPlaceIn = LookupPathTree(pathTree);

			if (directoryToPlaceIn.Type != "Directory")
				throw new Exception("ENOENT");

			if (((Dir)directoryToPlaceIn).Entries.ContainsKey(dirName))
				throw new Exception("EEXIST");

			Dir directoryToMake = new();
			((Dir)directoryToPlaceIn).Entries.Add(dirName, directoryToMake);

			return directoryToMake;
		}

		public File Touch(string pathname)
		{
			if (!pathname.StartsWith("/"))
				throw new Exception("ENOENT");

			string[] pathTree = pathname[1..].Split("/");
			string fileName = pathTree.Last();
			pathTree = pathTree[0..-1];

			Stat directoryToPlaceIn = LookupPathTree(pathTree);

			if (directoryToPlaceIn.Type != "Directory")
				throw new Exception("ENOENT");

			if (((Dir)directoryToPlaceIn).Entries.ContainsKey(fileName))
				throw new Exception("EEXIST");

			File FileToTouch = new();
			((Dir)directoryToPlaceIn).Entries.Add(fileName, FileToTouch);

			return FileToTouch;
		}

		public Link SymLink(string LinkFrom, string LinkTo)
		{
			if (!LinkFrom.StartsWith("/"))
				throw new Exception("ENOENT");
			if (!LinkTo.StartsWith("/"))
				throw new Exception("ENOENT");

			string[] pathTree = LinkFrom[1..].Split("/");
			string linkName = pathTree.Last();
			pathTree = pathTree[0..-1];

			Stat directoryToPlaceIn = LookupPathTree(pathTree);

			if (directoryToPlaceIn.Type != "Directory")
				throw new Exception("ENOENT");
			if (((Dir)directoryToPlaceIn).Entries.ContainsKey(linkName))
				throw new Exception("EEXIST");

			Link symbolicLink = new();
			((Dir)directoryToPlaceIn).Entries.Add(linkName, symbolicLink);

			symbolicLink.Refer = LookupPathTree(LinkTo[1..].Split("/"));

			return symbolicLink;
		}

		public Pipe FIFO(string pathname)
		{
			if (!pathname.StartsWith("/"))
				throw new Exception("ENOENT");

			string[] pathTree = pathname[1..].Split("/");
			string PipeName = pathTree.Last();
			pathTree = pathTree[0..-1];

			Stat directoryToPlaceIn = LookupPathTree(pathTree);

			if (directoryToPlaceIn.Type != "Directory")
				throw new Exception("ENOENT");

			if (((Dir)directoryToPlaceIn).Entries.ContainsKey(PipeName))
				throw new Exception("EEXIST");

			Pipe PipeToCreate = new();
			((Dir)directoryToPlaceIn).Entries.Add(PipeName, PipeToCreate);

			return PipeToCreate;
		}
	}

	class Dir : Stat
	{
		public new int Size => Entries.Count;
		public new string Type => "Directory";
		public Dictionary<string, Stat> Entries = new();

	}

	class Link : Stat
	{
		public new int Size = 1;
		public new string Type => "Link";
		public Stat Refer = null;
	}

	class Pipe : Stat
	{
		public new int Size = 256;
		public new string Type => "Pipe";
		public string Buffer = "";
	}

	class File : Stat
	{
		public new int Size => Data.Length;

		public new string Type => "Directory";

		public string Data = "";
	}

	class Stat
	{
		public int Size => 0;
		public string Type => "NULL";
	}
}