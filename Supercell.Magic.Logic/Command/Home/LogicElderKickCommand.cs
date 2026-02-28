using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicElderKickCommand : LogicCommand
	{
		private LogicLong m_playerId;
		private string m_message;

		public override void Decode(ByteStream stream)
		{
			m_playerId = stream.ReadLong();

			if (stream.ReadBoolean())
			{
				m_message = stream.ReadString(900000);
			}

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteLong(m_playerId);

			if (m_message != null)
			{
				encoder.WriteBoolean(true);
				encoder.WriteString(m_message);
			}
			else
			{
				encoder.WriteBoolean(false);
			}

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.ELDER_KICK;

		public override void Destruct()
		{
			base.Destruct();

			m_playerId = null;
			m_message = null;
		}

		public override int Execute(LogicLevel level)
		{
			LogicAvatar homeOwnerAvatar = level.GetHomeOwnerAvatar();
			LogicAvatarAllianceRole allianceRole = homeOwnerAvatar.GetAllianceRole();

			if (allianceRole != LogicAvatarAllianceRole.MEMBER)
			{
				if (allianceRole == LogicAvatarAllianceRole.LEADER || allianceRole == LogicAvatarAllianceRole.CO_LEADER)
				{
					level.GetHomeOwnerAvatar().GetChangeListener().KickPlayer(m_playerId, m_message);
					return 0;
				}

				LogicBuilding allianceCastle = level.GetGameObjectManagerAt(0).GetAllianceCastle();

				if (allianceCastle != null)
				{
					LogicBunkerComponent bunkerComponent = allianceCastle.GetBunkerComponent();

					if (bunkerComponent != null)
					{
						if (bunkerComponent.GetElderCooldownTime() == 0)
						{
							bunkerComponent.StartElderKickCooldownTime();
							level.GetHomeOwnerAvatar().GetChangeListener().KickPlayer(m_playerId, m_message);
							return 0;
						}
					}
				}

				return -2;
			}

			return -1;
		}
	}
}