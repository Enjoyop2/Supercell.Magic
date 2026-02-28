using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public class ChallengeReplayStreamEntry : StreamEntry
	{
		private string m_battleLogJSON;

		private int m_replayMajorVersion;
		private int m_replayBuildVersion;
		private int m_replayContentVersion;
		private int m_replayShardId;

		private LogicLong m_replayId;

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_battleLogJSON = stream.ReadString(900000);
			m_replayMajorVersion = stream.ReadVInt();
			m_replayBuildVersion = stream.ReadVInt();
			m_replayContentVersion = stream.ReadVInt();

			stream.ReadVInt();
			stream.ReadBoolean();

			if (stream.ReadBoolean())
			{
				m_replayShardId = stream.ReadVInt();
				m_replayId = stream.ReadLong();
			}
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteString(m_battleLogJSON);
			stream.WriteVInt(m_replayMajorVersion);
			stream.WriteVInt(m_replayBuildVersion);
			stream.WriteVInt(m_replayContentVersion);
			stream.WriteVInt(0);
			stream.WriteBoolean(false);

			if (m_replayId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteVInt(m_replayShardId);
				stream.WriteLong(m_replayId);
			}
			else
			{
				stream.WriteBoolean(false);
			}
		}

		public string GetBattleLogJSON()
			=> m_battleLogJSON;

		public void SetBattleLogJSON(string value)
		{
			m_battleLogJSON = value;
		}

		public int GetReplayMajorVersion()
			=> m_replayMajorVersion;

		public void SetReplayMajorVersion(int value)
		{
			m_replayMajorVersion = value;
		}

		public int GetReplayBuildVersion()
			=> m_replayBuildVersion;

		public void SetReplayBuildVersion(int value)
		{
			m_replayBuildVersion = value;
		}

		public int GetReplayContentVersion()
			=> m_replayContentVersion;

		public void SetReplayContentVersion(int value)
		{
			m_replayContentVersion = value;
		}

		public int GetReplayShardId()
			=> m_replayShardId;

		public void SetReplayShardId(int value)
		{
			m_replayShardId = value;
		}

		public LogicLong GetReplayId()
			=> m_replayId;

		public void SetReplayId(LogicLong value)
		{
			m_replayId = value;
		}

		public override StreamEntryType GetStreamEntryType()
			=> StreamEntryType.CHALLENGE_REPLAY;

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = jsonObject.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("ChallengeReplayStreamEntry::load base is NULL");
			}

			base.Load(baseObject);

			m_battleLogJSON = jsonObject.GetJSONString("battleLog").GetStringValue();
			m_replayMajorVersion = jsonObject.GetJSONNumber("replay_major_v").GetIntValue();
			m_replayBuildVersion = jsonObject.GetJSONNumber("replay_build_v").GetIntValue();
			m_replayContentVersion = jsonObject.GetJSONNumber("replay_content_v").GetIntValue();

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
			jsonObject.Put("replay_major_v", new LogicJSONNumber(m_replayMajorVersion));
			jsonObject.Put("replay_build_v", new LogicJSONNumber(m_replayBuildVersion));
			jsonObject.Put("replay_content_v", new LogicJSONNumber(m_replayContentVersion));

			if (m_replayId != null)
			{
				jsonObject.Put("replay_shard_id", new LogicJSONNumber(m_replayShardId));
				jsonObject.Put("replay_id_hi", new LogicJSONNumber(m_replayId.GetHigherInt()));
				jsonObject.Put("replay_id_lo", new LogicJSONNumber(m_replayId.GetLowerInt()));
			}
		}
	}
}