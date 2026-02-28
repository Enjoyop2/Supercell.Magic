using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Google
{
	public class BindGoogleServiceAccountMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14262;

		private bool m_force;
		private string m_googleServiceId;
		private string m_accessToken;

		public BindGoogleServiceAccountMessage() : this(0)
		{
			// BindGoogleServiceAccountMessage.
		}

		public BindGoogleServiceAccountMessage(short messageVersion) : base(messageVersion)
		{
			// BindGoogleServiceAccountMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_force = m_stream.ReadBoolean();
			m_googleServiceId = m_stream.ReadString(900000);
			m_accessToken = m_stream.ReadString(900000);
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteBoolean(m_force);
			m_stream.WriteString(m_googleServiceId);
			m_stream.WriteString(m_accessToken);
		}

		public override short GetMessageType()
			=> BindGoogleServiceAccountMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();

			m_googleServiceId = null;
			m_accessToken = null;
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

		public string RemoveAccessToken()
		{
			string tmp = m_accessToken;
			m_accessToken = null;
			return tmp;
		}

		public void SetAccessToken(string value)
		{
			m_accessToken = value;
		}
	}
}