using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request.Stream
{
	public class LoadReplayStreamResponseMessage : ServerResponseMessage
	{
		public byte[] StreamData
		{
			get; set;
		}

		public int MajorVersion
		{
			get; set;
		}
		public int BuildVersion
		{
			get; set;
		}
		public int ContentVersion
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			if (Success)
			{
				stream.WriteBytes(StreamData, StreamData.Length);
				stream.WriteVInt(MajorVersion);
				stream.WriteVInt(BuildVersion);
				stream.WriteVInt(ContentVersion);
			}
		}

		public override void Decode(ByteStream stream)
		{
			if (Success)
			{
				StreamData = stream.ReadBytes(stream.ReadBytesLength(), 900000);
				MajorVersion = stream.ReadVInt();
				BuildVersion = stream.ReadVInt();
				ContentVersion = stream.ReadVInt();
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.LOAD_REPLAY_STREAM_RESPONSE;
	}
}