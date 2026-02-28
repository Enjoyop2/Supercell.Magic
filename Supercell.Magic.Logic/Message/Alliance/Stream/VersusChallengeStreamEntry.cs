using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public class VersusChallengeStreamEntry : StreamEntry
	{
		private string m_message;
		private string m_battleLogJSON;

		private int m_spectatorCount;
		private int m_layoutId;

		private bool m_started;

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_message = stream.ReadString(900000);

			if (stream.ReadBoolean())
			{
				m_battleLogJSON = stream.ReadString(900000);
			}

			stream.ReadVInt();
			m_spectatorCount = stream.ReadVInt();
			m_started = stream.ReadBoolean();
			stream.ReadVInt();
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteString(m_message);

			if (m_battleLogJSON != null)
			{
				stream.WriteBoolean(true);
				stream.WriteString(m_message);
			}
			else
			{
				stream.WriteBoolean(false);
			}

			stream.WriteVInt(0);
			stream.WriteVInt(m_spectatorCount);
			stream.WriteBoolean(m_started);
			stream.WriteVInt(0);
		}

		public override StreamEntryType GetStreamEntryType()
			=> StreamEntryType.VERSUS_BATTLE_REQUEST;

		public string GetMessage()
			=> m_message;

		public void SetMessage(string value)
		{
			m_message = value;
		}

		public int GetLayoutId()
			=> m_layoutId;

		public void SetLayoutId(int value)
		{
			m_layoutId = value;
		}

		public string GetBattleLogJSON()
			=> m_battleLogJSON;

		public void SetBattleLogJSON(string value)
		{
			m_battleLogJSON = value;
		}

		public int GetSpectatorCount()
			=> m_spectatorCount;

		public void SetSpectatorCount(int value)
		{
			m_spectatorCount = value;
		}

		public bool IsStarted()
			=> m_started;

		public void SetStarted(bool started)
		{
			m_started = started;
		}

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = jsonObject.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("VersusChallengeStreamEntry::load base is NULL");
			}

			base.Load(baseObject);

			m_message = jsonObject.GetJSONString("message").GetStringValue();

			LogicJSONString battleLogString = jsonObject.GetJSONString("battleLog");

			if (battleLogString != null)
			{
				m_battleLogJSON = battleLogString.GetStringValue();
			}
		}

		public override void Save(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = new LogicJSONObject();

			base.Save(baseObject);

			jsonObject.Put("base", baseObject);
			jsonObject.Put("message", new LogicJSONString(m_message));

			if (m_battleLogJSON != null)
			{
				jsonObject.Put("battleLog", new LogicJSONString(m_battleLogJSON));
			}
		}
	}
}