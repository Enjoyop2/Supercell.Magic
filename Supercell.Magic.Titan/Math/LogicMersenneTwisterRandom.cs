namespace Supercell.Magic.Titan.Math
{
	public class LogicMersenneTwisterRandom
	{
		public const int SEED_COUNT = 624;

		private readonly int[] m_seeds;
		private int m_ix;

		public LogicMersenneTwisterRandom() : this(324876476)
		{
			// LogicMersenneTwisterRandom.
		}

		public LogicMersenneTwisterRandom(int seed)
		{
			m_seeds = new int[LogicMersenneTwisterRandom.SEED_COUNT];
			m_seeds[0] = seed;

			for (int i = 1; i < LogicMersenneTwisterRandom.SEED_COUNT; i++)
			{
				seed = 1812433253 * (seed ^ (seed >> 30)) + 1812433253;
				m_seeds[i] = seed;
			}
		}

		public void Initialize(int seed)
		{
			m_ix = 0;
			m_seeds[0] = seed;

			for (int i = 1; i < LogicMersenneTwisterRandom.SEED_COUNT; i++)
			{
				seed = 1812433253 * (seed ^ (seed >> 30)) + 1812433253;
				m_seeds[i] = seed;
			}
		}

		public void Generate()
		{
			for (int i = 1, j = 0; i <= LogicMersenneTwisterRandom.SEED_COUNT; i++, j++)
			{
				int v4 = (m_seeds[i % m_seeds.Length] & 0x7fffffff) + (m_seeds[j] & -0x80000000);
				int v6 = (v4 >> 1) ^ m_seeds[(i + 396) % m_seeds.Length];

				if ((v4 & 1) == 1)
				{
					v6 ^= -0x66F74F21;
				}

				m_seeds[j] = v6;
			}
		}

		public int NextInt()
		{
			if (m_ix == 0)
			{
				Generate();
			}

			int seed = m_seeds[m_ix];
			m_ix = (m_ix + 1) % 624;

			seed ^= seed >> 11;
			seed = seed ^ ((seed << 7) & -1658038656) ^ (((seed ^ ((seed << 7) & -1658038656)) << 15) & -0x103A0000) ^
				   ((seed ^ ((seed << 7) & -1658038656) ^ (((seed ^ ((seed << 7) & -1658038656)) << 15) & -0x103A0000)) >> 18);

			return seed;
		}

		public int Rand(int max)
		{
			int rnd = NextInt();

			if (rnd < 0)
			{
				rnd = -rnd;
			}

			return rnd % max;
		}
	}
}