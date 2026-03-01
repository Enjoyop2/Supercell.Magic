using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message
{
	public abstract class ServerMessage
	{
		public int SenderType
		{
			get; internal set;
		}
		public int SenderId
		{
			get; internal set;
		}

		public ServerSocket Sender
		{
			get
			{
				return ServerManager.GetSocket(SenderType, SenderId);
			}
		}

		public virtual void EncodeHeader(ByteStream stream)
		{
			stream.WriteVInt(SenderType);
			stream.WriteVInt(SenderId);
		}

		public virtual void DecodeHeader(ByteStream stream)
		{
			SenderType = stream.ReadVInt();
			SenderId = stream.ReadVInt();
		}

		public abstract void Encode(ByteStream stream);
		public abstract void Decode(ByteStream stream);

		public abstract ServerMessageCategory GetMessageCategory();
		public abstract ServerMessageType GetMessageType();
	}
}