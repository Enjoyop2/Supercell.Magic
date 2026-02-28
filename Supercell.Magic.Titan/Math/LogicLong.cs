using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Titan.Math
{
	public class LogicLong
	{
		private int m_highInteger;
		private int m_lowInteger;

		public LogicLong()
		{
			// LogicLong.
		}

		public LogicLong(int highInteger, int lowInteger)
		{
			m_highInteger = highInteger;
			m_lowInteger = lowInteger;
		}

		public static long ToLong(int highValue, int lowValue)
			=> ((long)highValue << 32) | (uint)lowValue;

		public LogicLong Clone()
			=> new LogicLong(m_highInteger, m_lowInteger);

		public bool IsZero()
			=> m_highInteger == 0 && m_lowInteger == 0;

		public int GetHigherInt()
			=> m_highInteger;

		public int GetLowerInt()
			=> m_lowInteger;

		public void Decode(ByteStream stream)
		{
			m_highInteger = stream.ReadInt();
			m_lowInteger = stream.ReadInt();
		}

		public void Encode(ChecksumEncoder stream)
		{
			stream.WriteInt(m_highInteger);
			stream.WriteInt(m_lowInteger);
		}

		public int HashCode()
			=> m_lowInteger + 31 * m_highInteger;

		public override int GetHashCode()
			=> HashCode();

		public override bool Equals(object obj)
		{
			if (obj != null && obj is LogicLong logicLong)
				return logicLong.m_highInteger == m_highInteger && logicLong.m_lowInteger == m_lowInteger;
			return false;
		}

		public static bool Equals(LogicLong a1, LogicLong a2)
		{
			if (a1 == null || a2 == null)
				return a1 == null && a2 == null;
			return a1.m_highInteger == a2.m_highInteger && a1.m_lowInteger == a2.m_lowInteger;
		}

		public override string ToString()
			=> string.Format("LogicLong({0}-{1})", m_highInteger, m_lowInteger);

		public static implicit operator LogicLong(long Long)
			=> new LogicLong((int)(Long >> 32), (int)Long);

		public static implicit operator long(LogicLong Long)
			=> ((long)Long.m_highInteger << 32) | (uint)Long.m_lowInteger;
	}
}