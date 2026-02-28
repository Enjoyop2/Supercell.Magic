using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public class ChatStreamEntry : StreamEntry
	{
		private string m_message;

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);
			m_message = stream.ReadString(900000);
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);
			stream.WriteString(m_message);
		}

		public string GetMessage()
			=> m_message;

		public void SetMessage(string message)
		{
			m_message = message;
		}

		public override StreamEntryType GetStreamEntryType()
			=> StreamEntryType.CHAT;

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = jsonObject.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("ChatStreamEntry::load base is NULL");
			}

			base.Load(baseObject);

			LogicJSONString messageObject = jsonObject.GetJSONString("message");

			if (messageObject != null)
			{
				m_message = messageObject.GetStringValue();
			}
		}

		public override void Save(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = new LogicJSONObject();

			base.Save(baseObject);

			jsonObject.Put("base", baseObject);
			jsonObject.Put("message", new LogicJSONString(m_message));
		}
	}
}