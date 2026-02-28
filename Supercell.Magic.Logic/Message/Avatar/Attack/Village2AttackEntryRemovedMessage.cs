using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Avatar.Attack
{
	public class Village2AttackEntryRemovedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24373;
		private LogicLong m_streamId;

		public Village2AttackEntryRemovedMessage() : this(0)
		{
			// Village2AttackEntryRemovedMessage.
		}

		public Village2AttackEntryRemovedMessage(short messageVersion) : base(messageVersion)
		{
			// Village2AttackEntryRemovedMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_streamId = m_stream.ReadLong();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteLong(m_streamId);
		}

		public override short GetMessageType()
			=> Village2AttackEntryRemovedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
		}

		public LogicLong GetStreamId()
			=> m_streamId;

		public void SetStreamId(LogicLong id)
		{
			m_streamId = id;
		}
	}
}