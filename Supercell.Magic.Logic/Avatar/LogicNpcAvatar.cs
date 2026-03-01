using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Avatar
{
	public class LogicNpcAvatar : LogicAvatar
	{
		private LogicNpcData m_npcData;

		public LogicNpcAvatar()
		{
			InitBase();
		}

		public override void Destruct()
		{
			base.Destruct();
			m_npcData = null;
		}

		public LogicNpcData GetNpcData()
			=> m_npcData;

		public override int GetExpLevel()
			=> m_npcData.GetExpLevel();

		public override string GetName()
			=> m_npcData.GetPlayerName();

		public override int GetAllianceBadgeId()
			=> m_npcData.GetAllianceBadge();

		public void Encode(ChecksumEncoder encoder)
		{
			ByteStreamHelper.WriteDataReference(encoder, m_npcData);
		}

		public void Decode(ByteStream stream)
		{
			SetNpcData((LogicNpcData)ByteStreamHelper.ReadDataReference(stream, DataType.NPC));
		}

		public static LogicNpcAvatar GetNpcAvatar(LogicNpcData data)
		{
			LogicNpcAvatar npcAvatar = new LogicNpcAvatar();
			npcAvatar.SetNpcData(data);
			return npcAvatar;
		}

		public override bool IsNpcAvatar()
			=> true;

		public override LogicLeagueData GetLeagueTypeData()
			=> null;

		public override void SaveToReplay(LogicJSONObject jsonObject)
		{
		}

		public override void SaveToDirect(LogicJSONObject jsonObject)
		{
		}

		public override void LoadForReplay(LogicJSONObject jsonObject, bool direct)
		{
		}

		public void SetNpcData(LogicNpcData data)
		{
			m_npcData = data;

			SetResourceCount(LogicDataTables.GetGoldData(), m_npcData.GetGoldCount());
			SetResourceCount(LogicDataTables.GetElixirData(), m_npcData.GetElixirCount());

			if (m_allianceUnitCount.Size() != 0)
			{
				ClearUnitSlotArray(m_allianceUnitCount);
				m_allianceUnitCount = null;
			}

			if (m_unitCount.Size() != 0)
			{
				ClearDataSlotArray(m_unitCount);
				m_unitCount = null;
			}

			m_allianceUnitCount = new LogicArrayList<LogicUnitSlot>();
			m_unitCount = m_npcData.GetClonedUnits();
		}
	}
}