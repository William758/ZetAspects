using BepInEx;
using RoR2;
using UnityEngine;

using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace TPDespair.ZetAspects
{
	[BepInPlugin(ModGuid, ModName, ModVer)]
	[BepInDependency("com.groovesalad.GrooveSaladSpikestripContent", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.plasmacore.PlasmaCoreSpikestripContent", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.Skell.GoldenCoastPlus", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.KomradeSpectre.Aetherium", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("bubbet.bubbetsitems", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.PopcornFactory.WispMod", BepInDependency.DependencyFlags.SoftDependency)]
	//[BepInDependency("com.Moffein.BlightedElites", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.TheBestAssociatedLargelyLudicrousSillyheadGroup.GOTCE", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.jt_hehe.Thalassophobia", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.themysticsword.risingtides", BepInDependency.DependencyFlags.SoftDependency)]

	public class ZetAspectsPlugin : BaseUnityPlugin
	{
		public const string ModVer = "2.7.29";
		public const string ModName = "ZetAspects";
		public const string ModGuid = "com.TPDespair.ZetAspects";



		public void Awake()
		{
			Configuration.Init(Config);
			ZetAspects.Logger.logSource = Logger;
			Catalog.OnAwake();

			StatHooks.Init();
			EffectHooks.Init();
			DropHooks.Init();
			DisplayHooks.Init();

			// - EliteReworks is LateSetup only

			if (Catalog.Aetherium.Enabled && Configuration.AetheriumHooks.Value) Compat.Aetherium.Init();

			// - Targeting this specific assembly from SpikeStrip
			if (Catalog.PluginLoaded("com.plasmacore.PlasmaCoreSpikestripContent") && Configuration.SpikeStripHooks.Value) Compat.PlasmaSpikeStrip.Init();

			if (Catalog.WarWisp.Enabled && Configuration.WarWispHooks.Value) Compat.WarWisp.Init();

			// - Blighted is LateSetup only

			if (Catalog.GOTCE.Enabled && Configuration.GotceHooks.Value) Compat.GOTCE.Init();

			if (Catalog.RisingTides.Enabled && Configuration.RisingTidesHooks.Value) Compat.RisingTides.Init();

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
