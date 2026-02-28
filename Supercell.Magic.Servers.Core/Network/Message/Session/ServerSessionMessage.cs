using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public abstract class ServerSessionMessage : ServerMessage
	{
		public long SessionId
		{
			get; set;
		}

		public sealed override void EncodeHeader(ByteStream stream)
		{
			base.EncodeHeader(stream);
			stream.WriteLongLong(SessionId);
		}

		public sealed override void DecodeHeader(ByteStream stream)
		{
			base.DecodeHeader(stream);
			SessionId = stream.ReadLongLong();
		}

		public sealed override ServerMessageCategory GetMessageCategory()
			=> ServerMessageCategory.SESSION;
	}
}