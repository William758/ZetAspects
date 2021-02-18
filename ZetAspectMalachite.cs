using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class ZetAspectMalachite
	{
		public static ItemIndex itemIndex;

		public static string nameToken = "ZETASPECTMALACHITE";

		internal static void Init()
		{
			DefineItem();
			DamageTakenHook();
			NullDurationHook();
			LifeGainHook();
			HealthHook();
			SpikeballHook();
		}

		private static void DefineItem()
		{
			string icon;

			if (ZetAspectsPlugin.ZetAspectRedTierCfg.Value)
			{
				icon = "@ZetAspects:Assets/Import/icons/texAffixPoisonIconRed.png";
			}
			else
			{
				icon = "@ZetAspects:Assets/Import/icons/texAffixPoisonIconYellow.png";
			}

			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };

			if (!ZetAspectsPlugin.ZetAspectRedTierCfg.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = new ItemDef
			{
				name = "ITEM_" + nameToken,
				nameToken = "ITEM_" + nameToken + "_NAME",
				pickupToken = "ITEM_" + nameToken + "_PICKUP",
				descriptionToken = "ITEM_" + nameToken + "_DESCRIPTION",
				loreToken = "ITEM_" + nameToken + "_LORE",
				pickupModelPath = "Prefabs/PickupModels/PickupAffixPoison",
				pickupIconPath = icon,
				tier = ZetAspectsPlugin.ZetAspectRedTierCfg.Value ? ItemTier.Tier3 : ItemTier.Boss,
				tags = tags
			};

			ItemDisplayRuleDict disp = new ItemDisplayRuleDict(null);

			itemIndex = ItemAPI.Add(new CustomItem(itemDef, disp));

			LanguageAPI.Add("ITEM_" + nameToken + "_NAME", "N'kuhana's Retort");
			LanguageAPI.Add("ITEM_" + nameToken + "_PICKUP", "Become an aspect of corruption.");
			LanguageAPI.Add("ITEM_" + nameToken + "_DESCRIPTION", BuildDescription());
			LanguageAPI.Add("ITEM_" + nameToken + "_LORE", "...");
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Corruption</style> :";
			if (ZetAspectsPlugin.ZetAspectPoisonFireSpikesCfg.Value)
			{
				output += "\nPeriodically releases spiked balls that sprout spike pits from where they land.";
			}
			output += "\nAttacks <style=cIsUtility>nullify</style> on hit for ";
			output += ZetAspectsPlugin.FormatSeconds(ZetAspectsPlugin.ZetAspectPoisonNullDurationCfg.Value);
			if (ZetAspectsPlugin.ZetAspectPoisonNullDamageTakenCfg.Value != 0f)
			{
				output += ", increasing <style=cIsUtility>damage taken</style> by <style=cIsUtility>";
				output += Mathf.Abs(ZetAspectsPlugin.ZetAspectPoisonNullDamageTakenCfg.Value) * 100f + "%</style>";
			}
			output += ".";
			if (ZetAspectsPlugin.ZetAspectPoisonBaseHealthGainCfg.Value > 0f)
			{
				output += "\nIncrease <style=cIsHealing>maximum health</style> by <style=cIsHealing>";
				output += ZetAspectsPlugin.ZetAspectPoisonBaseHealthGainCfg.Value + "</style>";
				if (ZetAspectsPlugin.ZetAspectPoisonStackHealthGainCfg.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += ZetAspectsPlugin.ZetAspectPoisonStackHealthGainCfg.Value + " per stack)</style>";
				}
				output += ".";
			}
			if (ZetAspectsPlugin.ZetAspectPoisonBaseLgohCfg.Value > 0)
			{
				output += "\nDealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>";
				output += ZetAspectsPlugin.ZetAspectPoisonBaseLgohCfg.Value + "</style>";
				if (ZetAspectsPlugin.ZetAspectPoisonStackLgohCfg.Value != 0)
				{
					output += " <style=cStack>(+";
					output += ZetAspectsPlugin.ZetAspectPoisonStackLgohCfg.Value + " per stack)</style>";
				}
				output += " health.";
			}

			return output;
		}

		private static void DamageTakenHook()
		{
			IL.RoR2.HealthComponent.TakeDamage += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(1),
					x => x.MatchLdfld<DamageInfo>("damage"),
					x => x.MatchStloc(5)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Ldloc, 5);
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<float, HealthComponent, float>>((damage, healthComponent) =>
					{
						if (healthComponent.body.HasBuff(BuffIndex.HealingDisabled) && ZetAspectsPlugin.ZetAspectPoisonNullDamageTakenCfg.Value != 0f)
						{
							float extraDamage = Mathf.Abs(ZetAspectsPlugin.ZetAspectPoisonNullDamageTakenCfg.Value);
							if (healthComponent.body.teamComponent.teamIndex == TeamIndex.Player) extraDamage *= ZetAspectsPlugin.ZetAspectEffectPlayerDebuffMultCfg.Value;
							return damage * (1f + extraDamage);
						}
						else
						{
							return damage;
						}
					});
					c.Emit(OpCodes.Stloc, 5);
				}
				else
				{
					Debug.LogWarning("ZetAspect : Malachite - Nullified Damage Taken Hook Failed");
				}
			};
		}

		private static void NullDurationHook()
		{
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(1),
					x => x.MatchLdcI4(0x20),
					x => x.MatchLdcR4(8f),
					x => x.MatchLdarg(1),
					x => x.MatchLdfld<DamageInfo>("procCoefficient"),
					x => x.MatchMul()
				);

				if (found)
				{
					c.Index += 6;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc_0);
					c.Emit(OpCodes.Ldarg_1);
					c.EmitDelegate<Func<CharacterBody, DamageInfo, float>>((self, damageInfo) =>
					{
						float duration = ZetAspectsPlugin.ZetAspectPoisonNullDurationCfg.Value;
						if (self.teamComponent.teamIndex != TeamIndex.Player) duration = 8f * damageInfo.procCoefficient;

						return duration;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Malachite - Nullified Duration Hook Failed");
				}
			};
		}

		private static void LifeGainHook()
		{
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				// LGOH
				bool found = c.TryGotoNext(
					x => x.MatchLdloc(3),
					x => x.MatchLdcI4(13),
					x => x.MatchCallvirt<Inventory>("GetItemCount"),
					x => x.MatchStloc(18)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Ldloc_0);
					c.Emit(OpCodes.Ldloc, 18);
					c.EmitDelegate<Func<int, CharacterBody, int, int>>((lgoh, self, seed) =>
					{
						if (ZetAspectsPlugin.ZetAspectLeechSeedLgohCfg.Value > 1)
						{
							lgoh += seed * (ZetAspectsPlugin.ZetAspectLeechSeedLgohCfg.Value - 1);
						}

						if (self.HasBuff(BuffIndex.AffixPoison) && ZetAspectsPlugin.ZetAspectPoisonBaseLgohCfg.Value > 0)
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, itemIndex);
							lgoh += ZetAspectsPlugin.ZetAspectPoisonBaseLgohCfg.Value + (int)(ZetAspectsPlugin.ZetAspectPoisonStackLgohCfg.Value * (count - 1f) + 0.55f);
						}

						return lgoh;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Malachite - LGOH Hook Failed");
				}
			};
		}

		private static void HealthHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(41),
					x => x.MatchLdloc(42),
					x => x.MatchMul(),
					x => x.MatchStloc(41)
				);

				if (found)
				{
					c.Emit(OpCodes.Ldarg_0);
					c.Emit(OpCodes.Ldloc, 41);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, health) =>
					{
						if (self.HasBuff(BuffIndex.AffixPoison) && ZetAspectsPlugin.ZetAspectPoisonBaseHealthGainCfg.Value > 0f)
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, itemIndex);
							float addedHealth = ZetAspectsPlugin.ZetAspectPoisonBaseHealthGainCfg.Value + ZetAspectsPlugin.ZetAspectPoisonStackHealthGainCfg.Value * (count - 1f);
							//if (self.teamComponent.teamIndex != TeamIndex.Player) addedHealth *= 0.5f;
							health += addedHealth;
						}

						return health;
					});
					c.Emit(OpCodes.Stloc, 41);
				}
				else
				{
					Debug.LogWarning("ZetAspect : Malachite - Health Hook Failed");
				}
			};
		}

		private static void SpikeballHook()
		{
			IL.RoR2.CharacterBody.UpdateAffixPoison += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("poisonballTimer"),
					x => x.MatchLdarg(1),
					x => x.MatchAdd(),
					x => x.MatchStfld<CharacterBody>("poisonballTimer")
				);

				if (found)
				{
					c.Index += 4;

					c.Emit(OpCodes.Ldarg_0);
					c.EmitDelegate<Func<float, CharacterBody, float>>((deltaTime, self) =>
					{
						if (self.teamComponent.teamIndex == TeamIndex.Player && !ZetAspectsPlugin.ZetAspectPoisonFireSpikesCfg.Value) return 0f;

						return deltaTime;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Malachite - Spikeball Hook Failed");
				}
			};
		}
	}
}
