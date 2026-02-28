using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class ServerErrorMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24115;

		private string m_errorMessage;

		public ServerErrorMessage() : this(0)
		{
			// ServerErrorMessage.
		}

		public ServerErrorMessage(short messageVersion) : base(messageVersion)
		{
			// ServerErrorMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_errorMessage = m_stream.ReadString(900000);
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteString(m_errorMessage);
		}

		public override short GetMessageType()
			=> ServerErrorMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public override void Destruct()
		{
			base.Destruct();
			m_errorMessage = null;
		}

		public string GetErrorMessage()
			=> m_errorMessage;

		public void SetErrorMessage(string value)
		{
			m_errorMessage = value;
		}
	}
}