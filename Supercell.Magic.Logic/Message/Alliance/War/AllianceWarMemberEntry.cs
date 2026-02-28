using Supercell.Magic.Logic.Message.Alliance.Stream;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Alliance.War
{
	public class AllianceWarMemberEntry
	{
		private LogicLong m_accountId;
		private LogicLong m_avatarId;
		private LogicLong m_homeId;

		private LogicArrayList<DonationContainer> m_donations;

		private string m_name;

		private int m_expLevel;
		private int m_index;

		public void Decode(ByteStream stream)
		{
			m_accountId = stream.ReadLong();
			m_avatarId = stream.ReadLong();
			m_homeId = stream.ReadLong();
			m_name = stream.ReadString(900000);
			m_expLevel = stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			m_index = stream.ReadInt();

			if (stream.ReadBoolean())
			{
				stream.ReadString(900000);
				stream.ReadInt();
				stream.ReadInt();
				stream.ReadInt();
				stream.ReadInt();
			}

			if (stream.ReadBoolean())
			{
				stream.ReadLong();
			}

			if (stream.ReadBoolean())
			{
				stream.ReadLong();
			}

			if (stream.ReadBoolean())
			{
				stream.ReadLong();
			}

			stream.ReadInt();
			stream.ReadInt();
			stream.ReadInt();
			stream.ReadString(900000);
			stream.ReadInt();

			int count = stream.ReadInt();

			if (count >= 0)
			{
				Debugger.DoAssert(count < 10000, "Too large amount of donations in AllianceWarMemberEntry");

				m_donations = new LogicArrayList<DonationContainer>();
				m_donations.EnsureCapacity(count);

				for (int i = stream.ReadInt(); i > 0; i--)
				{
					DonationContainer donationContainer = new DonationContainer();
					donationContainer.Decode(stream);
					m_donations.Add(donationContainer);
				}
			}
		}

		public void Encode(ByteStream encoder)
		{
			encoder.WriteLong(m_accountId);
			encoder.WriteLong(m_avatarId);
			encoder.WriteLong(m_homeId);
			encoder.WriteString(m_name);
			encoder.WriteInt(m_expLevel);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);

			encoder.WriteBoolean(false);

			if (false)
			{
				encoder.WriteString(null);
				encoder.WriteInt(0);
				encoder.WriteInt(0);
				encoder.WriteInt(0);
				encoder.WriteInt(0);
			}

			if (false)
			{
				encoder.WriteBoolean(true);
				encoder.WriteLong(0);
			}

			encoder.WriteBoolean(false);

			if (false)
			{
				encoder.WriteBoolean(true);
				encoder.WriteLong(0);
			}

			encoder.WriteBoolean(false);

			if (false)
			{
				encoder.WriteBoolean(true);
				encoder.WriteLong(0);
			}

			encoder.WriteBoolean(false);

			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteInt(0);
			encoder.WriteString(null);
			encoder.WriteInt(0);

			if (m_donations != null)
			{
				encoder.WriteInt(m_donations.Size());

				for (int i = 0; i < m_donations.Size(); i++)
				{
					m_donations[i].Encode(encoder);
				}
			}
			else
			{
				encoder.WriteInt(0);
			}
		}

		public LogicLong GetAccountId()
			=> m_accountId;

		public void SetAccountId(LogicLong value)
		{
			m_accountId = value;
		}

		public LogicLong GetAvatarId()
			=> m_avatarId;

		public void SetAvatarId(LogicLong value)
		{
			m_avatarId = value;
		}

		public LogicLong GetHomeId()
			=> m_homeId;

		public void SetHomeId(LogicLong value)
		{
			m_homeId = value;
		}

		public LogicArrayList<DonationContainer> GetDonations()
			=> m_donations;

		public string GetName()
			=> m_name;

		public void SetName(string value)
		{
			m_name = value;
		}

		public int GetExpLevel()
			=> m_expLevel;

		public void SetExpLevel(int value)
		{
			m_expLevel = value;
		}
	}
}