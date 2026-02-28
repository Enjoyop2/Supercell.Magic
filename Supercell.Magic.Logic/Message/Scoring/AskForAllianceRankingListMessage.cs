using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Scoring
{
	public class AskForAllianceRankingListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14401;

		private LogicLong m_allianceId;

		private int m_villageType;
		private bool m_localRanking;

		public AskForAllianceRankingListMessage() : this(0)
		{
			// AskForAllianceRankingListMessage.
		}

		public AskForAllianceRankingListMessage(short messageVersion) : base(messageVersion)
		{
			// AskForAllianceRankingListMessage.
		}

		public override void Decode()
		{
			base.Decode();

			if (m_stream.ReadBoolean())
			{
				m_allianceId = m_stream.ReadLong();
			}

			m_localRanking = m_stream.ReadBoolean();
			m_villageType = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			if (m_allianceId != null)
			{
				m_stream.WriteBoolean(true);
				m_stream.WriteLong(m_allianceId);
			}
			else
			{
				m_stream.WriteBoolean(false);
			}

			m_localRanking = m_stream.ReadBoolean();
			m_villageType = m_stream.ReadInt();
		}

		public override short GetMessageType()
			=> AskForAllianceRankingListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 28;

		public override void Destruct()
		{
			base.Destruct();
		}

		public LogicLong RemoveAllianceId()
		{
			LogicLong tmp = m_allianceId;
			m_allianceId = null;
			return tmp;
		}

		public void SetAllianceId(LogicLong id)
		{
			m_allianceId = id;
		}

		public bool LocalRanking()
			=> m_localRanking;

		public void SetLocalRanking(bool value)
		{
			m_localRanking = value;
		}

		public int GetVillageType()
			=> m_villageType;

		public void SetVillageType(int value)
		{
			m_villageType = value;
		}
	}
}