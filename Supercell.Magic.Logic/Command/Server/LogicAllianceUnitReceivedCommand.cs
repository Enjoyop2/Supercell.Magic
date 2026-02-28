using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Server
{
	public class LogicAllianceUnitReceivedCommand : LogicServerCommand
	{
		private LogicCombatItemData m_unitData;

		private int m_upgLevel;
		private string m_senderName;

		public void SetData(string senderName, LogicCombatItemData data, int upgLevel)
		{
			m_senderName = senderName;
			m_unitData = data;
			m_upgLevel = upgLevel;
		}

		public override void Destruct()
		{
			base.Destruct();
			m_unitData = null;
		}

		public override void Decode(ByteStream stream)
		{
			m_senderName = stream.ReadString(900000);
			m_unitData = (LogicCombatItemData)ByteStreamHelper.ReadDataReference(stream, stream.ReadInt() != 0 ? LogicDataType.SPELL : LogicDataType.CHARACTER);
			m_upgLevel = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteString(m_senderName);
			encoder.WriteInt(m_unitData.GetCombatItemType());
			ByteStreamHelper.WriteDataReference(encoder, m_unitData);
			encoder.WriteInt(m_upgLevel);

			base.Encode(encoder);
		}

		public override int Execute(LogicLevel level)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				if (m_unitData != null)
				{
					playerAvatar.AddAllianceUnit(m_unitData, m_upgLevel);
					playerAvatar.GetChangeListener().AllianceUnitAdded(m_unitData, m_upgLevel);
					level.GetGameListener().UnitReceivedFromAlliance(m_senderName, m_unitData, m_upgLevel);

					if (level.GetState() == 1 || level.GetState() == 3)
					{
						level.GetComponentManagerAt(0).AddAvatarAllianceUnitsToCastle();
					}

					return 0;
				}
			}

			return -1;
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.ALLIANCE_UNIT_RECEIVED;
	}
}