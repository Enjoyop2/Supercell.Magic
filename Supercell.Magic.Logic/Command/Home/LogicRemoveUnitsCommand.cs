using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.GameObject.Listener;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicRemoveUnitsCommand : LogicCommand
	{
		private readonly LogicArrayList<int> m_removeType;
		private readonly LogicArrayList<int> m_unitsUpgLevel;
		private readonly LogicArrayList<int> m_unitsCount;

		private readonly LogicArrayList<LogicCombatItemData> m_unitsData;

		public LogicRemoveUnitsCommand()
		{
			m_removeType = new LogicArrayList<int>();
			m_unitsUpgLevel = new LogicArrayList<int>();
			m_unitsCount = new LogicArrayList<int>();
			m_unitsData = new LogicArrayList<LogicCombatItemData>();
		}

		public override void Decode(ByteStream stream)
		{
			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				m_removeType.Add(stream.ReadInt());
				m_unitsData.Add((LogicCombatItemData)ByteStreamHelper.ReadDataReference(stream,
																							  stream.ReadInt() != 0 ? DataType.SPELL : DataType.CHARACTER));
				m_unitsCount.Add(stream.ReadInt());
				m_unitsUpgLevel.Add(stream.ReadInt());
			}

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			for (int i = 0; i < m_removeType.Size(); i++)
			{
				encoder.WriteInt(m_removeType[i]);
				ByteStreamHelper.WriteDataReference(encoder, m_unitsData[i]);
				encoder.WriteInt(m_unitsCount[i]);
				encoder.WriteInt(m_unitsUpgLevel[i]);
			}

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.REMOVE_UNITS;

		public override int Execute(LogicLevel level)
		{
			for (int i = 0; i < m_unitsCount.Size(); i++)
			{
				if (m_unitsCount[i] < 0)
				{
					return -1;
				}
			}

			if (LogicDataTables.GetGlobals().EnableTroopDeletion() && level.GetState() == 1 && m_unitsData.Size() > 0)
			{
				LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
				int removedUnits = 0;

				for (int i = 0; i < m_unitsData.Size(); i++)
				{
					LogicCombatItemData data = m_unitsData[i];
					int unitCount = m_unitsCount[i];

					if (m_removeType[i] != 0)
					{
						int upgLevel = m_unitsUpgLevel[i];

						if (data.GetCombatItemType() != LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
						{
							if (data.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_SPELL)
							{
								playerAvatar.SetAllianceUnitCount(data, upgLevel, LogicMath.Max(0, playerAvatar.GetAllianceUnitCount(data, upgLevel) - unitCount));

								if (unitCount > 0)
								{
									do
									{
										playerAvatar.GetChangeListener().AllianceUnitRemoved(data, upgLevel);
									} while (--unitCount != 0);
								}

								removedUnits |= 2;
							}
						}
						else
						{
							LogicBuilding allianceCastle = level.GetGameObjectManagerAt(0).GetAllianceCastle();

							if (allianceCastle != null)
							{
								LogicBunkerComponent bunkerComponent = allianceCastle.GetBunkerComponent();
								int unitTypeIndex = bunkerComponent.GetUnitTypeIndex(data);

								if (unitTypeIndex != -1)
								{
									int cnt = bunkerComponent.GetUnitCount(unitTypeIndex);

									if (cnt > 0)
									{
										bunkerComponent.RemoveUnits(data, upgLevel, unitCount);
										playerAvatar.SetAllianceUnitCount(data, upgLevel, LogicMath.Max(0, playerAvatar.GetAllianceUnitCount(data, upgLevel) - unitCount));

										removedUnits |= 1;

										if (unitCount > 0)
										{
											do
											{
												playerAvatar.GetChangeListener().AllianceUnitRemoved(data, upgLevel);
											} while (--unitCount != 0);
										}
									}
								}
							}
						}
					}
					else
					{
						if (playerAvatar != null && data != null)
						{
							playerAvatar.CommodityCountChangeHelper(0, data, -unitCount);
						}

						LogicArrayList<LogicComponent> components = level.GetComponentManager().GetComponents(LogicComponentType.UNIT_STORAGE);

						for (int j = 0; j < components.Size(); j++)
						{
							if (unitCount > 0)
							{
								LogicUnitStorageComponent storageComponent = (LogicUnitStorageComponent)components[j];
								int unitTypeIndex = storageComponent.GetUnitTypeIndex(data);

								if (unitTypeIndex != -1)
								{
									int cnt = storageComponent.GetUnitCount(unitTypeIndex);

									if (cnt > 0)
									{
										cnt = LogicMath.Min(cnt, unitCount);
										storageComponent.RemoveUnits(data, cnt);

										int type = 2;

										if (data.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
										{
											if (storageComponent.GetParentListener() != null)
											{
												LogicGameObjectListener listener = storageComponent.GetParentListener();

												for (int k = 0; k < cnt; k++)
												{
													listener.UnitRemoved(data);
												}
											}

											type = 1;
										}

										unitCount -= cnt;
										removedUnits |= type;
									}
								}
							}
						}
					}
				}

				switch (removedUnits)
				{
					case 3:
						if (LogicDataTables.GetGlobals().UseNewTraining())
						{
							level.GetGameObjectManager().GetUnitProduction().MergeSlots();
							level.GetGameObjectManager().GetSpellProduction().MergeSlots();
						}

						break;
					case 2:
						if (LogicDataTables.GetGlobals().UseNewTraining())
						{
							level.GetGameObjectManager().GetSpellProduction().MergeSlots();
						}

						break;
					case 1:
						if (LogicDataTables.GetGlobals().UseNewTraining())
						{
							level.GetGameObjectManager().GetUnitProduction().MergeSlots();
						}

						break;
				}
			}

			return 0;
		}
	}
}