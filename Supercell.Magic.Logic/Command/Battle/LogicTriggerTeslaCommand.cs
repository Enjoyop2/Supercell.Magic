using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Listener;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Logic.Command.Battle
{
	public sealed class LogicTriggerTeslaCommand : LogicCommand
	{
		private int m_id;

		private LogicJSONObject m_json;
		private LogicGameObjectData m_data;

		public LogicTriggerTeslaCommand()
		{
			m_json = new LogicJSONObject();
		}

		public LogicTriggerTeslaCommand(LogicGameObject gameObject) : this()
		{
			m_id = gameObject.GetGlobalID();
			m_data = gameObject.GetData();

			gameObject.Save(m_json, 0);
		}

		public override void Decode(ByteStream stream)
		{
			m_id = stream.ReadVInt();
			m_data = (LogicGameObjectData)LogicDataTables.GetDataById(stream.ReadVInt());
			m_json = LogicJSONParser.ParseObject(stream.ReadString(900000) ?? string.Empty);

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteVInt(m_id);
			encoder.WriteVInt(m_data.GetGlobalID());
			encoder.WriteString(LogicJSONParser.CreateJSONString(m_json));

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.TRIGGER_TESLA;

		public override void Destruct()
		{
			base.Destruct();

			m_data = null;
			m_json = null;
		}

		public override int Execute(LogicLevel level)
		{
			if (level != null)
			{
				LogicGameObject gameObject;

				if (level.GetState() == 5)
				{
					gameObject = LogicGameObjectFactory.CreateGameObject(m_data, level, level.GetVillageType());
					gameObject.Load(m_json);
					level.GetGameObjectManager().AddGameObject(gameObject, -1);
				}
				else
				{
					gameObject = level.GetGameObjectManager().GetGameObjectByID(m_id);
				}

				if (gameObject != null && gameObject.GetGameObjectType() == LogicGameObjectType.BUILDING)
				{
					if (gameObject.IsHidden())
					{
						LogicBuilding building = (LogicBuilding)gameObject;
						LogicGameObjectManagerListener listener = level.GetGameObjectManager().GetListener();

						listener.AddGameObject(gameObject);

						gameObject.LoadingFinished();
						gameObject.GetListener().RefreshState();

						building.Trigger();

						return 0;
					}

					Debugger.Warning("PGO building not hidden");
					return -3;
				}

				Debugger.Warning("PGO == NULL in LogicTriggerTeslaCommand");
				return -2;
			}

			return -1;
		}

		public override void LoadFromJSON(LogicJSONObject jsonRoot)
		{
			LogicJSONObject baseObject = jsonRoot.GetJSONObject("base");

			if (baseObject == null)
			{
				Debugger.Error("Replay LogicTriggerTeslaCommand load failed! Base missing!");
			}

			base.LoadFromJSON(baseObject);

			m_id = jsonRoot.GetJSONNumber("id").GetIntValue();
			m_data = (LogicGameObjectData)LogicDataTables.GetDataById(jsonRoot.GetJSONNumber("dataid").GetIntValue());
			m_json = jsonRoot.GetJSONObject("objs");
		}

		public override LogicJSONObject GetJSONForReplay()
		{
			LogicJSONObject jsonObject = new LogicJSONObject();

			jsonObject.Put("base", base.GetJSONForReplay());
			jsonObject.Put("id", new LogicJSONNumber(m_id));
			jsonObject.Put("dataid", new LogicJSONNumber(m_data.GetGlobalID()));
			jsonObject.Put("objs", m_json);

			return jsonObject;
		}
	}
}