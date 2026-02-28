using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Scoring
{
	public class AskForAvatarDuelLastSeasonRankingListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14408;

		private LogicLong m_avatarId;

		public AskForAvatarDuelLastSeasonRankingListMessage() : this(0)
		{
			// AskForAvatarDuelLastSeasonRankingListMessage.
		}

		public AskForAvatarDuelLastSeasonRankingListMessage(short messageVersion) : base(messageVersion)
		{
			// AskForAvatarDuelLastSeasonRankingListMessage.
		}

		public override void Decode()
		{
			base.Decode();

			if (m_stream.ReadBoolean())
			{
				m_avatarId = m_stream.ReadLong();
			}
		}

		public override void Encode()
		{
			base.Encode();

			if (m_avatarId != null)
			{
				m_stream.WriteBoolean(true);
				m_stream.WriteLong(m_avatarId);
			}
			else
			{
				m_stream.WriteBoolean(false);
			}
		}

		public override short GetMessageType()
			=> AskForAvatarDuelLastSeasonRankingListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 28;

		public override void Destruct()
		{
			base.Destruct();
			m_avatarId = null;
		}

		public LogicLong RemoveAvatarId()
		{
			LogicLong tmp = m_avatarId;
			m_avatarId = null;
			return tmp;
		}

		public void SetAvatarId(LogicLong id)
		{
			m_avatarId = id;
		}
	}
}