﻿using BepInEx;
using RoR2;
using UnityEngine;

using System.Security;
using System.Security.Permissions;
using HarmonyLib;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace TPDespair.ZetAspects
{
	[BepInPlugin(ModGuid, ModName, ModVer)]
	[BepInDependency("_com.groovesalad.GrooveUnsharedUtils", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.groovesalad.GrooveSaladSpikestripContent", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.plasmacore.PlasmaCoreSpikestripContent", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.Skell.GoldenCoastPlus", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.Phreel.GoldenCoastPlusRevived", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.KomradeSpectre.Aetherium", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("bubbet.bubbetsitems", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.PopcornFactory.WispMod", BepInDependency.DependencyFlags.SoftDependency)]
	//[BepInDependency("com.Moffein.EliteReworks", BepInDependency.DependencyFlags.SoftDependency)] ### DEPENDENCY OF
	//[BepInDependency("com.Moffein.BlightedElites", BepInDependency.DependencyFlags.SoftDependency)] ### DEPENDENCY OF
	[BepInDependency("com.TheBestAssociatedLargelyLudicrousSillyheadGroup.GOTCE", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.jt_hehe.Thalassophobia", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.themysticsword.risingtides", BepInDependency.DependencyFlags.SoftDependency)]
	//[BepInDependency("prodzpod.NemesisSpikestrip", BepInDependency.DependencyFlags.SoftDependency)] ### DEPENDENCY OF
	//[BepInDependency("prodzpod.NemesisRisingTides", BepInDependency.DependencyFlags.SoftDependency)] ### DEPENDENCY OF
	[BepInDependency("com.Nuxlar.MoreElites", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.themysticsword.elitevariety", BepInDependency.DependencyFlags.SoftDependency)]
	//[BepInDependency("com.BrandonRosa.Augmentum", BepInDependency.DependencyFlags.SoftDependency)] ### DEPENDENCY OF
	[BepInDependency("com.TeamSandswept.Sandswept", BepInDependency.DependencyFlags.SoftDependency)]

	public class ZetAspectsPlugin : BaseUnityPlugin
	{
		public const string ModVer = "2.8.6";
		public const string ModName = "ZetAspects";
		public const string ModGuid = "com.TPDespair.ZetAspects";

		public static Harmony harmony;



		public void Awake()
		{
			harmony = new Harmony(ModGuid);

			Configuration.Init(Config);
			ZetAspects.Logger.logSource = Logger;
			Catalog.OnAwake();

			StatHooks.Init();
			EffectHooks.Init();
			DropHooks.Init();
			DisplayHooks.Init();

			// - EliteReworks is LateSetup only

			if (Catalog.Aetherium.Enabled && Configuration.AetheriumHooks.Value) Compat.Aetherium.Init();

			if (Configuration.SpikeStripHooks.Value)
			{
				if (Catalog.PluginLoaded("_com.groovesalad.GrooveUnsharedUtils")) Compat.SpikeStripUnsharedUtils.Init();
				if (Catalog.PluginLoaded("com.groovesalad.GrooveSaladSpikestripContent")) Compat.GrooveSpikeStrip.Init();
				if (Catalog.PluginLoaded("com.plasmacore.PlasmaCoreSpikestripContent")) Compat.PlasmaSpikeStrip.Init();
			}

			if (Catalog.WarWisp.Enabled && Configuration.WarWispHooks.Value) Compat.WarWisp.Init();

			// - Blighted is LateSetup only

			if (Catalog.GOTCE.Enabled && Configuration.GotceHooks.Value) Compat.GOTCE.Init();

			if (Catalog.RisingTides.Enabled && Configuration.RisingTidesHooks.Value) Compat.RisingTides.Init();

			// - NemSpikeStrip is LateSetup only

			// - NemRisingTides

			if (Catalog.MoreElites.Enabled && Configuration.MoreElitesHooks.Value) Compat.MoreElites.Init();

			if (Catalog.EliteVariety.Enabled && Configuration.EliteVarietyHooks.Value) Compat.EliteVariety.Init();

			// - Augmentum is LateSetup only

			if (Catalog.Sandswept.Enabled && Configuration.SandsweptHooks.Value) Compat.Sandswept.Init();

			Language.Init();
		}

		public void FixedUpdate()
		{
			EffectHooks.OnFixedUpdate();
		}
		/*
		public void Update()
		{
			DebugDrops();
		}
		//*/


		/*
		private static void DebugDrops()
		{
			if (Input.GetKeyDown(KeyCode.F2))
			{
				var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

				CreateDroplet(Catalog.Equip.AffixWhite, transform.position + new Vector3(-5f, 5f, 5f));
				CreateDroplet(Catalog.Equip.AffixBlue, transform.position + new Vector3(0f, 5f, 7.5f));
				CreateDroplet(Catalog.Equip.AffixRed, transform.position + new Vector3(5f, 5f, 5f));
				CreateDroplet(Catalog.Equip.AffixHaunted, transform.position + new Vector3(-5f, 5f, -5f));
				CreateDroplet(Catalog.Equip.AffixPoison, transform.position + new Vector3(0f, 5f, -7.5f));
				CreateDroplet(Catalog.Equip.AffixLunar, transform.position + new Vector3(5f, 5f, -5f));

				CreateDroplet(Catalog.Equip.AffixEarth, transform.position + new Vector3(-10f, 10f, 10f));
				CreateDroplet(Catalog.Equip.AffixVoid, transform.position + new Vector3(0f, 10f, 15f));
			}

			if (Input.GetKeyDown(KeyCode.F3))
			{
				var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

				CreateDroplet(Catalog.Equip.AffixPlated, transform.position + new Vector3(-5f, 5f, 5f));
				CreateDroplet(Catalog.Equip.AffixWarped, transform.position + new Vector3(0f, 5f, 7.5f));
				CreateDroplet(Catalog.Equip.AffixVeiled, transform.position + new Vector3(5f, 5f, 5f));
				CreateDroplet(Catalog.Equip.AffixAragonite, transform.position + new Vector3(-5f, 5f, -5f));

				CreateDroplet(Catalog.Equip.AffixBuffered, transform.position + new Vector3(0f, 5f, -7.5f));
				CreateDroplet(Catalog.Equip.AffixOppressive, transform.position + new Vector3(5f, 5f, -5f));

				CreateDroplet(Catalog.Equip.AffixEmpowering, transform.position + new Vector3(-10f, 10f, 10f));
				CreateDroplet(Catalog.Equip.AffixFrenzied, transform.position + new Vector3(0f, 10f, 15f));
				CreateDroplet(Catalog.Equip.AffixVolatile, transform.position + new Vector3(10f, 10f, 10f));
				CreateDroplet(Catalog.Equip.AffixEcho, transform.position + new Vector3(-10f, 10f, -10f));
			}

			if (Input.GetKeyDown(KeyCode.F4))
			{
				var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

				CreateDroplet(Catalog.Equip.AffixBarrier, transform.position + new Vector3(-5f, 5f, 5f));
				CreateDroplet(Catalog.Equip.AffixBlackHole, transform.position + new Vector3(0f, 5f, 7.5f));
				CreateDroplet(Catalog.Equip.AffixMoney, transform.position + new Vector3(5f, 5f, 5f));
				CreateDroplet(Catalog.Equip.AffixNight, transform.position + new Vector3(-5f, 5f, -5f));
				CreateDroplet(Catalog.Equip.AffixWater, transform.position + new Vector3(0f, 5f, -7.5f));
				CreateDroplet(Catalog.Equip.AffixRealgar, transform.position + new Vector3(5f, 5f, -5f));

				CreateDroplet(Catalog.Equip.AffixArmored, transform.position + new Vector3(-10f, 10f, 10f));
				CreateDroplet(Catalog.Equip.AffixBuffing, transform.position + new Vector3(0f, 10f, 15f));
				CreateDroplet(Catalog.Equip.AffixImpPlane, transform.position + new Vector3(10f, 10f, 10f));
				CreateDroplet(Catalog.Equip.AffixPillaging, transform.position + new Vector3(-10f, 10f, -10f));
				CreateDroplet(Catalog.Equip.AffixSandstorm, transform.position + new Vector3(0f, 10f, -15f));
				CreateDroplet(Catalog.Equip.AffixTinkerer, transform.position + new Vector3(10f, 10f, -10f));
			}

			if (Input.GetKeyDown(KeyCode.F5))
			{
				var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

				CreateDroplet(Catalog.Equip.AffixGold, transform.position + new Vector3(-5f, 5f, 5f));
				CreateDroplet(Catalog.Equip.AffixSanguine, transform.position + new Vector3(0f, 5f, 7.5f));
				CreateDroplet(Catalog.Equip.AffixSepia, transform.position + new Vector3(5f, 5f, 5f));
				CreateDroplet(Catalog.Equip.AffixNullifier, transform.position + new Vector3(-5f, 5f, -5f));
				CreateDroplet(Catalog.Equip.AffixBlighted, transform.position + new Vector3(0f, 5f, -7.5f));
				CreateDroplet(Catalog.Equip.AffixBackup, transform.position + new Vector3(5f, 5f, -5f));

				CreateDroplet(Catalog.Equip.AffixPurity, transform.position + new Vector3(-10f, 10f, 10f));
				CreateDroplet(Catalog.Equip.AffixAdaptive, transform.position + new Vector3(0f, 10f, 15f));
				CreateDroplet(Catalog.Equip.AffixMotivator, transform.position + new Vector3(10f, 10f, 10f));
				CreateDroplet(Catalog.Equip.AffixOsmium, transform.position + new Vector3(-10f, 10f, -10f));
			}

			if (Input.GetKeyDown(KeyCode.F6))
			{
				var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

				CreateDroplet(RoR2Content.Items.Knurl, transform.position + new Vector3(-5f, 5f, 5f));
				CreateDroplet(RoR2Content.Items.ShieldOnly, transform.position + new Vector3(0f, 5f, 7.5f));
				CreateDroplet(DLC1Content.Items.MissileVoid, transform.position + new Vector3(5f, 5f, 5f));
				CreateDroplet(RoR2Content.Items.PersonalShield, transform.position + new Vector3(-5f, 5f, -5f));
				CreateDroplet(RoR2Content.Items.HeadHunter, transform.position + new Vector3(0f, 5f, -7.5f));
				CreateDroplet(DLC1Content.Items.BearVoid, transform.position + new Vector3(5f, 5f, -5f));

				CreateDroplet(RoR2Content.Items.ExtraLife, transform.position + new Vector3(-10f, 10f, 10f));
				CreateDroplet(RoR2Content.Equipment.Cleanse, transform.position + new Vector3(0f, 10f, 15f));
				CreateDroplet(RoR2Content.Items.Infusion, transform.position + new Vector3(10f, 10f, 10f));
				CreateDroplet(DLC1Content.Items.ExtraLifeVoid, transform.position + new Vector3(-10f, 10f, -10f));
				CreateDroplet(DLC1Content.Items.VoidMegaCrabItem, transform.position + new Vector3(0f, 10f, -15f));
			}
		}

		private static void CreateDroplet(EquipmentDef def, Vector3 pos)
		{
			if (!def) return;

			PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(def.equipmentIndex), pos, Vector3.zero);
		}

		private static void CreateDroplet(ItemDef def, Vector3 pos)
		{
			if (!def) return;

			PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(def.itemIndex), pos, Vector3.zero);
		}
		//*/
	}
}
