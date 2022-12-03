using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;

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
					if (Catalog.HasAspectItemOrEquipment(inventory, buffIndex) || BlightedStateManager.HasAspectFromBlighted(body, buffIndex))
					{
						body.AddTimedBuff(buffIndex, refreshDuration);
					}
				}
			}

			buffsToRefresh.Clear();
		}
	}



	public static class BlightedStateManager
	{
		public static Dictionary<CharacterBody, BuffIndex[]> Entries = new Dictionary<CharacterBody, BuffIndex[]>();
		public static List<NetworkInstanceId> NetIds = new List<NetworkInstanceId>();



		internal static void Activated(CharacterBody body, BuffIndex firstBuff, BuffIndex secondBuff)
		{
			//Logger.Warn("ZetAspects - BlightedStateManager - SetElites : " + body.name + " - " + body.netId);
			//Logger.Warn("Setting EliteBuffs : [" + BuffCatalog.GetBuffDef(firstBuff).name + "] - [" + BuffCatalog.GetBuffDef(secondBuff).name + "]");

			if (!Entries.ContainsKey(body)) CreateEntry(body);

			BuffIndex oldFirstBuff = Entries[body][0];
			BuffIndex oldSecondBuff = Entries[body][1];

			Entries[body][0] = firstBuff;
			Entries[body][1] = secondBuff;

			if (NetworkServer.active)
			{
				bool clearBuffs = true;

				Inventory inventory = body.inventory;
				if (inventory && inventory.GetItemCount(RoR2Content.Items.HeadHunter) > 0) clearBuffs = false;
				bool destroyed = EffectHooks.DestroyedBodies.ContainsKey(body.netId);
				if (destroyed) clearBuffs = false;

				if (clearBuffs)
				{
					if (oldFirstBuff != BuffIndex.None) body.ClearTimedBuffs(oldFirstBuff);
					if (oldSecondBuff != BuffIndex.None) body.ClearTimedBuffs(oldSecondBuff);
				}

				if (!destroyed) ApplyBlightedAspectBuffs(body);
			}
		}

		internal static void Deactivated(CharacterBody body, bool destroyEntry = false)
		{
			if (Entries.ContainsKey(body))
			{
				BuffIndex firstBuff = Entries[body][0];
				BuffIndex secondBuff = Entries[body][1];

				Entries[body][0] = BuffIndex.None;
				Entries[body][1] = BuffIndex.None;

				if (NetworkServer.active)
				{
					bool clearBuffs = true;

					Inventory inventory = body.inventory;
					if (inventory && inventory.GetItemCount(RoR2Content.Items.HeadHunter) > 0) clearBuffs = false;
					if (EffectHooks.DestroyedBodies.ContainsKey(body.netId)) clearBuffs = false;

					if (clearBuffs)
					{
						if (firstBuff != BuffIndex.None) body.ClearTimedBuffs(firstBuff);
						if (secondBuff != BuffIndex.None) body.ClearTimedBuffs(secondBuff);
					}
				}

				if (destroyEntry)
				{
					Entries.Remove(body);
					NetIds.Remove(body.netId);
				}
			}
		}

		

		private static void CreateEntry(CharacterBody body)
		{
			BuffIndex[] buffs = new BuffIndex[] { BuffIndex.None, BuffIndex.None };
			Entries.Add(body, buffs);
			NetIds.Add(body.netId);
		}

		internal static void DestroyEntry(NetworkInstanceId netId)
		{
			if (NetIds.Contains(netId))
			{
				foreach (CharacterBody body in Entries.Keys)
				{
					if (body.netId == netId)
					{
						Deactivated(body, true);

						return;
					}
				}
			}
		}



		public static bool HasAspectFromBlighted(CharacterBody body, BuffDef buffDef)
		{
			BuffIndex buffIndex = buffDef.buffIndex;

			return HasAspectFromBlighted(body, buffIndex);
		}

		public static bool HasAspectFromBlighted(CharacterBody body, BuffIndex buffIndex)
		{
			if (Entries.ContainsKey(body))
			{
				if (buffIndex == BuffIndex.None) return false;
				if (Entries[body][0] == buffIndex) return true;
				if (Entries[body][1] == buffIndex) return true;
			}

			return false;
		}

		internal static void ApplyBlightedAspectBuffs(CharacterBody body)
		{
			if (Entries.ContainsKey(body))
			{
				BuffIndex targetBuff;

				targetBuff = Entries[body][0];
				if (targetBuff != BuffIndex.None && !body.HasBuff(targetBuff))
				{
					body.AddTimedBuff(targetBuff, EffectHooks.BuffCycleDuration);
				}
				targetBuff = Entries[body][1];
				if (targetBuff != BuffIndex.None && !body.HasBuff(targetBuff))
				{
					body.AddTimedBuff(targetBuff, EffectHooks.BuffCycleDuration);
				}
			}
		}
	}
}
