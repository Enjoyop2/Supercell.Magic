using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Request
{
	public abstract class ServerRequestMessage : ServerMessage
	{
		internal long RequestId
		{
			get; set;
		}

		public sealed override void EncodeHeader(ByteStream stream)
		{
			base.EncodeHeader(stream);
			stream.WriteLongLong(RequestId);
		}

		public sealed override void DecodeHeader(ByteStream stream)
		{
			base.DecodeHeader(stream);
			RequestId = stream.ReadLongLong();
		}

		public sealed override ServerMessageCategory GetMessageCategory()
			=> ServerMessageCategory.REQUEST;
	}
}