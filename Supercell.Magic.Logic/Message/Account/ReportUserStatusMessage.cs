using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class ReportUserStatusMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20117;

		public ReportUserStatusMessage() : this(0)
		{
			// ReportUserStatusMessage.
		}

		public ReportUserStatusMessage(short messageVersion) : base(messageVersion)
		{
			// ReportUserStatusMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_stream.ReadInt();
			m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(0);
			m_stream.WriteInt(0);
		}

		public override short GetMessageType()
			=> ReportUserStatusMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}