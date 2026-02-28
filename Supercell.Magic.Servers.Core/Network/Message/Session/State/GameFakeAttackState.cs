using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class GameFakeAttackState : GameState
	{
		public LogicClientAvatar HomeOwnerAvatar
		{
			get; set;
		}
		public int MaintenanceTime
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);
			HomeOwnerAvatar.Encode(stream);
			stream.WriteVInt(MaintenanceTime);
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);
			HomeOwnerAvatar = new LogicClientAvatar();
			HomeOwnerAvatar.Decode(stream);
			MaintenanceTime = stream.ReadVInt();
		}

		public override GameStateType GetGameStateType()
			=> GameStateType.FAKE_ATTACK;

		public override SimulationServiceNodeType GetSimulationServiceNodeType()
			=> SimulationServiceNodeType.BATTLE;
	}
}