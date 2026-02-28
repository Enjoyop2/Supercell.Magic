using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Command.Battle;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic
{
	public class LogicNpcAttack
	{
		private LogicLevel m_level;
		private LogicNpcAvatar m_npcAvatar;
		private LogicBuildingClassData m_buildingClass;

		private bool m_unitsDeployed;
		private bool m_unitsDeployStarted;

		private int m_placePositionX;
		private int m_placePositionY;
		private int m_nextUnit;

		public LogicNpcAttack(LogicLevel level)
		{
			m_placePositionX = -1;
			m_placePositionY = -1;
			m_level = level;
			m_npcAvatar = (LogicNpcAvatar)level.GetVisitorAvatar();
			m_buildingClass = LogicDataTables.GetBuildingClassByName("Defense", null);

			if (m_buildingClass == null)
			{
				Debugger.Error("LogicNpcAttack - Unable to find Defense building class");
			}
		}

		public void Destruct()
		{
			m_level = null;
			m_npcAvatar = null;
			m_buildingClass = null;
		}

		public bool PlaceOneUnit()
		{
			if (m_placePositionX == -1 && m_placePositionY == -1)
			{
				int startAreaY = m_level.GetPlayArea().GetStartY();
				int widthInTiles = m_level.GetWidthInTiles();

				int minDistance = -1;

				for (int i = 0; i < widthInTiles; i++)
				{
					int centerY = (startAreaY - 1) / 2;

					for (int j = 0; j < startAreaY - 1; j++, centerY--)
					{
						int distance = ((widthInTiles >> 1) - i) * ((widthInTiles >> 1) - i) + centerY * centerY;

						if (minDistance == -1 || distance < minDistance)
						{
							LogicTile tile = m_level.GetTileMap().GetTile(i, j);

							if (tile.GetPassableFlag() != 0)
							{
								m_placePositionX = i;
								m_placePositionY = j;
								minDistance = distance;
							}
						}
					}
				}
			}

			if (m_placePositionX == -1 && m_placePositionY == -1)
			{
				Debugger.Error("LogicNpcAttack::placeOneUnit - No attack position found!");
			}
			else
			{
				LogicArrayList<LogicDataSlot> units = m_npcAvatar.GetUnits();

				for (int i = 0; i < units.Size(); i++)
				{
					LogicDataSlot slot = units[i];

					if (slot.GetCount() > 0)
					{
						LogicCharacter character = LogicPlaceAttackerCommand.PlaceAttacker(m_npcAvatar, (LogicCharacterData)slot.GetData(), m_level,
																						   m_placePositionX << 9,
																						   m_placePositionY << 9);

						if (!m_unitsDeployStarted)
						{
							character.GetListener().MapUnlocked();
						}

						character.GetCombatComponent().SetPreferredTarget(m_buildingClass, 100, false);

						m_unitsDeployStarted = true;

						return true;
					}
				}
			}

			return false;
		}

		public void Tick()
		{
			if (!m_unitsDeployed)
			{
				m_nextUnit -= 64;

				if (m_nextUnit <= 0)
				{
					m_unitsDeployed = !PlaceOneUnit();
					m_nextUnit = 200;
				}
			}
		}
	}
}