using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicSetLayoutStateCommand : LogicCommand
	{
		private int m_layoutId;
		private int m_state;

		private bool m_updateListener;

		public override void Decode(ByteStream stream)
		{
			m_layoutId = stream.ReadInt();
			m_state = stream.ReadInt();
			m_updateListener = stream.ReadBoolean();

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_layoutId);
			encoder.WriteInt(m_state);
			encoder.WriteBoolean(m_updateListener);

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.SET_LAYOUT_STATE;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			if (m_layoutId != 6)
			{
				if (m_layoutId != 7)
				{
					if (m_state == 0)
					{
						LogicGameObjectFilter filter = new LogicGameObjectFilter();
						LogicArrayList<LogicGameObject> gameObjects = new LogicArrayList<LogicGameObject>(500);

						filter.AddGameObjectType(LogicGameObjectType.BUILDING);
						filter.AddGameObjectType(LogicGameObjectType.TRAP);
						filter.AddGameObjectType(LogicGameObjectType.DECO);

						level.GetGameObjectManager().GetGameObjects(gameObjects, filter);

						for (int i = 0; i < gameObjects.Size(); i++)
						{
							gameObjects[i].SetPositionLayoutXY(-1, -1, m_layoutId, true);
						}

						filter.Destruct();
					}

					level.SetLayoutState(m_layoutId, level.GetVillageType(), m_state);

					if (m_updateListener)
					{
					}

					return 0;
				}

				return -11;
			}

			return -10;
		}
	}
}