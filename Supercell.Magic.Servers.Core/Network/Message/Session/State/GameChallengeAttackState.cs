using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class GameChallengeAttackState : GameState
	{
		public LogicLong LiveReplayId
		{
			get; set;
		}
		public LogicLong StreamId
		{
			get; set;
		}
		public LogicLong AllianceId
		{
			get; set;
		}

		public int MapId
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteLong(LiveReplayId);
			stream.WriteLong(StreamId);
			stream.WriteLong(AllianceId);
			stream.WriteVInt(MapId);
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			LiveReplayId = stream.ReadLong();
			StreamId = stream.ReadLong();
			AllianceId = stream.ReadLong();
			MapId = stream.ReadVInt();
		}

		public override GameStateType GetGameStateType()
			=> GameStateType.CHALLENGE_ATTACK;

		public override SimulationServiceNodeType GetSimulationServiceNodeType()
			=> SimulationServiceNodeType.BATTLE;
	}
}