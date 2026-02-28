using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Scoring
{
	public class AskForAvatarLocalRankingListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14404;

		private LogicLong m_avatarId;
		private int m_villageType;

		public AskForAvatarLocalRankingListMessage() : this(0)
		{
			// AskForAvatarLocalRankingListMessage.
		}

		public AskForAvatarLocalRankingListMessage(short messageVersion) : base(messageVersion)
		{
			// AskForAvatarLocalRankingListMessage.
		}

		public override void Decode()
		{
			base.Decode();

			if (m_stream.ReadBoolean())
			{
				m_avatarId = m_stream.ReadLong();
			}

			m_villageType = m_stream.ReadInt();
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

			m_villageType = m_stream.ReadInt();
		}

		public override short GetMessageType()
			=> AskForAvatarLocalRankingListMessage.MESSAGE_TYPE;

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

		public int GetVillageType()
			=> m_villageType;

		public void SetVillageType(int value)
		{
			m_villageType = value;
		}
	}
}