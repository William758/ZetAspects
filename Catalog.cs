using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class Catalog
	{
		public static bool set = false;
		public static bool menu = false;

		public static List<ItemDef> disabledItemDefs = new List<ItemDef>();
		public static List<ItemIndex> disabledItemIndexes = new List<ItemIndex>();

		public static int barrierDecayMode = 0;
		public static bool limitChillStacks = false;
		public static bool borboFrostBlade = false;
		public static bool aspectAbilities = false;

		public static bool ChillCanStack => RoR2Content.Buffs.Slow80.canStack;



		internal static void Init()
		{
			OnTransmuteManagerInit();
			OnRuleCatalogInit();
			OnLogBookInit();
			OnMainMenuEnter();
		}

		private static void OnTransmuteManagerInit()
		{
			On.RoR2.PickupTransmutationManager.Init += (orig) =>
			{
				try
				{
					EliteVariety.PreInit();
					LostInTransit.PreInit();
					Aetherium.PreInit();
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}

				orig();
			};
		}

		private static void OnRuleCatalogInit()
		{
			On.RoR2.RuleCatalog.Init += (orig) =>
			{
				try
				{
					EliteVariety.PreInit();
					LostInTransit.PreInit();
					Aetherium.PreInit();

					if (Configuration.AspectWorldUnique.Value)
					{
						RiskOfRain.ItemEntries(false);
						if (EliteVariety.Enabled) EliteVariety.ItemEntries(false);
						if (LostInTransit.Enabled) LostInTransit.ItemEntries(false);
						if (Aetherium.Enabled) Aetherium.ItemEntries(false);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}

				orig();
			};
		}

		private static void OnLogBookInit()
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

		private static void OnMainMenuEnter()
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

			HandleDisabledItems();

			Debug.LogWarning("ZetAspects Catalog - Setup Complete");

			set = true;
		}

		private static void HandleDisabledItems()
		{
			foreach (RuleDef ruleDef in RuleCatalog.allRuleDefs)
			{
				if (ruleDef.choices != null && ruleDef.choices.Count > 0)
				{
					ItemIndex itemIndex = ruleDef.choices[0].itemIndex;

					if (itemIndex != ItemIndex.None && disabledItemIndexes.Contains(itemIndex))
					{
						foreach (RuleChoiceDef ruleDefChoice in ruleDef.choices) ruleDefChoice.excludeByDefault = true;
					}
				}
			}
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
				ItemEntries(DropHooks.CanObtainItem());

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

			internal static void ItemEntries(bool shown)
			{
				SetItemState(ZetAspectsContent.Items.ZetAspectIce, shown);
				SetItemState(ZetAspectsContent.Items.ZetAspectLightning, shown);
				SetItemState(ZetAspectsContent.Items.ZetAspectFire, shown);
				SetItemState(ZetAspectsContent.Items.ZetAspectCelestial, shown);
				SetItemState(ZetAspectsContent.Items.ZetAspectMalachite, shown);
				SetItemState(ZetAspectsContent.Items.ZetAspectPerfect, shown);
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

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(RoR2Content.Equipment.AffixWhite, shown);
				SetEquipmentState(RoR2Content.Equipment.AffixBlue, shown);
				SetEquipmentState(RoR2Content.Equipment.AffixRed, shown);
				SetEquipmentState(RoR2Content.Equipment.AffixHaunted, shown);
				SetEquipmentState(RoR2Content.Equipment.AffixPoison, shown);
				SetEquipmentState(RoR2Content.Equipment.AffixLunar, shown);
			}
		}

		public static class EliteVariety
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
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



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
				}
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
					ItemEntries(DropHooks.CanObtainItem());

					CopyModelPrefabs();
					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);

					EliteVarietyHooks.LateSetup();

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

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

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

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

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(ZetAspectsContent.Items.ZetAspectArmor, ref Equipment.AffixArmored, ref Buffs.AffixArmored, state);
				DisableInactiveItem(ZetAspectsContent.Items.ZetAspectBanner, ref Equipment.AffixBuffing, ref Buffs.AffixBuffing, state);
				DisableInactiveItem(ZetAspectsContent.Items.ZetAspectImpale, ref Equipment.AffixImpPlane, ref Buffs.AffixImpPlane, state);
				DisableInactiveItem(ZetAspectsContent.Items.ZetAspectGolden, ref Equipment.AffixPillaging, ref Buffs.AffixPillaging, state);
				DisableInactiveItem(ZetAspectsContent.Items.ZetAspectCyclone, ref Equipment.AffixSandstorm, ref Buffs.AffixSandstorm, state);
				DisableInactiveItem(ZetAspectsContent.Items.ZetAspectTinker, ref Equipment.AffixTinkerer, ref Buffs.AffixTinkerer, state);
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

			internal static void ItemEntries(bool shown)
			{
				SetItemState(ZetAspectsContent.Items.ZetAspectArmor, shown);
				SetItemState(ZetAspectsContent.Items.ZetAspectBanner, shown);
				SetItemState(ZetAspectsContent.Items.ZetAspectImpale, shown);
				SetItemState(ZetAspectsContent.Items.ZetAspectGolden, shown);
				SetItemState(ZetAspectsContent.Items.ZetAspectCyclone, shown);
				SetItemState(ZetAspectsContent.Items.ZetAspectTinker, shown);
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

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equipment.AffixArmored, shown);
				SetEquipmentState(Equipment.AffixBuffing, shown);
				SetEquipmentState(Equipment.AffixImpPlane, shown);
				SetEquipmentState(Equipment.AffixPillaging, shown);
				SetEquipmentState(Equipment.AffixSandstorm, shown);
				SetEquipmentState(Equipment.AffixTinkerer, shown);
			}
		}



		public static class LostInTransit
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
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



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
				}
			}

			internal static void Init()
			{
				if (Enabled)
				{
					PopulateEquipment();
					PopulateBuffs();

					DisableInactiveItems();
					SetupText();
					ItemEntries(DropHooks.CanObtainItem());

					CopyModelPrefabs();
					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("AffixLeeching");
				if (index != EquipmentIndex.None) Equipment.AffixLeeching = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("AffixFrenzied");
				if (index != EquipmentIndex.None) Equipment.AffixFrenzied = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("AffixVolatile");
				if (index != EquipmentIndex.None) Equipment.AffixVolatile = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("AffixBlighted");
				if (index != EquipmentIndex.None) Equipment.AffixBlighted = EquipmentCatalog.GetEquipmentDef(index);

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				BuffIndex index;

				index = BuffCatalog.FindBuffIndex("AffixLeeching");
				if (index != BuffIndex.None) Buffs.AffixLeeching = BuffCatalog.GetBuffDef(index);

				index = BuffCatalog.FindBuffIndex("AffixFrenzied");
				if (index != BuffIndex.None) Buffs.AffixFrenzied = BuffCatalog.GetBuffDef(index);

				index = BuffCatalog.FindBuffIndex("AffixVolatile");
				if (index != BuffIndex.None) Buffs.AffixVolatile = BuffCatalog.GetBuffDef(index);

				index = BuffCatalog.FindBuffIndex("AffixBlighted");
				if (index != BuffIndex.None) Buffs.AffixBlighted = BuffCatalog.GetBuffDef(index);

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				if (!LostInTransitHooks.blightBuffControl) DeactivateItem(ZetAspectsContent.Items.ZetAspectBlighted);

				DisableInactiveItem(ZetAspectsContent.Items.ZetAspectLeeching, ref Equipment.AffixLeeching, ref Buffs.AffixLeeching, state);
				DisableInactiveItem(ZetAspectsContent.Items.ZetAspectFrenzied, ref Equipment.AffixFrenzied, ref Buffs.AffixFrenzied, state);
				DisableInactiveItem(ZetAspectsContent.Items.ZetAspectVolatile, ref Equipment.AffixVolatile, ref Buffs.AffixVolatile, state);
				DisableInactiveItem(ZetAspectsContent.Items.ZetAspectBlighted, ref Equipment.AffixBlighted, ref Buffs.AffixBlighted, state);
			}

			private static void SetupText()
			{
				ZetAspectLeeching.SetupTokens();
				ZetAspectFrenzied.SetupTokens();
				ZetAspectVolatile.SetupTokens();
				ZetAspectBlighted.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(ZetAspectsContent.Items.ZetAspectLeeching, shown);
				SetItemState(ZetAspectsContent.Items.ZetAspectFrenzied, shown);
				SetItemState(ZetAspectsContent.Items.ZetAspectVolatile, shown);
				SetItemState(ZetAspectsContent.Items.ZetAspectBlighted, shown);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectLeeching, Equipment.AffixLeeching);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectFrenzied, Equipment.AffixFrenzied);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectVolatile, Equipment.AffixVolatile);
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectBlighted, Equipment.AffixBlighted);
			}

			private static void ApplyEquipmentIcons()
			{
				ReplaceEquipmentIcon(Equipment.AffixLeeching, ZetAspectsContent.Sprites.AffixLeeching, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixFrenzied, ZetAspectsContent.Sprites.AffixFrenzied, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixVolatile, ZetAspectsContent.Sprites.AffixVolatile, ZetAspectsContent.Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equipment.AffixBlighted, ZetAspectsContent.Sprites.AffixBlighted, ZetAspectsContent.Sprites.OutlineOrange);
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equipment.AffixLeeching, shown);
				SetEquipmentState(Equipment.AffixFrenzied, shown);
				SetEquipmentState(Equipment.AffixVolatile, shown);
				SetEquipmentState(Equipment.AffixBlighted, shown);
			}
		}



		public static class Aetherium
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
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



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
				}
			}

			internal static void Init()
			{
				if (Enabled)
				{
					PopulateEquipment();
					PopulateBuffs();

					DisableInactiveItems();
					SetupText();
					ItemEntries(DropHooks.CanObtainItem());

					CopyModelPrefabs();
					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("AETHERIUM_ELITE_EQUIPMENT_AFFIX_SANGUINE");
				if (index != EquipmentIndex.None) Equipment.AffixSanguine = EquipmentCatalog.GetEquipmentDef(index);

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				BuffIndex index;

				index = BuffCatalog.FindBuffIndex("AFFIX_SANGUINE");
				if (index != BuffIndex.None) Buffs.AffixSanguine = BuffCatalog.GetBuffDef(index);

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(ZetAspectsContent.Items.ZetAspectSanguine, ref Equipment.AffixSanguine, ref Buffs.AffixSanguine, state);
			}

			private static void SetupText()
			{
				ZetAspectSanguine.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(ZetAspectsContent.Items.ZetAspectSanguine, shown);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(ZetAspectsContent.Items.ZetAspectSanguine, Equipment.AffixSanguine);
			}

			private static void ApplyEquipmentIcons()
			{
				ReplaceEquipmentIcon(Equipment.AffixSanguine, ZetAspectsContent.Sprites.AffixSanguine, ZetAspectsContent.Sprites.OutlineOrange);
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equipment.AffixSanguine, shown);
			}
		}



		private static int GetPopulatedState(bool equip, bool buff)
		{
			int value = 0;

			if (equip) value += 1;
			if (buff) value += 2;

			return value;
		}

		private static void DisableInactiveItem(ItemDef itemDef, ref EquipmentDef equipDef, ref BuffDef buffDeff, int state)
		{
			if (!itemDef) return;

			if (state > 0)
			{
				if ((state & 1) > 0) DeactivateItem(itemDef, equipDef);
				if ((state & 2) > 0) DeactivateItem(itemDef, buffDeff);
			}
			else
			{
				Debug.LogWarning("ZetAspects Catalog - Tried to disable " + itemDef.name + " without any BuffDefs or EquipmentDefs populated!");
			}

			if (disabledItemDefs.Contains(itemDef))
			{
				if (equipDef != null) equipDef = null;
				if (buffDeff != null) buffDeff = null;
			}
		}

		private static void DeactivateItem(ItemDef itemDef, EquipmentDef equipDef)
		{
			if (disabledItemDefs.Contains(itemDef)) return;

			if (!equipDef)
			{
				Debug.LogWarning(itemDef.name + " : associated equipment not found!");
				DeactivateItem(itemDef);
			}
		}

		private static void DeactivateItem(ItemDef itemDef, BuffDef buffDeff)
		{
			if (disabledItemDefs.Contains(itemDef)) return;

			if (!buffDeff)
			{
				Debug.LogWarning(itemDef.name + " : associated buff not found!");
				DeactivateItem(itemDef);
			}
		}

		private static void DeactivateItem(ItemDef itemDef)
		{
			if (disabledItemDefs.Contains(itemDef)) return;

			Debug.LogWarning("ZetAspects - Deactivating : " + itemDef.name);

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

			disabledItemDefs.Add(itemDef);
			disabledItemIndexes.Add(itemDef.itemIndex);
		}



		private static void SetItemState(ItemDef itemDef, bool shown)
		{
			if (itemDef && !itemDef.hidden)
			{
				if (!shown) itemDef.tier = ItemTier.NoTier;
				else itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			}
		}

		public static void CopyEquipmentPrefab(ItemDef itemDef, EquipmentDef equipDef)
		{
			if (!itemDef || disabledItemDefs.Contains(itemDef)) return;

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

		private static void SetEquipmentState(EquipmentDef equipDef, bool canDrop)
		{
			if (equipDef) equipDef.canDrop = canDrop;
		}



		public static bool PluginLoaded(string key)
		{
			return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(key);
		}
	}
}
