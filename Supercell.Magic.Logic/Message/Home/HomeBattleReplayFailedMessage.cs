using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class HomeBattleReplayFailedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24116;
		private Reason m_reason;

		public HomeBattleReplayFailedMessage() : this(0)
		{
			// HomeBattleReplayFailedMessage.
		}

		public HomeBattleReplayFailedMessage(short messageVersion) : base(messageVersion)
		{
			// HomeBattleReplayFailedMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_reason = (Reason)m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteInt((int)m_reason);
		}

		public override short GetMessageType()
			=> HomeBattleReplayFailedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
		}

		public enum Reason
		{
		}
	}
}