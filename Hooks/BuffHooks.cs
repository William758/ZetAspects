using System;
using UnityEngine.Networking;
using RoR2;
using MonoMod.Cil;
using Mono.Cecil.Cil;

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
			On.RoR2.CharacterBody.OnBuffFinalStackLost += UpdateOnBuffLostHook;

			CharacterBody.onBodyInventoryChangedGlobal += HandleEliteBuffManager;
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

								if (aspectDef.PreBuffGrant != null)
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
			}

			orig(self);
		}

		private static void UpdateOnBuffLostHook(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
		{
			orig(self, buffDef);

			if (!self || buffDef.buffIndex == BuffIndex.None || !NetworkServer.active || !Run.instance) return;

			if (!EffectHooks.DestroyedBodies.ContainsKey(self.netId))
			{
				if (buffDef.buffIndex == Catalog.lampBuff) return;

				Inventory inventory = self.inventory;
				if (inventory)
				{
					bool aspectBuff = false;
					BuffDef buffToCheck = buffDef;

					if (BuffDefOf.ZetEchoPrimer && buffDef == BuffDefOf.ZetEchoPrimer) buffToCheck = BuffDefOf.AffixEcho;

					if (Catalog.aspectBuffIndexes.Contains(buffToCheck.buffIndex))
					{
						aspectBuff = true;

						if (Catalog.BodyAllowedAffix(self, buffToCheck) && Catalog.HasAspectItemOrEquipment(inventory, buffToCheck))
						{
							self.AddTimedBuff(buffToCheck, BuffCycleDuration);
						}
						else
						{
							if (buffToCheck == BuffDefOf.AffixVeiled)
							{
								for (int i = 0; i < 2; i++)
								{
									if (self.HasBuff(Catalog.veiledBuffer))
									{
										self.RemoveBuff(Catalog.veiledBuffer);
									}
								}
							}
						}
					}

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
			if (NetworkServer.active)
			{
				int count = EffectHooks.DestroyedBodies.ContainsKey(body.netId) ? 0 : body.eliteBuffCount;
				body.AddItemBehavior<EliteBuffManager>(count);

				if (body.HasBuff(BuffDefOf.AffixVeiled))
				{
					bool nemCloak = Compat.NemSpikeStrip.VeiledEnabled && Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.VeiledHitToShowField, true);
					if (nemCloak)
					{
						if (!body.HasBuff(Catalog.veiledCooldown) && !body.HasBuff(RoR2Content.Buffs.Cloak))
						{
							body.AddTimedBuff(Catalog.veiledCooldown, 0.1f);
						}

						if (body.HasBuff(Catalog.veiledCooldown) && body.HasBuff(BuffDefOf.ZetElusive))
						{
							body.ClearTimedBuffs(BuffDefOf.ZetElusive);
						}
					}
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
}
