using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicSaveUsedArmyCommand : LogicServerCommand
	{
		private readonly LogicArrayList<LogicDataSlot> m_unitCount;
		private readonly LogicArrayList<LogicDataSlot> m_spellCount;

		public LogicSaveUsedArmyCommand()
		{
			m_unitCount = new LogicArrayList<LogicDataSlot>();
			m_spellCount = new LogicArrayList<LogicDataSlot>();
		}

		public override void Destruct()
		{
			base.Destruct();
		}

		public override void Decode(ByteStream stream)
		{
			base.Decode(stream);

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);
				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_unitCount.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicSaveUsedArmyCommand::decode - troop data is NULL");
				}
			}

			for (int i = 0, size = stream.ReadInt(); i < size; i++)
			{
				LogicDataSlot slot = new LogicDataSlot(null, 0);
				slot.Decode(stream);

				if (slot.GetData() != null)
				{
					m_spellCount.Add(slot);
				}
				else
				{
					slot.Destruct();
					slot = null;

					Debugger.Error("LogicSaveUsedArmyCommand::decode - spell data is NULL");
				}
			}
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			base.Encode(encoder);

			encoder.WriteInt(m_unitCount.Size());

			for (int i = 0; i < m_unitCount.Size(); i++)
			{
				m_unitCount[i].Encode(encoder);
			}

			encoder.WriteInt(m_spellCount.Size());

			for (int i = 0; i < m_spellCount.Size(); i++)
			{
				m_spellCount[i].Encode(encoder);
			}
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				playerAvatar.SetLastUsedArmy(m_unitCount, m_spellCount);
				return 0;
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.SAVE_USED_ARMY;

		public void AddUnit(LogicCharacterData data, int count)
		{
			int index = -1;

			for (int i = 0; i < m_unitCount.Size(); i++)
			{
				if (m_unitCount[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_unitCount[index].SetCount(m_unitCount[index].GetCount() + count);
			}
			else
			{
				m_unitCount.Add(new LogicDataSlot(data, count));
			}
		}

		public void AddSpell(LogicSpellData data, int count)
		{
			int index = -1;

			for (int i = 0; i < m_spellCount.Size(); i++)
			{
				if (m_spellCount[i].GetData() == data)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				m_spellCount[index].SetCount(m_spellCount[index].GetCount() + count);
			}
			else
			{
				m_spellCount.Add(new LogicDataSlot(data, count));
			}
		}
	}
}