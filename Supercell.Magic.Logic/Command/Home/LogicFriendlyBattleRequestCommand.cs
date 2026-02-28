using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Titan.DataStream;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicFriendlyBattleRequestCommand : LogicCommand
	{
		private int m_layoutId;

		private bool m_village2;
		private bool m_challenge;
		private bool m_unk;

		private string m_message;

		private LogicLong m_battleId;

		public override void Decode(ByteStream stream)
		{
			m_message = stream.ReadString(900000);
			m_layoutId = stream.ReadVInt();

			m_unk = stream.ReadBoolean();
			m_challenge = stream.ReadBoolean();
			m_village2 = stream.ReadBoolean();

			if (stream.ReadBoolean())
			{
				m_battleId = stream.ReadLong();
			}

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteString(m_message);
			encoder.WriteInt(m_layoutId);
			encoder.WriteBoolean(m_unk);
			encoder.WriteBoolean(m_challenge);
			encoder.WriteBoolean(m_village2);

			if (m_battleId != null)
			{
				encoder.WriteBoolean(true);
				encoder.WriteLong(m_battleId);
			}
			else
			{
				encoder.WriteBoolean(false);
			}

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.FRIENDLY_BATTLE_REQUEST;

		public override void Destruct()
		{
			base.Destruct();
		}

		public override int Execute(LogicLevel level)
		{
			if (m_layoutId == 7)
			{
				return -21;
			}

			if (m_village2)
			{
				if (m_layoutId != 0 && m_layoutId != 2 && m_layoutId != 3)
				{
					return -22;
				}
			}

			if (LogicDataTables.GetGlobals().UseVersusBattle())
			{
				int villageType = m_village2 ? 1 : 0;

				if (level.GetTownHallLevel(villageType) < level.GetRequiredTownHallLevelForLayout(m_layoutId, villageType))
				{
					return -3;
				}

				if (level.GetPlayerAvatar() == null)
				{
					return -10;
				}

				LogicArrayList<LogicGameObject> gameObjects = new LogicArrayList<LogicGameObject>(500);
				LogicGameObjectFilter filter = new LogicGameObjectFilter();

				filter.AddGameObjectType(LogicGameObjectType.BUILDING);
				filter.AddGameObjectType(LogicGameObjectType.TRAP);
				filter.AddGameObjectType(LogicGameObjectType.DECO);

				level.GetGameObjectManagerAt(m_village2 ? 1 : 0).GetGameObjects(gameObjects, filter);

				for (int i = 0; i < gameObjects.Size(); i++)
				{
					LogicVector2 position = gameObjects[i].GetPositionLayout(m_layoutId, false);

					if ((m_layoutId & 0xFFFFFFFE) != 6 && (position.m_x == -1 || position.m_y == -1))
					{
						return -5;
					}
				}

				gameObjects.Destruct();
				filter.Destruct();

				if (!m_village2)
				{
					LogicAvatar homeOwnerAvatar = level.GetHomeOwnerAvatar();

					if (homeOwnerAvatar == null || homeOwnerAvatar.IsChallengeStarted())
					{
						if (level.GetLayoutCooldown(m_layoutId) > 0)
						{
							return -7;
						}
					}
				}

				LogicBuilding allianceCastle = level.GetGameObjectManagerAt(0).GetAllianceCastle();

				if (allianceCastle != null)
				{
					LogicBunkerComponent bunkerComponent = allianceCastle.GetBunkerComponent();

					if (bunkerComponent == null || bunkerComponent.GetChallengeCooldownTime() != 0)
					{
						return -6;
					}

					LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

					if (!m_challenge)
					{
						if (playerAvatar.GetChallengeId() != null)
						{
							int challengeState = playerAvatar.GetChallengeState();

							if (challengeState != 2 && challengeState != 4)
							{
								Debugger.Warning("chal state: " + challengeState);
								return -8;
							}
						}
					}

					int friendlyCost = LogicDataTables.GetGlobals().GetFriendlyBattleCost(playerAvatar.GetTownHallLevel());

					if (friendlyCost != 0)
					{
						if (!playerAvatar.HasEnoughResources(LogicDataTables.GetGoldData(), friendlyCost, true, this, false))
						{
							return 0;
						}

						if (friendlyCost > 0)
						{
							playerAvatar.CommodityCountChangeHelper(0, LogicDataTables.GetGoldData(), friendlyCost);
						}
					}

					bunkerComponent.StartChallengeCooldownTime();

					bool warLayout = m_layoutId == 1 || m_layoutId == 4 || m_layoutId == 5;

					if (m_village2)
					{
						if (m_challenge)
						{
							playerAvatar.GetChangeListener().SendChallengeRequest(m_message, m_layoutId, warLayout, villageType);
						}
						else
						{
							playerAvatar.GetChangeListener().SendFriendlyBattleRequest(m_message, m_battleId, m_layoutId, warLayout, villageType);
						}
					}
					else
					{
						SaveChallengeLayout(level, warLayout);

						if (m_challenge)
						{
							playerAvatar.GetChangeListener().SendChallengeRequest(m_message, m_layoutId, warLayout, villageType);
						}
						else
						{
							playerAvatar.GetChangeListener().SendFriendlyBattleRequest(m_message, m_battleId, m_layoutId, warLayout, villageType);
						}

						playerAvatar.SetVariableByName("ChallengeStarted", 1);
					}

					return 0;
				}

				return -3;
			}

			return 2;
		}

		public void SaveChallengeLayout(LogicLevel level, bool warLayout)
		{
			LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

			if (playerAvatar != null)
			{
				playerAvatar.SetVariableByName("ChallengeLayoutIsWar", warLayout ? 1 : 0);
			}

			level.SaveLayout(m_layoutId, 6);
		}
	}
}