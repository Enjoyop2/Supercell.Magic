using System;

namespace Supercell.Magic.Titan.Exceptions
{
	public class LogicException : Exception
	{
		public LogicException()
		{
		}

		public LogicException(string message) : base(message)
		{
		}
	}
}