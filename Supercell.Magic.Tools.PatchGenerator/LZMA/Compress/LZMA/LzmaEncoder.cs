// LzmaEncoder.cs

using System;
using System.IO;

using SevenZip.Compression.LZ;
using SevenZip.Compression.RangeCoder;

namespace SevenZip.Compression.LZMA
{
	public class Encoder : ICoder, ISetCoderProperties, IWriteCoderProperties
	{
		private enum EMatchFinderType
		{
			BT2,
			BT4
		}

		private const uint kIfinityPrice = 0xFFFFFFF;

		private static readonly byte[] g_FastPos = new byte[1 << 11];

		static Encoder()
		{
			const byte kFastSlots = 22;
			int c = 2;
			Encoder.g_FastPos[0] = 0;
			Encoder.g_FastPos[1] = 1;
			for (byte slotFast = 2; slotFast < kFastSlots; slotFast++)
			{
				uint k = (uint)1 << ((slotFast >> 1) - 1);
				for (uint j = 0; j < k; j++, c++)
				{
					Encoder.g_FastPos[c] = slotFast;
				}
			}
		}

		private static uint GetPosSlot(uint pos)
		{
			if (pos < 1 << 11)
			{
				return Encoder.g_FastPos[pos];
			}

			if (pos < 1 << 21)
			{
				return (uint)(Encoder.g_FastPos[pos >> 10] + 20);
			}

			return (uint)(Encoder.g_FastPos[pos >> 20] + 40);
		}

		private static uint GetPosSlot2(uint pos)
		{
			if (pos < 1 << 17)
			{
				return (uint)(Encoder.g_FastPos[pos >> 6] + 12);
			}

			if (pos < 1 << 27)
			{
				return (uint)(Encoder.g_FastPos[pos >> 16] + 32);
			}

			return (uint)(Encoder.g_FastPos[pos >> 26] + 52);
		}

		private Base.State m_state = new Base.State();
		private byte m_previousByte;
		private readonly uint[] m_repDistances = new uint[Base.kNumRepDistances];

		private void BaseInit()
		{
			m_state.Init();
			m_previousByte = 0;
			for (uint i = 0; i < Base.kNumRepDistances; i++)
			{
				m_repDistances[i] = 0;
			}
		}

		private const int kDefaultDictionaryLogSize = 22;
		private const uint kNumFastBytesDefault = 0x20;

		private class LiteralEncoder
		{
			public struct Encoder2
			{
				private BitEncoder[] m_Encoders;

				public void Create()
				{
					m_Encoders = new BitEncoder[0x300];
				}

				public void Init()
				{
					for (int i = 0; i < 0x300; i++)
					{
						m_Encoders[i].Init();
					}
				}

				public void Encode(RangeCoder.Encoder rangeEncoder, byte symbol)
				{
					uint context = 1;
					for (int i = 7; i >= 0; i--)
					{
						uint bit = (uint)((symbol >> i) & 1);
						m_Encoders[context].Encode(rangeEncoder, bit);
						context = (context << 1) | bit;
					}
				}

				public void EncodeMatched(RangeCoder.Encoder rangeEncoder, byte matchByte, byte symbol)
				{
					uint context = 1;
					bool same = true;
					for (int i = 7; i >= 0; i--)
					{
						uint bit = (uint)((symbol >> i) & 1);
						uint state = context;
						if (same)
						{
							uint matchBit = (uint)((matchByte >> i) & 1);
							state += (1 + matchBit) << 8;
							same = matchBit == bit;
						}

						m_Encoders[state].Encode(rangeEncoder, bit);
						context = (context << 1) | bit;
					}
				}

				public uint GetPrice(bool matchMode, byte matchByte, byte symbol)
				{
					uint price = 0;
					uint context = 1;
					int i = 7;
					if (matchMode)
					{
						for (; i >= 0; i--)
						{
							uint matchBit = (uint)(matchByte >> i) & 1;
							uint bit = (uint)(symbol >> i) & 1;
							price += m_Encoders[((1 + matchBit) << 8) + context].GetPrice(bit);
							context = (context << 1) | bit;
							if (matchBit != bit)
							{
								i--;
								break;
							}
						}
					}

					for (; i >= 0; i--)
					{
						uint bit = (uint)(symbol >> i) & 1;
						price += m_Encoders[context].GetPrice(bit);
						context = (context << 1) | bit;
					}

					return price;
				}
			}

			private Encoder2[] m_Coders;
			private int m_NumPrevBits;
			private int m_NumPosBits;
			private uint m_PosMask;

			public void Create(int numPosBits, int numPrevBits)
			{
				if (m_Coders != null && m_NumPrevBits == numPrevBits && m_NumPosBits == numPosBits)
				{
					return;
				}

				m_NumPosBits = numPosBits;
				m_PosMask = ((uint)1 << numPosBits) - 1;
				m_NumPrevBits = numPrevBits;
				uint numStates = (uint)1 << (m_NumPrevBits + m_NumPosBits);
				m_Coders = new Encoder2[numStates];
				for (uint i = 0; i < numStates; i++)
				{
					m_Coders[i].Create();
				}
			}

			public void Init()
			{
				uint numStates = (uint)1 << (m_NumPrevBits + m_NumPosBits);
				for (uint i = 0; i < numStates; i++)
				{
					m_Coders[i].Init();
				}
			}

			public Encoder2 GetSubCoder(uint pos, byte prevByte)
			{
				return m_Coders[((pos & m_PosMask) << m_NumPrevBits) + (uint)(prevByte >> (8 - m_NumPrevBits))];
			}
		}

		private class LenEncoder
		{
			private BitEncoder m_choice = new BitEncoder();
			private BitEncoder m_choice2 = new BitEncoder();
			private readonly BitTreeEncoder[] m_lowCoder = new BitTreeEncoder[Base.kNumPosStatesEncodingMax];
			private readonly BitTreeEncoder[] m_midCoder = new BitTreeEncoder[Base.kNumPosStatesEncodingMax];
			private BitTreeEncoder m_highCoder = new BitTreeEncoder(Base.kNumHighLenBits);

			public LenEncoder()
			{
				for (uint posState = 0; posState < Base.kNumPosStatesEncodingMax; posState++)
				{
					m_lowCoder[posState] = new BitTreeEncoder(Base.kNumLowLenBits);
					m_midCoder[posState] = new BitTreeEncoder(Base.kNumMidLenBits);
				}
			}

			public void Init(uint numPosStates)
			{
				m_choice.Init();
				m_choice2.Init();
				for (uint posState = 0; posState < numPosStates; posState++)
				{
					m_lowCoder[posState].Init();
					m_midCoder[posState].Init();
				}

				m_highCoder.Init();
			}

