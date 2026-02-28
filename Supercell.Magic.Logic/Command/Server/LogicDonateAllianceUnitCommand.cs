using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Unit;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicDonateAllianceUnitCommand : LogicServerCommand
	{
		private LogicLong m_streamId;
		private LogicCombatItemData m_unitData;

		private bool m_quickDonate;

		public void SetData(LogicCombatItemData data, LogicLong streamId, bool quickDonate)
		{
			m_unitData = data;
			m_streamId = streamId;
			m_quickDonate = quickDonate;
		}

		public override void Destruct()
		{
			base.Destruct();

			m_streamId = null;
			m_unitData = null;
		}

		public override void Decode(ByteStream stream)
		{
			m_streamId = stream.ReadLong();
			m_unitData = (LogicCombatItemData)ByteStreamHelper.ReadDataReference(stream, stream.ReadInt() != 0 ? LogicDataType.SPELL : LogicDataType.CHARACTER);
			m_quickDonate = stream.ReadBoolean();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteLong(m_streamId);
			encoder.WriteInt(m_unitData.GetCombatItemType());
			ByteStreamHelper.WriteDataReference(encoder, m_unitData);
			encoder.WriteBoolean(m_quickDonate);

			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				if (m_unitData != null)
				{
					if (m_unitData.GetVillageType() == 0)
					{
						if (!m_unitData.IsDonationDisabled())
						{
							bool removeFromTrainQueue = false;
							int upgLevel = playerAvatar.GetUnitUpgradeLevel(m_unitData);

							LogicUnitProductionComponent unitProductionComponent = null;

							if (m_quickDonate)
							{
								int cost = m_unitData.GetDonateCost();

								if (!playerAvatar.HasEnoughDiamonds(cost, true, level) ||
									!m_unitData.IsUnlockedForProductionHouseLevel(
										level.GetGameObjectManagerAt(0).GetHighestBuildingLevel(m_unitData.GetProductionHouseData())) ||
									!LogicDataTables.GetGlobals().EnableQuickDonate())
								{
									playerAvatar.GetChangeListener().AllianceUnitDonateFailed(m_unitData, upgLevel, m_streamId, m_quickDonate);
									return 0;
								}
							}
							else
							{
								LogicGameObjectManager gameObjectManager = level.GetGameObjectManagerAt(0);

								if (LogicDataTables.GetGlobals().UseNewTraining())
								{
									LogicUnitProduction unitProduction = gameObjectManager.GetUnitProduction();
									LogicUnitProduction spellProduction = gameObjectManager.GetSpellProduction();

									if (unitProduction.GetWaitingForSpaceUnitCount(m_unitData) > 0)
									{
										if (unitProduction.GetUnitProductionType() == m_unitData.GetDataType())
										{
											removeFromTrainQueue = true;
										}
									}

									if (spellProduction.GetWaitingForSpaceUnitCount(m_unitData) > 0)
									{
										if (spellProduction.GetUnitProductionType() == m_unitData.GetDataType())
										{
											removeFromTrainQueue = true;
										}
									}
								}
								else
								{
									for (int i = 0, c = gameObjectManager.GetNumGameObjects(); i < c; i++)
									{
										LogicGameObject gameObject = gameObjectManager.GetGameObjectByIndex(i);

										if (gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
										{
											LogicBuilding building = (LogicBuilding)gameObject;
											LogicUnitProductionComponent component = building.GetUnitProductionComponent();

											if (component != null)
											{
												unitProductionComponent = component;

												if (component.ContainsUnit(m_unitData))
												{
													if (component.GetRemainingSeconds() == 0 && component.GetCurrentlyTrainedUnit() == m_unitData)
													{
														removeFromTrainQueue = true;
													}
												}
												else
												{
													unitProductionComponent = null;
												}
											}
										}
									}
								}

								if (!removeFromTrainQueue)
								{
									if (playerAvatar.GetUnitCount(m_unitData) <= 0)
									{
										playerAvatar.GetChangeListener().AllianceUnitDonateFailed(m_unitData, upgLevel, m_streamId, m_quickDonate);
										return 0;
									}
								}
							}

							if (m_unitData.GetCombatItemType() != LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
							{
								playerAvatar.XpGainHelper(m_unitData.GetHousingSpace() * LogicDataTables.GetGlobals().GetDarkSpellDonationXP());
								level.GetAchievementManager().AlianceSpellDonated((LogicSpellData)m_unitData);
							}
							else
							{
								playerAvatar.XpGainHelper(((LogicCharacterData)m_unitData).GetDonateXP());
								level.GetAchievementManager().AlianceUnitDonated((LogicCharacterData)m_unitData);
							}

							playerAvatar.GetChangeListener().AllianceUnitDonateOk(m_unitData, upgLevel, m_streamId, m_quickDonate);

							if (m_quickDonate)
							{
								int cost = m_unitData.GetDonateCost();

								playerAvatar.UseDiamonds(cost);
								playerAvatar.GetChangeListener().DiamondPurchaseMade(12, m_unitData.GetGlobalID(), 0, cost, level.GetVillageType());

								if ((level.GetState() == 1 || level.GetState() == 3) && m_unitData.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
								{
									LogicGameObject gameObject = level.GetGameObjectManagerAt(0).GetHighestBuilding(m_unitData.GetProductionHouseData());

									if (gameObject != null)
									{
										// Listener.
									}
								}

								return 0;
							}

							if (!removeFromTrainQueue)
							{
								playerAvatar.CommodityCountChangeHelper(0, m_unitData, -1);
							}

							LogicResourceData trainingResource = m_unitData.GetTrainingResource();

							int trainingCost = level.GetGameMode().GetCalendar().GetTrainingCost(m_unitData, upgLevel);
							int refund = playerAvatar.GetTroopDonationRefund() * trainingCost / 100;

							playerAvatar.CommodityCountChangeHelper(0, trainingResource, LogicMath.Max(refund, 0));

							if (level.GetState() == 1 || level.GetState() == 3)
							{
								if (removeFromTrainQueue)
								{
									if (LogicDataTables.GetGlobals().UseNewTraining())
									{
										LogicGameObjectManager gameObjectManager = level.GetGameObjectManagerAt(0);
										LogicUnitProduction unitProduction = m_unitData.GetCombatItemType() != LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER
											? gameObjectManager.GetSpellProduction()
											: gameObjectManager.GetUnitProduction();

										unitProduction.RemoveTrainedUnit(m_unitData);
									}

									if (m_unitData.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
									{
										LogicBuilding productionHouse = null;

										if (unitProductionComponent != null)
										{
											productionHouse = (LogicBuilding)unitProductionComponent.GetParent();
										}
										else
										{
											if (LogicDataTables.GetGlobals().UseTroopWalksOutFromTraining())
											{
												LogicGameObjectManager gameObjectManager = level.GetGameObjectManagerAt(0);
												int gameObjectCount = gameObjectManager.GetNumGameObjects();

												for (int i = 0; i < gameObjectCount; i++)
												{
													LogicGameObject gameObject = gameObjectManager.GetGameObjectByIndex(i);

													if (gameObject != null && gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
													{
														LogicBuilding tmpBuilding = (LogicBuilding)gameObject;
														LogicUnitProductionComponent tmpComponent = tmpBuilding.GetUnitProductionComponent();

														if (tmpComponent != null)
														{
															if (tmpComponent.GetProductionType() == m_unitData.GetCombatItemType())
															{
																if (tmpBuilding.GetBuildingData().GetProducesUnitsOfType() == m_unitData.GetUnitOfType() &&
																	!tmpBuilding.IsUpgrading() &&
																	!tmpBuilding.IsConstructing())
																{
																	if (m_unitData.IsUnlockedForProductionHouseLevel(tmpBuilding.GetUpgradeLevel()))
																	{
																		if (productionHouse != null)
																		{
																			int seed = playerAvatar.GetExpPoints();

																			if (tmpBuilding.Rand(seed) % 1000 > 750)
																			{
																				productionHouse = tmpBuilding;
																			}
																		}
																		else
																		{
																			productionHouse = tmpBuilding;
																		}
																	}
																}
															}
														}
													}
												}
											}
										}

										if (productionHouse != null)
										{
											// TODO: Implement listener.
										}
									}
								}
								else
								{
									LogicArrayList<LogicComponent> components = level.GetComponentManagerAt(0).GetComponents(LogicComponentType.UNIT_STORAGE);

									for (int i = 0; i < components.Size(); i++)
									{
										LogicUnitStorageComponent unitStorageComponent = (LogicUnitStorageComponent)components[i];
										int idx = unitStorageComponent.GetUnitTypeIndex(m_unitData);

										if (idx != -1)
										{
											if (unitStorageComponent.GetUnitCount(idx) > 0)
											{
												unitStorageComponent.RemoveUnits(m_unitData, 1);

												if (LogicDataTables.GetGlobals().UseNewTraining())
												{
													LogicGameObjectManager gameObjectManager = level.GetGameObjectManagerAt(0);
													LogicUnitProduction unitProduction = m_unitData.GetCombatItemType() != LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER
														? gameObjectManager.GetSpellProduction()
														: gameObjectManager.GetUnitProduction();

													unitProduction.MergeSlots();
													unitProduction.UnitRemoved();
												}

												break;
											}
										}
									}
								}

								// TODO: Finish this.
							}

							return 0;
						}

						return -91;
					}

					return -45;
				}
			}

			return 0;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.DONATE_ALLIANCE_UNIT;
	}
}