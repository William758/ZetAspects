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
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("LIT_EQUIP_AFFIXLEECHING_DESC", Language.EquipmentDescription(desc, "Restore nearby allies's health."));
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
				float value = LostInTransitHooks.leechMatchedIndex == 1 ? 100f : 50f;
				output += "\n<style=cIsHealing>Heal</style> for <style=cIsHealing>" + value + "%</style> of the <style=cIsDamage>damage</style> you deal.";
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Parasitism</style> :";
			output += "\nPeriodically heal nearby allies.";
			if (LostInTransitHooks.leechHook)
			{
				if (Configuration.AspectLeechingBaseLeechGain.Value > 0)
				{
					value = Configuration.AspectLeechingBaseLeechGain.Value + Configuration.AspectLeechingStackLeechGain.Value * (stacks - 1f);
					output += "\n<style=cIsHealing>Heal</style> for <style=cIsHealing>";
					output += value * 100f + "%</style>";
					output += " of the <style=cIsDamage>damage</style> you deal.";
				}
			}
			else
			{
				float value2 = LostInTransitHooks.leechMatchedIndex == 1 ? 100f : 50f;
				output += "\n<style=cIsHealing>Heal</style> for <style=cIsHealing>" + value2 + "%</style> of the <style=cIsDamage>damage</style> you deal.";
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
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("LIT_EQUIP_AFFIXFRENZIED_DESC", Language.EquipmentDescription(desc, "Reduced Cooldowns on Use."));
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

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

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
					value = frenzyBaseMS + frenzyStackMS * (stacks - 1f);
					output += "\nIncreases <style=cIsUtility>movement speed</style> and <style=cIsUtility>attack speed</style> by <style=cIsUtility>";
					output += value * 100f + "%</style>.";
				}
				else
				{
					if (LostInTransitHooks.frenzyMSHook)
					{
						if (frenzyBaseMS > 0f)
						{
							value = frenzyBaseMS + frenzyStackMS * (stacks - 1f);
							output += "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>";
							output += value * 100f + "%</style>.";
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
							value = frenzyBaseAS + frenzyStackAS * (stacks - 1f);
							output += "\nIncreases <style=cIsUtility>attack speed</style> by <style=cIsUtility>";
							output += value * 100f + "%</style>.";
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
					value = Mathf.Abs(Configuration.AspectFrenziedBaseCooldownGain.Value) + Mathf.Abs(Configuration.AspectFrenziedStackCooldownGain.Value) * (stacks - 1f);
					output += "\nReduces <style=cIsUtility>skill cooldowns</style> by <style=cIsUtility>";
					output += value * 100f + "%</style>.";
				}
			}
			else
			{
				output += "\nGreatly Reduces <style=cIsUtility>skill cooldowns</style>.";
			}

			return output;
		}
	}

	public static class ZetAspectVolatile
	{
		public static string identifier = "ZetAspectVolatile";

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
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixVolatile, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Lingering Torment");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of instability.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("LIT_EQUIP_AFFIXVOLATILE_DESC", Language.EquipmentDescription(desc, "Charge a Mini Explosion that harms nearby enemies."));
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Instability</style> :";
			output += "\nOccasionally drop Spite bombs when damaged.";

			if (LostInTransitHooks.explodeHook)
			{
				if (Configuration.AspectVolatileBaseDamage.Value > 0)
				{
					output += "\nAttacks <style=cIsDamage>explode</style> on hit, dealing <style=cIsDamage>";
					output += Configuration.AspectVolatileBaseDamage.Value * 100f + "%</style>";
					if (Configuration.AspectVolatileStackDamage.Value != 0)
					{
						output += " " + Language.StackText(Configuration.AspectVolatileStackDamage.Value * 100f, "", "%");
					}
					output += " TOTAL damage.";
				}
			}
			else
			{
				output += "\nAttacks <style=cIsDamage>explode</style> on hit, dealing <style=cIsDamage>30%</style> TOTAL damage.";
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Instability</style> :";
			output += "\nOccasionally drop Spite bombs when damaged.";

			if (LostInTransitHooks.explodeHook)
			{
				if (Configuration.AspectVolatileBaseDamage.Value > 0)
				{
					value = Configuration.AspectVolatileBaseDamage.Value + Configuration.AspectVolatileStackDamage.Value * (stacks - 1f);
					output += "\nAttacks <style=cIsDamage>explode</style> on hit, dealing <style=cIsDamage>";
					output += value * 100f + "%</style>";
					output += " TOTAL damage.";
				}
			}
			else
			{
				output += "\nAttacks <style=cIsDamage>explode</style> on hit, dealing <style=cIsDamage>30%</style> TOTAL damage.";
			}

			return output;
		}
	}

	public static class ZetAspectBlighted
	{
		public static string identifier = "ZetAspectBlighted";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Damage, ItemTag.Utility };
			if (Configuration.AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 4);
				tags[3] = ItemTag.WorldUnique;
			}

			Sprite outlineSprite;
			if (Configuration.AspectRedTier.Value) outlineSprite = ZetAspectsContent.Sprites.OutlineRed;
			else outlineSprite = ZetAspectsContent.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixBlighted, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Betrayal of the Bulwark");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become another misguided soul.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("LIT_EQUIP_AFFIXBLIGHTED_DESC", Language.EquipmentDescription(desc, "Reshuffle your 2 Elite Buffs."));
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Misguided Soul</style> :";
			output += "\nGain 2 random Elite Affixes.";

			if (LostInTransitHooks.blightStatControl)
			{
				if (Configuration.AspectBlightedBaseHealthGain.Value > 0f)
				{
					output += "\nIncreases <style=cIsHealing>maximum health</style> by <style=cIsHealing>";
					output += Configuration.AspectBlightedBaseHealthGain.Value * 100f + "%</style>";
					if (Configuration.AspectBlightedStackHealthGain.Value != 0f)
					{
						output += " " + Language.StackText(Configuration.AspectBlightedStackHealthGain.Value * 100f, "", "%");
					}
					output += ".";
				}
				if (Configuration.AspectBlightedBaseDamageGain.Value > 0f)
				{
					output += "\nIncreases <style=cIsDamage>damage</style> by <style=cIsDamage>";
					output += Configuration.AspectBlightedBaseDamageGain.Value * 100f + "%</style>";
					if (Configuration.AspectBlightedStackDamageGain.Value != 0f)
					{
						output += " " + Language.StackText(Configuration.AspectBlightedStackDamageGain.Value * 100f, "", "%");
					}
					output += ".";
				}
			}

			if (LostInTransitHooks.blightCDRHook == 4)
			{
				if (Configuration.AspectBlightedBaseCooldownGain.Value != 0f)
				{
					output += "\nReduces <style=cIsUtility>skill cooldowns</style> by <style=cIsUtility>";
					output += Mathf.Abs(Configuration.AspectBlightedBaseCooldownGain.Value) * 100f + "%</style>";
					if (Configuration.AspectBlightedStackCooldownGain.Value != 0f)
					{
						output += " " + Language.StackText(Mathf.Abs(Configuration.AspectBlightedStackCooldownGain.Value) * 100f, "", "%");
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

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Misguided Soul</style> :";
			output += "\nGain 2 random Elite Affixes.";

			if (LostInTransitHooks.blightStatControl)
			{
				if (Configuration.AspectBlightedBaseHealthGain.Value > 0f)
				{
					value = Configuration.AspectBlightedBaseHealthGain.Value + Configuration.AspectBlightedStackHealthGain.Value * (stacks - 1f);
					output += "\nIncreases <style=cIsHealing>maximum health</style> by <style=cIsHealing>";
					output += value * 100f + "%</style>.";
				}
				if (Configuration.AspectBlightedBaseDamageGain.Value > 0f)
				{
					value = Configuration.AspectBlightedBaseDamageGain.Value + Configuration.AspectBlightedStackDamageGain.Value * (stacks - 1f);
					output += "\nIncreases <style=cIsDamage>damage</style> by <style=cIsDamage>";
					output += value * 100f + "%</style>.";
				}
			}
			else
			{
				output += "\nGreatly Increases <style=cIsUtility>stats</style>.";
			}

			if (LostInTransitHooks.blightCDRHook == 4)
			{
				if (Configuration.AspectBlightedBaseCooldownGain.Value != 0f)
				{
					value = Mathf.Abs(Configuration.AspectBlightedBaseCooldownGain.Value) + Mathf.Abs(Configuration.AspectBlightedStackCooldownGain.Value) * (stacks - 1f);
					output += "\nReduces <style=cIsUtility>skill cooldowns</style> by <style=cIsUtility>";
					output += value * 100f + "%</style>.";
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
