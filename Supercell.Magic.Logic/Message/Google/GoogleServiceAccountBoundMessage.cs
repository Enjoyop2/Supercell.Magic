using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Google
{
	public class GoogleServiceAccountBoundMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20261;

		private int m_resultCode;

		public GoogleServiceAccountBoundMessage() : this(0)
		{
			// GoogleServiceAccountBoundMessage.
		}

		public GoogleServiceAccountBoundMessage(short messageVersion) : base(messageVersion)
		{
			// GoogleServiceAccountBoundMessage.
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
			=> 24261;

		public override int GetServiceNodeType()
			=> 1;

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