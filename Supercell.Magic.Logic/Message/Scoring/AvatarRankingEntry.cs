using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Scoring
{
	public class AvatarRankingEntry : RankingEntry
	{
		private const string JSON_ATTRIBUTE_EXP_LEVEL = "xpLvl";
		private const string JSON_ATTRIBUTE_ATTACK_WIN_COUNT = "attWinCnt";
		private const string JSON_ATTRIBUTE_ATTACK_LOSE_COUNT = "attLoseCnt";
		private const string JSON_ATTRIBUTE_DEFENSE_WIN_COUNT = "defWinCnt";
		private const string JSON_ATTRIBUTE_DEFENSE_LOSE_COUNT = "defLoseCnt";
		private const string JSON_ATTRIBUTE_LEAGUE_TYPE = "leagueT";
		private const string JSON_ATTRIBUTE_ALLIANCE = "alli";
		private const string JSON_ATTRIBUTE_ALLIANCE_ID = "id";
		private const string JSON_ATTRIBUTE_ALLIANCE_NAME = "name";
		private const string JSON_ATTRIBUTE_ALLIANCE_BADGE_ID = "badgeId";

		private int m_expLevel;
		private int m_attackWinCount;
		private int m_attackLoseCount;
		private int m_defenseWinCount;
		private int m_defenseLoseCount;
		private int m_leagueType;
		private int m_allianceBadgeId;

		private string m_country;
		private string m_allianceName;

		private LogicLong m_homeId;
		private LogicLong m_allianceId;

		public AvatarRankingEntry()
		{
			m_allianceBadgeId = -1;
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteInt(m_expLevel);
			stream.WriteInt(m_attackWinCount);
			stream.WriteInt(m_attackLoseCount);
			stream.WriteInt(m_defenseWinCount);
			stream.WriteInt(m_defenseLoseCount);
			stream.WriteInt(m_leagueType);
			stream.WriteString(m_country);
			stream.WriteLong(m_homeId);
			stream.WriteInt(0);
			stream.WriteInt(0);

			if (m_allianceId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteLong(m_allianceId);
				stream.WriteString(m_allianceName);
				stream.WriteInt(m_allianceBadgeId);
			}
			else
			{
				stream.WriteBoolean(false);
			}
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_expLevel = stream.ReadInt();
			m_attackWinCount = stream.ReadInt();
			m_attackLoseCount = stream.ReadInt();
			m_defenseWinCount = stream.ReadInt();
			m_defenseLoseCount = stream.ReadInt();
			m_leagueType = stream.ReadInt();
			m_country = stream.ReadString(900000);
			m_homeId = stream.ReadLong();

			stream.ReadInt();
			stream.ReadInt();

			if (stream.ReadBoolean())
			{
				m_allianceId = stream.ReadLong();
				m_allianceName = stream.ReadString(900000);
				m_allianceBadgeId = stream.ReadInt();
			}
		}

		public int GetExpLevel()
			=> m_expLevel;

		public void SetExpLevel(int value)
		{
			m_expLevel = value;
		}

		public int GetAttackWinCount()
			=> m_attackWinCount;

		public void SetAttackWinCount(int value)
		{
			m_attackWinCount = value;
		}

		public int GetAttackLoseCount()
			=> m_attackLoseCount;

		public void SetAttackLoseCount(int value)
		{
			m_attackLoseCount = value;
		}

		public int GetDefenseWinCount()
			=> m_defenseWinCount;

		public void SetDefenseWinCount(int value)
		{
			m_defenseWinCount = value;
		}

		public int GetDefenseLoseCount()
			=> m_defenseLoseCount;

		public void SetDefenseLoseCount(int value)
		{
			m_defenseLoseCount = value;
		}

		public int GetLeagueType()
			=> m_leagueType;

		public void SetLeagueType(int value)
		{
			m_leagueType = value;
		}

		public int GetAllianceBadgeId()
			=> m_allianceBadgeId;

		public void SetAllianceBadgeId(int value)
		{
			m_allianceBadgeId = value;
		}

		public string GetCountry()
			=> m_country;

		public void SetCountry(string value)
		{
			m_country = value;
		}

		public string GetAllianceName()
			=> m_allianceName;

		public void SetAllianceName(string value)
		{
			m_allianceName = value;
		}

		public LogicLong GetHomeId()
			=> m_homeId;

		public void SetHomeId(LogicLong value)
		{
			m_homeId = value;
		}

		public LogicLong GetAllianceId()
			=> m_allianceId;

		public void SetAllianceId(LogicLong value)
		{
			m_allianceId = value;
		}

		public override LogicJSONObject Save()
		{
			LogicJSONObject jsonObject = base.Save();

			jsonObject.Put(AvatarRankingEntry.JSON_ATTRIBUTE_EXP_LEVEL, new LogicJSONNumber(m_expLevel));
			jsonObject.Put(AvatarRankingEntry.JSON_ATTRIBUTE_ATTACK_WIN_COUNT, new LogicJSONNumber(m_attackWinCount));
			jsonObject.Put(AvatarRankingEntry.JSON_ATTRIBUTE_ATTACK_LOSE_COUNT, new LogicJSONNumber(m_attackLoseCount));
			jsonObject.Put(AvatarRankingEntry.JSON_ATTRIBUTE_DEFENSE_WIN_COUNT, new LogicJSONNumber(m_defenseWinCount));
			jsonObject.Put(AvatarRankingEntry.JSON_ATTRIBUTE_DEFENSE_LOSE_COUNT, new LogicJSONNumber(m_defenseLoseCount));
			jsonObject.Put(AvatarRankingEntry.JSON_ATTRIBUTE_LEAGUE_TYPE, new LogicJSONNumber(m_leagueType));

			if (m_allianceId != null)
			{
				LogicJSONObject allianceObject = new LogicJSONObject();
				LogicJSONArray allianceIdArray = new LogicJSONArray(2);

				allianceIdArray.Add(new LogicJSONNumber(m_allianceId.GetHigherInt()));
				allianceIdArray.Add(new LogicJSONNumber(m_allianceId.GetLowerInt()));

				allianceObject.Put(AvatarRankingEntry.JSON_ATTRIBUTE_ALLIANCE_ID, allianceIdArray);
				allianceObject.Put(AvatarRankingEntry.JSON_ATTRIBUTE_ALLIANCE_NAME, new LogicJSONString(m_allianceName));
				allianceObject.Put(AvatarRankingEntry.JSON_ATTRIBUTE_ALLIANCE_BADGE_ID, new LogicJSONNumber(m_allianceBadgeId));

				jsonObject.Put(AvatarRankingEntry.JSON_ATTRIBUTE_ALLIANCE, allianceObject);
			}

			return jsonObject;
		}

		public override void Load(LogicJSONObject jsonObject)
		{
			base.Load(jsonObject);

			m_expLevel = jsonObject.GetJSONNumber(AvatarRankingEntry.JSON_ATTRIBUTE_EXP_LEVEL).GetIntValue();
			m_attackWinCount = jsonObject.GetJSONNumber(AvatarRankingEntry.JSON_ATTRIBUTE_ATTACK_WIN_COUNT).GetIntValue();
			m_attackLoseCount = jsonObject.GetJSONNumber(AvatarRankingEntry.JSON_ATTRIBUTE_ATTACK_LOSE_COUNT).GetIntValue();
			m_defenseWinCount = jsonObject.GetJSONNumber(AvatarRankingEntry.JSON_ATTRIBUTE_DEFENSE_WIN_COUNT).GetIntValue();
			m_defenseLoseCount = jsonObject.GetJSONNumber(AvatarRankingEntry.JSON_ATTRIBUTE_DEFENSE_LOSE_COUNT).GetIntValue();
			m_leagueType = jsonObject.GetJSONNumber(AvatarRankingEntry.JSON_ATTRIBUTE_LEAGUE_TYPE).GetIntValue();

			LogicJSONObject allianceObject = jsonObject.GetJSONObject(AvatarRankingEntry.JSON_ATTRIBUTE_ALLIANCE);

			if (allianceObject != null)
			{
				LogicJSONArray allianceIdArray = allianceObject.GetJSONArray(AvatarRankingEntry.JSON_ATTRIBUTE_ALLIANCE_ID);

				m_allianceId = new LogicLong(allianceIdArray.GetJSONNumber(0).GetIntValue(), allianceIdArray.GetJSONNumber(1).GetIntValue());
				m_allianceName = allianceObject.GetJSONString(AvatarRankingEntry.JSON_ATTRIBUTE_ALLIANCE_NAME).GetStringValue();
				m_allianceBadgeId = allianceObject.GetJSONNumber(AvatarRankingEntry.JSON_ATTRIBUTE_ALLIANCE_BADGE_ID).GetIntValue();
			}

			m_homeId = GetId().Clone();
		}
	}
}