using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Message.Avatar.Stream
{
	public class AdminMessageAvatarStreamEntry : AvatarStreamEntry
	{
		private string m_titleTID;
		private string m_descriptionTID;
		private string m_buttonTID;
		private string m_helpshiftLink;
		private string m_urlLink;

		private int m_diamondCount;

		private bool m_supportMessage;
		private bool m_claimed;

		public AdminMessageAvatarStreamEntry()
		{
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteBoolean(false);
			stream.WriteString(m_titleTID);
			stream.WriteString(m_descriptionTID);
			stream.WriteString(m_helpshiftLink);
			stream.WriteString(m_urlLink);
			stream.WriteString(m_buttonTID);
			stream.WriteBoolean(m_supportMessage);
			stream.WriteInt(m_diamondCount);
			stream.WriteBoolean(m_claimed);
			stream.WriteInt(0);
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			stream.ReadBoolean();

			m_titleTID = stream.ReadString(900000);
			m_descriptionTID = stream.ReadString(900000);
			m_helpshiftLink = stream.ReadString(900000);
			m_urlLink = stream.ReadString(900000);
			m_buttonTID = stream.ReadString(900000);
			m_supportMessage = stream.ReadBoolean();
			m_diamondCount = stream.ReadInt();
			m_claimed = stream.ReadBoolean();

			stream.ReadInt();
		}

		public override AvatarStreamEntryType GetAvatarStreamEntryType()
			=> AvatarStreamEntryType.ADMIN_MESSAGE;

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = jsonObject.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("AllianceInvitationAvatarStreamEntry::load base is NULL");
			}

			base.Load(baseObject);

			m_titleTID = jsonObject.GetJSONString("title").GetStringValue();
			m_descriptionTID = jsonObject.GetJSONString("description").GetStringValue();

			LogicJSONString buttonString = jsonObject.GetJSONString("button");

			if (buttonString != null)
			{
				m_buttonTID = buttonString.GetStringValue();
			}

			LogicJSONString helpshiftUrlString = jsonObject.GetJSONString("helpshift_url");

			if (helpshiftUrlString != null)
			{
				m_helpshiftLink = helpshiftUrlString.GetStringValue();
			}

			LogicJSONString urlString = jsonObject.GetJSONString("url");

			if (urlString != null)
			{
				m_urlLink = urlString.GetStringValue();
			}

			LogicJSONNumber diamondsNumber = jsonObject.GetJSONNumber("diamonds");

			if (diamondsNumber != null)
			{
				m_diamondCount = diamondsNumber.GetIntValue();
			}

			LogicJSONBoolean supportMessageBoolean = jsonObject.GetJSONBoolean("support_msg");

			if (supportMessageBoolean != null)
			{
				m_supportMessage = supportMessageBoolean.IsTrue();
			}

			LogicJSONBoolean claimedBoolean = jsonObject.GetJSONBoolean("claimed");

			if (claimedBoolean != null)
			{
				m_claimed = claimedBoolean.IsTrue();
			}
		}

		public override void Save(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = new LogicJSONObject();

			base.Save(baseObject);

			jsonObject.Put("base", baseObject);
			jsonObject.Put("title", new LogicJSONString(m_titleTID));
			jsonObject.Put("description", new LogicJSONString(m_descriptionTID));

			if (m_buttonTID != null)
			{
				jsonObject.Put("button", new LogicJSONString(m_buttonTID));
			}

			if (m_helpshiftLink != null)
			{
				jsonObject.Put("helpshift_url", new LogicJSONString(m_helpshiftLink));
			}

			if (m_urlLink != null)
			{
				jsonObject.Put("url", new LogicJSONString(m_urlLink));
			}

			if (m_diamondCount != 0)
			{
				jsonObject.Put("diamonds", new LogicJSONNumber(m_diamondCount));
			}

			if (m_supportMessage)
			{
				jsonObject.Put("support_msg", new LogicJSONBoolean(m_supportMessage));
			}

			if (m_claimed)
			{
				jsonObject.Put("claimed", new LogicJSONBoolean(m_claimed));
			}
		}

		public string GetTitleTID()
			=> m_titleTID;

		public void SetTitleTID(string value)
		{
			m_titleTID = value;
		}

		public string GetDescriptionTID()
			=> m_descriptionTID;

		public void SetDescriptionTID(string value)
		{
			m_descriptionTID = value;
		}

		public string GetButtonTID()
			=> m_buttonTID;

		public void SetButtonTID(string value)
		{
			m_buttonTID = value;
		}

		public string GetHelpshiftLink()
			=> m_helpshiftLink;

		public void SetHelpshiftLink(string value)
		{
			m_helpshiftLink = value;
		}

		public string GetUrlLink()
			=> m_urlLink;

		public void SetUrlLink(string value)
		{
			m_urlLink = value;
		}

		public int GetDiamondCount()
			=> m_diamondCount;

		public void SetDiamondCount(int value)
		{
			m_diamondCount = value;
		}

		public bool IsSupportMessage()
			=> m_supportMessage;

		public void SetSupportMessage(bool value)
		{
			m_supportMessage = value;
		}

		public bool IsClaimed()
			=> m_claimed;

		public void SetClaimed(bool value)
		{
			m_claimed = value;
		}
	}
}