using System;
using UnityEngine;
using RoR2;

using static TPDespair.ZetAspects.Configuration;
using static TPDespair.ZetAspects.Language;

namespace TPDespair.ZetAspects.Items
{
	public static class ZetAspectPlated
	{
		public static string identifier = "ZetAspectPlated";

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
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixPlated, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetFragmentLanguage = "default";
			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_PLATED_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_PLATED_PICKUP"));
			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXPLATED_DESCRIPTION", EquipmentDescription(desc, TextFragment("AFFIX_PLATED_ACTIVE")));
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_ENDURANCE");
			output += TextFragment("PASSIVE_DEFENSE_PLATING");
			output += String.Format(
				TextFragment("DAMAGEREDUCTION_ON_HIT"),
				SecondText(8f, "for")
			);
			if (AspectPlatedBaseArmorGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_ARMOR"),
					ScalingText(AspectPlatedBaseArmorGain.Value, AspectPlatedStackArmorGain.Value, "flat", "cIsHealing", combine)
				);
			}

			return output;
		}
	}



	public static class ZetAspectWarped
	{
		public static string identifier = "ZetAspectWarped";

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
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixWarped, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetFragmentLanguage = "default";
			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_WARPED_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_WARPED_PICKUP"));

			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXWARPED_DESCRIPTION", EquipmentDescription(desc, TextFragment("AFFIX_WARPED_ACTIVE")));
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_GRAVITY");
			output += TextFragment("PASSIVE_DEFLECT_PROJ");
			output += String.Format(
				TextFragment("LEVITATE_ON_HIT"),
				SecondText(4f, "for")
			);
			if (AspectWarpedBaseCooldownGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_COOLDOWN"),
					ScalingText(AspectWarpedBaseCooldownGain.Value, AspectWarpedStackCooldownGain.Value, "percent", "cIsUtility", combine)
				);
			}

			return output;
		}
	}
}
