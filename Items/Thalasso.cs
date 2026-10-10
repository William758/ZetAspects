using System;
using UnityEngine;
using RoR2;

using static TPDespair.ZetAspects.Configuration;
using static TPDespair.ZetAspects.Language;

namespace TPDespair.ZetAspects.Items
{
	public static class ZetAspectPurity
	{
		public static string identifier = "ZetAspectPurity";

		internal static ItemDef DefineItem()
		{
			return Catalog.CreateItemTemplate(identifier, false, true);
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "PURITY";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixPurity;
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
			string output = TextFragment("ASPECT_OF_PURITY");
			output += TextFragment("PASSIVE_PURITY");
			output += TextFragment("CLEANSE_ON_HIT");

			if (AspectPurityBaseHealthGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_HEALTH"),
					ScalingText(AspectPurityBaseHealthGain.Value, AspectPurityStackHealthGain.Value, "flat", "cIsHealing", combine)
				);
			}

			if (AspectPurityBaseRegenGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_REGENERATION"),
					ScalingText(AspectPurityBaseRegenGain.Value, AspectPurityStackRegenGain.Value, "flatregen", "cIsHealing", combine)
				);
			}

			return output;
		}
	}
}
