using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject.Component
{
	public sealed class LogicSpawnerComponent : LogicComponent
	{
		private LogicArrayList<int> m_spawned;
		private LogicMersenneTwisterRandom m_randomizer;
		private LogicTimer m_timer;
		private LogicGameObjectData m_spawnData;

		private readonly int m_radius;
		private readonly int m_intervalSeconds;
		private readonly int m_spawnCount;
		private readonly int m_maxSpawned;
		private readonly int m_maxLifetimeSpawns;
		private int m_lifeTimeSpawns;

		private bool m_initialSpawnDone;

		public LogicSpawnerComponent(LogicGameObject gameObject, LogicObstacleData spawnData, int radius, int intervalSeconds, int spawnCount, int maxSpawned,
								   int maxLifetimeSpawns) : base(gameObject)
		{
			m_spawned = new LogicArrayList<int>();
			m_randomizer = new LogicMersenneTwisterRandom();

			m_spawnData = spawnData;
			m_radius = radius;
			m_intervalSeconds = intervalSeconds;
			m_spawnCount = spawnCount;
			m_maxSpawned = maxSpawned;
			m_maxLifetimeSpawns = maxLifetimeSpawns;

			m_randomizer.Initialize(m_parent.GetGlobalID());
			m_spawned.EnsureCapacity(maxSpawned);
		}

		public override void Destruct()
		{
			base.Destruct();

			if (m_timer != null)
			{
				m_timer.Destruct();
				m_timer = null;
			}

			m_spawnData = null;
			m_spawned = null;
			m_randomizer = null;
		}

		public override LogicComponentType GetComponentType()
			=> LogicComponentType.SPAWNER;

		public override void Tick()
		{
			if (!m_initialSpawnDone)
			{
				Spawn();
				m_initialSpawnDone = true;
			}
		}

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONBoolean initialSpawnDoneBoolean = jsonObject.GetJSONBoolean("initial_spawn_done");

			if (initialSpawnDoneBoolean != null)
			{
				m_initialSpawnDone = initialSpawnDoneBoolean.IsTrue();
			}

			if (m_timer != null)
			{
				m_timer.Destruct();
				m_timer = null;
			}

			m_timer = LogicTimer.GetLogicTimer(jsonObject, m_parent.GetLevel().GetLogicTime(), "spawn_timer", m_intervalSeconds);

			LogicJSONNumber lifetimeSpawnsNumber = jsonObject.GetJSONNumber("lifetime_spawns");

			if (lifetimeSpawnsNumber != null)
			{
				m_lifeTimeSpawns = lifetimeSpawnsNumber.GetIntValue();
			}

			LogicJSONArray spawnedArray = jsonObject.GetJSONArray("spawned");

			if (spawnedArray != null)
			{
				for (int i = 0; i < spawnedArray.Size(); i++)
				{
					m_spawned.Add(spawnedArray.GetJSONNumber(i).GetIntValue());
				}
			}
		}

		public override void LoadFromSnapshot(LogicJSONObject jsonObject)
		{
			if (m_timer != null)
			{
				m_timer.Destruct();
				m_timer = null;
			}
		}

		public override void Save(LogicJSONObject jsonObject, int villageType)
		{
			LogicTimer.SetLogicTimer(jsonObject, m_timer, m_parent.GetLevel(), "spawn_timer");

			jsonObject.Put("initial_spawn_done", new LogicJSONBoolean(m_initialSpawnDone));
			jsonObject.Put("lifetime_spawns", new LogicJSONNumber(m_lifeTimeSpawns));

			LogicJSONArray jsonArray = new LogicJSONArray();

			for (int i = 0; i < m_spawned.Size(); i++)
			{
				jsonArray.Add(new LogicJSONNumber(m_spawned[i]));
			}

			jsonObject.Put("spawned", jsonArray);
		}

		public override void FastForwardTime(int time)
		{
			while (time > 0)
			{
				LogicGameObjectManager gameObjectManager = m_parent.GetGameObjectManager();

				for (int i = 0; i < m_spawned.Size(); i++)
				{
					if (gameObjectManager.GetGameObjectByID(m_spawned[i]) == null)
					{
						m_spawned.Remove(i--);
					}
				}

				if (m_lifeTimeSpawns < m_maxLifetimeSpawns && m_spawned.Size() < m_maxSpawned)
				{
					if (m_timer == null)
					{
						m_timer = new LogicTimer();
						m_timer.StartTimer(m_intervalSeconds, m_parent.GetLevel().GetLogicTime(), false, -1);
					}

					int remainingSeconds = m_timer.GetRemainingSeconds(m_parent.GetLevel().GetLogicTime());

					if (time < remainingSeconds)
					{
						break;
					}

					m_timer.FastForward(remainingSeconds);
					Spawn();
					m_timer.StartTimer(m_intervalSeconds, m_parent.GetLevel().GetLogicTime(), false, -1);

					time -= remainingSeconds;
				}
				else
				{
					if (m_timer != null)
					{
						m_timer.Destruct();
						m_timer = null;
					}

					break;
				}
			}
		}

		private void Spawn()
		{
			int free = LogicMath.Min(LogicMath.Min(m_spawnCount, m_maxSpawned - m_spawned.Size()), m_maxLifetimeSpawns - m_lifeTimeSpawns);

			if (free > 0)
			{
				int x = m_parent.GetX();
				int y = m_parent.GetY();
				int tileX = m_parent.GetTileX();
				int tileY = m_parent.GetTileY();
				int width = m_parent.GetWidthInTiles();
				int height = m_parent.GetHeightInTiles();
				int levelWidth = m_parent.GetLevel().GetWidthInTiles();
				int levelHeight = m_parent.GetLevel().GetHeightInTiles();

				int startTileX = LogicMath.Clamp(tileX - m_radius, 0, levelWidth);
				int startTileY = LogicMath.Clamp(tileY - m_radius, 0, levelHeight);
				int endTileX = LogicMath.Clamp(tileX + m_radius + width, 0, levelWidth);
				int endTileY = LogicMath.Clamp(tileY + m_radius + height, 0, levelHeight);

				int radius = (m_radius << 9) * (m_radius << 9);
				int possibility = (endTileX - startTileX) * (endTileY - startTileY);

				LogicArrayList<LogicTile> spawnPoints = new LogicArrayList<LogicTile>(possibility);
				LogicTileMap tileMap = m_parent.GetLevel().GetTileMap();

				int spawnPointUpStartX = x + (width << 9);
				int spawnPointUpStartY = y + (height << 9);

				int tmp4 = y - 256 - (startTileY << 9);

				int startMidX = (startTileX << 9) | 256;
				int startMidY = (startTileY << 9) | 256;

				for (int i = startTileX, j = startMidX; i < endTileX; i++, j += 512)
				{
					int tmp1 = j >= spawnPointUpStartX ? -spawnPointUpStartX + j + 1 : 0;
					int tmp2 = j >= x ? tmp1 : x - j;

					tmp2 *= tmp2;

					for (int k = startTileY, l = startMidY, m = tmp4; k < endTileY; k++, l += 512, m -= 512)
					{
						LogicTile tile = tileMap.GetTile(i, k);

						if (tile.GetGameObjectCount() == 0)
						{
							int tmp3 = y <= l ? l < spawnPointUpStartY ? 0 : -spawnPointUpStartY + l + 1 : m;

							tmp3 *= tmp3;

							if (tmp2 + tmp3 <= radius)
							{
								spawnPoints.Add(tile);
							}
						}
					}
				}

				for (int i = free; i > 0 && spawnPoints.Size() > 0; i--, ++m_lifeTimeSpawns)
				{
					int idx = m_randomizer.Rand(spawnPoints.Size());

					LogicTile tile = spawnPoints[idx];
					LogicGameObject gameObject = LogicGameObjectFactory.CreateGameObject(m_spawnData, m_parent.GetLevel(), m_parent.GetVillageType());

					gameObject.SetInitialPosition(tile.GetX() << 9, tile.GetY() << 9);

					m_parent.GetGameObjectManager().AddGameObject(gameObject, -1);
					m_spawned.Add(gameObject.GetGlobalID());

					spawnPoints.Remove(idx);
				}
			}
		}
	}
}