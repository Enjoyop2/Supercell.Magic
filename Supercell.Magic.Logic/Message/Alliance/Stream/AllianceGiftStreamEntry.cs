using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Message.Alliance.Stream
{
	public class AllianceGiftStreamEntry : StreamEntry
	{
		private int m_diamondReward;
		private int m_giftCount;

		private LogicArrayList<LogicLong> m_collectedPlayers;

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			m_diamondReward = stream.ReadVInt();
			m_giftCount = stream.ReadVInt();

			int count = stream.ReadInt();

			if (count <= 50)
			{
				if (count > 0)
				{
					m_collectedPlayers = new LogicArrayList<LogicLong>(count);

					for (int i = 0; i < count; i++)
					{
						m_collectedPlayers.Add(stream.ReadLong());
					}
				}
			}
			else
			{
				Debugger.Error(string.Format("Number of collected players ({0}) is too high.", count));
			}
		}

		public override void Encode(ByteStream stream)
		{
			base.Encode(stream);

			stream.WriteVInt(m_diamondReward);
			stream.WriteVInt(m_giftCount);

			if (m_collectedPlayers != null)
			{
				stream.WriteInt(m_collectedPlayers.Size());

				for (int i = 0; i < m_collectedPlayers.Size(); i++)
				{
					stream.WriteLong(m_collectedPlayers[i]);
				}
			}
			else
			{
				stream.WriteInt(0);
			}
		}

		public bool CanClaimGift(LogicLong id)
		{
			if (m_collectedPlayers.Size() < m_giftCount)
			{
				return m_collectedPlayers.IndexOf(id) == -1;
			}

			return false;
		}

		public void AddCollectedPlayer(LogicLong id)
		{
			if (m_collectedPlayers.IndexOf(id) != -1)
			{
				Debugger.Error("AllianceGiftStreamEntry::addCollectedPlayer - specified player already added!");
			}

			m_collectedPlayers.Add(id);
		}

		public int GetDiamondReward()
			=> m_diamondReward;

		public void SetDiamondReward(int value)
		{
			m_diamondReward = value;
		}

		public int GetGiftCount()
			=> m_giftCount;

		public void SetGiftCount(int value)
		{
			m_giftCount = value;
		}

		public override StreamEntryType GetStreamEntryType()
			=> StreamEntryType.ALLIANCE_GIFT;

		public override void Load(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = jsonObject.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("AllianceGiftStreamEntry::load base is NULL");
			}

			base.Load(baseObject);

			m_diamondReward = jsonObject.GetJSONNumber("diamond_reward").GetIntValue();
			m_giftCount = jsonObject.GetJSONNumber("gift_count").GetIntValue();

			LogicJSONArray collectedPlayersArray = jsonObject.GetJSONArray("collected_players");

			for (int i = 0; i < collectedPlayersArray.Size(); i++)
			{
				LogicJSONArray idArray = collectedPlayersArray.GetJSONArray(i);
				LogicLong id = new LogicLong(idArray.GetJSONNumber(0).GetIntValue(), idArray.GetJSONNumber(1).GetIntValue());

				if (m_collectedPlayers.IndexOf(id) == -1)
				{
					m_collectedPlayers.Add(id);
				}
			}
		}

		public override void Save(LogicJSONObject jsonObject)
		{
			LogicJSONObject baseObject = new LogicJSONObject();

			base.Save(baseObject);

			jsonObject.Put("base", baseObject);
			jsonObject.Put("diamond_reward", new LogicJSONNumber(m_diamondReward));
			jsonObject.Put("gift_count", new LogicJSONNumber(m_giftCount));

			LogicJSONArray collectedPlayersArray = new LogicJSONArray(m_collectedPlayers.Size());

			for (int i = 0; i < m_collectedPlayers.Size(); i++)
			{
				LogicJSONArray array = new LogicJSONArray(2);
				array.Add(new LogicJSONNumber(m_collectedPlayers[i].GetHigherInt()));
				array.Add(new LogicJSONNumber(m_collectedPlayers[i].GetLowerInt()));
				collectedPlayersArray.Add(array);
			}

			jsonObject.Put("collected_players", collectedPlayersArray);
		}
	}
}