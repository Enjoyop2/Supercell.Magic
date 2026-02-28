using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class HomeBattleReplayMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14114;
		private int m_replayShardId;
		private LogicLong m_replayId;

		public HomeBattleReplayMessage() : this(0)
		{
			// HomeBattleReplayMessage.
		}

		public HomeBattleReplayMessage(short messageVersion) : base(messageVersion)
		{
			// HomeBattleReplayMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_replayShardId = m_stream.ReadInt();
			m_replayId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_replayShardId);
			m_stream.WriteLong(m_replayId);
		}

		public int GetReplayShardId()
			=> m_replayShardId;

		public void SetReplayShardId(int value)
		{
			m_replayShardId = value;
		}

		public LogicLong GetReplayId()
			=> m_replayId;

		public void SetReplayId(LogicLong value)
		{
			m_replayId = value;
		}

		public override short GetMessageType()
			=> HomeBattleReplayMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}