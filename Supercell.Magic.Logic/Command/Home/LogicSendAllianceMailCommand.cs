using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicSendAllianceMailCommand : LogicCommand
	{
		private string m_message;

		public LogicSendAllianceMailCommand()
		{
			// LogicSendAllianceMailCommand.
		}

		public LogicSendAllianceMailCommand(string message)
		{
			m_message = message;
		}

		public override void Decode(ByteStream stream)
		{
			m_message = stream.ReadString(900000);
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteString(m_message);
			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.SEND_ALLIANCE_MAIL;

		public override void Destruct()
		{
			base.Destruct();
			m_message = null;
		}

		public override int Execute(LogicLevel level)
		{
			LogicAvatarAllianceRole allianceRole = level.GetHomeOwnerAvatar().GetAllianceRole();

			if (allianceRole == LogicAvatarAllianceRole.LEADER || allianceRole == LogicAvatarAllianceRole.CO_LEADER)
			{
				LogicBuilding allianceCastle = level.GetGameObjectManagerAt(0).GetAllianceCastle();

				if (allianceCastle != null)
				{
					LogicBunkerComponent bunkerComponent = allianceCastle.GetBunkerComponent();

					if (bunkerComponent != null && bunkerComponent.GetClanMailCooldownTime() == 0)
					{
						bunkerComponent.StartClanMailCooldownTime();
						level.GetHomeOwnerAvatar().GetChangeListener().SendClanMail(m_message);

						return 0;
					}
				}

				return -2;
			}

			return -1;
		}
	}
}