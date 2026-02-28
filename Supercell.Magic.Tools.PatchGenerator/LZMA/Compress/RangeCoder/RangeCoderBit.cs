namespace SevenZip.Compression.RangeCoder
{
	internal struct BitEncoder
	{
		public const int kNumBitModelTotalBits = 11;
		public const uint kBitModelTotal = 1 << BitEncoder.kNumBitModelTotalBits;
		private const int kNumMoveBits = 5;
		private const int kNumMoveReducingBits = 2;
		public const int kNumBitPriceShiftBits = 6;

		private uint Prob;

		public void Init()
		{
			Prob = BitEncoder.kBitModelTotal >> 1;
		}

		public void UpdateModel(uint symbol)
		{
			if (symbol == 0)
			{
				Prob += (BitEncoder.kBitModelTotal - Prob) >> BitEncoder.kNumMoveBits;
			}
			else
			{
				Prob -= Prob >> BitEncoder.kNumMoveBits;
			}
		}

		public void Encode(Encoder encoder, uint symbol)
		{
			// encoder.EncodeBit(Prob, kNumBitModelTotalBits, symbol);
			// UpdateModel(symbol);
			uint newBound = (encoder.Range >> BitEncoder.kNumBitModelTotalBits) * Prob;
			if (symbol == 0)
			{
				encoder.Range = newBound;
				Prob += (BitEncoder.kBitModelTotal - Prob) >> BitEncoder.kNumMoveBits;
			}
			else
			{
				encoder.Low += newBound;
				encoder.Range -= newBound;
				Prob -= Prob >> BitEncoder.kNumMoveBits;
			}

			if (encoder.Range < Encoder.kTopValue)
			{
				encoder.Range <<= 8;
				encoder.ShiftLow();
			}
		}

		private static readonly uint[] ProbPrices = new uint[BitEncoder.kBitModelTotal >> BitEncoder.kNumMoveReducingBits];

		static BitEncoder()
		{
			const int kNumBits = BitEncoder.kNumBitModelTotalBits - BitEncoder.kNumMoveReducingBits;
			for (int i = kNumBits - 1; i >= 0; i--)
			{
				uint start = (uint)1 << (kNumBits - i - 1);
				uint end = (uint)1 << (kNumBits - i);
				for (uint j = start; j < end; j++)
				{
					BitEncoder.ProbPrices[j] = ((uint)i << BitEncoder.kNumBitPriceShiftBits) +
											   (((end - j) << BitEncoder.kNumBitPriceShiftBits) >> (kNumBits - i - 1));
				}
			}
		}

		public uint GetPrice(uint symbol)
			=> BitEncoder.ProbPrices[(((Prob - symbol) ^ -(int)symbol) & (BitEncoder.kBitModelTotal - 1)) >> BitEncoder.kNumMoveReducingBits];

		public uint GetPrice0()
			=> BitEncoder.ProbPrices[Prob >> BitEncoder.kNumMoveReducingBits];

		public uint GetPrice1()
			=> BitEncoder.ProbPrices[(BitEncoder.kBitModelTotal - Prob) >> BitEncoder.kNumMoveReducingBits];
	}

	internal struct BitDecoder
	{
		public const int kNumBitModelTotalBits = 11;
		public const uint kBitModelTotal = 1 << BitDecoder.kNumBitModelTotalBits;
		private const int kNumMoveBits = 5;

		private uint Prob;

		public void UpdateModel(int numMoveBits, uint symbol)
		{
			if (symbol == 0)
			{
				Prob += (BitDecoder.kBitModelTotal - Prob) >> numMoveBits;
			}
			else
			{
				Prob -= Prob >> numMoveBits;
			}
		}

		public void Init()
		{
			Prob = BitDecoder.kBitModelTotal >> 1;
		}

		public uint Decode(Decoder rangeDecoder)
		{
			uint newBound = (rangeDecoder.Range >> BitDecoder.kNumBitModelTotalBits) * Prob;
			if (rangeDecoder.Code < newBound)
			{
				rangeDecoder.Range = newBound;
				Prob += (BitDecoder.kBitModelTotal - Prob) >> BitDecoder.kNumMoveBits;
				if (rangeDecoder.Range < Decoder.kTopValue)
				{
					rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte)rangeDecoder.Stream.ReadByte();
					rangeDecoder.Range <<= 8;
				}

				return 0;
			}

			rangeDecoder.Range -= newBound;
			rangeDecoder.Code -= newBound;
			Prob -= Prob >> BitDecoder.kNumMoveBits;
			if (rangeDecoder.Range < Decoder.kTopValue)
			{
				rangeDecoder.Code = (rangeDecoder.Code << 8) | (byte)rangeDecoder.Stream.ReadByte();
				rangeDecoder.Range <<= 8;
			}

			return 1;
		}
	}
}