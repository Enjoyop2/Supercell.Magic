using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class UnlockAccountOkMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20132;

		private LogicLong m_accountId;
		private string m_passToken;

		public UnlockAccountOkMessage() : this(0)
		{
			// UnlockAccountOkMessage.
		}

		public UnlockAccountOkMessage(short messageVersion) : base(messageVersion)
		{
			// UnlockAccountOkMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_accountId = m_stream.ReadLong();
			m_passToken = m_stream.ReadString(900000);
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteLong(m_accountId);
			m_stream.WriteString(m_passToken);
		}

		public override short GetMessageType()
			=> UnlockAccountOkMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();

			m_accountId = null;
			m_passToken = null;
		}

		public LogicLong GetAccountId()
			=> m_accountId;

		public void SetAccountId(LogicLong id)
		{
			m_accountId = id;
		}

		public string GetPassToken()
			=> m_passToken;

		public void SetPassToken(string value)
		{
			m_passToken = value;
		}
	}
}