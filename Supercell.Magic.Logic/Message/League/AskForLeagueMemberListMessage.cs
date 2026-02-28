using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.League
{
	public class AskForLeagueMemberListMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 14503;
		private LogicLong m_leagueInstanceId;

		public AskForLeagueMemberListMessage() : this(0)
		{
			// AskForLeagueMemberListMessage.
		}

		public AskForLeagueMemberListMessage(short messageVersion) : base(messageVersion)
		{
			// AskForLeagueMemberListMessage.
		}

		public override void Decode()
		{
			base.Decode();

			if (m_stream.ReadBoolean())
			{
				m_leagueInstanceId = m_stream.ReadLong();
			}
		}

		public override void Encode()
		{
			base.Encode();

			if (m_leagueInstanceId != null)
			{
				m_stream.WriteBoolean(true);
				m_stream.WriteLong(m_leagueInstanceId);
			}
			else
			{
				m_stream.WriteBoolean(false);
			}
		}

		public override short GetMessageType()
			=> AskForLeagueMemberListMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 13;

		public override void Destruct()
		{
			base.Destruct();
			m_leagueInstanceId = null;
		}

		public LogicLong GetLeagueInstanceId()
			=> m_leagueInstanceId;

		public void SetLeagueInstanceId(LogicLong id)
		{
			m_leagueInstanceId = id;
		}
	}
}