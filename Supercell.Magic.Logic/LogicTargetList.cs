using System;

using Supercell.Magic.Logic.Data;
using Supercell.Magic.Logic.GameObject;
using Supercell.Magic.Logic.GameObject.Component;

namespace Supercell.Magic.Logic
{
	public class LogicTargetList
	{
		private int m_charVersusCharRandomDistance;
		private int m_targetListSize;

		private readonly LogicGameObject[] m_targets;
		private readonly int[] m_targetCosts;

		public LogicTargetList()
		{
			m_targets = new LogicGameObject[10];
			m_targetCosts = new int[10];

			Clear();

			m_charVersusCharRandomDistance = LogicDataTables.GetGlobals().GetCharVersusCharRandomDistanceLimit();
			m_targetListSize = LogicDataTables.GetGlobals().GetTargetListSize();
		}

		public void Destruct()
		{
			Clear();

			m_charVersusCharRandomDistance = 0;
			m_targetListSize = 3;
		}

		public void Clear()
		{
			for (int i = 0; i < 10; i++)
			{
				m_targets[i] = null;
				m_targetCosts[i] = 0x7FFFFFFF;
			}
		}

		public LogicGameObject EvaluateTargets(LogicMovementComponent component)
		{
			if (component != null && !component.IsFlying() && m_targetListSize > 1)
			{
				bool fullChar = true;

				int count = 0;

				int minCost = 0x7FFFFFFF;
				int maxCost = 0;

				LogicGameObject minCostTarget = null;

				for (int i = 0; i < m_targetListSize; i++)
				{
					LogicGameObject target = m_targets[i];

					if (target != null)
					{
						LogicCombatComponent combatComponent = component.GetParent().GetCombatComponent();

						if (combatComponent != null && combatComponent.IsInRange(target))
						{
							return target;
						}

						int targetCost = component.EvaluateTargetCost(target);

						if (target.GetMovementComponent() == null)
						{
							fullChar = false;
						}

						if (targetCost > maxCost)
						{
							maxCost = targetCost;
						}

						if (targetCost < minCost)
						{
							minCost = targetCost;
							minCostTarget = target;
						}

						++count;
					}
				}

				if (count >= 2 && fullChar && minCost != 0x7FFFFFFF && maxCost - minCost < m_charVersusCharRandomDistance)
				{
					return m_targets[component.GetParent().GetGlobalID() % count];
				}

				return minCostTarget;
			}

			return m_targets[0];
		}

		public void AddCandidate(LogicGameObject target, int cost)
		{
			int index = -1;

			for (int i = 0; i < m_targetListSize; i++)
			{
				if (m_targetCosts[i] > cost)
				{
					index = i;
					break;
				}
			}

			if (index != -1)
			{
				Array.Copy(m_targets, index, m_targets, index + 1, m_targetListSize - index);
				Array.Copy(m_targetCosts, index, m_targetCosts, index + 1, m_targetListSize - index);

				m_targets[index] = target;
				m_targetCosts[index] = cost;
			}
		}
	}
}