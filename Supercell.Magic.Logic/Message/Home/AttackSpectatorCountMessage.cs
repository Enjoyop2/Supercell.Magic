using Supercell.Magic.Titan.Message;

namespace Supercell.Magic.Logic.Message.Home
{
	public class AttackSpectatorCountMessage : PiranhaMessage
	{
		public const int MESSAGE_TYPE = 24125;

		private int m_viewerCount;
		private int m_enemyViewerCount;

		public AttackSpectatorCountMessage() : this(0)
		{
			// AttackSpectatorCountMessage.
		}

		public AttackSpectatorCountMessage(short messageVersion) : base(messageVersion)
		{
			// AttackSpectatorCountMessage.
		}

		public override void Decode()
		{
			base.Decode();

			m_viewerCount = m_stream.ReadInt();
			m_enemyViewerCount = m_stream.ReadInt();
		}

		public override void Encode()
		{
			base.Encode();

			m_stream.WriteInt(m_viewerCount);
			m_stream.WriteInt(m_enemyViewerCount);
		}

		public override short GetMessageType()
			=> AttackSpectatorCountMessage.MESSAGE_TYPE;

		public override int GetServiceNodeType()
			=> 9;

		public override void Destruct()
		{
			base.Destruct();
		}

		public void SetViewerCount(int value)
		{
			m_viewerCount = value;
		}

		public int GetViewerCount()
			=> m_viewerCount;

		public void SetEnemyViewerCount(int value)
		{
			m_enemyViewerCount = value;
		}

		public int GetEnemyViewerCount()
			=> m_enemyViewerCount;
	}
}