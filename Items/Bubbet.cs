using System;
using UnityEngine;
using RoR2;

using static TPDespair.ZetAspects.Configuration;
using static TPDespair.ZetAspects.Language;

namespace TPDespair.ZetAspects.Items
{
	public static class ZetAspectSepia
	{
		public static string identifier = "ZetAspectSepia";

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
			itemDef.pickupModelPrefab = Catalog.Prefabs.AffixSepia;
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixSepia, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetLanguage = "default";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_SEPIA_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_SEPIA_PICKUP"));
			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("BUB_SEPIA_ASPECT_NAME", TextFragment("AFFIX_SEPIA_NAME"));
			RegisterToken("BUB_SEPIA_ASPECT_PICKUP", TextFragment("AFFIX_SEPIA_PICKUP"));
			RegisterToken("BUB_SEPIA_ASPECT_DESC", EquipmentDescription(desc, TextFragment("AFFIX_SEPIA_ACTIVE")));



			targetLanguage = "pt-BR";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_SEPIA_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_SEPIA_PICKUP"));
			desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("BUB_SEPIA_ASPECT_NAME", TextFragment("AFFIX_SEPIA_NAME"));
			RegisterToken("BUB_SEPIA_ASPECT_PICKUP", TextFragment("AFFIX_SEPIA_PICKUP"));
			RegisterToken("BUB_SEPIA_ASPECT_DESC", EquipmentDescription(desc, TextFragment("AFFIX_SEPIA_ACTIVE")));



			targetLanguage = "ko";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_SEPIA_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_SEPIA_PICKUP"));
			desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("BUB_SEPIA_ASPECT_NAME", TextFragment("AFFIX_SEPIA_NAME"));
			RegisterToken("BUB_SEPIA_ASPECT_PICKUP", TextFragment("AFFIX_SEPIA_PICKUP"));
			RegisterToken("BUB_SEPIA_ASPECT_DESC", EquipmentDescription(desc, TextFragment("AFFIX_SEPIA_ACTIVE")));



			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_ILLUSION");

			if (AspectSepiaBaseCooldownGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_COOLDOWN"),
					ScalingText(AspectSepiaBaseCooldownGain.Value, AspectSepiaStackCooldownGain.Value, "percent", "cIsUtility", combine)
				);
			}

			if (AspectSepiaBaseRegenGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_REGENERATION"),
					ScalingText(AspectSepiaBaseRegenGain.Value, AspectSepiaStackRegenGain.Value, "flatregen", "cIsHealing", combine)
				);
			}

			if (AspectSepiaBlindDuration.Value > 0f)
			{
				output += String.Format(
					TextFragment("SEPIABLIND_ON_HIT"),
					SecondText(AspectSepiaBlindDuration.Value, "for"),
					ScalingText(AspectSepiaBlindDodgeEffect.Value, "chance", "cIsHealing")
				);
			}

			if (AspectSepiaBaseDodgeGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("DODGE_CHANCE"),
					ScalingText(AspectSepiaBaseDodgeGain.Value, AspectSepiaStackDodgeGain.Value, "chance", "cIsHealing", combine)
				);

				output += TextFragment("DODGE_DETAIL");
			}

			return output;
		}
	}
}
