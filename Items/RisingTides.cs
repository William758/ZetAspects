using System;
using UnityEngine;
using RoR2;

using static TPDespair.ZetAspects.Configuration;
using static TPDespair.ZetAspects.Language;

namespace TPDespair.ZetAspects.Items
{
	public static class ZetAspectBarrier
	{
		public static string identifier = "ZetAspectBarrier";

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
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixBarrier, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "BARRIER";

			targetLanguage = "default";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

			EquipmentDef equipDef = Catalog.Equip.AffixBarrier;
			if (equipDef)
			{
				RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
				RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
			}



			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_UNITY");

			output += TextFragment("PASSIVE_BARRIER_STOP");
			output += TextFragment("FORCE_IMMUNE_BARRIER");

			if (AspectBarrierBaseDamageReductionGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_DAMAGE_TAKEN"),
					ScalingText(AspectBarrierBaseDamageReductionGain.Value, AspectBarrierStackDamageReductionGain.Value, "percent", "cIsHealing", combine)
				);
			}

			if (AspectBarrierBaseBarrierDamageReductionGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_DAMAGE_TAKEN_BARRIER"),
					ScalingText(AspectBarrierBaseBarrierDamageReductionGain.Value, AspectBarrierStackBarrierDamageReductionGain.Value, "percent", "cIsHealing", combine)
				);
			}

			output += TextFragment("RANDOM_DEBUFF_ON_HIT");

			return output;
		}
	}

	public static class ZetAspectBlackHole
	{
		public static string identifier = "ZetAspectBlackHole";

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
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixBlackHole, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "BLACKHOLE";

			targetLanguage = "default";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

			EquipmentDef equipDef = Catalog.Equip.AffixBlackHole;
			if (equipDef)
			{
				RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
				RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
			}



			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_INVASION");

			float cfgValue = Compat.RisingTides.GetConfigValue(Compat.RisingTides.BlackHoleDamageScale, 60f);
			output += String.Format(
					TextFragment("BLACKMARK_ON_HIT"),
					Mathf.Round(cfgValue * 8f) / 10f,
					Mathf.Round(cfgValue * 2f) / 10f
				); ;

			if (AspectBlackHoleBaseDamageGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_DAMAGE"),
					ScalingText(AspectBlackHoleBaseDamageGain.Value, AspectBlackHoleStackDamageGain.Value, "percent", "cIsDamage", combine)
				);
			}

			return output;
		}
	}

	public static class ZetAspectMoney
	{
		public static string identifier = "ZetAspectMoney";

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
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixMoney, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "MONEY";

			targetLanguage = "default";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

			EquipmentDef equipDef = Catalog.Equip.AffixMoney;
			if (equipDef)
			{
				RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
				RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
			}



			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_POLARITY");

			output += TextFragment("PULLDOWN_ON_HIT");
			output += TextFragment("PASSIVE_DRAIN_MONEY");

			if (AspectMoneyBaseGoldMult.Value > 0f)
			{
				output += String.Format(
					TextFragment("GOLD_FROM_KILL"),
					ScalingText(AspectMoneyBaseGoldMult.Value, AspectMoneyStackGoldMult.Value, "percent", "cIsUtility")
				);
			}

			if (AspectMoneyBaseRegenGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_REGENERATION"),
					ScalingText(AspectMoneyBaseRegenGain.Value, AspectMoneyStackRegenGain.Value, "flatregen", "cIsHealing", combine)
				);
			}

			return output;
		}
	}

	public static class ZetAspectNight
	{
		public static string identifier = "ZetAspectNight";

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
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixNight, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "NIGHT";

			targetLanguage = "default";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

			EquipmentDef equipDef = Catalog.Equip.AffixNight;
			if (equipDef)
			{
				RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
				RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
			}



			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_DARKNESS");

			output += String.Format(
				TextFragment("NIGHTBLIND_ON_HIT"),
				SecondText(4f, "for"),
				ScalingText(AspectNightBlindDodgeEffect.Value, "chance", "cIsHealing")
			);

			float cfgValue = Compat.RisingTides.GetConfigValue(Compat.RisingTides.NightBlindRange, 20f);
			output += String.Format(
				TextFragment("NIGHTBLIND_DETAIL"),
				ScalingText(cfgValue, "meter")
			);

			float baseMS = AspectNightBaseMovementGain.Value;
			float stackMS = AspectNightStackMovementGain.Value;
			float baseAS = AspectNightBaseAtkSpdGain.Value;
			float stackAS = AspectNightStackAtkSpdGain.Value;

			if (baseMS == baseAS && stackMS == stackAS)
			{
				if (baseMS > 0f)
				{
					output += String.Format(
						TextFragment("STAT_BOTHSPEED"),
						ScalingText(baseMS, stackMS, "percent", "cIsDamage", combine)
					);
				}
			}
			else
			{
				if (baseMS > 0f)
				{
					output += String.Format(
						TextFragment("STAT_MOVESPEED"),
						ScalingText(baseMS, stackMS, "percent", "cIsUtility", combine)
					);
				}
				if (baseAS > 0f)
				{
					output += String.Format(
						TextFragment("STAT_ATTACKSPEED"),
						ScalingText(baseAS, stackAS, "percent", "cIsDamage", combine)
					);
				}
			}

			baseMS = 1f;
			stackMS = 0f;
			baseAS = 0.3f;
			stackAS = 0f;

			if (Compat.RisingTides.nightSpeedStatHook)
			{
				baseMS = AspectNightBaseSafeMovementGain.Value;
				stackMS = AspectNightStackSafeMovementGain.Value;
				baseAS = AspectNightBaseSafeAtkSpdGain.Value;
				stackAS = AspectNightStackSafeAtkSpdGain.Value;
			}

			if (baseMS == baseAS && stackMS == stackAS)
			{
				if (baseMS > 0f)
				{
					output += String.Format(
						TextFragment("OOD_NIGHT_BOTHSAMESPD"),
						ScalingText(baseMS, stackMS, "percent", "cIsDamage", combine)
					);
				}
			}
			else if (baseMS > 0f && baseAS > 0f)
			{
				output += String.Format(
					TextFragment("OOD_NIGHT_BOTHDIFFSPD"),
					ScalingText(baseMS, stackMS, "percent", "cIsUtility", combine),
					ScalingText(baseAS, stackAS, "percent", "cIsDamage", combine)
				);
			}
			else
			{
				if (baseMS > 0f)
				{
					output += String.Format(
						TextFragment("OOD_NIGHT_MOVSPD"),
						ScalingText(baseMS, stackMS, "percent", "cIsUtility", combine)
					);
				}
				if (baseAS > 0f)
				{
					output += String.Format(
						TextFragment("OOD_NIGHT_ATKSPD"),
						ScalingText(baseAS, stackAS, "percent", "cIsDamage", combine)
					);
				}
			}

			return output;
		}
	}

	public static class ZetAspectWater
	{
		public static string identifier = "ZetAspectWater";

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
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixWater, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "WATER";

			targetLanguage = "default";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

			EquipmentDef equipDef = Catalog.Equip.AffixWater;
			if (equipDef)
			{
				RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
				RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
			}



			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_ALTERABILITY");

			output += TextFragment("PASSIVE_IDLE_INVULN");
			output += TextFragment("BUBBLE_ON_HIT");

			if (AspectWaterBaseCooldownGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_COOLDOWN"),
					ScalingText(AspectWaterBaseCooldownGain.Value, AspectWaterStackCooldownGain.Value, "percent", "cIsUtility", combine)
				);
			}

			return output;
		}
	}

	public static class ZetAspectRealgar
	{
		public static string identifier = "ZetAspectRealgar";

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
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixRealgar, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "REALGAR";

			targetLanguage = "default";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

			EquipmentDef equipDef = Catalog.Equip.AffixRealgar;
			if (equipDef)
			{
				RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
				RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
			}



			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_EXTRINSICALITY");

			output += TextFragment("PASSIVE_RED_FISSURE");
			output += TextFragment("SCAR_ON_HIT");

			if (AspectRealgarBaseDotAmp.Value > 0f)
			{
				output += String.Format(
					TextFragment("DOT_AMP"),
					ScalingText(AspectRealgarBaseDotAmp.Value, AspectRealgarStackDotAmp.Value, "percent", "cIsDamage", combine)
				);
			}

			return output;
		}
	}
}
