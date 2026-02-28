using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class UnlockAccountMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 10121;

		private LogicLong m_accountId;

		private string m_passToken;
		private string m_unlockCode;

		public UnlockAccountMessage() : this(0)
		{
			// UnlockAccountMessage.
		}

		public UnlockAccountMessage(short messageVersion) : base(messageVersion)
		{
			// UnlockAccountMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_accountId = m_stream.ReadLong();
			m_passToken = m_stream.ReadString(900000);
			m_unlockCode = m_stream.ReadString(900000);
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteLong(m_accountId);
			m_stream.WriteString(m_passToken);
			m_stream.WriteString(m_unlockCode);
		}

		public override short GetMessageType()
			=> UnlockAccountMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();

			m_accountId = null;
			m_passToken = null;
			m_unlockCode = null;
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

		public string GetUnlockCode()
			=> m_unlockCode;

		public void SetUnlockCode(string value)
		{
			m_unlockCode = value;
		}
	}
}