			public void Encode(RangeCoder.Encoder rangeEncoder, uint symbol, uint posState)
			{
				if (symbol < Base.kNumLowLenSymbols)
				{
					m_choice.Encode(rangeEncoder, 0);
					m_lowCoder[posState].Encode(rangeEncoder, symbol);
				}
				else
				{
					symbol -= Base.kNumLowLenSymbols;
					m_choice.Encode(rangeEncoder, 1);
					if (symbol < Base.kNumMidLenSymbols)
					{
						m_choice2.Encode(rangeEncoder, 0);
						m_midCoder[posState].Encode(rangeEncoder, symbol);
					}
					else
					{
						m_choice2.Encode(rangeEncoder, 1);
						m_highCoder.Encode(rangeEncoder, symbol - Base.kNumMidLenSymbols);
					}
				}
			}

			public void SetPrices(uint posState, uint numSymbols, uint[] prices, uint st)
			{
				uint a0 = m_choice.GetPrice0();
				uint a1 = m_choice.GetPrice1();
				uint b0 = a1 + m_choice2.GetPrice0();
				uint b1 = a1 + m_choice2.GetPrice1();
				uint i = 0;
				for (i = 0; i < Base.kNumLowLenSymbols; i++)
				{
					if (i >= numSymbols)
					{
						return;
					}

					prices[st + i] = a0 + m_lowCoder[posState].GetPrice(i);
				}

				for (; i < Base.kNumLowLenSymbols + Base.kNumMidLenSymbols; i++)
				{
					if (i >= numSymbols)
					{
						return;
					}

					prices[st + i] = b0 + m_midCoder[posState].GetPrice(i - Base.kNumLowLenSymbols);
				}

				for (; i < numSymbols; i++)
				{
					prices[st + i] = b1 + m_highCoder.GetPrice(i - Base.kNumLowLenSymbols - Base.kNumMidLenSymbols);
				}
			}
		}

		private const uint kNumLenSpecSymbols = Base.kNumLowLenSymbols + Base.kNumMidLenSymbols;

		private class LenPriceTableEncoder : LenEncoder
		{
			private readonly uint[] m_prices = new uint[Base.kNumLenSymbols << Base.kNumPosStatesBitsEncodingMax];
			private uint m_tableSize;
			private readonly uint[] m_counters = new uint[Base.kNumPosStatesEncodingMax];

			public void SetTableSize(uint tableSize)
			{
				m_tableSize = tableSize;
			}

			public uint GetPrice(uint symbol, uint posState)
			{
				return m_prices[posState * Base.kNumLenSymbols + symbol];
			}

			private void UpdateTable(uint posState)
			{
				SetPrices(posState, m_tableSize, m_prices, posState * Base.kNumLenSymbols);
				m_counters[posState] = m_tableSize;
			}

			public void UpdateTables(uint numPosStates)
			{
				for (uint posState = 0; posState < numPosStates; posState++)
				{
					UpdateTable(posState);
				}
			}

			public new void Encode(RangeCoder.Encoder rangeEncoder, uint symbol, uint posState)
			{
				base.Encode(rangeEncoder, symbol, posState);
				if (--m_counters[posState] == 0)
				{
					UpdateTable(posState);
				}
			}
		}

		private const uint kNumOpts = 1 << 12;

		private class Optimal
		{
			public Base.State State;

			public bool Prev1IsChar;
			public bool Prev2;

			public uint PosPrev2;
			public uint BackPrev2;

			public uint Price;
			public uint PosPrev;
			public uint BackPrev;

			public uint Backs0;
			public uint Backs1;
			public uint Backs2;
			public uint Backs3;

			public void MakeAsChar()
			{
				BackPrev = 0xFFFFFFFF;
				Prev1IsChar = false;
			}

			public void MakeAsShortRep()
			{
				BackPrev = 0;
				;
				Prev1IsChar = false;
			}

			public bool IsShortRep()
			{
				return BackPrev == 0;
			}
		}

		private readonly Optimal[] m_optimum = new Optimal[Encoder.kNumOpts];
		private IMatchFinder m_matchFinder;
		private readonly RangeCoder.Encoder m_rangeEncoder = new RangeCoder.Encoder();

		private readonly BitEncoder[] m_isMatch = new BitEncoder[Base.kNumStates << Base.kNumPosStatesBitsMax];
		private readonly BitEncoder[] m_isRep = new BitEncoder[Base.kNumStates];
		private readonly BitEncoder[] m_isRepG0 = new BitEncoder[Base.kNumStates];
		private readonly BitEncoder[] m_isRepG1 = new BitEncoder[Base.kNumStates];
		private readonly BitEncoder[] m_isRepG2 = new BitEncoder[Base.kNumStates];
		private readonly BitEncoder[] m_isRep0Long = new BitEncoder[Base.kNumStates << Base.kNumPosStatesBitsMax];

		private readonly BitTreeEncoder[] m_posSlotEncoder = new BitTreeEncoder[Base.kNumLenToPosStates];

		private readonly BitEncoder[] m_posEncoders = new BitEncoder[Base.kNumFullDistances - Base.kEndPosModelIndex];
		private BitTreeEncoder m_posAlignEncoder = new BitTreeEncoder(Base.kNumAlignBits);

		private readonly LenPriceTableEncoder m_lenEncoder = new LenPriceTableEncoder();
		private readonly LenPriceTableEncoder m_repMatchLenEncoder = new LenPriceTableEncoder();

		private readonly LiteralEncoder m_literalEncoder = new LiteralEncoder();

		private readonly uint[] m_matchDistances = new uint[Base.kMatchMaxLen * 2 + 2];

		private uint m_numFastBytes = Encoder.kNumFastBytesDefault;
		private uint m_longestMatchLength;
		private uint m_numDistancePairs;

		private uint m_additionalOffset;

		private uint m_optimumEndIndex;
		private uint m_optimumCurrentIndex;

		private bool m_longestMatchWasFound;

		private readonly uint[] m_posSlotPrices = new uint[1 << (Base.kNumPosSlotBits + Base.kNumLenToPosStatesBits)];
		private readonly uint[] m_distancesPrices = new uint[Base.kNumFullDistances << Base.kNumLenToPosStatesBits];
		private readonly uint[] m_alignPrices = new uint[Base.kAlignTableSize];
		private uint m_alignPriceCount;

		private uint m_distTableSize = Encoder.kDefaultDictionaryLogSize * 2;

		private int m_posStateBits = 2;
		private uint m_posStateMask = 4 - 1;
		private int m_numLiteralPosStateBits;
		private int m_numLiteralContextBits = 3;

		private uint m_dictionarySize = 1 << Encoder.kDefaultDictionaryLogSize;
		private uint m_dictionarySizePrev = 0xFFFFFFFF;
		private uint m_numFastBytesPrev = 0xFFFFFFFF;

		private long nowPos64;
		private bool m_finished;
		private Stream m_inStream;

		private EMatchFinderType m_matchFinderType = EMatchFinderType.BT4;
		private bool m_writeEndMark;

		private bool m_needReleaseMFStream;

