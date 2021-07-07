using RoR2;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class Catalog
	{
		public static bool set = false;

		internal static void SetOnRunStartHook()
		{
			On.RoR2.Run.Start += (orig, self) =>
			{
				SetCatalog();

				orig(self);
			};
		}

		internal static void SetOnLogBookControllerInit()
		{
			On.RoR2.UI.LogBook.LogBookController.Init += (orig) =>
			{
				SetCatalog();

				orig();
			};
		}

		private static void SetCatalog()
		{
			if (!set)
			{
				RiskOfRain.ApplyEquipmentIcons();
				RiskOfRain.UpdateEquipmentText();

				if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.zombieseatflesh7.dynamicbarrierdecay")) DynamicBarrierDecay.enabled = true;
				if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TPDespair.ZetTweaks")) DynamicBarrierDecay.slowed = true;

				if (EliteVariety.enabled)
				{
					EliteVariety.PopulateBuffs();
					EliteVariety.PopulateEquipment();
					EliteVariety.TinkerDroneBodyIndex = BodyCatalog.FindBodyIndex("EliteVariety_TinkererDroneBody");

					EliteVariety.CopyModelPrefabs();

					EliteVariety.ApplyEquipmentIcons();
					EliteVariety.UpdateEquipmentText();

					if (DynamicBarrierDecay.enabled || DynamicBarrierDecay.slowed) ZetAspectsPlugin.RegisterLanguageToken("ITEM_ZETASPECTARMOR_DESC", ZetAspectArmor.BuildDescription());

					EliteVariety.populated = true;
				}

				Debug.LogWarning("ZetAspects Catalog Set");

				set = true;
			}
		}



		public static class RiskOfRain
		{
			internal static void ApplyEquipmentIcons()
			{
				EquipmentIndex index;
				EquipmentDef def;

				index = EquipmentCatalog.FindEquipmentIndex("AffixWhite");
				if (index != EquipmentIndex.None)
				{
					def = EquipmentCatalog.GetEquipmentDef(index);
					def.pickupIconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixWhiteIconOrange.png");
				}

				index = EquipmentCatalog.FindEquipmentIndex("AffixBlue");
				if (index != EquipmentIndex.None)
				{
					def = EquipmentCatalog.GetEquipmentDef(index);
					def.pickupIconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBlueIconOrange.png");
				}

				index = EquipmentCatalog.FindEquipmentIndex("AffixRed");
				if (index != EquipmentIndex.None)
				{
					def = EquipmentCatalog.GetEquipmentDef(index);
					def.pickupIconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixRedIconOrange.png");
				}

				index = EquipmentCatalog.FindEquipmentIndex("AffixHaunted");
				if (index != EquipmentIndex.None)
				{
					def = EquipmentCatalog.GetEquipmentDef(index);
					def.pickupIconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixHauntedIconOrange.png");
				}

				index = EquipmentCatalog.FindEquipmentIndex("AffixPoison");
				if (index != EquipmentIndex.None)
				{
					def = EquipmentCatalog.GetEquipmentDef(index);
					def.pickupIconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPoisonIconOrange.png");
				}

				index = EquipmentCatalog.FindEquipmentIndex("AffixLunar");
				if (index != EquipmentIndex.None)
				{
					def = EquipmentCatalog.GetEquipmentDef(index);
					def.pickupIconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixLunarIconBlue.png");
				}
			}

			internal static void UpdateEquipmentText()
			{
				string text;
				float value = Configuration.AspectEquipmentEffect.Value;
				string stacks = "\n\nCounts as " + value + " stack" + (value == 1f ? "" : "s");
				string convertText = "\nClick bottom-right equipment icon to convert";

				bool activeEffect = AspectAbilities.enabled;
				bool showConvertInfo = Configuration.AspectEquipmentConversion.Value;

				text = "";
				if (activeEffect) text += "Deploy a health-reducing ice crystal on use.\n\n";
				text += ZetAspectIce.BuildDescription();
				text += stacks;
				if (showConvertInfo) text += convertText;
				ZetAspectsPlugin.RegisterLanguageToken("EQUIPMENT_AFFIXWHITE_DESC", text);

				text = "";
				if (activeEffect) text += "Teleport on use.\n\n";
				text += ZetAspectLightning.BuildDescription();
				text += stacks;
				if (showConvertInfo) text += convertText;
				ZetAspectsPlugin.RegisterLanguageToken("EQUIPMENT_AFFIXBLUE_DESC", text);

				text = "";
				if (activeEffect) text += "Release a barrage of seeking flame missiles on use.\n\n";
				text += ZetAspectFire.BuildDescription();
				text += stacks;
				if (showConvertInfo) text += convertText;
				ZetAspectsPlugin.RegisterLanguageToken("EQUIPMENT_AFFIXRED_DESC", text);

				text = "";
				if (activeEffect) text += "Heal all allies inside the invisibility aura on use.\n\n";
				text += ZetAspectCelestial.BuildDescription();
				text += stacks;
				if (showConvertInfo) text += convertText;
				ZetAspectsPlugin.RegisterLanguageToken("EQUIPMENT_AFFIXHAUNTED_DESC", text);

				text = "";
				if (activeEffect) text += "Summon an ally Malachite Urchin that inherits your items on use.\n\n";
				text += ZetAspectMalachite.BuildDescription();
				text += stacks;
				if (showConvertInfo) text += convertText;
				ZetAspectsPlugin.RegisterLanguageToken("EQUIPMENT_AFFIXPOISON_DESC", text);

				text = "";
				if (activeEffect) text += "Gain temporary defense from powerful attacks on use.\n\n";
				text += ZetAspectPerfect.BuildDescription();
				text += stacks;
				if (showConvertInfo) text += convertText;
				ZetAspectsPlugin.RegisterLanguageToken("EQUIPMENT_AFFIXLUNAR_DESC", text);
			}
		}

		public static class AspectAbilities
		{
			private static int state = -1;
			public static bool enabled
			{
				get
				{
					if (state == -1)
					{
						if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TheMysticSword.AspectAbilities")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}
		}

		public static class EliteVariety
		{
			public static bool populated = false;

			private static int state = -1;
			public static bool enabled
			{
				get
				{
					if (state == -1)
					{
						if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.themysticsword.elitevariety")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}

			internal static void PopulateBuffs()
			{
				BuffIndex index;

				index = BuffCatalog.FindBuffIndex("EliteVariety_AffixArmored");
				if (index != BuffIndex.None) Buffs.AffixArmored = BuffCatalog.GetBuffDef(index);

				index = BuffCatalog.FindBuffIndex("EliteVariety_AffixBuffing");
				if (index != BuffIndex.None) Buffs.AffixBuffing = BuffCatalog.GetBuffDef(index);

				index = BuffCatalog.FindBuffIndex("EliteVariety_AffixImpPlane");
				if (index != BuffIndex.None) Buffs.AffixImpPlane = BuffCatalog.GetBuffDef(index);

				index = BuffCatalog.FindBuffIndex("EliteVariety_AffixPillaging");
				if (index != BuffIndex.None) Buffs.AffixPillaging = BuffCatalog.GetBuffDef(index);

				index = BuffCatalog.FindBuffIndex("EliteVariety_AffixSandstorm");
				if (index != BuffIndex.None) Buffs.AffixSandstorm = BuffCatalog.GetBuffDef(index);

				index = BuffCatalog.FindBuffIndex("EliteVariety_AffixTinkerer");
				if (index != BuffIndex.None) Buffs.AffixTinkerer = BuffCatalog.GetBuffDef(index);
			}

			internal static void PopulateEquipment()
			{
				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixArmored");
				if (index != EquipmentIndex.None) Equipment.AffixArmored = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixBuffing");
				if (index != EquipmentIndex.None) Equipment.AffixBuffing = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixImpPlane");
				if (index != EquipmentIndex.None) Equipment.AffixImpPlane = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixPillaging");
				if (index != EquipmentIndex.None) Equipment.AffixPillaging = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixSandstorm");
				if (index != EquipmentIndex.None) Equipment.AffixSandstorm = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixTinkerer");
				if (index != EquipmentIndex.None) Equipment.AffixTinkerer = EquipmentCatalog.GetEquipmentDef(index);
			}

			internal static void CopyModelPrefabs()
			{
				PickupDef pickupDef;
				EquipmentDef equipDef;
				ItemDef itemDef;

				equipDef = Equipment.AffixArmored;
				if(equipDef)
				{
					itemDef = ZetAspectsContent.Items.ZetAspectArmor;
					itemDef.pickupModelPrefab = equipDef.pickupModelPrefab;
					pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(itemDef.itemIndex));
					pickupDef.displayPrefab = equipDef.pickupModelPrefab;
				}
				equipDef = Equipment.AffixBuffing;
				if (equipDef)
				{
					itemDef = ZetAspectsContent.Items.ZetAspectBanner;
					itemDef.pickupModelPrefab = equipDef.pickupModelPrefab;
					pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(itemDef.itemIndex));
					pickupDef.displayPrefab = equipDef.pickupModelPrefab;
				}
				equipDef = Equipment.AffixImpPlane;
				if (equipDef)
				{
					itemDef = ZetAspectsContent.Items.ZetAspectImpale;
					itemDef.pickupModelPrefab = equipDef.pickupModelPrefab;
					pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(itemDef.itemIndex));
					pickupDef.displayPrefab = equipDef.pickupModelPrefab;
				}
				equipDef = Equipment.AffixPillaging;
				if (equipDef)
				{
					itemDef = ZetAspectsContent.Items.ZetAspectGolden;
					itemDef.pickupModelPrefab = equipDef.pickupModelPrefab;
					pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(itemDef.itemIndex));
					pickupDef.displayPrefab = equipDef.pickupModelPrefab;
				}
				equipDef = Equipment.AffixSandstorm;
				if (equipDef)
				{
					itemDef = ZetAspectsContent.Items.ZetAspectCyclone;
					itemDef.pickupModelPrefab = equipDef.pickupModelPrefab;
					pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(itemDef.itemIndex));
					pickupDef.displayPrefab = equipDef.pickupModelPrefab;
				}
				equipDef = Equipment.AffixTinkerer;
				if (equipDef)
				{
					itemDef = ZetAspectsContent.Items.ZetAspectTinker;
					itemDef.pickupModelPrefab = equipDef.pickupModelPrefab;
					pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(itemDef.itemIndex));
					pickupDef.displayPrefab = equipDef.pickupModelPrefab;
				}
			}

			internal static void ApplyEquipmentIcons()
			{
				EquipmentIndex index;
				EquipmentDef def;

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixArmored");
				if (index != EquipmentIndex.None)
				{
					def = EquipmentCatalog.GetEquipmentDef(index);
					def.pickupIconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixArmoredIconOrange.png");
				}

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixBuffing");
				if (index != EquipmentIndex.None)
				{
					def = EquipmentCatalog.GetEquipmentDef(index);
					def.pickupIconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBuffingIconOrange.png");
				}

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixImpPlane");
				if (index != EquipmentIndex.None)
				{
					def = EquipmentCatalog.GetEquipmentDef(index);
					def.pickupIconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixImpPlaneIconOrange.png");
				}

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixPillaging");
				if (index != EquipmentIndex.None)
				{
					def = EquipmentCatalog.GetEquipmentDef(index);
					def.pickupIconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPillagingIconOrange.png");
				}

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixSandstorm");
				if (index != EquipmentIndex.None)
				{
					def = EquipmentCatalog.GetEquipmentDef(index);
					def.pickupIconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixSandstormIconOrange.png");
				}

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixTinkerer");
				if (index != EquipmentIndex.None)
				{
					def = EquipmentCatalog.GetEquipmentDef(index);
					def.pickupIconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixTinkererIconOrange.png");
				}
			}

			internal static void UpdateEquipmentText()
			{
				string text;
				float value = Configuration.AspectEquipmentEffect.Value;
				string stacks = "\n\nCounts as " + value + " stack" + (value == 1f ? "" : "s");
				string convertText = "\nClick bottom-right equipment icon to convert";

				bool activeEffect = AspectAbilities.enabled;
				bool showConvertInfo = Configuration.AspectEquipmentConversion.Value;

				text = "";
				if (activeEffect) text += "Gain temporary armor increase on use.\n\n";
				text += ZetAspectArmor.BuildDescription();
				text += stacks;
				if (showConvertInfo) text += convertText;
				ZetAspectsPlugin.RegisterLanguageToken("EQUIPMENT_ELITEVARIETY_AFFIXARMORED_DESC", text);

				text = "";
				if (activeEffect) text += "Increase banner radius on use.\n\n";
				text += ZetAspectBanner.BuildDescription();
				text += stacks;
				if (showConvertInfo) text += convertText;
				ZetAspectsPlugin.RegisterLanguageToken("EQUIPMENT_ELITEVARIETY_AFFIXBUFFING_DESC", text);

				text = "";
				if (activeEffect) text += "Teleport to a target and deal damage on use.\n\n";
				text += ZetAspectImpale.BuildDescription();
				text += stacks;
				if (showConvertInfo) text += convertText;
				ZetAspectsPlugin.RegisterLanguageToken("EQUIPMENT_ELITEVARIETY_AFFIXIMPPLANE_DESC", text);

				text = "";
				if (activeEffect) text += "Spend all of your gold for a random item. The more gold spent, the higher chance of getting a rarer item.\n\n";
				text += ZetAspectGolden.BuildDescription();
				text += stacks;
				if (showConvertInfo) text += convertText;
				ZetAspectsPlugin.RegisterLanguageToken("EQUIPMENT_ELITEVARIETY_AFFIXPILLAGING_DESC", text);

				text = "";
				if (activeEffect) text += "Dash on use, knocking nearby enemies up.\n\n";
				text += ZetAspectCyclone.BuildDescription();
				text += stacks;
				if (showConvertInfo) text += convertText;
				ZetAspectsPlugin.RegisterLanguageToken("EQUIPMENT_ELITEVARIETY_AFFIXSANDSTORM_DESC", text);

				text = "";
				if (activeEffect) text += "Heal drones on use.\n\n";
				text += ZetAspectTinker.BuildDescription();
				text += stacks;
				if (showConvertInfo) text += convertText;
				ZetAspectsPlugin.RegisterLanguageToken("EQUIPMENT_ELITEVARIETY_AFFIXTINKERER_DESC", text);
			}

			public static class Buffs
			{
				public static BuffDef AffixArmored;
				public static BuffDef AffixBuffing;
				public static BuffDef AffixImpPlane;
				public static BuffDef AffixPillaging;
				public static BuffDef AffixSandstorm;
				public static BuffDef AffixTinkerer;
			}

			public static class Equipment
			{
				public static EquipmentDef AffixArmored;
				public static EquipmentDef AffixBuffing;
				public static EquipmentDef AffixImpPlane;
				public static EquipmentDef AffixPillaging;
				public static EquipmentDef AffixSandstorm;
				public static EquipmentDef AffixTinkerer;
			}

			public static BodyIndex TinkerDroneBodyIndex = BodyIndex.None;
		}

		public static class DynamicBarrierDecay
		{
			public static bool enabled = false;
			public static bool slowed = false;
		}
	}
}
