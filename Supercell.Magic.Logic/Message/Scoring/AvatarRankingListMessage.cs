using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Scoring
{
	public class AvatarRankingListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24403;

		private int m_nextEndTimeSeconds;
		private int m_seasonYear;
		private int m_seasonMonth;
		private int m_lastSeasonYear;
		private int m_lastSeasonMonth;

		private LogicArrayList<AvatarRankingEntry> m_avatarRankingList;
		private LogicArrayList<AvatarRankingEntry> m_lastSeasonAvatarRankingList;

		public AvatarRankingListMessage() : this(0)
		{
			// AvatarRankingListMessage.
		}

		public AvatarRankingListMessage(short messageVersion) : base(messageVersion)
		{
			// AvatarRankingListMessage.
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

			int count2 = m_stream.ReadInt();

			if (count2 > -1)
			{
				m_lastSeasonAvatarRankingList = new LogicArrayList<AvatarRankingEntry>(count2);

				for (int i = 0; i < count2; i++)
				{
					AvatarRankingEntry avatarRankingEntry = new AvatarRankingEntry();
					avatarRankingEntry.Decode(m_stream);
					m_lastSeasonAvatarRankingList.Add(avatarRankingEntry);
				}
			}

			m_nextEndTimeSeconds = m_stream.ReadInt();
			m_seasonYear = m_stream.ReadInt();
			m_seasonMonth = m_stream.ReadInt();
			m_lastSeasonYear = m_stream.ReadInt();
			m_lastSeasonMonth = m_stream.ReadInt();
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

			if (m_lastSeasonAvatarRankingList != null)
			{
				m_stream.WriteInt(m_lastSeasonAvatarRankingList.Size());

				for (int i = 0; i < m_lastSeasonAvatarRankingList.Size(); i++)
				{
					m_lastSeasonAvatarRankingList[i].Encode(m_stream);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}

			m_stream.WriteInt(m_nextEndTimeSeconds);
			m_stream.WriteInt(m_seasonYear);
			m_stream.WriteInt(m_seasonMonth);
			m_stream.WriteInt(m_lastSeasonYear);
			m_stream.WriteInt(m_lastSeasonMonth);
		}

		public override short GetMessageType()
			=> AvatarRankingListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 28;

		public override void Destruct()
		{
			base.Destruct();

			m_avatarRankingList = null;
			m_lastSeasonAvatarRankingList = null;
		}

		public LogicArrayList<AvatarRankingEntry> RemoveAvatarRankingList()
		{
			LogicArrayList<AvatarRankingEntry> tmp = m_avatarRankingList;
			m_avatarRankingList = null;
			return tmp;
		}

		public LogicArrayList<AvatarRankingEntry> RemoveLastSeasonAvatarRankingList()
		{
			LogicArrayList<AvatarRankingEntry> tmp = m_lastSeasonAvatarRankingList;
			m_lastSeasonAvatarRankingList = null;
			return tmp;
		}

		public void SetAvatarRankingList(LogicArrayList<AvatarRankingEntry> list)
		{
			m_avatarRankingList = list;
		}

		public void SetLastSeasonAvatarRankingList(LogicArrayList<AvatarRankingEntry> list)
		{
			m_lastSeasonAvatarRankingList = list;
		}

		public int GetNextEndTimeSeconds()
			=> m_nextEndTimeSeconds;

		public void SetNextEndTimeSeconds(int value)
		{
			m_nextEndTimeSeconds = value;
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

		public int GetLastSeasonYear()
			=> m_lastSeasonYear;

		public void SetLastSeasonYear(int value)
		{
			m_lastSeasonYear = value;
		}

		public int GetLastSeasonMonth()
			=> m_lastSeasonMonth;

		public void SetLastSeasonMonth(int value)
		{
			m_lastSeasonMonth = value;
		}
	}
}