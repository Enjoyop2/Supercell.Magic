// LzOutWindow.cs

using System.IO;

namespace SevenZip.Compression.LZ
{
	public class OutWindow
	{
		private byte[] m_buffer;
		private uint m_pos;
		private uint m_windowSize;
		private uint m_streamPos;
		private Stream m_stream;

		public uint TrainSize;

		public void Create(uint windowSize)
		{
			if (m_windowSize != windowSize)
			{
				// System.GC.Collect();
				m_buffer = new byte[windowSize];
			}

			m_windowSize = windowSize;
			m_pos = 0;
			m_streamPos = 0;
		}

		public void Init(Stream stream, bool solid)
		{
			ReleaseStream();
			m_stream = stream;
			if (!solid)
			{
				m_streamPos = 0;
				m_pos = 0;
				TrainSize = 0;
			}
		}

		public bool Train(Stream stream)
		{
			long len = stream.Length;
			uint size = len < m_windowSize ? (uint)len : m_windowSize;
			TrainSize = size;
			stream.Position = len - size;
			m_streamPos = m_pos = 0;
			while (size > 0)
			{
				uint curSize = m_windowSize - m_pos;
				if (size < curSize)
				{
					curSize = size;
				}

				int numReadBytes = stream.Read(m_buffer, (int)m_pos, (int)curSize);
				if (numReadBytes == 0)
				{
					return false;
				}

				size -= (uint)numReadBytes;
				m_pos += (uint)numReadBytes;
				m_streamPos += (uint)numReadBytes;
				if (m_pos == m_windowSize)
				{
					m_streamPos = m_pos = 0;
				}
			}

			return true;
		}

		public void ReleaseStream()
		{
			Flush();
			m_stream = null;
		}

		public void Flush()
		{
			uint size = m_pos - m_streamPos;
			if (size == 0)
			{
				return;
			}

			m_stream.Write(m_buffer, (int)m_streamPos, (int)size);
			if (m_pos >= m_windowSize)
			{
				m_pos = 0;
			}

			m_streamPos = m_pos;
		}

		public void CopyBlock(uint distance, uint len)
		{
			uint pos = m_pos - distance - 1;
			if (pos >= m_windowSize)
			{
				pos += m_windowSize;
			}

			for (; len > 0; len--)
			{
				if (pos >= m_windowSize)
				{
					pos = 0;
				}

				m_buffer[m_pos++] = m_buffer[pos++];
				if (m_pos >= m_windowSize)
				{
					Flush();
				}
			}
		}

		public void PutByte(byte b)
		{
			m_buffer[m_pos++] = b;
			if (m_pos >= m_windowSize)
			{
				Flush();
			}
		}

		public byte GetByte(uint distance)
		{
			uint pos = m_pos - distance - 1;
			if (pos >= m_windowSize)
			{
				pos += m_windowSize;
			}

			return m_buffer[pos];
		}
	}
}