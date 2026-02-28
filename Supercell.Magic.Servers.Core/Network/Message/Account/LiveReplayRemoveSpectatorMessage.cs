using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class LiveReplayRemoveSpectatorMessage : ServerAccountMessage
	{
		public long SessionId
		{
			get; set;
		}
		public int SlotId
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLongLong(SessionId);
			stream.WriteVInt(SlotId);
		}

		public override void Decode(ByteStream stream)
		{
			SessionId = stream.ReadLongLong();
			SlotId = stream.ReadVInt();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.LIVE_REPLAY_REMOVE_SPECTATOR;
	}
}