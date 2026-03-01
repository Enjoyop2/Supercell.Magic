using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicTrainUnitVillage2Command : LogicCommand
	{
		private LogicCombatItemData m_unitData;

		private int m_unitType;
		private int m_gameObjectId;

		public LogicTrainUnitVillage2Command()
		{
			// LogicTrainUnitVillage2Command.
		}

		public LogicTrainUnitVillage2Command(int gameObjectId, LogicCombatItemData combatItemData)
		{
			m_gameObjectId = gameObjectId;
			m_unitData = combatItemData;
			m_unitType = m_unitData.GetDataType() == DataType.SPELL ? 1 : 0;
		}

		public override void Decode(ByteStream stream)
		{
			m_gameObjectId = stream.ReadInt();
			m_unitType = stream.ReadInt();
			m_unitData = (LogicCombatItemData)ByteStreamHelper.ReadDataReference(stream, m_unitType != 0 ? DataType.SPELL : DataType.CHARACTER);

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_gameObjectId);
			encoder.WriteInt(m_unitType);
			ByteStreamHelper.WriteDataReference(encoder, m_unitData);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.TRAIN_UNIT_VILLAGE2;

		public override void Destruct()
		{
			base.Destruct();

			m_gameObjectId = 0;
			m_unitType = 0;
			m_unitData = null;
		}

		public override int Execute(LogicLevel level)
		{
			if (level.GetVillageType() == 1)
			{
				if (m_gameObjectId != 0)
				{
					LogicGameObjectManager gameObjectManager = level.GetGameObjectManagerAt(1);
					LogicGameObject gameObject = gameObjectManager.GetGameObjectByID(m_gameObjectId);

					if (gameObject != null && gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
					{
						LogicBuilding building = (LogicBuilding)gameObject;

						if (m_unitData != null && level.GetGameMode().GetCalendar().IsProductionEnabled(m_unitData))
						{
							if (m_unitData.GetVillageType() == 1)
							{
								LogicVillage2UnitComponent village2UnitComponent = building.GetVillage2UnitComponent();

								if (village2UnitComponent != null)
								{
									if (m_unitData.IsUnlockedForProductionHouseLevel(gameObjectManager.GetHighestBuildingLevel(m_unitData.GetProductionHouseData(), true))
									)
									{
										LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
										LogicResourceData trainResource = m_unitData.GetTrainingResource();
										int trainCost = m_unitData.GetTrainingCost(playerAvatar.GetUnitUpgradeLevel(m_unitData));

										if (playerAvatar.HasEnoughResources(trainResource, trainCost, true, this, false))
										{
											village2UnitComponent.TrainUnit(m_unitData);
											playerAvatar.CommodityCountChangeHelper(0, trainResource, -trainCost);
										}

										return 0;
									}

									return -7;
								}

								return -4;
							}

							return -8;
						}
					}

					return -5;
				}

				return -1;
			}

			return -10;
		}
	}
}