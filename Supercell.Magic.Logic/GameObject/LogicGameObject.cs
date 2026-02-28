using System.Runtime.CompilerServices;

using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.GameObject.Listener;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Logic.GameObject
{
	public class LogicGameObject
	{
		public const int GAMEOBJECT_TYPE_COUNT = 9;

		protected readonly LogicGameObjectData m_data;
		protected readonly LogicLevel m_level;
		protected readonly LogicComponent[] m_components;

		protected LogicVector2 m_position;
		protected LogicGameObjectListener m_listener;

		protected int m_villageType;
		protected int m_globalId;
		protected int m_seed;

		private int m_freezeTime;
		private int m_freezeDelay;
		private int m_damageTime;
		private int m_preventsHealingTime;
		private int m_stealthTime;

		public LogicGameObject(LogicGameObjectData data, LogicLevel level, int villageType)
		{
			Debugger.DoAssert(villageType < 2, "VillageType not set! Game object has not been added to LogicGameObjectManager.");

			m_data = data;
			m_level = level;
			m_villageType = villageType;

			m_position = new LogicVector2();
			m_listener = new LogicGameObjectListener();
			m_components = new LogicComponent[LogicComponent.COMPONENT_TYPE_COUNT];
		}

		public virtual void Destruct()
		{
			if (m_level != null)
			{
				m_level.GetTileMap().RemoveGameObject(this);
			}

			for (int i = 0; i < LogicComponent.COMPONENT_TYPE_COUNT; i++)
			{
				if (m_components[i] != null)
				{
					m_components[i].Destruct();
					m_components[i] = null;
				}
			}

			if (m_position != null)
			{
				m_position.Destruct();
				m_position = null;
			}

			if (m_listener != null)
			{
				m_listener.Destruct();
				m_listener = null;
			}
		}

		public virtual void RemoveGameObjectReferences(LogicGameObject gameObject)
		{
			// RemoveGameObjectReferences.
		}

		public virtual void DeathEvent()
		{
			m_level.GetTileMap().RefreshPassable(this);

			if (m_listener != null)
			{
				m_listener.RefreshState();
			}
		}

		public virtual void SpawnEvent(LogicCharacterData data, int count, int upgLevel)
		{
		}

		public void AddComponent(LogicComponent component)
		{
			LogicComponentType componentType = component.GetComponentType();

			if (m_components[(int)componentType] == null)
			{
				m_level.GetComponentManagerAt(m_villageType).AddComponent(component);
				m_components[(int)componentType] = component;
			}
			else
			{
				Debugger.Error("LogicGameObject::addComponent - Component is already added.");
			}
		}

		public void EnableComponent(LogicComponentType componentType, bool enable)
		{
			LogicComponent component = m_components[(int)componentType];

			if (component != null)
			{
				component.SetEnabled(enable);
			}
		}

		public int GetX()
			=> m_position.m_x;

		public int GetY()
			=> m_position.m_y;

		public int GetTileX()
			=> m_position.m_x >> 9;

		public virtual int GetMidX()
			=> m_position.m_x + (GetWidthInTiles() << 8);

		public int GetTileY()
			=> m_position.m_y >> 9;

		public virtual int GetMidY()
			=> m_position.m_y + (GetHeightInTiles() << 8);

		public int GetDistanceSquaredTo(LogicGameObject gameObject)
		{
			int midX = GetMidX() - gameObject.GetMidX();
			int midY = GetMidY() - gameObject.GetMidY();

			return midX * midX + midY * midY;
		}

		public int GetVillageType()
			=> m_villageType;

		public int GetGlobalID()
			=> m_globalId;

		public LogicGameObjectData GetData()
			=> m_data;

		public LogicLevel GetLevel()
			=> m_level;

		public LogicGameObjectListener GetListener()
			=> m_listener;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public LogicComponent GetComponent(LogicComponentType componentType)
		{
			LogicComponent component = m_components[(int)componentType];

			if (component != null && component.IsEnabled())
			{
				return component;
			}

			return null;
		}

		public LogicCombatComponent GetCombatComponent(bool enabledOnly)
		{
			LogicCombatComponent component = (LogicCombatComponent)m_components[(int)LogicComponentType.COMBAT];

			if (component != null && (!enabledOnly || component.IsEnabled()))
			{
				return component;
			}

			return null;
		}

		public LogicCombatComponent GetCombatComponent()
			=> (LogicCombatComponent)GetComponent(LogicComponentType.COMBAT);

		public LogicHitpointComponent GetHitpointComponent()
			=> (LogicHitpointComponent)GetComponent(LogicComponentType.HITPOINT);

		public LogicMovementComponent GetMovementComponent()
			=> (LogicMovementComponent)GetComponent(LogicComponentType.MOVEMENT);

		public LogicResourceProductionComponent GetResourceProductionComponent()
			=> (LogicResourceProductionComponent)GetComponent(LogicComponentType.RESOURCE_PRODUCTION);

		public LogicBunkerComponent GetBunkerComponent()
			=> (LogicBunkerComponent)GetComponent(LogicComponentType.BUNKER);

		public LogicTriggerComponent GetTriggerComponent()
			=> (LogicTriggerComponent)GetComponent(LogicComponentType.TRIGGER);

		public LogicLayoutComponent GetLayoutComponent()
			=> (LogicLayoutComponent)GetComponent(LogicComponentType.LAYOUT);

		public LogicDefenceUnitProductionComponent GetDefenceUnitProduction()
			=> (LogicDefenceUnitProductionComponent)GetComponent(LogicComponentType.DEFENCE_UNIT_PRODUCTION);

		public void RemoveComponent(LogicComponentType componentType)
		{
			if (m_components[(int)componentType] != null)
			{
				m_components[(int)componentType].Destruct();
				m_components[(int)componentType] = null;
			}
		}

		public void Shrink(int time, int speedBoost)
		{
			LogicCombatComponent combatComponent = GetCombatComponent();

			if (combatComponent != null)
			{
				combatComponent.Boost(0, speedBoost, time);
			}

			LogicMovementComponent movementComponent = GetMovementComponent();

			if (movementComponent != null)
			{
				movementComponent.GetMovementSystem().Boost(speedBoost, time * 4);
			}
		}

		public void Freeze(int time, int delay)
		{
			if (m_freezeTime > 0 && m_freezeDelay == 0)
			{
				m_freezeTime = LogicMath.Max(time - delay, m_freezeTime);
			}
			else if (m_freezeDelay > 0)
			{
				m_freezeDelay = LogicMath.Max(delay, m_freezeDelay);
				m_freezeTime = LogicMath.Max(time, m_freezeTime);
			}
			else
			{
				m_freezeTime = time;
				m_freezeDelay = delay;
			}
		}

		public bool IsFrozen()
		{
			if (m_freezeTime <= 0 || m_freezeDelay != 0)
			{
				return false;
			}

			LogicMovementComponent movementComponent = GetMovementComponent();

			if (movementComponent != null)
			{
				return !movementComponent.IsInNotPassablePosition();
			}

			return true;
		}

		public bool IsPreventsHealing()
			=> m_preventsHealingTime > 0;

		public bool IsDamagedRecently()
			=> m_damageTime > 0;

		public bool IsStealthy()
			=> m_stealthTime > 0;

		public void SetStealthTime(int time)
		{
			m_stealthTime = time;
		}

		public void SetDamageTime(int time)
		{
			m_damageTime = time;
		}

		public void SetPreventsHealingTime(int time)
		{
			m_preventsHealingTime = time;
		}

		public LogicComponentManager GetComponentManager()
			=> m_level.GetComponentManagerAt(m_villageType);

		public LogicGameObjectManager GetGameObjectManager()
			=> m_level.GetGameObjectManagerAt(m_villageType);

		public void RefreshPassable()
		{
			m_level.GetTileMap().RefreshPassable(this);
		}

		public LogicVector2 GetPosition()
			=> m_position;

		public void SetPosition(LogicVector2 vector2)
		{
			m_position.Set(vector2.m_x, vector2.m_y);
		}

		public void SetPositionXY(int x, int y)
		{
			if (m_position.m_x != x || m_position.m_y != y)
			{
				int prevX = GetTileX();
				int prevY = GetTileY();

				m_position.Set(x, y);

				LogicLayoutComponent layoutComponent = GetLayoutComponent();

				if (layoutComponent != null)
				{
					layoutComponent.SetPositionLayout(m_level.GetActiveLayout(), x >> 9, y >> 9);
				}

				if (m_globalId != 0)
				{
					m_level.GetTileMap().GameObjectMoved(this, prevX, prevY);
				}
			}
		}

		public LogicVector2 GetPositionLayout(int layoutId, bool editMode)
		{
			LogicLayoutComponent layoutComponent = GetLayoutComponent();
			Debugger.DoAssert(layoutComponent != null, "LayoutComponent is null");

			if (editMode)
			{
				return layoutComponent.GetEditModePositionLayout(layoutId);
			}

			return layoutComponent.GetPositionLayout(layoutId);
		}

		public void SetPositionLayoutXY(int tileX, int tileY, int activeLayout, bool editMode)
		{
			if (m_components[(int)LogicComponentType.LAYOUT] != null)
			{
				LogicLayoutComponent layoutComponent = (LogicLayoutComponent)m_components[(int)LogicComponentType.LAYOUT];

				if (layoutComponent.IsEnabled())
				{
					if (editMode)
					{
						layoutComponent.SetEditModePositionLayout(activeLayout, tileX, tileY);
					}
					else
					{
						layoutComponent.SetPositionLayout(activeLayout, tileX, tileY);
					}
				}
			}
		}

		public void SetGlobalID(int globalId)
		{
			m_globalId = globalId;
		}

		public void SetSeed(int seed)
		{
			m_seed = seed;
		}

		public int Rand(int rnd)
		{
			int seed = m_seed + rnd;

			if (seed == 0)
			{
				seed = -1;
			}

			int tmp1 = seed ^ (seed << 14) ^ ((seed ^ (seed << 14)) >> 16);
			int tmp2 = (tmp1 ^ (32 * tmp1)) & 0x7FFFFFFF;

			return tmp2;
		}

		public void SetListener(LogicGameObjectListener listener)
		{
			m_listener = listener;
		}

		public void XpGainHelper(int xp, LogicAvatar homeOwnerAvatar, bool inHomeState)
		{
			LogicClientAvatar playerAvatar = m_level.GetPlayerAvatar();

			if (!homeOwnerAvatar.IsInExpLevelCap())
			{
				if (homeOwnerAvatar == playerAvatar && m_level.GetState() == 1 && inHomeState)
				{
					if (m_listener != null)
					{
						m_listener.XpGained(xp);
					}
				}
			}

			homeOwnerAvatar.XpGainHelper(xp);
		}

		public virtual void SetInitialPosition(int x, int y)
		{
			m_position.Set(x, y);

			LogicLayoutComponent layoutComponent = GetLayoutComponent();

			if (layoutComponent != null && m_level != null)
			{
				layoutComponent.SetPositionLayout(m_level.GetActiveLayout(), x >> 9, y >> 9);
			}
		}

		public virtual int GetDirection()
			=> 0;

		public virtual int PassableSubtilesAtEdge()
			=> 1;

		public virtual LogicGameObjectType GetGameObjectType()
			=> 0;

		public virtual bool IsStaticObject()
			=> true;

		public virtual bool IsHidden()
			=> false;

		public bool IsAlive()
		{
			LogicHitpointComponent hitpointComponent = GetHitpointComponent();
			return hitpointComponent == null || hitpointComponent.GetHitpoints() > 0;
		}

		public virtual bool IsBuilding()
			=> false;

		public virtual bool IsFlying()
		{
			LogicMovementComponent movementComponent = GetMovementComponent();
			return movementComponent != null && movementComponent.IsFlying();
		}

		public virtual bool IsPassable()
			=> true;

		public virtual bool IsUnbuildable()
			=> true;

		public virtual bool IsWall()
			=> false;

		public virtual bool IsHero()
			=> false;

		public virtual int PathFinderCost()
			=> 0;

		public virtual int GetHeightInTiles()
			=> 1;

		public virtual int GetWidthInTiles()
			=> 1;

		public virtual int GetRemainingBoostTime()
			=> 0;

		public virtual bool IsBoostPaused()
			=> false;

		public virtual void StopBoost()
		{
			// StopBoost.
		}

		public virtual int GetMaxFastForwardTime()
			=> -1;

		public virtual bool ShouldDestruct()
			=> false;

		public virtual int GetStrengthWeight()
			=> 0;

		public virtual void FastForwardTime(int secs)
		{
			for (int i = 0; i < LogicComponent.COMPONENT_TYPE_COUNT; i++)
			{
				LogicComponent component = m_components[i];

				if (component != null && component.IsEnabled())
				{
					component.FastForwardTime(secs);
				}
			}
		}

		public virtual void FastForwardBoost(int secs)
		{
			// FastForwardBoost.
		}

		public virtual int GetHitEffectOffset()
			=> 0;

		public virtual void GetChecksum(ChecksumHelper checksum, bool includeGameObjects)
		{
			if (includeGameObjects)
			{
				checksum.StartObject("LogicGameObject");

				checksum.WriteValue("type", (int)GetGameObjectType());
				checksum.WriteValue("globalID", m_globalId);
				checksum.WriteValue("dataGlobalID", m_data.GetGlobalID());
				checksum.WriteValue("x", GetX());
				checksum.WriteValue("y", GetY());
				checksum.WriteValue("seed", m_seed);

				LogicHitpointComponent hitpointComponent = GetHitpointComponent();

				if (hitpointComponent != null)
				{
					checksum.WriteValue("m_hp", hitpointComponent.GetHitpoints());
					checksum.WriteValue("m_maxHP", hitpointComponent.GetMaxHitpoints());
				}

				LogicCombatComponent combatComponent = GetCombatComponent();

				if (combatComponent != null)
				{
					LogicGameObject target = combatComponent.GetTarget(0);

					if (target != null)
					{
						checksum.WriteValue("target", target.GetGlobalID());
					}
				}

				checksum.EndObject();
			}
		}

		public virtual void SubTick()
		{
			// SubTick.
		}

		public virtual void Tick()
		{
			if (m_freezeTime <= 0 || m_freezeDelay != 0)
			{
				if (m_freezeDelay > 0)
				{
					m_freezeDelay -= 1;
				}
			}
			else
			{
				m_freezeTime -= 1;
			}

			if (m_preventsHealingTime > 0)
			{
				m_preventsHealingTime -= 1;
			}

			if (m_stealthTime > 0)
			{
				m_stealthTime -= 1;
			}

			if (m_damageTime > 0)
			{
				m_damageTime -= 1;
			}
		}

		public virtual void Load(LogicJSONObject jsonObject)
		{
			LoadPosition(jsonObject);

			for (int i = 0; i < LogicComponent.COMPONENT_TYPE_COUNT; i++)
			{
				LogicComponent component = m_components[i];

				if (component != null)
				{
					component.Load(jsonObject);
				}
			}
		}

		public virtual void LoadFromSnapshot(LogicJSONObject jsonObject)
		{
			LogicJSONNumber xNumber = jsonObject.GetJSONNumber("x");
			LogicJSONNumber yNumber = jsonObject.GetJSONNumber("y");

			if (xNumber == null || yNumber == null)
			{
				Debugger.Error("LogicGameObject::load - x or y is NULL!");
			}

			SetInitialPosition(xNumber.GetIntValue() << 9, yNumber.GetIntValue() << 9);

			for (int i = 0; i < LogicComponent.COMPONENT_TYPE_COUNT; i++)
			{
				LogicComponent component = m_components[i];

				if (component != null)
				{
					component.LoadFromSnapshot(jsonObject);
				}
			}
		}

		public void LoadPosition(LogicJSONObject jsonObject)
		{
			LogicJSONNumber xNumber = jsonObject.GetJSONNumber("x");
			LogicJSONNumber yNumber = jsonObject.GetJSONNumber("y");

			if (xNumber == null || yNumber == null)
			{
				Debugger.Error("LogicGameObject::load - x or y is NULL!");
			}

			SetInitialPosition(xNumber.GetIntValue() << 9, yNumber.GetIntValue() << 9);
		}

		public virtual void Save(LogicJSONObject jsonObject, int villageType)
		{
			jsonObject.Put("x", new LogicJSONNumber(GetTileX() & 63));
			jsonObject.Put("y", new LogicJSONNumber(GetTileY() & 63));

			for (int i = 0; i < LogicComponent.COMPONENT_TYPE_COUNT; i++)
			{
				LogicComponent component = m_components[i];

				if (component != null)
				{
					component.Save(jsonObject, villageType);
				}
			}
		}

		public virtual void SaveToSnapshot(LogicJSONObject jsonObject, int layoutId)
		{
			for (int i = 0; i < LogicComponent.COMPONENT_TYPE_COUNT; i++)
			{
				LogicComponent component = m_components[i];

				if (component != null)
				{
					component.SaveToSnapshot(jsonObject, layoutId);
				}
			}
		}

		public virtual void LoadingFinished()
		{
			for (int i = 0; i < LogicComponent.COMPONENT_TYPE_COUNT; i++)
			{
				LogicComponent component = m_components[i];

				if (component != null)
				{
					component.LoadingFinished();
				}
			}
		}
	}

	public enum LogicGameObjectType
	{
		BUILDING,
		CHARACTER,
		PROJECTILE,
		OBSTACLE,
		TRAP,
		ALLIANCE_PORTAL,
		DECO,
		SPELL,
		VILLAGE_OBJECT
	}
}