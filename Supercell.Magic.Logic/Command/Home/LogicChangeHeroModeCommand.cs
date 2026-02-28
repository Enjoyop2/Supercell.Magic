using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicChangeHeroModeCommand : LogicCommand
	{
		private int m_gameObjectId;
		private int m_mode;

		public LogicChangeHeroModeCommand()
		{
			// LogicChangeHeroModeCommand.
		}

		public LogicChangeHeroModeCommand(int gameObjectId, int mode)
		{
			m_gameObjectId = gameObjectId;
			m_mode = mode;
		}

		public override void Decode(ByteStream stream)
		{
			m_gameObjectId = stream.ReadInt();
			m_mode = stream.ReadInt();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_gameObjectId);
			encoder.WriteInt(m_mode);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.CHANGE_HERO_MODE;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(m_gameObjectId);

			if (gameObject != null && gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
			{
				LogicBuilding building = (LogicBuilding)gameObject;
				LogicHeroBaseComponent heroBaseComponent = building.GetHeroBaseComponent();

				if (heroBaseComponent != null && (uint)m_mode <= 1)
				{
					return heroBaseComponent.SetHeroMode(m_mode) ? 0 : -2;
				}
			}

			return -1;
		}
	}
}