using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Scoring
{
	public class AvatarDuelLastSeasonRankingListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24408;

		private int m_seasonYear;
		private int m_seasonMonth;

		private LogicArrayList<AvatarDuelRankingEntry> m_avatarRankingList;

		public AvatarDuelLastSeasonRankingListMessage() : this(0)
		{
			// AvatarLastSeasonRankingListMessage.
		}

		public AvatarDuelLastSeasonRankingListMessage(short messageVersion) : base(messageVersion)
		{
			// AvatarLastSeasonRankingListMessage.
		}

		public override void Decode()
		{
			base.Decode();

			int count = m_stream.ReadInt();

			if (count > -1)
			{
				m_avatarRankingList = new LogicArrayList<AvatarDuelRankingEntry>(count);

				for (int i = 0; i < count; i++)
				{
					AvatarDuelRankingEntry avatarRankingEntry = new AvatarDuelRankingEntry();
					avatarRankingEntry.Decode(m_stream);
					m_avatarRankingList.Add(avatarRankingEntry);
				}
			}

			m_seasonMonth = m_stream.ReadInt();
			m_seasonYear = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			if (m_avatarRankingList != null)
			{
				m_stream.WriteInt(m_avatarRankingList.Size());

				for (int i = 0; i < m_avatarRankingList.Size(); i++)
				{
					m_avatarRankingList[i].Encode(m_stream);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}

			m_stream.WriteInt(m_seasonMonth);
			m_stream.WriteInt(m_seasonYear);
		}

		public override short GetMessageType()
			=> AvatarDuelLastSeasonRankingListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 28;

		public override void Destruct()
		{
			base.Destruct();

			m_avatarRankingList = null;
		}

		public LogicArrayList<AvatarDuelRankingEntry> RemoveAvatarRankingList()
		{
			LogicArrayList<AvatarDuelRankingEntry> tmp = m_avatarRankingList;
			m_avatarRankingList = null;
			return tmp;
		}

		public void SetAvatarRankingList(LogicArrayList<AvatarDuelRankingEntry> list)
		{
			m_avatarRankingList = list;
		}

		public int GetSeasonYear()
			=> m_seasonYear;

		public void SetSeasonYear(int value)
		{
			m_seasonYear = value;
		}

		public int GetSeasonMonth()
			=> m_seasonMonth;

		public void SetSeasonMonth(int value)
		{
			m_seasonMonth = value;
		}
	}
}