using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class GameVisitState : GameState
	{
		public LogicClientAvatar HomeOwnerAvatar
		{
			get; set;
		}
		public int VisitType
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);
			HomeOwnerAvatar.Encode(stream);

			stream.WriteVInt(VisitType);
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);
			HomeOwnerAvatar = new LogicClientAvatar();
			HomeOwnerAvatar.Decode(stream);

			VisitType = stream.ReadVInt();
		}

		public override GameStateType GetGameStateType()
			=> GameStateType.VISIT;
	}
}