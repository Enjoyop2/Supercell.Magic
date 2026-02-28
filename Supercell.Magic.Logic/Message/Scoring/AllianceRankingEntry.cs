using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Message.Scoring
{
	public class AllianceRankingEntry : RankingEntry
	{
		public const string JSON_ATTRIBUTE_BADGE_ID = "badgeId";
		public const string JSON_ATTRIBUTE_EXP_LEVEL = "xpLvl";
		public const string JSON_ATTRIBUTE_MEMBER_COUNT = "memberCnt";
		public const string JSON_ATTRIBUTE_ORIGIN = "origin";

		private int m_badgeId;
		private int m_expLevel;
		private int m_memberCount;

		private LogicData m_originData;

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteInt(m_badgeId);
			stream.WriteInt(m_memberCount);
			ByteStreamHelper.WriteDataReference(stream, m_originData);
			stream.WriteInt(m_expLevel);
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_badgeId = stream.ReadInt();
			m_memberCount = stream.ReadInt();
			m_originData = ByteStreamHelper.ReadDataReference(stream);
			m_expLevel = stream.ReadInt();
		}

		public int GetAllianceBadgeId()
			=> m_badgeId;

		public void SetAllianceBadgeId(int value)
		{
			m_badgeId = value;
		}

		public int GetMemberCount()
			=> m_memberCount;

		public void SetMemberCount(int value)
		{
			m_memberCount = value;
		}

		public int GetAllianceLevel()
			=> m_expLevel;

		public void SetAllianceLevel(int value)
		{
			m_expLevel = value;
		}

		public LogicData GetOriginData()
			=> m_originData;

		public void SetOriginData(LogicData data)
		{
			m_originData = data;
		}

		public override LogicJSONObject Save()
		{
			LogicJSONObject jsonObject = base.Save();

			jsonObject.Put(AllianceRankingEntry.JSON_ATTRIBUTE_BADGE_ID, new LogicJSONNumber(m_badgeId));
			jsonObject.Put(AllianceRankingEntry.JSON_ATTRIBUTE_EXP_LEVEL, new LogicJSONNumber(m_expLevel));
			jsonObject.Put(AllianceRankingEntry.JSON_ATTRIBUTE_MEMBER_COUNT, new LogicJSONNumber(m_memberCount));

			if (m_originData != null)
				jsonObject.Put(AllianceRankingEntry.JSON_ATTRIBUTE_ORIGIN, new LogicJSONNumber(m_originData.GetGlobalID()));

			return jsonObject;
		}

		public override void Load(LogicJSONObject jsonObject)
		{
			base.Load(jsonObject);

			m_badgeId = jsonObject.GetJSONNumber(AllianceRankingEntry.JSON_ATTRIBUTE_BADGE_ID).GetIntValue();
			m_expLevel = jsonObject.GetJSONNumber(AllianceRankingEntry.JSON_ATTRIBUTE_EXP_LEVEL).GetIntValue();
			m_memberCount = jsonObject.GetJSONNumber(AllianceRankingEntry.JSON_ATTRIBUTE_MEMBER_COUNT).GetIntValue();

			LogicJSONNumber originNumber = jsonObject.GetJSONNumber(AllianceRankingEntry.JSON_ATTRIBUTE_ORIGIN);

			if (originNumber != null)
				m_originData = LogicDataTables.GetDataById(originNumber.GetIntValue());
		}
	}
}