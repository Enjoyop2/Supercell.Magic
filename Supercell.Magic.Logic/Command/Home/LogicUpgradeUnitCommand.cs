using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicUpgradeUnitCommand : LogicCommand
	{
		private LogicCombatItemData m_unitData;

		private LogicDataType m_unitType;
		private int m_gameObjectId;

		public LogicUpgradeUnitCommand()
		{
			// LogicUpgradeUnitCommand.
		}

		public LogicUpgradeUnitCommand(LogicCombatItemData combatItemData, int gameObjectId)
		{
			m_unitData = combatItemData;
			m_gameObjectId = gameObjectId;
			m_unitType = m_unitData.GetDataType();
		}

		public override void Decode(ByteStream stream)
		{
			m_gameObjectId = stream.ReadInt();
			m_unitType = (LogicDataType)stream.ReadInt();
			m_unitData = (LogicCombatItemData)ByteStreamHelper.ReadDataReference(stream, m_unitType != 0 ? LogicDataType.SPELL : LogicDataType.CHARACTER);

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_gameObjectId);
			encoder.WriteInt((int)m_unitType);
			ByteStreamHelper.WriteDataReference(encoder, m_unitData);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.UPGRADE_UNIT;

		public override void Destruct()
		{
			base.Destruct();

			m_gameObjectId = 0;
			m_unitType = 0;
			m_unitData = null;
		}

		public override int Execute(LogicLevel level)
		{
			LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(m_gameObjectId);

			if (gameObject != null && gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
			{
				LogicBuilding building = (LogicBuilding)gameObject;

				if (m_unitData != null)
				{
					LogicUnitUpgradeComponent unitUpgradeComponent = building.GetUnitUpgradeComponent();

					if (unitUpgradeComponent != null && unitUpgradeComponent.CanStartUpgrading(m_unitData))
					{
						LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
						int upgradeLevel = playerAvatar.GetUnitUpgradeLevel(m_unitData);
						int upgradeCost = m_unitData.GetUpgradeCost(upgradeLevel);
						LogicResourceData upgradeResourceData = m_unitData.GetUpgradeResource(upgradeLevel);

						if (playerAvatar.HasEnoughResources(upgradeResourceData, upgradeCost, true, this, false))
						{
							playerAvatar.CommodityCountChangeHelper(0, upgradeResourceData, -upgradeCost);
							unitUpgradeComponent.StartUpgrading(m_unitData);

							return 0;
						}
					}
				}
			}

			return -1;
		}
	}
}