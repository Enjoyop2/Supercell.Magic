using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Avatar.Stream
{
	public class AvatarStreamMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24411;

		private LogicArrayList<AvatarStreamEntry> m_entries;

		public AvatarStreamMessage() : this(0)
		{
			// AvatarStreamMessage.
		}

		public AvatarStreamMessage(short messageVersion) : base(messageVersion)
		{
			// AvatarStreamMessage.
		}

		public override void Decode()
		{
			base.Decode();

			int cnt = m_stream.ReadInt();

			if (cnt != -1)
			{
				m_entries = new LogicArrayList<AvatarStreamEntry>(cnt);

				for (int i = 0; i < cnt; i++)
				{
					AvatarStreamEntry entry = AvatarStreamEntryFactory.CreateStreamEntryByType((AvatarStreamEntryType)m_stream.ReadInt());

					if (entry == null)
					{
						Debugger.Warning("Corrupted AvatarStreamMessage");
						break;
					}

					entry.Decode(m_stream);
				}
			}
			else
			{
				m_entries = null;
			}
		}

		public override void Encode()
		{
			base.Encode();

			if (m_entries != null)
			{
				m_stream.WriteInt(m_entries.Size());

				for (int i = 0; i < m_entries.Size(); i++)
				{
					m_stream.WriteInt((int)m_entries[i].GetAvatarStreamEntryType());
					m_entries[i].Encode(m_stream);
				}
			}
			else
			{
				m_stream.WriteInt(-1);
			}
		}

		public override short GetMessageType()
			=> AvatarStreamMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();

			if (m_entries != null)
			{
				if (m_entries.Size() != 0)
				{
					do
					{
						m_entries[0].Destruct();
						m_entries.Remove(0);
					} while (m_entries.Size() != 0);
				}

				m_entries = null;
			}
		}

		public LogicArrayList<AvatarStreamEntry> RemoveStreamEntries()
		{
			LogicArrayList<AvatarStreamEntry> tmp = m_entries;
			m_entries = null;
			return tmp;
		}

		public void SetStreamEntries(LogicArrayList<AvatarStreamEntry> entry)
		{
			m_entries = entry;
		}
	}
}