using Supercell.Magic.Logic.Avatar;
using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.Helper;
using Supercell.Magic.Logic.Level;
using Supercell.Magic.Logic.Util;
using Supercell.Magic.Titan.DataStream;

namespace Supercell.Magic.Logic.Command.Home
{
	public sealed class LogicBuyResourcesCommand : LogicCommand
	{
		private LogicCommand m_command;
		private LogicResourceData m_resourceData;
		private LogicResourceData m_resource2Data;
		private int m_resourceCount;
		private int m_resource2Count;

		public LogicBuyResourcesCommand()
		{
			// LogicBuyResourcesCommand.
		}

		public LogicBuyResourcesCommand(LogicResourceData data, int resourceCount, LogicResourceData resource2Data, int resource2Count, LogicCommand resourceCommand)
		{
			m_resourceData = data;
			m_resource2Data = resource2Data;
			m_command = resourceCommand;
			m_resourceCount = resourceCount;
			m_resource2Count = resource2Count;
		}

		public override void Decode(ByteStream stream)
		{
			m_resourceCount = stream.ReadInt();
			m_resourceData = (LogicResourceData)ByteStreamHelper.ReadDataReference(stream, LogicDataType.RESOURCE);
			m_resource2Count = stream.ReadInt();

			if (m_resource2Count > 0)
			{
				m_resource2Data = (LogicResourceData)ByteStreamHelper.ReadDataReference(stream, LogicDataType.RESOURCE);
			}

			if (stream.ReadBoolean())
			{
				m_command = LogicCommandManager.DecodeCommand(stream);
			}

			base.Decode(stream);
		}

		public override void Encode(ChecksumEncoder encoder)
		{
			encoder.WriteInt(m_resourceCount);
			ByteStreamHelper.WriteDataReference(encoder, m_resourceData);
			encoder.WriteInt(m_resource2Count);

			if (m_resource2Count > 0)
			{
				ByteStreamHelper.WriteDataReference(encoder, m_resource2Data);
			}

			if (m_command != null)
			{
				encoder.WriteBoolean(true);
				LogicCommandManager.EncodeCommand(encoder, m_command);
			}
			else
			{
				encoder.WriteBoolean(false);
			}

			base.Encode(encoder);
		}

		public override LogicCommandType GetCommandType()
			=> LogicCommandType.BUY_RESOURCES;

		public override void Destruct()
		{
			base.Destruct();

			if (m_command != null)
			{
				m_command.Destruct();
				m_command = null;
			}

			m_resourceData = null;
			m_resource2Data = null;
		}

		public override int Execute(LogicLevel level)
		{
			if (m_resourceData != null && m_resourceCount > 0 && !m_resourceData.IsPremiumCurrency())
			{
				LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

				if (m_resource2Data != null && m_resource2Count > 0)
				{
					if (playerAvatar.GetUnusedResourceCap(m_resourceData) >= m_resourceCount &&
						playerAvatar.GetUnusedResourceCap(m_resource2Data) >= m_resource2Count)
					{
						int resourceCost = LogicGamePlayUtil.GetResourceDiamondCost(m_resourceCount, m_resourceData);
						int resourceCost2 = LogicGamePlayUtil.GetResourceDiamondCost(m_resource2Count, m_resource2Data);

						if (playerAvatar.HasEnoughDiamonds(resourceCost + resourceCost2, true, level))
						{
							playerAvatar.UseDiamonds(resourceCost + resourceCost2);
							playerAvatar.CommodityCountChangeHelper(0, m_resourceData, m_resourceCount);
							playerAvatar.CommodityCountChangeHelper(0, m_resource2Data, m_resource2Count);
							playerAvatar.GetChangeListener().DiamondPurchaseMade(5, m_resource2Data.GetGlobalID(), m_resource2Count, resourceCost + resourceCost2,
																				 level.GetVillageType());

							if (m_command != null)
							{
								int cmdType = (int)m_command.GetCommandType();

								if (cmdType < 1000)
								{
									if (cmdType >= 500 && cmdType < 700)
									{
										m_command.Execute(level);
									}
								}
							}

							return 0;
						}
					}
				}
				else
				{
					if (playerAvatar.GetUnusedResourceCap(m_resourceData) >= m_resourceCount)
					{
						int resourceCost = LogicGamePlayUtil.GetResourceDiamondCost(m_resourceCount, m_resourceData);

						if (playerAvatar.HasEnoughDiamonds(resourceCost, true, level))
						{
							playerAvatar.UseDiamonds(resourceCost);
							playerAvatar.CommodityCountChangeHelper(0, m_resourceData, m_resourceCount);
							playerAvatar.GetChangeListener().DiamondPurchaseMade(5, m_resourceData.GetGlobalID(), m_resourceCount, resourceCost, level.GetVillageType());

							if (m_command != null)
							{
								int cmdType = (int)m_command.GetCommandType();

								if (cmdType < 1000)
								{
									if (cmdType >= 500 && cmdType < 700)
									{
										m_command.Execute(level);
									}
								}
							}

							return 0;
						}
					}
				}
			}

			return -1;
		}
	}
}