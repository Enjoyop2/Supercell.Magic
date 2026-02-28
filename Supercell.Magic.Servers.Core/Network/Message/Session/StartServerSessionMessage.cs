using Supercell.Magic.Servers.Core.Network.Message.Request;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class StartServerSessionMessage : ServerSessionMessage
	{
		public LogicLong AccountId
		{
			get; set;
		}
		public string Country
		{
			get; set;
		}

		public LogicArrayList<int> ServerSocketTypeList
		{
			get; set;
		}
		public LogicArrayList<int> ServerSocketIdList
		{
			get; set;
		}
		public BindServerSocketRequestMessage BindRequestMessage
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLong(AccountId);
			stream.WriteString(Country);
			stream.WriteVInt(ServerSocketTypeList.Size());

			for (int i = 0; i < ServerSocketTypeList.Size(); i++)
			{
				stream.WriteVInt(ServerSocketTypeList[i]);
				stream.WriteVInt(ServerSocketIdList[i]);
			}

			if (BindRequestMessage != null)
			{
				stream.WriteBoolean(true);

				BindRequestMessage.EncodeHeader(stream);
				BindRequestMessage.Encode(stream);
			}
			else
			{
				stream.WriteBoolean(false);
			}
		}

		public override void Decode(ByteStream stream)
		{
			AccountId = stream.ReadLong();
			Country = stream.ReadString(900000);

			ServerSocketTypeList = new LogicArrayList<int>();
			ServerSocketIdList = new LogicArrayList<int>();

			for (int i = stream.ReadVInt(); i > 0; i--)
			{
				ServerSocketTypeList.Add(stream.ReadVInt());
				ServerSocketIdList.Add(stream.ReadVInt());
			}

			if (stream.ReadBoolean())
			{
				BindRequestMessage = new BindServerSocketRequestMessage();
				BindRequestMessage.DecodeHeader(stream);
				BindRequestMessage.Decode(stream);
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.START_SERVER_SESSION;
	}
}