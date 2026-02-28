using System;

using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public class ChallengeStreamEntry : StreamEntry
	{
		private string m_message;
		private string m_battleLogJSON;

		private int m_spectatorCount;

		private bool m_started;
		private bool m_warLayout;

		private byte[] m_snapshotHomeJSON;

		private LogicLong m_liveReplayId;

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
			=> StreamEntryType.CHALLENGE;

		public string GetMessage()
			=> m_message;

		public void SetMessage(string value)
		{
			m_message = value;
		}

		public string GetBattleLogJSON()
			=> m_battleLogJSON;

		public void SetBattleLogJSON(string value)
		{
			m_battleLogJSON = value;
		}


		public byte[] GetSnapshotHomeJSON()
			=> m_snapshotHomeJSON;

		public int GetSnapshotHomeLengthJSON()
			=> m_snapshotHomeJSON.Length;

		public void SetSnapshotHomeJSON(byte[] value)
		{
			m_snapshotHomeJSON = value;
		}

		public int GetSpectatorCount()
			=> m_spectatorCount;

		public void SetSpectatorCount(int value)
		{
			m_spectatorCount = value;
		}

		public LogicLong GetLiveReplayId()
			=> m_liveReplayId;

		public void SetLiveReplayId(LogicLong value)
		{
			m_liveReplayId = value;
		}

		public bool IsStarted()
			=> m_started;

		public void SetStarted(bool started)
		{
			m_started = started;
		}

		public bool IsWarLayout()
			=> m_warLayout;

		public void SetWarLayout(bool warLayout)
		{
			m_warLayout = warLayout;
		}

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = jsonObject.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("ReplayVersusBattleStreamEntry::load base is NULL");
			}

			base.Load(baseObject);

			m_message = jsonObject.GetJSONString("message").GetStringValue();
			m_snapshotHomeJSON = Convert.FromBase64String(jsonObject.GetJSONString("snapshot_home").GetStringValue());
			m_warLayout = jsonObject.GetJSONBoolean("war_layout").IsTrue();

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
			jsonObject.Put("snapshot_home", new LogicJSONString(Convert.ToBase64String(m_snapshotHomeJSON)));
			jsonObject.Put("war_layout", new LogicJSONBoolean(m_warLayout));

			if (m_battleLogJSON != null)
			{
				jsonObject.Put("battleLog", new LogicJSONString(m_battleLogJSON));
			}
		}
	}
}