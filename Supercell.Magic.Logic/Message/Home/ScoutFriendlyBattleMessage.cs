using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class ScoutFriendlyBattleMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14111;
		private LogicLong m_streamId;

		public ScoutFriendlyBattleMessage() : this(0)
		{
			// GoHomeMessage.
		}

		public ScoutFriendlyBattleMessage(short messageVersion) : base(messageVersion)
		{
			// GoHomeMessage.
		}

		public override void Decode()
		{
			m_streamId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			m_stream.WriteLong(m_streamId);
		}

		public LogicLong GetStreamId()
			=> m_streamId;

		public void SetStreamId(LogicLong id)
		{
			m_streamId = id;
		}

		public override short GetMessageType()
			=> ScoutFriendlyBattleMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}