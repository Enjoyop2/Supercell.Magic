using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicUpgradeMultipleBuildingsCommand : LogicCommand
	{
		private bool m_useAltResource;
		private readonly LogicArrayList<int> m_gameObjectIds;

		public LogicUpgradeMultipleBuildingsCommand()
		{
			m_gameObjectIds = new LogicArrayList<int>(300);
		}

		public override void Decode(ByteStream stream)
		{
			m_useAltResource = stream.ReadBoolean();

			int count = stream.ReadInt();

			if (count > 300)
			{
				count = 300;
			}

			for (int i = 0; i < count; i++)
			{
				m_gameObjectIds.Add(stream.ReadInt());
			}

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteBoolean(m_useAltResource);

			int count = m_gameObjectIds.Size();

			if (count > 300)
			{
				count = 300;
			}

			for (int i = 0; i < count; i++)
			{
				encoder.WriteInt(m_gameObjectIds[i]);
			}

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.UPGRADE_MULTIPLE_BUILDINGS;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			if (m_gameObjectIds.Size() > 0)
			{
				LogicResourceData buildResourceData = null;
				int buildCost = 0;

				for (int i = 0; i < m_gameObjectIds.Size(); i++)
				{
					LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(m_gameObjectIds[i]);

					if (gameObject != null && gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
					{
						LogicBuilding building = (LogicBuilding)gameObject;
						LogicBuildingData buildingData = building.GetBuildingData();

						if (buildingData.IsTownHallVillage2())
						{
							return -76;
						}

						int nextUpgradeLevel = building.GetUpgradeLevel() + 1;

						if (building.CanUpgrade(false) && buildingData.GetUpgradeLevelCount() > nextUpgradeLevel && buildingData.GetAmountCanBeUpgraded(nextUpgradeLevel) == 0)
						{
							buildResourceData = buildingData.GetBuildResource(nextUpgradeLevel);

							if (m_useAltResource)
							{
								buildResourceData = buildingData.GetAltBuildResource(nextUpgradeLevel);
							}

							buildCost += buildingData.GetBuildCost(nextUpgradeLevel, level);
						}
					}
				}

				if (buildResourceData != null)
				{
					LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

					if (playerAvatar.HasEnoughResources(buildResourceData, buildCost, true, this, false))
					{
						if (level.HasFreeWorkers(this, -1))
						{
							bool updateListener = true;

							for (int i = 0; i < m_gameObjectIds.Size(); i++)
							{
								LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(m_gameObjectIds[i]);

								if (gameObject != null && gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
								{
									LogicBuilding building = (LogicBuilding)gameObject;
									LogicBuildingData buildingData = building.GetBuildingData();

									int nextUpgradeLevel = building.GetUpgradeLevel() + 1;

									if (building.CanUpgrade(false) && buildingData.GetUpgradeLevelCount() > nextUpgradeLevel &&
										buildingData.GetAmountCanBeUpgraded(nextUpgradeLevel) == 0)
									{
										if (m_gameObjectIds.Size() > 6)
										{
											updateListener = (building.GetTileX() + building.GetTileY()) % (m_gameObjectIds.Size() / 4) == 0;
										}

										building.StartUpgrading(updateListener, false);
									}
								}
							}

							playerAvatar.CommodityCountChangeHelper(0, buildResourceData, -buildCost);

							return 0;
						}
					}
				}
			}

			return -2;
		}
	}
}