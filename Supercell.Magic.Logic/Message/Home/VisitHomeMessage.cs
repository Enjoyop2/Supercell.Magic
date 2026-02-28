using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class VisitHomeMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14113;

		private LogicLong m_homeId;
		private int m_villageType;

		public VisitHomeMessage() : this(0)
		{
			// VisitHomeMessage.
		}

		public VisitHomeMessage(short messageVersion) : base(messageVersion)
		{
			// VisitHomeMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_homeId = m_stream.ReadLong();
			m_villageType = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteLong(m_homeId);
			m_stream.WriteInt(m_villageType);
		}

		public override short GetMessageType()
			=> VisitHomeMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public override void Destruct()
		{
			base.Destruct();
			m_homeId = null;
		}

		public int GetVillageType()
			=> m_villageType;

		public void SetVillageType(int villageType)
		{
			m_villageType = villageType;
		}

		public LogicLong RemoveHomeId()
		{
			LogicLong tmp = m_homeId;
			m_homeId = null;
			return tmp;
		}

		public void SetHomeId(LogicLong id)
		{
			m_homeId = id;
		}
	}
}