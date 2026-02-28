using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Offer
{
	public class LogicDeliverableSpecial : LogicDeliverable
	{
		private int m_id;

		public LogicDeliverableSpecial()
		{
			m_id = -1;
		}

		public override void Destruct()
		{
			base.Destruct();
			m_id = -1;
		}

		public override void WriteToJSON(LogicJSONObject jsonObject)
		{
			base.WriteToJSON(jsonObject);
			jsonObject.Put("id", new LogicJSONNumber(m_id));
		}

		public override void ReadFromJSON(LogicJSONObject jsonObject)
		{
			base.ReadFromJSON(jsonObject);
			m_id = LogicJSONHelper.GetInt(jsonObject, "id");
		}

		public override int GetDeliverableType()
			=> 4;

		public override bool Deliver(LogicLevel level)
		{
			LogicAvatar avatar = level.GetHomeOwnerAvatar();

			switch (m_id)
			{
				case 0:
					avatar.SetRedPackageState(avatar.GetRedPackageState() | 0x13);
					break;
				default:
					Debugger.Error("Unknown special delivery id " + m_id);
					break;
			}

			return true;
		}

		public override bool CanBeDeliver(LogicLevel level)
			=> true;

		public override LogicDeliverableBundle Compensate(LogicLevel level)
			=> null;

		public int GetId()
			=> m_id;

		public void SetId(int value)
		{
			m_id = value;
		}
	}
}