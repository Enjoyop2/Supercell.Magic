using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicClientGlobals : LogicDataTable
	{
		private bool m_pepperEnabled;
		private bool m_powerSaveModeLessEndTurnMessages;

		public LogicClientGlobals(CSVTable table, LogicDataType index) : base(table, index)
		{
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_pepperEnabled = GetBoolValue("USE_PEPPER_CRYPTO");
			m_powerSaveModeLessEndTurnMessages = GetBoolValue("POWER_SAVE_MODE_LESS_ENDTURN_MESSAGES");
		}

		private LogicGlobalData GetGlobalData(string name)
			=> LogicDataTables.GetClientGlobalByName(name, null);

		private bool GetBoolValue(string name)
			=> GetGlobalData(name).GetBooleanValue();

		private int GetIntValue(string name)
			=> GetGlobalData(name).GetNumberValue();

		public bool PepperEnabled()
			=> m_pepperEnabled;

		public bool PowerSaveModeLessEndTurnMessages()
			=> m_powerSaveModeLessEndTurnMessages;
	}
}