		private void Create()
		{
			if (m_matchFinder == null)
			{
				BinTree bt = new BinTree();
				int numHashBytes = 4;
				if (m_matchFinderType == EMatchFinderType.BT2)
				{
					numHashBytes = 2;
				}

				bt.SetType(numHashBytes);
				m_matchFinder = bt;
			}

			m_literalEncoder.Create(m_numLiteralPosStateBits, m_numLiteralContextBits);

			if (m_dictionarySize == m_dictionarySizePrev && m_numFastBytesPrev == m_numFastBytes)
			{
				return;
			}

			m_matchFinder.Create(m_dictionarySize, Encoder.kNumOpts, m_numFastBytes, Base.kMatchMaxLen + 1);
			m_dictionarySizePrev = m_dictionarySize;
			m_numFastBytesPrev = m_numFastBytes;
		}

		public Encoder()
		{
			for (int i = 0; i < Encoder.kNumOpts; i++)
			{
				m_optimum[i] = new Optimal();
			}

			for (int i = 0; i < Base.kNumLenToPosStates; i++)
			{
				m_posSlotEncoder[i] = new BitTreeEncoder(Base.kNumPosSlotBits);
			}
		}

		private void SetWriteEndMarkerMode(bool writeEndMarker)
		{
			m_writeEndMark = writeEndMarker;
		}

		private void Init()
		{
			BaseInit();
			m_rangeEncoder.Init();

			uint i;
			for (i = 0; i < Base.kNumStates; i++)
			{
				for (uint j = 0; j <= m_posStateMask; j++)
				{
					uint complexState = (i << Base.kNumPosStatesBitsMax) + j;
					m_isMatch[complexState].Init();
					m_isRep0Long[complexState].Init();
				}

				m_isRep[i].Init();
				m_isRepG0[i].Init();
				m_isRepG1[i].Init();
				m_isRepG2[i].Init();
			}

			m_literalEncoder.Init();
			for (i = 0; i < Base.kNumLenToPosStates; i++)
			{
				m_posSlotEncoder[i].Init();
			}

			for (i = 0; i < Base.kNumFullDistances - Base.kEndPosModelIndex; i++)
			{
				m_posEncoders[i].Init();
			}

			m_lenEncoder.Init((uint)1 << m_posStateBits);
			m_repMatchLenEncoder.Init((uint)1 << m_posStateBits);

			m_posAlignEncoder.Init();

			m_longestMatchWasFound = false;
			m_optimumEndIndex = 0;
			m_optimumCurrentIndex = 0;
			m_additionalOffset = 0;
		}

		private void ReadMatchDistances(out uint lenRes, out uint numDistancePairs)
		{
			lenRes = 0;
			numDistancePairs = m_matchFinder.GetMatches(m_matchDistances);
			if (numDistancePairs > 0)
			{
				lenRes = m_matchDistances[numDistancePairs - 2];
				if (lenRes == m_numFastBytes)
				{
					lenRes += m_matchFinder.GetMatchLen((int)lenRes - 1, m_matchDistances[numDistancePairs - 1],
															Base.kMatchMaxLen - lenRes);
				}
			}

			m_additionalOffset++;
		}


		private void MovePos(uint num)
		{
			if (num > 0)
			{
				m_matchFinder.Skip(num);
				m_additionalOffset += num;
			}
		}

