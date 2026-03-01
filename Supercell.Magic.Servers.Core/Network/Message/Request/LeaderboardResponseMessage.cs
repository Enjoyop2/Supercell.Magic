using Supercell.Magic.Logic.Message.Scoring;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class LeaderboardResponseMessage : ServerResponseMessage
	{

		public LogicArrayList<AvatarRankingEntry> MainLeaderboard
		{
			get; set;
		}

		public LogicArrayList<AvatarDuelRankingEntry> SecondaryLeaderboard
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			if (base.Success)
			{
				stream.WriteVInt(MainLeaderboard.Size());
				for (int i = 0; i < MainLeaderboard.Size(); i++)
				{
					MainLeaderboard[i].Encode(stream);
				}
				stream.WriteVInt(SecondaryLeaderboard.Size());
				for (int j = 0; j < SecondaryLeaderboard.Size(); j++)
				{
					SecondaryLeaderboard[j].Encode(stream);
				}
			}
		}

		public override void Decode(ByteStream stream)
		{
			if (base.Success)
			{
				MainLeaderboard = new LogicArrayList<AvatarRankingEntry>();
				SecondaryLeaderboard = new LogicArrayList<AvatarDuelRankingEntry>();
				for (int i = stream.ReadVInt(); i > 0; i--)
				{
					AvatarRankingEntry avatarRankingEntry = new AvatarRankingEntry();
					avatarRankingEntry.Decode(stream);
					MainLeaderboard.Add(avatarRankingEntry);
				}
				for (int j = stream.ReadVInt(); j > 0; j--)
				{
					AvatarDuelRankingEntry avatarDuelRankingEntry = new AvatarDuelRankingEntry();
					avatarDuelRankingEntry.Decode(stream);
					SecondaryLeaderboard.Add(avatarDuelRankingEntry);
				}
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.LEADERBOARD_RESPONSE;
	}

}