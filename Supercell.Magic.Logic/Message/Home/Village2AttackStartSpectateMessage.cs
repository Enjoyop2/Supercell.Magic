using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class Village2AttackStartSpectateMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 15110;
		private LogicLong m_streamId;

		public Village2AttackStartSpectateMessage() : this(0)
		{
			// Village2AttackStartSpectateMessage.
		}

		public Village2AttackStartSpectateMessage(short messageVersion) : base(messageVersion)
		{
			// Village2AttackStartSpectateMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_streamId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteLong(m_streamId);
		}

		public override short GetMessageType()
			=> Village2AttackStartSpectateMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
			m_streamId = null;
		}

		public LogicLong GetStreamId()
			=> m_streamId;

		public void SetStreamId(LogicLong id)
		{
			m_streamId = id;
		}
	}
}