using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Titan.Message.Udp
{
	public class UdpPacket
	{
		private readonly LogicArrayList<UdpMessage> m_messages;

		private byte[] m_ackMessageIds;
		private int m_ackMessageIdCount;

		public UdpPacket()
		{
			m_messages = new LogicArrayList<UdpMessage>();
		}

		public void Decode(byte[] buffer, int length, LogicMessageFactory factory)
		{
			ByteStream stream = new ByteStream(buffer, length);

			if (!stream.IsAtEnd())
			{
				m_ackMessageIdCount = stream.ReadVInt();

				if (m_ackMessageIdCount <= 1400)
				{
					m_ackMessageIds = stream.ReadBytes(m_ackMessageIdCount, 1400);

					if (!stream.IsAtEnd())
					{
						int messageCount = stream.ReadVInt();

						if (messageCount <= 1400)
						{
							m_messages.EnsureCapacity(messageCount);

							for (int i = 0; i < messageCount; i++)
							{
								UdpMessage message = new UdpMessage();

								message.Decode(stream, factory);

								if (message.GetPiranhaMessage() == null)
								{
									break;
								}

								m_messages.Add(message);
							}
						}
					}
				}
			}

			stream.Destruct();
		}

		public void Encode(ByteStream byteStream)
		{
			if (m_ackMessageIdCount != 0 || m_messages.Size() != 0)
			{
				byteStream.WriteVInt(m_ackMessageIdCount);
				byteStream.WriteBytes(m_ackMessageIds, m_ackMessageIdCount);

				ByteStream stream = new ByteStream(1400 - byteStream.GetOffset());

				if (m_messages.Size() > 0)
				{
					int streamLength = 0;
					int encodedMessage = 0;

					for (int i = m_messages.Size() - 1; i >= 0; i--, encodedMessage++, streamLength = stream.GetLength())
					{
						m_messages[i].Encode(stream);

						if (stream.GetLength() + byteStream.GetLength() > 1410)
						{
							Debugger.Warning("UdpPacket::encode over max size");
							break;
						}
					}

					if (encodedMessage > 0)
					{
						stream.WriteVInt(encodedMessage);
						stream.WriteBytes(stream.GetByteArray(), streamLength);
					}
				}

				stream.Destruct();

				if (byteStream.GetLength() > 1400)
				{
					Debugger.Warning("UdpPacket::encode too big");
				}
			}
		}

		public void AddMessage(UdpMessage message)
		{
			m_messages.Add(message);
		}

		public LogicArrayList<UdpMessage> GetMessages()
			=> m_messages;

		public byte[] GetAckMessageIds()
			=> m_ackMessageIds;

		public int GetAckMessageIdCount()
			=> m_ackMessageIdCount;

		public void SetAckMessageIds(byte[] ackMessageIds, int count)
		{
			m_ackMessageIds = ackMessageIds;
			m_ackMessageIdCount = count;
		}
	}
}