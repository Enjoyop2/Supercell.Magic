using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Debug;

namespace Supercell.Magic.Logic.GameObject
{
	public static class LogicGameObjectFactory
	{
		public static LogicGameObject CreateGameObject(LogicGameObjectData data, LogicLevel level, int villageType)
		{
			LogicGameObject gameObject = null;

			switch (data.GetDataType())
			{
				case DataType.BUILDING:
					gameObject = new LogicBuilding(data, level, villageType);
					break;
				case DataType.CHARACTER:
				case DataType.HERO:
					gameObject = new LogicCharacter(data, level, villageType);
					break;
				case DataType.PROJECTILE:
					gameObject = new LogicProjectile(data, level, villageType);
					break;
				case DataType.OBSTACLE:
					gameObject = new LogicObstacle(data, level, villageType);
					break;
				case DataType.TRAP:
					gameObject = new LogicTrap(data, level, villageType);
					break;
				case DataType.ALLIANCE_PORTAL:
					gameObject = new LogicAlliancePortal(data, level, villageType);
					break;
				case DataType.DECO:
					gameObject = new LogicDeco(data, level, villageType);
					break;
				case DataType.SPELL:
					gameObject = new LogicSpell(data, level, villageType);
					break;
				case DataType.VILLAGE_OBJECT:
					gameObject = new LogicVillageObject(data, level, villageType);
					break;
				default:
					{
						Debugger.Warning("Trying to create game object with data that does not inherit LogicGameObjectData. GlobalId=" + data.GetGlobalID());
						break;
					}
			}

			return gameObject;
		}
	}
}