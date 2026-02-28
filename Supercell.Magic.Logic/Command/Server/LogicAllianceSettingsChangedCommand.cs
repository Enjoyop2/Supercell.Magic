using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicAllianceSettingsChangedCommand : LogicServerCommand
	{
		private LogicLong m_allianceId;
		private int m_allianceBadgeId;

		public override void Destruct()
		{
			base.Destruct();
			m_allianceId = null;
		}

		public override void Decode(ByteStream stream)
		{
			m_allianceId = stream.ReadLong();
			m_allianceBadgeId = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteLong(m_allianceId);
			encoder.WriteInt(m_allianceBadgeId);

			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				if (LogicLong.Equals(playerAvatar.GetAllianceId(), m_allianceId))
				{
					playerAvatar.SetAllianceBadgeId(m_allianceBadgeId);
					level.GetGameListener().AllianceSettingsChanged();
				}

				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.ALLIANCE_SETTINGS_CHANGED;

		public void SetAllianceData(LogicLong allianceId, int allianceBadgeId)
		{
			m_allianceId = allianceId;
			m_allianceBadgeId = allianceBadgeId;
		}
	}
}