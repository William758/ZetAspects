using System;
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

			Sprite outlineSprite;
			if (AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixWhite, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetFragmentLanguage = "default";
			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_WHITE_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_WHITE_PICKUP"));
			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXWHITE_DESC", EquipmentDescription(desc, TextFragment("AFFIX_WHITE_ACTIVE")));
			/*
			Language.helperTarget = "tr";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Acıtan Kucaklaması", "tr");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Buzun bir yüzü ol.", "tr");
			*/
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_ICE");
			output += String.Format(TextFragment("CHILL_ON_HIT"), SecondText(AspectWhiteSlowDuration.Value, "for"));
			if (AspectWhiteBaseFreezeChance.Value > 0f)
			{
				output += String.Format(
					TextFragment("CHANCE_TO_FREEZE"),
					ScalingText(AspectWhiteBaseFreezeChance.Value * 0.01f, AspectWhiteStackFreezeChance.Value * 0.01f, "percent", "cIsUtility", combine),
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

			Sprite outlineSprite;
			if (AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixBlue");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixBlue, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetFragmentLanguage = "default";
			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_BLUE_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_BLUE_PICKUP"));

			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXBLUE_DESC", EquipmentDescription(desc, TextFragment("AFFIX_BLUE_ACTIVE")));
			/*
			Language.helperTarget = "tr";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "İki Darbe Arası Sessizlik", "tr");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Yıldırımın bir yüzü ol.", "tr");
			*/
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
					PercentText(Mathf.Abs(AspectBlueSappedDamage.Value), "cIsUtility")
				);
			}
			if (!Compat.EliteReworks.affixBlueEnabled || !Compat.EliteReworks.affixBlueRemoveShield)
			{
				if (AspectBlueHealthConverted.Value > 0f)
				{
					output += String.Format(
						TextFragment("CONVERT_SHIELD"),
						PercentText(AspectBlueHealthConverted.Value, "cIsHealing")
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
				if (Compat.EliteReworks.affixBlueScatter)
				{
					output += String.Format(
						TextFragment("SCATTER_BOMB"),
						PercentText(Compat.EliteReworks.affixBlueDamage, "cIsDamage")
					);
				}
				else
				{
					output += String.Format(
						TextFragment("LIGHTNING_BOMB"),
						PercentText(Compat.EliteReworks.affixBlueDamage, "cIsDamage"),
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
						PercentText(0.5f, "cIsDamage"),
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

			Sprite outlineSprite;
			if (AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixRed");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixRed, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetFragmentLanguage = "default";
			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_RED_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_RED_PICKUP"));

			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXRED_DESC", EquipmentDescription(desc, TextFragment("AFFIX_RED_ACTIVE")));
			/*
			Language.helperTarget = "tr";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "İfritin Farkı", "tr");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Ateşin bir yüzü ol.", "tr");
			*/
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

			Sprite outlineSprite;
			if (AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixHaunted");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixHaunted, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetFragmentLanguage = "default";
			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_HAUNTED_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_HAUNTED_PICKUP"));

			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXHAUNTED_DESC", EquipmentDescription(desc, TextFragment("AFFIX_HAUNTED_ACTIVE")));
			/*
			Language.helperTarget = "tr";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Hayali Taç", "tr");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Soyutluğun bir yüzü ol.", "tr");
			*/
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
			if (AspectHauntedSlowEffect.Value)
			{
				output += String.Format(TextFragment("CHILL_ON_HIT"), SecondText(AspectWhiteSlowDuration.Value, "for"));
			}
			if (!Compat.EliteReworks.affixHauntedEnabled)
			{
				if (AspectHauntedShredDuration.Value > 0f)
				{
					output += String.Format(
						TextFragment("SHRED_ON_HIT"),
						SecondText(AspectHauntedShredDuration.Value, "for"),
						FlatText(Mathf.Abs(AspectHauntedShredArmor.Value), "cIsDamage")
					);
				}
			}
			else
			{
				output += String.Format(
					TextFragment("SHRED_ON_HIT"),
					SecondText(3f, "for"),
					FlatText(20f, "cIsDamage")
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
					FlatText(AspectHauntedAllyArmorGain.Value, "cIsHealing")
				);
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

			Sprite outlineSprite;
			if (AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixPoison");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixPoison, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetFragmentLanguage = "default";
			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_POISON_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_POISON_PICKUP"));

			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXPOISON_DESC", EquipmentDescription(desc, TextFragment("AFFIX_POISON_ACTIVE")));
			/*
			Language.helperTarget = "tr";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "N'kuhana'nın Cevabı", "tr");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Yozlaşmanın bir yüzü ol.", "tr");
			*/
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
						PercentText(Mathf.Abs(AspectPoisonNullDamageTaken.Value), "cIsDamage")
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

			Sprite outlineSprite;
			if (AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixLunar");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixLunar, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetFragmentLanguage = "default";
			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_LUNAR_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_LUNAR_PICKUP"));

			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXLUNAR_DESC", EquipmentDescription(desc, TextFragment("AFFIX_LUNAR_ACTIVE")));
			/*
			Language.helperTarget = "tr";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Paylaşımlı Tasarım", "tr");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Kusursuzluğun bir yüzü ol.", "tr");
			*/
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
				PercentText(1f, "cIsHealing")
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
					PercentText(AspectLunarRegen.Value, "cIsHealing")
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

			Sprite outlineSprite;
			if (AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixEarth, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetFragmentLanguage = "default";
			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_EARTH_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_EARTH_PICKUP"));

			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXEARTH_DESC", EquipmentDescription(desc, TextFragment("AFFIX_EARTH_ACTIVE")));
		}

		public static string BuildDescription(bool combine)
		{
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
						PercentText(atkSpd, "cIsUtility")
					);
					if (poachLeech > 0f)
					{
						output += String.Format(
							TextFragment("POACH_DETAIL"),
							PercentText(poachLeech)
						);
					}
				}
				else
				{
					output += String.Format(
						TextFragment("POACH_ON_HIT_BASIC"),
						SecondText(AspectEarthPoachedDuration.Value, "for"),
						PercentText(poachLeech, "cIsHealing")
					);
				}
			}
			if (AspectEarthBaseLeech.Value > 0)
			{
				output += String.Format(
					TextFragment("HEAL_PERCENT_ON_HIT"),
					ScalingText(AspectEarthBaseLeech.Value, AspectEarthStackLeech.Value, "percent", "cIsHealing", combine)
				);
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

			Sprite outlineSprite;
			if (AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = Catalog.Prefabs.AffixVoid;
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixVoid, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetFragmentLanguage = "default";
			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_VOID_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_VOID_PICKUP"));

			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXVOID_NAME", TextFragment("AFFIX_VOID_NAME"));
			RegisterToken("EQUIPMENT_AFFIXVOID_PICKUP", TextFragment("AFFIX_VOID_PICKUP"));
			RegisterToken("EQUIPMENT_AFFIXVOID_DESC", EquipmentDescription(desc, TextFragment("AFFIX_VOID_ACTIVE")));
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
