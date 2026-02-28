using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Alliance
{
	public class AcceptFriendlyBattleMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14120;

		private LogicLong m_streamId;
		private int m_layoutId;

		public AcceptFriendlyBattleMessage() : this(0)
		{
			// AcceptFriendlyBattleMessage.
		}

		public AcceptFriendlyBattleMessage(short messageVersion) : base(messageVersion)
		{
			// AcceptFriendlyBattleMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_streamId = m_stream.ReadLong();
			m_layoutId = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteLong(m_streamId);
			m_stream.WriteInt(m_layoutId);
		}

		public override short GetMessageType()
			=> AcceptFriendlyBattleMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 11;

		public override void Destruct()
		{
			base.Destruct();
		}

		public LogicLong GetStreamId()
			=> m_streamId;

		public void SetStreamId(LogicLong value)
		{
			m_streamId = value;
		}

		public int GetLayoutId()
			=> m_layoutId;

		public void SetLayoutId(int value)
		{
			m_layoutId = value;
		}
	}
}