using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Account
{
	public class InboxOpenedMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 10905;
		private LogicArrayList<int> m_eventInboxIds;

		public InboxOpenedMessage() : this(0)
		{
			// UnlockAccountMessage.
		}

		public InboxOpenedMessage(short messageVersion) : base(messageVersion)
		{
			// InboxOpenedMessage.
		}

		public override void Decode()
		{
			int count = m_stream.ReadVInt();

			m_eventInboxIds = new LogicArrayList<int>(count);
			Debugger.DoAssert(count < 1000, "Too many event inbox ids");

			for (int i = count; i > 0; i--)
			{
				m_eventInboxIds.Add(m_stream.ReadVInt());
			}
		}

		public override void Encode()
		{
			m_stream.WriteVInt(m_eventInboxIds.Size());

			for (int i = 0; i < m_eventInboxIds.Size(); i++)
			{
				m_stream.WriteVInt(m_eventInboxIds[i]);
			}
		}

		public override short GetMessageType()
			=> InboxOpenedMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 1;

		public override void Destruct()
		{
			base.Destruct();
		}

		public LogicArrayList<int> GetEventInboxIds()
			=> m_eventInboxIds;

		public void SetEventInboxIds(LogicArrayList<int> ids)
		{
			m_eventInboxIds = ids;
		}
	}
}