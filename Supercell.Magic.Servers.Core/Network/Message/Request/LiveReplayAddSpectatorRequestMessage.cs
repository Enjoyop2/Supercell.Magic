using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class LiveReplayAddSpectatorRequestMessage : ServerRequestMessage
	{
		public LogicLong LiveReplayId
		{
			get; set;
		}
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
			stream.WriteLong(LiveReplayId);
			stream.WriteLongLong(SessionId);
			stream.WriteVInt(SlotId);
		}

		public override void Decode(ByteStream stream)
		{
			LiveReplayId = stream.ReadLong();
			SessionId = stream.ReadLongLong();
			SlotId = stream.ReadVInt();
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.LIVE_REPLAY_ADD_SPECTATOR_REQUEST;
	}
}