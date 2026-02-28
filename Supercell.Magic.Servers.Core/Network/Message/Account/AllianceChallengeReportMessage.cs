using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class AllianceChallengeReportMessage : ServerAccountMessage
	{
		public LogicLong StreamId
		{
			get; set;
		}
		public LogicLong ReplayId
		{
			get; set;
		}
		public string BattleLog
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(StreamId);

			if (ReplayId != null)
			{
				stream.WriteBoolean(true);
				stream.WriteLong(ReplayId);
			}
			else
			{
				stream.WriteBoolean(false);
			}

			stream.WriteString(BattleLog);
		}

		public override void Decode(ByteStream stream)
		{
			StreamId = stream.ReadLong();

			if (stream.ReadBoolean())
			{
				ReplayId = stream.ReadLong();
			}

			BattleLog = stream.ReadString(900000);
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.ALLIANCE_CHALLENGE_REPORT;
	}
}