using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class AccountSwitchedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 10118;

		private LogicLong m_switchedToAccountId;

		public AccountSwitchedMessage() : this(0)
		{
			// AccountSwitchedMessage.
		}

		public AccountSwitchedMessage(short messageVersion) : base(messageVersion)
		{
			// AccountSwitchedMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_switchedToAccountId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteLong(m_switchedToAccountId);
		}

		public override short GetMessageType()
			=> AccountSwitchedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
		}

		public LogicLong RemoveSwitchedToAccountId()
		{
			LogicLong tmp = m_switchedToAccountId;
			m_switchedToAccountId = null;
			return tmp;
		}

		public void SetSwitchedToAccountId(LogicLong id)
		{
			m_switchedToAccountId = id;
		}
	}
}