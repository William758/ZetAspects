using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class ZetAspectVoid
	{
		public static ItemIndex itemIndex;

		public static string nameToken = "ZETASPECTVOID";

		internal static void Init()
		{
			DefineItem();
			DamageHook();
			DamageTakenHook();
			ItemBehaviorHook();
		}

		private static void DefineItem()
		{
			bool hide = true;
			string icon;
			ItemTier tier;

			if (StarCompat.enabled && StarCompat.EliteCoreEnabled()) hide = false;

			if (ZetAspectsPlugin.ZetAspectRedTierCfg.Value && !hide)
			{
				tier = ItemTier.Tier3;
				icon = "@ZetAspects:Assets/Import/icons/texAffixVoidIconRed.png";
			}
			else
			{
				if (hide) tier = ItemTier.NoTier;
				else tier = ItemTier.Boss;
				icon = "@ZetAspects:Assets/Import/icons/texAffixVoidIconYellow.png";
			}

			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };

			if (!ZetAspectsPlugin.ZetAspectRedTierCfg.Value || hide)
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
				pickupModelPath = "@ZetAspects:Assets/Import/void/PickupAffixVoid.prefab",
				pickupIconPath = icon,
				tier = tier,
				tags = tags
			};

			ItemDisplayRuleDict disp = new ItemDisplayRuleDict(null);

			itemIndex = ItemAPI.Add(new CustomItem(itemDef, disp));

			LanguageAPI.Add("ITEM_" + nameToken + "_NAME", "Condemnation's Lament");
			LanguageAPI.Add("ITEM_" + nameToken + "_PICKUP", "Become an aspect of void.");
			LanguageAPI.Add("ITEM_" + nameToken + "_DESCRIPTION", BuildDescription());
			LanguageAPI.Add("ITEM_" + nameToken + "_LORE", "...");
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Void</style> :";
			if (ZetAspectsPlugin.ZetAspectVoidBaseDamageGainCfg.Value > 0f)
			{
				output += "\nIncrease <style=cIsDamage>damage</style> by <style=cIsDamage>";
				output += ZetAspectsPlugin.ZetAspectVoidBaseDamageGainCfg.Value * 100f + "%</style>";
				if (ZetAspectsPlugin.ZetAspectVoidStackDamageGainCfg.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += ZetAspectsPlugin.ZetAspectVoidStackDamageGainCfg.Value * 100f + "% per stack)</style>";
				}
				output += ".";
			}
			if (ZetAspectsPlugin.ZetAspectVoidBaseDamageTakenCfg.Value > 0f)
			{
				output += "\nReduce <style=cIsHealing>damage taken</style> by <style=cIsHealing>";
				output += ZetAspectsPlugin.ZetAspectVoidBaseDamageTakenCfg.Value * 100f + "%</style>";
				if (ZetAspectsPlugin.ZetAspectVoidStackDamageTakenCfg.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += ZetAspectsPlugin.ZetAspectVoidStackDamageTakenCfg.Value * 100f + "% per stack)</style>";
				}
				output += ".";
			}
			output += "\n<style=cIsUtility>Hinder</style> enemies inside its spherical aura, reducing <style=cIsUtility>jump power</style> and <style=cIsUtility>movement speed</style> by up to <style=cIsUtility>40%</style>.";

			return output;
		}

		private static void DamageHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(57),
					x => x.MatchLdloc(58),
					x => x.MatchMul(),
					x => x.MatchStloc(57)
				);

				if (found)
				{
					c.Index += 4;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 57);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, damage) =>
					{
						if (ZetAspectsPlugin.HasValidBuff(self, ZetAspectsPlugin.StarVoidAffixBuffIndex) && ZetAspectsPlugin.ZetAspectVoidBaseDamageGainCfg.Value > 0f)
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, itemIndex);
							float addedDamage = ZetAspectsPlugin.ZetAspectVoidBaseDamageGainCfg.Value + ZetAspectsPlugin.ZetAspectVoidStackDamageGainCfg.Value * (count - 1f);

							if (self.teamComponent.teamIndex != TeamIndex.Player) addedDamage *= ZetAspectsPlugin.ZetAspectEffectMonsterDamageMultCfg.Value;
							damage *= 1f + addedDamage;
						}

						return damage;
					});
					c.Emit(OpCodes.Stloc, 57);
				}
				else
				{
					Debug.LogWarning("ZetAspect : Void - Damage Hook Failed");
				}
			};
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
						if (ZetAspectsPlugin.HasValidBuff(healthComponent.body, ZetAspectsPlugin.StarVoidAffixBuffIndex) && ZetAspectsPlugin.ZetAspectVoidBaseDamageTakenCfg.Value > 0f)
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(healthComponent.body, itemIndex);
							float damageReduction = ZetAspectsPlugin.ZetAspectVoidBaseDamageTakenCfg.Value + ZetAspectsPlugin.ZetAspectVoidStackDamageTakenCfg.Value * (count - 1f);
							damageReduction = Util.ConvertAmplificationPercentageIntoReductionPercentage(damageReduction * 100f) * 0.01f;
							damage *= 1f - damageReduction;
						}

						return damage;
					});
					c.Emit(OpCodes.Stloc, 5);
				}
				else
				{
					Debug.LogWarning("ZetAspect : Void - Damage Taken Hook Failed");
				}
			};
		}

		private static void ItemBehaviorHook()
		{
			On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
			{
				if (StarCompat.enabled) StarCompat.UpdateAffixVoidAuraItemBehavior(self);

				orig(self);
			};
		}
	}
}
