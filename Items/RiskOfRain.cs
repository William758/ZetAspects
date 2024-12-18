﻿using System;
using UnityEngine;
using RoR2;

using static TPDespair.ZetAspects.Configuration;
using static TPDespair.ZetAspects.Language;

namespace TPDespair.ZetAspects.Items
{
	public static class ZetAspectWhite
	{
		public static string identifier = "ZetAspectWhite";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Damage, ItemTag.Utility };
			if (AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = Catalog.WhiteAspectPrefab;
			itemDef.pickupIconSprite = Catalog.Sprites.AffixUnknown;

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "WHITE";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixWhite;
				if (equipDef)
				{
					string equipActiveFragment = Catalog.altIceActive ? "AFFIX_WHITE_ACTIVE_ALT" : "AFFIX_WHITE_ACTIVE";

					RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
					RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
					RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment(equipActiveFragment)));
				}
			}

			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_ICE");
			output += String.Format(TextFragment("CHILL_ON_HIT"), SecondText(AspectWhiteSlowDuration.Value, "for"));
			if (AspectWhiteBaseFreezeChance.Value > 0f)
			{
				output += String.Format(
					TextFragment("CHANCE_TO_FREEZE"),
					ScalingText(AspectWhiteBaseFreezeChance.Value, AspectWhiteStackFreezeChance.Value, "chance", "cIsUtility", combine),
					SecondText(AspectWhiteFreezeDuration.Value, "for")
				);
			}
			if (AspectWhiteBaseDamage.Value > 0f)
			{
				output += String.Format(
					TextFragment("FROST_BLADE"),
					ScalingText(AspectWhiteBaseDamage.Value, AspectWhiteStackDamage.Value, "percent", "cIsDamage", combine)
				);
			}

			return output;
		}
	}



	public static class ZetAspectBlue
	{
		public static string identifier = "ZetAspectBlue";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Damage, ItemTag.Utility };
			if (AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = Catalog.WhiteAspectPrefab;
			itemDef.pickupIconSprite = Catalog.Sprites.AffixUnknown;

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "BLUE";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixBlue;
				if (equipDef)
				{
					RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
					RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
					RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
				}
			}

			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_LIGHTNING");
			if (Compat.EliteReworks.affixBlueEnabled && Compat.EliteReworks.affixBluePassive)
			{
				output += TextFragment("PASSIVE_SCATTER_BOMB");
			}
			if (AspectBlueSappedDuration.Value > 0f)
			{
				output += String.Format(
					TextFragment("SAP_ON_HIT"),
					SecondText(AspectBlueSappedDuration.Value, "for"),
					ScalingText(Mathf.Abs(AspectBlueSappedDamage.Value), "percent", "cIsUtility")
				);
			}
			if (!Compat.EliteReworks.affixBlueEnabled || !Compat.EliteReworks.affixBlueRemoveShield)
			{
				if (AspectBlueHealthConverted.Value > 0f)
				{
					output += String.Format(
						TextFragment("CONVERT_SHIELD"),
						ScalingText(AspectBlueHealthConverted.Value, "percent", "cIsHealing")
					);
				}
			}
			if (AspectBlueBaseShieldGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_HEALTH_EXTRA_SHIELD"),
					ScalingText(AspectBlueBaseShieldGain.Value, AspectBlueStackShieldGain.Value, "percent", "cIsHealing", combine)
				);
			}
			if (Compat.EliteReworks.affixBlueEnabled && Compat.EliteReworks.affixBlueOnHit)
			{
				string damageEffectText;

				if (Compat.EliteReworks.overrideAffixBlue)
				{
					damageEffectText = ScalingText(AspectBlueBaseDamage.Value, AspectBlueStackDamage.Value, "percent", "cIsDamage", combine);
				}
				else
				{
					damageEffectText = ScalingText(Compat.EliteReworks.affixBlueDamage, "percent", "cIsDamage");
				}

				if (Compat.EliteReworks.affixBlueScatter)
				{
					output += String.Format(
						TextFragment("SCATTER_BOMB"),
						damageEffectText
					);
				}
				else
				{
					output += String.Format(
						TextFragment("LIGHTNING_BOMB"),
						damageEffectText,
						SecondText(1.5f, "after")
					);
				}
			}
			else
			{
				if (EffectHooks.preventedDefaultOverloadingBomb)
				{
					if (AspectBlueBaseDamage.Value > 0f)
					{
						output += String.Format(
							TextFragment("LIGHTNING_BOMB"),
							ScalingText(AspectBlueBaseDamage.Value, AspectBlueStackDamage.Value, "percent", "cIsDamage", combine),
							SecondText(AspectBlueBombDuration.Value, "after")
						);
					}
				}
				else
				{
					output += String.Format(
						TextFragment("LIGHTNING_BOMB"),
						ScalingText(0.5f, "percent", "cIsDamage"),
						SecondText(1.5f, "after")
					);
				}
			}

			return output;
		}
	}



	public static class ZetAspectRed
	{
		public static string identifier = "ZetAspectRed";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Damage, ItemTag.Utility };
			if (AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = Catalog.WhiteAspectPrefab;
			itemDef.pickupIconSprite = Catalog.Sprites.AffixUnknown;

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "RED";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixRed;
				if (equipDef)
				{
					RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
					RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
					RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
				}
			}

			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_FIRE");
			if (AspectRedTrail.Value)
			{
				output += TextFragment("PASSIVE_FIRE_TRAIL");
			}
			if (AspectRedExtraJump.Value)
			{
				output += TextFragment("STAT_EXTRA_JUMP");
			}
			if (AspectRedBaseMovementGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_MOVESPEED"),
					ScalingText(AspectRedBaseMovementGain.Value, AspectRedStackMovementGain.Value, "percent", "cIsUtility", combine)
				);
			}
			if (AspectRedBaseBurnDamage.Value > 0f)
			{
				output += String.Format(
					TextFragment("BURN_DOT"),
					ScalingText(AspectRedBaseBurnDamage.Value, AspectRedStackBurnDamage.Value, "percent", "cIsDamage", combine),
					AspectRedUseBase.Value ? TextFragment("BASE_DAMAGE") : TextFragment("TOTAL_DAMAGE"),
					SecondText(AspectRedBurnDuration.Value, "over")
				);
			}

			return output;
		}
	}



	public static class ZetAspectHaunted
	{
		public static string identifier = "ZetAspectHaunted";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };
			if (AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = Catalog.WhiteAspectPrefab;
			itemDef.pickupIconSprite = Catalog.Sprites.AffixUnknown;

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "HAUNTED";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixHaunted;
				if (equipDef)
				{
					RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
					RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
					RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
				}
			}

			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_INCORPOREALITY");
			if (!Compat.EliteReworks.affixHauntedEnabled)
			{
				output += TextFragment("PASSIVE_GHOST_AURA");
			}
			else
			{
				output += TextFragment("PASSIVE_POSSESS");
			}
			bool slowIsDisabled = !AspectHauntedSlowEffect.Value || (Compat.EliteReworks.affixHauntedEnabled && Compat.EliteReworks.affixHauntedOnHit);
			if (!slowIsDisabled)
			{
				output += String.Format(TextFragment("CHILL_ON_HIT"), SecondText(AspectWhiteSlowDuration.Value, "for"));
			}
			if (!Compat.EliteReworks.affixHauntedEnabled || !Compat.EliteReworks.affixHauntedOnHit)
			{
				if (AspectHauntedShredDuration.Value > 0f)
				{
					output += String.Format(
						TextFragment("SHRED_ON_HIT"),
						SecondText(AspectHauntedShredDuration.Value, "for"),
						ScalingText(Mathf.Abs(AspectHauntedShredArmor.Value), "flat", "cIsDamage")
					);
				}
			}
			else
			{
				output += String.Format(
					TextFragment("SHRED_ON_HIT"),
					SecondText(3f, "for"),
					ScalingText(20f, "flat", "cIsDamage")
				);
			}
			if (AspectHauntedBaseArmorGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_ARMOR"),
					ScalingText(AspectHauntedBaseArmorGain.Value, AspectHauntedStackArmorGain.Value, "flat", "cIsHealing", combine)
				);
			}
			if (!Compat.EliteReworks.affixHauntedEnabled && AspectHauntedAllyArmorGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("GHOST_ARMOR"),
					ScalingText(AspectHauntedAllyArmorGain.Value, "flat", "cIsHealing")
				);
			}
			bool dodgeEnabled = false;
			if (AspectHauntedBaseDodgeGain.Value > 0f)
			{
				dodgeEnabled = true;

				output += String.Format(
					TextFragment("DODGE_CHANCE"),
					ScalingText(AspectHauntedBaseDodgeGain.Value, AspectHauntedStackDodgeGain.Value, "chance", "cIsHealing", combine)
				);
			}
			if (!Compat.EliteReworks.affixHauntedEnabled && AspectHauntedAllyDodgeGain.Value > 0f)
			{
				dodgeEnabled = true;

				output += String.Format(
					TextFragment("GHOST_DODGE"),
					ScalingText(AspectHauntedAllyDodgeGain.Value, "chance", "cIsHealing")
				);
			}

			if (dodgeEnabled)
			{
				output += TextFragment("DODGE_DETAIL");
			}

			return output;
		}
	}



	public static class ZetAspectPoison
	{
		public static string identifier = "ZetAspectPoison";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };
			if (AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = Catalog.WhiteAspectPrefab;
			itemDef.pickupIconSprite = Catalog.Sprites.AffixUnknown;

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "POISON";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixPoison;
				if (equipDef)
				{
					RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
					RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
					RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
				}
			}

			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			float ruinDuration = AspectPoisonNullDuration.Value;

			string output = TextFragment("ASPECT_OF_CORRUPTION");
			if (AspectPoisonFireSpikes.Value)
			{
				output += TextFragment("PASSIVE_SPIKEBALL");
			}
			if (Compat.EliteReworks.affixPoisonEnabled)
			{
				output += TextFragment("PASSIVE_RUIN_AURA");
				ruinDuration = 8f;
			}
			if (ruinDuration > 0f)
			{
				if (AspectPoisonNullDamageTaken.Value != 0f)
				{
					output += String.Format(
						TextFragment("RUIN_ON_HIT"),
						SecondText(ruinDuration, "for"),
						ScalingText(Mathf.Abs(AspectPoisonNullDamageTaken.Value), "percent", "cIsDamage")
					);
					output += TextFragment("RUIN_DETAIL");
				}
				else
				{
					output += String.Format(
						TextFragment("RUIN_ON_HIT_BASIC"),
						SecondText(ruinDuration, "for")
					);
				}
			}
			if (Compat.EliteReworks.affixPoisonEnabled)
			{
				output += String.Format(
					TextFragment("WEAKEN_ON_HIT"),
					SecondText(ruinDuration, "for")
				);
			}
			if (AspectPoisonBaseHealthGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_HEALTH"),
					ScalingText(AspectPoisonBaseHealthGain.Value, AspectPoisonStackHealthGain.Value, "flat", "cIsHealing", combine)
				);
			}

			return output;
		}
	}



	public static class ZetAspectLunar
	{
		public static string identifier = "ZetAspectLunar";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };
			if (AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = Catalog.WhiteAspectPrefab;
			itemDef.pickupIconSprite = Catalog.Sprites.AffixUnknown;

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "LUNAR";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixLunar;
				if (equipDef)
				{
					RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
					RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
					RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
				}
			}

			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_PERFECTION");
			if (AspectLunarProjectiles.Value)
			{
				output += TextFragment("PASSIVE_LUNAR_PROJ");
			}
			output += String.Format(TextFragment("CRIPPLE_ON_HIT"), SecondText(AspectLunarCrippleDuration.Value, "for"));
			if (AspectLunarBaseMovementGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_MOVESPEED"),
					ScalingText(AspectLunarBaseMovementGain.Value, AspectLunarStackMovementGain.Value, "percent", "cIsUtility", combine)
				);
			}
			output += String.Format(
				TextFragment("CONVERT_SHIELD"),
				ScalingText(1f, "percent", "cIsHealing")
			);
			if (AspectLunarBaseShieldGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("EXTRA_SHIELD_CONVERT"),
					ScalingText(AspectLunarBaseShieldGain.Value, AspectLunarStackShieldGain.Value, "percent", "cIsHealing", combine)
				);
			}
			if (AspectLunarRegen.Value > 0f)
			{
				output += String.Format(
					TextFragment("CONVERT_SHIELD_REGEN"),
					ScalingText(AspectLunarRegen.Value, "percent", "cIsHealing")
				);
			}

			return output;
		}
	}



	public static class ZetAspectEarth
	{
		public static string identifier = "ZetAspectEarth";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };
			if (AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = Catalog.WhiteAspectPrefab;
			itemDef.pickupIconSprite = Catalog.Sprites.AffixUnknown;

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "EARTH";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixEarth;
				if (equipDef)
				{
					RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
					RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
					RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
				}
			}

			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			bool leechEffect = false;
			bool advLeechDesc = false;

			string output = TextFragment("ASPECT_OF_EARTH");
			output += TextFragment("PASSIVE_HEAL_ALLY");
			if (AspectEarthRegeneration.Value > 0)
			{
				output += String.Format(
					TextFragment("STAT_REGENERATION"),
					ScalingText(AspectEarthRegeneration.Value, 0f, "percentregen", "cIsHealing", combine)
				);
			}
			if (AspectEarthPoachedDuration.Value > 0f)
			{
				float atkSpd = Mathf.Abs(AspectEarthPoachedAttackSpeed.Value);
				float poachLeech = AspectEarthPoachedLeech.Value;

				if (atkSpd != 0f)
				{
					output += String.Format(
						TextFragment("POACH_ON_HIT"),
						SecondText(AspectEarthPoachedDuration.Value, "for"),
						ScalingText(atkSpd, "percent", "cIsUtility")
					);
					if (poachLeech > 0f)
					{
						leechEffect = true;

						output += String.Format(
							TextFragment("POACH_DETAIL"),
							ScalingText(poachLeech, "percent")
						);
					}
				}
				else
				{
					leechEffect = true;

					output += String.Format(
						TextFragment("POACH_ON_HIT_BASIC"),
						SecondText(AspectEarthPoachedDuration.Value, "for"),
						ScalingText(poachLeech, "percent", "cIsHealing")
					);
				}
			}
			if (AspectEarthBaseLeech.Value > 0)
			{
				leechEffect = true;

				output += String.Format(
					TextFragment("HEAL_PERCENT_ON_HIT"),
					ScalingText(AspectEarthBaseLeech.Value, AspectEarthStackLeech.Value, "percent", "cIsHealing", combine)
				);

				if (AspectEarthStackLeech.Value > 0f)
				{
					advLeechDesc = true;
				}
			}
			if (leechEffect)
			{
				int leechMod = AspectEarthLeechModifier.Value;
				if (leechMod > 0 && leechMod < 3)
				{
					float modMult = AspectEarthLeechModMult.Value;
					float postModMult = AspectEarthLeechPostModMult.Value;

					output += "\n" + String.Format(
						TextFragment("LEECH_MODIFIER_FORMULA"),
						advLeechDesc ? "(tl / bl) * " : "",
						leechMod == 1 ? "POW" : "LOG",
						modMult == 1f ? "" : (" * " + modMult),
						AspectEarthLeechParameter.Value,
						postModMult == 1f ? "" : (" * " + postModMult)
					);
				}
			}

			return output;
		}
	}



	public static class ZetAspectVoid
	{
		public static string identifier = "ZetAspectVoid";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Damage, ItemTag.Utility };
			if (AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			if (AspectVoidContagiousItem.Value)
			{
				Catalog.AssignDepricatedTier(itemDef, AspectRedTier.Value ? ItemTier.VoidTier3 : ItemTier.VoidBoss);
			}
			else
			{
				itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			}
			itemDef.pickupModelPrefab = Catalog.Prefabs.AffixVoid;
			itemDef.pickupIconSprite = Catalog.Sprites.AffixUnknown;

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "VOID";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				string corruptText = AspectVoidContagiousItem.Value ? TextFragment("CORRUPT_ASPECT_ITEM", true) : "";
				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP") + " " + corruptText);

				string desc = BuildDescription(false);
				corruptText = AspectVoidContagiousItem.Value ? TextFragment("CORRUPT_ASPECT_ITEM") : "";
				RegisterToken("ITEM_" + locToken + "_DESC", desc + corruptText);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixVoid;
				if (equipDef)
				{
					RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
					RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
					RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
				}
			}

			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_VOID");
			output += TextFragment("PASSIVE_BLOCK");
			if (AspectVoidBaseHealthGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_HEALTH"),
					ScalingText(AspectVoidBaseHealthGain.Value, AspectVoidStackHealthGain.Value, "percent", "cIsHealing", combine)
				);
			}
			if (AspectVoidBaseDamageGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_DAMAGE"),
					ScalingText(AspectVoidBaseDamageGain.Value, AspectVoidStackDamageGain.Value, "percent", "cIsDamage", combine)
				);
			}
			if (Compat.EliteReworks.eliteVoidEnabled)
			{
				output += String.Format(
					TextFragment("NULLIFY_ON_HIT"),
					SecondText(8f, "for")
				);
				output += TextFragment("NULLIFY_DETAIL");
			}
			else
			{
				if (EffectHooks.preventedDefaultVoidCollapse)
				{
					if (AspectVoidBaseCollapseDamage.Value > 0f)
					{
						output += String.Format(
							TextFragment("COLLAPSE_DOT"),
							ScalingText(AspectVoidBaseCollapseDamage.Value, AspectVoidStackCollapseDamage.Value, "percent", "cIsDamage", combine),
							AspectVoidUseBase.Value ? TextFragment("BASE_DAMAGE") : TextFragment("TOTAL_DAMAGE"),
							SecondText(3f, "after")
						);
					}
				}
				else
				{
					output += TextFragment("COLLAPSE_DEFAULT");
				}
			}

			return output;
		}
	}
}
