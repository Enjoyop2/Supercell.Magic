using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Scoring
{
	public class AvatarDuelLocalRankingListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24407;

		private LogicArrayList<AvatarDuelRankingEntry> m_avatarRankingList;

		public AvatarDuelLocalRankingListMessage() : this(0)
		{
			// AvatarDuelLocalRankingListMessage.
		}

		public AvatarDuelLocalRankingListMessage(short messageVersion) : base(messageVersion)
		{
			// AvatarDuelLocalRankingListMessage.
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
					AvatarDuelRankingEntry avatarDuelRankingEntry = new AvatarDuelRankingEntry();
					avatarDuelRankingEntry.Decode(m_stream);
					m_avatarRankingList.Add(avatarDuelRankingEntry);
				}
			}
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
		}

		public override short GetMessageType()
			=> AvatarDuelLocalRankingListMessage.MESSAGE_TYPE;

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
	}
}