﻿using UnityEngine;
using RoR2;

namespace TPDespair.ZetAspects
{
	public static partial class Catalog
	{
		public static class RiskOfRain
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;



			internal static void PreInit()
			{
				PopulateEquipment();
				ApplyEquipmentIcons();
			}

			internal static void Init()
			{
				mithrixBodyIndex = BodyCatalog.FindBodyIndex("BrotherBody");
				voidlingBodyIndex = BodyCatalog.FindBodyIndex("VoidRaidCrabBody");
				urchinTurretBodyIndex = BodyCatalog.FindBodyIndex("UrchinTurretBody");
				healOrbBodyIndex = BodyCatalog.FindBodyIndex("AffixEarthHealerBody");
				artifactShellBodyIndex = BodyCatalog.FindBodyIndex("ArtifactShellBody");
				goldenTitanBodyIndex = BodyCatalog.FindBodyIndex("TitanGoldBody");

				PopulateEquipment();
				PopulateBuffs();

				SetupText();
				ItemEntries(DropHooks.CanObtainItem());

				CopyExpansionReq();
				CopyModelPrefabs();

				ApplyEquipmentIcons();
				if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
				EquipmentColor();

				BuffDef buffDef = RoR2Content.Buffs.AffixHauntedRecipient;
				buffDef.buffColor = Color.white;
				buffDef.iconSprite = Sprites.HauntCloak;

				FillEqualities();

				populated = true;
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				Equip.AffixWhite = RoR2Content.Equipment.AffixWhite;
				Equip.AffixBlue = RoR2Content.Equipment.AffixBlue;
				Equip.AffixRed = RoR2Content.Equipment.AffixRed;
				Equip.AffixHaunted = RoR2Content.Equipment.AffixHaunted;
				Equip.AffixPoison = RoR2Content.Equipment.AffixPoison;
				Equip.AffixLunar = RoR2Content.Equipment.AffixLunar;

				Equip.AffixEarth = EquipmentCatalog.GetEquipmentDef(EquipmentCatalog.FindEquipmentIndex("EliteEarthEquipment"));
				Equip.AffixVoid = EquipmentCatalog.GetEquipmentDef(EquipmentCatalog.FindEquipmentIndex("EliteVoidEquipment"));

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				Buff.AffixWhite = RoR2Content.Buffs.AffixWhite;
				Buff.AffixBlue = RoR2Content.Buffs.AffixBlue;
				Buff.AffixRed = RoR2Content.Buffs.AffixRed;
				Buff.AffixHaunted = RoR2Content.Buffs.AffixHaunted;
				Buff.AffixPoison = RoR2Content.Buffs.AffixPoison;
				Buff.AffixLunar = RoR2Content.Buffs.AffixLunar;

				Buff.AffixEarth = DLC1Content.Buffs.EliteEarth;
				Buff.AffixVoid = DLC1Content.Buffs.EliteVoid;

				buffDefPopulated = true;
			}



			private static void SetupText()
			{
				Items.ZetAspectWhite.SetupTokens();
				Items.ZetAspectBlue.SetupTokens();
				Items.ZetAspectRed.SetupTokens();
				Items.ZetAspectHaunted.SetupTokens();
				Items.ZetAspectPoison.SetupTokens();
				Items.ZetAspectLunar.SetupTokens();

				Items.ZetAspectEarth.SetupTokens();
				Items.ZetAspectVoid.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectWhite, shown);
				SetItemState(Item.ZetAspectBlue, shown);
				SetItemState(Item.ZetAspectRed, shown);
				SetItemState(Item.ZetAspectHaunted, shown);
				SetItemState(Item.ZetAspectPoison, shown);
				SetItemState(Item.ZetAspectLunar, shown);

				SetItemState(Item.ZetAspectEarth, shown);
				SetItemState(Item.ZetAspectVoid, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectWhite, Equip.AffixWhite);
				CopyExpansion(Item.ZetAspectBlue, Equip.AffixBlue);
				CopyExpansion(Item.ZetAspectRed, Equip.AffixRed);
				CopyExpansion(Item.ZetAspectHaunted, Equip.AffixHaunted);
				CopyExpansion(Item.ZetAspectPoison, Equip.AffixPoison);
				CopyExpansion(Item.ZetAspectLunar, Equip.AffixLunar);

