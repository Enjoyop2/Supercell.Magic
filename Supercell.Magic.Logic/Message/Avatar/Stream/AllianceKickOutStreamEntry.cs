using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.Message.Avatar.Stream
{
	public class AllianceKickOutStreamEntry : AvatarStreamEntry
	{
		private LogicLong m_allianceId;
		private LogicLong m_senderHomeId;

		private string m_message;
		private string m_allianceName;

		private int m_allianceBadgeId;

		public AllianceKickOutStreamEntry()
		{
			m_allianceBadgeId = -1;
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteString(m_message);
			stream.WriteLong(m_allianceId);
			stream.WriteString(m_allianceName);
			stream.WriteInt(m_allianceBadgeId);

			if (m_senderHomeId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteLong(m_senderHomeId);
			}
			else
			{
				stream.WriteBoolean(false);
			}
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_message = stream.ReadString(900000);
			m_allianceId = stream.ReadLong();
			m_allianceName = stream.ReadString(900000);
			m_allianceBadgeId = stream.ReadInt();

			if (stream.ReadBoolean())
			{
				m_senderHomeId = stream.ReadLong();
			}
		}

		public LogicLong GetAllianceId()
			=> m_allianceId;

		public void SetAllianceId(LogicLong allianceId)
		{
			m_allianceId = allianceId;
		}

		public LogicLong GetSenderHomeId()
			=> m_senderHomeId;

		public void SetSenderHomeId(LogicLong allianceId)
		{
			m_senderHomeId = allianceId;
		}

		public string GetAllianceName()
			=> m_allianceName;

		public void SetAllianceName(string name)
		{
			m_allianceName = name;
		}

		public int GetAllianceBadgeId()
			=> m_allianceBadgeId;

		public void SetAllianceBadgeId(int value)
		{
			m_allianceBadgeId = value;
		}

		public string GetMessage()
			=> m_message;

		public void SetMessage(string message)
		{
			m_message = message;
		}

		public override AvatarStreamEntryType GetAvatarStreamEntryType()
			=> AvatarStreamEntryType.ALLIANCE_KICKOUT;

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = jsonObject.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("AllianceKickOutStreamEntry::load base is NULL");
			}

			base.Load(baseObject);

			LogicJSONNumber allianceIdHighNumber = jsonObject.GetJSONNumber("alli_id_high");
			LogicJSONNumber allianceIdLowNumber = jsonObject.GetJSONNumber("alli_id_low");

			if (allianceIdHighNumber != null && allianceIdLowNumber != null)
			{
				m_allianceId = new LogicLong(allianceIdHighNumber.GetIntValue(), allianceIdLowNumber.GetIntValue());
			}

			m_allianceName = LogicJSONHelper.GetString(jsonObject, "alli_name");
			m_allianceBadgeId = LogicJSONHelper.GetInt(jsonObject, "alli_badge_id");
			m_message = LogicJSONHelper.GetString(jsonObject, "message");

			LogicJSONNumber senderIdHighNumber = jsonObject.GetJSONNumber("sender_id_high");
			LogicJSONNumber senderIdLowNumber = jsonObject.GetJSONNumber("sender_id_low");

			if (senderIdHighNumber != null && senderIdLowNumber != null)
			{
				m_senderHomeId = new LogicLong(senderIdHighNumber.GetIntValue(), senderIdLowNumber.GetIntValue());
			}
		}

		public override void Save(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = new LogicJSONObject();

			base.Save(baseObject);

			jsonObject.Put("base", baseObject);
			jsonObject.Put("alli_id_high", new LogicJSONNumber(m_allianceId.GetHigherInt()));
			jsonObject.Put("alli_id_low", new LogicJSONNumber(m_allianceId.GetLowerInt()));
			jsonObject.Put("alli_name", new LogicJSONString(m_allianceName));
			jsonObject.Put("alli_badge_id", new LogicJSONNumber(m_allianceBadgeId));
			jsonObject.Put("message", new LogicJSONString(m_message));

			if (m_senderHomeId != null)
			{
				jsonObject.Put("sender_id_high", new LogicJSONNumber(m_senderHomeId.GetHigherInt()));
				jsonObject.Put("sender_id_low", new LogicJSONNumber(m_senderHomeId.GetLowerInt()));
			}
		}
	}
}