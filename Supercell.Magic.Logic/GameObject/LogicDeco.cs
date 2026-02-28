using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;

namespace Supercell.Magic.Logic.GameObject
{
	public sealed class LogicDeco : LogicGameObject
	{
		public LogicDeco(LogicGameObjectData data, LogicLevel level, int villageType) : base(data, level, villageType)
		{
			AddComponent(new LogicLayoutComponent(this));
		}

		public LogicDecoData GetDecoData()
			=> (LogicDecoData)m_data;

		public override LogicGameObjectType GetGameObjectType()
			=> LogicGameObjectType.DECO;

		public override int GetWidthInTiles()
			=> GetDecoData().GetWidth();

		public override int GetHeightInTiles()
			=> GetDecoData().GetHeight();

		public override bool IsPassable()
			=> GetDecoData().IsPassable();
	}
}