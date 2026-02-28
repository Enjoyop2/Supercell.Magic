using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Facebook
{
	public class FacebookAccountBoundMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24201;

		private int m_resultCode;

		public FacebookAccountBoundMessage() : this(0)
		{
			// FacebookAccountBoundMessage.
		}

		public FacebookAccountBoundMessage(short messageVersion) : base(messageVersion)
		{
			// FacebookAccountBoundMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_resultCode = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt(m_resultCode);
		}

		public override short GetMessageType()
			=> FacebookAccountBoundMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public override void Destruct()
		{
			base.Destruct();
		}

		public int GetResultCode()
			=> m_resultCode;

		public void SetResultCode(int value)
		{
			m_resultCode = value;
		}
	}
}