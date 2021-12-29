using RoR2;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class ZetAspectIce
	{
		public static string identifier = "ZetAspectIce";

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
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixWhite, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Her Biting Embrace");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of ice.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", BuildDescription());
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("EQUIPMENT_AFFIXWHITE_DESC", Language.EquipmentDescription(desc, "Deploy a health-reducing ice crystal on use."));

			Language.helperTarget = "tr";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Acıtan Kucaklaması", "tr");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Buzun bir yüzü ol.", "tr");
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Ice</style> :\nAttacks <style=cIsUtility>chill</style> on hit for ";
			output += Language.SecondText(Configuration.AspectWhiteSlowDuration.Value);
			output += ", reducing <style=cIsUtility>movement speed</style> by <style=cIsUtility>80%</style>.";
			if (Configuration.AspectWhiteBaseFreezeChance.Value > 0f)
			{
				output += "\nAttacks have a <style=cIsUtility>" + Configuration.AspectWhiteBaseFreezeChance.Value + "%</style>";
				if (Configuration.AspectWhiteStackFreezeChance.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectWhiteStackFreezeChance.Value, "", "%");
				}
				output += " chance to <style=cIsUtility>freeze</style> for ";
				output += Language.SecondText(Configuration.AspectWhiteFreezeDuration.Value) + ".";
			}
			if (Configuration.AspectWhiteBaseDamage.Value > 0f)
			{
				output += "\nAttacks fire a <style=cIsDamage>blade</style> that deals <style=cIsDamage>";
				output += Configuration.AspectWhiteBaseDamage.Value * 100f + "%</style>";
				if (Configuration.AspectWhiteStackDamage.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectWhiteStackDamage.Value * 100f, "", "%");
				}
				output += " TOTAL damage.";
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Ice</style> :\nAttacks <style=cIsUtility>chill</style> on hit for ";
			output += Language.SecondText(Configuration.AspectWhiteSlowDuration.Value);
			output += ", reducing <style=cIsUtility>movement speed</style> by <style=cIsUtility>80%</style>.";
			if (Configuration.AspectWhiteBaseFreezeChance.Value > 0f)
			{
				value = Configuration.AspectWhiteBaseFreezeChance.Value + Configuration.AspectWhiteStackFreezeChance.Value * (stacks - 1f);
				output += "\nAttacks have a <style=cIsUtility>" + value + "%</style>";
				output += " chance to <style=cIsUtility>freeze</style> for ";
				output += Language.SecondText(Configuration.AspectWhiteFreezeDuration.Value) + ".";
			}
			if (Configuration.AspectWhiteBaseDamage.Value > 0f)
			{
				value = Configuration.AspectWhiteBaseDamage.Value + Configuration.AspectWhiteStackDamage.Value * (stacks - 1f);
				output += "\nAttacks fire a <style=cIsDamage>blade</style> that deals <style=cIsDamage>";
				output += value * 100f + "%</style>";
				output += " TOTAL damage.";
			}

			return output;
		}
	}



	public static class ZetAspectLightning
	{
		public static string identifier = "ZetAspectLightning";

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
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixBlue");
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixBlue, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Silence Between Two Strikes");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of lightning.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("EQUIPMENT_AFFIXBLUE_DESC", Language.EquipmentDescription(desc, "Teleport on use."));

			Language.helperTarget = "tr";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "İki Darbe Arası Sessizlik", "tr");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Yıldırımın bir yüzü ol.", "tr");
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Lightning</style> :";
			if (Configuration.AspectBlueSappedDuration.Value > 0f)
			{
				output += "\nAttacks <style=cIsUtility>sap</style> on hit for ";
				output += Language.SecondText(Configuration.AspectBlueSappedDuration.Value);
				output += ", reducing <style=cIsUtility>damage</style> by <style=cIsUtility>";
				output += Mathf.Abs(Configuration.AspectBlueSappedDamage.Value) * 100f + "%</style>.";
			}
			if (Configuration.AspectBlueHealthConverted.Value > 0f)
			{
				output += "\nConvert <style=cIsHealing>";
				output += Configuration.AspectBlueHealthConverted.Value * 100f;
				output += "%</style> of health into <style=cIsHealing>regenerating shields</style>.";
			}
			if (Configuration.AspectBlueBaseShieldGain.Value > 0f)
			{
				output += "\nGain <style=cIsHealing>";
				output += Configuration.AspectBlueBaseShieldGain.Value * 100f + "%</style>";
				if (Configuration.AspectBlueStackShieldGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectBlueStackShieldGain.Value * 100f, "", "%");
				}
				output += " of health as extra <style=cIsHealing>shield</style>.";
			}
			if (Configuration.AspectBlueBaseDamage.Value > 0f)
			{
				output += "\nAttacks attach a <style=cIsDamage>bomb</style> that explodes for <style=cIsDamage>";
				output += Configuration.AspectBlueBaseDamage.Value * 100f + "%</style>";
				if (Configuration.AspectBlueStackDamage.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectBlueStackDamage.Value * 100f, "", "%");
				}
				output += " TOTAL damage after ";
				output += Language.SecondText(Configuration.AspectWhiteSlowDuration.Value) + ".";
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Lightning</style> :";
			if (Configuration.AspectBlueSappedDuration.Value > 0f)
			{
				output += "\nAttacks <style=cIsUtility>sap</style> on hit for ";
				output += Language.SecondText(Configuration.AspectBlueSappedDuration.Value);
				output += ", reducing <style=cIsUtility>damage</style> by <style=cIsUtility>";
				output += Mathf.Abs(Configuration.AspectBlueSappedDamage.Value) * 100f + "%</style>.";
			}
			if (Configuration.AspectBlueHealthConverted.Value > 0f)
			{
				output += "\nConvert <style=cIsHealing>";
				output += Configuration.AspectBlueHealthConverted.Value * 100f;
				output += "%</style> of health into <style=cIsHealing>regenerating shields</style>.";
			}
			if (Configuration.AspectBlueBaseShieldGain.Value > 0f)
			{
				value = Configuration.AspectBlueBaseShieldGain.Value + Configuration.AspectBlueStackShieldGain.Value * (stacks - 1f);
				output += "\nGain <style=cIsHealing>";
				output += value * 100f + "%</style>";
				output += " of health as extra <style=cIsHealing>shield</style>.";
			}
			if (Configuration.AspectBlueBaseDamage.Value > 0f)
			{
				value = Configuration.AspectBlueBaseDamage.Value + Configuration.AspectBlueStackDamage.Value * (stacks - 1f);
				output += "\nAttacks attach a <style=cIsDamage>bomb</style> that explodes for <style=cIsDamage>";
				output += value * 100f + "%</style>";
				output += " TOTAL damage after ";
				output += Language.SecondText(Configuration.AspectWhiteSlowDuration.Value) + ".";
			}

			return output;
		}
	}



	public static class ZetAspectFire
	{
		public static string identifier = "ZetAspectFire";

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
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixRed");
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixRed, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Ifrit's Distinction");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of fire.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("EQUIPMENT_AFFIXRED_DESC", Language.EquipmentDescription(desc, "Release a barrage of seeking flame missiles on use."));

			Language.helperTarget = "tr";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "İfritin Farkı", "tr");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Ateşin bir yüzü ol.", "tr");
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Fire</style> :";
			if (Configuration.AspectRedTrail.Value)
			{
				output += "\nLeave behind a fiery trail that damages enemies on contact.";
			}
			if (Configuration.AspectRedExtraJump.Value)
			{
				output += "\nGain <style=cIsUtility>+1</style> maximum <style=cIsUtility>jump count</style>.";
			}
			if (Configuration.AspectRedBaseMovementGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>";
				output += Configuration.AspectRedBaseMovementGain.Value * 100f + "%</style>";
				if (Configuration.AspectRedStackMovementGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectRedStackMovementGain.Value * 100f, "", "%");
				}
				output += ".";
			}
			if (Configuration.AspectRedBaseDamage.Value > 0f)
			{
				output += "\nAttacks <style=cIsDamage>burn</style> on hit for <style=cIsDamage>";
				output += Configuration.AspectRedBaseDamage.Value * 100f + "%</style>";
				if (Configuration.AspectRedStackDamage.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectRedStackDamage.Value * 100f, "", "%");
				}
				output += Configuration.AspectRedUseBase.Value ? " base" : " TOTAL";
				output += " damage over ";
				output += Language.SecondText(Configuration.AspectRedBurnDuration.Value) + ".";
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Fire</style> :";
			if (Configuration.AspectRedTrail.Value)
			{
				output += "\nLeave behind a fiery trail that damages enemies on contact.";
			}
			if (Configuration.AspectRedExtraJump.Value)
			{
				output += "\nGain <style=cIsUtility>+1</style> maximum <style=cIsUtility>jump count</style>.";
			}
			if (Configuration.AspectRedBaseMovementGain.Value > 0f)
			{
				value = Configuration.AspectRedBaseMovementGain.Value + Configuration.AspectRedStackMovementGain.Value * (stacks - 1f);
				output += "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>";
				output += value * 100f + "%</style>.";
			}
			if (Configuration.AspectRedBaseDamage.Value > 0f)
			{
				value = Configuration.AspectRedBaseDamage.Value + Configuration.AspectRedStackDamage.Value * (stacks - 1f);
				output += "\nAttacks <style=cIsDamage>burn</style> on hit for <style=cIsDamage>";
				output += value * 100f + "%</style>";
				output += Configuration.AspectRedUseBase.Value ? " base" : " TOTAL";
				output += " damage over ";
				output += Language.SecondText(Configuration.AspectRedBurnDuration.Value) + ".";
			}

			return output;
		}
	}



	public static class ZetAspectCelestial
	{
		public static string identifier = "ZetAspectCelestial";

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
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixHaunted");
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixHaunted, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Spectral Circlet");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of incorporeality.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("EQUIPMENT_AFFIXHAUNTED_DESC", Language.EquipmentDescription(desc, "Heal all allies inside the invisibility aura on use."));

			Language.helperTarget = "tr";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Hayali Taç", "tr");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Soyutluğun bir yüzü ol.", "tr");
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Incorporeality</style> :";
			output += "\nEmit an aura that cloaks nearby allies.";
			if (Configuration.AspectGhostSlowEffect.Value)
			{
				output += "\nAttacks <style=cIsUtility>chill</style> on hit for ";
				output += Language.SecondText(Configuration.AspectWhiteSlowDuration.Value);
				output += ", reducing <style=cIsUtility>movement speed</style> by <style=cIsUtility>80%</style>.";
			}
			if (Configuration.AspectGhostShredDuration.Value > 0f)
			{
				output += "\nAttacks <style=cIsDamage>shred</style> on hit for ";
				output += Language.SecondText(Configuration.AspectGhostShredDuration.Value);
				output += ", reducing <style=cIsDamage>armor</style> by <style=cIsDamage>";
				output += Mathf.Abs(Configuration.AspectGhostShredArmor.Value) + "</style>.";
			}
			if (Configuration.AspectGhostBaseArmorGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsHealing>armor</style> by <style=cIsHealing>";
				output += Configuration.AspectGhostBaseArmorGain.Value + "</style>";
				if (Configuration.AspectGhostStackArmorGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectGhostStackArmorGain.Value);
				}
				output += ".";
			}
			if (Configuration.AspectGhostAllyArmorGain.Value > 0f)
			{
				output += "\nGrants nearby allies <style=cIsHealing>";
				output += Configuration.AspectGhostAllyArmorGain.Value + " armor</style>.";
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Incorporeality</style> :";
			output += "\nEmit an aura that cloaks nearby allies.";
			if (Configuration.AspectGhostSlowEffect.Value)
			{
				output += "\nAttacks <style=cIsUtility>chill</style> on hit for ";
				output += Language.SecondText(Configuration.AspectWhiteSlowDuration.Value);
				output += ", reducing <style=cIsUtility>movement speed</style> by <style=cIsUtility>80%</style>.";
			}
			if (Configuration.AspectGhostShredDuration.Value > 0f)
			{
				output += "\nAttacks <style=cIsDamage>shred</style> on hit for ";
				output += Language.SecondText(Configuration.AspectGhostShredDuration.Value);
				output += ", reducing <style=cIsDamage>armor</style> by <style=cIsDamage>";
				output += Mathf.Abs(Configuration.AspectGhostShredArmor.Value) + "</style>.";
			}
			if (Configuration.AspectGhostBaseArmorGain.Value > 0f)
			{
				value = Configuration.AspectGhostBaseArmorGain.Value + Configuration.AspectGhostStackArmorGain.Value * (stacks - 1f);
				output += "\nIncreases <style=cIsHealing>armor</style> by <style=cIsHealing>";
				output += value + "</style>.";
			}
			if (Configuration.AspectGhostAllyArmorGain.Value > 0f)
			{
				output += "\nGrants nearby allies <style=cIsHealing>";
				output += Configuration.AspectGhostAllyArmorGain.Value + " armor</style>.";
			}

			return output;
		}
	}



	public static class ZetAspectMalachite
	{
		public static string identifier = "ZetAspectMalachite";

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
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixPoison");
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixPoison, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "N'kuhana's Retort");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of corruption.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("EQUIPMENT_AFFIXPOISON_DESC", Language.EquipmentDescription(desc, "Summon an ally Malachite Urchin that inherits your items on use."));

			Language.helperTarget = "tr";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "N'kuhana'nın Cevabı", "tr");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Yozlaşmanın bir yüzü ol.", "tr");
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Corruption</style> :";
			if (Configuration.AspectPoisonFireSpikes.Value)
			{
				output += "\nPeriodically releases spiked balls that sprout spike pits from where they land.";
			}
			output += "\nAttacks <style=cIsDamage>nullify</style> on hit for ";
			output += Language.SecondText(Configuration.AspectPoisonNullDuration.Value);
			if (Configuration.AspectPoisonNullDamageTaken.Value != 0f)
			{
				output += ", increasing <style=cIsDamage>damage taken</style> by <style=cIsDamage>";
				output += Mathf.Abs(Configuration.AspectPoisonNullDamageTaken.Value) * 100f + "%</style>";
			}
			output += ".";
			if (Configuration.AspectPoisonBaseHealthGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsHealing>maximum health</style> by <style=cIsHealing>";
				output += Configuration.AspectPoisonBaseHealthGain.Value + "</style>";
				if (Configuration.AspectPoisonStackHealthGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectPoisonStackHealthGain.Value);
				}
				output += ".";
			}
			if (Configuration.AspectPoisonBaseHeal.Value > 0)
			{
				output += "\nDealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>";
				output += Configuration.AspectPoisonBaseHeal.Value + "</style>";
				if (Configuration.AspectPoisonStackHeal.Value != 0)
				{
					output += " " + Language.StackText(Configuration.AspectPoisonStackHeal.Value);
				}
				output += " <style=cIsHealing>health</style>.";
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Corruption</style> :";
			if (Configuration.AspectPoisonFireSpikes.Value)
			{
				output += "\nPeriodically releases spiked balls that sprout spike pits from where they land.";
			}
			output += "\nAttacks <style=cIsDamage>nullify</style> on hit for ";
			output += Language.SecondText(Configuration.AspectPoisonNullDuration.Value);
			if (Configuration.AspectPoisonNullDamageTaken.Value != 0f)
			{
				output += ", increasing <style=cIsDamage>damage taken</style> by <style=cIsDamage>";
				output += Mathf.Abs(Configuration.AspectPoisonNullDamageTaken.Value) * 100f + "%</style>";
			}
			output += ".";
			if (Configuration.AspectPoisonBaseHealthGain.Value > 0f)
			{
				value = Configuration.AspectPoisonBaseHealthGain.Value + Configuration.AspectPoisonStackHealthGain.Value * (stacks - 1f);
				output += "\nIncreases <style=cIsHealing>maximum health</style> by <style=cIsHealing>";
				output += value + "</style>.";
			}
			if (Configuration.AspectPoisonBaseHeal.Value > 0)
			{
				value = Configuration.AspectPoisonBaseHeal.Value + Configuration.AspectPoisonStackHeal.Value * (stacks - 1f);
				output += "\nDealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>";
				output += value + "</style>";
				output += " <style=cIsHealing>health</style>.";
			}

			return output;
		}
	}



	public static class ZetAspectPerfect
	{
		public static string identifier = "ZetAspectPerfect";

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
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixLunar");
			itemDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(ZetAspectsContent.Sprites.AffixLunar, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Shared Design");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of perfection.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("EQUIPMENT_AFFIXLUNAR_DESC", Language.EquipmentDescription(desc, "Gain temporary defense from powerful attacks on use."));

			Language.helperTarget = "tr";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Paylaşımlı Tasarım", "tr");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Kusursuzluğun bir yüzü ol.", "tr");
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Perfection</style> :";
			if (Configuration.AspectLunarProjectiles.Value)
			{
				output += "\nPeriodically fire projectiles while in combat.";
			}
			output += "\nAttacks <style=cIsDamage>cripple</style> on hit for ";
			output += Language.SecondText(Configuration.AspectLunarCrippleDuration.Value);
			output += ", reducing <style=cIsDamage>armor</style> by <style=cIsDamage>20</style> and";
			output += " <style=cIsUtility>movement speed</style> by <style=cIsUtility>50%</style>.";
			if (Configuration.AspectLunarBaseMovementGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>";
				output += Configuration.AspectLunarBaseMovementGain.Value * 100f + "%</style>";
				if (Configuration.AspectLunarStackMovementGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectLunarStackMovementGain.Value * 100f, "", "%");
				}
				output += ".";
			}
			output += "\nConvert <style=cIsHealing>100%</style> of health into <style=cIsHealing>regenerating shields</style>.";
			if (Configuration.AspectLunarBaseShieldGain.Value > 0f)
			{
				output += "\nGain <style=cIsHealing>";
				output += Configuration.AspectLunarBaseShieldGain.Value * 100f + "%</style>";
				if (Configuration.AspectLunarStackShieldGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectLunarStackShieldGain.Value * 100f, "", "%");
				}
				output += " extra <style=cIsHealing>shield</style> from conversion.";
			}
			if (Configuration.AspectLunarRegen.Value > 0f)
			{
				output += "\nAt least <style=cIsHealing>";
				output += Configuration.AspectLunarRegen.Value * 100f + "%</style>";
				output += " of <style=cIsHealing>health regeneration</style> applies to <style=cIsHealing>shields</style>.";
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Perfection</style> :";
			if (Configuration.AspectLunarProjectiles.Value)
			{
				output += "\nPeriodically fire projectiles while in combat.";
			}
			output += "\nAttacks <style=cIsDamage>cripple</style> on hit for ";
			output += Language.SecondText(Configuration.AspectLunarCrippleDuration.Value);
			output += ", reducing <style=cIsDamage>armor</style> by <style=cIsDamage>20</style> and";
			output += " <style=cIsUtility>movement speed</style> by <style=cIsUtility>50%</style>.";
			if (Configuration.AspectLunarBaseMovementGain.Value > 0f)
			{
				value = Configuration.AspectLunarBaseMovementGain.Value + Configuration.AspectLunarStackMovementGain.Value * (stacks - 1f);
				output += "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>";
				output += value * 100f + "%</style>.";
			}
			output += "\nConvert <style=cIsHealing>100%</style> of health into <style=cIsHealing>regenerating shields</style>.";
			if (Configuration.AspectLunarBaseShieldGain.Value > 0f)
			{
				value = Configuration.AspectLunarBaseShieldGain.Value + Configuration.AspectLunarStackShieldGain.Value * (stacks - 1f);
				output += "\nGain <style=cIsHealing>";
				output += value * 100f + "%</style>";
				output += " extra <style=cIsHealing>shield</style> from conversion.";
			}
			if (Configuration.AspectLunarRegen.Value > 0f)
			{
				output += "\nAt least <style=cIsHealing>";
				output += Configuration.AspectLunarRegen.Value * 100f + "%</style>";
				output += " of <style=cIsHealing>health regeneration</style> applies to <style=cIsHealing>shields</style>.";
			}

			return output;
		}
	}
}
