using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Titan.Message
{
	public class PiranhaMessage
	{
		protected ByteStream m_stream;
		protected int m_version;

		public PiranhaMessage(short messageVersion)
		{
			m_stream = new ByteStream(10);
			m_version = messageVersion;
		}

		public virtual void Decode()
		{
		}

		public virtual void Encode()
		{
		}

		public virtual short GetMessageType()
			=> 0;

		public virtual void Destruct()
		{
			m_stream.Destruct();
		}

		public virtual int GetServiceNodeType()
			=> -1;

		public int GetMessageVersion()
			=> m_version;

		public void SetMessageVersion(int version)
		{
			m_version = version;
		}

		public bool IsServerToClientMessage()
			=> GetMessageType() >= 20000;

		public byte[] GetMessageBytes()
			=> m_stream.GetByteArray();

		public int GetEncodingLength()
			=> m_stream.GetLength();

		public ByteStream GetByteStream()
			=> m_stream;
	}
}