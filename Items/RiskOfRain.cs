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

			Sprite sprite;
			if (Configuration.AspectRedTier.Value)
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixWhiteIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixWhiteIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Her Biting Embrace");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of ice.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Ice</style> :\nAttacks <style=cIsUtility>chill</style> on hit for ";
			output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectWhiteSlowDuration.Value);
			output += ", reducing <style=cIsUtility>movement speed</style> by <style=cIsUtility>80%</style>.";
			if (Configuration.AspectWhiteFreezeChance.Value > 0f)
			{
				output += "\nAttacks have a <style=cIsUtility>";
				output += Configuration.AspectWhiteFreezeChance.Value;
				output += "%</style> <style=cStack>(+";
				output += Configuration.AspectWhiteFreezeChance.Value;
				output += "% per stack)</style> chance to <style=cIsUtility>freeze</style> for ";
				output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectWhiteFreezeDuration.Value) + ".";
			}
			if (Configuration.AspectWhiteBaseDamage.Value > 0f)
			{
				output += "\nAttacks fire a <style=cIsDamage>blade</style> that deals <style=cIsDamage>";
				output += Configuration.AspectWhiteBaseDamage.Value * 100f + "%</style>";
				if (Configuration.AspectWhiteStackDamage.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectWhiteStackDamage.Value * 100f + "% per stack)</style>";
				}
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

			Sprite sprite;
			if (Configuration.AspectRedTier.Value)
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBlueIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBlueIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixBlue");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Silence Between Two Strikes");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of lightning.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Lightning</style> :";
			if (Configuration.AspectBlueSappedDuration.Value > 0f)
			{
				output += "\nAttacks <style=cIsUtility>sap</style> on hit for ";
				output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectBlueSappedDuration.Value);
				output += ", reducing <style=cIsUtility>damage</style> by <style=cIsUtility>";
				output += Mathf.Abs(Configuration.AspectBlueSappedDamage.Value) * 100f + "%</style>.";
			}
			output += "\nConvert <style=cIsHealing>";
			output += Configuration.AspectBlueHealthConverted.Value * 100f;
			output += "%</style> of health into <style=cIsHealing>regenerating shields</style>.";
			if (Configuration.AspectBlueBaseShieldGain.Value > 0f)
			{
				output += "\nGain <style=cIsHealing>";
				output += Configuration.AspectBlueBaseShieldGain.Value * 100f + "%</style>";
				if (Configuration.AspectBlueStackShieldGain.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectBlueStackShieldGain.Value * 100f + "% per stack)</style>";
				}
				output += " of health as extra <style=cIsHealing>shield</style>.";
			}
			if (Configuration.AspectBlueBaseDamage.Value > 0f)
			{
				output += "\nAttacks attach a <style=cIsDamage>bomb</style> that explodes for <style=cIsDamage>";
				output += Configuration.AspectBlueBaseDamage.Value * 100f + "%</style>";
				if (Configuration.AspectBlueStackDamage.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectBlueStackDamage.Value * 100f + "% per stack)</style>";
				}
				output += " TOTAL damage after ";
				output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectBlueBombDuration.Value) + ".";
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

			Sprite sprite;
			if (Configuration.AspectRedTier.Value)
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixRedIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixRedIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixRed");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Ifrit's Distinction");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of fire.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
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
					output += " <style=cStack>(+";
					output += Configuration.AspectRedStackMovementGain.Value * 100f + "% per stack)</style>";
				}
				output += ".";
			}
			if (Configuration.AspectRedBaseDamage.Value > 0f)
			{
				output += "\nAttacks <style=cIsDamage>burn</style> on hit for <style=cIsDamage>";
				output += Configuration.AspectRedBaseDamage.Value * 100f + "%</style>";
				if (Configuration.AspectRedStackDamage.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectRedStackDamage.Value * 100f + "% per stack)</style>";
				}
				output += Configuration.AspectRedUseBase.Value ? " base" : " TOTAL";
				output += " damage over ";
				output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectRedBurnDuration.Value) + ".";
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

			Sprite sprite;
			if (Configuration.AspectRedTier.Value)
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixHauntedIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixHauntedIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixHaunted");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Spectral Circlet");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of incorporeality.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Incorporeality</style> :";
			if (Configuration.AspectGhostSlowEffect.Value)
			{
				output += "\nAttacks <style=cIsUtility>chill</style> on hit for ";
				output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectWhiteSlowDuration.Value);
				output += ", reducing <style=cIsUtility>movement speed</style> by <style=cIsUtility>80%</style>.";
			}
			if (Configuration.AspectGhostShredDuration.Value > 0f)
			{
				output += "\nAttacks <style=cIsUtility>shred</style> on hit for ";
				output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectGhostShredDuration.Value);
				output += ", reducing <style=cIsUtility>armor</style> by <style=cIsUtility>";
				output += Mathf.Abs(Configuration.AspectGhostShredArmor.Value) + "</style>.";
			}
			if (Configuration.AspectGhostBaseArmorGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsHealing>armor</style> by <style=cIsHealing>";
				output += Configuration.AspectGhostBaseArmorGain.Value + "</style>";
				if (Configuration.AspectGhostStackArmorGain.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectGhostStackArmorGain.Value + " per stack)</style>";
				}
				output += ".";
			}
			if (Configuration.AspectGhostAllyArmorGain.Value > 0f)
			{
				output += "\nGrants allies inside its spherical aura <style=cIsHealing>";
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

			Sprite sprite;
			if (Configuration.AspectRedTier.Value)
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPoisonIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPoisonIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixPoison");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "N'kuhana's Retort");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of corruption.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}



		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Corruption</style> :";
			if (Configuration.AspectPoisonFireSpikes.Value)
			{
				output += "\nPeriodically releases spiked balls that sprout spike pits from where they land.";
			}
			output += "\nAttacks <style=cIsUtility>nullify</style> on hit for ";
			output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectPoisonNullDuration.Value);
			if (Configuration.AspectPoisonNullDamageTaken.Value != 0f)
			{
				output += ", increasing <style=cIsUtility>damage taken</style> by <style=cIsUtility>";
				output += Mathf.Abs(Configuration.AspectPoisonNullDamageTaken.Value) * 100f + "%</style>";
			}
			output += ".";
			if (Configuration.AspectPoisonBaseHealthGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsHealing>maximum health</style> by <style=cIsHealing>";
				output += Configuration.AspectPoisonBaseHealthGain.Value + "</style>";
				if (Configuration.AspectPoisonStackHealthGain.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectPoisonStackHealthGain.Value + " per stack)</style>";
				}
				output += ".";
			}
			if (Configuration.AspectPoisonBaseHeal.Value > 0)
			{
				output += "\nDealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>";
				output += Configuration.AspectPoisonBaseHeal.Value + "</style>";
				if (Configuration.AspectPoisonStackHeal.Value != 0)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectPoisonStackHeal.Value + " per stack)</style>";
				}
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

			Sprite sprite;
			if (Configuration.AspectRedTier.Value)
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixLunarIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixLunarIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixLunar");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Shared Design");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of perfection.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Perfection</style> :";
			if (Configuration.AspectLunarProjectiles.Value)
			{
				output += "\nPeriodically fire projectiles while in combat.";
			}
			output += "\nAttacks <style=cIsUtility>cripple</style> on hit for ";
			output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectLunarCrippleDuration.Value);
			output += ", reducing <style=cIsUtility>armor</style> by <style=cIsUtility>20</style> and";
			output += " <style=cIsUtility>movement speed</style> by <style=cIsUtility>50%</style>.";
			if (Configuration.AspectLunarBaseMovementGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>";
				output += Configuration.AspectLunarBaseMovementGain.Value * 100f + "%</style>";
				if (Configuration.AspectLunarStackMovementGain.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectLunarStackMovementGain.Value * 100f + "% per stack)</style>";
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
					output += " <style=cStack>(+";
					output += Configuration.AspectLunarStackShieldGain.Value * 100f + "% per stack)</style>";
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
	}
}
