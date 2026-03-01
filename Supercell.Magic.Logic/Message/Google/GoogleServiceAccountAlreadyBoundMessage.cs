using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Google
{
	public class GoogleServiceAccountAlreadyBoundMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24262;

		private LogicLong m_accountId;
		private LogicClientAvatar m_avatar;

		private string m_googleServiceId;
		private string m_passToken;

		public GoogleServiceAccountAlreadyBoundMessage() : this(0)
		{
			// GoogleServiceAccountAlreadyBoundMessage.
		}

		public GoogleServiceAccountAlreadyBoundMessage(short messageVersion) : base(messageVersion)
		{
			// GoogleServiceAccountAlreadyBoundMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_googleServiceId = m_stream.ReadString(900000);

			if (m_stream.ReadBoolean())
			{
				m_accountId = m_stream.ReadLong();
			}

			m_passToken = m_stream.ReadString(900000);
			m_avatar = new LogicClientAvatar();
			m_avatar.Decode(m_stream);
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteString(m_googleServiceId);

			if (!m_accountId.IsZero())
			{
				m_stream.WriteBoolean(true);
				m_stream.WriteLong(m_accountId);
			}
			else
			{
				m_stream.WriteBoolean(false);
			}

			m_stream.WriteString(m_passToken);
			m_avatar.Encode(m_stream);
		}

		public override short GetMessageType()
			=> 24262;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();

			m_googleServiceId = null;
			m_passToken = null;
			m_avatar = null;
		}

		public string RemoveGoogleServiceId()
		{
			string tmp = m_googleServiceId;
			m_googleServiceId = null;
			return tmp;
		}

		public void SetGoogleServiceId(string value)
		{
			m_googleServiceId = value;
		}

		public string RemovePassToken()
		{
			string tmp = m_passToken;
			m_passToken = null;
			return tmp;
		}

		public void SetPassToken(string value)
		{
			m_passToken = value;
		}

		public LogicLong RemoveAccountId()
		{
			LogicLong tmp = m_accountId;
			m_accountId = null;
			return tmp;
		}

		public void SetAccountId(LogicLong value)
		{
			m_accountId = value;
		}

		public LogicClientAvatar RemoveLogicClientAvatar()
		{
			LogicClientAvatar tmp = m_avatar;
			m_avatar = null;
			return tmp;
		}

		public void SetAvatar(LogicClientAvatar avatar)
		{
			m_avatar = avatar;
		}
	}
}