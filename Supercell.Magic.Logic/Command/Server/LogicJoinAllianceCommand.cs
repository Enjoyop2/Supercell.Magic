using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Mode;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicJoinAllianceCommand : LogicServerCommand
	{
		private LogicLong m_allianceId;
		private string m_allianceName;
		private int m_allianceBadgeId;
		private int m_allianceExpLevel;
		private bool m_allianceCreate;

		public override void Destruct()
		{
			base.Destruct();

			m_allianceId = null;
			m_allianceName = null;
		}

		public override void Decode(ByteStream stream)
		{
			m_allianceId = stream.ReadLong();
			m_allianceName = stream.ReadString(900000);
			m_allianceBadgeId = stream.ReadInt();
			m_allianceCreate = stream.ReadBoolean();
			m_allianceExpLevel = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteLong(m_allianceId);
			encoder.WriteString(m_allianceName);
			encoder.WriteInt(m_allianceBadgeId);
			encoder.WriteBoolean(m_allianceCreate);
			encoder.WriteInt(m_allianceExpLevel);

			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				if (m_allianceCreate)
				{
					LogicGlobals globals = LogicDataTables.GetGlobals();
					LogicResourceData resource = globals.GetAllianceCreateResourceData();

					int removeCount = LogicMath.Min(globals.GetAllianceCreateCost(), playerAvatar.GetResourceCount(resource));

					playerAvatar.CommodityCountChangeHelper(0, resource, -removeCount);
				}

				playerAvatar.SetAllianceId(m_allianceId.Clone());
				playerAvatar.SetAllianceName(m_allianceName);
				playerAvatar.SetAllianceBadgeId(m_allianceBadgeId);
				playerAvatar.SetAllianceLevel(m_allianceExpLevel);
				playerAvatar.SetAllianceRole(m_allianceCreate ? LogicAvatarAllianceRole.LEADER : LogicAvatarAllianceRole.MEMBER);
				playerAvatar.GetChangeListener().AllianceJoined(playerAvatar.GetAllianceId(), m_allianceName, m_allianceBadgeId, m_allianceExpLevel,
																playerAvatar.GetAllianceRole());

				LogicGameListener gameListener = level.GetGameListener();

				if (gameListener != null)
				{
					if (m_allianceCreate)
					{
						gameListener.AllianceCreated();
					}
					else
					{
						gameListener.AllianceJoined();
					}
				}

				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.JOIN_ALLIANCE;

		public void SetAllianceData(LogicLong allianceId, string allianceName, int allianceBadgeId, int allianceExpLevel, bool isNewAlliance)
		{
			m_allianceId = allianceId;
			m_allianceName = allianceName;
			m_allianceBadgeId = allianceBadgeId;
			m_allianceExpLevel = allianceExpLevel;
			m_allianceCreate = isNewAlliance;
		}

		public LogicLong GetAllianceId()
			=> m_allianceId;
	}
}