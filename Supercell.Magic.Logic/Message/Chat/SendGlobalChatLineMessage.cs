using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Chat
{
	public class SendGlobalChatLineMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14715;

		private string m_message;

		public SendGlobalChatLineMessage() : this(0)
		{
			// SendGlobalChatLineMessage.
		}

		public SendGlobalChatLineMessage(short messageVersion) : base(messageVersion)
		{
			// SendGlobalChatLineMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_message = m_stream.ReadString(900000);
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteString(m_message);
		}

		public override short GetMessageType()
			=> SendGlobalChatLineMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 6;

		public override void Destruct()
		{
			base.Destruct();
			m_message = null;
		}

		public string RemoveMessage()
		{
			string tmp = m_message;
			m_message = null;
			return tmp;
		}

		public void SetMessage(string message)
		{
			m_message = message;
		}
	}
}