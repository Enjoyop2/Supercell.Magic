using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public class VersusBattleReplayStreamEntry : StreamEntry
	{
		private LogicLong m_matchId;
		private LogicLong m_attackerLiveReplayId;
		private LogicLong m_opponentLiveReplayId;

		private int m_attackerBattleStatus;
		private int m_opponentBattleStatus;

		private int m_attackerStars;
		private int m_attackerDestructionPercentage;

		private int m_opponentStars;
		private int m_opponentDestructionPercentage;

		private int m_attackerReplayShardId;
		private int m_opponentReplayShardId;

		private int m_attackerReplayMajorVersion;
		private int m_attackerReplayBuildVersion;
		private int m_attackerReplayContentVersion;

		private int m_opponentReplayMajorVersion;
		private int m_opponentReplayBuildVersion;
		private int m_opponentReplayContentVersion;

		private LogicLong m_attackerReplayId;
		private LogicLong m_opponentReplayId;

		private string m_attackerBattleLog;
		private string m_opponentBattleLog;
		private string m_opponentName;

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteInt(m_attackerBattleStatus);
			stream.WriteInt(m_attackerStars);
			stream.WriteInt(m_attackerDestructionPercentage);
			stream.WriteInt(m_opponentBattleStatus);
			stream.WriteInt(m_opponentStars);
			stream.WriteInt(m_opponentDestructionPercentage);
			stream.WriteString(m_opponentName);

			if (m_attackerReplayId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteLong(m_attackerReplayId);
				stream.WriteInt(m_attackerReplayShardId);
				stream.WriteInt(m_attackerReplayMajorVersion);
				stream.WriteInt(m_attackerReplayBuildVersion);
				stream.WriteInt(m_attackerReplayContentVersion);
			}
			else
			{
				stream.WriteBoolean(false);
			}

			if (m_opponentReplayId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteLong(m_opponentReplayId);
				stream.WriteInt(m_opponentReplayShardId);
				stream.WriteInt(m_opponentReplayMajorVersion);
				stream.WriteInt(m_opponentReplayBuildVersion);
				stream.WriteInt(m_opponentReplayContentVersion);
			}
			else
			{
				stream.WriteBoolean(false);
			}

			stream.WriteLong(m_matchId);
			stream.WriteString(m_attackerBattleLog);
			stream.WriteString(m_opponentBattleLog);
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_attackerBattleStatus = stream.ReadInt();
			m_attackerStars = stream.ReadInt();
			m_attackerDestructionPercentage = stream.ReadInt();
			m_opponentBattleStatus = stream.ReadInt();
			m_opponentStars = stream.ReadInt();
			m_opponentDestructionPercentage = stream.ReadInt();
			m_opponentName = stream.ReadString(900000);

			if (stream.ReadBoolean())
			{
				m_attackerReplayId = stream.ReadLong();
				m_attackerReplayShardId = stream.ReadInt();
				m_attackerReplayMajorVersion = stream.ReadInt();
				m_attackerReplayBuildVersion = stream.ReadInt();
				m_attackerReplayContentVersion = stream.ReadInt();
			}

			if (stream.ReadBoolean())
			{
				m_opponentReplayId = stream.ReadLong();
				m_opponentReplayShardId = stream.ReadInt();
				m_opponentReplayMajorVersion = stream.ReadInt();
				m_opponentReplayBuildVersion = stream.ReadInt();
				m_opponentReplayContentVersion = stream.ReadInt();
			}

			m_matchId = stream.ReadLong();
			m_attackerBattleLog = stream.ReadString(900000);
			m_opponentBattleLog = stream.ReadString(900000);
		}

		public string GetOpponentName()
			=> m_opponentName;

		public void SetOpponentName(string value)
		{
			m_opponentName = value;
		}

		public LogicLong GetMatchId()
			=> m_matchId;

		public void SetMatchId(LogicLong value)
		{
			m_matchId = value;
		}

		public int GetAttackerBattleStatus()
			=> m_attackerBattleStatus;

		public void SetAttackerBattleStatus(int value)
		{
			m_attackerBattleStatus = value;
		}

		public int GetOpponentBattleStatus()
			=> m_opponentBattleStatus;

		public void SetOpponentBattleStatus(int value)
		{
			m_opponentBattleStatus = value;
		}

		public int GetAttackerStars()
			=> m_attackerStars;

		public void SetAttackerStars(int value)
		{
			m_attackerStars = value;
		}

		public int GetAttackerDestructionPercentage()
			=> m_attackerDestructionPercentage;

		public void SetAttackerDestructionPercentage(int value)
		{
			m_attackerDestructionPercentage = value;
		}

		public int GetOpponentStars()
			=> m_opponentStars;

		public void SetOpponentStars(int value)
		{
			m_opponentStars = value;
		}

		public int GetOpponentDestructionPercentage()
			=> m_opponentDestructionPercentage;

		public void SetOpponentDestructionPercentage(int value)
		{
			m_opponentDestructionPercentage = value;
		}

		public int GetAttackerReplayShardId()
			=> m_attackerReplayShardId;

		public void SetAttackerReplayShardId(int value)
		{
			m_attackerReplayShardId = value;
		}

		public int GetOpponentReplayShardId()
			=> m_opponentReplayShardId;

		public void SetOpponentReplayShardId(int value)
		{
			m_opponentReplayShardId = value;
		}

		public int GetAttackerReplayMajorVersion()
			=> m_attackerReplayMajorVersion;

		public void SetAttackerReplayMajorVersion(int value)
		{
			m_attackerReplayMajorVersion = value;
		}

		public int GetAttackerReplayBuildVersion()
			=> m_attackerReplayBuildVersion;

		public void SetAttackerReplayBuildVersion(int value)
		{
			m_attackerReplayBuildVersion = value;
		}

		public int GetAttackerReplayContentVersion()
			=> m_attackerReplayContentVersion;

		public void SetAttackerReplayContentVersion(int value)
		{
			m_attackerReplayContentVersion = value;
		}

		public int GetOpponentReplayMajorVersion()
			=> m_opponentReplayMajorVersion;

		public void SetOpponentReplayMajorVersion(int value)
		{
			m_opponentReplayMajorVersion = value;
		}

		public int GetOpponentReplayBuildVersion()
			=> m_opponentReplayBuildVersion;

		public void SetOpponentReplayBuildVersion(int value)
		{
			m_opponentReplayBuildVersion = value;
		}

		public int GetOpponentReplayContentVersion()
			=> m_opponentReplayContentVersion;

		public void SetOpponentReplayContentVersion(int value)
		{
			m_opponentReplayContentVersion = value;
		}

		public LogicLong GetAttackerReplayId()
			=> m_attackerReplayId;

		public void SetAttackerReplayId(LogicLong value)
		{
			m_attackerReplayId = value;
		}

		public LogicLong GetOpponentReplayId()
			=> m_opponentReplayId;

		public void SetOpponentReplayId(LogicLong value)
		{
			m_opponentReplayId = value;
		}

		public string GetAttackerBattleLog()
			=> m_attackerBattleLog;

		public void SetAttackerBattleLog(string value)
		{
			m_attackerBattleLog = value;
		}

		public string GetOpponentBattleLog()
			=> m_opponentBattleLog;

		public void SetOpponentBattleLog(string value)
		{
			m_opponentBattleLog = value;
		}

		public LogicLong GetAttackerLiveReplayId()
			=> m_attackerLiveReplayId;

		public void SetAttackerLiveReplayId(LogicLong value)
		{
			m_attackerLiveReplayId = value;
		}

		public LogicLong GetOpponentLiveReplayId()
			=> m_opponentLiveReplayId;

		public void SetOpponentLiveReplayId(LogicLong value)
		{
			m_opponentLiveReplayId = value;
		}

		public override StreamEntryType GetStreamEntryType()
			=> StreamEntryType.VERSUS_BATTLE_REPLAY;

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = jsonObject.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("VersusBattleReplayStreamEntry::load base is NULL");
			}

			base.Load(baseObject);

			m_matchId = new LogicLong(jsonObject.GetJSONNumber("match_id_hi").GetIntValue(), jsonObject.GetJSONNumber("match_id_lo").GetIntValue());
			m_attackerBattleStatus = jsonObject.GetJSONNumber("attacker_state").GetIntValue();
			m_attackerStars = jsonObject.GetJSONNumber("attacker_stars").GetIntValue();
			m_attackerDestructionPercentage = jsonObject.GetJSONNumber("attacker_destruction_percentage").GetIntValue();
			m_opponentBattleStatus = jsonObject.GetJSONNumber("opponent_state").GetIntValue();
			m_opponentStars = jsonObject.GetJSONNumber("opponent_stars").GetIntValue();
			m_opponentDestructionPercentage = jsonObject.GetJSONNumber("opponent_destruction_percentage").GetIntValue();
			m_opponentName = jsonObject.GetJSONString("opponent_name").GetStringValue();

			LogicJSONNumber attackerReplayIdHighNumber = jsonObject.GetJSONNumber("attacker_replay_id_hi");

			if (attackerReplayIdHighNumber != null)
			{
				m_attackerReplayId = new LogicLong(attackerReplayIdHighNumber.GetIntValue(), jsonObject.GetJSONNumber("attacker_replay_id_lo").GetIntValue());
				m_attackerReplayShardId = jsonObject.GetJSONNumber("attacker_replay_shard_id").GetIntValue();
				m_attackerReplayMajorVersion = jsonObject.GetJSONNumber("attacker_replay_major_v").GetIntValue();
				m_attackerReplayBuildVersion = jsonObject.GetJSONNumber("attacker_replay_build_v").GetIntValue();
				m_attackerReplayContentVersion = jsonObject.GetJSONNumber("attacker_replay_content_v").GetIntValue();
			}

			LogicJSONNumber opponentReplayIdHighNumber = jsonObject.GetJSONNumber("opponent_replay_id_hi");

			if (opponentReplayIdHighNumber != null)
			{
				m_opponentReplayId = new LogicLong(opponentReplayIdHighNumber.GetIntValue(), jsonObject.GetJSONNumber("opponent_replay_id_lo").GetIntValue());
				m_opponentReplayShardId = jsonObject.GetJSONNumber("opponent_replay_shard_id").GetIntValue();
				m_opponentReplayMajorVersion = jsonObject.GetJSONNumber("opponent_replay_major_v").GetIntValue();
				m_opponentReplayBuildVersion = jsonObject.GetJSONNumber("opponent_replay_build_v").GetIntValue();
				m_opponentReplayContentVersion = jsonObject.GetJSONNumber("opponent_replay_content_v").GetIntValue();
			}

			m_attackerBattleLog = jsonObject.GetJSONString("attacker_battleLog").GetStringValue();
			m_opponentBattleLog = jsonObject.GetJSONString("opponent_battleLog").GetStringValue();
		}

		public override void Save(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = new LogicJSONObject();

			base.Save(baseObject);

			jsonObject.Put("base", baseObject);
			jsonObject.Put("match_id_hi", new LogicJSONNumber(m_matchId.GetHigherInt()));
			jsonObject.Put("match_id_lo", new LogicJSONNumber(m_matchId.GetLowerInt()));
			jsonObject.Put("attacker_state", new LogicJSONNumber(m_attackerBattleStatus));
			jsonObject.Put("attacker_stars", new LogicJSONNumber(m_attackerStars));
			jsonObject.Put("attacker_destruction_percentage", new LogicJSONNumber(m_attackerDestructionPercentage));
			jsonObject.Put("opponent_state", new LogicJSONNumber(m_opponentBattleStatus));
			jsonObject.Put("opponent_stars", new LogicJSONNumber(m_opponentStars));
			jsonObject.Put("opponent_destruction_percentage", new LogicJSONNumber(m_opponentDestructionPercentage));
			jsonObject.Put("opponent_name", new LogicJSONString(m_opponentName));

			if (m_attackerReplayId != null)
			{
				jsonObject.Put("attacker_replay_id_hi", new LogicJSONNumber(m_attackerReplayId.GetHigherInt()));
				jsonObject.Put("attacker_replay_id_lo", new LogicJSONNumber(m_attackerReplayId.GetLowerInt()));
				jsonObject.Put("attacker_replay_shard_id", new LogicJSONNumber(m_attackerReplayShardId));
				jsonObject.Put("attacker_replay_major_v", new LogicJSONNumber(m_attackerReplayMajorVersion));
				jsonObject.Put("attacker_replay_build_v", new LogicJSONNumber(m_attackerReplayBuildVersion));
				jsonObject.Put("attacker_replay_content_v", new LogicJSONNumber(m_attackerReplayContentVersion));
			}

			if (m_opponentReplayId != null)
			{
				jsonObject.Put("opponent_replay_id_hi", new LogicJSONNumber(m_opponentReplayId.GetHigherInt()));
				jsonObject.Put("opponent_replay_id_lo", new LogicJSONNumber(m_opponentReplayId.GetLowerInt()));
				jsonObject.Put("opponent_replay_shard_id", new LogicJSONNumber(m_opponentReplayShardId));
				jsonObject.Put("opponent_replay_major_v", new LogicJSONNumber(m_opponentReplayMajorVersion));
				jsonObject.Put("opponent_replay_build_v", new LogicJSONNumber(m_opponentReplayBuildVersion));
				jsonObject.Put("opponent_replay_content_v", new LogicJSONNumber(m_opponentReplayContentVersion));
			}

			jsonObject.Put("attacker_battleLog", new LogicJSONString(m_attackerBattleLog));
			jsonObject.Put("opponent_battleLog", new LogicJSONString(m_opponentBattleLog));
		}
	}
}