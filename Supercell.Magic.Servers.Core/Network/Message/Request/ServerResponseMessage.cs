using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public abstract class ServerResponseMessage : ServerMessage
	{
		internal long RequestId
		{
			get; set;
		}
		public bool Success
		{
			get; set;
		}

		public sealed override void EncodeHeader(ByteStream stream)
		{
			base.EncodeHeader(stream);
			stream.WriteLongLong(RequestId);
			stream.WriteBoolean(Success);
		}

		public sealed override void DecodeHeader(ByteStream stream)
		{
			base.DecodeHeader(stream);
			RequestId = stream.ReadLongLong();
			Success = stream.ReadBoolean();
		}

		public sealed override ServerMessageCategory GetMessageCategory()
			=> ServerMessageCategory.RESPONSE;
	}
}