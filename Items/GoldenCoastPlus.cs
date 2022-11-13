using System;
using UnityEngine;
using RoR2;

using static TPDespair.ZetAspects.Configuration;
using static TPDespair.ZetAspects.Language;

namespace TPDespair.ZetAspects.Items
{
	public static class ZetAspectGold
	{
		public static string identifier = "ZetAspectGold";

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
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixGold, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetLanguage = "default";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_GOLD_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_GOLD_PICKUP"));
			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXGOLD_DESC", EquipmentDescription(desc, TextFragment("AFFIX_GOLD_ACTIVE")));



			targetLanguage = "pt-BR";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_GOLD_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_GOLD_PICKUP"));
			desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXGOLD_NAME", TextFragment("AFFIX_GOLD_NAME"));
			RegisterToken("EQUIPMENT_AFFIXGOLD_PICKUP", TextFragment("AFFIX_GOLD_PICKUP"));
			RegisterToken("EQUIPMENT_AFFIXGOLD_DESC", EquipmentDescription(desc, TextFragment("AFFIX_GOLD_ACTIVE")));



			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_FORTUNE");
			output += TextFragment("GOLD_ON_HIT");
			if (AspectGoldBaseRegenGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_REGENERATION"),
					ScalingText(AspectGoldBaseRegenGain.Value, AspectGoldStackRegenGain.Value, "flatregen", "cIsHealing", combine)
				);
			}
			if (AspectGoldBaseScoredRegenGain.Value > 0f)
			{
				output += TextFragment("ITEMSCORE_REGEN");
				if (AspectGoldStackScoredRegenGain.Value > 0f)
				{
					output += String.Format(
						TextFragment("ITEMSCORE_REGEN_MULT"),
						ScalingText(1f, AspectGoldStackScoredRegenGain.Value / AspectGoldBaseScoredRegenGain.Value, "percent", "cIsHealing", combine)
					);
				}
			}

			return output;
		}
	}
}
