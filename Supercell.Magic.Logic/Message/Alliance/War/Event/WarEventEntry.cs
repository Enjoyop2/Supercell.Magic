using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Alliance.War.Event
{
	public class WarEventEntry
	{
		public const int WAR_EVENT_ENTRY_TYPE_ATTACK = 1;

		private LogicLong m_id;
		private int m_ageSeconds;

		public WarEventEntry()
		{
		}

		public virtual void Encode(ByteStream stream)
		{
			stream.WriteLong(m_id);
			stream.WriteInt(m_ageSeconds);
		}

		public virtual void Decode(ByteStream stream)
		{
			m_id = stream.ReadLong();
			m_ageSeconds = stream.ReadInt();
		}

		public virtual int GetWarEventEntryType()
			=> -1;
	}
}