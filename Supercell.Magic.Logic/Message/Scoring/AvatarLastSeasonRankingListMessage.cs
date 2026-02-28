using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Scoring
{
	public class AvatarLastSeasonRankingListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24405;

		private int m_seasonYear;
		private int m_seasonMonth;

		private LogicArrayList<AvatarRankingEntry> m_avatarRankingList;

		public AvatarLastSeasonRankingListMessage() : this(0)
		{
			// AvatarLastSeasonRankingListMessage.
		}

		public AvatarLastSeasonRankingListMessage(short messageVersion) : base(messageVersion)
		{
			// AvatarLastSeasonRankingListMessage.
		}

		public override void Decode()
		{
			base.Decode();

			int count = m_stream.ReadInt();

			if (count > -1)
			{
				m_avatarRankingList = new LogicArrayList<AvatarRankingEntry>(count);

				for (int i = 0; i < count; i++)
				{
					AvatarRankingEntry avatarRankingEntry = new AvatarRankingEntry();
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
			=> AvatarLastSeasonRankingListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 28;

		public override void Destruct()
		{
			base.Destruct();

			m_avatarRankingList = null;
		}

		public LogicArrayList<AvatarRankingEntry> RemoveAvatarRankingList()
		{
			LogicArrayList<AvatarRankingEntry> tmp = m_avatarRankingList;
			m_avatarRankingList = null;
			return tmp;
		}

		public void SetAvatarRankingList(LogicArrayList<AvatarRankingEntry> list)
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