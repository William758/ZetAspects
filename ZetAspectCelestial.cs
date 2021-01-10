using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class ZetAspectCelestial
	{
		public static ItemIndex itemIndex;

		internal static void Init()
		{
			DefineItem();
			SetHook();
			ItemBehaviorHook();
		}

		private static void DefineItem()
		{
			string st1 = ZetAspectsPlugin.ZetAspectGhostShredDurationCfg.Value == 1.0f ? "" : "s";

			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };

			if (!ZetAspectsPlugin.ZetAspectRedTierCfg.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = new ItemDef
			{
				name = "ZetAspectCelestial",
				tier = ZetAspectsPlugin.ZetAspectRedTierCfg.Value ? ItemTier.Tier3 : ItemTier.Boss,
				pickupModelPath = "Prefabs/PickupModels/PickupAffixHaunted",
				pickupIconPath = "Textures/ItemIcons/texAffixHauntedIcon",
				nameToken = "Spectral Circlet",
				pickupToken = "Become an aspect of incorporeality.",
				descriptionToken = "<style=cDeath>Aspect of Incorporeality</style> :\nAttacks <style=cIsUtility>shred</style> on hit for " + ZetAspectsPlugin.ZetAspectGhostShredDurationCfg.Value + " second" + st1 + ", reducing <style=cIsUtility>armor</style> by <style=cIsUtility>"+ Mathf.Abs(ZetAspectsPlugin.ZetAspectGhostShredArmorCfg.Value) + "</style>.\n<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + ZetAspectsPlugin.ZetAspectGhostBaseArmorGainCfg.Value + "</style> <style=cStack>(+" + ZetAspectsPlugin.ZetAspectGhostStackArmorGainCfg.Value + " per stack)</style>.\nGrants allies inside its spherical aura <style=cIsHealing>" + ZetAspectsPlugin.ZetAspectGhostAllyArmorGainCfg.Value + " armor</style>.",
				loreToken = "Become an aspect of incorporeality.",
				tags = tags
			};

			ItemDisplayRuleDict disp = new ItemDisplayRuleDict(null);

			itemIndex = ItemAPI.Add(new CustomItem(itemDef, disp));
		}

		private static void SetHook()
		{
			// Remove slow
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcI4(2),
					x => x.MatchAdd(),
					x => x.MatchStloc(8)
				);

				if (found)
				{
					c.Index += 1;

					// Celestial does not slow
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 0);
				}
				else
				{
					Debug.LogWarning("ZetAspect : Celestial - Slow Hook Failed");
				}
			};

			// Modify armor values
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdarg(0),
					x => x.MatchCallvirt<CharacterBody>("get_armor"),
					x => x.MatchLdloc(22),
					x => x.MatchConvR4()
				);

				if (found)
				{
					c.Index += 8;

					c.Emit(OpCodes.Ldarg_0);
					c.EmitDelegate<Func<float, CharacterBody, float>>((armor, self) =>
					{
						// increase armor
						float addedArmor = 0f;
						if (self.HasBuff(BuffIndex.AffixHaunted))
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, itemIndex);
							addedArmor += ZetAspectsPlugin.ZetAspectGhostBaseArmorGainCfg.Value + ZetAspectsPlugin.ZetAspectGhostStackArmorGainCfg.Value * (count - 1f);
						}
						else if (self.HasBuff(BuffIndex.AffixHauntedRecipient))
						{
							addedArmor += ZetAspectsPlugin.ZetAspectGhostAllyArmorGainCfg.Value;
						}
						//if (self.teamComponent.teamIndex != TeamIndex.Player) addedArmor *= 0.5f;
						armor += addedArmor;

						// reduce armor
						addedArmor = 0f;
						if (self.HasBuff(ZetAspectsPlugin.ZetShreddedDebuff))
						{
							addedArmor -= Mathf.Abs(ZetAspectsPlugin.ZetAspectGhostShredArmorCfg.Value);
						}
						if (self.teamComponent.teamIndex == TeamIndex.Player) addedArmor *= ZetAspectsPlugin.ZetAspectEffectPlayerDebuffMultCfg.Value;
						armor += addedArmor;

						return armor;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Celestial - Armor And Shredded Armor Hook Failed");
				}
			};
		}

		private static void ItemBehaviorHook()
		{
			On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
			{
				self.AddItemBehavior<CharacterBody.AffixHauntedBehavior>(self.HasBuff(BuffIndex.AffixHaunted) ? 1 : 0);

				orig(self);
			};
		}
	}
}
