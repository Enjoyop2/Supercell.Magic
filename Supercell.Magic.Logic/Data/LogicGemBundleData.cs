using Supercell.Magic.Logic.Offer;
using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Data
{
	public class LogicGemBundleData : LogicData
	{
		private bool m_enabled;
		private bool m_existsApple;
		private bool m_existsAndroid;
		private bool m_existsKunlun;
		private bool m_existsBazaar;
		private bool m_existsTencent;
		private bool m_redPackage;
		private bool m_offeredByCalendar;
		private bool m_resourceAmountFromTownHallCSV;
		private bool m_alternativePackage;
		private bool m_hideTimer;
		private bool m_frontPageItem;
		private bool m_treasureItem;

		private string m_shopItemExportName;
		private string m_shopItemInfoExportName;
		private string m_shopItemBG;
		private string m_valueTID;

		private int m_townHallLimitMin;
		private int m_townHallLimitMax;
		private int m_priority;
		private int m_valueForUI;
		private int m_timesCanBePurchased;
		private int m_availableTimeMinutes;
		private int m_cooldownAfterPurchaseMinutes;
		private int m_shopFrontPageCooldownAfterPurchaseMin;
		private int m_linkedPackageId;
		private int m_giftGems;
		private int m_giftUsers;
		private int m_townHallResourceMultiplier;
		private int m_villageType;

		private LogicDeliverableBundle m_deliverableBundle;
		private LogicBillingPackageData m_billingPackageData;
		private LogicBillingPackageData m_replaceBillingPackageData;

		private LogicArrayList<LogicCombatItemData> m_unlockTroopData;
		private LogicArrayList<LogicResourceData> m_resourceData;
		private LogicArrayList<LogicData> m_buildingData;

		private LogicArrayList<int> m_resourceCount;
		private LogicArrayList<int> m_buildingNumber;
		private LogicArrayList<int> m_buildingLevel;
		private LogicArrayList<int> m_gemCost;

		public LogicGemBundleData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicGemBundleData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_enabled = GetBooleanValue("Disabled", 0) ^ true;
			m_existsApple = GetBooleanValue("ExistsApple", 0);
			m_existsAndroid = GetBooleanValue("ExistsAndroid", 0);
			m_existsKunlun = GetBooleanValue("ExistsKunlun", 0);
			m_existsBazaar = GetBooleanValue("ExistsBazaar", 0);
			m_existsTencent = GetBooleanValue("ExistsTencent", 0);
			m_shopItemExportName = GetValue("ShopItemExportName", 0);
			m_shopItemInfoExportName = GetValue("ShopItemInfoExportName", 0);
			m_shopItemBG = GetValue("ShopItemBG", 0);
			m_redPackage = GetBooleanValue("RED", 0);
			m_offeredByCalendar = GetBooleanValue("OfferedByCalendar", 0);
			m_townHallLimitMin = GetIntegerValue("TownhallLimitMin", 0);
			m_townHallLimitMax = GetIntegerValue("TownhallLimitMax", 0);
			m_resourceAmountFromTownHallCSV = GetBooleanValue("ResourceAmountFromThCSV", 0);

			int arraySize = GetArraySize("Resources");

			m_resourceData = new LogicArrayList<LogicResourceData>(arraySize);
			m_resourceCount = new LogicArrayList<int>(arraySize);

			for (int i = 0; i < arraySize; i++)
			{
				string resourceText = GetValue("Resources", i);

				if (resourceText.Length > 0)
				{
					LogicResourceData data = LogicDataTables.GetResourceByName(resourceText, this);

					if (data != null)
					{
						if (data.GetWarResourceReferenceData() != null)
						{
							Debugger.Error("Can't give WarResource as Resource in GemBundleData");
						}

						if (data.IsPremiumCurrency())
						{
							Debugger.Error("Can't give PremiumCurrency as Resource in GemBundleData");
						}

						m_resourceCount.Add(GetIntegerValue("ResourceAmounts", i));
					}
				}
			}

			arraySize = GetArraySize("Buildings");

			m_buildingData = new LogicArrayList<LogicData>(arraySize);
			m_buildingNumber = new LogicArrayList<int>(arraySize);
			m_buildingLevel = new LogicArrayList<int>(arraySize);
			m_gemCost = new LogicArrayList<int>(arraySize);

			for (int i = 0; i < arraySize; i++)
			{
				m_buildingNumber.Add(GetIntegerValue("BuildingNumber", i));
				m_buildingLevel.Add(GetIntegerValue("BuildingLevel", i));
				m_gemCost.Add(GetIntegerValue("GemCost", i));

				string buildingName = GetValue("Buildings", i);

				if (buildingName.Length > 0)
				{
					LogicData data = null;

					switch (GetValue("BuildingType", i))
					{
						case "building":
							data = LogicDataTables.GetBuildingByName(buildingName, this);
							break;
						case "deco":
							data = LogicDataTables.GetDecoByName(buildingName, this);
							break;
					}

					if (data != null)
					{
						m_buildingData.Add(data);
					}
				}
			}

			arraySize = GetArraySize("UnlocksTroop");

			m_unlockTroopData = new LogicArrayList<LogicCombatItemData>(arraySize);

			for (int i = 0; i < arraySize; i++)
			{
				string unlockTroopName = GetValue("UnlocksTroop", i);

				if (unlockTroopName.Length > 0)
				{
					LogicCombatItemData data = null;

					switch (GetValue("TroopType", i))
					{
						case "troop":
							data = LogicDataTables.GetCharacterByName(unlockTroopName, this);
							break;
						case "spell":
							data = LogicDataTables.GetSpellByName(unlockTroopName, this);
							break;
					}

					if (data != null)
					{
						m_unlockTroopData.Add(data);
					}
				}
			}

			m_billingPackageData = LogicDataTables.GetBillingPackageByName(GetValue("BillingPackage", 0), this);

			if (m_billingPackageData == null)
			{
				Debugger.Error("No billing package set!");
			}

			m_priority = GetIntegerValue("Priority", 0);
			m_frontPageItem = GetBooleanValue("FrontPageItem", 0);
			m_treasureItem = GetBooleanValue("TreasureItem", 0);
			m_valueForUI = GetIntegerValue("ValueForUI", 0);
			m_valueTID = GetValue("ValueTID", 0);
			m_timesCanBePurchased = GetIntegerValue("TimesCanBePurchased", 0);
			m_availableTimeMinutes = GetIntegerValue("AvailableTimeMinutes", 0);
			m_cooldownAfterPurchaseMinutes = GetIntegerValue("CooldownAfterPurchaseMinutes", 0);
			m_shopFrontPageCooldownAfterPurchaseMin = GetIntegerValue("ShopFrontPageCooldownAfterPurchaseMin", 0);
			m_hideTimer = GetBooleanValue("HideTimer", 0);
			m_linkedPackageId = GetIntegerValue("LinkedPackageID", 0);
			m_alternativePackage = GetName().EndsWith("_ALT");
			m_giftGems = GetIntegerValue("GiftGems", 0);
			m_giftUsers = GetIntegerValue("GiftUsers", 0);

			string replacesBillingPackageName = GetValue("ReplacesBillingPackage", 0);

			if (replacesBillingPackageName.Length > 0)
			{
				m_replaceBillingPackageData = LogicDataTables.GetBillingPackageByName(replacesBillingPackageName, this);
			}

			if (m_giftGems > 0 != m_giftUsers > 0)
			{
				Debugger.Error("Gift values should both be ZERO or both be NON-ZERO");
			}

			if (!m_frontPageItem && m_shopFrontPageCooldownAfterPurchaseMin > 0)
			{
				Debugger.Error("FrontPageItem = FALSE => ShopFrontPageCooldownAfterPurchaseMin must be set 0");
			}

			m_villageType = GetIntegerValue("VillageType", 0);

			if (m_villageType != -1)
			{
				if ((uint)m_villageType > 1)
				{
					Debugger.Error("invalid VillageType");
				}
			}

			if (m_enabled && m_availableTimeMinutes > 0)
			{
				Debugger.Warning("We should no longer use timed offers. Use chronos instead.");
			}

			if (m_offeredByCalendar)
			{
				Debugger.Warning("We no longer support enabling/disabling gem bundles thru chronos. Use chronos offers instead.");
				m_offeredByCalendar = false;
			}

			m_townHallResourceMultiplier = GetIntegerValue("THResourceMultiplier", 0);

			if (m_townHallResourceMultiplier <= 0)
			{
				m_townHallResourceMultiplier = 100;
			}

			m_deliverableBundle = CreateBundle();
		}

		public LogicDeliverableBundle CreateBundle()
		{
			LogicDeliverableBundle bundle = new LogicDeliverableBundle();

			if (m_buildingData != null)
			{
				for (int i = 0; i < m_buildingData.Size(); i++)
				{
					LogicData data = m_buildingData[i];

					int buildingLevel = m_buildingLevel[i];
					int buildingCount = m_buildingNumber[i];

					switch (data.GetDataType())
					{
						case LogicDataType.BUILDING:
							LogicDeliverableBuilding deliverableBuilding = new LogicDeliverableBuilding();

							deliverableBuilding.SetBuildingData((LogicBuildingData)data);
							deliverableBuilding.SetBuildingLevel(buildingLevel);
							deliverableBuilding.SetBuildingCount(buildingCount);

							bundle.AddDeliverable(deliverableBuilding);

							break;
						case LogicDataType.DECO:
							LogicDeliverableDecoration deliverableDecoration = new LogicDeliverableDecoration();
							deliverableDecoration.SetDecorationData((LogicDecoData)data);
							bundle.AddDeliverable(deliverableDecoration);

							break;
					}
				}
			}

			for (int i = 0; i < m_resourceData.Size(); i++)
			{
				if (m_resourceAmountFromTownHallCSV)
				{
					LogicDeliverableScaledMultiplier deliverableScaledMultiplier = new LogicDeliverableScaledMultiplier();

					deliverableScaledMultiplier.SetScaledResourceData(m_resourceData[i]);
					deliverableScaledMultiplier.SetScaledResourceMultiplier(m_townHallResourceMultiplier);

					bundle.AddDeliverable(deliverableScaledMultiplier);
				}
				else
				{
					LogicDeliverableResource deliverableResource = new LogicDeliverableResource();

					deliverableResource.SetResourceData(m_resourceData[i]);
					deliverableResource.SetResourceAmount(m_resourceCount[i]);

					bundle.AddDeliverable(deliverableResource);
				}
			}

			if (m_redPackage)
			{
				LogicDeliverableSpecial deliverableSpecial = new LogicDeliverableSpecial();
				deliverableSpecial.SetId(0);
				bundle.AddDeliverable(deliverableSpecial);
			}

			if (m_giftGems > 0)
			{
				LogicDeliverableGift deliverableGift = new LogicDeliverableGift();
				LogicDeliverableResource deliverableResource = new LogicDeliverableResource();

				deliverableGift.SetGiftLimit(m_giftUsers);
				deliverableResource.SetResourceData(LogicDataTables.GetDiamondsData());
				deliverableResource.SetResourceAmount(m_giftGems);

				bundle.AddDeliverable(deliverableGift);
				bundle.AddDeliverable(deliverableResource);
			}

			return bundle;
		}

		public LogicBillingPackageData GetBillingPackage()
			=> m_billingPackageData;

		public int GetLinkedPackageId()
			=> m_linkedPackageId;

		public int GetShopFrontPageCooldownAfterPurchaseSeconds()
			=> 60 * m_shopFrontPageCooldownAfterPurchaseMin;

		public int GetVillageType()
			=> m_villageType;
	}
}