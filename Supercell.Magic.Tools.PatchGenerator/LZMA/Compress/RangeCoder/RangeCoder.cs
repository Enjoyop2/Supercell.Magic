using System.IO;

namespace SevenZip.Compression.RangeCoder
{
	internal class Encoder
	{
		public const uint kTopValue = 1 << 24;

		private Stream Stream;

		public ulong Low;
		public uint Range;
		private uint m_cacheSize;
		private byte m_cache;

		private long StartPosition;

		public void SetStream(Stream stream)
		{
			Stream = stream;
		}

		public void ReleaseStream()
		{
			Stream = null;
		}

		public void Init()
		{
			StartPosition = Stream.Position;

			Low = 0;
			Range = 0xFFFFFFFF;
			m_cacheSize = 1;
			m_cache = 0;
		}

		public void FlushData()
		{
			for (int i = 0; i < 5; i++)
			{
				ShiftLow();
			}
		}

		public void FlushStream()
		{
			Stream.Flush();
		}

		public void CloseStream()
		{
			Stream.Close();
		}

		public void Encode(uint start, uint size, uint total)
		{
			Low += start * (Range /= total);
			Range *= size;
			while (Range < Encoder.kTopValue)
			{
				Range <<= 8;
				ShiftLow();
			}
		}

		public void ShiftLow()
		{
			if ((uint)Low < 0xFF000000 || (uint)(Low >> 32) == 1)
			{
				byte temp = m_cache;
				do
				{
					Stream.WriteByte((byte)(temp + (Low >> 32)));
					temp = 0xFF;
				} while (--m_cacheSize != 0);

				m_cache = (byte)((uint)Low >> 24);
			}

			m_cacheSize++;
			Low = (uint)Low << 8;
		}

		public void EncodeDirectBits(uint v, int numTotalBits)
		{
			for (int i = numTotalBits - 1; i >= 0; i--)
			{
				Range >>= 1;
				if (((v >> i) & 1) == 1)
				{
					Low += Range;
				}

				if (Range < Encoder.kTopValue)
				{
					Range <<= 8;
					ShiftLow();
				}
			}
		}

		public void EncodeBit(uint size0, int numTotalBits, uint symbol)
		{
			uint newBound = (Range >> numTotalBits) * size0;
			if (symbol == 0)
			{
				Range = newBound;
			}
			else
			{
				Low += newBound;
				Range -= newBound;
			}

			while (Range < Encoder.kTopValue)
			{
				Range <<= 8;
				ShiftLow();
			}
		}

		public long GetProcessedSizeAdd()
		{
			return m_cacheSize + Stream.Position - StartPosition + 4;
			// (long)Stream.GetProcessedSize();
		}
	}

	internal class Decoder
	{
		public const uint kTopValue = 1 << 24;
		public uint Range;

		public uint Code;

		// public Buffer.InBuffer Stream = new Buffer.InBuffer(1 << 16);
		public Stream Stream;

		public void Init(Stream stream)
		{
			// Stream.Init(stream);
			Stream = stream;

			Code = 0;
			Range = 0xFFFFFFFF;
			for (int i = 0; i < 5; i++)
			{
				Code = (Code << 8) | (byte)Stream.ReadByte();
			}
		}

		public void ReleaseStream()
		{
			// Stream.ReleaseStream();
			Stream = null;
		}

		public void CloseStream()
		{
			Stream.Close();
		}

		public void Normalize()
		{
			while (Range < Decoder.kTopValue)
			{
				Code = (Code << 8) | (byte)Stream.ReadByte();
				Range <<= 8;
			}
		}

		public void Normalize2()
		{
			if (Range < Decoder.kTopValue)
			{
				Code = (Code << 8) | (byte)Stream.ReadByte();
				Range <<= 8;
			}
		}

		public uint GetThreshold(uint total)
			=> Code / (Range /= total);

		public void Decode(uint start, uint size, uint total)
		{
			Code -= start * Range;
			Range *= size;
			Normalize();
		}

		public uint DecodeDirectBits(int numTotalBits)
		{
			uint range = Range;
			uint code = Code;
			uint result = 0;
			for (int i = numTotalBits; i > 0; i--)
			{
				range >>= 1;
				/*
                result <<= 1;
                if (code >= range)
                {
                    code -= range;
                    result |= 1;
                }
                */
				uint t = (code - range) >> 31;
				code -= range & (t - 1);
				result = (result << 1) | (1 - t);

				if (range < Decoder.kTopValue)
				{
					code = (code << 8) | (byte)Stream.ReadByte();
					range <<= 8;
				}
			}

			Range = range;
			Code = code;
			return result;
		}

		public uint DecodeBit(uint size0, int numTotalBits)
		{
			uint newBound = (Range >> numTotalBits) * size0;
			uint symbol;
			if (Code < newBound)
			{
				symbol = 0;
				Range = newBound;
			}
			else
			{
				symbol = 1;
				Code -= newBound;
				Range -= newBound;
			}

			Normalize();
			return symbol;
		}

		// ulong GetProcessedSize() {return Stream.GetProcessedSize(); }
	}
}