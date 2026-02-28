using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Facebook
{
	public class BindFacebookAccountMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14201;

		private bool m_force;
		private string m_googleServiceId;
		private string m_authToken;

		public BindFacebookAccountMessage() : this(0)
		{
			// BindFacebookAccountMessage.
		}

		public BindFacebookAccountMessage(short messageVersion) : base(messageVersion)
		{
			// BindFacebookAccountMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_force = m_stream.ReadBoolean();
			m_googleServiceId = m_stream.ReadString(900000);
			m_authToken = m_stream.ReadString(900000);
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteBoolean(m_force);
			m_stream.WriteString(m_googleServiceId);
			m_stream.WriteString(m_authToken);
		}

		public override short GetMessageType()
			=> BindFacebookAccountMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();

			m_googleServiceId = null;
			m_authToken = null;
		}

		public string RemoveFacebookId()
		{
			string tmp = m_googleServiceId;
			m_googleServiceId = null;
			return tmp;
		}

		public void SetFacebookId(string value)
		{
			m_googleServiceId = value;
		}

		public string RemoveAuthentificationToken()
		{
			string tmp = m_authToken;
			m_authToken = null;
			return tmp;
		}

		public void SetAuthentificationToken(string value)
		{
			m_authToken = value;
		}
	}
}