using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Core.Network.Message.Session
{
	public class SendAllianceBookmarksFullDataToClientMessage : ServerSessionMessage
	{
		public LogicArrayList<LogicLong> AllianceIds
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteVInt(AllianceIds.Size());

			for (int i = 0; i < AllianceIds.Size(); i++)
			{
				stream.WriteLong(AllianceIds[i]);
			}
		}

		public override void Decode(ByteStream stream)
		{
			AllianceIds = new LogicArrayList<LogicLong>();

			for (int i = stream.ReadVInt(); i > 0; i--)
			{
				AllianceIds.Add(stream.ReadLong());
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.SEND_ALLIANCE_BOOKMARKS_FULL_DATA_TO_CLIENT;
	}
}