using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class AppleBillingProcessedByServerMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 20151;

		private string m_tid;
		private string m_prodId;

		public AppleBillingProcessedByServerMessage() : this(0)
		{
			// AppleBillingProcessedByServerMessage.
		}

		public AppleBillingProcessedByServerMessage(short messageVersion) : base(messageVersion)
		{
			// AppleBillingProcessedByServerMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_tid = m_stream.ReadString(900000);
			m_prodId = m_stream.ReadString(900000);
			m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteString(m_tid);
			m_stream.WriteString(m_prodId);
			m_stream.WriteInt(0);
		}

		public override short GetMessageType()
			=> AppleBillingProcessedByServerMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();

			m_tid = null;
			m_prodId = null;
		}

		public string GetTID()
			=> m_tid;

		public void SetTID(string value)
		{
			m_tid = value;
		}

		public string GetProdID()
			=> m_prodId;

		public void SetProdID(string value)
		{
			m_prodId = value;
		}
	}
}