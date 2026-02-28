using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public class DuelReplayStreamEntry : StreamEntry
	{
		private LogicLong m_opponentAllianceId;
		private LogicLong m_opponentAvatarId;
		private LogicLong m_opponentHomeId;

		private int m_attackerStars;
		private int m_attackerDestructionPercentage;

		private int m_opponentStars;
		private int m_opponentDestructionPercentage;

		private string m_attackerAllianceName;

		private string m_opponentName;
		private string m_opponentAllianceName;

		private int m_attackerAllianceLevel;
		private int m_attackerAllianceBadgeId;

		private int m_opponentAllianceLevel;
		private int m_opponentAllianceBadgeId;

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

		public DuelReplayStreamEntry()
		{
			m_attackerAllianceBadgeId = -1;
			m_opponentAllianceBadgeId = -1;
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_attackerStars = stream.ReadInt();
			m_attackerDestructionPercentage = stream.ReadInt();
			m_opponentStars = stream.ReadInt();
			m_opponentDestructionPercentage = stream.ReadInt();

			stream.ReadInt();

			m_opponentName = stream.ReadString(900000);

			if (stream.ReadBoolean())
			{
				m_attackerAllianceName = stream.ReadString(900000);
				m_attackerAllianceLevel = stream.ReadInt();
				m_attackerAllianceBadgeId = stream.ReadInt();
			}

			if (stream.ReadBoolean())
			{
				m_opponentAllianceName = stream.ReadString(900000);
				m_opponentAllianceLevel = stream.ReadInt();
				m_opponentAllianceBadgeId = stream.ReadInt();
				m_opponentAllianceId = stream.ReadLong();
			}

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

				if (stream.ReadBoolean())
				{
					m_opponentHomeId = stream.ReadLong();
				}

				if (stream.ReadBoolean())
				{
					m_opponentAvatarId = stream.ReadLong();
				}
			}
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteInt(m_attackerStars);
			stream.WriteInt(m_attackerDestructionPercentage);
			stream.WriteInt(m_opponentStars);
			stream.WriteInt(m_opponentDestructionPercentage);
			stream.WriteInt(-1);
			stream.WriteString(m_opponentName);

			if (m_attackerAllianceName != null)
			{
				stream.WriteBoolean(true);
				stream.WriteString(m_attackerAllianceName);
				stream.WriteInt(m_attackerAllianceBadgeId);
				stream.WriteInt(m_attackerAllianceLevel);
			}
			else
			{
				stream.WriteBoolean(false);
			}

			if (m_opponentAllianceId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteString(m_opponentAllianceName);
				stream.WriteInt(m_opponentAllianceBadgeId);
				stream.WriteInt(m_opponentAllianceLevel);
				stream.WriteLong(m_opponentAllianceId);
			}
			else
			{
				stream.WriteBoolean(false);
			}

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

				if (m_opponentHomeId != null)
				{
					stream.WriteBoolean(true);
					stream.WriteLong(m_opponentHomeId);
				}
				else
				{
					stream.WriteBoolean(false);
				}

				if (m_opponentAvatarId != null)
				{
					stream.WriteBoolean(true);
					stream.WriteLong(m_opponentAvatarId);
				}
				else
				{
					stream.WriteBoolean(false);
				}
			}
			else
			{
				stream.WriteBoolean(false);
			}
		}

		public LogicLong GetOpponentAllianceId()
			=> m_opponentAllianceId;

		public void SetOpponentAllianceId(LogicLong value)
		{
			m_opponentAllianceId = value;
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

		public string GetAttackerAllianceName()
			=> m_attackerAllianceName;

		public void SetAttackerAllianceName(string value)
		{
			m_attackerAllianceName = value;
		}

		public string GetOpponentName()
			=> m_opponentName;

		public void SetOpponentName(string value)
		{
			m_opponentName = value;
		}

		public string GetOpponentAllianceName()
			=> m_opponentAllianceName;

		public void SetOpponentAllianceName(string value)
		{
			m_opponentAllianceName = value;
		}

		public int GetAttackerAllianceLevel()
			=> m_attackerAllianceLevel;

		public void SetAttackerAllianceLevel(int value)
		{
			m_attackerAllianceLevel = value;
		}

		public int GetAttackerAllianceBadgeId()
			=> m_attackerAllianceBadgeId;

		public void SetAttackerAllianceBadgeId(int value)
		{
			m_attackerAllianceBadgeId = value;
		}

		public int GetOpponentAllianceLevel()
			=> m_opponentAllianceLevel;

		public void SetOpponentAllianceLevel(int value)
		{
			m_opponentAllianceLevel = value;
		}

		public int GetOpponentAllianceBadgeId()
			=> m_opponentAllianceBadgeId;

		public void SetOpponentAllianceBadgeId(int value)
		{
			m_opponentAllianceBadgeId = value;
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

		public LogicLong GetOpponentAvatarId()
			=> m_opponentAvatarId;

		public void SetOpponentAvatarId(LogicLong value)
		{
			m_opponentAvatarId = value;
		}

		public LogicLong GetOpponentHomeId()
			=> m_opponentHomeId;

		public void SetOpponentHomeId(LogicLong value)
		{
			m_opponentHomeId = value;
		}

		public override StreamEntryType GetStreamEntryType()
			=> StreamEntryType.DUEL_REPLAY;

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = jsonObject.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("ReplayBattleReplayStreamEntry::load base is NULL");
			}

			base.Load(baseObject);

			m_attackerStars = jsonObject.GetJSONNumber("attacker_stars").GetIntValue();
			m_attackerDestructionPercentage = jsonObject.GetJSONNumber("attacker_destruction_percentage").GetIntValue();
			m_opponentStars = jsonObject.GetJSONNumber("opponent_stars").GetIntValue();
			m_opponentDestructionPercentage = jsonObject.GetJSONNumber("opponent_destruction_percentage").GetIntValue();
			m_opponentName = jsonObject.GetJSONString("opponent_name").GetStringValue();

			LogicJSONObject attackerAllianceObject = jsonObject.GetJSONObject("attacker_alliance");

			if (attackerAllianceObject != null)
			{
				m_attackerAllianceName = attackerAllianceObject.GetJSONString("name").GetStringValue();
				m_attackerAllianceLevel = attackerAllianceObject.GetJSONNumber("level").GetIntValue();
				m_attackerAllianceBadgeId = attackerAllianceObject.GetJSONNumber("badge_id").GetIntValue();
			}

			LogicJSONObject opponentAllianceObject = jsonObject.GetJSONObject("opponent_alliance");

			if (opponentAllianceObject != null)
			{
				m_opponentAllianceId = new LogicLong(opponentAllianceObject.GetJSONNumber("id_hi").GetIntValue(), opponentAllianceObject.GetJSONNumber("id_lo").GetIntValue());
				m_opponentAllianceName = opponentAllianceObject.GetJSONString("name").GetStringValue();
				m_opponentAllianceLevel = opponentAllianceObject.GetJSONNumber("level").GetIntValue();
				m_opponentAllianceBadgeId = opponentAllianceObject.GetJSONNumber("badge_id").GetIntValue();
			}

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

				m_opponentHomeId = new LogicLong(jsonObject.GetJSONNumber("opponent_home_id_high").GetIntValue(),
													  jsonObject.GetJSONNumber("opponent_home_id_low").GetIntValue());
				m_opponentAvatarId = new LogicLong(jsonObject.GetJSONNumber("opponent_avatar_id_high").GetIntValue(),
														jsonObject.GetJSONNumber("opponent_avatar_id_low").GetIntValue());
			}
		}

		public override void Save(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = new LogicJSONObject();

			base.Save(baseObject);

			jsonObject.Put("base", baseObject);
			jsonObject.Put("attacker_stars", new LogicJSONNumber(m_attackerStars));
			jsonObject.Put("attacker_destruction_percentage", new LogicJSONNumber(m_attackerDestructionPercentage));
			jsonObject.Put("opponent_stars", new LogicJSONNumber(m_opponentStars));
			jsonObject.Put("opponent_destruction_percentage", new LogicJSONNumber(m_opponentDestructionPercentage));
			jsonObject.Put("opponent_name", new LogicJSONString(m_opponentName));

			if (m_attackerAllianceName != null)
			{
				LogicJSONObject obj = new LogicJSONObject(3);

				obj.Put("name", new LogicJSONString(m_attackerAllianceName));
				obj.Put("level", new LogicJSONNumber(m_attackerAllianceLevel));
				obj.Put("badge_id", new LogicJSONNumber(m_attackerAllianceBadgeId));

				jsonObject.Put("attacker_alliance", obj);
			}

			if (m_opponentAllianceId != null)
			{
				LogicJSONObject obj = new LogicJSONObject(5);

				obj.Put("id_hi", new LogicJSONNumber(m_opponentAllianceId.GetHigherInt()));
				obj.Put("id_lo", new LogicJSONNumber(m_opponentAllianceId.GetLowerInt()));
				obj.Put("name", new LogicJSONString(m_opponentAllianceName));
				obj.Put("level", new LogicJSONNumber(m_opponentAllianceLevel));
				obj.Put("badge_id", new LogicJSONNumber(m_opponentAllianceBadgeId));

				jsonObject.Put("opponent_alliance", obj);
			}

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

				jsonObject.Put("opponent_home_id_high", new LogicJSONNumber(m_opponentHomeId.GetHigherInt()));
				jsonObject.Put("opponent_home_id_low", new LogicJSONNumber(m_opponentHomeId.GetLowerInt()));
				jsonObject.Put("opponent_avatar_id_high", new LogicJSONNumber(m_opponentAvatarId.GetHigherInt()));
				jsonObject.Put("opponent_avatar_id_low", new LogicJSONNumber(m_opponentAvatarId.GetLowerInt()));
			}
		}
	}
}