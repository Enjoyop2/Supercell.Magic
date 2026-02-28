using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Avatar.Attack
{
	public class Village2BattleProgressAttackEntry : Village2AttackEntry
	{
		private int m_resultType;

		private int m_attackerStars;
		private int m_attackerDestructionPercentage;

		private int m_opponentStars;
		private int m_opponentDestructionPercentage;

		private int m_goldCount;
		private int m_elixirCount;
		private int m_scoreCount;

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

		private bool m_battleEnded;

		public override void Encode(ChecksumEncoder encoder)
		{
			base.Encode(encoder);

			encoder.WriteVInt(m_resultType);
			encoder.WriteInt(m_attackerStars);
			encoder.WriteInt(m_attackerDestructionPercentage);
			encoder.WriteInt(m_opponentStars);
			encoder.WriteInt(m_opponentDestructionPercentage);
			encoder.WriteInt(m_goldCount);
			encoder.WriteInt(m_elixirCount);
			encoder.WriteInt(m_scoreCount);
			encoder.WriteBoolean(m_battleEnded);

			if (m_attackerReplayId != null)
			{
				encoder.WriteBoolean(true);
				encoder.WriteLong(m_attackerReplayId);
				encoder.WriteInt(m_attackerReplayShardId);
				encoder.WriteInt(m_attackerReplayMajorVersion);
				encoder.WriteInt(m_attackerReplayBuildVersion);
				encoder.WriteInt(m_attackerReplayContentVersion);
			}
			else
			{
				encoder.WriteBoolean(false);
			}

			if (m_opponentReplayId != null)
			{
				encoder.WriteBoolean(true);
				encoder.WriteLong(m_opponentReplayId);
				encoder.WriteInt(m_opponentReplayShardId);
				encoder.WriteInt(m_opponentReplayMajorVersion);
				encoder.WriteInt(m_opponentReplayBuildVersion);
				encoder.WriteInt(m_opponentReplayContentVersion);
			}
			else
			{
				encoder.WriteBoolean(false);
			}

			encoder.WriteInt(0);
			encoder.WriteString(m_attackerBattleLog);
			encoder.WriteString(m_opponentBattleLog);
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_resultType = stream.ReadVInt();

			m_attackerStars = stream.ReadInt();
			m_attackerDestructionPercentage = stream.ReadInt();
			m_opponentStars = stream.ReadInt();
			m_opponentDestructionPercentage = stream.ReadInt();
			m_goldCount = stream.ReadInt();
			m_elixirCount = stream.ReadInt();
			m_scoreCount = stream.ReadInt();
			m_battleEnded = stream.ReadBoolean();

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

			stream.ReadInt();

			m_attackerBattleLog = stream.ReadString(900000);
			m_opponentBattleLog = stream.ReadString(900000);
		}

		public override int GetAttackEntryType()
			=> Village2AttackEntry.ATTACK_ENTRY_TYPE_BATTLE_PROGRESS;

		public bool IsBattleEnded()
			=> m_battleEnded;

		public void SetBattleEnded(bool ended)
		{
			m_battleEnded = ended;
		}

		public int GetResultType()
			=> m_resultType;

		public void SetResultType(int value)
		{
			m_resultType = value;
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

		public int GetGoldCount()
			=> m_goldCount;

		public void SetGoldCount(int value)
		{
			m_goldCount = value;
		}

		public int GetElixirCount()
			=> m_elixirCount;

		public void SetElixirCount(int value)
		{
			m_elixirCount = value;
		}

		public int GetScoreCount()
			=> m_scoreCount;

		public void SetScoreCount(int value)
		{
			m_scoreCount = value;
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

		public override void Load(LogicJSONObject jsonObject)
		{
			base.Load(jsonObject.GetJSONObject("base"));

			m_resultType = jsonObject.GetJSONNumber("battle_result").GetIntValue();
			m_attackerStars = jsonObject.GetJSONNumber("attacker_stars").GetIntValue();
			m_attackerDestructionPercentage = jsonObject.GetJSONNumber("attacker_destruction_percentage").GetIntValue();
			m_opponentStars = jsonObject.GetJSONNumber("opponent_stars").GetIntValue();
			m_opponentDestructionPercentage = jsonObject.GetJSONNumber("opponent_destruction_percentage").GetIntValue();
			m_goldCount = jsonObject.GetJSONNumber("golds").GetIntValue();
			m_elixirCount = jsonObject.GetJSONNumber("elixirs").GetIntValue();
			m_scoreCount = jsonObject.GetJSONNumber("scores").GetIntValue();
			m_battleEnded = jsonObject.GetJSONBoolean("battle_ended").IsTrue();

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
				m_attackerReplayId = new LogicLong(opponentReplayIdHighNumber.GetIntValue(), jsonObject.GetJSONNumber("opponent_replay_id_lo").GetIntValue());
				m_attackerReplayShardId = jsonObject.GetJSONNumber("opponent_replay_shard_id").GetIntValue();
				m_attackerReplayMajorVersion = jsonObject.GetJSONNumber("opponent_replay_major_v").GetIntValue();
				m_attackerReplayBuildVersion = jsonObject.GetJSONNumber("opponent_replay_build_v").GetIntValue();
				m_attackerReplayContentVersion = jsonObject.GetJSONNumber("opponent_replay_content_v").GetIntValue();
			}

			m_attackerBattleLog = jsonObject.GetJSONString("attacker_battleLog").GetStringValue();
			m_opponentBattleLog = jsonObject.GetJSONString("opponent_battleLog").GetStringValue();
		}

		public override void Save(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = new LogicJSONObject();

			base.Save(baseObject);

			jsonObject.Put("base", baseObject);
			jsonObject.Put("battle_result", new LogicJSONNumber(m_resultType));
			jsonObject.Put("attacker_stars", new LogicJSONNumber(m_attackerStars));
			jsonObject.Put("attacker_destruction_percentage", new LogicJSONNumber(m_attackerDestructionPercentage));
			jsonObject.Put("opponent_stars", new LogicJSONNumber(m_opponentStars));
			jsonObject.Put("opponent_destruction_percentage", new LogicJSONNumber(m_opponentDestructionPercentage));
			jsonObject.Put("golds", new LogicJSONNumber(m_goldCount));
			jsonObject.Put("elixirs", new LogicJSONNumber(m_elixirCount));
			jsonObject.Put("scores", new LogicJSONNumber(m_scoreCount));
			jsonObject.Put("battle_ended", new LogicJSONBoolean(m_battleEnded));

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