using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class DonateAllianceUnitMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14310;

		private LogicCombatItemData m_unitData;
		private LogicLong m_streamId;

		private bool m_quickDonate;

		public DonateAllianceUnitMessage() : this(0)
		{
			// DonateAllianceUnitMessage.
		}

		public DonateAllianceUnitMessage(short messageVersion) : base(messageVersion)
		{
			// DonateAllianceUnitMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_unitData = (LogicCombatItemData)ByteStreamHelper.ReadDataReference(m_stream,
																					   m_stream.ReadInt() != 0 ? LogicDataType.SPELL : LogicDataType.CHARACTER);
			m_streamId = m_stream.ReadLong();
			m_quickDonate = m_stream.ReadBoolean();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_unitData.GetCombatItemType());
			ByteStreamHelper.WriteDataReference(m_stream, m_unitData);
			m_stream.WriteBoolean(m_quickDonate);
		}

		public override short GetMessageType()
			=> DonateAllianceUnitMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();

			m_unitData = null;
			m_streamId = null;
		}

		public LogicCombatItemData GetAllianceUnitData()
			=> m_unitData;

		public void SetAllianceUnitData(LogicCombatItemData data)
		{
			m_unitData = data;
		}

		public LogicLong GetStreamId()
			=> m_streamId;

		public void SetStreamId(LogicLong id)
		{
			m_streamId = id;
		}

		public bool UseQuickDonate()
			=> m_quickDonate;

		public void SetQuickDonate(bool enabled)
		{
			m_quickDonate = enabled;
		}
	}
}