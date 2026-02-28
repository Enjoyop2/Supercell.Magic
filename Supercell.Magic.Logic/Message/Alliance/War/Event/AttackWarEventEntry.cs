using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Alliance.War.Event
{
	public class AttackWarEventEntry : WarEventEntry
	{
		private LogicLong m_accountId;
		private LogicLong m_avatarId;
		private LogicLong m_homeId;
		private LogicLong m_allianceId;

		private readonly string m_avatarName;
		private readonly string m_allianceName;

		private int m_stars;
		private int m_destructionPercentage;
		private int m_replayShardId;

		private LogicLong m_replayId;

		public AttackWarEventEntry()
		{
			m_avatarName = string.Empty;
			m_allianceName = string.Empty;
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteLong(m_allianceId);
			stream.WriteLong(m_accountId);
			stream.WriteLong(m_avatarId);
			stream.WriteLong(m_homeId);

			stream.WriteString(m_allianceName);
			stream.WriteString(m_avatarName);

			stream.WriteInt(1);
			stream.WriteInt(2);
			stream.WriteInt(3);
			stream.WriteInt(4);
			stream.WriteInt(5);
			stream.WriteInt(6);
			stream.WriteInt(7);

			if (m_replayId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteInt(m_replayShardId);
				stream.WriteLong(m_replayId);
			}
			else
			{
				stream.WriteBoolean(false);
			}

			stream.WriteInt(8);
		}

		public override int GetWarEventEntryType()
			=> WarEventEntry.WAR_EVENT_ENTRY_TYPE_ATTACK;
	}
}