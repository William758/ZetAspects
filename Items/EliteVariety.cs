using RoR2;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class ZetAspectArmor
	{
		public static string identifier = "ZetAspectArmor";

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
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixArmored, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Service for the Bulwark");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of durability.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("EQUIPMENT_ELITEVARIETY_AFFIXARMORED_DESC", Language.EquipmentDescription(desc, "Gain temporary armor increase on use."));
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Durability</style> :";
			output += "\nSpawn with full barrier";
			if (Catalog.barrierDecayMode == 0) output += " that doesn't decay";
			else if (Catalog.barrierDecayMode == 1) output += " that slowly decays";
			output += ".\nAttacks <style=cIsUtility>root</style> on hit, immobilizing the target.";
			if (Configuration.AspectArmorBaseArmorGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsHealing>armor</style> by <style=cIsHealing>";
				output += Configuration.AspectArmorBaseArmorGain.Value + "</style>";
				if (Configuration.AspectArmorStackArmorGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectArmorStackArmorGain.Value);
				}
				output += ".";
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Durability</style> :";
			output += "\nSpawn with full barrier";
			if (Catalog.barrierDecayMode == 0) output += " that doesn't decay";
			else if (Catalog.barrierDecayMode == 1) output += " that slowly decays";
			output += ".\nAttacks <style=cIsUtility>root</style> on hit, immobilizing the target.";
			if (Configuration.AspectArmorBaseArmorGain.Value > 0f)
			{
				value = Configuration.AspectArmorBaseArmorGain.Value + Configuration.AspectArmorStackArmorGain.Value * (stacks - 1f);
				output += "\nIncreases <style=cIsHealing>armor</style> by <style=cIsHealing>";
				output += value + "</style>.";
			}

			return output;
		}
	}



	public static class ZetAspectBanner
	{
		public static string identifier = "ZetAspectBanner";

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
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixBuffing, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Retaliation of the Planet");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of battle.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("EQUIPMENT_ELITEVARIETY_AFFIXBUFFING_DESC", Language.EquipmentDescription(desc, "Increase banner radius on use."));
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Battle</style> :";
			output += "\nCarry a warbanner that empowers nearby allies.";

			if (Configuration.AspectBannerExtraJump.Value)
			{
				output += "\nGain <style=cIsUtility>+1</style> maximum <style=cIsUtility>jump count</style>.";
			}

			if (Configuration.AspectBannerBaseDamageGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsDamage>damage</style> by <style=cIsDamage>";
				output += Configuration.AspectBannerBaseDamageGain.Value * 100f + "%</style>";
				if (Configuration.AspectBannerStackDamageGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectBannerStackDamageGain.Value * 100f, "", "%");
				}
				output += ".";
			}

			output += "\nGrants you and nearby allies increased <style=cIsUtility>movement speed</style> and <style=cIsDamage>attack speed</style>.";

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Battle</style> :";
			output += "\nCarry a warbanner that empowers nearby allies.";

			if (Configuration.AspectBannerExtraJump.Value)
			{
				output += "\nGain <style=cIsUtility>+1</style> maximum <style=cIsUtility>jump count</style>.";
			}

			if (Configuration.AspectBannerBaseDamageGain.Value > 0f)
			{
				value = Configuration.AspectBannerBaseDamageGain.Value + Configuration.AspectBannerStackDamageGain.Value * (stacks - 1f);
				output += "\nIncreases <style=cIsDamage>damage</style> by <style=cIsDamage>";
				output += value * 100f + "%</style>.";
			}

			output += "\nGrants you and nearby allies increased <style=cIsUtility>movement speed</style> and <style=cIsDamage>attack speed</style>.";

			return output;
		}
	}



	public static class ZetAspectImpale
	{
		public static string identifier = "ZetAspectImpale";

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
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixImpPlane, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Invasion from the Red Plane");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of violence.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("EQUIPMENT_ELITEVARIETY_AFFIXIMPPLANE_DESC", Language.EquipmentDescription(desc, "Teleport to a target and deal damage on use."));
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Violence</style> :";
			output += "\nEnemies looking directly at you are <style=cIsDamage>marked</style>, increasing <style=cIsDamage>damage taken</style>.";
			output += "\nAttacks <style=cIsDamage>impale</style> on hit, periodically dealing heavy damage.";
			if (Configuration.AspectImpaleBaseDotAmp.Value > 0f)
			{
				output += "\nIncreases <style=cIsDamage>damage over time multiplier</style> by <style=cIsDamage>";
				output += Configuration.AspectImpaleBaseDotAmp.Value * 100f + "%</style>";
				if (Configuration.AspectImpaleStackDotAmp.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectImpaleStackDotAmp.Value * 100f, "", "%");
				}
				output += ".";
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Violence</style> :";
			output += "\nEnemies looking directly at you are <style=cIsDamage>marked</style>, increasing <style=cIsDamage>damage taken</style>.";
			output += "\nAttacks <style=cIsDamage>impale</style> on hit, periodically dealing heavy damage.";
			if (Configuration.AspectImpaleBaseDotAmp.Value > 0f)
			{
				value = Configuration.AspectImpaleBaseDotAmp.Value + Configuration.AspectImpaleStackDotAmp.Value * (stacks - 1f);
				output += "\nIncreases <style=cIsDamage>damage over time multiplier</style> by <style=cIsDamage>";
				output += value * 100f + "%</style>.";
			}

			return output;
		}
	}



	public static class ZetAspectGolden
	{
		public static string identifier = "ZetAspectGolden";

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
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixPillaging, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Curse of Greed");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of theft.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("EQUIPMENT_ELITEVARIETY_AFFIXPILLAGING_DESC", Language.EquipmentDescription(desc, "Spend all of your gold for a random item. The more gold spent, the higher chance of getting a rarer item."));
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Theft</style> :";
			output += "\nAttacks steal gold on hit.";
			if (Configuration.AspectGoldenBaseRegenGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsHealing>health regeneration</style> by <style=cIsHealing>";
				output += Configuration.AspectGoldenBaseRegenGain.Value + "hp/s</style>";
				if (Configuration.AspectGoldenStackRegenGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectGoldenStackRegenGain.Value, "", "hp/s");
				}
				output += ".";
			}
			if (Configuration.AspectGoldenBaseScoredRegenGain.Value > 0f)
			{
				output += "\nBonus <style=cIsHealing>health regeneration</style> based on quantity and tier of items owned.";
				output += "\nBonus multiplier of <style=cIsHealing>";
				output += Configuration.AspectGoldenBaseScoredRegenGain.Value * 100f + "%</style>";
				if (Configuration.AspectGoldenStackScoredRegenGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectGoldenStackScoredRegenGain.Value * 100f, "", "%");
				}
				output += ".";
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Theft</style> :";
			output += "\nAttacks steal gold on hit.";
			if (Configuration.AspectGoldenBaseRegenGain.Value > 0f)
			{
				value = Configuration.AspectGoldenBaseRegenGain.Value + Configuration.AspectGoldenStackRegenGain.Value * (stacks - 1f);
				output += "\nIncreases <style=cIsHealing>health regeneration</style> by <style=cIsHealing>";
				output += value + "hp/s</style>.";
			}
			if (Configuration.AspectGoldenBaseScoredRegenGain.Value > 0f)
			{
				output += "\nBonus <style=cIsHealing>health regeneration</style> based on quantity and tier of items owned.";
			}

			return output;
		}
	}



	public static class ZetAspectCyclone
	{
		public static string identifier = "ZetAspectCyclone";

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
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixSandstorm, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "The Third Wish");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of sand.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("EQUIPMENT_ELITEVARIETY_AFFIXSANDSTORM_DESC", Language.EquipmentDescription(desc, "Dash on use, knocking nearby enemies up."));
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Sand</style> :";
			output += "\nSurrounded by a sandstorm that damages enemies on contact.";

			output += "\nAttacks <style=cIsUtility>blind</style> on hit";
			if (Configuration.AspectCycloneBlindDodgeEffect.Value > 0f)
			{
				output += ", reducing visibility and hit-chance.";
			}
			else
			{
				output += ", reducing visibility.";
			}

			if (Configuration.AspectCycloneBaseDodgeGain.Value > 0f)
			{
				output += "\n<style=cIsHealing>";
				output += Configuration.AspectCycloneBaseDodgeGain.Value * 100f + "%</style>";
				if (Configuration.AspectCycloneStackDodgeGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectCycloneStackDodgeGain.Value * 100f, "", "%");
				}
				output += " chance to <style=cIsHealing>dodge</style> incoming damage. <style=cIsUtility>Unaffected by luck</style>.";
			}

			float cycloneBaseMS = Configuration.AspectCycloneBaseMovementGain.Value;
			float cycloneStackMS = Configuration.AspectCycloneStackMovementGain.Value;
			float cycloneBaseAS = Configuration.AspectCycloneBaseAttackSpeedGain.Value;
			float cycloneStackAS = Configuration.AspectCycloneStackAttackSpeedGain.Value;

			if (cycloneBaseMS == cycloneBaseAS && cycloneStackMS == cycloneStackAS)
			{
				output += "\nIncreases <style=cIsUtility>movement speed</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>";
				output += cycloneBaseMS * 100f + "%</style>";
				if (cycloneStackMS != 0f)
				{
					output += " " + Language.StackText(cycloneStackMS * 100f, "", "%");
				}
				output += ".";
			}
			else
			{
				if (cycloneBaseMS > 0f)
				{
					output += "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>";
					output += cycloneBaseMS * 100f + "%</style>";
					if (cycloneStackMS != 0f)
					{
						output += " " + Language.StackText(cycloneStackMS * 100f, "", "%");
					}
					output += ".";
				}
				if (cycloneBaseAS > 0f)
				{
					output += "\nIncreases <style=cIsDamage>attack speed</style> by <style=cIsDamage>";
					output += cycloneBaseAS * 100f + "%</style>";
					if (cycloneStackAS != 0f)
					{
						output += " " + Language.StackText(cycloneStackAS * 100f, "", "%");
					}
					output += ".";
				}
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Sand</style> :";
			output += "\nSurrounded by a sandstorm that damages enemies on contact.";

			output += "\nAttacks <style=cIsUtility>blind</style> on hit";
			if (Configuration.AspectCycloneBlindDodgeEffect.Value > 0f)
			{
				output += ", reducing visibility and hit-chance.";
			}
			else
			{
				output += ", reducing visibility.";
			}

			if (Configuration.AspectCycloneBaseDodgeGain.Value > 0f)
			{
				value = Configuration.AspectCycloneBaseDodgeGain.Value + Configuration.AspectCycloneStackDodgeGain.Value * (stacks - 1f);
				output += "\n<style=cIsHealing>" + value * 100f + "%</style>";
				output += " chance to <style=cIsHealing>dodge</style> incoming damage. <style=cIsUtility>Unaffected by luck</style>.";
			}

			float cycloneBaseMS = Configuration.AspectCycloneBaseMovementGain.Value;
			float cycloneStackMS = Configuration.AspectCycloneStackMovementGain.Value;
			float cycloneBaseAS = Configuration.AspectCycloneBaseAttackSpeedGain.Value;
			float cycloneStackAS = Configuration.AspectCycloneStackAttackSpeedGain.Value;

			if (cycloneBaseMS == cycloneBaseAS && cycloneStackMS == cycloneStackAS)
			{
				value = cycloneBaseMS + cycloneStackMS * (stacks - 1f);
				output += "\nIncreases <style=cIsUtility>movement speed</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>";
				output += value * 100f + "%</style>.";
			}
			else
			{
				if (cycloneBaseMS > 0f)
				{
					value = cycloneBaseMS + cycloneStackMS * (stacks - 1f);
					output += "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>";
					output += value * 100f + "%</style>.";
				}
				if (cycloneBaseAS > 0f)
				{
					value = cycloneBaseAS + cycloneStackAS * (stacks - 1f);
					output += "\nIncreases <style=cIsDamage>attack speed</style> by <style=cIsDamage>";
					output += value * 100f + "%</style>.";
				}
			}

			return output;
		}
	}



	public static class ZetAspectTinker
	{
		public static string identifier = "ZetAspectTinker";

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
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixTinkerer, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Neural Link");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of Automatization.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("EQUIPMENT_ELITEVARIETY_AFFIXTINKERER_DESC", Language.EquipmentDescription(desc, "Heal drones on use."));
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Automatization</style> :";
			if (!Configuration.AspectTinkerTweaks.Value)
			{
				output += "\nSpawn up to 3 Tinkerer's Drones that become stronger with scrap.";
				output += "\nAttacks steal scrap from the victim.";
			}
			else
			{
				if (EliteVarietyHooks.tinkerLimitHook)
				{
					if (Configuration.AspectTinkerPlayerLimit.Value < 2)
					{
						output += "\nSpawn a Tinkerer's Drone that become stronger with scrap.";
					}
					else
					{
						output += "\nSpawn upto " + Configuration.AspectTinkerPlayerLimit.Value + " Tinkerer's Drones that become stronger with scrap.";
					}
				}
				else
				{
					output += "\nSpawn up to 3 Tinkerer's Drones that become stronger with scrap.";
				}

				if (!EliteVarietyHooks.tinkerStealHook) output += "\nAttacks steal scrap from the victim.";
			}
			if (Configuration.AspectTinkerBaseDamageGain.Value > 0f)
			{
				output += "\nDrones deal <style=cIsDamage>";
				output += Configuration.AspectTinkerBaseDamageGain.Value * 100f + "%</style>";
				if (Configuration.AspectTinkerStackDamageGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectTinkerStackDamageGain.Value * 100f, "", "%");
				}
				output += " increased <style=cIsDamage>damage</style>.";
			}
			if (Configuration.AspectTinkerBaseDamageResistGain.Value > 0f)
			{
				output += "\nDrones have <style=cIsHealing>damage taken</style> reduced by <style=cIsHealing>";
				output += Configuration.AspectTinkerBaseDamageResistGain.Value * 100f + "%</style>";
				if (Configuration.AspectTinkerStackDamageResistGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectTinkerStackDamageResistGain.Value * 100f, "", "%");
				}
				output += ".";
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Automatization</style> :";
			if (!Configuration.AspectTinkerTweaks.Value)
			{
				output += "\nSpawn up to 3 Tinkerer's Drones that become stronger with scrap.";
				output += "\nAttacks steal scrap from the victim.";
			}
			else
			{
				if (EliteVarietyHooks.tinkerLimitHook)
				{
					if (Configuration.AspectTinkerPlayerLimit.Value < 2)
					{
						output += "\nSpawn a Tinkerer's Drone that become stronger with scrap.";
					}
					else
					{
						output += "\nSpawn upto " + Configuration.AspectTinkerPlayerLimit.Value + " Tinkerer's Drones that become stronger with scrap.";
					}
				}
				else
				{
					output += "\nSpawn up to 3 Tinkerer's Drones that become stronger with scrap.";
				}

				if (!EliteVarietyHooks.tinkerStealHook) output += "\nAttacks steal scrap from the victim.";
			}
			if (Configuration.AspectTinkerBaseDamageGain.Value > 0f)
			{
				value = Configuration.AspectTinkerBaseDamageGain.Value + Configuration.AspectTinkerStackDamageGain.Value * (stacks - 1f);
				output += "\nDrones deal <style=cIsDamage>";
				output += value * 100f + "%</style> increased <style=cIsDamage>damage</style>.";
			}
			if (Configuration.AspectTinkerBaseDamageResistGain.Value > 0f)
			{
				value = Configuration.AspectTinkerBaseDamageResistGain.Value + Configuration.AspectTinkerStackDamageResistGain.Value * (stacks - 1f);
				output += "\nDrones have <style=cIsHealing>damage taken</style> reduced by <style=cIsHealing>";
				output += value * 100f + "%</style>.";
			}

			return output;
		}
	}
}
