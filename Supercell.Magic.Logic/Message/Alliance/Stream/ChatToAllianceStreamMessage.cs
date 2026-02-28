using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public class ChatToAllianceStreamMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14315;

		private string m_message;

		public ChatToAllianceStreamMessage() : this(0)
		{
			// ChatToAllianceStreamMessage.
		}

		public ChatToAllianceStreamMessage(short messageVersion) : base(messageVersion)
		{
			// ChatToAllianceStreamMessage.
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
			=> ChatToAllianceStreamMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

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