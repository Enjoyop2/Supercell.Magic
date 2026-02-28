// LzInWindow.cs

using System.IO;

namespace SevenZip.Compression.LZ
{
	public class InWindow
	{
		public byte[] m_bufferBase; // pointer to buffer with data
		private Stream m_stream;
		private uint m_posLimit; // offset (from m_buffer) of first byte when new block reading must be done
		private bool m_streamEndWasReached; // if (true) then m_streamPos shows real end of stream

		private uint m_pointerToLastSafePosition;

		public uint m_bufferOffset;

		public uint m_blockSize; // Size of Allocated memory block
		public uint m_pos; // offset (from m_buffer) of curent byte
		private uint m_keepSizeBefore; // how many BYTEs must be kept in buffer before m_pos
		private uint m_keepSizeAfter; // how many BYTEs must be kept buffer after m_pos
		public uint m_streamPos; // offset (from m_buffer) of first not read byte from Stream

		public void MoveBlock()
		{
			uint offset = m_bufferOffset + m_pos - m_keepSizeBefore;
			// we need one additional byte, since MovePos moves on 1 byte.
			if (offset > 0)
			{
				offset--;
			}

			uint numBytes = m_bufferOffset + m_streamPos - offset;

			// check negative offset ????
			for (uint i = 0; i < numBytes; i++)
			{
				m_bufferBase[i] = m_bufferBase[offset + i];
			}

			m_bufferOffset -= offset;
		}

		public virtual void ReadBlock()
		{
			if (m_streamEndWasReached)
			{
				return;
			}

			while (true)
			{
				int size = (int)(0 - m_bufferOffset + m_blockSize - m_streamPos);
				if (size == 0)
				{
					return;
				}

				int numReadBytes = m_stream.Read(m_bufferBase, (int)(m_bufferOffset + m_streamPos), size);
				if (numReadBytes == 0)
				{
					m_posLimit = m_streamPos;
					uint pointerToPostion = m_bufferOffset + m_posLimit;
					if (pointerToPostion > m_pointerToLastSafePosition)
					{
						m_posLimit = m_pointerToLastSafePosition - m_bufferOffset;
					}

					m_streamEndWasReached = true;
					return;
				}

				m_streamPos += (uint)numReadBytes;
				if (m_streamPos >= m_pos + m_keepSizeAfter)
				{
					m_posLimit = m_streamPos - m_keepSizeAfter;
				}
			}
		}

		private void Free()
		{
			m_bufferBase = null;
		}

		public void Create(uint keepSizeBefore, uint keepSizeAfter, uint keepSizeReserv)
		{
			m_keepSizeBefore = keepSizeBefore;
			m_keepSizeAfter = keepSizeAfter;
			uint blockSize = keepSizeBefore + keepSizeAfter + keepSizeReserv;
			if (m_bufferBase == null || m_blockSize != blockSize)
			{
				Free();
				m_blockSize = blockSize;
				m_bufferBase = new byte[m_blockSize];
			}

			m_pointerToLastSafePosition = m_blockSize - keepSizeAfter;
		}

		public void SetStream(Stream stream)
		{
			m_stream = stream;
		}

		public void ReleaseStream()
		{
			m_stream = null;
		}

		public void Init()
		{
			m_bufferOffset = 0;
			m_pos = 0;
			m_streamPos = 0;
			m_streamEndWasReached = false;
			ReadBlock();
		}

		public void MovePos()
		{
			m_pos++;
			if (m_pos > m_posLimit)
			{
				uint pointerToPostion = m_bufferOffset + m_pos;
				if (pointerToPostion > m_pointerToLastSafePosition)
				{
					MoveBlock();
				}

				ReadBlock();
			}
		}

		public byte GetIndexByte(int index)
			=> m_bufferBase[m_bufferOffset + m_pos + index];

		// index + limit have not to exceed m_keepSizeAfter;
		public uint GetMatchLen(int index, uint distance, uint limit)
		{
			if (m_streamEndWasReached)
			{
				if (m_pos + index + limit > m_streamPos)
				{
					limit = m_streamPos - (uint)(m_pos + index);
				}
			}

			distance++;
			// Byte *pby = m_buffer + (size_t)_pos + index;
			uint pby = m_bufferOffset + m_pos + (uint)index;

			uint i;
			for (i = 0; i < limit && m_bufferBase[pby + i] == m_bufferBase[pby + i - distance]; i++)
			{
				;
			}

			return i;
		}

		public uint GetNumAvailableBytes()
			=> m_streamPos - m_pos;

		public void ReduceOffsets(int subValue)
		{
			m_bufferOffset += (uint)subValue;
			m_posLimit -= (uint)subValue;
			m_pos -= (uint)subValue;
			m_streamPos -= (uint)subValue;
		}
	}
}