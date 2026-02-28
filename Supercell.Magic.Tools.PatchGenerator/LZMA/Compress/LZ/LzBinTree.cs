// LzBinTree.cs

using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
	public class BinTree : InWindow, IMatchFinder
	{
		private uint m_cyclicBufferPos;
		private uint m_cyclicBufferSize;
		private uint m_matchMaxLen;

		private uint[] m_son;
		private uint[] m_hash;

		private uint m_cutValue = 0xFF;
		private uint m_hashMask;
		private uint m_hashSizeSum;

		private bool HASH_ARRAY = true;

		private const uint kHash2Size = 1 << 10;
		private const uint kHash3Size = 1 << 16;
		private const uint kBT2HashSize = 1 << 16;
		private const uint kStartMaxLen = 1;
		private const uint kHash3Offset = BinTree.kHash2Size;
		private const uint kEmptyHashValue = 0;
		private const uint kMaxValForNormalize = ((uint)1 << 31) - 1;

		private uint kNumHashDirectBytes;
		private uint kMinMatchCheck = 4;
		private uint kFixHashSize = BinTree.kHash2Size + BinTree.kHash3Size;

		public void SetType(int numHashBytes)
		{
			HASH_ARRAY = numHashBytes > 2;
			if (HASH_ARRAY)
			{
				kNumHashDirectBytes = 0;
				kMinMatchCheck = 4;
				kFixHashSize = BinTree.kHash2Size + BinTree.kHash3Size;
			}
			else
			{
				kNumHashDirectBytes = 2;
				kMinMatchCheck = 2 + 1;
				kFixHashSize = 0;
			}
		}

		public new void SetStream(Stream stream)
		{
			base.SetStream(stream);
		}

		public new void ReleaseStream()
		{
			base.ReleaseStream();
		}

		public new void Init()
		{
			base.Init();
			for (uint i = 0; i < m_hashSizeSum; i++)
			{
				m_hash[i] = BinTree.kEmptyHashValue;
			}

			m_cyclicBufferPos = 0;
			ReduceOffsets(-1);
		}

		public new void MovePos()
		{
			if (++m_cyclicBufferPos >= m_cyclicBufferSize)
			{
				m_cyclicBufferPos = 0;
			}

			base.MovePos();
			if (m_pos == BinTree.kMaxValForNormalize)
			{
				Normalize();
			}
		}

		public new byte GetIndexByte(int index)
			=> base.GetIndexByte(index);

		public new uint GetMatchLen(int index, uint distance, uint limit)
			=> base.GetMatchLen(index, distance, limit);

		public new uint GetNumAvailableBytes()
			=> base.GetNumAvailableBytes();

		public void Create(uint historySize, uint keepAddBufferBefore,
						   uint matchMaxLen, uint keepAddBufferAfter)
		{
			if (historySize > BinTree.kMaxValForNormalize - 256)
			{
				throw new Exception();
			}

			m_cutValue = 16 + (matchMaxLen >> 1);

			uint windowReservSize = (historySize + keepAddBufferBefore +
									 matchMaxLen + keepAddBufferAfter) / 2 + 256;

			base.Create(historySize + keepAddBufferBefore, matchMaxLen + keepAddBufferAfter, windowReservSize);

			m_matchMaxLen = matchMaxLen;

			uint cyclicBufferSize = historySize + 1;
			if (m_cyclicBufferSize != cyclicBufferSize)
			{
				m_son = new uint[(m_cyclicBufferSize = cyclicBufferSize) * 2];
			}

			uint hs = BinTree.kBT2HashSize;

			if (HASH_ARRAY)
			{
				hs = historySize - 1;
				hs |= hs >> 1;
				hs |= hs >> 2;
				hs |= hs >> 4;
				hs |= hs >> 8;
				hs >>= 1;
				hs |= 0xFFFF;
				if (hs > 1 << 24)
				{
					hs >>= 1;
				}

				m_hashMask = hs;
				hs++;
				hs += kFixHashSize;
			}

			if (hs != m_hashSizeSum)
			{
				m_hash = new uint[m_hashSizeSum = hs];
			}
		}

		public uint GetMatches(uint[] distances)
		{
			uint lenLimit;
			if (m_pos + m_matchMaxLen <= m_streamPos)
			{
				lenLimit = m_matchMaxLen;
			}
			else
			{
				lenLimit = m_streamPos - m_pos;
				if (lenLimit < kMinMatchCheck)
				{
					MovePos();
					return 0;
				}
			}

			uint offset = 0;
			uint matchMinPos = m_pos > m_cyclicBufferSize ? m_pos - m_cyclicBufferSize : 0;
			uint cur = m_bufferOffset + m_pos;
			uint maxLen = BinTree.kStartMaxLen; // to avoid items for len < hashSize;
			uint hashValue, hash2Value = 0, hash3Value = 0;

			if (HASH_ARRAY)
			{
				uint temp = CRC.Table[m_bufferBase[cur]] ^ m_bufferBase[cur + 1];
				hash2Value = temp & (BinTree.kHash2Size - 1);
				temp ^= (uint)m_bufferBase[cur + 2] << 8;
				hash3Value = temp & (BinTree.kHash3Size - 1);
				hashValue = (temp ^ (CRC.Table[m_bufferBase[cur + 3]] << 5)) & m_hashMask;
			}
			else
			{
				hashValue = m_bufferBase[cur] ^ ((uint)m_bufferBase[cur + 1] << 8);
			}

			uint curMatch = m_hash[kFixHashSize + hashValue];
			if (HASH_ARRAY)
			{
				uint curMatch2 = m_hash[hash2Value];
				uint curMatch3 = m_hash[BinTree.kHash3Offset + hash3Value];
				m_hash[hash2Value] = m_pos;
				m_hash[BinTree.kHash3Offset + hash3Value] = m_pos;
				if (curMatch2 > matchMinPos)
				{
					if (m_bufferBase[m_bufferOffset + curMatch2] == m_bufferBase[cur])
					{
						distances[offset++] = maxLen = 2;
						distances[offset++] = m_pos - curMatch2 - 1;
					}
				}

				if (curMatch3 > matchMinPos)
				{
					if (m_bufferBase[m_bufferOffset + curMatch3] == m_bufferBase[cur])
					{
						if (curMatch3 == curMatch2)
						{
							offset -= 2;
						}

						distances[offset++] = maxLen = 3;
						distances[offset++] = m_pos - curMatch3 - 1;
						curMatch2 = curMatch3;
					}
				}

				if (offset != 0 && curMatch2 == curMatch)
				{
					offset -= 2;
					maxLen = BinTree.kStartMaxLen;
				}
			}

			m_hash[kFixHashSize + hashValue] = m_pos;

			uint ptr0 = (m_cyclicBufferPos << 1) + 1;
			uint ptr1 = m_cyclicBufferPos << 1;

			uint len0, len1;
			len0 = len1 = kNumHashDirectBytes;

			if (kNumHashDirectBytes != 0)
			{
				if (curMatch > matchMinPos)
				{
					if (m_bufferBase[m_bufferOffset + curMatch + kNumHashDirectBytes] != m_bufferBase[cur + kNumHashDirectBytes])
					{
						distances[offset++] = maxLen = kNumHashDirectBytes;
						distances[offset++] = m_pos - curMatch - 1;
					}
				}
			}

			uint count = m_cutValue;

			while (true)
			{
				if (curMatch <= matchMinPos || count-- == 0)
				{
					m_son[ptr0] = m_son[ptr1] = BinTree.kEmptyHashValue;
					break;
				}

				uint delta = m_pos - curMatch;
				uint cyclicPos = (delta <= m_cyclicBufferPos ? m_cyclicBufferPos - delta : m_cyclicBufferPos - delta + m_cyclicBufferSize) << 1;

				uint pby1 = m_bufferOffset + curMatch;
				uint len = Math.Min(len0, len1);
				if (m_bufferBase[pby1 + len] == m_bufferBase[cur + len])
				{
					while (++len != lenLimit)
					{
						if (m_bufferBase[pby1 + len] != m_bufferBase[cur + len])
						{
							break;
						}
					}

					if (maxLen < len)
					{
						distances[offset++] = maxLen = len;
						distances[offset++] = delta - 1;
						if (len == lenLimit)
						{
							m_son[ptr1] = m_son[cyclicPos];
							m_son[ptr0] = m_son[cyclicPos + 1];
							break;
						}
					}
				}

				if (m_bufferBase[pby1 + len] < m_bufferBase[cur + len])
				{
					m_son[ptr1] = curMatch;
					ptr1 = cyclicPos + 1;
					curMatch = m_son[ptr1];
					len1 = len;
				}
				else
				{
					m_son[ptr0] = curMatch;
					ptr0 = cyclicPos;
					curMatch = m_son[ptr0];
					len0 = len;
				}
			}

			MovePos();
			return offset;
		}

		public void Skip(uint num)
		{
			do
			{
				uint lenLimit;
				if (m_pos + m_matchMaxLen <= m_streamPos)
				{
					lenLimit = m_matchMaxLen;
				}
				else
				{
					lenLimit = m_streamPos - m_pos;
					if (lenLimit < kMinMatchCheck)
					{
						MovePos();
						continue;
					}
				}

				uint matchMinPos = m_pos > m_cyclicBufferSize ? m_pos - m_cyclicBufferSize : 0;
				uint cur = m_bufferOffset + m_pos;

				uint hashValue;

				if (HASH_ARRAY)
				{
					uint temp = CRC.Table[m_bufferBase[cur]] ^ m_bufferBase[cur + 1];
					uint hash2Value = temp & (BinTree.kHash2Size - 1);
					m_hash[hash2Value] = m_pos;
					temp ^= (uint)m_bufferBase[cur + 2] << 8;
					uint hash3Value = temp & (BinTree.kHash3Size - 1);
					m_hash[BinTree.kHash3Offset + hash3Value] = m_pos;
					hashValue = (temp ^ (CRC.Table[m_bufferBase[cur + 3]] << 5)) & m_hashMask;
				}
				else
				{
					hashValue = m_bufferBase[cur] ^ ((uint)m_bufferBase[cur + 1] << 8);
				}

				uint curMatch = m_hash[kFixHashSize + hashValue];
				m_hash[kFixHashSize + hashValue] = m_pos;

				uint ptr0 = (m_cyclicBufferPos << 1) + 1;
				uint ptr1 = m_cyclicBufferPos << 1;

				uint len0, len1;
				len0 = len1 = kNumHashDirectBytes;

				uint count = m_cutValue;
				while (true)
				{
					if (curMatch <= matchMinPos || count-- == 0)
					{
						m_son[ptr0] = m_son[ptr1] = BinTree.kEmptyHashValue;
						break;
					}

					uint delta = m_pos - curMatch;
					uint cyclicPos = (delta <= m_cyclicBufferPos ? m_cyclicBufferPos - delta : m_cyclicBufferPos - delta + m_cyclicBufferSize) << 1;

					uint pby1 = m_bufferOffset + curMatch;
					uint len = Math.Min(len0, len1);
					if (m_bufferBase[pby1 + len] == m_bufferBase[cur + len])
					{
						while (++len != lenLimit)
						{
							if (m_bufferBase[pby1 + len] != m_bufferBase[cur + len])
							{
								break;
							}
						}

						if (len == lenLimit)
						{
							m_son[ptr1] = m_son[cyclicPos];
							m_son[ptr0] = m_son[cyclicPos + 1];
							break;
						}
					}

					if (m_bufferBase[pby1 + len] < m_bufferBase[cur + len])
					{
						m_son[ptr1] = curMatch;
						ptr1 = cyclicPos + 1;
						curMatch = m_son[ptr1];
						len1 = len;
					}
					else
					{
						m_son[ptr0] = curMatch;
						ptr0 = cyclicPos;
						curMatch = m_son[ptr0];
						len0 = len;
					}
				}

				MovePos();
			} while (--num != 0);
		}

		private void NormalizeLinks(uint[] items, uint numItems, uint subValue)
		{
			for (uint i = 0; i < numItems; i++)
			{
				uint value = items[i];
				if (value <= subValue)
				{
					value = BinTree.kEmptyHashValue;
				}
				else
				{
					value -= subValue;
				}

				items[i] = value;
			}
		}

		private void Normalize()
		{
			uint subValue = m_pos - m_cyclicBufferSize;
			NormalizeLinks(m_son, m_cyclicBufferSize * 2, subValue);
			NormalizeLinks(m_hash, m_hashSizeSum, subValue);
			ReduceOffsets((int)subValue);
		}

		public void SetCutValue(uint cutValue)
		{
			m_cutValue = cutValue;
		}
	}
}