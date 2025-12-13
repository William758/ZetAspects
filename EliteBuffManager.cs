using RoR2;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace TPDespair.ZetAspects
{
	public interface IAspectProvider
	{
		void OnBodyDiscard(NetworkInstanceId netId)
		{

		}

		float StackCount(CharacterBody body)
		{
			return 0f;
		}

		bool HasAspect(CharacterBody body, BuffIndex buffIndex)
		{
			IEnumerable<BuffIndex> aspects = Aspects(body);

			if (aspects == null) return false;

			return aspects.Contains(buffIndex);
		}

		IEnumerable<BuffIndex> Aspects(CharacterBody body);
	}



	// behavior should only be added on the server
	// primary goal is to prevent OnFinalBuffStackLost being called every buff refresh cycle
	public class EliteBuffManager : CharacterBody.ItemBehavior
	{
		private const float refreshInterval = 0.5f;
		private const float refreshThreshold = 1.5f;
		private const float refreshDuration = 5f;

		private float refreshTimer = 0f;

		private readonly List<BuffIndex> buffsToRefresh = new List<BuffIndex>();
		private readonly List<BuffIndex> promotedBuffs = new List<BuffIndex>();

		public void AddBuffToRefresh(BuffIndex buffIndex)
		{
			if (!buffsToRefresh.Contains(buffIndex))
			{
				buffsToRefresh.Add(buffIndex);
			}
		}



		private void Awake()
		{
			enabled = false;
		}

		private void FixedUpdate()
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

		internal void RefreshEliteBuffs()
		{
			refreshTimer = refreshInterval;

			Inventory inventory = body.inventory;
			if (inventory)
			{
				// aspectsWithPromoters should only contain valid aspects
				foreach (AspectDef aspectDef in Catalog.aspectsWithPromoters)
				{
					if (body.HasBuff(aspectDef.buffDef))
					{
						IEnumerable<BuffIndex> susBuffs = aspectDef.Promoter(body);
						if (susBuffs != null && susBuffs.Any())
						{
							foreach (BuffIndex buffIndex in susBuffs)
							{
								if (!promotedBuffs.Contains(buffIndex))
								{
									promotedBuffs.Add(buffIndex);
								}
							}
						}
					}
				}

				for (int i = body.timedBuffs.Count - 1; i >= 0; i--)
				{
					CharacterBody.TimedBuff timedBuff = body.timedBuffs[i];
					BuffIndex buffIndex = timedBuff.buffIndex;

					if (timedBuff.timer <= refreshThreshold)
					{
						if (Monitored.Contains(buffIndex))
						{
							Action<EliteBuffManager, BuffIndex> action = MonitorTriggered;
							if (action != null)
							{
								action(this, buffIndex);
							}
						}

						if (Catalog.aspectBuffIndexes.Contains(buffIndex) || promotedBuffs.Contains(buffIndex))
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
					if (Catalog.HasAspectItemOrEquipment(inventory, buffIndex) || Catalog.HasAspectFromProviders(body, buffIndex) || promotedBuffs.Contains(buffIndex))
					{
						AspectDef aspectDef = Catalog.GetAspectDef(buffIndex);
						if (aspectDef != null)
						{
							if (aspectDef.OnRefresh != null)
							{
								aspectDef.OnRefresh(body);
							}
						}

						body.AddTimedBuff(buffIndex, refreshDuration);
					}
				}
			}

			buffsToRefresh.Clear();
			promotedBuffs.Clear();
		}



		internal static HashSet<BuffIndex> Monitored = new HashSet<BuffIndex>();

		public static event Action<EliteBuffManager, BuffIndex> MonitorTriggered;

		public static void MonitorBuff(BuffIndex buffIndex)
		{
			if (!Monitored.Contains(buffIndex))
			{
				Monitored.Add(buffIndex);
			}
		}



		internal static List<IAspectProvider> Providers = new List<IAspectProvider>();

		public static void AddProvider(IAspectProvider provider)
		{
			Providers.Add(provider);
		}



		public static void ApplyBuffs(CharacterBody body, IEnumerable<BuffIndex> buffs)
		{
			if (buffs == null) return;

			foreach (BuffIndex buffIndex in buffs)
			{
				ApplyBuff(body, buffIndex);
			}
		}

		public static void ApplyBuff(CharacterBody body, BuffIndex buffIndex)
		{
			if (buffIndex != BuffIndex.None && !body.HasBuff(buffIndex))
			{
				body.AddTimedBuff(buffIndex, BuffHooks.BuffCycleDuration);
			}
		}



		public static void CheckSustains(CharacterBody body, IEnumerable<BuffIndex> buffs)
		{
			if (buffs == null) return;

			foreach (BuffIndex buffIndex in buffs)
			{
				CheckSustain(body, buffIndex);
			}
		}

		public static void CheckSustain(CharacterBody body, BuffIndex buffIndex)
		{
			if (buffIndex != BuffIndex.None && !HasSustain(body, buffIndex))
			{
				body.ClearTimedBuffs(buffIndex);
			}
		}

		private static bool HasSustain(CharacterBody body, BuffIndex buffIndex)
		{
			Inventory inventory = body.inventory;
			if (inventory)
			{
				if (inventory.GetItemCountEffective(RoR2Content.Items.HeadHunter) > 0) return true;
				if (Catalog.HasAspectItemOrEquipment(inventory, buffIndex)) return true;
			}

			return Catalog.HasAspectFromProviders(body, buffIndex);
		}
	}
}
