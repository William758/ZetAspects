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

			Sprite sprite;
			if (Configuration.AspectRedTier.Value)
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixArmoredIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixArmoredIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Service for the Bulwark");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of durability.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Durability</style> :";
			output += "\nSpawn with full barrier";
			if (Catalog.DynamicBarrierDecay.slowed)
			{
				output += " that slowly decays";
			}
			else if (!Catalog.DynamicBarrierDecay.enabled)
			{
				output += " that doesn't decay";
			}
			output += ".\nAttacks <style=cIsUtility>root</style> on hit, immobilizing the target.";
			if (Configuration.AspectArmorBaseArmorGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsHealing>armor</style> by <style=cIsHealing>";
				output += Configuration.AspectArmorBaseArmorGain.Value + "</style>";
				if (Configuration.AspectArmorStackArmorGain.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectArmorStackArmorGain.Value + " per stack)</style>";
				}
				output += ".";
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

			Sprite sprite;
			if (Configuration.AspectRedTier.Value)
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBuffingIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBuffingIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Retaliation of the Planet");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of battle.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Battle</style> :";
			output += "\nGrants allies inside its spherical aura increased <style=cIsDamage>movement speed</style> and <style=cIsDamage>attack speed</style>.";
			if (Configuration.AspectBannerBaseAttackSpeedGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsUtility>attack speed</style> by <style=cIsUtility>";
				output += Configuration.AspectBannerBaseAttackSpeedGain.Value * 100f + "%</style>";
				if (Configuration.AspectBannerStackAttackSpeedGain.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectBannerStackAttackSpeedGain.Value * 100f + "% per stack)</style>";
				}
				output += ".";
			}

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

			Sprite sprite;
			if (Configuration.AspectRedTier.Value)
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixImpPlaneIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixImpPlaneIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Invasion from the Red Plane");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of violence.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Violence</style> :";
			output += "\nEnemies looking directly at you are <style=cIsUtility>marked</style>, increasing damage taken.";
			output += "\nAttacks <style=cIsDamage>impale</style> on hit, periodically dealing heavy damage over 60 seconds.";
			if (Configuration.AspectImpaleBaseDotAmp.Value > 0f)
			{
				output += "\nIncreases <style=cIsUtility>damage over time multiplier</style> by <style=cIsUtility>";
				output += Configuration.AspectImpaleBaseDotAmp.Value * 100f + "%</style>";
				if (Configuration.AspectImpaleStackDotAmp.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectImpaleStackDotAmp.Value * 100f + "% per stack)</style>";
				}
				output += ".";
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

			Sprite sprite;
			if (Configuration.AspectRedTier.Value)
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPillagingIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPillagingIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Curse of Greed");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of theft.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
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
					output += " <style=cStack>(+";
					output += Configuration.AspectGoldenStackRegenGain.Value + "hp/s per stack)</style>";
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
					output += " <style=cStack>(+";
					output += Configuration.AspectGoldenStackScoredRegenGain.Value * 100f + "% per stack)</style>";
				}
				output += ".";
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

			Sprite sprite;
			if (Configuration.AspectRedTier.Value)
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixSandstormIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixSandstormIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "The Third Wish");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of sand.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Sand</style> :";
			output += "\nSurrounded by a sandstorm that damages enemies on contact.";
			output += "\nAttacks <style=cIsUtility>blind</style> on hit, reducing visibility.";
			if (Configuration.AspectCycloneBaseMovementGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>";
				output += Configuration.AspectCycloneBaseMovementGain.Value * 100f + "%</style>";
				if (Configuration.AspectCycloneStackMovementGain.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectCycloneStackMovementGain.Value * 100f + "% per stack)</style>";
				}
				output += ".";
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

			Sprite sprite;
			if (Configuration.AspectRedTier.Value)
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixTinkererIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixTinkererIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Neural Link");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of Automatization.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Automatization</style> :";
			output += "\nSpawn up to 3 Tinkerer's Drones that become stronger with scrap.";
			output += "\nAttacks steal scrap from the victim.";
			if (Configuration.AspectTinkerBaseDamageResistGain.Value > 0f)
			{
				output += "\nDrones have <style=cIsHealing>damage taken</style> reduced by <style=cIsHealing>";
				output += Configuration.AspectTinkerBaseDamageResistGain.Value * 100f + "%</style>";
				if (Configuration.AspectTinkerStackDamageResistGain.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectTinkerStackDamageResistGain.Value * 100f + "% per stack)</style>";
				}
				output += ".";
			}

			return output;
		}
	}
}
