using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	// behavior should only be added on the server
	// primary goal is to prevent OnFinalBuffStackLost being called every buff refresh cycle
	public class EliteBuffManager : CharacterBody.ItemBehavior
	{
		private const float refreshInterval = 0.5f;
		private const float refreshThreshold = 1.5f;
		private const float refreshDuration = 5f;

		private float refreshTimer = 0f;

		private readonly List<BuffIndex> buffsToRefresh = new List<BuffIndex>();



		public void Awake()
		{
			enabled = false;
		}

		public void FixedUpdate()
		{
			if (body && body.isElite)
			{
				refreshTimer -= Time.fixedDeltaTime;

				if (refreshTimer <= 0f)
				{
					RefreshEliteBuffs();
				}
			}
		}

		public void RefreshEliteBuffs()
		{
			refreshTimer = refreshInterval;

			Inventory inventory = body.inventory;
			if (inventory)
			{
				for (int i = body.timedBuffs.Count - 1; i >= 0; i--)
				{
					CharacterBody.TimedBuff timedBuff = body.timedBuffs[i];
					BuffIndex buffIndex = timedBuff.buffIndex;

					if (Catalog.aspectBuffIndexes.Contains(buffIndex))
					{
						if (timedBuff.timer <= refreshThreshold)
						{
							if (!buffsToRefresh.Contains(buffIndex))
							{
								buffsToRefresh.Add(buffIndex);
							}
						}
					}
				}

				foreach (BuffIndex buffIndex in buffsToRefresh)
				{
					if (Catalog.HasAspectItemOrEquipment(inventory, buffIndex))
					{
						body.AddTimedBuff(buffIndex, refreshDuration);
					}
				}
			}

			buffsToRefresh.Clear();
		}
	}
}
