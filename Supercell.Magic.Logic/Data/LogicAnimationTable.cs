using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicAnimationTable : LogicDataTable
	{
		public LogicAnimationTable(CSVNode node, DataType index) : base(node.GetTable(), index)
		{
		}

		public override void CreateReferences()
		{
			for (int i = 0; i < m_items.Size(); i++)
			{
				m_items[i].CreateReferences();
			}
		}

		public void SetTable(CSVNode node)
		{
			// TODO: Implement this.
		}
	}
}