using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicChangeAllianceRoleCommand : LogicServerCommand
	{
		private LogicLong m_allianceId;
		private LogicAvatarAllianceRole m_allianceRole;

		public void SetData(LogicLong allianceId, LogicAvatarAllianceRole allianceRole)
		{
			m_allianceId = allianceId;
			m_allianceRole = allianceRole;
		}

		public override void Destruct()
		{
			base.Destruct();
			m_allianceId = null;
		}

		public override void Decode(ByteStream stream)
		{
			m_allianceId = stream.ReadLong();
			m_allianceRole = (LogicAvatarAllianceRole)stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteLong(m_allianceId);
			encoder.WriteInt((int)m_allianceRole);

			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				if (playerAvatar.IsInAlliance())
				{
					if (LogicLong.Equals(playerAvatar.GetAllianceId(), m_allianceId))
					{
						playerAvatar.SetAllianceRole(m_allianceRole);
					}
				}

				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.CHANGE_ALLIANCE_ROLE;
	}
}