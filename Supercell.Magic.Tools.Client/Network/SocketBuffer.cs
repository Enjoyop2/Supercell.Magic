using System;

namespace Supercell.Magic.Tools.Client.Network
{
	public class SocketBuffer
	{
		private byte[] m_buffer;
		private int m_size;

		public SocketBuffer(int initCapacity)
		{
			m_buffer = new byte[initCapacity];
		}

		public void Destruct()
		{
			m_buffer = null;
			m_size = 0;
		}

		public void Write(byte[] buffer, int length)
		{
			if (m_buffer.Length < m_size + length)
			{
				byte[] biggestArray = new byte[m_size + length];
				Buffer.BlockCopy(m_buffer, 0, biggestArray, 0, m_size);
				m_buffer = biggestArray;
			}

			Buffer.BlockCopy(buffer, 0, m_buffer, m_size, length);
			m_size += length;
		}

		public void Remove(int length)
		{
			Buffer.BlockCopy(m_buffer, length, m_buffer, 0, m_size -= length);
		}

		public int Size()
			=> m_size;

		public byte[] GetBuffer()
			=> m_buffer;
	}
}