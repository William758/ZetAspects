using RoR2;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class ZetAspectLeeching
	{
		public static string identifier = "ZetAspectLeeching";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };
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
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixLeeching, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Guttural Whimpers");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of parasitism.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			Language.RegisterToken("LIT_EQUIP_AFFIXLEECHING_DESC", Language.EquipmentDescription(desc, "- ? Activation Effect ? -"));
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Parasitism</style> :";
			output += "\nPeriodically heal nearby allies.";
			if (LostInTransitHooks.leechHook)
			{
				if (Configuration.AspectLeechingBaseLeechGain.Value > 0)
				{
					output += "\n<style=cIsHealing>Heal</style> for <style=cIsHealing>";
					output += Configuration.AspectLeechingBaseLeechGain.Value * 100f + "%</style>";
					if (Configuration.AspectLeechingStackLeechGain.Value != 0)
					{
						output += " " + Language.StackText(Configuration.AspectLeechingStackLeechGain.Value * 100f, "", "%");
					}
					output += " of the <style=cIsDamage>damage</style> you deal.";
				}
			}
			else
			{
				output += "\n<style=cIsHealing>Heal</style> for <style=cIsHealing>100%</style> of the <style=cIsDamage>damage</style> you deal.";
			}

			return output;
		}
	}

	public static class ZetAspectFrenzied
	{
		public static string identifier = "ZetAspectFrenzied";

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
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixFrenzied, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Primordial Rage");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of seething.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			Language.RegisterToken("LIT_EQUIP_AFFIXFRENZIED_DESC", Language.EquipmentDescription(desc, "- ? Activation Effect ? -"));
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Seething</style> :";
			output += "\nPeriodically dash towards enemies.";

			float frenzyBaseMS = Configuration.AspectFrenziedBaseMovementGain.Value;
			float frenzyStackMS = Configuration.AspectFrenziedStackMovementGain.Value;
			float frenzyBaseAS = Configuration.AspectFrenziedBaseAttackSpeedGain.Value;
			float frenzyStackAS = Configuration.AspectFrenziedStackAttackSpeedGain.Value;

			if (LostInTransitHooks.frenzyMSHook || LostInTransitHooks.frenzyASHook)
			{
				if (LostInTransitHooks.frenzyMSHook && LostInTransitHooks.frenzyASHook && frenzyBaseMS == frenzyBaseAS && frenzyStackMS == frenzyStackAS)
				{
					output += "\nIncreases <style=cIsUtility>movement speed</style> and <style=cIsUtility>attack speed</style> by <style=cIsUtility>";
					output += frenzyBaseMS * 100f + "%</style>";
					if (frenzyStackMS != 0f)
					{
						output += " " + Language.StackText(frenzyStackMS * 100f, "", "%");
					}
					output += ".";
				}
				else
				{
					if (LostInTransitHooks.frenzyMSHook)
					{
						if (frenzyBaseMS > 0f)
						{
							output += "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>";
							output += frenzyBaseMS * 100f + "%</style>";
							if (frenzyStackMS != 0f)
							{
								output += " " + Language.StackText(frenzyStackMS * 100f, "", "%");
							}
							output += ".";
						}
					}
					else
					{
						output += "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>100%</style>.";
					}
					if (LostInTransitHooks.frenzyASHook)
					{
						if (frenzyBaseAS > 0f)
						{
							output += "\nIncreases <style=cIsUtility>attack speed</style> by <style=cIsUtility>";
							output += frenzyBaseAS * 100f + "%</style>";
							if (frenzyStackAS != 0f)
							{
								output += " " + Language.StackText(frenzyStackAS * 100f, "", "%");
							}
							output += ".";
						}
					}
					else
					{
						output += "\nIncreases <style=cIsUtility>attack speed</style> by <style=cIsUtility>100%</style>.";
					}
				}
			}
			else
			{
				output += "\nIncreases <style=cIsUtility>movement speed</style> and <style=cIsUtility>attack speed</style> by <style=cIsUtility>100%</style>.";
			}

			if (LostInTransitHooks.frenzyCDRHook == 4)
			{
				if (Configuration.AspectFrenziedBaseCooldownGain.Value != 0f)
				{
					output += "\nReduces <style=cIsUtility>skill cooldowns</style> by <style=cIsUtility>";
					output += Mathf.Abs(Configuration.AspectFrenziedBaseCooldownGain.Value) * 100f + "%</style>";
					if (Configuration.AspectFrenziedStackCooldownGain.Value != 0f)
					{
						output += " " + Language.StackText(Mathf.Abs(Configuration.AspectFrenziedStackCooldownGain.Value) * 100f, "", "%");
					}
					output += ".";
				}
			}
			else
			{
				output += "\nGreatly Reduces <style=cIsUtility>skill cooldowns</style>.";
			}

			return output;
		}
	}
}
