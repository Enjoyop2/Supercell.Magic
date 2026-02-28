using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicLeaveAllianceCommand : LogicServerCommand
	{
		private LogicLong m_allianceId;

		public void SetAllianceData(LogicLong value)
		{
			m_allianceId = value;
		}

		public override void Destruct()
		{
			base.Destruct();
			m_allianceId = null;
		}

		public override void Decode(ByteStream stream)
		{
			m_allianceId = stream.ReadLong();
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteLong(m_allianceId);
			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				if (playerAvatar.IsInAlliance())
				{
					if (playerAvatar.GetAllianceId().Equals(m_allianceId))
					{
						playerAvatar.SetAllianceId(null);
						playerAvatar.SetAllianceName(null);
						playerAvatar.SetAllianceBadgeId(-1);
						playerAvatar.SetAllianceLevel(-1);
						playerAvatar.GetChangeListener().AllianceLeft();
					}
				}

				level.GetGameListener().AllianceLeft();

				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.LEAVE_ALLIANCE;
	}
}