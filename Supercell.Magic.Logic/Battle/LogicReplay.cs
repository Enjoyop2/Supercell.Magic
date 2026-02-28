using Supercell.Magic.Logic.Command;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Battle
{
	public class LogicReplay
	{
		private readonly LogicLevel m_level;
		private LogicJSONObject m_replayObject;
		private LogicJSONNumber m_endTickNumber;
		private LogicJSONNumber m_preparationSkipNumber;

		public LogicReplay(LogicLevel level)
		{
			m_level = level;
			m_replayObject = new LogicJSONObject();
			StartRecord();
		}

		public void Destruct()
		{
			m_replayObject = null;
			m_endTickNumber = null;
			m_preparationSkipNumber = null;
		}

		public void StartRecord()
		{
			m_replayObject = new LogicJSONObject();
			m_endTickNumber = new LogicJSONNumber();

			LogicJSONObject levelObject = new LogicJSONObject();
			LogicJSONObject visitorObject = new LogicJSONObject();
			LogicJSONObject homeOwnerAvatarObject = new LogicJSONObject();

			m_level.SaveToJSON(levelObject);
			m_level.GetVisitorAvatar().SaveToReplay(visitorObject);
			m_level.GetHomeOwnerAvatar().SaveToReplay(homeOwnerAvatarObject);

			m_replayObject.Put("level", levelObject);
			m_replayObject.Put("attacker", visitorObject);
			m_replayObject.Put("defender", homeOwnerAvatarObject);
			m_replayObject.Put("end_tick", m_endTickNumber);
			m_replayObject.Put("timestamp", new LogicJSONNumber(m_level.GetGameMode().GetStartTime()));
			m_replayObject.Put("cmd", new LogicJSONArray());
			m_replayObject.Put("calendar", m_level.GetCalendar().Save());

			if (m_level.GetGameMode().GetConfiguration().GetJson() != null)
			{
				m_replayObject.Put("globals", m_level.GetGameMode().GetConfiguration().GetJson());
			}
		}

		public void SubTick()
		{
			m_endTickNumber.SetIntValue(m_level.GetLogicTime().GetTick() + 1);
		}

		public void RecordCommand(LogicCommand command)
		{
			LogicJSONArray commandArray = m_replayObject.GetJSONArray("cmd");
			LogicJSONObject commandObject = new LogicJSONObject();
			LogicCommandManager.SaveCommandToJSON(commandObject, command);

			commandArray.Add(commandObject);
		}

		public void RecordPreparationSkipTime(int secs)
		{
			if (secs > 0)
			{
				if (m_preparationSkipNumber == null)
				{
					m_preparationSkipNumber = new LogicJSONNumber();
					m_replayObject.Put("prep_skip", m_preparationSkipNumber);
				}

				m_preparationSkipNumber.SetIntValue(secs);
			}
		}

		public LogicJSONObject GetJson()
			=> m_replayObject;
	}
}