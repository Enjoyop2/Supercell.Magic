using System;
using System.Text;

using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Titan.DataStream
{
	public class ByteStream : ChecksumEncoder
	{
		private int m_bitIdx;

		private byte[] m_buffer;
		private int m_length;
		private int m_offset;

		public ByteStream(int capacity)
		{
			m_buffer = new byte[capacity];
		}

		public ByteStream(byte[] buffer, int length)
		{
			m_length = length;
			m_buffer = buffer;
		}

		public int GetLength()
		{
			if (m_offset < m_length)
			{
				return m_length;
			}

			return m_offset;
		}

		public int GetOffset()
			=> m_offset;

		public bool IsAtEnd()
			=> m_offset >= m_length;

		public void Clear(int capacity)
		{
			m_buffer = new byte[capacity];
			m_offset = 0;
		}

		public byte[] GetByteArray()
			=> m_buffer;

		public bool ReadBoolean()
		{
			if (m_bitIdx == 0)
			{
				++m_offset;
			}

			bool value = (m_buffer[m_offset - 1] & (1 << m_bitIdx)) != 0;
			m_bitIdx = (m_bitIdx + 1) & 7;
			return value;
		}

		public byte ReadByte()
		{
			m_bitIdx = 0;
			return m_buffer[m_offset++];
		}

		public short ReadShort()
		{
			m_bitIdx = 0;

			return (short)((m_buffer[m_offset++] << 8) |
					m_buffer[m_offset++]);
		}

		public int ReadInt()
		{
			m_bitIdx = 0;

			return (m_buffer[m_offset++] << 24) |
				   (m_buffer[m_offset++] << 16) |
				   (m_buffer[m_offset++] << 8) |
				   m_buffer[m_offset++];
		}

		public int ReadVInt()
		{
			m_bitIdx = 0;
			int value = 0;
			byte byteValue = m_buffer[m_offset++];

			if ((byteValue & 0x40) != 0)
			{
				value |= byteValue & 0x3F;

				if ((byteValue & 0x80) != 0)
				{
					value |= ((byteValue = m_buffer[m_offset++]) & 0x7F) << 6;

					if ((byteValue & 0x80) != 0)
					{
						value |= ((byteValue = m_buffer[m_offset++]) & 0x7F) << 13;

						if ((byteValue & 0x80) != 0)
						{
							value |= ((byteValue = m_buffer[m_offset++]) & 0x7F) << 20;

							if ((byteValue & 0x80) != 0)
							{
								value |= ((byteValue = m_buffer[m_offset++]) & 0x7F) << 27;
								return (int)(value | 0x80000000);
							}

							return (int)(value | 0xF8000000);
						}

						return (int)(value | 0xFFF00000);
					}

					return (int)(value | 0xFFFFE000);
				}

				return (int)(value | 0xFFFFFFC0);
			}

			value |= byteValue & 0x3F;

			if ((byteValue & 0x80) != 0)
			{
				value |= ((byteValue = m_buffer[m_offset++]) & 0x7F) << 6;

				if ((byteValue & 0x80) != 0)
				{
					value |= ((byteValue = m_buffer[m_offset++]) & 0x7F) << 13;

					if ((byteValue & 0x80) != 0)
					{
						value |= ((byteValue = m_buffer[m_offset++]) & 0x7F) << 20;

						if ((byteValue & 0x80) != 0)
						{
							value |= ((byteValue = m_buffer[m_offset++]) & 0x7F) << 27;
						}
					}
				}
			}

			return value;
		}

		public LogicLong ReadLong()
		{
			LogicLong logicLong = new LogicLong();
			logicLong.Decode(this);
			return logicLong;
		}

		public LogicLong ReadLong(LogicLong longValue)
		{
			longValue.Decode(this);
			return longValue;
		}

		public long ReadLongLong()
			=> LogicLong.ToLong(ReadInt(), ReadInt());

		public int ReadBytesLength()
		{
			m_bitIdx = 0;
			return (m_buffer[m_offset++] << 24) |
				   (m_buffer[m_offset++] << 16) |
				   (m_buffer[m_offset++] << 8) |
				   m_buffer[m_offset++];
		}

		public byte[] ReadBytes(int length, int maxCapacity)
		{
			m_bitIdx = 0;

			if (length <= -1)
			{
				if (length != -1)
				{
					Debugger.Warning("Negative readBytes length encountered.");
				}

				return null;
			}

			if (length <= maxCapacity)
			{
				byte[] array = new byte[length];
				Buffer.BlockCopy(m_buffer, m_offset, array, 0, length);
				m_offset += length;
				return array;
			}

			Debugger.Warning("readBytes too long array, max " + maxCapacity);

			return null;
		}

		public string ReadString(int maxCapacity)
		{
			int length = ReadBytesLength();

			if (length <= -1)
			{
				if (length != -1)
				{
					Debugger.Warning("Negative String length encountered.");
				}
			}
			else
			{
				if (length <= maxCapacity)
				{
					string value = Encoding.UTF8.GetString(m_buffer, m_offset, length);
					m_offset += length;
					return value;
				}

				Debugger.Warning("Too long String encountered, max " + maxCapacity);
			}

			return null;
		}

		public string ReadStringReference(int maxCapacity)
		{
			int length = ReadBytesLength();

			if (length <= -1)
			{
				Debugger.Warning("Negative String length encountered.");
			}
			else
			{
				if (length <= maxCapacity)
				{
					string value = Encoding.UTF8.GetString(m_buffer, m_offset, length);
					m_offset += length;
					return value;
				}

				Debugger.Warning("Too long String encountered, max " + maxCapacity);
			}

			return string.Empty;
		}

		public override void WriteBoolean(bool value)
		{
			base.WriteBoolean(value);

			if (m_bitIdx == 0)
			{
				EnsureCapacity(1);
				m_buffer[m_offset++] = 0;
			}

			if (value)
			{
				m_buffer[m_offset - 1] |= (byte)(1 << m_bitIdx);
			}

			m_bitIdx = (m_bitIdx + 1) & 7;
		}

		public override void WriteByte(byte value)
		{
			base.WriteByte(value);
			EnsureCapacity(1);

			m_bitIdx = 0;

			m_buffer[m_offset++] = value;
		}

		public override void WriteShort(short value)
		{
			base.WriteShort(value);
			EnsureCapacity(2);

			m_bitIdx = 0;

			m_buffer[m_offset++] = (byte)(value >> 8);
			m_buffer[m_offset++] = (byte)value;
		}

		public override void WriteInt(int value)
		{
			base.WriteInt(value);
			EnsureCapacity(4);

			m_bitIdx = 0;

			m_buffer[m_offset++] = (byte)(value >> 24);
			m_buffer[m_offset++] = (byte)(value >> 16);
			m_buffer[m_offset++] = (byte)(value >> 8);
			m_buffer[m_offset++] = (byte)value;
		}

		public override void WriteVInt(int value)
		{
			base.WriteVInt(value);
			EnsureCapacity(5);

			m_bitIdx = 0;

			if (value >= 0)
			{
				if (value >= 64)
				{
					if (value >= 0x2000)
					{
						if (value >= 0x100000)
						{
							if (value >= 0x8000000)
							{
								m_buffer[m_offset++] = (byte)((value & 0x3F) | 0x80);
								m_buffer[m_offset++] = (byte)(((value >> 6) & 0x7F) | 0x80);
								m_buffer[m_offset++] = (byte)(((value >> 13) & 0x7F) | 0x80);
								m_buffer[m_offset++] = (byte)(((value >> 20) & 0x7F) | 0x80);
								m_buffer[m_offset++] = (byte)((value >> 27) & 0xF);
							}
							else
							{
								m_buffer[m_offset++] = (byte)((value & 0x3F) | 0x80);
								m_buffer[m_offset++] = (byte)(((value >> 6) & 0x7F) | 0x80);
								m_buffer[m_offset++] = (byte)(((value >> 13) & 0x7F) | 0x80);
								m_buffer[m_offset++] = (byte)((value >> 20) & 0x7F);
							}
						}
						else
						{
							m_buffer[m_offset++] = (byte)((value & 0x3F) | 0x80);
							m_buffer[m_offset++] = (byte)(((value >> 6) & 0x7F) | 0x80);
							m_buffer[m_offset++] = (byte)((value >> 13) & 0x7F);
						}
					}
					else
					{
						m_buffer[m_offset++] = (byte)((value & 0x3F) | 0x80);
						m_buffer[m_offset++] = (byte)((value >> 6) & 0x7F);
					}
				}
				else
				{
					m_buffer[m_offset++] = (byte)(value & 0x3F);
				}
			}
			else
			{
				if (value <= -0x40)
				{
					if (value <= -0x2000)
					{
						if (value <= -0x100000)
						{
							if (value <= -0x8000000)
							{
								m_buffer[m_offset++] = (byte)((value & 0x3F) | 0xC0);
								m_buffer[m_offset++] = (byte)(((value >> 6) & 0x7F) | 0x80);
								m_buffer[m_offset++] = (byte)(((value >> 13) & 0x7F) | 0x80);
								m_buffer[m_offset++] = (byte)(((value >> 20) & 0x7F) | 0x80);
								m_buffer[m_offset++] = (byte)((value >> 27) & 0xF);
							}
							else
							{
								m_buffer[m_offset++] = (byte)((value & 0x3F) | 0xC0);
								m_buffer[m_offset++] = (byte)(((value >> 6) & 0x7F) | 0x80);
								m_buffer[m_offset++] = (byte)(((value >> 13) & 0x7F) | 0x80);
								m_buffer[m_offset++] = (byte)((value >> 20) & 0x7F);
							}
						}
						else
						{
							m_buffer[m_offset++] = (byte)((value & 0x3F) | 0xC0);
							m_buffer[m_offset++] = (byte)(((value >> 6) & 0x7F) | 0x80);
							m_buffer[m_offset++] = (byte)((value >> 13) & 0x7F);
						}
					}
					else
					{
						m_buffer[m_offset++] = (byte)((value & 0x3F) | 0xC0);
						m_buffer[m_offset++] = (byte)((value >> 6) & 0x7F);
					}
				}
				else
				{
					m_buffer[m_offset++] = (byte)((value & 0x3F) | 0x40);
				}
			}
		}

		public void WriteIntToByteArray(int value)
		{
			EnsureCapacity(4);
			m_bitIdx = 0;

			m_buffer[m_offset++] = (byte)(value >> 24);
			m_buffer[m_offset++] = (byte)(value >> 16);
			m_buffer[m_offset++] = (byte)(value >> 8);
			m_buffer[m_offset++] = (byte)value;
		}

		public override void WriteLongLong(long value)
		{
			base.WriteLongLong(value);

			WriteIntToByteArray((int)(value >> 32));
			WriteIntToByteArray((int)value);
		}

		public override void WriteBytes(byte[] value, int length)
		{
			base.WriteBytes(value, length);

			if (value == null)
			{
				WriteIntToByteArray(-1);
			}
			else
			{
				EnsureCapacity(length + 4);
				WriteIntToByteArray(length);

				Buffer.BlockCopy(value, 0, m_buffer, m_offset, length);

				m_offset += length;
			}
		}

		public void WriteBytesWithoutLength(byte[] value, int length)
		{
			base.WriteBytes(value, length);

			if (value != null)
			{
				EnsureCapacity(length);
				Buffer.BlockCopy(value, 0, m_buffer, m_offset, length);
				m_offset += length;
			}
		}

		public override void WriteString(string value)
		{
			base.WriteString(value);

			if (value == null)
			{
				WriteIntToByteArray(-1);
			}
			else
			{
				byte[] bytes = LogicStringUtil.GetBytes(value);
				int length = bytes.Length;

				if (length <= 900000)
				{
					EnsureCapacity(length + 4);
					WriteIntToByteArray(length);

					Buffer.BlockCopy(bytes, 0, m_buffer, m_offset, length);

					m_offset += length;
				}
				else
				{
					Debugger.Warning("ByteStream::writeString invalid string length " + length);
					WriteIntToByteArray(-1);
				}
			}
		}

		public override void WriteStringReference(string value)
		{
			base.WriteStringReference(value);

			byte[] bytes = LogicStringUtil.GetBytes(value);
			int length = bytes.Length;

			if (length <= 900000)
			{
				EnsureCapacity(length + 4);
				WriteIntToByteArray(length);

				Buffer.BlockCopy(bytes, 0, m_buffer, m_offset, length);

				m_offset += length;
			}
			else
			{
				Debugger.Warning("ByteStream::writeString invalid string length " + length);
				WriteIntToByteArray(-1);
			}
		}

		public void SetByteArray(byte[] buffer, int length)
		{
			m_offset = 0;
			m_bitIdx = 0;
			m_buffer = buffer;
			m_length = length;
		}

		public void ResetOffset()
		{
			m_offset = 0;
			m_bitIdx = 0;
		}

		public void SetOffset(int offset)
		{
			m_offset = offset;
			m_bitIdx = 0;
		}

		public byte[] RemoveByteArray()
		{
			byte[] byteArray = m_buffer;
			m_buffer = null;
			return byteArray;
		}

		public void EnsureCapacity(int capacity)
		{
			int bufferLength = m_buffer.Length;

			if (m_offset + capacity > bufferLength)
			{
				byte[] tmpBuffer = new byte[bufferLength + capacity + 100];
				Buffer.BlockCopy(m_buffer, 0, tmpBuffer, 0, bufferLength);
				m_buffer = tmpBuffer;
			}
		}

		public void Destruct()
		{
			m_buffer = null;
			m_bitIdx = 0;
			m_length = 0;
			m_offset = 0;
		}
	}
}