using System;

using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Calendar
{
	public class LogicCalendarFunction
	{
		private LogicCalendarEventFunctionData m_functionData;
		private LogicCalendarEvent m_calendarEvent;
		private LogicCalendarErrorHandler m_errorHandler;
		private LogicArrayList<string> m_parameters;

		private int m_idx;

		public LogicCalendarFunction(LogicCalendarEvent calendarEvent, int idx, LogicJSONObject jsonObject, LogicCalendarErrorHandler errorHandler)
		{
			m_parameters = new LogicArrayList<string>();

			m_calendarEvent = calendarEvent;
			m_errorHandler = errorHandler;

			m_idx = idx;

			Load(jsonObject);
		}

		public void Destruct()
		{
			m_functionData = null;
			m_calendarEvent = null;
			m_errorHandler = null;
			m_parameters = null;
		}

		public void Load(LogicJSONObject jsonObject)
		{
			Debugger.DoAssert(m_errorHandler != null, "LogicCalendarErrorHandler must not be NULL!");

			if (jsonObject == null)
			{
				m_errorHandler.ErrorFunction(m_calendarEvent, this, "Event function malformed.");
				return;
			}

			string name = LogicJSONHelper.GetString(jsonObject, "name");
			m_functionData = LogicDataTables.GetCalendarEventFunctionByName(name, null);

			if (m_functionData == null)
			{
				m_errorHandler.ErrorFunction(m_calendarEvent, this, string.Format("event function '{0}' not found.", name));
				return;
			}

			LogicJSONArray parameterArray = jsonObject.GetJSONArray("parameters");

			if (parameterArray != null)
			{
				for (int i = 0; i < parameterArray.Size(); i++)
				{
					m_parameters.Add(parameterArray.GetJSONString(i).GetStringValue());
				}
			}

			LoadingFinished();
		}

		public void LoadingFinished()
		{
			if (m_functionData.IsDeprecated())
			{
				m_errorHandler.ErrorFunction(m_calendarEvent, this, "Function is deprecated.");
			}

			if (m_parameters.Size() != m_functionData.GetParameterCount())
			{
				m_errorHandler.ErrorFunction(m_calendarEvent, this,
												  string.Format("Invalid number of parameters defined. Expected {0} got {1}.", m_functionData.GetParameterCount(),
																m_parameters.Size()));
			}

			for (int i = 0; i < m_parameters.Size(); i++)
			{
				int type = m_functionData.GetParameterType(i);

				switch (type)
				{
					case LogicCalendarEventFunctionData.PARAMETER_TYPE_BOOLEAN:
						GetBoolParameter(i);
						break;
					case LogicCalendarEventFunctionData.PARAMETER_TYPE_INT:
						GetIntParameter(i);
						break;
					case LogicCalendarEventFunctionData.PARAMETER_TYPE_STRING:
						GetStringParameter(i);
						break;
					case LogicCalendarEventFunctionData.PARAMETER_TYPE_TROOP:
						GetDataParameter(i, LogicDataType.CHARACTER);
						break;
					case LogicCalendarEventFunctionData.PARAMETER_TYPE_SPELL:
						GetDataParameter(i, LogicDataType.SPELL);
						break;
					case LogicCalendarEventFunctionData.PARAMETER_TYPE_BUILDING:
						GetDataParameter(i, LogicDataType.BUILDING);
						break;
					case LogicCalendarEventFunctionData.PARAMETER_TYPE_TRAP:
						GetDataParameter(i, LogicDataType.TRAP);
						break;
					case LogicCalendarEventFunctionData.PARAMETER_TYPE_BUNDLE:
						GetDataParameter(i, LogicDataType.GEM_BUNDLE);
						break;
					case LogicCalendarEventFunctionData.PARAMETER_TYPE_BILLING_PACKAGE:
						GetDataParameter(i, LogicDataType.BILLING_PACKAGE);
						break;
					case LogicCalendarEventFunctionData.PARAMETER_TYPE_ANIMATION:
						// TODO: Implement this (client)
						break;
					case LogicCalendarEventFunctionData.PARAMETER_TYPE_HERO:
						GetDataParameter(i, LogicDataType.HERO);
						break;
					default:
						m_errorHandler.ErrorFunction(m_calendarEvent, this, m_functionData.GetParameterName(i),
														  string.Format("Unhandled parameter type {0}!", type));
						break;
				}
			}
		}

		public LogicJSONObject Save()
		{
			LogicJSONObject jsonObject = new LogicJSONObject();
			LogicJSONArray parameterArray = new LogicJSONArray(m_parameters.Size());

			jsonObject.Put("name", new LogicJSONString(m_functionData.GetName()));

			for (int i = 0; i < m_parameters.Size(); i++)
			{
				parameterArray.Add(new LogicJSONString(m_parameters[i]));
			}

			jsonObject.Put("parameters", parameterArray);

			return jsonObject;
		}

		public void ApplyToEvent(LogicCalendarEvent calendarEvent)
		{
			switch (m_functionData.GetFunctionType())
			{
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_TROOP_TRAINING_BOOST:
					calendarEvent.SetNewTrainingBoostBarracksCost(GetIntParameter(0));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_SPELL_BREWING_BOOST:
					calendarEvent.SetNewTrainingBoostSpellCost(GetIntParameter(0));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_BUILDING_BOOST:
					calendarEvent.AddBuildingBoost(GetDataParameter(0, LogicDataType.BUILDING), GetIntParameter(1));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_TROOP_DISCOUNT:
					calendarEvent.AddTroopDiscount(GetDataParameter(0, LogicDataType.CHARACTER), GetIntParameter(1));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_SPELL_DISCOUNT:
					calendarEvent.AddTroopDiscount(GetDataParameter(0, LogicDataType.SPELL), GetIntParameter(1));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_CLAN_XP_MULTIPLIER:
					calendarEvent.SetAllianceXpMultiplier(GetIntParameter(0));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_OFFER_BUNDLE:
					calendarEvent.AddEnabledData(GetDataParameter(0, LogicDataType.GEM_BUNDLE));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_STAR_BONUS_MULTIPLIER:
					calendarEvent.SetStarBonusMultiplier(GetIntParameter(0));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_ENABLE_TROOP:
					calendarEvent.AddEnabledData(GetDataParameter(0, LogicDataType.CHARACTER));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_ENABLE_SPELL:
					calendarEvent.AddEnabledData(GetDataParameter(0, LogicDataType.SPELL));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_ENABLE_TRAP:
					calendarEvent.AddEnabledData(GetDataParameter(0, LogicDataType.TRAP));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_USE_TROOP:
					calendarEvent.AddUseTroop((LogicCombatItemData)GetDataParameter(0, LogicDataType.CHARACTER), GetIntParameter(1), GetIntParameter(2),
											  GetIntParameter(3), GetIntParameter(4));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_TARGETING_TOWN_HALL_LEVEL:
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_TARGETING_PURCHASED_DIAMONDS:
					Debugger.Warning("You should no longer target thru event functions.");
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_ENABLE_BILLING_PACKAGE:
					calendarEvent.AddEnabledData(GetDataParameter(0, LogicDataType.BILLING_PACKAGE));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_CHANGE_WORKER_LOOK:
					// TODO: Implement this (client).
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_GIVE_FREE_TROOPS:
					calendarEvent.AddFreeTroop((LogicCombatItemData)GetDataParameter(0, LogicDataType.CHARACTER), GetIntParameter(1));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_GIVE_FREE_SPELLS:
					calendarEvent.AddFreeTroop((LogicCombatItemData)GetDataParameter(0, LogicDataType.SPELL), GetIntParameter(1));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_GIVE_FREE_HERO_HEALTH:
					if (GetDataParameter(0, LogicDataType.HERO) != null)
					{
						calendarEvent.AddFreeTroop((LogicCombatItemData)GetDataParameter(0, LogicDataType.HERO), 1);
					}
					else
					{
						LogicDataTable table = LogicDataTables.GetTable(LogicDataType.HERO);

						for (int i = 0; i < table.GetItemCount(); i++)
						{
							calendarEvent.AddFreeTroop((LogicCombatItemData)table.GetItemAt(i), 1);
						}
					}

					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_BUILDING_SIGN:
					// TODO: Implement this (client).
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_BUILDING_DESTROYED_SPAWN_UNIT:
					calendarEvent.AddBuildingDestroyedSpawnUnit((LogicBuildingData)GetDataParameter(0, LogicDataType.BUILDING),
																(LogicCharacterData)GetDataParameter(1, LogicDataType.CHARACTER),
																GetIntParameter(2));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_USE_SPELL:
					calendarEvent.AddUseTroop((LogicCombatItemData)GetDataParameter(0, LogicDataType.SPELL), GetIntParameter(1), GetIntParameter(2),
											  GetIntParameter(3), GetIntParameter(4));
					break;
				case LogicCalendarEventFunctionData.FUNCTION_TYPE_CLAN_WAR_LOOT_MULTIPLIER:
					calendarEvent.SetAllianceWarWinLootMultiplier(GetIntParameter(0));
					calendarEvent.SetAllianceWarDrawLootMultiplier(GetIntParameter(1));
					calendarEvent.SetAllianceWarLooseLootMultiplier(GetIntParameter(2));
					break;
				default:
					Debugger.Error(string.Format("Unknown function type: {0}.", m_functionData.GetFunctionType()));
					break;
			}
		}

		public LogicData GetDataParameter(int idx, LogicDataType tableIdx)
		{
			if (IsValidParameter(idx))
			{
				int value = LogicStringUtil.ConvertToInt(m_parameters[idx]);

				if (value != 0)
				{
					LogicData data = LogicDataTables.GetDataById(value, tableIdx);

					if (data != null)
					{
						return data;
					}

					m_errorHandler.WarningFunction(m_calendarEvent, this, m_functionData.GetParameterName(idx),
														string.Format("Unable to find data by id {0} from tableId {1}.", value, tableIdx));
				}
				else
				{
					m_errorHandler.WarningFunction(m_calendarEvent, this, m_functionData.GetParameterName(idx),
														string.Format("Expected globalId got {0}.", value));
				}
			}

			return null;
		}

		public int GetIntParameter(int idx)
		{
			if (IsValidParameter(idx))
			{
				int value = LogicStringUtil.ConvertToInt(m_parameters[idx]);
				int minValue = m_functionData.GetMinValue(idx);
				int maxValue = m_functionData.GetMaxValue(idx);

				if (value < minValue || value > maxValue)
				{
					m_errorHandler.WarningFunction(m_calendarEvent, this, m_functionData.GetParameterName(idx),
														string.Format("Value {0} is not between {1} and {2}.", value, minValue, maxValue));
					return minValue;
				}

				return value;
			}

			return 0;
		}

		public bool GetBoolParameter(int index)
		{
			if (IsValidParameter(index))
			{
				string value = m_parameters[index];

				if (!value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
				{
					if (!value.Equals("false", StringComparison.InvariantCultureIgnoreCase))
					{
						m_errorHandler.ErrorFunction(m_calendarEvent, this, value, string.Format("Invalid boolean value {0}.", value));
					}

					return false;
				}

				return true;
			}

			return false;
		}

		public string GetStringParameter(int index)
		{
			if (IsValidParameter(index))
			{
				return m_parameters[index];
			}

			return string.Empty;
		}

		public bool IsValidParameter(int idx)
		{
			if (m_parameters.Size() <= idx)
			{
				m_errorHandler.ErrorFunction(m_calendarEvent, this, idx.ToString(), "Parameter has not been defined.");
			}
			else
			{
				if (idx >= 0)
				{
					return true;
				}

				m_errorHandler.ErrorFunction(m_calendarEvent, this, "Got negative parameter index");
			}

			return false;
		}

		public string GetName()
			=> m_functionData.GetName();

		public LogicArrayList<string> GetParameters()
			=> m_parameters;
	}
}