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

			//bool logInfo = false;
			//logInfo = body.isPlayerControlled;

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
							//if (logInfo) Logger.Info("EBM - " + timedBuff.buffIndex + " : " + timedBuff.timer);

							if (!buffsToRefresh.Contains(buffIndex))
							{
								buffsToRefresh.Add(buffIndex);
							}
						}
					}
				}
				/*
				if (logInfo && buffsToRefresh.Count > 0)
				{
					Logger.Info("EBM RefreshCount - " + buffsToRefresh.Count);
				}
				*/
				foreach (BuffIndex buffIndex in buffsToRefresh)
				{
					if (Catalog.HasAspectItemOrEquipment(inventory, buffIndex) || BlightedStateManager.HasAspectFromBlighted(body, buffIndex))
					{
						//if (logInfo) Logger.Info("EBM Refresh - " + buffIndex);

						body.AddTimedBuff(buffIndex, refreshDuration);
					}
				}
			}

			buffsToRefresh.Clear();
		}
	}



	public static class BlightedStateManager
	{
		public static Dictionary<NetworkInstanceId, BuffIndex[]> Affixes = new Dictionary<NetworkInstanceId, BuffIndex[]>();



		internal static void Activated(CharacterBody body, BuffIndex firstBuff, BuffIndex secondBuff)
		{
			//Logger.Warn("ZetAspects - BlightedStateManager - SetElites : " + body.name + " - " + body.netId);
			//Logger.Warn("Setting EliteBuffs : [" + BuffCatalog.GetBuffDef(firstBuff).name + "] - [" + BuffCatalog.GetBuffDef(secondBuff).name + "]");

			NetworkInstanceId netId = body.netId;

			if (!Affixes.ContainsKey(netId)) CreateEntry(netId);

			BuffIndex oldFirstBuff = Affixes[netId][0];
			BuffIndex oldSecondBuff = Affixes[netId][1];

			Affixes[netId][0] = firstBuff;
			Affixes[netId][1] = secondBuff;

			if (NetworkServer.active)
			{
				if (!EffectHooks.DestroyedBodies.ContainsKey(netId))
				{
					bool clearBuffs = true;

					Inventory inventory = body.inventory;
					if (inventory && inventory.GetItemCount(RoR2Content.Items.HeadHunter) > 0) clearBuffs = false;

					if (clearBuffs)
					{
						if (oldFirstBuff != BuffIndex.None) body.ClearTimedBuffs(oldFirstBuff);
						if (oldSecondBuff != BuffIndex.None) body.ClearTimedBuffs(oldSecondBuff);
					}

					ApplyBlightedAspectBuffs(body);
				}
			}
		}

		internal static void Deactivated(CharacterBody body)
		{
			NetworkInstanceId netId = body.netId;

			if (Affixes.ContainsKey(netId))
			{
				BuffIndex firstBuff = Affixes[netId][0];
				BuffIndex secondBuff = Affixes[netId][1];

				Affixes[netId][0] = BuffIndex.None;
				Affixes[netId][1] = BuffIndex.None;

				if (NetworkServer.active)
				{
					if (!EffectHooks.DestroyedBodies.ContainsKey(netId))
					{
						bool clearBuffs = true;

						Inventory inventory = body.inventory;
						if (inventory && inventory.GetItemCount(RoR2Content.Items.HeadHunter) > 0) clearBuffs = false;

						if (clearBuffs)
						{
							if (firstBuff != BuffIndex.None) body.ClearTimedBuffs(firstBuff);
							if (secondBuff != BuffIndex.None) body.ClearTimedBuffs(secondBuff);
						}
					}
				}
			}
		}

		

		private static void CreateEntry(NetworkInstanceId netId)
		{
			BuffIndex[] buffs = new BuffIndex[] { BuffIndex.None, BuffIndex.None };
			Affixes.Add(netId, buffs);
		}

		internal static void DestroyEntry(NetworkInstanceId netId)
		{
			if (Affixes.ContainsKey(netId))
			{
				Affixes.Remove(netId);
			}
		}



		public static bool HasAspectFromBlighted(CharacterBody body, BuffDef buffDef)
		{
			BuffIndex buffIndex = buffDef.buffIndex;

			return HasAspectFromBlighted(body, buffIndex);
		}

		public static bool HasAspectFromBlighted(CharacterBody body, BuffIndex buffIndex)
		{
			NetworkInstanceId netId = body.netId;

			if (Affixes.ContainsKey(netId))
			{
				if (buffIndex == BuffIndex.None) return false;
				if (Affixes[netId][0] == buffIndex) return true;
				if (Affixes[netId][1] == buffIndex) return true;
			}

			return false;
		}

		internal static void ApplyBlightedAspectBuffs(CharacterBody body)
		{
			NetworkInstanceId netId = body.netId;

			if (Affixes.ContainsKey(netId))
			{
				BuffIndex targetBuff;

				targetBuff = Affixes[netId][0];
				if (targetBuff != BuffIndex.None && !body.HasBuff(targetBuff))
				{
					body.AddTimedBuff(targetBuff, EffectHooks.BuffCycleDuration);
				}
				targetBuff = Affixes[netId][1];
				if (targetBuff != BuffIndex.None && !body.HasBuff(targetBuff))
				{
					body.AddTimedBuff(targetBuff, EffectHooks.BuffCycleDuration);
				}
			}
		}
	}


	/*
	public class GoldPowerBehavior : CharacterBody.ItemBehavior
	{
		public int maxStacks = 0;
		public int currentStacks = 0;

		public float tracker = 0f;
		public float chargeRate = 2.5f;
		public float decayRate = 5f;

		public float timeSinceLastCharge = 0f;
		public float chargeThreshold = 0.5f;
		public float holdTimer = 0f;

		public static BuffIndex buffIndex = BuffIndex.None;

		public void Awake()
		{
			enabled = false;
		}

		public void OnEnable()
		{
			maxStacks = RoseBuckler.MaxMomentum.Value;

			chargeThreshold = Mathf.Max(0f, RoseBuckler.ChargeThreshold.Value);

			chargeRate = RoseBuckler.ChargeInterval.Value;
			chargeRate = (chargeRate >= 0.01f) ? (1f / chargeRate) : 0f;

			decayRate = RoseBuckler.DecayInterval.Value;
			decayRate = (decayRate >= 0.01f) ? (1f / decayRate) : 0f;

			if (body)
			{
				body.SetBuffCount(buffIndex, 0);
			}

			timeSinceLastCharge = chargeThreshold + 1f;
		}

		public void OnDisable()
		{
			if (body)
			{
				body.SetBuffCount(buffIndex, 0);
			}
		}

		public void FixedUpdate()
		{
			if (body)
			{
				if (maxStacks > 0)
				{
					float deltaTime = Time.fixedDeltaTime;

					if (body.isSprinting) timeSinceLastCharge = 0f;
					else timeSinceLastCharge += deltaTime;

					if (timeSinceLastCharge <= chargeThreshold)
					{
						if (chargeRate == 0f)
						{
							tracker = 0f;
							currentStacks = maxStacks;
						}
						else
						{
							if (timeSinceLastCharge == 0f)
							{
								tracker += deltaTime * chargeRate;
							}

							if (currentStacks >= maxStacks)
							{
								tracker = Mathf.Min(tracker, 0f);
							}
							else if (tracker >= 1f)
							{
								tracker -= 1f;
								currentStacks++;
							}
						}
					}
					else
					{
						if (decayRate == 0f)
						{
							tracker = 0f;
							currentStacks = 0;
						}
						else
						{
							tracker -= deltaTime * decayRate;

							if (currentStacks <= 0)
							{
								tracker = Mathf.Max(tracker, 0f);
							}
							else if (tracker <= -1f)
							{
								tracker += 1f;
								currentStacks--;
							}
						}
					}
				}
				else
				{
					currentStacks = 0;
				}

				int buffCount = body.GetBuffCount(buffIndex);
				if (buffCount != currentStacks)
				{
					body.SetBuffCount(buffIndex, currentStacks);
				}
			}
		}
	}
	*/
}
