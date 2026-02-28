using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicSpeedUpClearingCommand : LogicCommand
	{
		private int m_gameObjectId;

		public LogicSpeedUpClearingCommand()
		{
			// LogicSpeedUpClearingCommand.
		}

		public LogicSpeedUpClearingCommand(int gameObjectId)
		{
			m_gameObjectId = gameObjectId;
		}

		public override void Decode(ByteStream stream)
		{
			m_gameObjectId = stream.ReadInt();
			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_gameObjectId);
			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.SPEED_UP_CLEARING;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(m_gameObjectId);

			if (gameObject != null && gameObject.GetGameObjectType() == LogicGameObjectType.OBSTACLE)
			{
				return ((LogicObstacle)gameObject).SpeedUpClearing() ? 0 : -2;
			}

			return -1;
		}
	}
}