using RoR2;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class Catalog
	{
		public static bool set = false;
		public static bool menu = false;



		public static int barrierDecayMode = 0;
		public static bool limitChillStacks = false;
		public static bool borboFrostBlade = false;
		public static bool aspectAbilities = false;



		public static bool ChillCanStack
		{
			get
			{
				return RoR2Content.Buffs.Slow80.canStack;
			}
		}



		internal static void OnLogBookInit()
		{
			On.RoR2.UI.LogBook.LogBookController.Init += (orig) =>
			{
				try
				{
					SetupCatalog();
				}
				catch (Exception ex)
				{
					Debug.Log("Failed To Setup ZetAspects Catalog!");
					Debug.LogError(ex);
				}

				orig();
			};
		}

		internal static void OnMainMenuEnter()
		{
			On.RoR2.UI.MainMenu.BaseMainMenuScreen.OnEnter += (orig, self, controller) =>
			{
				orig(self, controller);

				try
				{
					if (!menu) FirstMenuVisit();
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}
			};
		}



		private static void SetupCatalog()
		{
			if (PluginLoaded("com.zombieseatflesh7.dynamicbarrierdecay")) barrierDecayMode = 2;
			else if(PluginLoaded("com.TPDespair.StatAdjustment")) barrierDecayMode = 1;

			if (PluginLoaded("com.Borbo.ArtificerExtended")) limitChillStacks = true;
			if (PluginLoaded("com.Borbo.BORBO")) borboFrostBlade = true;
			if (PluginLoaded("com.TheMysticSword.AspectAbilities")) aspectAbilities = true;

			RiskOfRain.Init();
			EliteVariety.Init();
			LostInTransit.Init();
			Aetherium.Init();

			Debug.LogWarning("ZetAspects Catalog - Setup Complete");

			set = true;
		}

		private static void FirstMenuVisit()
		{
			Debug.LogWarning("ZetAspects ItemObtainable : " + DropHooks.CanObtainItem());
			Debug.LogWarning("ZetAspects EquipObtainable : " + DropHooks.CanObtainEquipment());

			RiskOfRain.ItemEntries(true);
			RiskOfRain.EquipmentEntries(false);

			if (EliteVariety.populated)
			{
				EliteVariety.ItemEntries(true);
				EliteVariety.EquipmentEntries(false);
			}
			if (LostInTransit.populated)
			{
				LostInTransit.ItemEntries(true);
				LostInTransit.EquipmentEntries(false);
			}
			if (Aetherium.populated)
			{
				Aetherium.ItemEntries(true);
				Aetherium.EquipmentEntries(false);
			}

			Debug.LogWarning("ZetAspects Catalog - Reset Entries");

			menu = true;
		}



		public static class RiskOfRain
		{
			internal static void Init()
			{
				SetupText();
				if (!DropHooks.CanObtainItem()) ItemEntries(false);

				ApplyEquipmentIcons();
				if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
			}



			private static void SetupText()
			{
				ZetAspectIce.SetupTokens();
				ZetAspectLightning.SetupTokens();
				ZetAspectFire.SetupTokens();
				ZetAspectCelestial.SetupTokens();
				ZetAspectMalachite.SetupTokens();
				ZetAspectPerfect.SetupTokens();
			}

			internal static void ItemEntries(bool value)
			{
				SetItemState(ZetAspectsContent.Items.ZetAspectIce, value);
				SetItemState(ZetAspectsContent.Items.ZetAspectLightning, value);
				SetItemState(ZetAspectsContent.Items.ZetAspectFire, value);
				SetItemState(ZetAspectsContent.Items.ZetAspectCelestial, value);
				SetItemState(ZetAspectsContent.Items.ZetAspectMalachite, value);
				SetItemState(ZetAspectsContent.Items.ZetAspectPerfect, value);
			}

			private static void ApplyEquipmentIcons()
			{
				ReplaceEquipmentIcon(RoR2Content.Equipment.AffixWhite, ZetAspectsContent.Sprites.AffixWhite, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(RoR2Content.Equipment.AffixBlue, ZetAspectsContent.Sprites.AffixBlue, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(RoR2Content.Equipment.AffixRed, ZetAspectsContent.Sprites.AffixRed, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(RoR2Content.Equipment.AffixHaunted, ZetAspectsContent.Sprites.AffixHaunted, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(RoR2Content.Equipment.AffixPoison, ZetAspectsContent.Sprites.AffixPoison, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(RoR2Content.Equipment.AffixLunar, ZetAspectsContent.Sprites.AffixLunar, ZetAspectsContent.Sprites.OutlineBlue);
			}

			internal static void EquipmentEntries(bool value)
			{
				SetEquipmentDropField(RoR2Content.Equipment.AffixWhite, value);
				SetEquipmentDropField(RoR2Content.Equipment.AffixBlue, value);
				SetEquipmentDropField(RoR2Content.Equipment.AffixRed, value);
				SetEquipmentDropField(RoR2Content.Equipment.AffixHaunted, value);
				SetEquipmentDropField(RoR2Content.Equipment.AffixPoison, value);
				SetEquipmentDropField(RoR2Content.Equipment.AffixLunar, value);
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
			public static DotController.DotIndex impaleDotIndex = DotController.DotIndex.None;

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



			internal static void Init()
			{
				if (Enabled)
				{
					PopulateEquipment();
					PopulateBuffs();
					tinkerDroneBodyIndex = BodyCatalog.FindBodyIndex("EliteVariety_TinkererDroneBody");

					DisableInactiveItems();
					SetupText();
					if (!DropHooks.CanObtainItem()) ItemEntries(false);

					CopyModelPrefabs();
					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);

					EliteVarietyHooks.LateSetup();

					populated = true;
				}
			}



			private static void PopulateEquipment()
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

			private static void PopulateBuffs()
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



			private static void DisableInactiveItems()
			{
				DeactivateItem(ZetAspectsContent.Items.ZetAspectArmor, ref Equipment.AffixArmored, ref Buffs.AffixArmored);
				DeactivateItem(ZetAspectsContent.Items.ZetAspectBanner, ref Equipment.AffixBuffing, ref Buffs.AffixBuffing);
				DeactivateItem(ZetAspectsContent.Items.ZetAspectImpale, ref Equipment.AffixImpPlane, ref Buffs.AffixImpPlane);
				DeactivateItem(ZetAspectsContent.Items.ZetAspectGolden, ref Equipment.AffixPillaging, ref Buffs.AffixPillaging);
				DeactivateItem(ZetAspectsContent.Items.ZetAspectCyclone, ref Equipment.AffixSandstorm, ref Buffs.AffixSandstorm);
				DeactivateItem(ZetAspectsContent.Items.ZetAspectTinker, ref Equipment.AffixTinkerer, ref Buffs.AffixTinkerer);
			}

			private static void SetupText()
			{
				ZetAspectArmor.SetupTokens();
				ZetAspectBanner.SetupTokens();
				ZetAspectImpale.SetupTokens();
				ZetAspectGolden.SetupTokens();
				ZetAspectCyclone.SetupTokens();
				ZetAspectTinker.SetupTokens();
			}

			internal static void ItemEntries(bool value)
			{
				SetItemState(ZetAspectsContent.Items.ZetAspectArmor, value);
				SetItemState(ZetAspectsContent.Items.ZetAspectBanner, value);
				SetItemState(ZetAspectsContent.Items.ZetAspectImpale, value);
				SetItemState(ZetAspectsContent.Items.ZetAspectGolden, value);
				SetItemState(ZetAspectsContent.Items.ZetAspectCyclone, value);
				SetItemState(ZetAspectsContent.Items.ZetAspectTinker, value);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectArmor, Equipment.AffixArmored);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectBanner, Equipment.AffixBuffing);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectImpale, Equipment.AffixImpPlane);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectGolden, Equipment.AffixPillaging);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectCyclone, Equipment.AffixSandstorm);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectTinker, Equipment.AffixTinkerer);
			}

			private static void ApplyEquipmentIcons()
			{
				ReplaceEquipmentIcon(Equipment.AffixArmored, ZetAspectsContent.Sprites.AffixArmored, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixBuffing, ZetAspectsContent.Sprites.AffixBuffing, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixImpPlane, ZetAspectsContent.Sprites.AffixImpPlane, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixPillaging, ZetAspectsContent.Sprites.AffixPillaging, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixSandstorm, ZetAspectsContent.Sprites.AffixSandstorm, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixTinkerer, ZetAspectsContent.Sprites.AffixTinkerer, ZetAspectsContent.Sprites.OutlineOrange);
			}

			internal static void EquipmentEntries(bool value)
			{
				SetEquipmentDropField(Equipment.AffixArmored, value);
				SetEquipmentDropField(Equipment.AffixBuffing, value);
				SetEquipmentDropField(Equipment.AffixImpPlane, value);
				SetEquipmentDropField(Equipment.AffixPillaging, value);
				SetEquipmentDropField(Equipment.AffixSandstorm, value);
				SetEquipmentDropField(Equipment.AffixTinkerer, value);
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
				public static EquipmentDef AffixVolatile;
				public static EquipmentDef AffixBlighted;
			}

			public static class Buffs
			{
				public static BuffDef AffixLeeching;
				public static BuffDef AffixFrenzied;
				public static BuffDef AffixVolatile;
				public static BuffDef AffixBlighted;
			}



			internal static void Init()
			{
				if (Enabled)
				{
					PopulateEquipment();
					PopulateBuffs();

					DisableInactiveItems();
					SetupText();
					if (!DropHooks.CanObtainItem()) ItemEntries(false);

					CopyModelPrefabs();
					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("AffixLeeching");
				if (index != EquipmentIndex.None) Equipment.AffixLeeching = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("AffixFrenzied");
				if (index != EquipmentIndex.None) Equipment.AffixFrenzied = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("AffixVolatile");
				if (index != EquipmentIndex.None) Equipment.AffixVolatile = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("AffixBlighted");
				if (index != EquipmentIndex.None) Equipment.AffixBlighted = EquipmentCatalog.GetEquipmentDef(index);
			}

			private static void PopulateBuffs()
			{
				BuffIndex index;

				index = BuffCatalog.FindBuffIndex("AffixLeeching");
				if (index != BuffIndex.None) Buffs.AffixLeeching = BuffCatalog.GetBuffDef(index);

				index = BuffCatalog.FindBuffIndex("AffixFrenzied");
				if (index != BuffIndex.None) Buffs.AffixFrenzied = BuffCatalog.GetBuffDef(index);

				index = BuffCatalog.FindBuffIndex("AffixVolatile");
				if (index != BuffIndex.None) Buffs.AffixVolatile = BuffCatalog.GetBuffDef(index);

				index = BuffCatalog.FindBuffIndex("AffixBlighted");
				if (index != BuffIndex.None) Buffs.AffixBlighted = BuffCatalog.GetBuffDef(index);
			}



			private static void DisableInactiveItems()
			{
				DeactivateItem(ZetAspectsContent.Items.ZetAspectLeeching, ref Equipment.AffixLeeching, ref Buffs.AffixLeeching);
				DeactivateItem(ZetAspectsContent.Items.ZetAspectFrenzied, ref Equipment.AffixFrenzied, ref Buffs.AffixFrenzied);
				DeactivateItem(ZetAspectsContent.Items.ZetAspectVolatile, ref Equipment.AffixVolatile, ref Buffs.AffixVolatile);
			}

			private static void SetupText()
			{
				ZetAspectLeeching.SetupTokens();
				ZetAspectFrenzied.SetupTokens();
				ZetAspectVolatile.SetupTokens();
			}

			internal static void ItemEntries(bool value)
			{
				SetItemState(ZetAspectsContent.Items.ZetAspectLeeching, value);
				SetItemState(ZetAspectsContent.Items.ZetAspectFrenzied, value);
				SetItemState(ZetAspectsContent.Items.ZetAspectVolatile, value);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectLeeching, Equipment.AffixLeeching);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectFrenzied, Equipment.AffixFrenzied);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectVolatile, Equipment.AffixVolatile);
			}

			private static void ApplyEquipmentIcons()
			{
				ReplaceEquipmentIcon(Equipment.AffixLeeching, ZetAspectsContent.Sprites.AffixLeeching, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixFrenzied, ZetAspectsContent.Sprites.AffixFrenzied, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixVolatile, ZetAspectsContent.Sprites.AffixVolatile, ZetAspectsContent.Sprites.OutlineOrange);
			}

			internal static void EquipmentEntries(bool value)
			{
				SetEquipmentDropField(Equipment.AffixLeeching, value);
				SetEquipmentDropField(Equipment.AffixFrenzied, value);
				SetEquipmentDropField(Equipment.AffixVolatile, value);
			}
		}



		public static class Aetherium
		{
			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("com.KomradeSpectre.Aetherium")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			public static class Equipment
			{
				public static EquipmentDef AffixSanguine;
			}

			public static class Buffs
			{
				public static BuffDef AffixSanguine;
			}



			internal static void Init()
			{
				if (Enabled)
				{
					PopulateEquipment();
					PopulateBuffs();

					DisableInactiveItems();
					SetupText();
					if (!DropHooks.CanObtainItem()) ItemEntries(false);

					CopyModelPrefabs();
					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("AETHERIUM_ELITE_EQUIPMENT_AFFIX_SANGUINE");
				if (index != EquipmentIndex.None) Equipment.AffixSanguine = EquipmentCatalog.GetEquipmentDef(index);
			}

			private static void PopulateBuffs()
			{
				BuffIndex index;

				index = BuffCatalog.FindBuffIndex("AFFIX_SANGUINE");
				if (index != BuffIndex.None) Buffs.AffixSanguine = BuffCatalog.GetBuffDef(index);
			}



			private static void DisableInactiveItems()
			{
				DeactivateItem(ZetAspectsContent.Items.ZetAspectSanguine, ref Equipment.AffixSanguine, ref Buffs.AffixSanguine);
			}

			private static void SetupText()
			{
				ZetAspectSanguine.SetupTokens();
			}

			internal static void ItemEntries(bool value)
			{
				SetItemState(ZetAspectsContent.Items.ZetAspectSanguine, value);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectSanguine, Equipment.AffixSanguine);
			}

			private static void ApplyEquipmentIcons()
			{
				ReplaceEquipmentIcon(Equipment.AffixSanguine, ZetAspectsContent.Sprites.AffixSanguine, ZetAspectsContent.Sprites.OutlineOrange);
			}

			internal static void EquipmentEntries(bool value)
			{
				SetEquipmentDropField(Equipment.AffixSanguine, value);
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

		internal static void SetItemState(ItemDef itemDef, bool shown)
		{
			if (itemDef && !itemDef.hidden)
			{
				if (!shown) itemDef.tier = ItemTier.NoTier;
				else itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
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

		internal static void SetEquipmentDropField(EquipmentDef equipDef, bool canDrop)
		{
			if (equipDef) equipDef.canDrop = canDrop;
		}



		public static bool PluginLoaded(string key)
		{
			return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(key);
		}
	}
}
