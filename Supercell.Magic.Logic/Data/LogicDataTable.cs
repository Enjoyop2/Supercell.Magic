using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Data
{
	public class LogicDataTable
	{
		private LogicDataType m_tableIndex;
		private string m_tableName;
		private bool m_loaded;
		private bool m_loaded2;

		protected CSVTable m_table;
		protected LogicArrayList<LogicData> m_items;

		public LogicDataTable(CSVTable table, LogicDataType index)
		{
			m_tableIndex = index;
			m_table = table;
			m_items = new LogicArrayList<LogicData>();

			LoadTable();
		}

		public void Destruct()
		{
			m_items.Destruct();
			m_tableName = null;
			m_tableIndex = 0;
		}

		public void LoadTable()
		{
			for (int i = 0, j = m_table.GetRowCount(); i < j; i++)
			{
				AddItem(m_table.GetRowAt(i));
			}
		}

		public void SetTable(CSVTable table)
		{
			m_table = table;

			for (int i = 0; i < m_items.Size(); i++)
			{
				m_items[i].SetCSVRow(table.GetRowAt(i));
			}
		}

		public void AddItem(CSVRow row)
		{
			m_items.Add(CreateItem(row));
		}

		public LogicData CreateItem(CSVRow row)
		{
			LogicData data = null;

			switch (m_tableIndex)
			{
				case LogicDataType.BUILDING:
					{
						data = new LogicBuildingData(row, this);
						break;
					}

				case LogicDataType.LOCALE:
					{
						data = new LogicLocaleData(row, this);
						break;
					}

				case LogicDataType.RESOURCE:
					{
						data = new LogicResourceData(row, this);
						break;
					}

				case LogicDataType.CHARACTER:
					{
						data = new LogicCharacterData(row, this);
						break;
					}

				case LogicDataType.ANIMATION:
					{
						data = new LogicAnimationData(row, this);
						break;
					}

				case LogicDataType.PROJECTILE:
					{
						data = new LogicProjectileData(row, this);
						break;
					}

				case LogicDataType.BUILDING_CLASS:
					{
						data = new LogicBuildingClassData(row, this);
						break;
					}

				case LogicDataType.OBSTACLE:
					{
						data = new LogicObstacleData(row, this);
						break;
					}

				case LogicDataType.EFFECT:
					{
						data = new LogicEffectData(row, this);
						break;
					}

				case LogicDataType.PARTICLE_EMITTER:
					{
						data = new LogicParticleEmitterData(row, this);
						break;
					}

				case LogicDataType.EXPERIENCE_LEVEL:
					{
						data = new LogicExperienceLevelData(row, this);
						break;
					}

				case LogicDataType.TRAP:
					{
						data = new LogicTrapData(row, this);
						break;
					}

				case LogicDataType.ALLIANCE_BADGE:
					{
						data = new LogicAllianceBadgeData(row, this);
						break;
					}

				case LogicDataType.GLOBAL:
				case LogicDataType.CLIENT_GLOBAL:
					{
						data = new LogicGlobalData(row, this);
						break;
					}

				case LogicDataType.TOWNHALL_LEVEL:
					{
						data = new LogicTownhallLevelData(row, this);
						break;
					}

				case LogicDataType.ALLIANCE_PORTAL:
					{
						data = new LogicAlliancePortalData(row, this);
						break;
					}

				case LogicDataType.NPC:
					{
						data = new LogicNpcData(row, this);
						break;
					}

				case LogicDataType.DECO:
					{
						data = new LogicDecoData(row, this);
						break;
					}

				case LogicDataType.RESOURCE_PACK:
					{
						data = new LogicResourcePackData(row, this);
						break;
					}

				case LogicDataType.SHIELD:
					{
						data = new LogicShieldData(row, this);
						break;
					}

				case LogicDataType.MISSION:
					{
						data = new LogicMissionData(row, this);
						break;
					}

				case LogicDataType.BILLING_PACKAGE:
					{
						data = new LogicBillingPackageData(row, this);
						break;
					}

				case LogicDataType.ACHIEVEMENT:
					{
						data = new LogicAchievementData(row, this);
						break;
					}

				case LogicDataType.SPELL:
					{
						data = new LogicSpellData(row, this);
						break;
					}

				case LogicDataType.HINT:
					{
						data = new LogicHintData(row, this);
						break;
					}

				case LogicDataType.HERO:
					{
						data = new LogicHeroData(row, this);
						break;
					}

				case LogicDataType.LEAGUE:
					{
						data = new LogicLeagueData(row, this);
						break;
					}

				case LogicDataType.NEWS:
					{
						data = new LogicNewsData(row, this);
						break;
					}

				case LogicDataType.WAR:
					{
						data = new LogicWarData(row, this);
						break;
					}

				case LogicDataType.REGION:
					{
						data = new LogicRegionData(row, this);
						break;
					}

				case LogicDataType.ALLIANCE_BADGE_LAYER:
					{
						data = new LogicAllianceBadgeLayerData(row, this);
						break;
					}

				case LogicDataType.ALLIANCE_LEVEL:
					{
						data = new LogicAllianceLevelData(row, this);
						break;
					}

				case LogicDataType.HELPSHIFT:
					{
						data = new LogicHelpshiftData(row, this);
						break;
					}

				case LogicDataType.CREDIT:
				case LogicDataType.FAQ:
				case LogicDataType.VARIABLE:
					{
						data = new LogicData(row, this);
						break;
					}

				case LogicDataType.GEM_BUNDLE:
					{
						data = new LogicGemBundleData(row, this);
						break;
					}

				case LogicDataType.VILLAGE_OBJECT:
					{
						data = new LogicVillageObjectData(row, this);
						break;
					}

				case LogicDataType.CALENDAR_EVENT_FUNCTION:
					{
						data = new LogicCalendarEventFunctionData(row, this);
						break;
					}

				case LogicDataType.BOOMBOX:
					{
						data = new LogicBoomboxData(row, this);
						break;
					}

				case LogicDataType.EVENT_ENTRY:
					{
						data = new LogicEventEntryData(row, this);
						break;
					}

				case LogicDataType.DEEPLINK:
					{
						data = new LogicDeeplinkData(row, this);
						break;
					}

				case LogicDataType.LEAGUE_VILLAGE2:
					{
						data = new LogicLeagueVillage2Data(row, this);
						break;
					}

				default:
					{
						Debugger.Error("Invalid data table id: " + m_tableIndex);
						break;
					}
			}

			return data;
		}

		public virtual void CreateReferences()
		{
			if (LogicDataTables.CanReloadTable(this) || !m_loaded)
			{
				for (int i = 0; i < m_items.Size(); i++)
				{
					m_items[i].CreateReferences();
				}

				m_loaded = true;
			}
		}

		public virtual void CreateReferences2()
		{
			if (LogicDataTables.CanReloadTable(this) || !m_loaded2)
			{
				for (int i = 0; i < m_items.Size(); i++)
				{
					m_items[i].CreateReferences2();
				}

				m_loaded2 = true;
			}
		}

		public LogicData GetItemAt(int index)
			=> m_items[index];

		public LogicData GetDataByName(string name, LogicData caller)
		{
			if (!string.IsNullOrEmpty(name))
			{
				for (int i = 0; i < m_items.Size(); i++)
				{
					LogicData data = m_items[i];

					if (data.GetName().Equals(name))
					{
						return data;
					}
				}

				if (caller != null)
				{
					Debugger.Warning(string.Format("CSV row ({0}) has an invalid reference ({1})", caller.GetName(), name));
				}
			}

			return null;
		}

		public LogicData GetItemById(int globalId)
		{
			int instanceId = GlobalID.GetInstanceID(globalId);

			if (instanceId < 0 || instanceId >= m_items.Size())
			{
				Debugger.Warning("LogicDataTable::getItemById() - Instance id out of bounds! " + (instanceId + 1) + "/" + m_items.Size());
				return null;
			}

			return m_items[instanceId];
		}

		public int GetItemCount()
			=> m_items.Size();

		public LogicDataType GetTableIndex()
			=> m_tableIndex;

		public string GetTableName()
			=> m_tableName;

		public void SetName(string name)
		{
			m_tableName = name;
		}
	}
}