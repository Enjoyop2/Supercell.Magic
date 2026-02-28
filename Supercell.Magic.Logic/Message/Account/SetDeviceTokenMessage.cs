using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Account
{
	public class SetDeviceTokenMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 10113;

		private byte[] m_deviceToken;
		private int m_deviceTokenLength;

		public SetDeviceTokenMessage() : this(0)
		{
			// SetDeviceTokenMessage.
		}

		public SetDeviceTokenMessage(short messageVersion) : base(messageVersion)
		{
			// SetDeviceTokenMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_deviceTokenLength = m_stream.ReadBytesLength();

			if (m_deviceTokenLength > 1000)
			{
				Debugger.Error("Illegal byte array length encountered.");
			}

			m_deviceToken = m_stream.ReadBytes(m_deviceTokenLength, 900000);
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteBytes(m_deviceToken, m_deviceTokenLength);
		}

		public override short GetMessageType()
			=> SetDeviceTokenMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
			m_deviceToken = null;
		}

		public byte[] GetDeviceToken()
			=> m_deviceToken;

		public int GetDeviceTokenLength()
			=> m_deviceTokenLength;

		public void SetDeviceToken(byte[] value, int length)
		{
			m_deviceToken = value;
			m_deviceTokenLength = length;
		}
	}
}