using System;
using UnityEngine;
using RoR2;

using static TPDespair.ZetAspects.Configuration;
using static TPDespair.ZetAspects.Language;

namespace TPDespair.ZetAspects.Items
{
	public static class ZetAspectBackup
	{
		public static string identifier = "ZetAspectBackup";

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
			string affix = "BACKUP";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixBackup;
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
			string output = TextFragment("ASPECT_OF_BACKUP");
			output += String.Format(
				TextFragment("BACKUPED_ON_HIT"),
				SecondText(5f, "for")
			);
			output += TextFragment("BACKUPED_DETAIL");

			if (Compat.GOTCE.chargeOverride)
			{
				if (AspectBackupBaseChargesGain.Value > 0)
				{
					output += String.Format(
						TextFragment("STAT_SECONDARY_CHARGE"),
						ScalingText(AspectBackupBaseChargesGain.Value, AspectBackupStackChargesGain.Value, "flat", "cIsUtility", combine)
					);
				}
			}
			else
			{
				output += String.Format(
					TextFragment("STAT_SECONDARY_CHARGE"),
					ScalingText(3, "flat", "cIsUtility")
				);
			}

			if (Compat.GOTCE.backupStatHook)
			{
				if (AspectBackupBaseCooldownGain.Value > 0)
				{
					output += String.Format(
						TextFragment("STAT_SECONDARY_COOLDOWN"),
						ScalingText(AspectBackupBaseCooldownGain.Value, AspectBackupStackCooldownGain.Value, "percent", "cIsUtility", combine)
					);
				}
			}
			else
			{
				output += String.Format(
					TextFragment("STAT_SECONDARY_COOLDOWN"),
					ScalingText(0.5f, "percent", "cIsUtility")
				);
			}

			return output;
		}
	}
}
