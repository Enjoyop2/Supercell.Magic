using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public class LiveReplayAddSpectatorResponseMessage : ServerResponseMessage
	{
		public Reason ErrorCode
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			if (!Success)
			{
				stream.WriteVInt((int)ErrorCode);
			}
		}

		public override void Decode(ByteStream stream)
		{
			if (!Success)
			{
				ErrorCode = (Reason)stream.ReadVInt();
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.LIVE_REPLAY_ADD_SPECTATOR_RESPONSE;

		public enum Reason
		{
			NOT_EXISTS,
			FULL
		}
	}
}