using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Message.Avatar
{
	public class AvatarProfileFullEntry
	{
		private LogicClientAvatar m_clientAvatar;

		private byte[] m_compressedHomeJSON;

		private int m_donations;
		private int m_donationsReceived;
		private int m_remainingSecsForWar;

		public void Destruct()
		{
			m_clientAvatar = null;
			m_compressedHomeJSON = null;
		}

		public void Encode(ChecksumEncoder encoder)
		{
			m_clientAvatar.Encode(encoder);

			encoder.WriteBytes(m_compressedHomeJSON, m_compressedHomeJSON.Length);
			encoder.WriteInt(m_donations);
			encoder.WriteInt(m_donationsReceived);
			encoder.WriteInt(m_remainingSecsForWar);
			encoder.WriteBoolean(true);
			encoder.WriteInt(0);
		}

		public void Decode(ByteStream stream)
		{
			m_clientAvatar = new LogicClientAvatar();
			m_clientAvatar.Decode(stream);

			m_compressedHomeJSON = stream.ReadBytes(stream.ReadBytesLength(), 900000);
			m_donations = stream.ReadInt();
			m_donationsReceived = stream.ReadInt();
			m_remainingSecsForWar = stream.ReadInt();

			stream.ReadBoolean();
			stream.ReadInt();
		}

		public LogicClientAvatar GetLogicClientAvatar()
			=> m_clientAvatar;

		public void SetLogicClientAvatar(LogicClientAvatar avatar)
		{
			m_clientAvatar = avatar;
		}

		public byte[] GetCompressdHomeJSON()
			=> m_compressedHomeJSON;

		public void SetCompressedHomeJSON(byte[] compressibleString)
		{
			m_compressedHomeJSON = compressibleString;
		}

		public int GetDonations()
			=> m_donations;

		public void SetDonations(int value)
		{
			m_donations = value;
		}

		public int GetDonationsReceived()
			=> m_donationsReceived;

		public void SetDonationsReceived(int value)
		{
			m_donationsReceived = value;
		}

		public int GetRemainingSecondsForWar()
			=> m_remainingSecsForWar;

		public void SetRemainingSecondsForWar(int value)
		{
			m_remainingSecsForWar = value;
		}
	}
}