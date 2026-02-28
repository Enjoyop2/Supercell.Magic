using System;

using Supercell.Magic.Titan.Debug;

namespace Supercell.Magic.Tools.Client
{
	public class DebuggerListener : IDebuggerListener
	{
		public void HudPrint(string message)
		{
		}

		public void Print(string message)
		{
			Console.WriteLine(message);
		}

		public void Warning(string message)
		{
			Console.WriteLine(message);
		}

		public void Error(string message)
		{
			Console.WriteLine(message);
		}
	}
}