				CopyExpansion(Item.ZetAspectEarth, Equip.AffixEarth);
				CopyExpansion(Item.ZetAspectVoid, Equip.AffixVoid);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectEarth, Equip.AffixEarth);
				CopyItemPrefab(Item.ZetAspectVoid, Equip.AffixVoid);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixWhite, Sprites.AffixWhite, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixBlue, Sprites.AffixBlue, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixRed, Sprites.AffixRed, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixHaunted, Sprites.AffixHaunted, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixPoison, Sprites.AffixPoison, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixLunar, Sprites.AffixLunar, Sprites.OutlineBlue);

				ReplaceEquipmentIcon(Equip.AffixEarth, Sprites.AffixEarth, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixVoid, Sprites.AffixVoid, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixWhite, shown);
				SetEquipmentState(Equip.AffixBlue, shown);
				SetEquipmentState(Equip.AffixRed, shown);
				SetEquipmentState(Equip.AffixHaunted, shown);
				SetEquipmentState(Equip.AffixPoison, shown);
				SetEquipmentState(Equip.AffixLunar, shown);

				SetEquipmentState(Equip.AffixEarth, shown);
				SetEquipmentState(Equip.AffixVoid, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixWhite);
				ColorEquipmentDroplet(Equip.AffixBlue);
				ColorEquipmentDroplet(Equip.AffixRed);
				ColorEquipmentDroplet(Equip.AffixHaunted);
				ColorEquipmentDroplet(Equip.AffixPoison);
				ColorEquipmentDroplet(Equip.AffixLunar);

				ColorEquipmentDroplet(Equip.AffixEarth);
				ColorEquipmentDroplet(Equip.AffixVoid);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixWhite, Buff.AffixWhite, Item.ZetAspectWhite);
				CreateEquality(Equip.AffixBlue, Buff.AffixBlue, Item.ZetAspectBlue);
				CreateEquality(Equip.AffixRed, Buff.AffixRed, Item.ZetAspectRed);
				CreateEquality(Equip.AffixHaunted, Buff.AffixHaunted, Item.ZetAspectHaunted);
				CreateEquality(Equip.AffixPoison, Buff.AffixPoison, Item.ZetAspectPoison);
				CreateEquality(Equip.AffixLunar, Buff.AffixLunar, Item.ZetAspectLunar);

				CreateEquality(Equip.AffixEarth, Buff.AffixEarth, Item.ZetAspectEarth);
				CreateEquality(Equip.AffixVoid, Buff.AffixVoid, Item.ZetAspectVoid);
			}
		}



		public static class SpikeStrip
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("com.groovesalad.GrooveSaladSpikestripContent")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
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

					CopyExpansionReq();
					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					FillEqualities();

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("EQUIPMENT_AFFIXPLATED");
				if (index != EquipmentIndex.None)
				{
					Equip.AffixPlated = EquipmentCatalog.GetEquipmentDef(index);
					//Logger.Warn(Equip.AffixPlated.passiveBuffDef.name);
				}
				index = EquipmentCatalog.FindEquipmentIndex("EQUIPMENT_AFFIXWARPED");
				if (index != EquipmentIndex.None)
				{
					Equip.AffixWarped = EquipmentCatalog.GetEquipmentDef(index);
					//Logger.Warn(Equip.AffixWarped.passiveBuffDef.name);
				}
				index = EquipmentCatalog.FindEquipmentIndex("EQUIPMENT_AFFIXVEILED");
				if (index != EquipmentIndex.None)
				{
					Equip.AffixVeiled = EquipmentCatalog.GetEquipmentDef(index);
					//Logger.Warn(Equip.AffixVeiled.passiveBuffDef.name);
				}
				index = EquipmentCatalog.FindEquipmentIndex("EQUIPMENT_AFFIXARAGONITE");
				if (index != EquipmentIndex.None)
				{
					Equip.AffixAragonite = EquipmentCatalog.GetEquipmentDef(index);
					//Logger.Warn(Equip.AffixAragonite.passiveBuffDef.name);
				}

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				if (Equip.AffixPlated)
				{
					Buff.AffixPlated = Equip.AffixPlated.passiveBuffDef;
				}
				if (Equip.AffixWarped)
				{
					Buff.AffixWarped = Equip.AffixWarped.passiveBuffDef;
				}
				if (Equip.AffixVeiled)
				{
					Buff.AffixVeiled = Equip.AffixVeiled.passiveBuffDef;
				}
				if (Equip.AffixAragonite)
				{
					Buff.AffixAragonite = Equip.AffixAragonite.passiveBuffDef;
				}

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectPlated, ref Equip.AffixPlated, ref Buff.AffixPlated, state);
				DisableInactiveItem(Item.ZetAspectWarped, ref Equip.AffixWarped, ref Buff.AffixWarped, state);
				DisableInactiveItem(Item.ZetAspectVeiled, ref Equip.AffixVeiled, ref Buff.AffixVeiled, state);
				DisableInactiveItem(Item.ZetAspectAragonite, ref Equip.AffixAragonite, ref Buff.AffixAragonite, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectPlated.SetupTokens();
				Items.ZetAspectWarped.SetupTokens();
				Items.ZetAspectVeiled.SetupTokens();
				Items.ZetAspectAragonite.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectPlated, shown);
				SetItemState(Item.ZetAspectWarped, shown);
				SetItemState(Item.ZetAspectVeiled, shown);
				SetItemState(Item.ZetAspectAragonite, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectPlated, Equip.AffixPlated);
				CopyExpansion(Item.ZetAspectWarped, Equip.AffixWarped);
				CopyExpansion(Item.ZetAspectVeiled, Equip.AffixVeiled);
				CopyExpansion(Item.ZetAspectAragonite, Equip.AffixAragonite);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectPlated, Equip.AffixPlated);
				CopyEquipmentPrefab(Item.ZetAspectWarped, Equip.AffixWarped);
				CopyEquipmentPrefab(Item.ZetAspectVeiled, Equip.AffixVeiled);
				CopyEquipmentPrefab(Item.ZetAspectAragonite, Equip.AffixAragonite);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixPlated, Sprites.AffixPlated, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixWarped, Sprites.AffixWarped, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixVeiled, Sprites.AffixVeiled, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixAragonite, Sprites.AffixAragonite, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixPlated, shown);
				SetEquipmentState(Equip.AffixWarped, shown);
				SetEquipmentState(Equip.AffixVeiled, shown);
				SetEquipmentState(Equip.AffixAragonite, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixPlated);
				ColorEquipmentDroplet(Equip.AffixWarped);
				ColorEquipmentDroplet(Equip.AffixVeiled);
				ColorEquipmentDroplet(Equip.AffixAragonite);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixPlated, Buff.AffixPlated, Item.ZetAspectPlated);
				CreateEquality(Equip.AffixWarped, Buff.AffixWarped, Item.ZetAspectWarped);
				CreateEquality(Equip.AffixVeiled, Buff.AffixVeiled, Item.ZetAspectVeiled);
				CreateEquality(Equip.AffixAragonite, Buff.AffixAragonite, Item.ZetAspectAragonite);
			}
		}



		public static class GoldenCoastPlus
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("com.Skell.GoldenCoastPlus") || PluginLoaded("com.Phreel.GoldenCoastPlusRevived")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
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

					CopyExpansionReq();
					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					FillEqualities();

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("EliteGoldEquipment");
				if (index != EquipmentIndex.None)
				{
					Equip.AffixGold = EquipmentCatalog.GetEquipmentDef(index);
					//Logger.Warn(Equip.AffixGold.passiveBuffDef.name);
				}

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				if (Equip.AffixGold)
				{
					Buff.AffixGold = Equip.AffixGold.passiveBuffDef;
				}

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectGold, ref Equip.AffixGold, ref Buff.AffixGold, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectGold.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectGold, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectGold, Equip.AffixGold);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectGold, Equip.AffixGold);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixGold, Sprites.AffixGold, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixGold, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixGold);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixGold, Buff.AffixGold, Item.ZetAspectGold);
			}
		}



		public static class Aetherium
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

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



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
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

					CopyExpansionReq();
					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					FillEqualities();

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("AETHERIUM_ELITE_EQUIPMENT_AFFIX_SANGUINE");
				if (index != EquipmentIndex.None) Equip.AffixSanguine = EquipmentCatalog.GetEquipmentDef(index);

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				BuffIndex index;

				index = BuffCatalog.FindBuffIndex("AFFIX_SANGUINE");
				if (index != BuffIndex.None) Buff.AffixSanguine = BuffCatalog.GetBuffDef(index);

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectSanguine, ref Equip.AffixSanguine, ref Buff.AffixSanguine, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectSanguine.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectSanguine, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectSanguine, Equip.AffixSanguine);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectSanguine, Equip.AffixSanguine);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixSanguine, Sprites.AffixSanguine, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixSanguine, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixSanguine);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixSanguine, Buff.AffixSanguine, Item.ZetAspectSanguine);
			}
		}



		public static class Bubbet
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("bubbet.bubbetsitems")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
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

					CopyExpansionReq();
					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					BuffDef buffDef = Buff.AffixSepia;
					if (buffDef)
					{
						buffDef.buffColor = Color.white;
						buffDef.iconSprite = Sprites.SepiaElite;
					}

					FillEqualities();

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("EquipmentDefSepiaElite");
				if (index != EquipmentIndex.None) Equip.AffixSepia = EquipmentCatalog.GetEquipmentDef(index);

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				BuffIndex index;

				index = BuffCatalog.FindBuffIndex("BuffDefSepia");
				if (index != BuffIndex.None) Buff.AffixSepia = BuffCatalog.GetBuffDef(index);

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectSepia, ref Equip.AffixSepia, ref Buff.AffixSepia, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectSepia.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectSepia, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectSepia, Equip.AffixSepia);
			}

			private static void CopyModelPrefabs()
			{
				CopyItemPrefab(Item.ZetAspectSepia, Equip.AffixSepia);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixSepia, Sprites.AffixSepia, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixSepia, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixSepia);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixSepia, Buff.AffixSepia, Item.ZetAspectSepia);
			}
		}



		public static class WarWisp
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("com.PopcornFactory.WispMod")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
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

					CopyExpansionReq();
					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					FillEqualities();

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("WARFRAMEWISP_ELITE_EQUIPMENT_AFFIX_NULLIFIER");
				if (index != EquipmentIndex.None) Equip.AffixNullifier = EquipmentCatalog.GetEquipmentDef(index);

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				BuffIndex index;

				index = BuffCatalog.FindBuffIndex("AFFIX_NULLIFIER");
				if (index != BuffIndex.None) Buff.AffixNullifier = BuffCatalog.GetBuffDef(index);

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectNullifier, ref Equip.AffixNullifier, ref Buff.AffixNullifier, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectNullifier.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectNullifier, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectNullifier, Equip.AffixNullifier);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectNullifier, Equip.AffixNullifier);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixNullifier, Sprites.AffixNullifier, Sprites.NullOutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixNullifier, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixNullifier);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixNullifier, Buff.AffixNullifier, Item.ZetAspectNullifier);
			}
		}



		public static class Blighted
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("com.Moffein.BlightedElites")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
				}
				else
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

					CopyExpansionReq();
					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					FillEqualities();

					populated = true;
				}
				else
				{
					PopulateEquipment();
					DisableInactiveItems();
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("AffixBlightedMoffein");
				if (index != EquipmentIndex.None) Equip.AffixBlighted = EquipmentCatalog.GetEquipmentDef(index);

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				if (Equip.AffixBlighted)
				{
					Buff.AffixBlighted = Equip.AffixBlighted.passiveBuffDef;
				}

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectBlighted, ref Equip.AffixBlighted, ref Buff.AffixBlighted, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectBlighted.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectBlighted, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectBlighted, Equip.AffixBlighted);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectBlighted, Equip.AffixBlighted);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixBlighted, Sprites.AffixBlighted, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixBlighted, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixBlighted);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixBlighted, Buff.AffixBlighted, Item.ZetAspectBlighted);
			}
		}


		
		public static class GOTCE
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("com.TheBestAssociatedLargelyLudicrousSillyheadGroup.GOTCE")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
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

					CopyExpansionReq();
					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					BuffDef buffDef = Buff.BackupDebuff;
					if (buffDef)
					{
						buffDef.buffColor = Color.white;
						buffDef.iconSprite = Sprites.BackupDebuff;
					}

					FillEqualities();

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("ELITE_EQUIPMENT_BACKUP");
				if (index != EquipmentIndex.None) Equip.AffixBackup = EquipmentCatalog.GetEquipmentDef(index);

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				if (Equip.AffixBackup)
				{
					Buff.AffixBackup = Equip.AffixBackup.passiveBuffDef;
				}

				BuffIndex index = BuffCatalog.FindBuffIndex("Backuped");
				if (index != BuffIndex.None) Buff.BackupDebuff = BuffCatalog.GetBuffDef(index);

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectBackup, ref Equip.AffixBackup, ref Buff.AffixBackup, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectBackup.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectBackup, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectBackup, Equip.AffixBackup);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectBackup, Equip.AffixBackup);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixBackup, Sprites.AffixBackup, Sprites.CrackedOutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixBackup, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixBackup);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixBackup, Buff.AffixBackup, Item.ZetAspectBackup);
			}
		}



		public static class Thalasso
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("com.jt_hehe.Thalassophobia")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
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

					CopyExpansionReq();
					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					FillEqualities();

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("THALASSOPHOBIA_ELITE_EQUIPMENT_AFFIX_PURE");
				if (index != EquipmentIndex.None) Equip.AffixPurity = EquipmentCatalog.GetEquipmentDef(index);

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				if (Equip.AffixPurity)
				{
					Buff.AffixPurity = Equip.AffixPurity.passiveBuffDef;
				}

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectPurity, ref Equip.AffixPurity, ref Buff.AffixPurity, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectPurity.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectPurity, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectPurity, Equip.AffixPurity);
			}

			private static void CopyModelPrefabs()
			{
				CopyItemPrefab(Item.ZetAspectPurity, Equip.AffixPurity);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixPurity, Sprites.AffixPurity, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixPurity, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixPurity);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixPurity, Buff.AffixPurity, Item.ZetAspectPurity);
			}
		}



		public static class RisingTides
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("com.themysticsword.risingtides")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
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

					CopyExpansionReq();
					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					FillEqualities();

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("RisingTides_AffixBarrier");
				if (index != EquipmentIndex.None) Equip.AffixBarrier = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("RisingTides_AffixBlackHole");
				if (index != EquipmentIndex.None) Equip.AffixBlackHole = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("RisingTides_AffixMoney");
				if (index != EquipmentIndex.None) Equip.AffixMoney = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("RisingTides_AffixNight");
				if (index != EquipmentIndex.None) Equip.AffixNight = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("RisingTides_AffixWater");
				if (index != EquipmentIndex.None) Equip.AffixWater = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("RisingTides_AffixImpPlane");
				if (index != EquipmentIndex.None) Equip.AffixRealgar = EquipmentCatalog.GetEquipmentDef(index);

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				if (Equip.AffixBarrier)
				{
					Buff.AffixBarrier = Equip.AffixBarrier.passiveBuffDef;
				}
				if (Equip.AffixBlackHole)
				{
					Buff.AffixBlackHole = Equip.AffixBlackHole.passiveBuffDef;
				}
				if (Equip.AffixMoney)
				{
					Buff.AffixMoney = Equip.AffixMoney.passiveBuffDef;
				}
				if (Equip.AffixNight)
				{
					Buff.AffixNight = Equip.AffixNight.passiveBuffDef;
				}
				if (Equip.AffixWater)
				{
					Buff.AffixWater = Equip.AffixWater.passiveBuffDef;
				}
				if (Equip.AffixRealgar)
				{
					Buff.AffixRealgar = Equip.AffixRealgar.passiveBuffDef;
				}

				BuffIndex index = BuffCatalog.FindBuffIndex("RisingTides_NightSpeedBoost");
				if (index != BuffIndex.None) Buff.NightSpeed = BuffCatalog.GetBuffDef(index);

				index = BuffCatalog.FindBuffIndex("RisingTides_NightReducedVision");
				if (index != BuffIndex.None) Buff.NightBlind = BuffCatalog.GetBuffDef(index);

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectBarrier, ref Equip.AffixBarrier, ref Buff.AffixBarrier, state);
				DisableInactiveItem(Item.ZetAspectBlackHole, ref Equip.AffixBlackHole, ref Buff.AffixBlackHole, state);
				DisableInactiveItem(Item.ZetAspectMoney, ref Equip.AffixMoney, ref Buff.AffixMoney, state);
				DisableInactiveItem(Item.ZetAspectNight, ref Equip.AffixNight, ref Buff.AffixNight, state);
				DisableInactiveItem(Item.ZetAspectWater, ref Equip.AffixWater, ref Buff.AffixWater, state);
				DisableInactiveItem(Item.ZetAspectRealgar, ref Equip.AffixRealgar, ref Buff.AffixRealgar, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectBarrier.SetupTokens();
				Items.ZetAspectBlackHole.SetupTokens();
				Items.ZetAspectMoney.SetupTokens();
				Items.ZetAspectNight.SetupTokens();
				Items.ZetAspectWater.SetupTokens();
				Items.ZetAspectRealgar.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectBarrier, shown);
				SetItemState(Item.ZetAspectBlackHole, shown);
				SetItemState(Item.ZetAspectMoney, shown);
				SetItemState(Item.ZetAspectNight, shown);
				SetItemState(Item.ZetAspectWater, shown);
				SetItemState(Item.ZetAspectRealgar, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectBarrier, Equip.AffixBarrier);
				CopyExpansion(Item.ZetAspectBlackHole, Equip.AffixBlackHole);
				CopyExpansion(Item.ZetAspectMoney, Equip.AffixMoney);
				CopyExpansion(Item.ZetAspectNight, Equip.AffixNight);
				CopyExpansion(Item.ZetAspectWater, Equip.AffixWater);
				CopyExpansion(Item.ZetAspectRealgar, Equip.AffixRealgar);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectBarrier, Equip.AffixBarrier);
				CopyEquipmentPrefab(Item.ZetAspectBlackHole, Equip.AffixBlackHole);
				CopyEquipmentPrefab(Item.ZetAspectMoney, Equip.AffixMoney);
				CopyEquipmentPrefab(Item.ZetAspectNight, Equip.AffixNight);
				CopyEquipmentPrefab(Item.ZetAspectWater, Equip.AffixWater);
				CopyEquipmentPrefab(Item.ZetAspectRealgar, Equip.AffixRealgar);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixBarrier, Sprites.AffixBarrier, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixBlackHole, Sprites.AffixBlackHole, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixMoney, Sprites.AffixMoney, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixNight, Sprites.AffixNight, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixWater, Sprites.AffixWater, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixRealgar, Sprites.AffixRealgar, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixBarrier, shown);
				SetEquipmentState(Equip.AffixBlackHole, shown);
				SetEquipmentState(Equip.AffixMoney, shown);
				SetEquipmentState(Equip.AffixNight, shown);
				SetEquipmentState(Equip.AffixWater, shown);
				SetEquipmentState(Equip.AffixRealgar, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixBarrier);
				ColorEquipmentDroplet(Equip.AffixBlackHole);
				ColorEquipmentDroplet(Equip.AffixMoney);
				ColorEquipmentDroplet(Equip.AffixNight);
				ColorEquipmentDroplet(Equip.AffixWater);
				ColorEquipmentDroplet(Equip.AffixRealgar);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixBarrier, Buff.AffixBarrier, Item.ZetAspectBarrier);
				CreateEquality(Equip.AffixBlackHole, Buff.AffixBlackHole, Item.ZetAspectBlackHole);
				CreateEquality(Equip.AffixMoney, Buff.AffixMoney, Item.ZetAspectMoney);
				CreateEquality(Equip.AffixNight, Buff.AffixNight, Item.ZetAspectNight);
				CreateEquality(Equip.AffixWater, Buff.AffixWater, Item.ZetAspectWater);
				CreateEquality(Equip.AffixRealgar, Buff.AffixRealgar, Item.ZetAspectRealgar);
			}
		}



		public static class NemRisingTides
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("prodzpod.NemesisRisingTides")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
				}
				else
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

					CopyExpansionReq();
					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					FillEqualities();

					populated = true;
				}
				else
				{
					PopulateEquipment();
					DisableInactiveItems();
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				Compat.NemRisingTides.PrepareEquipmentCheck();

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("NemesisRisingTides_AffixBuffered");
				if (index != EquipmentIndex.None)
				{
					if (Compat.NemRisingTides.GetBufferedEnabled())
					{
						Equip.AffixBuffered = EquipmentCatalog.GetEquipmentDef(index);
					}
				}

				index = EquipmentCatalog.FindEquipmentIndex("NemesisRisingTides_AffixOppressive");
				if (index != EquipmentIndex.None)
				{
					if (Compat.NemRisingTides.GetOppressiveEnabled())
					{
						Equip.AffixOppressive = EquipmentCatalog.GetEquipmentDef(index);
					}
				}

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				if (Equip.AffixBuffered)
				{
					Buff.AffixBuffered = Equip.AffixBuffered.passiveBuffDef;
				}
				if (Equip.AffixOppressive)
				{
					Buff.AffixOppressive = Equip.AffixOppressive.passiveBuffDef;
				}

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectBuffered, ref Equip.AffixBuffered, ref Buff.AffixBuffered, state);
				DisableInactiveItem(Item.ZetAspectOppressive, ref Equip.AffixOppressive, ref Buff.AffixOppressive, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectBuffered.SetupTokens();
				Items.ZetAspectOppressive.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectBuffered, shown);
				SetItemState(Item.ZetAspectOppressive, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectBuffered, Equip.AffixBuffered);
				CopyExpansion(Item.ZetAspectOppressive, Equip.AffixOppressive);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectBuffered, Equip.AffixBuffered);
				CopyEquipmentPrefab(Item.ZetAspectOppressive, Equip.AffixOppressive);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixBuffered, Sprites.AffixBuffered, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixOppressive, Sprites.AffixOppressive, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixBuffered, shown);
				SetEquipmentState(Equip.AffixOppressive, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixBuffered);
				ColorEquipmentDroplet(Equip.AffixOppressive);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixBuffered, Buff.AffixBuffered, Item.ZetAspectBuffered);
				CreateEquality(Equip.AffixOppressive, Buff.AffixOppressive, Item.ZetAspectOppressive);
			}
		}



		public static class MoreElites
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("com.Nuxlar.MoreElites")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
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

					CopyExpansionReq();
					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					FillEqualities();

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("AffixEmpowering");
				if (index != EquipmentIndex.None)
				{
					Equip.AffixEmpowering = EquipmentCatalog.GetEquipmentDef(index);
				}
				index = EquipmentCatalog.FindEquipmentIndex("AffixFrenziedNuxlar");
				if (index != EquipmentIndex.None)
				{
					Equip.AffixFrenzied = EquipmentCatalog.GetEquipmentDef(index);
				}
				index = EquipmentCatalog.FindEquipmentIndex("AffixVolatile");
				if (index != EquipmentIndex.None)
				{
					Equip.AffixVolatile = EquipmentCatalog.GetEquipmentDef(index);
				}
				index = EquipmentCatalog.FindEquipmentIndex("AffixEcho");
				if (index != EquipmentIndex.None)
				{
					Equip.AffixEcho = EquipmentCatalog.GetEquipmentDef(index);
				}

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				if (Equip.AffixEmpowering)
				{
					Buff.AffixEmpowering = Equip.AffixEmpowering.passiveBuffDef;
				}
				if (Equip.AffixFrenzied)
				{
					Buff.AffixFrenzied = Equip.AffixFrenzied.passiveBuffDef;
				}
				if (Equip.AffixVolatile)
				{
					Buff.AffixVolatile = Equip.AffixVolatile.passiveBuffDef;
				}
				if (Equip.AffixEcho)
				{
					Buff.AffixEcho = Equip.AffixEcho.passiveBuffDef;
				}

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectEmpowering, ref Equip.AffixEmpowering, ref Buff.AffixEmpowering, state);
				DisableInactiveItem(Item.ZetAspectFrenzied, ref Equip.AffixFrenzied, ref Buff.AffixFrenzied, state);
				DisableInactiveItem(Item.ZetAspectVolatile, ref Equip.AffixVolatile, ref Buff.AffixVolatile, state);
				DisableInactiveItem(Item.ZetAspectEcho, ref Equip.AffixEcho, ref Buff.AffixEcho, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectEmpowering.SetupTokens();
				Items.ZetAspectFrenzied.SetupTokens();
				Items.ZetAspectVolatile.SetupTokens();
				Items.ZetAspectEcho.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectEmpowering, shown);
				SetItemState(Item.ZetAspectFrenzied, shown);
				SetItemState(Item.ZetAspectVolatile, shown);
				SetItemState(Item.ZetAspectEcho, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectEmpowering, Equip.AffixEmpowering);
				CopyExpansion(Item.ZetAspectFrenzied, Equip.AffixFrenzied);
				CopyExpansion(Item.ZetAspectVolatile, Equip.AffixVolatile);
				CopyExpansion(Item.ZetAspectEcho, Equip.AffixEcho);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectEmpowering, Equip.AffixEmpowering);
				CopyEquipmentPrefab(Item.ZetAspectFrenzied, Equip.AffixFrenzied);
				CopyEquipmentPrefab(Item.ZetAspectVolatile, Equip.AffixVolatile);
				CopyEquipmentPrefab(Item.ZetAspectEcho, Equip.AffixEcho);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixEmpowering, Sprites.AffixBuffing, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixFrenzied, Sprites.AffixFrenzied, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixVolatile, Sprites.AffixVolatile, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixEcho, Sprites.AffixEcho, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixEmpowering, shown);
				SetEquipmentState(Equip.AffixFrenzied, shown);
				SetEquipmentState(Equip.AffixVolatile, shown);
				SetEquipmentState(Equip.AffixEcho, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixEmpowering);
				ColorEquipmentDroplet(Equip.AffixFrenzied);
				ColorEquipmentDroplet(Equip.AffixVolatile);
				ColorEquipmentDroplet(Equip.AffixEcho);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixEmpowering, Buff.AffixEmpowering, Item.ZetAspectEmpowering);
				CreateEquality(Equip.AffixFrenzied, Buff.AffixFrenzied, Item.ZetAspectFrenzied);
				CreateEquality(Equip.AffixVolatile, Buff.AffixVolatile, Item.ZetAspectVolatile);
				CreateEquality(Equip.AffixEcho, Buff.AffixEcho, Item.ZetAspectEcho);
			}
		}



		public static class EliteVariety
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

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



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
				}
			}

			internal static void Init()
			{
				if (Enabled)
				{
					tinkerDroneBodyIndex = BodyCatalog.FindBodyIndex("EliteVariety_TinkererDroneBody");

					PopulateEquipment();
					PopulateBuffs();

					DisableInactiveItems();
					SetupText();
					ItemEntries(DropHooks.CanObtainItem());

					CopyExpansionReq();
					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					FillEqualities();

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixArmored");
				if (index != EquipmentIndex.None) Equip.AffixArmored = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixBuffing");
				if (index != EquipmentIndex.None) Equip.AffixBuffing = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixImpPlane");
				if (index != EquipmentIndex.None) Equip.AffixImpPlane = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixPillaging");
				if (index != EquipmentIndex.None) Equip.AffixPillaging = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixSandstorm");
				if (index != EquipmentIndex.None) Equip.AffixSandstorm = EquipmentCatalog.GetEquipmentDef(index);

				index = EquipmentCatalog.FindEquipmentIndex("EliteVariety_AffixTinkerer");
				if (index != EquipmentIndex.None) Equip.AffixTinkerer = EquipmentCatalog.GetEquipmentDef(index);

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				if (Equip.AffixArmored)
				{
					Buff.AffixArmored = Equip.AffixArmored.passiveBuffDef;
				}
				if (Equip.AffixBuffing)
				{
					Buff.AffixBuffing = Equip.AffixBuffing.passiveBuffDef;
				}
				if (Equip.AffixImpPlane)
				{
					Buff.AffixImpPlane = Equip.AffixImpPlane.passiveBuffDef;
				}
				if (Equip.AffixPillaging)
				{
					Buff.AffixPillaging = Equip.AffixPillaging.passiveBuffDef;
				}
				if (Equip.AffixSandstorm)
				{
					Buff.AffixSandstorm = Equip.AffixSandstorm.passiveBuffDef;
				}
				if (Equip.AffixTinkerer)
				{
					Buff.AffixTinkerer = Equip.AffixTinkerer.passiveBuffDef;
				}

				BuffIndex index = BuffCatalog.FindBuffIndex("EliteVariety_SandstormBlind");
				if (index != BuffIndex.None) Buff.SandBlind = BuffCatalog.GetBuffDef(index);

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectArmor, ref Equip.AffixArmored, ref Buff.AffixArmored, state);
				DisableInactiveItem(Item.ZetAspectBanner, ref Equip.AffixBuffing, ref Buff.AffixBuffing, state);
				DisableInactiveItem(Item.ZetAspectImpale, ref Equip.AffixImpPlane, ref Buff.AffixImpPlane, state);
				DisableInactiveItem(Item.ZetAspectGolden, ref Equip.AffixPillaging, ref Buff.AffixPillaging, state);
				DisableInactiveItem(Item.ZetAspectCyclone, ref Equip.AffixSandstorm, ref Buff.AffixSandstorm, state);
				DisableInactiveItem(Item.ZetAspectTinker, ref Equip.AffixTinkerer, ref Buff.AffixTinkerer, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectArmor.SetupTokens();
				Items.ZetAspectBanner.SetupTokens();
				Items.ZetAspectImpale.SetupTokens();
				Items.ZetAspectGolden.SetupTokens();
				Items.ZetAspectCyclone.SetupTokens();
				Items.ZetAspectTinker.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectArmor, shown);
				SetItemState(Item.ZetAspectBanner, shown);
				SetItemState(Item.ZetAspectImpale, shown);
				SetItemState(Item.ZetAspectGolden, shown);
				SetItemState(Item.ZetAspectCyclone, shown);
				SetItemState(Item.ZetAspectTinker, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectArmor, Equip.AffixArmored);
				CopyExpansion(Item.ZetAspectBanner, Equip.AffixBuffing);
				CopyExpansion(Item.ZetAspectImpale, Equip.AffixImpPlane);
				CopyExpansion(Item.ZetAspectGolden, Equip.AffixPillaging);
				CopyExpansion(Item.ZetAspectCyclone, Equip.AffixSandstorm);
				CopyExpansion(Item.ZetAspectTinker, Equip.AffixTinkerer);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectArmor, Equip.AffixArmored);
				CopyEquipmentPrefab(Item.ZetAspectBanner, Equip.AffixBuffing);
				CopyEquipmentPrefab(Item.ZetAspectImpale, Equip.AffixImpPlane);
				CopyEquipmentPrefab(Item.ZetAspectGolden, Equip.AffixPillaging);
				CopyEquipmentPrefab(Item.ZetAspectCyclone, Equip.AffixSandstorm);
				CopyEquipmentPrefab(Item.ZetAspectTinker, Equip.AffixTinkerer);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixArmored, Sprites.AffixArmored, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixBuffing, Sprites.AffixBuffing_EV, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixImpPlane, Sprites.AffixImpPlane, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixPillaging, Sprites.AffixPillaging, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixSandstorm, Sprites.AffixSandstorm, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixTinkerer, Sprites.AffixTinkerer, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixArmored, shown);
				SetEquipmentState(Equip.AffixBuffing, shown);
				SetEquipmentState(Equip.AffixImpPlane, shown);
				SetEquipmentState(Equip.AffixPillaging, shown);
				SetEquipmentState(Equip.AffixSandstorm, shown);
				SetEquipmentState(Equip.AffixTinkerer, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixArmored);
				ColorEquipmentDroplet(Equip.AffixBuffing);
				ColorEquipmentDroplet(Equip.AffixImpPlane);
				ColorEquipmentDroplet(Equip.AffixPillaging);
				ColorEquipmentDroplet(Equip.AffixSandstorm);
				ColorEquipmentDroplet(Equip.AffixTinkerer);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixArmored, Buff.AffixArmored, Item.ZetAspectArmor);
				CreateEquality(Equip.AffixBuffing, Buff.AffixBuffing, Item.ZetAspectBanner);
				CreateEquality(Equip.AffixImpPlane, Buff.AffixImpPlane, Item.ZetAspectImpale);
				CreateEquality(Equip.AffixPillaging, Buff.AffixPillaging, Item.ZetAspectGolden);
				CreateEquality(Equip.AffixSandstorm, Buff.AffixSandstorm, Item.ZetAspectCyclone);
				CreateEquality(Equip.AffixTinkerer, Buff.AffixTinkerer, Item.ZetAspectTinker);
			}
		}



		public static class Augmentum
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("com.BrandonRosa.Augmentum")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
				}
				else
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

					CopyExpansionReq();
					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					FillEqualities();

					populated = true;
				}
				else
				{
					PopulateEquipment();
					DisableInactiveItems();
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("BRANS_ELITE_EQUIPMENT_AFFIX_ADAPTIVE");
				if (index != EquipmentIndex.None)
				{
					Equip.AffixAdaptive = EquipmentCatalog.GetEquipmentDef(index);
				}

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				if (Equip.AffixAdaptive)
				{
					Buff.AffixAdaptive = Equip.AffixAdaptive.passiveBuffDef;
				}

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectAdaptive, ref Equip.AffixAdaptive, ref Buff.AffixAdaptive, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectAdaptive.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectAdaptive, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectAdaptive, Equip.AffixAdaptive);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectAdaptive, Equip.AffixAdaptive);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixAdaptive, Sprites.AffixAdaptive, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixAdaptive, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixAdaptive);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixAdaptive, Buff.AffixAdaptive, Item.ZetAspectAdaptive);
			}
		}



		public static class Sandswept
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("com.TeamSandswept.Sandswept")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
				}
				else
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

					CopyExpansionReq();
					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					FillEqualities();

					populated = true;
				}
				else
				{
					PopulateEquipment();
					DisableInactiveItems();
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("ELITE_EQUIPMENT_MOTIVATING");
				if (index != EquipmentIndex.None)
				{
					Equip.AffixMotivator = EquipmentCatalog.GetEquipmentDef(index);
				}

				index = EquipmentCatalog.FindEquipmentIndex("ELITE_EQUIPMENT_OSMIUM");
				if (index != EquipmentIndex.None)
				{
					Equip.AffixOsmium = EquipmentCatalog.GetEquipmentDef(index);
				}

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				if (Equip.AffixMotivator)
				{
					Buff.AffixMotivator = Equip.AffixMotivator.passiveBuffDef;
				}

				if (Equip.AffixOsmium)
				{
					Buff.AffixOsmium = Equip.AffixOsmium.passiveBuffDef;
				}

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectMotivator, ref Equip.AffixMotivator, ref Buff.AffixMotivator, state);
				DisableInactiveItem(Item.ZetAspectOsmium, ref Equip.AffixOsmium, ref Buff.AffixOsmium, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectMotivator.SetupTokens();
				Items.ZetAspectOsmium.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectMotivator, shown);
				SetItemState(Item.ZetAspectOsmium, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectMotivator, Equip.AffixMotivator);
				CopyExpansion(Item.ZetAspectOsmium, Equip.AffixOsmium);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectMotivator, Equip.AffixMotivator);
				CopyEquipmentPrefab(Item.ZetAspectOsmium, Equip.AffixOsmium);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixMotivator, Sprites.AffixMotivator, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixOsmium, Sprites.AffixOsmium, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixMotivator, shown);
				SetEquipmentState(Equip.AffixOsmium, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixMotivator);
				ColorEquipmentDroplet(Equip.AffixOsmium);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixMotivator, Buff.AffixMotivator, Item.ZetAspectMotivator);
				CreateEquality(Equip.AffixOsmium, Buff.AffixOsmium, Item.ZetAspectOsmium);
			}
		}
	}
}
