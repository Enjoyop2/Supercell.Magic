using System;
using System.Collections.Generic;

using Supercell.Magic.Logic.Command;
using Supercell.Magic.Logic.Message.Home;
using Supercell.Magic.Logic.Time;
using Supercell.Magic.Servers.Core.Network;
using Supercell.Magic.Servers.Core.Network.Message;
using Supercell.Magic.Servers.Core.Network.Message.Account;
using Supercell.Magic.Servers.Core.Network.Message.Session;

using Supercell.Magic.Servers.Game.Session;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Message;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Servers.Game.Logic.Live
{
	public class LiveReplay
	{
		public const int SLOT_CAPACITY = 100;

		private bool m_initialized;
		private bool m_ended;
		private bool m_end;

		private readonly LogicLong m_id;
		private readonly LogicLong m_allianceId;
		private readonly LogicLong m_allianceStreamId;
		private readonly GameSession m_attackerSession;
		private readonly Dictionary<long, LiveReplaySpectatorEntry>[] m_spectatorList;
		private readonly LogicArrayList<LogicCommand> m_commands;

		private byte[] m_streamData;
		private int m_clientSubTick;
		private int m_serverSubTick;

		public LiveReplay(LogicLong id, LogicLong allianceId, LogicLong allianceStreamId, GameSession attackerSession)
		{
			m_id = id;
			m_allianceId = allianceId;
			m_allianceStreamId = allianceStreamId;
			m_attackerSession = attackerSession;
			m_spectatorList = new Dictionary<long, LiveReplaySpectatorEntry>[2];
			m_spectatorList[0] = new Dictionary<long, LiveReplaySpectatorEntry>();
			m_spectatorList[1] = new Dictionary<long, LiveReplaySpectatorEntry>();
			m_commands = new LogicArrayList<LogicCommand>();
		}

		public LogicLong GetId()
			=> m_id;

		public bool IsInit()
			=> m_initialized;

		public bool IsEnded()
			=> m_ended;

		public void Init(byte[] streamData)
		{
			if (m_initialized)
				throw new Exception("LiveReplay.init: live already initialized!");

			m_streamData = streamData;
			m_initialized = true;
		}

		public void SetClientUpdate(int clientSubTick, LogicArrayList<LogicCommand> commands)
		{
			m_clientSubTick = clientSubTick;

			if (commands != null)
			{
				m_commands.AddAll(commands);
			}
		}

		public void SetEnd()
		{
			m_end = true;
		}

		public void Update(int ms)
		{
			if (m_initialized && !m_ended)
			{
				int totalSubTick = LogicTime.GetMSInTicks(ms);

				if (m_clientSubTick < m_serverSubTick + totalSubTick)
				{
					if (!m_end)
						return;

					totalSubTick = m_clientSubTick - m_serverSubTick;
					m_ended = true;
				}

				for (int i = 0; i < 2; i++)
				{
					Dictionary<long, LiveReplaySpectatorEntry> spectators = m_spectatorList[i];

					if (spectators.Count >= 1)
					{
						LiveReplayDataMessage liveReplayDataMessage = new LiveReplayDataMessage();

						liveReplayDataMessage.SetServerSubTick(m_serverSubTick + totalSubTick);
						liveReplayDataMessage.SetCommands(GetCommands(m_serverSubTick, m_serverSubTick + totalSubTick));

						if (i == 0)
						{
							liveReplayDataMessage.SetViewerCount(m_spectatorList[0].Count);
							liveReplayDataMessage.SetEnemyViewerCount(m_spectatorList[1].Count);
						}
						else
						{
							liveReplayDataMessage.SetViewerCount(m_spectatorList[1].Count);
							liveReplayDataMessage.SetEnemyViewerCount(m_spectatorList[0].Count);
						}

						foreach (LiveReplaySpectatorEntry entry in spectators.Values)
						{
							entry.SendPiranhaMessageToProxy(liveReplayDataMessage);
						}

						if (m_ended)
						{
							foreach (LiveReplaySpectatorEntry entry in spectators.Values)
							{
								entry.SendPiranhaMessageToProxy(new LiveReplayEndMessage());
							}
						}
					}
				}

				m_serverSubTick += totalSubTick;
			}
		}

		public bool IsFull()
			=> m_spectatorList[0].Count +
				   m_spectatorList[1].Count >= LiveReplay.SLOT_CAPACITY;

		public bool ContainsSession(long sessionId, int slot)
			=> m_spectatorList[slot].ContainsKey(sessionId);

		public void AddSpectator(long sessionId, int slot)
		{
			LiveReplaySpectatorEntry liveReplaySpectatorEntry = new LiveReplaySpectatorEntry(sessionId);
			LiveReplayHeaderMessage liveReplayHeaderMessage = new LiveReplayHeaderMessage();

			int serverSubTick = m_serverSubTick;

			liveReplayHeaderMessage.SetCompressedStreamHeaderJson(m_streamData);
			liveReplayHeaderMessage.SetCommands(GetCommands(0, serverSubTick));
			liveReplayHeaderMessage.SetServerSubTick(serverSubTick);
			liveReplaySpectatorEntry.SendPiranhaMessageToProxy(liveReplayHeaderMessage);

			m_spectatorList[slot].Add(sessionId, liveReplaySpectatorEntry);
			SendAttackSpectatorCountMessage();

			if (m_allianceId != null)
				SendSpectatorCountToStreamService();
		}

		public void RemoveSpectator(long sessionId, int slot)
		{
			if (m_spectatorList[slot].Remove(sessionId))
			{
				SendAttackSpectatorCountMessage();

				if (m_allianceId != null)
					SendSpectatorCountToStreamService();
			}
		}

		private void SendAttackSpectatorCountMessage()
		{
			if (!m_attackerSession.IsDestructed())
			{
				AttackSpectatorCountMessage attackSpectatorCountMessage = new AttackSpectatorCountMessage();

				attackSpectatorCountMessage.SetViewerCount(m_spectatorList[0].Count);
				attackSpectatorCountMessage.SetEnemyViewerCount(m_spectatorList[1].Count);

				m_attackerSession.SendPiranhaMessage(attackSpectatorCountMessage, 1);
			}
		}

		private void SendSpectatorCountToStreamService()
		{
			ServerMessageManager.SendMessage(new AllianceChallengeSpectatorCountMessage
			{
				AccountId = m_allianceId,
				StreamId = m_allianceStreamId,
				Count = m_spectatorList[0].Count +
						m_spectatorList[1].Count
			}, 11);
		}

		private LogicArrayList<LogicCommand> GetCommands(int minSubTick, int maxSubTick)
		{
			LogicArrayList<LogicCommand> commands = new LogicArrayList<LogicCommand>();

			for (int i = 0; i < m_commands.Size(); i++)
			{
				LogicCommand command = m_commands[i];

				if (command.GetExecuteSubTick() >= minSubTick && command.GetExecuteSubTick() < maxSubTick)
					commands.Add(command);
			}

			return commands;
		}
	}

	public class LiveReplaySpectatorEntry
	{
		public long SessionId
		{
			get;
		}

		public LiveReplaySpectatorEntry(long sessionId)
		{
			SessionId = sessionId;
		}

		public void SendPiranhaMessageToProxy(PiranhaMessage piranhaMessage)
		{
			if (piranhaMessage.GetEncodingLength() == 0)
				piranhaMessage.Encode();

			ServerMessageManager.SendMessage(new ForwardLogicMessage
			{
				SessionId = SessionId,
				MessageType = piranhaMessage.GetMessageType(),
				MessageVersion = (short)piranhaMessage.GetMessageVersion(),
				MessageLength = piranhaMessage.GetEncodingLength(),
				MessageBytes = piranhaMessage.GetMessageBytes()
			}, ServerManager.GetProxySocket(SessionId));
		}
	}
}