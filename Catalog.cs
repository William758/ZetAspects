using RoR2;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class Catalog
	{
		public static bool set = false;



		public static int barrierDecayMode = 0;
		public static bool limitChillStacks = false;
		public static bool aspectAbilities = false;



		public static bool ChillCanStack
		{
			get
			{
				return RoR2Content.Buffs.Slow80.canStack;
			}
		}



		internal static void SetOnLogBookControllerInit()
		{
			On.RoR2.UI.LogBook.LogBookController.Init += (orig) =>
			{
				try
				{
					SetCatalog();
				}
				catch (Exception ex)
				{
					Debug.Log("Failed To Setup ZetAspects Catalog!");
					Debug.LogError(ex);
				}

				orig();
			};
		}

		internal static void SetCatalog()
		{
			if (PluginLoaded("com.zombieseatflesh7.dynamicbarrierdecay")) barrierDecayMode = 2;
			else if(PluginLoaded("com.TPDespair.StatAdjustment")) barrierDecayMode = 1;

			if (PluginLoaded("com.Borbo.ArtificerExtended")) limitChillStacks = true;
			if (PluginLoaded("com.TheMysticSword.AspectAbilities")) aspectAbilities = true;

			RiskOfRain.Init();
			EliteVariety.Init();
			LostInTransit.Init();

			Debug.LogWarning("ZetAspects Catalog Set");

			set = true;
		}



		public static class RiskOfRain
		{
			public static void Init()
			{
				SetupText();

				ApplyEquipmentIcons();
			}



			internal static void SetupText()
			{
				ZetAspectIce.SetupTokens();
				ZetAspectLightning.SetupTokens();
				ZetAspectFire.SetupTokens();
				ZetAspectCelestial.SetupTokens();
				ZetAspectMalachite.SetupTokens();
				ZetAspectPerfect.SetupTokens();
			}

			internal static void ApplyEquipmentIcons()
			{
				ReplaceEquipmentIcon(RoR2Content.Equipment.AffixWhite, ZetAspectsContent.Sprites.AffixWhite, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(RoR2Content.Equipment.AffixBlue, ZetAspectsContent.Sprites.AffixBlue, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(RoR2Content.Equipment.AffixRed, ZetAspectsContent.Sprites.AffixRed, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(RoR2Content.Equipment.AffixHaunted, ZetAspectsContent.Sprites.AffixHaunted, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(RoR2Content.Equipment.AffixPoison, ZetAspectsContent.Sprites.AffixPoison, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(RoR2Content.Equipment.AffixLunar, ZetAspectsContent.Sprites.AffixLunar, ZetAspectsContent.Sprites.OutlineBlue);
			}
		}

		public static class EliteVariety
		{
			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("com.themysticsword.elitevariety")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			public static BodyIndex tinkerDroneBodyIndex = BodyIndex.None;

			public static class Equipment
			{
				public static EquipmentDef AffixArmored;
				public static EquipmentDef AffixBuffing;
				public static EquipmentDef AffixImpPlane;
				public static EquipmentDef AffixPillaging;
				public static EquipmentDef AffixSandstorm;
				public static EquipmentDef AffixTinkerer;
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



			public static void Init()
			{
				if (Enabled)
				{
					SetupText();

					PopulateEquipment();
					PopulateBuffs();
					tinkerDroneBodyIndex = BodyCatalog.FindBodyIndex("EliteVariety_TinkererDroneBody");

					DisableInactiveItems();

					CopyModelPrefabs();
					ApplyEquipmentIcons();

					populated = true;
				}
			}



			internal static void SetupText()
			{
				ZetAspectArmor.SetupTokens();
				ZetAspectBanner.SetupTokens();
				ZetAspectImpale.SetupTokens();
				ZetAspectGolden.SetupTokens();
				ZetAspectCyclone.SetupTokens();
				ZetAspectTinker.SetupTokens();
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

			internal static void DisableInactiveItems()
			{
				DeactivateItem(ZetAspectsContent.Items.ZetAspectArmor, ref Equipment.AffixArmored, ref Buffs.AffixArmored);
				DeactivateItem(ZetAspectsContent.Items.ZetAspectBanner, ref Equipment.AffixBuffing, ref Buffs.AffixBuffing);
				DeactivateItem(ZetAspectsContent.Items.ZetAspectImpale, ref Equipment.AffixImpPlane, ref Buffs.AffixImpPlane);
				DeactivateItem(ZetAspectsContent.Items.ZetAspectGolden, ref Equipment.AffixPillaging, ref Buffs.AffixPillaging);
				DeactivateItem(ZetAspectsContent.Items.ZetAspectCyclone, ref Equipment.AffixSandstorm, ref Buffs.AffixSandstorm);
				DeactivateItem(ZetAspectsContent.Items.ZetAspectTinker, ref Equipment.AffixTinkerer, ref Buffs.AffixTinkerer);
			}

			internal static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectArmor, Equipment.AffixArmored);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectBanner, Equipment.AffixBuffing);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectImpale, Equipment.AffixImpPlane);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectGolden, Equipment.AffixPillaging);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectCyclone, Equipment.AffixSandstorm);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectTinker, Equipment.AffixTinkerer);
			}

			internal static void ApplyEquipmentIcons()
			{
				ReplaceEquipmentIcon(Equipment.AffixArmored, ZetAspectsContent.Sprites.AffixArmored, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixBuffing, ZetAspectsContent.Sprites.AffixBuffing, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixImpPlane, ZetAspectsContent.Sprites.AffixImpPlane, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixPillaging, ZetAspectsContent.Sprites.AffixPillaging, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixSandstorm, ZetAspectsContent.Sprites.AffixSandstorm, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixTinkerer, ZetAspectsContent.Sprites.AffixTinkerer, ZetAspectsContent.Sprites.OutlineOrange);
			}
		}



		public static class LostInTransit
		{
			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("com.swuff.LostInTransit")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			public static class Equipment
			{
				public static EquipmentDef AffixLeeching;
				public static EquipmentDef AffixFrenzied;
			}

			public static class Buffs
			{
				public static BuffDef AffixLeeching;
				public static BuffDef AffixFrenzied;
			}



			public static void Init()
			{
				if (Enabled)
				{
					SetupText();

					PopulateEquipment();
					PopulateBuffs();

					DisableInactiveItems();

					CopyModelPrefabs();
					ApplyEquipmentIcons();

					populated = true;
				}
			}



			internal static void SetupText()
			{
				ZetAspectLeeching.SetupTokens();
				ZetAspectFrenzied.SetupTokens();
			}

			internal static void PopulateEquipment()
			{
				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("AffixLeeching");
				if (index != EquipmentIndex.None) Equipment.AffixLeeching = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("AffixFrenzied");
				if (index != EquipmentIndex.None) Equipment.AffixFrenzied = EquipmentCatalog.GetEquipmentDef(index);
			}

			internal static void PopulateBuffs()
			{
				BuffIndex index;

				index = BuffCatalog.FindBuffIndex("AffixLeeching");
				if (index != BuffIndex.None) Buffs.AffixLeeching = BuffCatalog.GetBuffDef(index);

				index = BuffCatalog.FindBuffIndex("AffixFrenzied");
				if (index != BuffIndex.None) Buffs.AffixFrenzied = BuffCatalog.GetBuffDef(index);
			}

			internal static void DisableInactiveItems()
			{
				DeactivateItem(ZetAspectsContent.Items.ZetAspectLeeching, ref Equipment.AffixLeeching, ref Buffs.AffixLeeching);
				DeactivateItem(ZetAspectsContent.Items.ZetAspectFrenzied, ref Equipment.AffixFrenzied, ref Buffs.AffixFrenzied);
			}

			internal static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectLeeching, Equipment.AffixLeeching);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectFrenzied, Equipment.AffixFrenzied);
			}

			internal static void ApplyEquipmentIcons()
			{
				ReplaceEquipmentIcon(Equipment.AffixLeeching, ZetAspectsContent.Sprites.AffixLeeching, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixFrenzied, ZetAspectsContent.Sprites.AffixFrenzied, ZetAspectsContent.Sprites.OutlineOrange);
			}
		}



		public static void DeactivateItem(ItemDef itemDef, ref EquipmentDef equipDef, ref BuffDef buffDeff)
		{
			if (!itemDef) return;

			bool deactivate = false;

			if (!equipDef)
			{
				deactivate = true;
				Debug.LogWarning(itemDef.name + " : associated equipment not found!");
			}

			if (!buffDeff)
			{
				deactivate = true;
				Debug.LogWarning(itemDef.name + " : associated buff not found!");
			}

			if (deactivate)
			{
				Debug.LogWarning("ZetAspects - Deactivating : " + itemDef.name);

				equipDef = null;
				buffDeff = null;

				itemDef.tier = ItemTier.NoTier;
				itemDef.hidden = true;
				if (itemDef.DoesNotContainTag(ItemTag.WorldUnique))
				{
					ItemTag[] tags = itemDef.tags;
					int index = tags.Length;

					Array.Resize(ref tags, index + 1);
					tags[index] = ItemTag.WorldUnique;

					itemDef.tags = tags;
				}
			}
		}



		public static void CopyEquipmentPrefab(ItemDef itemDef, EquipmentDef equipDef)
		{
			if (!itemDef) return;

			if (!equipDef)
			{
				Debug.LogWarning("ZetAspects - Could not copy model prefab for " + itemDef.name + " because its associated equipment was not found!");
				return;
			}

			itemDef.pickupModelPrefab = equipDef.pickupModelPrefab;
			PickupDef pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(itemDef.itemIndex));
			pickupDef.displayPrefab = equipDef.pickupModelPrefab;
		}

		public static void ReplaceEquipmentIcon(EquipmentDef equipDef, Sprite baseSprite, Sprite outlineSprite)
		{
			if (equipDef) equipDef.pickupIconSprite = ZetAspectsPlugin.CreateAspectSprite(baseSprite, outlineSprite);
		}



		public static bool PluginLoaded(string key)
		{
			return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(key);
		}
	}
}
