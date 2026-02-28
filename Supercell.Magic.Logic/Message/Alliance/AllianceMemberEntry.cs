using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class AllianceMemberEntry
	{
		private LogicLong m_avatarId;
		private LogicLong m_homeId;

		private string m_name;

		private LogicAvatarAllianceRole m_allianceRole;
		private int m_expLevel;
		private int m_leagueType;
		private int m_score;
		private int m_duelScore;
		private int m_donationCount;
		private int m_receivedDonationCount;
		private int m_order;
		private int m_previousOrder;
		private int m_orderVillage2;
		private int m_previousOrderVillage2;
		private int m_createdTime;
		private int m_warCooldown;
		private int m_warPreference;

		public void Decode(ByteStream stream)
		{
			m_avatarId = stream.ReadLong();
			m_name = stream.ReadString(900000);
			m_allianceRole = (LogicAvatarAllianceRole)stream.ReadInt();
			m_expLevel = stream.ReadInt();
			m_leagueType = stream.ReadInt();
			m_score = stream.ReadInt();
			m_duelScore = stream.ReadInt();
			m_donationCount = stream.ReadInt();
			m_receivedDonationCount = stream.ReadInt();
			m_order = stream.ReadInt();
			m_previousOrder = stream.ReadInt();
			m_orderVillage2 = stream.ReadInt();
			m_previousOrderVillage2 = stream.ReadInt();
			m_createdTime = stream.ReadInt();
			m_warCooldown = stream.ReadInt();
			m_warPreference = stream.ReadInt();

			if (stream.ReadBoolean())
			{
				m_homeId = stream.ReadLong();
			}
		}

		public void Encode(ByteStream stream)
		{
			stream.WriteLong(m_avatarId);
			stream.WriteString(m_name);
			stream.WriteInt((int)m_allianceRole);
			stream.WriteInt(m_expLevel);
			stream.WriteInt(m_leagueType);
			stream.WriteInt(m_score);
			stream.WriteInt(m_duelScore);
			stream.WriteInt(m_donationCount);
			stream.WriteInt(m_receivedDonationCount);
			stream.WriteInt(m_order);
			stream.WriteInt(m_previousOrder);
			stream.WriteInt(m_orderVillage2);
			stream.WriteInt(m_previousOrderVillage2);
			stream.WriteInt(m_createdTime);
			stream.WriteInt(m_warCooldown);
			stream.WriteInt(m_warPreference);

			if (m_homeId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteLong(m_homeId);
			}
			else
			{
				stream.WriteBoolean(false);
			}
		}

		public LogicLong GetAvatarId()
			=> m_avatarId;

		public void SetAvatarId(LogicLong value)
		{
			m_avatarId = value;
		}

		public LogicLong GetHomeId()
			=> m_homeId;

		public void SetHomeId(LogicLong value)
		{
			m_homeId = value;
		}

		public string GetName()
			=> m_name;

		public void SetName(string name)
		{
			m_name = name;
		}

		public LogicAvatarAllianceRole GetAllianceRole()
			=> m_allianceRole;

		public void SetAllianceRole(LogicAvatarAllianceRole value)
		{
			m_allianceRole = value;
		}

		public int GetExpLevel()
			=> m_expLevel;

		public void SetExpLevel(int value)
		{
			m_expLevel = value;
		}

		public int GetLeagueType()
			=> m_leagueType;

		public void SetLeagueType(int value)
		{
			m_leagueType = value;
		}

		public int GetScore()
			=> m_score;

		public void SetScore(int value)
		{
			m_score = value;
		}

		public int GetDuelScore()
			=> m_duelScore;

		public void SetDuelScore(int value)
		{
			m_duelScore = value;
		}

		public int GetDonations()
			=> m_donationCount;

		public void SetDonations(int value)
		{
			m_donationCount = value;
		}

		public int GetReceivedDonations()
			=> m_receivedDonationCount;

		public void SetReceivedDonations(int value)
		{
			m_receivedDonationCount = value;
		}

		public int GetOrder()
			=> m_order;

		public void SetOrder(int order)
		{
			m_order = order;
		}

		public int GetPreviousOrder()
			=> m_previousOrder;

		public void SetPreviousOrder(int order)
		{
			m_previousOrder = order;
		}

		public int GetOrderVillage2()
			=> m_orderVillage2;

		public void SetOrderVillage2(int order)
		{
			m_orderVillage2 = order;
		}

		public int GetPreviousOrderVillage2()
			=> m_previousOrderVillage2;

		public void SetPreviousOrderVillage2(int order)
		{
			m_previousOrderVillage2 = order;
		}

		public bool IsNewMember()
			=> m_createdTime < 259200;

		public int GetCreatedTime()
			=> m_createdTime;

		public void SetCreatedTime(int value)
		{
			m_createdTime = value;
		}

		public int GetWarCooldown()
			=> m_warCooldown;

		public void SetWarCooldown(int value)
		{
			m_warCooldown = value;
		}

		public int GetWarPreference()
			=> m_warPreference;

		public void SetWarPreference(int value)
		{
			m_warPreference = value;
		}

		public bool HasLowerRoleThan(LogicAvatarAllianceRole role)
		{
			switch (role)
			{
				case LogicAvatarAllianceRole.LEADER:
					return m_allianceRole != LogicAvatarAllianceRole.LEADER;
				case LogicAvatarAllianceRole.ELDER:
					return m_allianceRole == LogicAvatarAllianceRole.MEMBER;
				case LogicAvatarAllianceRole.CO_LEADER:
					return m_allianceRole != LogicAvatarAllianceRole.LEADER &&
						   m_allianceRole != LogicAvatarAllianceRole.CO_LEADER;
				default:
					return false;
			}
		}

		public static bool HasLowerRole(LogicAvatarAllianceRole role1, LogicAvatarAllianceRole role2)
		{
			switch (role2)
			{
				case LogicAvatarAllianceRole.LEADER:
					return role1 != LogicAvatarAllianceRole.LEADER;
				case LogicAvatarAllianceRole.ELDER:
					return role1 == LogicAvatarAllianceRole.MEMBER;
				case LogicAvatarAllianceRole.CO_LEADER:
					return role1 != LogicAvatarAllianceRole.LEADER &&
						   role1 != LogicAvatarAllianceRole.CO_LEADER;
				default:
					return false;
			}
		}

		public void Load(LogicJSONObject jsonObject)
		{
			LogicJSONNumber avatarIdHighObject = jsonObject.GetJSONNumber("avatar_id_high");
			LogicJSONNumber avatarIdLowObject = jsonObject.GetJSONNumber("avatar_id_low");

			if (avatarIdHighObject != null && avatarIdLowObject != null)
			{
				m_avatarId = new LogicLong(avatarIdHighObject.GetIntValue(), avatarIdLowObject.GetIntValue());
			}

			LogicJSONNumber homeIdHighObject = jsonObject.GetJSONNumber("home_id_high");
			LogicJSONNumber homeIdLowObject = jsonObject.GetJSONNumber("home_id_low");

			if (homeIdHighObject != null && homeIdLowObject != null)
			{
				m_homeId = new LogicLong(homeIdHighObject.GetIntValue(), homeIdLowObject.GetIntValue());
			}

			m_name = LogicJSONHelper.GetString(jsonObject, "name");
			m_allianceRole = (LogicAvatarAllianceRole)LogicJSONHelper.GetInt(jsonObject, "alliance_role");
			m_expLevel = LogicJSONHelper.GetInt(jsonObject, "xp_level");
			m_leagueType = LogicJSONHelper.GetInt(jsonObject, "league_type");
			m_score = LogicJSONHelper.GetInt(jsonObject, "score");
			m_duelScore = LogicJSONHelper.GetInt(jsonObject, "duel_score");
			m_donationCount = LogicJSONHelper.GetInt(jsonObject, "donations");
			m_receivedDonationCount = LogicJSONHelper.GetInt(jsonObject, "received_donations");
			m_order = LogicJSONHelper.GetInt(jsonObject, "order");
			m_previousOrder = LogicJSONHelper.GetInt(jsonObject, "prev_order");
			m_orderVillage2 = LogicJSONHelper.GetInt(jsonObject, "order_v2");
			m_previousOrderVillage2 = LogicJSONHelper.GetInt(jsonObject, "prev_order_v2");
			m_warCooldown = LogicJSONHelper.GetInt(jsonObject, "war_cooldown");
			m_warPreference = LogicJSONHelper.GetInt(jsonObject, "war_preference");
		}

		public LogicJSONObject Save()
		{
			LogicJSONObject jsonObject = new LogicJSONObject();

			jsonObject.Put("avatar_id_high", new LogicJSONNumber(m_avatarId.GetHigherInt()));
			jsonObject.Put("avatar_id_low", new LogicJSONNumber(m_avatarId.GetLowerInt()));

			if (m_homeId != null)
			{
				jsonObject.Put("home_id_high", new LogicJSONNumber(m_homeId.GetHigherInt()));
				jsonObject.Put("home_id_low", new LogicJSONNumber(m_homeId.GetLowerInt()));
			}

			jsonObject.Put("name", new LogicJSONString(m_name));
			jsonObject.Put("alliance_role", new LogicJSONNumber((int)m_allianceRole));
			jsonObject.Put("xp_level", new LogicJSONNumber(m_expLevel));
			jsonObject.Put("league_type", new LogicJSONNumber(m_leagueType));
			jsonObject.Put("score", new LogicJSONNumber(m_score));
			jsonObject.Put("duel_score", new LogicJSONNumber(m_duelScore));
			jsonObject.Put("donations", new LogicJSONNumber(m_donationCount));
			jsonObject.Put("received_donations", new LogicJSONNumber(m_receivedDonationCount));
			jsonObject.Put("order", new LogicJSONNumber(m_order));
			jsonObject.Put("prev_order", new LogicJSONNumber(m_previousOrder));
			jsonObject.Put("order_v2", new LogicJSONNumber(m_orderVillage2));
			jsonObject.Put("prev_order_v2", new LogicJSONNumber(m_previousOrderVillage2));
			jsonObject.Put("war_cooldown", new LogicJSONNumber(m_warCooldown));
			jsonObject.Put("war_preference", new LogicJSONNumber(m_warPreference));

			return jsonObject;
		}
	}
}