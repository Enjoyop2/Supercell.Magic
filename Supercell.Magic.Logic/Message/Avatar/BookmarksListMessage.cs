using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class BookmarksListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24340;
		private LogicArrayList<LogicLong> m_allianceIds;

		public BookmarksListMessage() : this(0)
		{
			// BookmarksListMessage.
		}

		public BookmarksListMessage(short messageVersion) : base(messageVersion)
		{
			// BookmarksListMessage.
		}

		public override void Decode()
		{
			base.Decode();

			int count = m_stream.ReadInt();

			if (count >= 0)
			{
				Debugger.DoAssert(count < 1000, "Too many alliance ids in BookmarksListMessage");
				m_allianceIds = new LogicArrayList<LogicLong>(count);

				for (int i = 0; i < count; i++)
				{
					m_allianceIds.Add(m_stream.ReadLong());
				}
			}
		}

		public override void Encode()
		{
			base.Encode();

			if (m_allianceIds != null)
			{
				m_stream.WriteInt(m_allianceIds.Size());

				for (int i = 0; i < m_allianceIds.Size(); i++)
				{
					m_stream.WriteLong(m_allianceIds[i]);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}
		}

		public override short GetMessageType()
			=> BookmarksListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
			m_allianceIds = null;
		}

		public LogicArrayList<LogicLong> GetAllianceIds()
			=> m_allianceIds;

		public void SetAllianceIds(LogicArrayList<LogicLong> list)
		{
			m_allianceIds = list;
		}
	}
}