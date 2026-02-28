using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class GameMatchedAttackState : GameState
	{
		public LogicLong LiveReplayId
		{
			get; set;
		}
		public LogicClientAvatar HomeOwnerAvatar
		{
			get; set;
		}
		public int MaintenanceTime
		{
			get; set;
		}
		public bool GameDefenderLocked
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);
			HomeOwnerAvatar.Encode(stream);

			stream.WriteVInt(MaintenanceTime);
			stream.WriteBoolean(GameDefenderLocked);
			stream.WriteLong(LiveReplayId);
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);
			HomeOwnerAvatar = new LogicClientAvatar();
			HomeOwnerAvatar.Decode(stream);
			MaintenanceTime = stream.ReadVInt();
			GameDefenderLocked = stream.ReadBoolean();
			LiveReplayId = stream.ReadLong();
		}

		public override GameStateType GetGameStateType()
			=> GameStateType.MATCHED_ATTACK;
	}
}