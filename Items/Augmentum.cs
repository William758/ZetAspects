using System;
using UnityEngine;
using RoR2;

using static TPDespair.ZetAspects.Configuration;
using static TPDespair.ZetAspects.Language;
using static TPDespair.ZetAspects.Compat.Augmentum;

namespace TPDespair.ZetAspects.Items
{
	public static class ZetAspectAdaptive
	{
		public static string identifier = "ZetAspectAdaptive";

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
			string affix = "ADAPTIVE";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixAdaptive;
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
			string output = TextFragment("ASPECT_OF_EVOLUTION");

			output += String.Format(
				TextFragment("ADAPTIVE_DEFENSE"),
				ScalingText(AdaptivePreHitArmor, "flat", "cIsHealing")
			);

			if (!AdaptiveInvisibility)
			{
				output += String.Format(
					TextFragment("ADAPTIVE_REACT"),
					SecondText(AdaptiveBoostDuration, "for")
				);
			}
			else
			{
				output += String.Format(
					TextFragment("ADAPTIVE_REACT_INVIS"),
					SecondText(AdaptiveInvisibilityDuration, "for"), SecondText(AdaptiveBoostDuration, "for")
				);
			}

			output += String.Format(
				TextFragment("LACERATION_ON_HIT"),
				20, SecondText(AdaptiveLacerationDuration, "for"), ScalingText(0.1f, "flat", "cIsDamage")
			);

			return output;
		}
	}
}