		private uint GetRepLen1Price(Base.State state, uint posState)
			=> m_isRepG0[state.Index].GetPrice0() + m_isRep0Long[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0();

		private uint GetPureRepPrice(uint repIndex, Base.State state, uint posState)
		{
			uint price;
			if (repIndex == 0)
			{
				price = m_isRepG0[state.Index].GetPrice0();
				price += m_isRep0Long[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
			}
			else
			{
				price = m_isRepG0[state.Index].GetPrice1();
				if (repIndex == 1)
				{
					price += m_isRepG1[state.Index].GetPrice0();
				}
				else
				{
					price += m_isRepG1[state.Index].GetPrice1();
					price += m_isRepG2[state.Index].GetPrice(repIndex - 2);
				}
			}

			return price;
		}

		private uint GetRepPrice(uint repIndex, uint len, Base.State state, uint posState)
		{
			uint price = m_repMatchLenEncoder.GetPrice(len - Base.kMatchMinLen, posState);
			return price + GetPureRepPrice(repIndex, state, posState);
		}

		private uint GetPosLenPrice(uint pos, uint len, uint posState)
		{
			uint price;
			uint lenToPosState = Base.GetLenToPosState(len);
			if (pos < Base.kNumFullDistances)
			{
				price = m_distancesPrices[lenToPosState * Base.kNumFullDistances + pos];
			}
			else
			{
				price = m_posSlotPrices[(lenToPosState << Base.kNumPosSlotBits) + Encoder.GetPosSlot2(pos)] + m_alignPrices[pos & Base.kAlignMask];
			}

			return price + m_lenEncoder.GetPrice(len - Base.kMatchMinLen, posState);
		}

		private uint Backward(out uint backRes, uint cur)
		{
			m_optimumEndIndex = cur;
			uint posMem = m_optimum[cur].PosPrev;
			uint backMem = m_optimum[cur].BackPrev;
			do
			{
				if (m_optimum[cur].Prev1IsChar)
				{
					m_optimum[posMem].MakeAsChar();
					m_optimum[posMem].PosPrev = posMem - 1;
					if (m_optimum[cur].Prev2)
					{
						m_optimum[posMem - 1].Prev1IsChar = false;
						m_optimum[posMem - 1].PosPrev = m_optimum[cur].PosPrev2;
						m_optimum[posMem - 1].BackPrev = m_optimum[cur].BackPrev2;
					}
				}

				uint posPrev = posMem;
				uint backCur = backMem;

				backMem = m_optimum[posPrev].BackPrev;
				posMem = m_optimum[posPrev].PosPrev;

				m_optimum[posPrev].BackPrev = backCur;
				m_optimum[posPrev].PosPrev = cur;
				cur = posPrev;
			} while (cur > 0);

			backRes = m_optimum[0].BackPrev;
			m_optimumCurrentIndex = m_optimum[0].PosPrev;
			return m_optimumCurrentIndex;
		}

		private readonly uint[] reps = new uint[Base.kNumRepDistances];
		private readonly uint[] repLens = new uint[Base.kNumRepDistances];


		private uint GetOptimum(uint position, out uint backRes)
		{
			if (m_optimumEndIndex != m_optimumCurrentIndex)
			{
				uint lenRes = m_optimum[m_optimumCurrentIndex].PosPrev - m_optimumCurrentIndex;
				backRes = m_optimum[m_optimumCurrentIndex].BackPrev;
				m_optimumCurrentIndex = m_optimum[m_optimumCurrentIndex].PosPrev;
				return lenRes;
			}

			m_optimumCurrentIndex = m_optimumEndIndex = 0;

			uint lenMain, numDistancePairs;
			if (!m_longestMatchWasFound)
			{
				ReadMatchDistances(out lenMain, out numDistancePairs);
			}
			else
			{
				lenMain = m_longestMatchLength;
				numDistancePairs = m_numDistancePairs;
				m_longestMatchWasFound = false;
			}

			uint numAvailableBytes = m_matchFinder.GetNumAvailableBytes() + 1;
			if (numAvailableBytes < 2)
			{
				backRes = 0xFFFFFFFF;
				return 1;
			}

			if (numAvailableBytes > Base.kMatchMaxLen)
			{
				numAvailableBytes = Base.kMatchMaxLen;
			}

			uint repMaxIndex = 0;
			uint i;
			for (i = 0; i < Base.kNumRepDistances; i++)
			{
				reps[i] = m_repDistances[i];
				repLens[i] = m_matchFinder.GetMatchLen(0 - 1, reps[i], Base.kMatchMaxLen);
				if (repLens[i] > repLens[repMaxIndex])
				{
					repMaxIndex = i;
				}
			}

			if (repLens[repMaxIndex] >= m_numFastBytes)
			{
				backRes = repMaxIndex;
				uint lenRes = repLens[repMaxIndex];
				MovePos(lenRes - 1);
				return lenRes;
			}

			if (lenMain >= m_numFastBytes)
			{
				backRes = m_matchDistances[numDistancePairs - 1] + Base.kNumRepDistances;
				MovePos(lenMain - 1);
				return lenMain;
			}

			byte currentByte = m_matchFinder.GetIndexByte(0 - 1);
			byte matchByte = m_matchFinder.GetIndexByte((int)(0 - m_repDistances[0] - 1 - 1));

			if (lenMain < 2 && currentByte != matchByte && repLens[repMaxIndex] < 2)
			{
				backRes = 0xFFFFFFFF;
				return 1;
			}

			m_optimum[0].State = m_state;

			uint posState = position & m_posStateMask;

			m_optimum[1].Price = m_isMatch[(m_state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0() +
									 m_literalEncoder.GetSubCoder(position, m_previousByte).GetPrice(!m_state.IsCharState(), matchByte, currentByte);
			m_optimum[1].MakeAsChar();

			uint matchPrice = m_isMatch[(m_state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
			uint repMatchPrice = matchPrice + m_isRep[m_state.Index].GetPrice1();

			if (matchByte == currentByte)
			{
				uint shortRepPrice = repMatchPrice + GetRepLen1Price(m_state, posState);
				if (shortRepPrice < m_optimum[1].Price)
				{
					m_optimum[1].Price = shortRepPrice;
					m_optimum[1].MakeAsShortRep();
				}
			}

			uint lenEnd = lenMain >= repLens[repMaxIndex] ? lenMain : repLens[repMaxIndex];

			if (lenEnd < 2)
			{
				backRes = m_optimum[1].BackPrev;
				return 1;
			}

			m_optimum[1].PosPrev = 0;

			m_optimum[0].Backs0 = reps[0];
			m_optimum[0].Backs1 = reps[1];
			m_optimum[0].Backs2 = reps[2];
			m_optimum[0].Backs3 = reps[3];

			uint len = lenEnd;
			do
			{
				m_optimum[len--].Price = Encoder.kIfinityPrice;
			} while (len >= 2);

			for (i = 0; i < Base.kNumRepDistances; i++)
			{
				uint repLen = repLens[i];
				if (repLen < 2)
				{
					continue;
				}

				uint price = repMatchPrice + GetPureRepPrice(i, m_state, posState);
				do
				{
					uint curAndLenPrice = price + m_repMatchLenEncoder.GetPrice(repLen - 2, posState);
					Optimal optimum = m_optimum[repLen];
					if (curAndLenPrice < optimum.Price)
					{
						optimum.Price = curAndLenPrice;
						optimum.PosPrev = 0;
						optimum.BackPrev = i;
						optimum.Prev1IsChar = false;
					}
				} while (--repLen >= 2);
			}

			uint normalMatchPrice = matchPrice + m_isRep[m_state.Index].GetPrice0();

			len = repLens[0] >= 2 ? repLens[0] + 1 : 2;
			if (len <= lenMain)
			{
				uint offs = 0;
				while (len > m_matchDistances[offs])
				{
					offs += 2;
				}

				for (; ; len++)
				{
					uint distance = m_matchDistances[offs + 1];
					uint curAndLenPrice = normalMatchPrice + GetPosLenPrice(distance, len, posState);
					Optimal optimum = m_optimum[len];
					if (curAndLenPrice < optimum.Price)
					{
						optimum.Price = curAndLenPrice;
						optimum.PosPrev = 0;
						optimum.BackPrev = distance + Base.kNumRepDistances;
						optimum.Prev1IsChar = false;
					}

					if (len == m_matchDistances[offs])
					{
						offs += 2;
						if (offs == numDistancePairs)
						{
							break;
						}
					}
				}
			}

			uint cur = 0;

			while (true)
			{
				cur++;
				if (cur == lenEnd)
				{
					return Backward(out backRes, cur);
				}

				uint newLen;
				ReadMatchDistances(out newLen, out numDistancePairs);
				if (newLen >= m_numFastBytes)
				{
					m_numDistancePairs = numDistancePairs;
					m_longestMatchLength = newLen;
					m_longestMatchWasFound = true;
					return Backward(out backRes, cur);
				}

				position++;
				uint posPrev = m_optimum[cur].PosPrev;
				Base.State state;
				if (m_optimum[cur].Prev1IsChar)
				{
					posPrev--;
					if (m_optimum[cur].Prev2)
					{
						state = m_optimum[m_optimum[cur].PosPrev2].State;
						if (m_optimum[cur].BackPrev2 < Base.kNumRepDistances)
						{
							state.UpdateRep();
						}
						else
						{
							state.UpdateMatch();
						}
					}
					else
					{
						state = m_optimum[posPrev].State;
					}

					state.UpdateChar();
				}
				else
				{
					state = m_optimum[posPrev].State;
				}

				if (posPrev == cur - 1)
				{
					if (m_optimum[cur].IsShortRep())
					{
						state.UpdateShortRep();
					}
					else
					{
						state.UpdateChar();
					}
				}
				else
				{
					uint pos;
					if (m_optimum[cur].Prev1IsChar && m_optimum[cur].Prev2)
					{
						posPrev = m_optimum[cur].PosPrev2;
						pos = m_optimum[cur].BackPrev2;
						state.UpdateRep();
					}
					else
					{
						pos = m_optimum[cur].BackPrev;
						if (pos < Base.kNumRepDistances)
						{
							state.UpdateRep();
						}
						else
						{
							state.UpdateMatch();
						}
					}

					Optimal opt = m_optimum[posPrev];
					if (pos < Base.kNumRepDistances)
					{
						if (pos == 0)
						{
							reps[0] = opt.Backs0;
							reps[1] = opt.Backs1;
							reps[2] = opt.Backs2;
							reps[3] = opt.Backs3;
						}
						else if (pos == 1)
						{
							reps[0] = opt.Backs1;
							reps[1] = opt.Backs0;
							reps[2] = opt.Backs2;
							reps[3] = opt.Backs3;
						}
						else if (pos == 2)
						{
							reps[0] = opt.Backs2;
							reps[1] = opt.Backs0;
							reps[2] = opt.Backs1;
							reps[3] = opt.Backs3;
						}
						else
						{
							reps[0] = opt.Backs3;
							reps[1] = opt.Backs0;
							reps[2] = opt.Backs1;
							reps[3] = opt.Backs2;
						}
					}
					else
					{
						reps[0] = pos - Base.kNumRepDistances;
						reps[1] = opt.Backs0;
						reps[2] = opt.Backs1;
						reps[3] = opt.Backs2;
					}
				}

				m_optimum[cur].State = state;
				m_optimum[cur].Backs0 = reps[0];
				m_optimum[cur].Backs1 = reps[1];
				m_optimum[cur].Backs2 = reps[2];
				m_optimum[cur].Backs3 = reps[3];
				uint curPrice = m_optimum[cur].Price;

				currentByte = m_matchFinder.GetIndexByte(0 - 1);
				matchByte = m_matchFinder.GetIndexByte((int)(0 - reps[0] - 1 - 1));

				posState = position & m_posStateMask;

				uint curAnd1Price = curPrice + m_isMatch[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice0() +
									m_literalEncoder.GetSubCoder(position, m_matchFinder.GetIndexByte(0 - 2)).GetPrice(!state.IsCharState(), matchByte, currentByte);

				Optimal nextOptimum = m_optimum[cur + 1];

				bool nextIsChar = false;
				if (curAnd1Price < nextOptimum.Price)
				{
					nextOptimum.Price = curAnd1Price;
					nextOptimum.PosPrev = cur;
					nextOptimum.MakeAsChar();
					nextIsChar = true;
				}

				matchPrice = curPrice + m_isMatch[(state.Index << Base.kNumPosStatesBitsMax) + posState].GetPrice1();
				repMatchPrice = matchPrice + m_isRep[state.Index].GetPrice1();

				if (matchByte == currentByte &&
					!(nextOptimum.PosPrev < cur && nextOptimum.BackPrev == 0))
				{
					uint shortRepPrice = repMatchPrice + GetRepLen1Price(state, posState);
					if (shortRepPrice <= nextOptimum.Price)
					{
						nextOptimum.Price = shortRepPrice;
						nextOptimum.PosPrev = cur;
						nextOptimum.MakeAsShortRep();
						nextIsChar = true;
					}
				}

				uint numAvailableBytesFull = m_matchFinder.GetNumAvailableBytes() + 1;
				numAvailableBytesFull = Math.Min(Encoder.kNumOpts - 1 - cur, numAvailableBytesFull);
				numAvailableBytes = numAvailableBytesFull;

				if (numAvailableBytes < 2)
				{
					continue;
				}

				if (numAvailableBytes > m_numFastBytes)
				{
					numAvailableBytes = m_numFastBytes;
				}

				if (!nextIsChar && matchByte != currentByte)
				{
					// try Literal + rep0
					uint t = Math.Min(numAvailableBytesFull - 1, m_numFastBytes);
					uint lenTest2 = m_matchFinder.GetMatchLen(0, reps[0], t);
					if (lenTest2 >= 2)
					{
						Base.State state2 = state;
						state2.UpdateChar();
						uint posStateNext = (position + 1) & m_posStateMask;
						uint nextRepMatchPrice = curAnd1Price + m_isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice1() +
												 m_isRep[state2.Index].GetPrice1();
						{
							uint offset = cur + 1 + lenTest2;
							while (lenEnd < offset)
							{
								m_optimum[++lenEnd].Price = Encoder.kIfinityPrice;
							}

							uint curAndLenPrice = nextRepMatchPrice + GetRepPrice(
													  0, lenTest2, state2, posStateNext);
							Optimal optimum = m_optimum[offset];
							if (curAndLenPrice < optimum.Price)
							{
								optimum.Price = curAndLenPrice;
								optimum.PosPrev = cur + 1;
								optimum.BackPrev = 0;
								optimum.Prev1IsChar = true;
								optimum.Prev2 = false;
							}
						}
					}
				}

				uint startLen = 2; // speed optimization 

				for (uint repIndex = 0; repIndex < Base.kNumRepDistances; repIndex++)
				{
					uint lenTest = m_matchFinder.GetMatchLen(0 - 1, reps[repIndex], numAvailableBytes);
					if (lenTest < 2)
					{
						continue;
					}

					uint lenTestTemp = lenTest;
					do
					{
						while (lenEnd < cur + lenTest)
						{
							m_optimum[++lenEnd].Price = Encoder.kIfinityPrice;
						}

						uint curAndLenPrice = repMatchPrice + GetRepPrice(repIndex, lenTest, state, posState);
						Optimal optimum = m_optimum[cur + lenTest];
						if (curAndLenPrice < optimum.Price)
						{
							optimum.Price = curAndLenPrice;
							optimum.PosPrev = cur;
							optimum.BackPrev = repIndex;
							optimum.Prev1IsChar = false;
						}
					} while (--lenTest >= 2);

					lenTest = lenTestTemp;

					if (repIndex == 0)
					{
						startLen = lenTest + 1;
					}

					// if (_maxMode)
					if (lenTest < numAvailableBytesFull)
					{
						uint t = Math.Min(numAvailableBytesFull - 1 - lenTest, m_numFastBytes);
						uint lenTest2 = m_matchFinder.GetMatchLen((int)lenTest, reps[repIndex], t);
						if (lenTest2 >= 2)
						{
							Base.State state2 = state;
							state2.UpdateRep();
							uint posStateNext = (position + lenTest) & m_posStateMask;
							uint curAndLenCharPrice =
								repMatchPrice + GetRepPrice(repIndex, lenTest, state, posState) +
								m_isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice0() +
																														m_literalEncoder
																														.GetSubCoder(position + lenTest,
																																	 m_matchFinder.GetIndexByte(
																																		 (int)lenTest - 1 - 1))
																														.GetPrice(true,
																																  m_matchFinder.GetIndexByte(
																																	  (int)lenTest - 1 -
																																	  (int)(reps[repIndex] + 1)),
																																  m_matchFinder.GetIndexByte(
																																	  (int)lenTest - 1));
							state2.UpdateChar();
							posStateNext = (position + lenTest + 1) & m_posStateMask;
							uint nextMatchPrice = curAndLenCharPrice + m_isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice1();
							uint nextRepMatchPrice = nextMatchPrice + m_isRep[state2.Index].GetPrice1();

							// for(; lenTest2 >= 2; lenTest2--)
							{
								uint offset = lenTest + 1 + lenTest2;
								while (lenEnd < cur + offset)
								{
									m_optimum[++lenEnd].Price = Encoder.kIfinityPrice;
								}

								uint curAndLenPrice = nextRepMatchPrice + GetRepPrice(0, lenTest2, state2, posStateNext);
								Optimal optimum = m_optimum[cur + offset];
								if (curAndLenPrice < optimum.Price)
								{
									optimum.Price = curAndLenPrice;
									optimum.PosPrev = cur + lenTest + 1;
									optimum.BackPrev = 0;
									optimum.Prev1IsChar = true;
									optimum.Prev2 = true;
									optimum.PosPrev2 = cur;
									optimum.BackPrev2 = repIndex;
								}
							}
						}
					}
				}

				if (newLen > numAvailableBytes)
				{
					newLen = numAvailableBytes;
					for (numDistancePairs = 0; newLen > m_matchDistances[numDistancePairs]; numDistancePairs += 2)
					{
						;
					}

					m_matchDistances[numDistancePairs] = newLen;
					numDistancePairs += 2;
				}

				if (newLen >= startLen)
				{
					normalMatchPrice = matchPrice + m_isRep[state.Index].GetPrice0();
					while (lenEnd < cur + newLen)
					{
						m_optimum[++lenEnd].Price = Encoder.kIfinityPrice;
					}

					uint offs = 0;
					while (startLen > m_matchDistances[offs])
					{
						offs += 2;
					}

					for (uint lenTest = startLen; ; lenTest++)
					{
						uint curBack = m_matchDistances[offs + 1];
						uint curAndLenPrice = normalMatchPrice + GetPosLenPrice(curBack, lenTest, posState);
						Optimal optimum = m_optimum[cur + lenTest];
						if (curAndLenPrice < optimum.Price)
						{
							optimum.Price = curAndLenPrice;
							optimum.PosPrev = cur;
							optimum.BackPrev = curBack + Base.kNumRepDistances;
							optimum.Prev1IsChar = false;
						}

						if (lenTest == m_matchDistances[offs])
						{
							if (lenTest < numAvailableBytesFull)
							{
								uint t = Math.Min(numAvailableBytesFull - 1 - lenTest, m_numFastBytes);
								uint lenTest2 = m_matchFinder.GetMatchLen((int)lenTest, curBack, t);
								if (lenTest2 >= 2)
								{
									Base.State state2 = state;
									state2.UpdateMatch();
									uint posStateNext = (position + lenTest) & m_posStateMask;
									uint curAndLenCharPrice = curAndLenPrice + m_isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice0() +
																																									   m_literalEncoder
																																									   .GetSubCoder(
																																										   position +
																																										   lenTest,

																																											   m_matchFinder
																																											   .GetIndexByte(
																																												   (int
																																												   )lenTest -
																																												   1 -
																																												   1))
																																									   .GetPrice(
																																										   true,

																																											   m_matchFinder
																																											   .GetIndexByte(
																																												   (int
																																												   )lenTest -
																																												   (int
																																												   )(
																																													   curBack +
																																													   1) -
																																												   1),

																																											   m_matchFinder
																																											   .GetIndexByte(
																																												   (int
																																												   )lenTest -
																																												   1));
									state2.UpdateChar();
									posStateNext = (position + lenTest + 1) & m_posStateMask;
									uint nextMatchPrice = curAndLenCharPrice + m_isMatch[(state2.Index << Base.kNumPosStatesBitsMax) + posStateNext].GetPrice1();
									uint nextRepMatchPrice = nextMatchPrice + m_isRep[state2.Index].GetPrice1();

									uint offset = lenTest + 1 + lenTest2;
									while (lenEnd < cur + offset)
									{
										m_optimum[++lenEnd].Price = Encoder.kIfinityPrice;
									}

									curAndLenPrice = nextRepMatchPrice + GetRepPrice(0, lenTest2, state2, posStateNext);
									optimum = m_optimum[cur + offset];
									if (curAndLenPrice < optimum.Price)
									{
										optimum.Price = curAndLenPrice;
										optimum.PosPrev = cur + lenTest + 1;
										optimum.BackPrev = 0;
										optimum.Prev1IsChar = true;
										optimum.Prev2 = true;
										optimum.PosPrev2 = cur;
										optimum.BackPrev2 = curBack + Base.kNumRepDistances;
									}
								}
							}

							offs += 2;
							if (offs == numDistancePairs)
							{
								break;
							}
						}
					}
				}
			}
		}

		private bool ChangePair(uint smallDist, uint bigDist)
		{
			const int kDif = 7;
			return smallDist < (uint)1 << (32 - kDif) && bigDist >= smallDist << kDif;
		}

		private void WriteEndMarker(uint posState)
		{
			if (!m_writeEndMark)
			{
				return;
			}

			m_isMatch[(m_state.Index << Base.kNumPosStatesBitsMax) + posState].Encode(m_rangeEncoder, 1);
			m_isRep[m_state.Index].Encode(m_rangeEncoder, 0);
			m_state.UpdateMatch();
			uint len = Base.kMatchMinLen;
			m_lenEncoder.Encode(m_rangeEncoder, len - Base.kMatchMinLen, posState);
			uint posSlot = (1 << Base.kNumPosSlotBits) - 1;
			uint lenToPosState = Base.GetLenToPosState(len);
			m_posSlotEncoder[lenToPosState].Encode(m_rangeEncoder, posSlot);
			int footerBits = 30;
			uint posReduced = ((uint)1 << footerBits) - 1;
			m_rangeEncoder.EncodeDirectBits(posReduced >> Base.kNumAlignBits, footerBits - Base.kNumAlignBits);
			m_posAlignEncoder.ReverseEncode(m_rangeEncoder, posReduced & Base.kAlignMask);
		}

		private void Flush(uint nowPos)
		{
			ReleaseMFStream();
			WriteEndMarker(nowPos & m_posStateMask);
			m_rangeEncoder.FlushData();
			m_rangeEncoder.FlushStream();
		}

		public void CodeOneBlock(out long inSize, out long outSize, out bool finished)
		{
			inSize = 0;
			outSize = 0;
			finished = true;

			if (m_inStream != null)
			{
				m_matchFinder.SetStream(m_inStream);
				m_matchFinder.Init();
				m_needReleaseMFStream = true;
				m_inStream = null;
				if (m_trainSize > 0)
				{
					m_matchFinder.Skip(m_trainSize);
				}
			}

			if (m_finished)
			{
				return;
			}

			m_finished = true;


			long progressPosValuePrev = nowPos64;
			if (nowPos64 == 0)
			{
				if (m_matchFinder.GetNumAvailableBytes() == 0)
				{
					Flush((uint)nowPos64);
					return;
				}

				uint len, numDistancePairs; // it's not used
				ReadMatchDistances(out len, out numDistancePairs);
				uint posState = (uint)nowPos64 & m_posStateMask;
				m_isMatch[(m_state.Index << Base.kNumPosStatesBitsMax) + posState].Encode(m_rangeEncoder, 0);
				m_state.UpdateChar();
				byte curByte = m_matchFinder.GetIndexByte((int)(0 - m_additionalOffset));
				m_literalEncoder.GetSubCoder((uint)nowPos64, m_previousByte).Encode(m_rangeEncoder, curByte);
				m_previousByte = curByte;
				m_additionalOffset--;
				nowPos64++;
			}

			if (m_matchFinder.GetNumAvailableBytes() == 0)
			{
				Flush((uint)nowPos64);
				return;
			}

			while (true)
			{
				uint pos;
				uint len = GetOptimum((uint)nowPos64, out pos);

				uint posState = (uint)nowPos64 & m_posStateMask;
				uint complexState = (m_state.Index << Base.kNumPosStatesBitsMax) + posState;
				if (len == 1 && pos == 0xFFFFFFFF)
				{
					m_isMatch[complexState].Encode(m_rangeEncoder, 0);
					byte curByte = m_matchFinder.GetIndexByte((int)(0 - m_additionalOffset));
					LiteralEncoder.Encoder2 subCoder = m_literalEncoder.GetSubCoder((uint)nowPos64, m_previousByte);
					if (!m_state.IsCharState())
					{
						byte matchByte = m_matchFinder.GetIndexByte((int)(0 - m_repDistances[0] - 1 - m_additionalOffset));
						subCoder.EncodeMatched(m_rangeEncoder, matchByte, curByte);
					}
					else
					{
						subCoder.Encode(m_rangeEncoder, curByte);
					}

					m_previousByte = curByte;
					m_state.UpdateChar();
				}
				else
				{
					m_isMatch[complexState].Encode(m_rangeEncoder, 1);
					if (pos < Base.kNumRepDistances)
					{
						m_isRep[m_state.Index].Encode(m_rangeEncoder, 1);
						if (pos == 0)
						{
							m_isRepG0[m_state.Index].Encode(m_rangeEncoder, 0);
							if (len == 1)
							{
								m_isRep0Long[complexState].Encode(m_rangeEncoder, 0);
							}
							else
							{
								m_isRep0Long[complexState].Encode(m_rangeEncoder, 1);
							}
						}
						else
						{
							m_isRepG0[m_state.Index].Encode(m_rangeEncoder, 1);
							if (pos == 1)
							{
								m_isRepG1[m_state.Index].Encode(m_rangeEncoder, 0);
							}
							else
							{
								m_isRepG1[m_state.Index].Encode(m_rangeEncoder, 1);
								m_isRepG2[m_state.Index].Encode(m_rangeEncoder, pos - 2);
							}
						}

						if (len == 1)
						{
							m_state.UpdateShortRep();
						}
						else
						{
							m_repMatchLenEncoder.Encode(m_rangeEncoder, len - Base.kMatchMinLen, posState);
							m_state.UpdateRep();
						}

						uint distance = m_repDistances[pos];
						if (pos != 0)
						{
							for (uint i = pos; i >= 1; i--)
							{
								m_repDistances[i] = m_repDistances[i - 1];
							}

							m_repDistances[0] = distance;
						}
					}
					else
					{
						m_isRep[m_state.Index].Encode(m_rangeEncoder, 0);
						m_state.UpdateMatch();
						m_lenEncoder.Encode(m_rangeEncoder, len - Base.kMatchMinLen, posState);
						pos -= Base.kNumRepDistances;
						uint posSlot = Encoder.GetPosSlot(pos);
						uint lenToPosState = Base.GetLenToPosState(len);
						m_posSlotEncoder[lenToPosState].Encode(m_rangeEncoder, posSlot);

						if (posSlot >= Base.kStartPosModelIndex)
						{
							int footerBits = (int)((posSlot >> 1) - 1);
							uint baseVal = (2 | (posSlot & 1)) << footerBits;
							uint posReduced = pos - baseVal;

							if (posSlot < Base.kEndPosModelIndex)
							{
								BitTreeEncoder.ReverseEncode(m_posEncoders,
															 baseVal - posSlot - 1, m_rangeEncoder, footerBits, posReduced);
							}
							else
							{
								m_rangeEncoder.EncodeDirectBits(posReduced >> Base.kNumAlignBits, footerBits - Base.kNumAlignBits);
								m_posAlignEncoder.ReverseEncode(m_rangeEncoder, posReduced & Base.kAlignMask);
								m_alignPriceCount++;
							}
						}

						uint distance = pos;
						for (uint i = Base.kNumRepDistances - 1; i >= 1; i--)
						{
							m_repDistances[i] = m_repDistances[i - 1];
						}

						m_repDistances[0] = distance;
						m_matchPriceCount++;
					}

					m_previousByte = m_matchFinder.GetIndexByte((int)(len - 1 - m_additionalOffset));
				}

				m_additionalOffset -= len;
				nowPos64 += len;
				if (m_additionalOffset == 0)
				{
					// if (!_fastMode)
					if (m_matchPriceCount >= 1 << 7)
					{
						FillDistancesPrices();
					}

					if (m_alignPriceCount >= Base.kAlignTableSize)
					{
						FillAlignPrices();
					}

					inSize = nowPos64;
					outSize = m_rangeEncoder.GetProcessedSizeAdd();
					if (m_matchFinder.GetNumAvailableBytes() == 0)
					{
						Flush((uint)nowPos64);
						return;
					}

					if (nowPos64 - progressPosValuePrev >= 1 << 12)
					{
						m_finished = false;
						finished = false;
						return;
					}
				}
			}
		}

		private void ReleaseMFStream()
		{
			if (m_matchFinder != null && m_needReleaseMFStream)
			{
				m_matchFinder.ReleaseStream();
				m_needReleaseMFStream = false;
			}
		}

		private void SetOutStream(Stream outStream)
		{
			m_rangeEncoder.SetStream(outStream);
		}

		private void ReleaseOutStream()
		{
			m_rangeEncoder.ReleaseStream();
		}

		private void ReleaseStreams()
		{
			ReleaseMFStream();
			ReleaseOutStream();
		}

		private void SetStreams(Stream inStream, Stream outStream,
								long inSize, long outSize)
		{
			m_inStream = inStream;
			m_finished = false;
			Create();
			SetOutStream(outStream);
			Init();

			// if (!_fastMode)
			{
				FillDistancesPrices();
				FillAlignPrices();
			}

			m_lenEncoder.SetTableSize(m_numFastBytes + 1 - Base.kMatchMinLen);
			m_lenEncoder.UpdateTables((uint)1 << m_posStateBits);
			m_repMatchLenEncoder.SetTableSize(m_numFastBytes + 1 - Base.kMatchMinLen);
			m_repMatchLenEncoder.UpdateTables((uint)1 << m_posStateBits);

			nowPos64 = 0;
		}


		public void Code(Stream inStream, Stream outStream,
						 long inSize, long outSize, ICodeProgress progress)
		{
			m_needReleaseMFStream = false;
			try
			{
				SetStreams(inStream, outStream, inSize, outSize);
				while (true)
				{
					long processedInSize;
					long processedOutSize;
					bool finished;
					CodeOneBlock(out processedInSize, out processedOutSize, out finished);
					if (finished)
					{
						return;
					}

					if (progress != null)
					{
						progress.SetProgress(processedInSize, processedOutSize);
					}
				}
			}
			finally
			{
				ReleaseStreams();
			}
		}

		private const int kPropSize = 5;
		private readonly byte[] properties = new byte[Encoder.kPropSize];

		public void WriteCoderProperties(Stream outStream)
		{
			properties[0] = (byte)((m_posStateBits * 5 + m_numLiteralPosStateBits) * 9 + m_numLiteralContextBits);
			for (int i = 0; i < 4; i++)
			{
				properties[1 + i] = (byte)((m_dictionarySize >> (8 * i)) & 0xFF);
			}

			outStream.Write(properties, 0, Encoder.kPropSize);
		}

		private readonly uint[] tempPrices = new uint[Base.kNumFullDistances];
		private uint m_matchPriceCount;

		private void FillDistancesPrices()
		{
			for (uint i = Base.kStartPosModelIndex; i < Base.kNumFullDistances; i++)
			{
				uint posSlot = Encoder.GetPosSlot(i);
				int footerBits = (int)((posSlot >> 1) - 1);
				uint baseVal = (2 | (posSlot & 1)) << footerBits;
				tempPrices[i] = BitTreeEncoder.ReverseGetPrice(m_posEncoders,
																	baseVal - posSlot - 1, footerBits, i - baseVal);
			}

			for (uint lenToPosState = 0; lenToPosState < Base.kNumLenToPosStates; lenToPosState++)
			{
				uint posSlot;
				BitTreeEncoder encoder = m_posSlotEncoder[lenToPosState];

				uint st = lenToPosState << Base.kNumPosSlotBits;
				for (posSlot = 0; posSlot < m_distTableSize; posSlot++)
				{
					m_posSlotPrices[st + posSlot] = encoder.GetPrice(posSlot);
				}

				for (posSlot = Base.kEndPosModelIndex; posSlot < m_distTableSize; posSlot++)
				{
					m_posSlotPrices[st + posSlot] += ((posSlot >> 1) - 1 - Base.kNumAlignBits) << BitEncoder.kNumBitPriceShiftBits;
				}

				uint st2 = lenToPosState * Base.kNumFullDistances;
				uint i;
				for (i = 0; i < Base.kStartPosModelIndex; i++)
				{
					m_distancesPrices[st2 + i] = m_posSlotPrices[st + i];
				}

				for (; i < Base.kNumFullDistances; i++)
				{
					m_distancesPrices[st2 + i] = m_posSlotPrices[st + Encoder.GetPosSlot(i)] + tempPrices[i];
				}
			}

			m_matchPriceCount = 0;
		}

		private void FillAlignPrices()
		{
			for (uint i = 0; i < Base.kAlignTableSize; i++)
			{
				m_alignPrices[i] = m_posAlignEncoder.ReverseGetPrice(i);
			}

			m_alignPriceCount = 0;
		}


		private static readonly string[] kMatchFinderIDs =
		{
			"BT2",
			"BT4"
		};

		private static int FindMatchFinder(string s)
		{
			for (int m = 0; m < Encoder.kMatchFinderIDs.Length; m++)
			{
				if (s == Encoder.kMatchFinderIDs[m])
				{
					return m;
				}
			}

			return -1;
		}

		public void SetCoderProperties(CoderPropID[] propIDs, object[] properties)
		{
			for (uint i = 0; i < properties.Length; i++)
			{
				object prop = properties[i];
				switch (propIDs[i])
				{
					case CoderPropID.NumFastBytes:
						{
							if (!(prop is int))
							{
								throw new InvalidParamException();
							}

							int numFastBytes = (int)prop;
							if (numFastBytes < 5 || numFastBytes > Base.kMatchMaxLen)
							{
								throw new InvalidParamException();
							}

							m_numFastBytes = (uint)numFastBytes;
							break;
						}
					case CoderPropID.Algorithm:
						{
							/*
							if (!(prop is Int32))
								throw new InvalidParamException();
							Int32 maximize = (Int32)prop;
							m_fastMode = (maximize == 0);
							m_maxMode = (maximize >= 2);
							*/
							break;
						}
					case CoderPropID.MatchFinder:
						{
							if (!(prop is string))
							{
								throw new InvalidParamException();
							}

							EMatchFinderType matchFinderIndexPrev = m_matchFinderType;
							int m = Encoder.FindMatchFinder(((string)prop).ToUpper());
							if (m < 0)
							{
								throw new InvalidParamException();
							}

							m_matchFinderType = (EMatchFinderType)m;
							if (m_matchFinder != null && matchFinderIndexPrev != m_matchFinderType)
							{
								m_dictionarySizePrev = 0xFFFFFFFF;
								m_matchFinder = null;
							}

							break;
						}
					case CoderPropID.DictionarySize:
						{
							const int kDicLogSizeMaxCompress = 30;
							if (!(prop is int))
							{
								throw new InvalidParamException();
							}

						;
							int dictionarySize = (int)prop;
							if (dictionarySize < (uint)(1 << Base.kDicLogSizeMin) ||
								dictionarySize > (uint)(1 << kDicLogSizeMaxCompress))
							{
								throw new InvalidParamException();
							}

							m_dictionarySize = (uint)dictionarySize;
							int dicLogSize;
							for (dicLogSize = 0; dicLogSize < (uint)kDicLogSizeMaxCompress; dicLogSize++)
							{
								if (dictionarySize <= (uint)1 << dicLogSize)
								{
									break;
								}
							}

							m_distTableSize = (uint)dicLogSize * 2;
							break;
						}
					case CoderPropID.PosStateBits:
						{
							if (!(prop is int))
							{
								throw new InvalidParamException();
							}

							int v = (int)prop;
							if (v < 0 || v > (uint)Base.kNumPosStatesBitsEncodingMax)
							{
								throw new InvalidParamException();
							}

							m_posStateBits = v;
							m_posStateMask = ((uint)1 << m_posStateBits) - 1;
							break;
						}
					case CoderPropID.LitPosBits:
						{
							if (!(prop is int))
							{
								throw new InvalidParamException();
							}

							int v = (int)prop;
							if (v < 0 || v > Base.kNumLitPosStatesBitsEncodingMax)
							{
								throw new InvalidParamException();
							}

							m_numLiteralPosStateBits = v;
							break;
						}
					case CoderPropID.LitContextBits:
						{
							if (!(prop is int))
							{
								throw new InvalidParamException();
							}

							int v = (int)prop;
							if (v < 0 || v > Base.kNumLitContextBitsMax)
							{
								throw new InvalidParamException();
							}

						;
							m_numLiteralContextBits = v;
							break;
						}
					case CoderPropID.EndMarker:
						{
							if (!(prop is bool))
							{
								throw new InvalidParamException();
							}

							SetWriteEndMarkerMode((bool)prop);
							break;
						}
					default:
						throw new InvalidParamException();
				}
			}
		}

		private uint m_trainSize;

		public void SetTrainSize(uint trainSize)
		{
			m_trainSize = trainSize;
		}
	}
}