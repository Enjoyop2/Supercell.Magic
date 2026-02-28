using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class DuelNpcMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14135;

		private LogicNpcData m_npcData;

		public DuelNpcMessage() : this(0)
		{
			// DuelNpcMessage.
		}

		public DuelNpcMessage(short messageVersion) : base(messageVersion)
		{
			// DuelNpcMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_npcData = (LogicNpcData)ByteStreamHelper.ReadDataReference(m_stream, LogicDataType.NPC);
		}

		public override void Encode()
		{
			base.Encode();
			ByteStreamHelper.WriteDataReference(m_stream, m_npcData);
		}

		public override short GetMessageType()
			=> DuelNpcMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public override void Destruct()
		{
			base.Destruct();
			m_npcData = null;
		}

		public LogicNpcData GetNpcData()
			=> m_npcData;

		public void SetNpcData(LogicNpcData data)
		{
			m_npcData = data;
		}
	}
}