using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public class ReplayStreamEntry : StreamEntry
	{
		private string m_message;
		private string m_opponentName;
		private string m_battleLogJSON;

		private int m_majorVersion;
		private int m_buildVersion;
		private int m_contentVersion;
		private int m_replayShardId;

		private bool m_attack;

		private LogicLong m_replayId;

		public ReplayStreamEntry()
		{
			m_message = string.Empty;
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_replayShardId = stream.ReadInt();
			m_replayId = stream.ReadLong();
			m_attack = stream.ReadBoolean();
			m_message = stream.ReadString(900000);
			m_opponentName = stream.ReadString(900000);
			m_battleLogJSON = stream.ReadString(900000);
			m_majorVersion = stream.ReadInt();
			m_buildVersion = stream.ReadInt();
			m_contentVersion = stream.ReadInt();
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteInt(m_replayShardId);
			stream.WriteLong(m_replayId);
			stream.WriteBoolean(m_attack);
			stream.WriteString(m_message);
			stream.WriteString(m_opponentName);
			stream.WriteString(m_battleLogJSON);
			stream.WriteInt(m_majorVersion);
			stream.WriteInt(m_buildVersion);
			stream.WriteInt(m_contentVersion);
		}

		public string GetMessage()
			=> m_message;

		public void SetMessage(string value)
		{
			m_message = value;
		}

		public string GetOpponentName()
			=> m_opponentName;

		public void SetOpponentName(string value)
		{
			m_opponentName = value;
		}

		public string GetBattleLogJSON()
			=> m_battleLogJSON;

		public void SetBattleLogJSON(string value)
		{
			m_battleLogJSON = value;
		}

		public int GetMajorVersion()
			=> m_majorVersion;

		public void SetMajorVersion(int value)
		{
			m_majorVersion = value;
		}

		public int GetBuildVersion()
			=> m_buildVersion;

		public void SetBuildVersion(int value)
		{
			m_buildVersion = value;
		}

		public int GetContentVersion()
			=> m_contentVersion;

		public void SetContentVersion(int value)
		{
			m_contentVersion = value;
		}

		public int GetReplayShardId()
			=> m_replayShardId;

		public void SetReplayShardId(int value)
		{
			m_replayShardId = value;
		}

		public bool IsAttack()
			=> m_attack;

		public void SetAttack(bool value)
		{
			m_attack = value;
		}

		public LogicLong GetReplayId()
			=> m_replayId;

		public void SetReplayId(LogicLong value)
		{
			m_replayId = value;
		}

		public override StreamEntryType GetStreamEntryType()
			=> StreamEntryType.REPLAY;

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = jsonObject.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("ReplayStreamEntry::load base is NULL");
			}

			base.Load(baseObject);

			m_battleLogJSON = jsonObject.GetJSONString("battleLog").GetStringValue();
			m_message = jsonObject.GetJSONString("message").GetStringValue();
			m_opponentName = jsonObject.GetJSONString("opponent_name").GetStringValue();
			m_attack = jsonObject.GetJSONBoolean("attack").IsTrue();
			m_majorVersion = jsonObject.GetJSONNumber("replay_major_v").GetIntValue();
			m_buildVersion = jsonObject.GetJSONNumber("replay_build_v").GetIntValue();
			m_contentVersion = jsonObject.GetJSONNumber("replay_content_v").GetIntValue();

			LogicJSONNumber replayShardId = jsonObject.GetJSONNumber("replay_shard_id");

			if (replayShardId != null)
			{
				m_replayShardId = replayShardId.GetIntValue();
				m_replayId = new LogicLong(jsonObject.GetJSONNumber("replay_id_hi").GetIntValue(), jsonObject.GetJSONNumber("replay_id_lo").GetIntValue());
			}
		}

		public override void Save(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = new LogicJSONObject();

			base.Save(baseObject);

			jsonObject.Put("base", baseObject);
			jsonObject.Put("battleLog", new LogicJSONString(m_battleLogJSON));
			jsonObject.Put("message", new LogicJSONString(m_message));
			jsonObject.Put("opponent_name", new LogicJSONString(m_opponentName));
			jsonObject.Put("attack", new LogicJSONBoolean(m_attack));
			jsonObject.Put("replay_major_v", new LogicJSONNumber(m_majorVersion));
			jsonObject.Put("replay_build_v", new LogicJSONNumber(m_buildVersion));
			jsonObject.Put("replay_content_v", new LogicJSONNumber(m_contentVersion));

			if (m_replayId != null)
			{
				jsonObject.Put("replay_shard_id", new LogicJSONNumber(m_replayShardId));
				jsonObject.Put("replay_id_hi", new LogicJSONNumber(m_replayId.GetHigherInt()));
				jsonObject.Put("replay_id_lo", new LogicJSONNumber(m_replayId.GetLowerInt()));
			}
		}
	}
}