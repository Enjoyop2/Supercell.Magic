using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public class JoinRequestAllianceStreamEntry : StreamEntry
	{
		private string m_message;
		private string m_responderName;

		private int m_state;

		public JoinRequestAllianceStreamEntry()
		{
			m_state = 1;
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_message = stream.ReadString(900000);
			m_responderName = stream.ReadString(900000);
			m_state = stream.ReadInt();
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteString(m_message);
			stream.WriteString(m_responderName);
			stream.WriteInt(m_state);
		}

		public string GetMessage()
			=> m_message;

		public void SetMessage(string value)
		{
			m_message = value;
		}

		public string GetResponderName()
			=> m_responderName;

		public void SetResponderName(string value)
		{
			m_responderName = value;
		}

		public int GetState()
			=> m_state;

		public void SetState(int value)
		{
			m_state = value;
		}

		public override StreamEntryType GetStreamEntryType()
			=> StreamEntryType.JOIN_REQUEST;

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = jsonObject.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("JoinRequestAllianceStreamEntry::load base is NULL");
			}

			base.Load(baseObject);

			m_message = jsonObject.GetJSONString("message").GetStringValue();
			m_state = jsonObject.GetJSONNumber("state").GetIntValue();

			if (m_state != 1)
			{
				m_responderName = jsonObject.GetJSONString("responder_name").GetStringValue();
			}
		}

		public override void Save(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = new LogicJSONObject();

			base.Save(baseObject);

			jsonObject.Put("base", baseObject);
			jsonObject.Put("message", new LogicJSONString(m_message));
			jsonObject.Put("state", new LogicJSONNumber(m_state));

			if (m_state != 1)
			{
				jsonObject.Put("responder_name", new LogicJSONString(m_responderName));
			}
		}
	}
}