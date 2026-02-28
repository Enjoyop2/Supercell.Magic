using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class SendAvatarStreamsToClientMessage : ServerSessionMessage
	{
		public LogicArrayList<LogicLong> StreamIds
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(StreamIds.Size());

			for (int i = 0; i < StreamIds.Size(); i++)
			{
				stream.WriteLong(StreamIds[i]);
			}
		}

		public override void Decode(ByteStream stream)
		{
			StreamIds = new LogicArrayList<LogicLong>();

			for (int i = stream.ReadVInt(); i > 0; i--)
			{
				StreamIds.Add(stream.ReadLong());
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.SEND_AVATAR_STREAMS_TO_CLIENT;
	}
}