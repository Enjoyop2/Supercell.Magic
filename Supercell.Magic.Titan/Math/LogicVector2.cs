using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Titan.Math
{
	public class LogicVector2
	{
		public int m_x;
		public int m_y;

		public LogicVector2()
		{
		}

		public LogicVector2(int x, int y)
		{
			m_x = x;
			m_y = y;
		}

		public void Destruct()
		{
			m_x = 0;
			m_y = 0;
		}

		public void Add(LogicVector2 vector2)
		{
			m_x += vector2.m_x;
			m_y += vector2.m_y;
		}

		public LogicVector2 Clone()
			=> new LogicVector2(m_x, m_y);

		public int Dot(LogicVector2 vector2)
			=> m_x * vector2.m_x + m_y * vector2.m_y;

		public int GetAngle()
			=> LogicMath.GetAngle(m_x, m_y);

		public int GetAngleBetween(int x, int y)
			=> LogicMath.GetAngleBetween(LogicMath.GetAngle(m_x, m_y), LogicMath.GetAngle(x, y));

		public int GetDistance(LogicVector2 vector2)
		{
			int x = m_x - vector2.m_x;
			int distance = 0x7FFFFFFF;

			if ((uint)(x + 46340) <= 92680)
			{
				int y = m_y - vector2.m_y;

				if ((uint)(y + 46340) <= 92680)
				{
					int distanceX = x * x;
					int distanceY = y * y;

					if ((uint)distanceY < (distanceX ^ 0x7FFFFFFFu))
					{
						distance = distanceX + distanceY;
					}
				}
			}

			return LogicMath.Sqrt(distance);
		}

		public int GetDistanceSquared(LogicVector2 vector2)
		{
			int x = m_x - vector2.m_x;
			int distance = 0x7FFFFFFF;

			if ((uint)(x + 46340) <= 92680)
			{
				int y = m_y - vector2.m_y;

				if ((uint)(y + 46340) <= 92680)
				{
					int distanceX = x * x;
					int distanceY = y * y;

					if ((uint)distanceY < (distanceX ^ 0x7FFFFFFFu))
					{
						distance = distanceX + distanceY;
					}
				}
			}

			return distance;
		}

		public int GetDistanceSquaredTo(int x, int y)
		{
			int distance = 0x7FFFFFFF;

			x -= m_x;

			if ((uint)(x + 46340) <= 92680)
			{
				y -= m_y;

				if ((uint)(y + 46340) <= 92680)
				{
					int distanceX = x * x;
					int distanceY = y * y;

					if ((uint)distanceY < (distanceX ^ 0x7FFFFFFFu))
					{
						distance = distanceX + distanceY;
					}
				}
			}

			return distance;
		}

		public int GetLength()
		{
			int length = 0x7FFFFFFF;

			if ((uint)(46340 - m_x) <= 92680)
			{
				if ((uint)(46340 - m_y) <= 92680)
				{
					int lengthX = m_x * m_x;
					int lengthY = m_y * m_y;

					if ((uint)lengthY < (lengthX ^ 0x7FFFFFFFu))
					{
						length = lengthX + lengthY;
					}
				}
			}

			return LogicMath.Sqrt(length);
		}

		public int GetLengthSquared()
		{
			int length = 0x7FFFFFFF;

			if ((uint)(46340 - m_x) <= 92680)
			{
				if ((uint)(46340 - m_y) <= 92680)
				{
					int lengthX = m_x * m_x;
					int lengthY = m_y * m_y;

					if ((uint)lengthY < (lengthX ^ 0x7FFFFFFFu))
					{
						length = lengthX + lengthY;
					}
				}
			}

			return length;
		}

		public bool IsEqual(LogicVector2 vector2)
			=> m_x == vector2.m_x && m_y == vector2.m_y;

		public bool IsInArea(int minX, int minY, int maxX, int maxY)
		{
			if (m_x >= minX && m_y >= minY)
				return m_x < minX + maxX && m_y < maxY + minY;
			return false;
		}

		public void Multiply(LogicVector2 vector2)
		{
			m_x *= vector2.m_x;
			m_y *= vector2.m_y;
		}

		public int Normalize(int value)
		{
			int length = GetLength();

			if (length != 0)
			{
				m_x = m_x * value / length;
				m_y = m_y * value / length;
			}

			return length;
		}

		public void Rotate(int degrees)
		{
			int newX = LogicMath.GetRotatedX(m_x, m_y, degrees);
			int newY = LogicMath.GetRotatedY(m_x, m_y, degrees);

			m_x = newX;
			m_y = newY;
		}

		public void Set(int x, int y)
		{
			m_x = x;
			m_y = y;
		}

		public void Substract(LogicVector2 vector2)
		{
			m_x -= vector2.m_x;
			m_y -= vector2.m_y;
		}

		public void Decode(ByteStream stream)
		{
			m_x = stream.ReadInt();
			m_y = stream.ReadInt();
		}

		public void Encode(ChecksumEncoder stream)
		{
			stream.WriteInt(m_x);
			stream.WriteInt(m_y);
		}

		public override string ToString()
			=> "LogicVector2(" + m_x + "," + m_y + ")";
	}
}