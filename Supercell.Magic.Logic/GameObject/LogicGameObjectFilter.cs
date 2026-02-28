using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.GameObject
{
	public class LogicGameObjectFilter
	{
		private int m_team;
		private bool m_enemyOnly;
		private bool[] m_gameObjectTypes;

		private LogicArrayList<LogicGameObject> m_ignoreGameObjects;

		public LogicGameObjectFilter()
		{
			m_team = -1;
		}

		public virtual void Destruct()
		{
			m_gameObjectTypes = null;

			if (m_ignoreGameObjects != null)
			{
				m_ignoreGameObjects.Destruct();
				m_ignoreGameObjects = null;
			}
		}

		public bool ContainsGameObjectType(int type)
		{
			if (m_gameObjectTypes != null)
				return m_gameObjectTypes[type];
			return true;
		}

		public void AddGameObjectType(LogicGameObjectType type)
		{
			if (m_gameObjectTypes == null)
				m_gameObjectTypes = new bool[LogicGameObject.GAMEOBJECT_TYPE_COUNT];
			m_gameObjectTypes[(int)type] = true;
		}


		public virtual bool TestGameObject(LogicGameObject gameObject)
		{
			if (m_gameObjectTypes != null && !m_gameObjectTypes[(int)gameObject.GetGameObjectType()])
				return false;
			if (m_ignoreGameObjects != null && m_ignoreGameObjects.IndexOf(gameObject) != -1)
				return false;

			if (m_team != -1)
			{
				LogicHitpointComponent hitpointComponent = gameObject.GetHitpointComponent();

				if (hitpointComponent != null)
				{
					if (hitpointComponent.GetHitpoints() > 0)
					{
						bool isEnemy = hitpointComponent.IsEnemyForTeam(m_team);

						if (isEnemy || !m_enemyOnly)
						{
							return m_enemyOnly || !isEnemy;
						}
					}

					return false;
				}
			}

			return true;
		}

		public virtual bool IsComponentFilter()
			=> false;

		public void PassEnemyOnly(LogicGameObject gameObject)
		{
			LogicHitpointComponent hitpointComponent = gameObject.GetHitpointComponent();

			if (hitpointComponent != null)
			{
				m_team = hitpointComponent.GetTeam();
				m_enemyOnly = true;
			}
			else
			{
				m_team = -1;
			}
		}

		public void PassFriendlyOnly(LogicGameObject gameObject)
		{
			LogicHitpointComponent hitpointComponent = gameObject.GetHitpointComponent();

			if (hitpointComponent != null)
			{
				m_team = hitpointComponent.GetTeam();
				m_enemyOnly = false;
			}
			else
			{
				m_team = -1;
			}
		}

		public void RemoveAllIgnoreObjects()
		{
			if (m_ignoreGameObjects != null)
			{
				m_ignoreGameObjects.Destruct();
				m_ignoreGameObjects = null;
			}
		}

		public void AddIgnoreObject(LogicGameObject gameObject)
		{
			if (m_ignoreGameObjects == null)
				m_ignoreGameObjects = new LogicArrayList<LogicGameObject>();
			m_ignoreGameObjects.Add(gameObject);
		}
	}
}