using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicBuildingClassData : LogicData
	{
		private bool m_townHall2Class;
		private bool m_townHallClass;
		private bool m_wallClass;
		private bool m_workerClass;
		private bool m_canBuy;
		private bool m_shopCategoryResource;
		private bool m_shopCategoryArmy;

		public LogicBuildingClassData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicBuildingClassData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_canBuy = GetBooleanValue("CanBuy", 0);
			m_shopCategoryResource = GetBooleanValue("ShopCategoryResource", 0);
			m_shopCategoryArmy = GetBooleanValue("ShopCategoryArmy", 0);

			m_workerClass = string.Equals("Worker", GetName());

			if (!m_workerClass)
			{
				m_workerClass = string.Equals("Worker2", GetName());
			}

			m_townHallClass = string.Equals("Town Hall", GetName());
			m_townHall2Class = string.Equals("Town Hall2", GetName());
			m_wallClass = string.Equals("Wall", GetName());
		}

		public bool IsWorker()
			=> m_workerClass;

		public bool IsTownHall()
			=> m_townHallClass;

		public bool IsTownHall2()
			=> m_townHall2Class;

		public bool IsWall()
			=> m_wallClass;

		public bool CanBuy()
			=> m_canBuy;
	}
}