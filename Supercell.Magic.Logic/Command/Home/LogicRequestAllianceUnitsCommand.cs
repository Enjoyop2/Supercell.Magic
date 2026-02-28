using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicRequestAllianceUnitsCommand : LogicCommand
	{
		private string m_message;

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			if (stream.ReadBoolean())
			{
				m_message = stream.ReadString(900000);
			}
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			base.Encode(encoder);

			if (m_message != null)
			{
				encoder.WriteBoolean(true);
				encoder.WriteString(m_message);
			}
			else
			{
				encoder.WriteBoolean(false);
			}
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.REQUEST_ALLIANCE_UNITS;

		public override void Destruct()
		{
			base.Destruct();
			m_message = null;
		}

		public override int Execute(LogicLevel level)
		{
			LogicBuilding allianceCastle = level.GetGameObjectManagerAt(0).GetAllianceCastle();

			if (allianceCastle != null)
			{
				LogicBunkerComponent bunkerComponent = allianceCastle.GetBunkerComponent();

				if (bunkerComponent != null && bunkerComponent.GetRequestCooldownTime() == 0)
				{
					LogicAvatar homeOwnerAvatar = level.GetHomeOwnerAvatar();

					homeOwnerAvatar.GetChangeListener().RequestAllianceUnits(allianceCastle.GetUpgradeLevel(),
																			 bunkerComponent.GetUsedCapacity(),
																			 bunkerComponent.GetMaxCapacity(),
																			 homeOwnerAvatar.GetAllianceCastleUsedSpellCapacity(),
																			 homeOwnerAvatar.GetAllianceCastleTotalSpellCapacity(),
																			 m_message);

					bunkerComponent.StartRequestCooldownTime();

					return 0;
				}
			}

			return -1;
		}
	}
}