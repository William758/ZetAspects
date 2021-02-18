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

		public static string nameToken = "ZETASPECTCELESTIAL";

		internal static void Init()
		{
			DefineItem();
			SlowHook();
			ArmorHook();
			ItemBehaviorHook();
		}

		private static void DefineItem()
		{
			string icon;

			if (ZetAspectsPlugin.ZetAspectRedTierCfg.Value)
			{
				icon = "@ZetAspects:Assets/Import/icons/texAffixHauntedIconRed.png";
			}
			else
			{
				icon = "@ZetAspects:Assets/Import/icons/texAffixHauntedIconYellow.png";
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
				pickupModelPath = "Prefabs/PickupModels/PickupAffixHaunted",
				pickupIconPath = icon,
				tier = ZetAspectsPlugin.ZetAspectRedTierCfg.Value ? ItemTier.Tier3 : ItemTier.Boss,
				tags = tags
			};

			ItemDisplayRuleDict disp = new ItemDisplayRuleDict(null);

			itemIndex = ItemAPI.Add(new CustomItem(itemDef, disp));

			LanguageAPI.Add("ITEM_" + nameToken + "_NAME", "Spectral Circlet");
			LanguageAPI.Add("ITEM_" + nameToken + "_PICKUP", "Become an aspect of incorporeality.");
			LanguageAPI.Add("ITEM_" + nameToken + "_DESCRIPTION", BuildDescription());
			LanguageAPI.Add("ITEM_" + nameToken + "_LORE", "...");
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Incorporeality</style> :";
			if (ZetAspectsPlugin.ZetAspectGhostSlowEffectCfg.Value)
			{
				output += "\nAttacks <style=cIsUtility>chill</style> on hit for ";
				output += ZetAspectsPlugin.FormatSeconds(ZetAspectsPlugin.ZetAspectWhiteSlowDurationCfg.Value);
				output += ", reducing <style=cIsUtility>movement speed</style> by <style=cIsUtility>80%</style>.";
			}
			if (ZetAspectsPlugin.ZetAspectGhostShredDurationCfg.Value > 0f)
			{
				output += "\nAttacks <style=cIsUtility>shred</style> on hit for ";
				output += ZetAspectsPlugin.FormatSeconds(ZetAspectsPlugin.ZetAspectGhostShredDurationCfg.Value);
				output += ", reducing <style=cIsUtility>armor</style> by <style=cIsUtility>";
				output += Mathf.Abs(ZetAspectsPlugin.ZetAspectGhostShredArmorCfg.Value) + "</style>.";
			}
			if (ZetAspectsPlugin.ZetAspectGhostBaseArmorGainCfg.Value > 0f)
			{
				output += "\nIncrease <style=cIsHealing>armor</style> by <style=cIsHealing>";
				output += ZetAspectsPlugin.ZetAspectGhostBaseArmorGainCfg.Value + "</style>";
				if (ZetAspectsPlugin.ZetAspectGhostStackArmorGainCfg.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += ZetAspectsPlugin.ZetAspectGhostStackArmorGainCfg.Value + " per stack)</style>";
				}
				output += ".";
			}
			if (ZetAspectsPlugin.ZetAspectGhostAllyArmorGainCfg.Value > 0f)
			{
				output += "\nGrants allies inside its spherical aura <style=cIsHealing>";
				output += ZetAspectsPlugin.ZetAspectGhostAllyArmorGainCfg.Value + " armor</style>.";
			}

			return output;
		}

		private static void SlowHook()
		{
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

					// Celestial slow
					c.EmitDelegate<Func<int, int>>((value) =>
					{
						if (!ZetAspectsPlugin.ZetAspectGhostSlowEffectCfg.Value) return 0;
						return value;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Celestial - Slow Hook Failed");
				}
			};
		}

		private static void ArmorHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(22),
					x => x.MatchConvR4(),
					x => x.MatchLdcR4(70f),
					x => x.MatchMul(),
					x => x.MatchAdd()
				);

				if (found)
				{
					c.Index += 5;

					c.Emit(OpCodes.Ldarg_0);
					c.EmitDelegate<Func<float, CharacterBody, float>>((armor, self) =>
					{
						return armor + GetArmorDelta(self);
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

		private static float GetArmorDelta(CharacterBody self)
        {
			// increase armor
			float addedArmor = 0f;
			if (self.HasBuff(BuffIndex.AffixHaunted) && ZetAspectsPlugin.ZetAspectGhostBaseArmorGainCfg.Value > 0f)
			{
				float count = ZetAspectsPlugin.GetStackMagnitude(self, itemIndex);
				addedArmor += ZetAspectsPlugin.ZetAspectGhostBaseArmorGainCfg.Value + ZetAspectsPlugin.ZetAspectGhostStackArmorGainCfg.Value * (count - 1f);
			}
			else if (self.HasBuff(BuffIndex.AffixHauntedRecipient) && ZetAspectsPlugin.ZetAspectGhostAllyArmorGainCfg.Value > 0f)
			{
				addedArmor += ZetAspectsPlugin.ZetAspectGhostAllyArmorGainCfg.Value;
			}
			//if (self.teamComponent.teamIndex != TeamIndex.Player) addedArmor *= 0.5f;

			// reduce armor
			float lostArmor = 0f;
			if (self.HasBuff(ZetAspectsPlugin.ZetShreddedDebuff))
			{
				lostArmor += Mathf.Abs(ZetAspectsPlugin.ZetAspectGhostShredArmorCfg.Value);
			}
			if (self.teamComponent.teamIndex == TeamIndex.Player) lostArmor *= ZetAspectsPlugin.ZetAspectEffectPlayerDebuffMultCfg.Value;

			return addedArmor - lostArmor;
		}
	}
}
