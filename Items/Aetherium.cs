using System;
using UnityEngine;
using RoR2;

using static TPDespair.ZetAspects.Configuration;
using static TPDespair.ZetAspects.Language;

namespace TPDespair.ZetAspects.Items
{
	public static class ZetAspectSanguine
	{
		public static string identifier = "ZetAspectSanguine";

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
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixSanguine, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "SANGUINE";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = Catalog.Equip.AffixSanguine;
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
			string output = TextFragment("ASPECT_OF_REDPLANE");

			if (Compat.Aetherium.bleedHook)
			{
				if (AspectSanguineBaseDamage.Value > 0f)
				{
					output += String.Format(
						TextFragment("BLEED_DOT"),
						ScalingText(AspectSanguineBaseDamage.Value, AspectSanguineStackDamage.Value, "percent", "cIsDamage", combine),
						SecondText(AspectSanguineBleedDuration.Value, "over")
					);
				}
			}
			else
			{
				output += String.Format(
					TextFragment("BLEED_DOT"),
					ScalingText(2.4f, 0f, "percent", "cIsDamage", combine),
					SecondText(3f, "over")
				);
			}

			if (AspectSanguineBaseDotAmp.Value > 0f)
			{
				output += String.Format(
					TextFragment("DOT_AMP"),
					ScalingText(AspectSanguineBaseDotAmp.Value, AspectSanguineStackDotAmp.Value, "percent", "cIsDamage", combine)
				);
			}

			return output;
		}
	}
}
