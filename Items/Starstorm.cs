using System;
using UnityEngine;
using RoR2;

using static TPDespair.ZetAspects.Configuration;
using static TPDespair.ZetAspects.Language;

namespace TPDespair.ZetAspects.Items
{
	public static class ZetAspectEmpyrean
	{
		public static string identifier = "ZetAspectEmpyrean";

		internal static ItemDef DefineItem()
		{
			return Catalog.CreateItemTemplate(identifier, true, true);
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "EMPYREAN";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixEmpyrean;
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
			string output = TextFragment("ASPECT_OF_EXISTENCE");
			output += TextFragment("PASSIVE_EMPYREAN");

			output += TextFragment("STUN_IMMUNE");

			if (AspectEmpyreanBaseHealthGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_HEALTH"),
					ScalingText(AspectEmpyreanBaseHealthGain.Value, AspectEmpyreanStackHealthGain.Value, "percent", "cIsHealing", combine)
				);
			}
			if (AspectEmpyreanBaseDamageGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_DAMAGE"),
					ScalingText(AspectEmpyreanBaseDamageGain.Value, AspectEmpyreanStackDamageGain.Value, "percent", "cIsDamage", combine)
				);
			}
			if (AspectEmpyreanBaseCooldownGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_COOLDOWN"),
					ScalingText(AspectEmpyreanBaseCooldownGain.Value, AspectEmpyreanStackCooldownGain.Value, "percent", "cIsUtility", combine)
				);
			}
			
			return output;
		}
	}
}
