using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Avatar.Stream
{
	public class BattleReportStreamEntry : AvatarStreamEntry
	{
		private LogicLong m_replayId;

		private readonly AvatarStreamEntryType m_streamType;
		private int m_majorVersion;
		private int m_buildVersion;
		private int m_contentVersion;
		private int m_replayShardId;

		private string m_battleLogJSON;

		private bool m_revengeUsed;

		public BattleReportStreamEntry(AvatarStreamEntryType streamType)
		{
			m_streamType = streamType;
			m_replayShardId = -1;
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteString(m_battleLogJSON);
			stream.WriteBoolean(m_revengeUsed);
			stream.WriteInt(0);
			stream.WriteInt(m_majorVersion);
			stream.WriteInt(m_buildVersion);
			stream.WriteInt(m_contentVersion);

			if (m_replayId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteInt(m_replayShardId);
				stream.WriteLong(m_replayId);
			}
			else
			{
				stream.WriteBoolean(false);
			}
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_battleLogJSON = stream.ReadString(900000);
			m_revengeUsed = stream.ReadBoolean();
			stream.ReadInt();
			m_majorVersion = stream.ReadInt();
			m_buildVersion = stream.ReadInt();
			m_contentVersion = stream.ReadInt();

			if (stream.ReadBoolean())
			{
				m_replayShardId = stream.ReadInt();
				m_replayId = stream.ReadLong();
			}
		}

		public LogicLong GetReplayId()
			=> m_replayId;

		public void SetReplayId(LogicLong value)
		{
			m_replayId = value;
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

		public string GetBattleLogJSON()
			=> m_battleLogJSON;

		public void SetBattleLogJSON(string value)
		{
			m_battleLogJSON = value;
		}

		public bool IsRevengeUsed()
			=> m_revengeUsed;

		public void SetRevengeUsed(bool value)
		{
			m_revengeUsed = value;
		}

		public override AvatarStreamEntryType GetAvatarStreamEntryType()
			=> m_streamType;

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = jsonObject.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("BattleReportStreamEntry::load base is NULL");
			}

			base.Load(baseObject);

			m_battleLogJSON = jsonObject.GetJSONString("battle_log").GetStringValue();
			m_majorVersion = jsonObject.GetJSONNumber("major_v").GetIntValue();
			m_buildVersion = jsonObject.GetJSONNumber("build_v").GetIntValue();
			m_contentVersion = jsonObject.GetJSONNumber("content_v").GetIntValue();
			m_replayShardId = jsonObject.GetJSONNumber("replay_shard_id").GetIntValue();
			m_revengeUsed = jsonObject.GetJSONBoolean("revenge_used").IsTrue();

			LogicJSONNumber replayIdHigh = jsonObject.GetJSONNumber("replay_id_hi");

			if (replayIdHigh != null)
			{
				m_replayId = new LogicLong(replayIdHigh.GetIntValue(), jsonObject.GetJSONNumber("replay_id_lo").GetIntValue());
			}
		}

		public override void Save(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = new LogicJSONObject();

			base.Save(baseObject);

			jsonObject.Put("base", baseObject);
			jsonObject.Put("battle_log", new LogicJSONString(m_battleLogJSON));
			jsonObject.Put("major_v", new LogicJSONNumber(m_majorVersion));
			jsonObject.Put("build_v", new LogicJSONNumber(m_buildVersion));
			jsonObject.Put("content_v", new LogicJSONNumber(m_contentVersion));
			jsonObject.Put("replay_shard_id", new LogicJSONNumber(m_replayShardId));
			jsonObject.Put("revenge_used", new LogicJSONBoolean(m_revengeUsed));

			if (m_replayId != null)
			{
				jsonObject.Put("replay_id_hi", new LogicJSONNumber(m_replayId.GetHigherInt()));
				jsonObject.Put("replay_id_lo", new LogicJSONNumber(m_replayId.GetLowerInt()));
			}
		}
	}
}