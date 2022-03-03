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
	//[BepInDependency("com.KomradeSpectre.Aetherium", BepInDependency.DependencyFlags.SoftDependency)]

	public class ZetAspectsPlugin : BaseUnityPlugin
	{
		public const string ModVer = "2.7.0";
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

			//if (Catalog.Aetherium.Enabled) Compat.Aetherium.Init();

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

				CreateDroplet(RoR2Content.Equipment.AffixWhite, transform.position + new Vector3(-5f, 5f, 5f));
				CreateDroplet(RoR2Content.Equipment.AffixBlue, transform.position + new Vector3(0f, 5f, 7.5f));
				CreateDroplet(RoR2Content.Equipment.AffixRed, transform.position + new Vector3(5f, 5f, 5f));
				CreateDroplet(RoR2Content.Equipment.AffixHaunted, transform.position + new Vector3(-5f, 5f, -5f));
				CreateDroplet(RoR2Content.Equipment.AffixPoison, transform.position + new Vector3(0f, 5f, -7.5f));
				CreateDroplet(RoR2Content.Equipment.AffixLunar, transform.position + new Vector3(5f, 5f, -5f));
			}
		}

		private static void CreateDroplet(EquipmentDef def, Vector3 pos)
		{
			PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(def.equipmentIndex), pos, Vector3.zero);
		}
		//*/
	}
}
