using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class GoHomeMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14101;

		private short m_mapId;
		private short m_layoutId;

		private int m_state3;

		private bool m_unk;

		public GoHomeMessage() : this(0)
		{
			// GoHomeMessage.
		}

		public GoHomeMessage(short messageVersion) : base(messageVersion)
		{
			// GoHomeMessage.
		}

		public override void Decode()
		{
			int a1 = m_stream.ReadInt();
			int a2 = m_stream.ReadInt();

			m_state3 = (int)(a1 & 0xffff0000) | (a2 >> 16);
			m_mapId = (short)(a1 & 0x0000ffff);
			m_layoutId = (short)(a2 & 0x0000ffff);
		}

		public override void Encode()
		{
			int a1 = (int)(m_state3 & 0xffff0000) | (m_mapId & 0x0000ffff);
			int a2 = ((m_state3 & 0x0000ffff) << 16) | (m_layoutId & 0x0000ffff);

			m_stream.WriteInt(a1);
			m_stream.WriteInt(a2);
		}

		public int GetMapId()
			=> m_mapId;

		public int GetLayoutId()
			=> m_layoutId;

		public int GetState3()
			=> m_state3;

		public override short GetMessageType()
			=> GoHomeMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 10;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}