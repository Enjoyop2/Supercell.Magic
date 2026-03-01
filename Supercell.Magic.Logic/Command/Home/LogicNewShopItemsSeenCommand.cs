using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicNewShopItemsSeenCommand : LogicCommand
	{
		private DataType m_newShopItemsType;
		private int m_newShopItemsIndex;
		private int m_newShopItemsCount;


		public LogicNewShopItemsSeenCommand()
		{
			// LogicNewShopItemsSeenCommand.
		}

		public LogicNewShopItemsSeenCommand(int index, int type, int count)
		{
			m_newShopItemsIndex = index;
			m_newShopItemsType = (DataType)type;
			m_newShopItemsCount = count;
		}

		public override void Decode(ByteStream stream)
		{
			m_newShopItemsIndex = stream.ReadInt();
			m_newShopItemsType = (DataType)stream.ReadInt();
			m_newShopItemsCount = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_newShopItemsIndex);
			encoder.WriteInt((int)m_newShopItemsType);
			encoder.WriteInt(m_newShopItemsCount);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.NEW_SHOP_ITEMS_SEEN;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			if (m_newShopItemsType == DataType.BUILDING ||
				m_newShopItemsType == DataType.TRAP ||
				m_newShopItemsType == DataType.DECO)
			{
				if (level.SetUnlockedShopItemCount((LogicGameObjectData)LogicDataTables.GetTable(m_newShopItemsType).GetItemAt(m_newShopItemsIndex),
					m_newShopItemsIndex,
					m_newShopItemsCount,
					level.GetVillageType()))
				{
					return 0;
				}

				return -2;
			}

			return -1;
		}
	}
}