using System;

using Supercell.Magic.Servers.Core.Network.Message.Request;

namespace Supercell.Magic.Servers.Core.Network.Request
{
	public class ServerRequestArgs
	{
		public delegate void CompleteHandler(ServerRequestArgs args);

		public CompleteHandler OnComplete { get; set; } = args => { };
		public ServerResponseMessage ResponseMessage
		{
			get; private set;
		}

		public ServerRequestError ErrorCode
		{
			get; private set;
		}

		internal DateTime ExpireTime
		{
			get; set;
		}

		private bool m_completed;

		public ServerRequestArgs(int timeout)
		{
			ExpireTime = DateTime.UtcNow.AddSeconds(timeout);
		}

		internal void Abort()
		{
			if (!m_completed)
			{
				m_completed = true;
				ErrorCode = ServerRequestError.Aborted;
				OnComplete(this);
			}
		}

		internal void SetResponseMessage(ServerResponseMessage message)
		{
			if (!m_completed)
			{
				m_completed = true;
				ResponseMessage = message;
				ErrorCode = ServerRequestError.Success;
				OnComplete(this);
			}
		}
	}

	public enum ServerRequestError
	{
		Success,
		Aborted
	}
}