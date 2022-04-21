using System;
using UnityEngine;
using RoR2;

namespace TPDespair.ZetAspects.Items
{
	public static class ZetAspectWhite
	{
		public static string identifier = "ZetAspectWhite";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Damage, ItemTag.Utility };
			if (Configuration.AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			Sprite outlineSprite;
			if (Configuration.AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = Configuration.AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixWhite, outlineSprite);

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



	public static class ZetAspectBlue
	{
		public static string identifier = "ZetAspectBlue";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Damage, ItemTag.Utility };
			if (Configuration.AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			Sprite outlineSprite;
			if (Configuration.AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = Configuration.AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixBlue");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixBlue, outlineSprite);

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
			bool defaultBombDisabled = false;

			string output = "<style=cDeath>Aspect of Lightning</style> :";
			if (Compat.EliteReworks.affixBlueEnabled)
			{
				if (Compat.EliteReworks.affixBluePassive)
				{
					output += "\nOccasionally Drop scatter bombs around you.";
				}
			}
			if (Configuration.AspectBlueSappedDuration.Value > 0f)
			{
				output += "\nAttacks <style=cIsUtility>sap</style> on hit for ";
				output += Language.SecondText(Configuration.AspectBlueSappedDuration.Value);
				output += ", reducing <style=cIsUtility>damage</style> by <style=cIsUtility>";
				output += Mathf.Abs(Configuration.AspectBlueSappedDamage.Value) * 100f + "%</style>.";
			}
			if (!Compat.EliteReworks.affixBlueEnabled || !Compat.EliteReworks.affixBlueRemoveShield)
			{
				if (Configuration.AspectBlueHealthConverted.Value > 0f)
				{
					output += "\nConvert <style=cIsHealing>";
					output += Configuration.AspectBlueHealthConverted.Value * 100f;
					output += "%</style> of health into <style=cIsHealing>regenerating shields</style>.";
				}
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
			if (Compat.EliteReworks.affixBlueEnabled)
			{
				if (Compat.EliteReworks.affixBlueOnHit)
				{
					defaultBombDisabled = true;

					output += "\nAttacks drop scatter bombs that explodes for <style=cIsDamage>";
					output += Compat.EliteReworks.affixBlueDamage * 100f + "%</style>";
					output += " TOTAL damage.";
				}
			}
			if (!defaultBombDisabled)
			{
				if (EffectHooks.preventedDefaultOverloadingBomb)
				{
					if (Configuration.AspectBlueBaseDamage.Value > 0f)
					{
						output += "\nAttacks attach a <style=cIsDamage>bomb</style> that explodes for <style=cIsDamage>";
						output += Configuration.AspectBlueBaseDamage.Value * 100f + "%</style>";
						if (Configuration.AspectBlueStackDamage.Value != 0f)
						{
							output += " " + Language.StackText(Configuration.AspectBlueStackDamage.Value * 100f, "", "%");
						}
						output += " TOTAL damage after ";
						output += Language.SecondText(Configuration.AspectBlueBombDuration.Value) + ".";
					}
				}
				else
				{
					output += "\nAttacks attach a <style=cIsDamage>bomb</style> that explodes for <style=cIsDamage>50%</style> TOTAL damage after 1.5 seconds.";
				}
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			bool defaultBombDisabled = false;

			string output = "<style=cDeath>Aspect of Lightning</style> :";
			if (Compat.EliteReworks.affixBlueEnabled)
			{
				if (Compat.EliteReworks.affixBluePassive)
				{
					output += "\nOccasionally Drop scatter bombs around you.";
				}
			}
			if (Configuration.AspectBlueSappedDuration.Value > 0f)
			{
				output += "\nAttacks <style=cIsUtility>sap</style> on hit for ";
				output += Language.SecondText(Configuration.AspectBlueSappedDuration.Value);
				output += ", reducing <style=cIsUtility>damage</style> by <style=cIsUtility>";
				output += Mathf.Abs(Configuration.AspectBlueSappedDamage.Value) * 100f + "%</style>.";
			}
			if (!Compat.EliteReworks.affixBlueEnabled || !Compat.EliteReworks.affixBlueRemoveShield)
			{
				if (Configuration.AspectBlueHealthConverted.Value > 0f)
				{
					output += "\nConvert <style=cIsHealing>";
					output += Configuration.AspectBlueHealthConverted.Value * 100f;
					output += "%</style> of health into <style=cIsHealing>regenerating shields</style>.";
				}
			}
			if (Configuration.AspectBlueBaseShieldGain.Value > 0f)
			{
				value = Configuration.AspectBlueBaseShieldGain.Value + Configuration.AspectBlueStackShieldGain.Value * (stacks - 1f);
				output += "\nGain <style=cIsHealing>";
				output += value * 100f + "%</style>";
				output += " of health as extra <style=cIsHealing>shield</style>.";
			}
			if (Compat.EliteReworks.affixBlueEnabled)
			{
				if (Compat.EliteReworks.affixBlueOnHit)
				{
					defaultBombDisabled = true;

					output += "\nAttacks drop scatter bombs that explodes for <style=cIsDamage>";
					output += Compat.EliteReworks.affixBlueDamage * 100f + "%</style>";
					output += " TOTAL damage.";
				}
			}
			if (!defaultBombDisabled)
			{
				if (EffectHooks.preventedDefaultOverloadingBomb)
				{
					if (Configuration.AspectBlueBaseDamage.Value > 0f)
					{
						value = Configuration.AspectBlueBaseDamage.Value + Configuration.AspectBlueStackDamage.Value * (stacks - 1f);
						output += "\nAttacks attach a <style=cIsDamage>bomb</style> that explodes for <style=cIsDamage>";
						output += value * 100f + "%</style>";
						output += " TOTAL damage after ";
						output += Language.SecondText(Configuration.AspectBlueBombDuration.Value) + ".";
					}
				}
				else
				{
					output += "\nAttacks attach a <style=cIsDamage>bomb</style> that explodes for <style=cIsDamage>50%</style> TOTAL damage after 1.5 seconds.";
				}
			}

			return output;
		}
	}



	public static class ZetAspectRed
	{
		public static string identifier = "ZetAspectRed";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Damage, ItemTag.Utility };
			if (Configuration.AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			Sprite outlineSprite;
			if (Configuration.AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = Configuration.AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixRed");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixRed, outlineSprite);

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
			if (Configuration.AspectRedBaseBurnDamage.Value > 0f)
			{
				output += "\nAttacks <style=cIsDamage>burn</style> on hit for <style=cIsDamage>";
				output += Configuration.AspectRedBaseBurnDamage.Value * 100f + "%</style>";
				if (Configuration.AspectRedStackBurnDamage.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectRedStackBurnDamage.Value * 100f, "", "%");
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
			if (Configuration.AspectRedBaseBurnDamage.Value > 0f)
			{
				value = Configuration.AspectRedBaseBurnDamage.Value + Configuration.AspectRedStackBurnDamage.Value * (stacks - 1f);
				output += "\nAttacks <style=cIsDamage>burn</style> on hit for <style=cIsDamage>";
				output += value * 100f + "%</style>";
				output += Configuration.AspectRedUseBase.Value ? " base" : " TOTAL";
				output += " damage over ";
				output += Language.SecondText(Configuration.AspectRedBurnDuration.Value) + ".";
			}

			return output;
		}
	}



	public static class ZetAspectHaunted
	{
		public static string identifier = "ZetAspectHaunted";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };
			if (Configuration.AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			Sprite outlineSprite;
			if (Configuration.AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = Configuration.AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixHaunted");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixHaunted, outlineSprite);

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
			if (!Compat.EliteReworks.affixHauntedEnabled)
			{
				output += "\nEmit an aura that cloaks nearby allies.";
			}
			else
			{
				output += "\nAttach to some nearby allies, possessing them.";
			}
			if (Configuration.AspectHauntedSlowEffect.Value)
			{
				output += "\nAttacks <style=cIsUtility>chill</style> on hit for ";
				output += Language.SecondText(Configuration.AspectWhiteSlowDuration.Value);
				output += ", reducing <style=cIsUtility>movement speed</style> by <style=cIsUtility>80%</style>.";
			}
			if (!Compat.EliteReworks.affixHauntedEnabled)
			{
				if (Configuration.AspectHauntedShredDuration.Value > 0f)
				{
					output += "\nAttacks <style=cIsDamage>shred</style> on hit for ";
					output += Language.SecondText(Configuration.AspectHauntedShredDuration.Value);
					output += ", reducing <style=cIsDamage>armor</style> by <style=cIsDamage>";
					output += Mathf.Abs(Configuration.AspectHauntedShredArmor.Value) + "</style>.";
				}
			}
			else
			{
				output += "\nAttacks <style=cIsDamage>shred</style> on hit for ";
				output += Language.SecondText(3f);
				output += ", reducing <style=cIsDamage>armor</style> by <style=cIsDamage>";
				output += 20f + "</style>.";
			}
			if (Configuration.AspectHauntedBaseArmorGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsHealing>armor</style> by <style=cIsHealing>";
				output += Configuration.AspectHauntedBaseArmorGain.Value + "</style>";
				if (Configuration.AspectHauntedStackArmorGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectHauntedStackArmorGain.Value);
				}
				output += ".";
			}
			if (!Compat.EliteReworks.affixHauntedEnabled)
			{
				if (Configuration.AspectHauntedAllyArmorGain.Value > 0f)
				{
					output += "\nGrants nearby allies <style=cIsHealing>";
					output += Configuration.AspectHauntedAllyArmorGain.Value + " armor</style>.";
				}
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Incorporeality</style> :";
			if (!Compat.EliteReworks.affixHauntedEnabled)
			{
				output += "\nEmit an aura that cloaks nearby allies.";
			}
			else
			{
				output += "\nAttach to some nearby allies, possessing them.";
			}
			if (Configuration.AspectHauntedSlowEffect.Value)
			{
				output += "\nAttacks <style=cIsUtility>chill</style> on hit for ";
				output += Language.SecondText(Configuration.AspectWhiteSlowDuration.Value);
				output += ", reducing <style=cIsUtility>movement speed</style> by <style=cIsUtility>80%</style>.";
			}
			if (!Compat.EliteReworks.affixHauntedEnabled)
			{
				if (Configuration.AspectHauntedShredDuration.Value > 0f)
				{
					output += "\nAttacks <style=cIsDamage>shred</style> on hit for ";
					output += Language.SecondText(Configuration.AspectHauntedShredDuration.Value);
					output += ", reducing <style=cIsDamage>armor</style> by <style=cIsDamage>";
					output += Mathf.Abs(Configuration.AspectHauntedShredArmor.Value) + "</style>.";
				}
			}
			else
			{
				output += "\nAttacks <style=cIsDamage>shred</style> on hit for ";
				output += Language.SecondText(3f);
				output += ", reducing <style=cIsDamage>armor</style> by <style=cIsDamage>";
				output += 20f + "</style>.";
			}
			if (Configuration.AspectHauntedBaseArmorGain.Value > 0f)
			{
				value = Configuration.AspectHauntedBaseArmorGain.Value + Configuration.AspectHauntedStackArmorGain.Value * (stacks - 1f);
				output += "\nIncreases <style=cIsHealing>armor</style> by <style=cIsHealing>";
				output += value + "</style>.";
			}
			if (!Compat.EliteReworks.affixHauntedEnabled)
			{
				if (Configuration.AspectHauntedAllyArmorGain.Value > 0f)
				{
					output += "\nGrants nearby allies <style=cIsHealing>";
					output += Configuration.AspectHauntedAllyArmorGain.Value + " armor</style>.";
				}
			}

			return output;
		}
	}



	public static class ZetAspectPoison
	{
		public static string identifier = "ZetAspectPoison";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };
			if (Configuration.AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			Sprite outlineSprite;
			if (Configuration.AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = Configuration.AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixPoison");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixPoison, outlineSprite);

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
			float ruinDuration = Configuration.AspectPoisonNullDuration.Value;
			string output = "<style=cDeath>Aspect of Corruption</style> :";
			if (Configuration.AspectPoisonFireSpikes.Value)
			{
				output += "\nPeriodically releases spiked balls that sprout spike pits from where they land.";
			}
			if (Compat.EliteReworks.affixPoisonEnabled)
			{
				output += "\nEmit an aura that applies <style=cIsDamage>ruin</style> to nearby enemies.";
				ruinDuration = 8f;
			}
			if (ruinDuration > 0f)
			{
				output += "\nAttacks <style=cIsDamage>ruin</style> on hit for ";
				output += Language.SecondText(ruinDuration);
				if (Configuration.AspectPoisonNullDamageTaken.Value != 0f)
				{
					output += ", increasing <style=cIsDamage>damage taken</style> by <style=cIsDamage>";
					output += Mathf.Abs(Configuration.AspectPoisonNullDamageTaken.Value) * 100f + "%</style>";
				}
				output += ".";
				output += "\n<style=cStack>(Ruin prevents health recovery)</style>";
			}
			if (Compat.EliteReworks.affixPoisonEnabled)
			{
				output += "\nAttacks <style=cIsDamage>weaken</style> on hit for 8 seconds";
				output += ", reducing <style=cIsDamage>armor</style> by <style=cIsDamage>30</style>";
				output += ", <style=cIsUtility>movement speed</style> by <style=cIsUtility>40%</style>";
				output += ", and <style=cIsDamage>damage</style> by <style=cIsDamage>40%</style>.";
			}
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

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			float ruinDuration = Configuration.AspectPoisonNullDuration.Value;
			string output = "<style=cDeath>Aspect of Corruption</style> :";
			if (Configuration.AspectPoisonFireSpikes.Value)
			{
				output += "\nPeriodically releases spiked balls that sprout spike pits from where they land.";
			}
			if (Compat.EliteReworks.affixPoisonEnabled)
			{
				output += "\nEmit an aura that applies <style=cIsDamage>ruin</style> to nearby enemies.";
				ruinDuration = 8f;
			}
			if (ruinDuration > 0f)
			{
				output += "\nAttacks <style=cIsDamage>ruin</style> on hit for ";
				output += Language.SecondText(ruinDuration);
				if (Configuration.AspectPoisonNullDamageTaken.Value != 0f)
				{
					output += ", increasing <style=cIsDamage>damage taken</style> by <style=cIsDamage>";
					output += Mathf.Abs(Configuration.AspectPoisonNullDamageTaken.Value) * 100f + "%</style>";
				}
				output += ".";
				output += "\n<style=cStack>(Ruin prevents health recovery)</style>";
			}
			if (Compat.EliteReworks.affixPoisonEnabled)
			{
				output += "\nAttacks <style=cIsDamage>weaken</style> on hit for 8 seconds";
				output += ", reducing <style=cIsDamage>armor</style> by <style=cIsDamage>30</style>";
				output += ", <style=cIsUtility>movement speed</style> by <style=cIsUtility>40%</style>";
				output += ", and <style=cIsDamage>damage</style> by <style=cIsDamage>40%</style>.";
			}
			if (Configuration.AspectPoisonBaseHealthGain.Value > 0f)
			{
				value = Configuration.AspectPoisonBaseHealthGain.Value + Configuration.AspectPoisonStackHealthGain.Value * (stacks - 1f);
				output += "\nIncreases <style=cIsHealing>maximum health</style> by <style=cIsHealing>";
				output += value + "</style>.";
			}

			return output;
		}
	}



	public static class ZetAspectLunar
	{
		public static string identifier = "ZetAspectLunar";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };
			if (Configuration.AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			Sprite outlineSprite;
			if (Configuration.AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = Configuration.AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixLunar");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixLunar, outlineSprite);

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



	public static class ZetAspectEarth
	{
		public static string identifier = "ZetAspectEarth";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };
			if (Configuration.AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			Sprite outlineSprite;
			if (Configuration.AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = Configuration.AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixEarth, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "His Reassurance");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of earth.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", BuildDescription());
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("EQUIPMENT_AFFIXEARTH_DESC", Language.EquipmentDescription(desc, ""));
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Earth</style> :";
			output += "\nHeal nearby allies.";
			if (Configuration.AspectEarthRegeneration.Value > 0)
			{
				output += "\nIncreases <style=cIsHealing>health regeneration</style> by <style=cIsHealing>" + Configuration.AspectEarthRegeneration.Value * 100f + "% hp/s</style>.";
			}
			if (Configuration.AspectEarthPoachedDuration.Value > 0f)
			{
				output += "\nAttacks <style=cIsUtility>poach</style> on hit for ";
				output += Language.SecondText(Configuration.AspectEarthPoachedDuration.Value);
				if (Configuration.AspectEarthPoachedAttackSpeed.Value != 0f)
				{
					output += ", reducing <style=cIsUtility>attack speed</style> by <style=cIsUtility>";
					output += Mathf.Abs(Configuration.AspectEarthPoachedAttackSpeed.Value) * 100f + "%</style>";
				}
				output += ".";
				if (Configuration.AspectEarthPoachedLeech.Value > 0f)
				{
					output += "\n<style=cStack>(Hits against poached enemies heal for "+ Configuration.AspectEarthPoachedLeech.Value * 100f + "% of damage dealt)</style>";
				}
			}
			if (Configuration.AspectEarthBaseLeech.Value > 0)
			{
				output += "\n<style=cIsHealing>Heal</style> for <style=cIsHealing>";
				output += Configuration.AspectEarthBaseLeech.Value * 100f + "%</style>";
				if (Configuration.AspectEarthStackLeech.Value != 0)
				{
					output += " " + Language.StackText(Configuration.AspectEarthStackLeech.Value * 100f, "", "%");
				}
				output += " of the <style=cIsDamage>damage</style> you deal.";
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Earth</style> :";
			output += "\nHeal nearby allies.";
			if (Configuration.AspectEarthRegeneration.Value > 0)
			{
				output += "\nIncreases <style=cIsHealing>health regeneration</style> by <style=cIsHealing>" + Configuration.AspectEarthRegeneration.Value * 100f + "% hp/s</style>.";
			}
			if (Configuration.AspectEarthPoachedDuration.Value > 0f)
			{
				output += "\nAttacks <style=cIsUtility>poach</style> on hit for ";
				output += Language.SecondText(Configuration.AspectEarthPoachedDuration.Value);
				if (Configuration.AspectEarthPoachedAttackSpeed.Value != 0f)
				{
					output += ", reducing <style=cIsUtility>attack speed</style> by <style=cIsUtility>";
					output += Mathf.Abs(Configuration.AspectEarthPoachedAttackSpeed.Value) * 100f + "%</style>";
				}
				output += ".";
				if (Configuration.AspectEarthPoachedLeech.Value > 0f)
				{
					output += "\n<style=cStack>(Hits against poached enemies heal for " + Configuration.AspectEarthPoachedLeech.Value * 100f + "% of damage dealt)</style>";
				}
			}
			if (Configuration.AspectEarthBaseLeech.Value > 0)
			{
				value = Configuration.AspectEarthBaseLeech.Value + Configuration.AspectEarthStackLeech.Value * (stacks - 1f);
				output += "\n<style=cIsHealing>Heal</style> for <style=cIsHealing>";
				output += value * 100f + "%</style>";
				output += " of the <style=cIsDamage>damage</style> you deal.";
			}

			return output;
		}
	}



	public static class ZetAspectVoid
	{
		public static string identifier = "ZetAspectVoid";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Damage, ItemTag.Utility };
			if (Configuration.AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			Sprite outlineSprite;
			if (Configuration.AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = Configuration.AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = Catalog.Prefabs.AffixVoid;
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixVoid, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			Language.helperTarget = "default";
			Language.RegisterToken("ITEM_" + locToken + "_NAME", "Entropic Fracture");
			Language.RegisterToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of void.");

			string desc = BuildDescription();
			Language.RegisterToken("ITEM_" + locToken + "_DESC", BuildDescription());
			if (!DropHooks.CanObtainItem()) desc = BuildEquipmentDescription();
			Language.RegisterToken("EQUIPMENT_AFFIXVOID_NAME", "Entropic Fracture");
			Language.RegisterToken("EQUIPMENT_AFFIXVOID_PICKUP", "Become an aspect of void.");
			Language.RegisterToken("EQUIPMENT_AFFIXVOID_DESC", Language.EquipmentDescription(desc, ""));
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Void</style> :";

			output += "\n<style=cIsHealing>Blocks</style> incoming damage once. Recharges after a delay";
			if (Configuration.AspectVoidBaseHealthGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsHealing>maximum health</style> by <style=cIsHealing>";
				output += Configuration.AspectVoidBaseHealthGain.Value * 100f + "%</style>";
				if (Configuration.AspectVoidStackHealthGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectVoidStackHealthGain.Value * 100f, "", "%");
				}
				output += ".";
			}
			if (Configuration.AspectVoidBaseDamageGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsDamage>damage</style> by <style=cIsDamage>";
				output += Configuration.AspectVoidBaseDamageGain.Value * 100f + "%</style>";
				if (Configuration.AspectVoidStackDamageGain.Value != 0f)
				{
					output += " " + Language.StackText(Configuration.AspectVoidStackDamageGain.Value * 100f, "", "%");
				}
				output += ".";
			}
			if (Compat.EliteReworks.eliteVoidEnabled)
			{
				output += "\nAttacks <style=cIsUtility>nullify</style> on hit for 8 seconds.";
				output += "\n<style=cStack>(Enemies with 3 nullify stacks are rooted for 3 seconds)</style>";
			}
			else
			{
				if (EffectHooks.preventedDefaultVoidCollapse)
				{
					if (Configuration.AspectVoidBaseCollapseDamage.Value > 0f)
					{
						output += "\nAttacks <style=cIsDamage>collapse</style> on hit for <style=cIsDamage>";
						output += Configuration.AspectVoidBaseCollapseDamage.Value * 100f + "%</style>";
						if (Configuration.AspectVoidStackCollapseDamage.Value != 0f)
						{
							output += " " + Language.StackText(Configuration.AspectVoidStackCollapseDamage.Value * 100f, "", "%");
						}
						output += Configuration.AspectVoidUseBase.Value ? " base" : " TOTAL";
						output += " damage after ";
						output += Language.SecondText(3f) + ".";
					}
				}
				else
				{
					output += "<style=cIsDamage>100%</style> chance to <style=cIsDamage>collapse</style> an enemy for <style=cIsDamage>400%</style> base damage.";
				}
			}

			return output;
		}

		public static string BuildEquipmentDescription()
		{
			float value, stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

			string output = "<style=cDeath>Aspect of Void</style> :";

			output += "\n<style=cIsHealing>Blocks</style> incoming damage once. Recharges after a delay";
			if (Configuration.AspectVoidBaseHealthGain.Value > 0f)
			{
				value = Configuration.AspectVoidBaseHealthGain.Value + Configuration.AspectVoidStackHealthGain.Value * (stacks - 1f);
				output += "\nIncreases <style=cIsHealing>maximum health</style> by <style=cIsHealing>";
				output += value * 100f + "%</style>.";
			}
			if (Configuration.AspectVoidBaseDamageGain.Value > 0f)
			{
				value = Configuration.AspectVoidBaseDamageGain.Value + Configuration.AspectVoidStackDamageGain.Value * (stacks - 1f);
				output += "\nIncreases <style=cIsDamage>damage</style> by <style=cIsDamage>";
				output += value * 100f + "%</style>.";
			}
			if (Compat.EliteReworks.eliteVoidEnabled)
			{
				output += "\nAttacks <style=cIsUtility>nullify</style> on hit for 8 seconds.";
				output += "\n<style=cStack>(Enemies with 3 nullify stacks are rooted for 3 seconds)</style>";
			}
			else
			{
				if (EffectHooks.preventedDefaultVoidCollapse)
				{
					if (Configuration.AspectVoidBaseCollapseDamage.Value > 0f)
					{
						value = Configuration.AspectVoidBaseCollapseDamage.Value + Configuration.AspectVoidStackCollapseDamage.Value * (stacks - 1f);
						output += "\nAttacks <style=cIsDamage>collapse</style> on hit for <style=cIsDamage>";
						output += value * 100f + "%</style>";
						output += Configuration.AspectVoidUseBase.Value ? " base" : " TOTAL";
						output += " damage after ";
						output += Language.SecondText(3f) + ".";
					}
				}
				else
				{
					output += "<style=cIsDamage>100%</style> chance to <style=cIsDamage>collapse</style> an enemy for <style=cIsDamage>400%</style> base damage.";
				}
			}

			return output;
		}
	}
}
