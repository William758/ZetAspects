using System;
using UnityEngine;
using RoR2;

using static TPDespair.ZetAspects.Configuration;
using static TPDespair.ZetAspects.Language;

namespace TPDespair.ZetAspects.Items
{
	public static class ZetAspectNullifier
	{
		public static string identifier = "ZetAspectNullifier";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };
			if (AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			Sprite outlineSprite;
			if (AspectRedTier.Value) outlineSprite = Catalog.Sprites.NullOutlineRed;
			else outlineSprite = Catalog.Sprites.NullOutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixNullifier, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "NULLIFIER";

			targetFragmentLanguage = "default";
			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_"+ affix +"_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

			EquipmentDef equipDef = Catalog.Equip.AffixNullifier;
			if (equipDef)
			{
				RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
				RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
			}
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_NULL");

			output += TextFragment("PASSIVE_NULL_AURA");
			/*
			float moveValue = Compat.WarWisp.GetMovementMult();
			if (moveValue != 1f)
			{
				output += String.Format(
					TextFragment("NULL_ON_HIT_SPD"),
					ScalingText(1f - moveValue, "percent", "cIsUtility")
				);
			}
			else
			{
				output += TextFragment("NULL_ON_HIT");
			}
			*/
			float blockValue = Compat.WarWisp.GetBlockChance();
			if (blockValue != -1000000f)
			{
				if (blockValue > 0f)
				{
					output += String.Format(
						TextFragment("BLOCK_CHANCE"),
						ScalingText(blockValue, "chance", "cIsHealing")
					);
					output += TextFragment("BLOCK_DETAIL");
				}
			}
			else
			{
				output += TextFragment("BLOCK_CHANCE_UNKNOWN");
			}

			if (Compat.WarWisp.shieldOverrideHook)
			{
				if (AspectNullifierHealthConverted.Value > 0f)
				{
					output += String.Format(
						TextFragment("CONVERT_SHIELD"),
						ScalingText(AspectNullifierHealthConverted.Value, "percent", "cIsHealing")
					);
				}

				if (AspectNullifierBaseShieldGain.Value > 0f)
				{
					output += String.Format(
						TextFragment("STAT_HEALTH_EXTRA_SHIELD"),
						ScalingText(AspectNullifierBaseShieldGain.Value, AspectNullifierStackShieldGain.Value, "percent", "cIsHealing", combine)
					);
				}
			}
			else
			{
				output += TextFragment("LARGE_SHIELD_UNKNOWN");
			}

			if (AspectNullifierRegen.Value > 0f)
			{
				output += String.Format(
					TextFragment("CONVERT_SHIELD_REGEN"),
					ScalingText(AspectNullifierRegen.Value, "percent", "cIsHealing")
				);
			}

			if (!Compat.WarWisp.shieldRecoveryHook)
			{
				output += TextFragment("DISABLE_OOD_SHIELD_RECOVERY");
			}

			if (AspectNullifierBaseArmorGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_ARMOR"),
					ScalingText(AspectNullifierBaseArmorGain.Value, AspectNullifierStackArmorGain.Value, "flat", "cIsHealing", combine)
				);
			}

			float armorValue = Compat.WarWisp.GetAllyArmor();
			if (armorValue != -1000000f)
			{
				if (AspectNullifierOverrideAllyArmor.Value)
				{
					armorValue = AspectNullifierAllyArmorGain.Value;
				}

				if (armorValue != 0f)
				{
					output += String.Format(
						TextFragment("NEARBY_ARMOR"),
						ScalingText(armorValue, "flat", "cIsHealing")
					);
				}
			}
			else
			{
				output += TextFragment("NEARBY_ARMOR_UNKNOWN");
			}

			return output;
		}
	}
}
