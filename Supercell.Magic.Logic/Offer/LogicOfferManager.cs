using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Offer
{
	public class LogicOfferManager
	{
		private LogicLevel m_level;
		private LogicTimer m_timer;
		private LogicOffer[] m_topOffer;
		private LogicJSONObject m_offerObject;
		private readonly LogicArrayList<LogicOffer> m_offers;

		private bool m_terminate;

		public LogicOfferManager(LogicLevel level)
		{
			m_level = level;
			m_offers = new LogicArrayList<LogicOffer>();
		}

		public void Init()
		{
			LogicDataTable table = LogicDataTables.GetTable(LogicDataType.GEM_BUNDLE);

			for (int i = 0; i < table.GetItemCount(); i++)
			{
				LogicGemBundleData data = (LogicGemBundleData)table.GetItemAt(i);
				LogicOffer offer = new LogicOffer(new LogicBundleOfferData(data), m_level);

				m_offers.Add(offer);
			}

			// TODO: Implement this.
		}

		public void Destruct()
		{
			m_level = null;
		}

		public void Load(LogicJSONObject root)
		{
			LogicJSONObject jsonObject = root.GetJSONObject("offer");

			if (jsonObject != null)
			{
				m_offerObject = jsonObject;

				if (m_timer != null)
				{
					m_timer.Destruct();
					m_timer = null;
				}

				m_timer = LogicTimer.GetLogicTimer(jsonObject, m_level.GetLogicTime(), "pct", 604800);

				if (jsonObject.Get("t") != null)
				{
					m_terminate = true;
				}

				LogicJSONArray offerArray = jsonObject.GetJSONArray("offers");

				if (offerArray != null)
				{
					for (int i = 0; i < offerArray.Size(); i++)
					{
						LogicJSONObject obj = (LogicJSONObject)offerArray.Get(i);

						if (obj != null)
						{
							int data = LogicJSONHelper.GetInt(obj, "data", -1);

							if (data != -1)
							{
								LogicOffer offer = GetOfferById(data);

								if (offer != null)
								{
									offer.Load(obj);
								}
							}
						}
						else
						{
							Debugger.Error("LogicOfferManager::load - Offer is NULL!");
						}
					}
				}

				for (int i = 0; i < 2; i++)
				{
					LogicJSONNumber number = (LogicJSONNumber)jsonObject.Get(i == 1 ? "top2" : "top");

					if (number != null)
					{
						m_topOffer[i] = GetOfferById(number.GetIntValue());
					}
				}
			}
		}

		public void Save(LogicJSONObject root)
		{
			if (m_offerObject != null && m_level.GetState() != 1)
			{
				root.Put("offer", m_offerObject);
			}
			else
			{
				LogicJSONObject jsonObject = new LogicJSONObject();
				LogicTimer.SetLogicTimer(jsonObject, m_timer, m_level, "pct");

				if (m_terminate)
				{
					jsonObject.Put("t", new LogicJSONBoolean(true));
				}

				LogicJSONArray offerArray = new LogicJSONArray();

				for (int i = 0; i < m_offers.Size(); i++)
				{
					LogicJSONObject obj = m_offers[i].Save();

					if (obj != null)
					{
						offerArray.Add(obj);
					}
				}

				if (m_offerObject != null)
				{
					LogicJSONArray oldArray = m_offerObject.GetJSONArray("offers");

					if (oldArray != null)
					{
						for (int i = 0; i < oldArray.Size(); i++)
						{
							LogicJSONObject obj = (LogicJSONObject)oldArray.Get(i);

							if (obj != null)
							{
								int data = LogicJSONHelper.GetInt(jsonObject, "data", -1);

								if (GetOfferById(data) == null)
								{
									offerArray.Add(obj);
								}
							}
						}
					}
				}

				root.Put("offers", offerArray);

				for (int i = 0; i < 2; i++)
				{
					if (m_topOffer[i] != null)
					{
						root.Put(i == 1 ? "top2" : "top", new LogicJSONNumber(m_topOffer[i].GetData().GetId()));
					}
				}

				root.Put("offer", jsonObject);
			}
		}

		public void FastForward(int time)
		{
			Debugger.HudPrint(string.Format("LogicOfferManager -> fastForward {0} sec", time));

			if (m_timer != null)
			{
				m_timer.FastForward(time);
			}
		}

		public LogicOffer GetOfferById(int id)
		{
			for (int i = 0; i < m_offers.Size(); i++)
			{
				if (m_offers[i].GetId() == id)
				{
					return m_offers[i];
				}
			}

			return null;
		}

		public void OfferBuyed(LogicOffer offer)
		{
			LogicOfferData data = offer.GetData();

			if (data.GetLinkedPackageId() != 0)
			{
				m_terminate = true;
			}

			int shopFrontPageCooldownAfterPurchaseSeconds = data.GetShopFrontPageCooldownAfterPurchaseSeconds();

			if (shopFrontPageCooldownAfterPurchaseSeconds > 0)
			{
				if (m_timer != null)
				{
					m_timer.Destruct();
					m_timer = null;
				}

				m_timer = new LogicTimer();
				m_timer.StartTimer(shopFrontPageCooldownAfterPurchaseSeconds, m_level.GetLogicTime(), false, -1);
			}
		}

		public void Tick()
		{
			if (m_timer != null && m_timer.GetRemainingSeconds(m_level.GetLogicTime()) <= 0)
			{
				if (m_timer != null)
				{
					m_timer.Destruct();
					m_timer = null;
				}
			}

			// TODO: Implement this.
		}
	}
}