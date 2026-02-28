using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class StartFriendlyChallengeSpectateMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14110;

		private LogicLong m_streamId;
		private LogicLong m_attackerId;

		public StartFriendlyChallengeSpectateMessage() : this(0)
		{
			// StartFriendlyChallengeSpectateMessage.
		}

		public StartFriendlyChallengeSpectateMessage(short messageVersion) : base(messageVersion)
		{
			// StartFriendlyChallengeSpectateMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_stream.ReadInt();

			if (m_stream.ReadBoolean())
			{
				m_streamId = m_stream.ReadLong();
			}

			if (m_stream.ReadBoolean())
			{
				m_attackerId = m_stream.ReadLong();
			}
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(0);
			m_stream.WriteBoolean(m_streamId != null);

			if (m_streamId != null)
			{
				m_stream.WriteLong(m_streamId);
			}

			m_stream.WriteBoolean(m_attackerId != null);

			if (m_attackerId != null)
			{
				m_stream.WriteLong(m_attackerId);
			}
		}

		public override short GetMessageType()
			=> StartFriendlyChallengeSpectateMessage.MESSAGE_TYPE;

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

		public LogicLong GetAttackerId()
			=> m_attackerId;

		public void SetAttackerId(LogicLong value)
		{
			m_attackerId = value;
		}
	}
}