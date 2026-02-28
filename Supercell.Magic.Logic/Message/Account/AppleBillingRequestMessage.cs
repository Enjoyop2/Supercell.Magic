using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class AppleBillingRequestMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 10150;

		private string m_tid;
		private string m_prodId;
		private string m_currencyCode;
		private string m_price;

		private byte[] m_receiptData;

		public AppleBillingRequestMessage() : this(0)
		{
			// AppleBillingRequestMessage.
		}

		public AppleBillingRequestMessage(short messageVersion) : base(messageVersion)
		{
			// AppleBillingRequestMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_tid = m_stream.ReadString(900000);
			m_prodId = m_stream.ReadString(900000);
			m_currencyCode = m_stream.ReadString(900000);
			m_price = m_stream.ReadString(900000);

			int length = m_stream.ReadBytesLength();

			if (length > 300000)
			{
				Debugger.Error("Illegal byte array length encountered.");
			}

			m_receiptData = m_stream.ReadBytes(length, 900000);
			m_stream.ReadVInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteString(m_tid);
			m_stream.WriteString(m_prodId);
			m_stream.WriteString(m_currencyCode);
			m_stream.WriteString(m_price);
			m_stream.WriteBytes(m_receiptData, m_receiptData.Length);
			m_stream.WriteVInt(0);
			m_stream.WriteInt(0);
		}

		public override short GetMessageType()
			=> AppleBillingRequestMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();

			m_tid = null;
			m_prodId = null;
			m_currencyCode = null;
			m_price = null;
			m_receiptData = null;
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

		public string GetCurrencyCode()
			=> m_currencyCode;

		public void SetCurrencyCode(string value)
		{
			m_currencyCode = value;
		}

		public byte[] GetReceiptData()
			=> m_receiptData;

		public void SetReceiptData(byte[] data, int length)
		{
			m_receiptData = null;

			if (length > -1)
			{
				m_receiptData = new byte[length];

				for (int i = 0; i < length; i++)
				{
					m_receiptData[i] = data[i];
				}
			}
		}
	}
}