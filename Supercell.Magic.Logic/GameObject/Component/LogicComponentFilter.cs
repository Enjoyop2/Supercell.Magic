namespace Supercell.Magic.Logic.GameObject.Component
{
	public sealed class LogicComponentFilter : LogicGameObjectFilter
	{
		private LogicComponentType m_componentType;

		public void SetComponentType(LogicComponentType type)
		{
			m_componentType = type;
		}

		public LogicComponentType GetComponentType()
			=> m_componentType;

		public override bool IsComponentFilter()
			=> true;

		public override bool TestGameObject(LogicGameObject gameObject)
		{
			if (gameObject.GetComponent(m_componentType) != null)
			{
				return base.TestGameObject(gameObject);
			}

			return false;
		}

		public bool TestComponent(LogicComponent component)
			=> TestGameObject(component.GetParent());
	}
}