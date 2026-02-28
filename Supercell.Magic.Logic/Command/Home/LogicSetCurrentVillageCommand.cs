using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicSetCurrentVillageCommand : LogicCommand
	{
		private int m_villageType;

		public LogicSetCurrentVillageCommand()
		{
			// LogicSetCurrentVillageCommand.
		}

		public LogicSetCurrentVillageCommand(int villageType)
		{
			m_villageType = villageType;
		}

		public override void Decode(ByteStream stream)
		{
			m_villageType = stream.ReadInt();
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_villageType);
			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.SET_CURRENT_VILLAGE;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			if (m_villageType >= 0)
			{
				if (m_villageType <= 1)
				{
					if (m_villageType != level.GetVillageType())
					{
						if (level.GetGameObjectManagerAt(1).GetTownHall() == null)
						{
							// ship was not found!.
						}

						return ChangeVillage(level, false);
					}

					return -3;
				}

				return -2;
			}

			return -1;
		}

		public int ChangeVillage(LogicLevel level, bool force)
		{
			if (m_villageType != 0 && !force)
			{
				LogicVillageObject ship = level.GetGameObjectManagerAt(0).GetShipyard();

				if (ship == null || ship.GetUpgradeLevel() <= 0)
				{
					return -23;
				}
			}

			if (level.GetGameObjectManagerAt(1).GetTownHall() != null)
			{
				level.SetVillageType(m_villageType);

				if (level.GetState() == 1)
				{
					level.GetPlayerAvatar().SetVariableByName("VillageToGoTo", m_villageType);
				}

				level.GetGameObjectManager().RespawnObstacles();
			}

			return 0;
		}
	}
}