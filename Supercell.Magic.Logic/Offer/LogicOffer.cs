using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Offer
{
	public class LogicOffer
	{
		private readonly LogicLevel m_level;
		private readonly LogicOfferData m_data;

		private int m_state;
		private int m_payCount;

		public LogicOffer(LogicOfferData data, LogicLevel level)
		{
			m_data = data;
			m_level = level;
		}

		public LogicOfferData GetData()
			=> m_data;

		public int GetId()
			=> m_data.GetId();

		public int GetState()
			=> m_state;

		public void SetState(int value)
		{
			m_state = value;
		}

		public LogicJSONObject Save()
		{
			if (m_payCount <= 0)
			{
				return null;
			}

			LogicJSONObject jsonObject = new LogicJSONObject();

			jsonObject.Put("data", new LogicJSONNumber(m_data.GetId()));
			jsonObject.Put("pc", new LogicJSONNumber(m_payCount));

			return jsonObject;
		}

		public void Load(LogicJSONObject jsonObject)
		{
			m_payCount = LogicJSONHelper.GetInt(jsonObject, "pc", 0);
		}

		public void AddPayCount(int value)
		{
			m_payCount += value;
			m_level.GetOfferManager().OfferBuyed(this);
		}
	}
}