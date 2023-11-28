using System.Collections;
using System;
using System.Linq;

namespace BetterLCTerminal.stdlib
{
	public class FileDescriptor
	{
		public event EventHandler<FD_OnData_args> OnData;
		private string buffer = "";
		private bool raw = false;

		void SetRaw(bool value)
		{
			if (value)
			{
				if (buffer.Length > 0)
					Flush();
			}
			raw = value;
		}

		void Flush()
		{
			FD_OnData_args EvArgs = new()
			{
				Data = buffer
			};
			OnData.Invoke(this, EvArgs);
			buffer = "";
		}

		public void Write(string text)
		{
			if (!raw)
			{
				char[] charByChar = text.ToCharArray();
				for (int i = 0; i < charByChar.Length; i++)
					addToBuffer(charByChar[i]);
			}
			else
			{
				FD_OnData_args EvArgs = new()
				{
					Data = text
				};
				OnData?.Invoke(this, EvArgs);
			}
		}

		void addToBuffer(char c)
		{
			buffer.Append(c);
			if (c != '\n')
			{
				Flush();
			}
			return;
		}
	}

	public class FD_OnData_args : EventArgs
	{
		public string Data { get; set; }
	}
}