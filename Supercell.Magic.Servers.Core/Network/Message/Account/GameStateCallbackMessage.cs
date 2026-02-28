using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Command;
using Supercell.Magic.Logic.Command.Server;

using Supercell.Magic.Servers.Core.Network.Message.Session.Change;

using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Core.Network.Message.Account
{
	public class GameStateCallbackMessage : ServerAccountMessage
	{
		public LogicClientAvatar LogicClientAvatar
		{
			get; set;
		}
		public LogicArrayList<AvatarChange> AvatarChanges
		{
			get; set;
		}
		public LogicArrayList<LogicServerCommand> ExecutedServerCommands
		{
			get; set;
		}

		public int SaveTime
		{
			get; set;
		}
		public int RemainingShieldTime
		{
			get; set;
		}
		public int RemainingGuardTime
		{
			get; set;
		}
		public int NextPersonalBreakTime
		{
			get; set;
		}
		public byte[] HomeJSON
		{
			get; set;
		}

		public long SessionId
		{
			get; set;
		}

		public override void Encode(ByteStream stream)
		{
			stream.WriteLongLong(SessionId);
			stream.WriteVInt(AvatarChanges.Size());

			for (int i = 0; i < AvatarChanges.Size(); i++)
			{
				AvatarChangeFactory.Encode(stream, AvatarChanges[i]);
			}

			LogicClientAvatar.Encode(stream);

			if (HomeJSON != null)
			{
				stream.WriteBoolean(true);
				stream.WriteVInt(ExecutedServerCommands.Size());

				for (int i = 0; i < ExecutedServerCommands.Size(); i++)
				{
					LogicCommandManager.EncodeCommand(stream, ExecutedServerCommands[i]);
				}

				stream.WriteVInt(SaveTime);
				stream.WriteVInt(RemainingShieldTime);
				stream.WriteVInt(RemainingGuardTime);
				stream.WriteVInt(NextPersonalBreakTime);
				stream.WriteBytes(HomeJSON, HomeJSON.Length);
			}
			else
			{
				stream.WriteBoolean(false);
			}
		}

		public override void Decode(ByteStream stream)
		{
			SessionId = stream.ReadLongLong();
			AvatarChanges = new LogicArrayList<AvatarChange>();

			for (int i = stream.ReadVInt(); i > 0; i--)
			{
				AvatarChanges.Add(AvatarChangeFactory.Decode(stream));
			}

			LogicClientAvatar = new LogicClientAvatar();
			LogicClientAvatar.Decode(stream);

			if (stream.ReadBoolean())
			{
				ExecutedServerCommands = new LogicArrayList<LogicServerCommand>();

				for (int i = stream.ReadVInt(); i > 0; i--)
				{
					ExecutedServerCommands.Add((LogicServerCommand)LogicCommandManager.DecodeCommand(stream));
				}

				SaveTime = stream.ReadVInt();
				RemainingShieldTime = stream.ReadVInt();
				RemainingGuardTime = stream.ReadVInt();
				NextPersonalBreakTime = stream.ReadVInt();
				HomeJSON = stream.ReadBytes(stream.ReadBytesLength(), 900000);
			}
		}

		public override ServerMessageType GetMessageType()
			=> ServerMessageType.GAME_STATE_CALLBACK;
	}
}