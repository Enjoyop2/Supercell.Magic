using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class HomeBattleReplayDataMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24114;
		private byte[] m_replayData;

		public HomeBattleReplayDataMessage() : this(0)
		{
			// HomeBattleReplayDataMessage.
		}

		public HomeBattleReplayDataMessage(short messageVersion) : base(messageVersion)
		{
			// HomeBattleReplayDataMessage.
		}

		public override void Decode()
		{
			base.Decode();
			m_replayData = m_stream.ReadBytes(m_stream.ReadBytesLength(), 900000);
		}

		public override void Encode()
		{
			base.Encode();
			m_stream.WriteBytes(m_replayData, m_replayData.Length);
		}

		public byte[] RemoveReplayData()
		{
			byte[] tmp = m_replayData;
			m_replayData = null;
			return tmp;
		}

		public void SetReplayData(byte[] data)
		{
			m_replayData = data;
		}

		public override short GetMessageType()
			=> HomeBattleReplayDataMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
		}
	}
}