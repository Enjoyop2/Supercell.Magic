using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public class DonateStreamEntry : StreamEntry
	{
		private int m_castleLevel;
		private int m_castleUsedCapacity;
		private int m_castleUsedSpellCapacity;
		private int m_castleTotalCapacity;
		private int m_castleTotalSpellCapacity;
		private int m_donationPendingRequestCount;

		private string m_message;

		private LogicArrayList<LogicUnitSlot> m_unitCount;
		private LogicArrayList<DonationContainer> m_donationContainerList;

		public DonateStreamEntry()
		{
			m_donationContainerList = new LogicArrayList<DonationContainer>();
		}

		public override void Destruct()
		{
			base.Destruct();

			if (m_donationContainerList != null)
			{
				for (int i = m_donationContainerList.Size() - 1; i >= 0; i--)
				{
					m_donationContainerList[i].Destruct();
					m_donationContainerList.Remove(i);
				}

				m_donationContainerList = null;
			}

			if (m_unitCount != null)
			{
				for (int i = m_unitCount.Size() - 1; i >= 0; i--)
				{
					m_unitCount[i].Destruct();
					m_unitCount.Remove(i);
				}

				m_unitCount = null;
			}

			m_message = null;
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_castleLevel = stream.ReadInt();
			m_castleTotalCapacity = stream.ReadInt();
			m_castleTotalSpellCapacity = stream.ReadInt();
			m_castleUsedCapacity = stream.ReadInt();
			m_castleUsedSpellCapacity = stream.ReadInt();

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				DonationContainer donationContainer = new DonationContainer();
				donationContainer.Decode(stream);
				m_donationContainerList.Add(donationContainer);
			}

			if (stream.ReadBoolean())
			{
				m_message = stream.ReadString(900000);
			}

			int count = stream.ReadInt();

			if (count > -1)
			{
				m_unitCount = new LogicArrayList<LogicUnitSlot>(count);

				for (int i = 0; i < count; i++)
				{
					LogicUnitSlot unitSlot = new LogicUnitSlot(null, -1, 0);
					unitSlot.Decode(stream);
					m_unitCount.Add(unitSlot);
				}
			}
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteInt(m_castleLevel);
			stream.WriteInt(m_castleTotalCapacity);
			stream.WriteInt(m_castleTotalSpellCapacity);
			stream.WriteInt(m_castleUsedCapacity);
			stream.WriteInt(m_castleUsedSpellCapacity);
			stream.WriteInt(m_donationContainerList.Size());

			for (int i = 0; i < m_donationContainerList.Size(); i++)
			{
				m_donationContainerList[i].Encode(stream);
			}

			if (m_message != null)
			{
				stream.WriteBoolean(true);
				stream.WriteString(m_message);
			}
			else
			{
				stream.WriteBoolean(false);
			}

			if (m_unitCount != null)
			{
				stream.WriteInt(m_unitCount.Size());

				for (int i = 0; i < m_unitCount.Size(); i++)
				{
					m_unitCount[i].Encode(stream);
				}
			}
			else
			{
				stream.WriteInt(-1);
			}
		}

		public int GetDonationPendingRequestCount()
			=> m_donationPendingRequestCount;

		public void SetDonationPendingRequestCount(int count)
		{
			m_donationPendingRequestCount = count;
		}

		public int GetCastleLevel()
			=> m_castleLevel;

		public void SetCasteLevel(int castleLevel, int castleUsedCapacity, int castleUsedSpellCapacity, int castleTotalCapacity, int castleTotalSpellCapacity)
		{
			m_castleLevel = castleLevel;
			m_castleUsedCapacity = castleUsedCapacity;
			m_castleUsedSpellCapacity = castleUsedSpellCapacity;
			m_castleTotalCapacity = castleTotalCapacity;
			m_castleTotalSpellCapacity = castleTotalSpellCapacity;
		}

		public int GetCastleTotalCapacity(int unitType)
			=> unitType == 1 ? m_castleTotalSpellCapacity : m_castleTotalCapacity;

		public int GetCastleUsedCapacity(int unitType)
			=> (unitType == 1 ? m_castleUsedSpellCapacity : m_castleUsedCapacity) +
				   LogicDonationHelper.GetTotalDonationCapacity(m_donationContainerList, unitType);

		public bool IsCastleFull()
			=> LogicDonationHelper.GetTotalDonationCapacity(m_donationContainerList, 0) + m_castleUsedCapacity >= m_castleTotalCapacity &&
				   LogicDonationHelper.GetTotalDonationCapacity(m_donationContainerList, 1) + m_castleUsedSpellCapacity >= m_castleTotalSpellCapacity;

		public int GetTotalDonateCount(LogicLong avatarId, int unitType)
			=> LogicDonationHelper.GetTotalDonateCount(m_donationContainerList, avatarId, unitType);

		public int GetDonateCount(LogicLong avatarId, LogicCombatItemData data)
			=> LogicDonationHelper.GetDonateCount(m_donationContainerList, avatarId, data);

		public int GetTotalDonationCapacity(int unitType)
			=> LogicDonationHelper.GetTotalDonationCapacity(m_donationContainerList, unitType);

		public bool CanDonateAnything(LogicLong avatarId, int allianceLevel, bool includeDarkSpell)
		{
			if (!LogicLong.Equals(avatarId, GetSenderAvatarId()))
			{
				if (!IsCastleFull())
				{
					int totalTroopDonation = GetTotalDonateCount(avatarId, 0);
					int totalSpellDonation = GetTotalDonateCount(avatarId, 1);
					int freeSpellCapacity = m_castleTotalSpellCapacity - m_castleUsedSpellCapacity - totalSpellDonation;
					int maxTroopDonation = LogicDonationHelper.GetMaxUnitDonationCount(allianceLevel, 0);
					int maxSpellDonation = LogicDonationHelper.GetMaxUnitDonationCount(allianceLevel, 1);

					if (maxTroopDonation == totalTroopDonation && maxSpellDonation == totalSpellDonation)
					{
						return false;
					}

					if (!includeDarkSpell && freeSpellCapacity < 2 && m_castleTotalCapacity ==
						m_castleUsedSpellCapacity + LogicDonationHelper.GetTotalDonationCapacity(m_donationContainerList, 0))
					{
						return false;
					}

					return LogicDonationHelper.CanDonateAnything(m_donationContainerList, avatarId, allianceLevel);
				}
			}

			return false;
		}

		public bool CanAddDonation(LogicLong avatarId, LogicCombatItemData data, int allianceLevel)
		{
			if (!LogicLong.Equals(avatarId, GetSenderAvatarId()))
			{
				if (data.GetCombatItemType() == LogicCombatItemData.COMBAT_ITEM_TYPE_CHARACTER)
				{
					if (data.GetHousingSpace() + m_castleUsedCapacity + LogicDonationHelper.GetTotalDonationCapacity(m_donationContainerList, 0) >
						m_castleTotalCapacity)
					{
						return false;
					}
				}
				else
				{
					if (m_castleTotalSpellCapacity == 0 ||
						data.GetHousingSpace() + m_castleUsedSpellCapacity + LogicDonationHelper.GetTotalDonationCapacity(m_donationContainerList, 1) >
						m_castleTotalSpellCapacity)
					{
						return false;
					}
				}

				return LogicDonationHelper.CanAddDonation(m_donationContainerList, avatarId, data, allianceLevel);
			}

			return false;
		}

		public void AddDonation(LogicLong avatarId, LogicCombatItemData data, int upgLevel)
		{
			LogicDonationHelper.AddDonation(m_donationContainerList, avatarId, data, upgLevel);
		}

		public void RemoveDonation(LogicLong avatarId, LogicCombatItemData data, int upgLevel)
		{
			LogicDonationHelper.RemoveDonation(m_donationContainerList, avatarId, data, upgLevel);
		}

		public string GetMessage()
			=> m_message;

		public void SetMessage(string message)
		{
			m_message = message;
		}

		public int GetUnitTypeCount()
		{
			if (m_unitCount != null)
			{
				return m_unitCount.Size();
			}

			return 0;
		}

		public LogicCombatItemData GetUnitType(int idx)
			=> (LogicCombatItemData)m_unitCount[idx].GetData();

		public LogicArrayList<LogicUnitSlot> GetUnits()
			=> m_unitCount;

		public void SetUnits(LogicArrayList<LogicUnitSlot> slot)
		{
			m_unitCount = slot;
		}

		public int GetXPReward(LogicLong avatarId)
		{
			for (int i = 0; i < m_donationContainerList.Size(); i++)
			{
				if (LogicLong.Equals(m_donationContainerList[i].GetAvatarId(), avatarId))
				{
					return m_donationContainerList[i].GetXPReward();
				}
			}

			return 0;
		}

		public DonationContainer GetDonationContainer(LogicLong avatarId)
		{
			for (int i = 0; i < m_donationContainerList.Size(); i++)
			{
				if (LogicLong.Equals(m_donationContainerList[i].GetAvatarId(), avatarId))
				{
					return m_donationContainerList[i];
				}
			}

			return null;
		}

		public override StreamEntryType GetStreamEntryType()
			=> StreamEntryType.DONATE;

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = jsonObject.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("ChatStreamEntry::load base is NULL");
			}

			base.Load(baseObject);

			m_castleLevel = LogicJSONHelper.GetInt(jsonObject, "castle_level");
			m_castleUsedCapacity = LogicJSONHelper.GetInt(jsonObject, "castle_used");
			m_castleUsedSpellCapacity = LogicJSONHelper.GetInt(jsonObject, "castle_sp_used");
			m_castleTotalCapacity = LogicJSONHelper.GetInt(jsonObject, "castle_total");
			m_castleTotalSpellCapacity = LogicJSONHelper.GetInt(jsonObject, "castle_sp_total");

			LogicJSONString messageObject = jsonObject.GetJSONString("message");

			if (messageObject != null)
			{
				m_message = messageObject.GetStringValue();
			}

			LogicJSONArray donationArray = jsonObject.GetJSONArray("donators");

			if (donationArray != null)
			{
				for (int i = 0; i < donationArray.Size(); i++)
				{
					DonationContainer donationContainer = new DonationContainer();
					donationContainer.Load(donationArray.GetJSONObject(i));
					m_donationContainerList.Add(donationContainer);
				}
			}

			LogicJSONArray unitArray = jsonObject.GetJSONArray("units");

			if (unitArray != null)
			{
				m_unitCount = new LogicArrayList<LogicUnitSlot>();

				for (int i = 0; i < unitArray.Size(); i++)
				{
					LogicUnitSlot unitSlot = new LogicUnitSlot(null, -1, 0);
					unitSlot.ReadFromJSON(unitArray.GetJSONObject(i));
					m_unitCount.Add(unitSlot);
				}
			}
		}

		public override void Save(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = new LogicJSONObject();

			base.Save(baseObject);

			jsonObject.Put("base", baseObject);
			jsonObject.Put("castle_level", new LogicJSONNumber(m_castleLevel));
			jsonObject.Put("castle_used", new LogicJSONNumber(m_castleUsedCapacity));
			jsonObject.Put("castle_sp_used", new LogicJSONNumber(m_castleUsedSpellCapacity));
			jsonObject.Put("castle_total", new LogicJSONNumber(m_castleTotalCapacity));
			jsonObject.Put("castle_sp_total", new LogicJSONNumber(m_castleTotalSpellCapacity));

			if (m_message != null)
			{
				jsonObject.Put("message", new LogicJSONString(m_message));
			}

			LogicJSONArray donationArray = new LogicJSONArray();

			for (int i = 0; i < m_donationContainerList.Size(); i++)
			{
				donationArray.Add(m_donationContainerList[i].Save());
			}

			jsonObject.Put("donators", donationArray);

			if (m_unitCount != null)
			{
				LogicJSONArray unitArray = new LogicJSONArray();

				for (int i = 0; i < m_unitCount.Size(); i++)
				{
					LogicJSONObject obj = new LogicJSONObject();
					m_unitCount[i].WriteToJSON(obj);
					unitArray.Add(obj);
				}

				jsonObject.Put("units", unitArray);
			}
		}
	}
}