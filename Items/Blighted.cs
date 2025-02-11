﻿using System;
using UnityEngine;
using RoR2;

using static TPDespair.ZetAspects.Configuration;
using static TPDespair.ZetAspects.Language;

namespace TPDespair.ZetAspects.Items
{
	public static class ZetAspectBlighted
	{
		public static string identifier = "ZetAspectBlighted";

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
			string affix = "BLIGHTED";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixBlighted;
				if (equipDef)
				{
					RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
					RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
					RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE"), true));
				}
			}

			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_DECAY");
			output += TextFragment("PASSIVE_BLIGHT");
			if (AspectBlightedBaseHealthGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_HEALTH"),
					ScalingText(AspectBlightedBaseHealthGain.Value, AspectBlightedStackHealthGain.Value, "percent", "cIsHealing", combine)
				);
			}
			if (AspectBlightedBaseDamageGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_DAMAGE"),
					ScalingText(AspectBlightedBaseDamageGain.Value, AspectBlightedStackDamageGain.Value, "percent", "cIsDamage", combine)
				);
			}
			if (AspectBlightedBaseCooldownGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_COOLDOWN"),
					ScalingText(AspectBlightedBaseCooldownGain.Value, AspectBlightedStackCooldownGain.Value, "percent", "cIsUtility", combine)
				);
			}

			return output;
		}
	}
}
