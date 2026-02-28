using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.GameObject
{
	public sealed class LogicAlliancePortal : LogicGameObject
	{
		public LogicAlliancePortal(LogicGameObjectData data, LogicLevel level, int villageType) : base(data, level, villageType)
		{
			LogicBunkerComponent bunkerComponent = new LogicBunkerComponent(this, 0);
			bunkerComponent.SetComponentMode(0);
			AddComponent(bunkerComponent);
		}

		public LogicAlliancePortalData GetAlliancePortalData()
			=> (LogicAlliancePortalData)m_data;

		public override int GetWidthInTiles()
			=> 1;

		public override int GetHeightInTiles()
			=> 1;

		public override void Save(LogicJSONObject jsonObject, int villageType)
		{
			Debugger.Error("LogicAlliancePortal can't be saved");
		}

		public override void SaveToSnapshot(LogicJSONObject jsonObject, int layoutId)
		{
			Debugger.Error("LogicAlliancePortal can't be saved");
		}

		public override void Load(LogicJSONObject jsonObject)
		{
			Debugger.Error("LogicAlliancePortal can't be loaded");
		}

		public override void LoadFromSnapshot(LogicJSONObject jsonObject)
		{
			Debugger.Error("LogicAlliancePortal can't be loaded");
		}

		public override void LoadingFinished()
		{
			base.LoadingFinished();

			if (m_listener != null)
			{
				m_listener.LoadedFromJSON();
			}
		}

		public override bool ShouldDestruct()
			=> !m_level.IsInCombatState();

		public override bool IsPassable()
			=> true;

		public override LogicGameObjectType GetGameObjectType()
			=> LogicGameObjectType.ALLIANCE_PORTAL;
	}
}