using RoR2;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class ZetAspectSanguine
	{
		public static string identifier = "ZetAspectSanguine";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Damage, ItemTag.Utility };
			if (Configuration.AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			Sprite outlineSprite;
			if (Configuration.AspectRedTier.Value) outlineSprite = ZetAspectsContent.Sprites.OutlineRed;
			else outlineSprite = ZetAspectsContent.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixSanguine, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Bloody Fealty");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of the red plane.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("AETHERIUM_ELITE_EQUIPMENT_AFFIX_SANGUINE_DESCRIPTION", Language.EquipmentDescription(desc, "Teleport dash and gain brief invulnurability on use.", true));
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of The Red Plane</style> :";
			if (AetheriumHooks.bleedHook)
			{
				if (Configuration.AspectSanguineBaseDamage.Value > 0f)
				{
					output += "\nAttacks <style=cIsDamage>bleed</style> on hit for <style=cIsDamage>";
					output += Configuration.AspectSanguineBaseDamage.Value * 100f + "%</style>";
					if (Configuration.AspectSanguineStackDamage.Value != 0f)
					{
						output += " " + Language.StackText(Configuration.AspectSanguineStackDamage.Value * 100f, "", "%");
					}
					output += " base damage over ";
					output += Language.SecondText(Configuration.AspectSanguineBleedDuration.Value) + ".";
				}
			}
			else
			{
				output += "\nAttacks <style=cIsDamage>bleed</style> on hit for <style=cIsDamage>240%</style> base damage over 3 seconds.";
			}
			if (Configuration.AspectSanguineBaseDotAmp.Value > 0f)
			{
				output += "\nIncreases <style=cIsDamage>damage over time multiplier</style> by <style=cIsDamage>";
				output += Configuration.AspectSanguineBaseDotAmp.Value * 100f + "%</style>";
				if (Configuration.AspectSanguineStackDotAmp.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectSanguineStackDotAmp.Value * 100f, "", "%");
				}
				output += ".";
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of The Red Plane</style> :";
			if (AetheriumHooks.bleedHook)
			{
				if (Configuration.AspectSanguineBaseDamage.Value > 0f)
				{
					value = Configuration.AspectSanguineBaseDamage.Value + Configuration.AspectSanguineStackDamage.Value * (stacks - 1f);
					output += "\nAttacks <style=cIsDamage>bleed</style> on hit for <style=cIsDamage>";
					output += value * 100f + "%</style>";
					output += " base damage over ";
					output += Language.SecondText(Configuration.AspectSanguineBleedDuration.Value) + ".";
				}
			}
			else
			{
				output += "\nAttacks <style=cIsDamage>bleed</style> on hit for <style=cIsDamage>240%</style> base damage over 3 seconds.";
			}
			if (Configuration.AspectSanguineBaseDotAmp.Value > 0f)
			{
				value = Configuration.AspectSanguineBaseDotAmp.Value + Configuration.AspectSanguineStackDotAmp.Value * (stacks - 1f);
				output += "\nIncreases <style=cIsDamage>damage over time multiplier</style> by <style=cIsDamage>";
				output += value * 100f + "%</style>.";
			}

			return output;
		}
	}
}
