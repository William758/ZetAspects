using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace TPDespair.ZetAspects
{
	public static class BuffHooks
	{
		internal const float BuffCycleDuration = 5f;

		internal static void Init()
		{
			IL.RoR2.CharacterBody.OnEquipmentLost += EquipmentLostBuffHook;
			IL.RoR2.CharacterBody.OnEquipmentGained += EquipmentGainedBuffHook;

			On.RoR2.CharacterBody.OnInventoryChanged += ApplyAspectBuffOnInventoryChangedHook;
			On.RoR2.CharacterBody.OnBuffFirstStackGained += UpdateOnBuffGainHook;
			On.RoR2.CharacterBody.OnBuffFinalStackLost += UpdateOnBuffLostHook;
		}

		

		private static void EquipmentLostBuffHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchLdarg(1),
				x => x.MatchLdfld<EquipmentDef>("passiveBuffDef"),
				x => x.MatchBrfalse(out _)
			);

			if (found)
			{
				c.Index += 2;

				c.EmitDelegate<Func<BuffDef, BuffDef>>((buffDef) =>
				{
					if (!buffDef) return buffDef;

					if (Catalog.GetAspectEquipIndex(buffDef.buffIndex) != EquipmentIndex.None) return null;

					return buffDef;
				});
			}
			else
			{
				Logger.Warn("EquipmentLostBuffHook Failed");
			}
		}

		private static void EquipmentGainedBuffHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchLdarg(1),
				x => x.MatchLdfld<EquipmentDef>("passiveBuffDef"),
				x => x.MatchBrfalse(out _)
			);

			if (found)
			{
				c.Index += 2;

				c.Emit(OpCodes.Ldarg, 0);
				c.EmitDelegate<Func<BuffDef, CharacterBody, BuffDef>>((buffDef, self) =>
				{
					if (!buffDef || !self) return buffDef;

					if (Catalog.GetAspectEquipIndex(buffDef.buffIndex) != EquipmentIndex.None)
					{
						if (!EffectHooks.DestroyedBodies.ContainsKey(self.netId))
						{
							AspectDef aspectDef = Catalog.GetAspectDef(buffDef.buffIndex);
							if (aspectDef != null && Catalog.BodyAllowedAffix(self, aspectDef))
							{
								bool proceed = true;

								if (!self.HasBuff(buffDef) && aspectDef.PreBuffGrant != null)
								{
									proceed = aspectDef.PreBuffGrant(self, self.inventory);
								}

								if (proceed)
								{
									self.AddTimedBuff(buffDef, 1.25f);
								}
							}
						}

						return null;
					}

					return buffDef;
				});
			}
			else
			{
				Logger.Warn("EquipmentGainedBuffHook Failed");
			}
		}

		private static void ApplyAspectBuffOnInventoryChangedHook(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
		{
			if (NetworkServer.active && self)
			{
				HandleEliteBuffManager(self);



				if (!EffectHooks.DestroyedBodies.ContainsKey(self.netId))
				{
					EliteBuffManager manager = self.GetComponent<EliteBuffManager>();
					if (manager)
					{
						manager.RefreshEliteBuffs();
					}

					foreach (IAspectProvider provider in EliteBuffManager.Providers)
					{
						EliteBuffManager.ApplyBuffs(self, provider.Aspects(self));
					}

					ApplyAspectBuffs(self);
				}



				if (self.HasBuff(BuffDefOf.AffixVeiled))
				{
					bool nemCloak = Compat.NemSpikeStrip.VeiledEnabled && Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.VeiledHitToShowField, true);
					if (nemCloak)
					{
						if (!self.HasBuff(Catalog.veiledCooldown) && !self.HasBuff(RoR2Content.Buffs.Cloak))
						{
							self.AddTimedBuff(Catalog.veiledCooldown, 0.1f);
						}

						if (self.HasBuff(Catalog.veiledCooldown) && self.HasBuff(BuffDefOf.ZetElusive))
						{
							self.SetBuffCount(BuffDefOf.ZetElusive.buffIndex, 0);
						}
					}
				}
			}

			orig(self);
		}

		private static void UpdateOnBuffGainHook(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buffDef)
		{
			orig(self, buffDef);

			if (!NetworkServer.active || !Run.instance) return;
			if (!self || !buffDef) return;

			BuffIndex buffIndex = buffDef.buffIndex;
			if (buffIndex == BuffIndex.None) return;

			HandleEliteBuffManager(self);

			if (!EffectHooks.DestroyedBodies.ContainsKey(self.netId))
			{
				if (Catalog.aspectBuffIndexes.Contains(buffIndex))
				{
					AspectDef aspectDef = Catalog.GetAspectDef(buffIndex);
					if (aspectDef != null)
					{
						if (aspectDef.OnApplied != null)
						{
							aspectDef.OnApplied(self);
						}
					}
				}
			}
		}

		private static void UpdateOnBuffLostHook(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
		{
			orig(self, buffDef);

			if (!NetworkServer.active || !Run.instance) return;
			if (!self || !buffDef) return;

			BuffIndex buffIndex = buffDef.buffIndex;
			if (buffIndex == BuffIndex.None) return;
			if (buffIndex == Catalog.lampBuff) return;

			HandleEliteBuffManager(self);

			if (!EffectHooks.DestroyedBodies.ContainsKey(self.netId))
			{
				Inventory inventory = self.inventory;
				
				bool aspectBuff = false;
				BuffDef buffToCheck = buffDef;

				if (BuffDefOf.ZetEchoPrimer && buffDef == BuffDefOf.ZetEchoPrimer) buffToCheck = BuffDefOf.AffixEcho;

				if (Catalog.aspectBuffIndexes.Contains(buffToCheck.buffIndex))
				{
					aspectBuff = true;

					if (Catalog.BodyAllowedAffix(self, buffToCheck) && ((inventory && Catalog.HasAspectItemOrEquipment(inventory, buffToCheck)) || Catalog.HasAspectFromProviders(self, buffToCheck)))
					{
						self.AddTimedBuff(buffToCheck, BuffCycleDuration);
					}
					else
					{
						AspectDef aspectDef = Catalog.GetAspectDef(buffToCheck.buffIndex);
						if (aspectDef != null)
						{
							if (aspectDef.OnExpire != null)
							{
								aspectDef.OnExpire(self);
							}
						}
					}
				}

				if (inventory)
				{
					// update itemBehaviors and itemDisplays
					int updateMode = Configuration.UpdateInventoryFromBuff.Value;
					if (updateMode > 0 && (updateMode > 1 || aspectBuff))
					{
						//Logger.Warn("UpdateInventory : [" + buffDef.buffIndex + "] " + buffDef.name);
						inventory.GiveItemPermanent(ItemDefOf.ZetAspectsUpdateInventory);
					}
				}
			}
		}



		private static void HandleEliteBuffManager(CharacterBody body)
		{
			if (NetworkServer.active && body)
			{
				int count = EffectHooks.DestroyedBodies.ContainsKey(body.netId) ? 0 : body.eliteBuffCount;
				body.AddItemBehavior<EliteBuffManager>(count);

				if (BuffDefOf.ZetElusive != null)
				{
					body.AddItemBehavior<ElusiveDecayBehavior>(body.HasBuff(BuffDefOf.ZetElusive) ? 1 : 0);
				}
			}
		}



		private static void ApplyAspectBuffs(CharacterBody self)
		{
			Inventory inventory = self.inventory;

			if (!inventory) return;

			foreach (AspectDef aspectDef in Catalog.aspectDefs)
			{
				if (!aspectDef.invalid && aspectDef.PackPopulated)
				{
					BuffDef buffDef = aspectDef.buffDef;

					if (!self.HasBuff(buffDef) && Catalog.BodyAllowedAffix(self, aspectDef))
					{
						if (Catalog.HasAspectItemOrEquipment(inventory, aspectDef.itemDef, aspectDef.equipmentDef))
						{
							bool proceed = true;

							if (aspectDef.PreBuffGrant != null)
							{
								proceed = aspectDef.PreBuffGrant(self, inventory);
							}

							if (proceed)
							{
								self.AddTimedBuff(buffDef, BuffCycleDuration);
							}
						}
					}
				}
			}
		}
	}



	public class ElusiveDecayBehavior : CharacterBody.ItemBehavior
	{
		private float stopwatch = 0f;
		private int internalStacks = -1000;

		private static float UpdateInterval
		{
			get
			{
				return Configuration.AspectVeiledElusiveDuration.Value / 20f;
			}
		}

		private void Awake()
		{
			enabled = false;
		}

		internal void FixedUpdate()
		{
			if (body)
			{
				stopwatch += Time.fixedDeltaTime;

				float interval = UpdateInterval;
				if (stopwatch > interval)
				{
					stopwatch -= interval;

					if (internalStacks == -1000)
					{
						internalStacks = body.GetBuffCount(BuffDefOf.ZetElusive);
					}

					internalStacks = Mathf.Max(0, internalStacks - 5);
					if (internalStacks > 0)
					{
						if (Configuration.AspectVeiledElusiveDecay.Value)
						{
							body.SetBuffCount(BuffDefOf.ZetElusive.buffIndex, Mathf.Max(25, internalStacks));
						}
					}
					else
					{
						body.ClearTimedBuffs(BuffDefOf.ZetElusive);
						body.SetBuffCount(BuffDefOf.ZetElusive.buffIndex, 0);
					}
				}
			}
		}

		public static bool SetStacks(CharacterBody body, int stacks, int buffer = 10)
		{
			if (stacks < 25) stacks = 25;

			int buffCount = body.GetBuffCount(BuffDefOf.ZetElusive);
			if (stacks > buffCount + buffer)
			{
				// should create the behavior if it doesnt already exist
				body.ClearTimedBuffs(BuffDefOf.ZetElusive);
				body.AddTimedBuff(BuffDefOf.ZetElusive.buffIndex, (stacks / 5) * UpdateInterval);
				body.SetBuffCount(BuffDefOf.ZetElusive.buffIndex, stacks);

				var comp = body.GetComponent<ElusiveDecayBehavior>();
				if (comp != null)
				{
					comp.internalStacks = stacks;
					comp.stopwatch = 0f;
				}

				return true;
			}

			return false;
		}
	}
}
