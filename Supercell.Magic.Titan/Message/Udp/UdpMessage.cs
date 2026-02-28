using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;

namespace Supercell.Magic.Titan.Message.Udp
{
	public class UdpMessage
	{
		private int m_messageId;
		private PiranhaMessage m_piranhaMessage;

		public UdpMessage()
		{
			// UdpMessage.
		}

		public UdpMessage(byte messageId)
		{
			m_messageId = messageId;
		}

		public int GetMessageId()
			=> m_messageId;

		public PiranhaMessage GetPiranhaMessage()
			=> m_piranhaMessage;

		public PiranhaMessage RemovePiranhaMessage()
		{
			PiranhaMessage message = m_piranhaMessage;
			m_piranhaMessage = null;
			return message;
		}

		public void SetPiranhaMessage(PiranhaMessage message)
		{
			m_piranhaMessage = message;
		}

		public void Decode(ByteStream stream, LogicMessageFactory factory)
		{
			m_messageId = stream.ReadByte();
			int messageType = stream.ReadVInt();
			m_piranhaMessage = factory.CreateMessageByType(messageType);

			if (m_piranhaMessage != null)
			{
				int encodingLength = stream.ReadVInt();
				m_piranhaMessage.GetByteStream().SetByteArray(stream.ReadBytes(encodingLength, 900000), encodingLength);
			}
			else
			{
				Debugger.Warning("UdpMessage::decode unable to read message type " + messageType);
			}
		}

		public void Encode(ByteStream stream)
		{
			int encodingLength = m_piranhaMessage.GetEncodingLength();

			stream.WriteByte((byte)m_messageId);
			stream.WriteVInt(m_piranhaMessage.GetMessageType());
			stream.WriteVInt(encodingLength);
			stream.WriteBytesWithoutLength(m_piranhaMessage.GetByteStream().GetByteArray(), encodingLength);
		}

		public bool IsMoreRecent(char messageId)
			=> m_messageId > messageId;
	}
}