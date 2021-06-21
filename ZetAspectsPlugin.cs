using BepInEx;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace TPDespair.ZetAspects
{
	[BepInPlugin(ModGuid, ModName, ModVer)]

	public class ZetAspectsPlugin : BaseUnityPlugin
	{
		public const string ModVer = "2.3.1";
		public const string ModName = "ZetAspects";
		public const string ModGuid = "com.TPDespair.ZetAspects";



		public static AssetBundle Assets;

		public static Dictionary<string, string> LangTokens = new Dictionary<string, string>();

		public static int RunDropCount = 0;



		public void Awake()
		{
			RoR2Application.isModded = true;
			NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append(ModGuid + ":" + ModVer);

			Configuration.Init(Config);
			ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;

			Catalog.PopulateOnRunStartHook();
			Catalog.PopulateOnLogBookControllerInit();

			StatHooks.Init();
			EffectHooks.Init();
			DropHooks.Init();
			DisplayHooks.Init();
			LanguageOverride();

			ChangeText();
		}
		/*
		public void Update()
		{
			DebugDrops();
		}
		*/


		internal static void LoadAssets()
		{
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TPDespair.ZetAspects.zetaspectbundle"))
			{
				Assets = AssetBundle.LoadFromStream(stream);
			}
		}

		private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
		{
			addContentPackProvider(new ZetAspectsContent());
		}

		private static void LanguageOverride()
		{
			On.RoR2.Language.TokenIsRegistered += (orig, self, token) =>
			{
				if (token != null && LangTokens.ContainsKey(token)) return true;

				return orig(self, token);
			};

			On.RoR2.Language.GetString_string += (orig, token) =>
			{
				if (token != null && LangTokens.ContainsKey(token)) return LangTokens[token];

				return orig(token);
			};
		}



		private static void ChangeText()
		{
			string text;
			RegisterLanguageToken("ITEM_SEED_DESC", "Dealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>" + Configuration.LeechSeedHeal.Value + "</style> <style=cStack>(+" + Configuration.LeechSeedHeal.Value + " per stack)</style> <style=cIsHealing>health</style>.");
			RegisterLanguageToken("ITEM_HEADHUNTER_DESC", "Gain the <style=cIsDamage>power</style> of any killed elite monster for <style=cIsDamage>" + Configuration.HeadHunterBaseDuration.Value + "s</style> <style=cStack>(+" + Configuration.HeadHunterStackDuration.Value + "s per stack)</style>.");
			text = "Convert <style=cIsHealing>100%</style> of health into <style=cIsHealing>regenerating shields</style>.\nGain <style=cIsHealing>50%</style> <style=cStack>(+25% per stack)</style> extra <style=cIsHealing>shield</style> from conversion.";
			if (Configuration.TranscendenceRegen.Value > 0f)
			{
				text += "\nAt least <style=cIsHealing>";
				text += Configuration.TranscendenceRegen.Value * 100f + "%</style>";
				text += " of <style=cIsHealing>health regeneration</style> applies to <style=cIsHealing>shields</style>.";
			}
			RegisterLanguageToken("ITEM_SHIELDONLY_DESC", text);
		}



		public static float GetStackMagnitude(CharacterBody self, BuffDef buffDef)
		{
			if (self.inventory == null) return 1f;

			float aspect = self.inventory.GetItemCount(GetAspectItemDef(buffDef));
			float hunter = self.inventory.GetItemCount(RoR2Content.Items.HeadHunter);

			if (HasAspectEquipment(self, buffDef))
			{
				if (self.teamComponent.teamIndex == TeamIndex.Player)
				{
					aspect += Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);
				}
				else aspect += 1f;
			}

			if (aspect == 0f)
			{
				aspect = 1f;
				hunter = Mathf.Max(0f, hunter - 1f);
			}

			if (Configuration.HeadHunterCountExtra.Value != 0)
			{
				if (Configuration.HeadHunterCountExtra.Value > 0) 
				{
					hunter = Mathf.Min(hunter, Configuration.HeadHunterCountExtra.Value);
				}
				hunter *= Configuration.HeadHunterExtraEffect.Value;
			}

			return aspect + hunter;
		}

		public static ItemDef GetAspectItemDef(BuffDef buffDef)
		{
			BuffDef modBuffDef;

			if (buffDef == RoR2Content.Buffs.AffixWhite) return ZetAspectsContent.Items.ZetAspectIce;
			if (buffDef == RoR2Content.Buffs.AffixBlue) return ZetAspectsContent.Items.ZetAspectLightning;
			if (buffDef == RoR2Content.Buffs.AffixRed) return ZetAspectsContent.Items.ZetAspectFire;
			if (buffDef == RoR2Content.Buffs.AffixHaunted) return ZetAspectsContent.Items.ZetAspectCelestial;
			if (buffDef == RoR2Content.Buffs.AffixPoison) return ZetAspectsContent.Items.ZetAspectMalachite;
			if (buffDef == RoR2Content.Buffs.AffixLunar) return ZetAspectsContent.Items.ZetAspectPerfect;

			if (Catalog.EliteVariety.populated)
			{
				modBuffDef = Catalog.EliteVariety.Buffs.AffixArmored;
				if (modBuffDef && modBuffDef == buffDef) return ZetAspectsContent.Items.ZetAspectArmor;
				modBuffDef = Catalog.EliteVariety.Buffs.AffixBuffing;
				if (modBuffDef && modBuffDef == buffDef) return ZetAspectsContent.Items.ZetAspectBanner;
				modBuffDef = Catalog.EliteVariety.Buffs.AffixImpPlane;
				if (modBuffDef && modBuffDef == buffDef) return ZetAspectsContent.Items.ZetAspectImpale;
				modBuffDef = Catalog.EliteVariety.Buffs.AffixPillaging;
				if (modBuffDef && modBuffDef == buffDef) return ZetAspectsContent.Items.ZetAspectGolden;
				modBuffDef = Catalog.EliteVariety.Buffs.AffixSandstorm;
				if (modBuffDef && modBuffDef == buffDef) return ZetAspectsContent.Items.ZetAspectCyclone;
				modBuffDef = Catalog.EliteVariety.Buffs.AffixTinkerer;
				if (modBuffDef && modBuffDef == buffDef) return ZetAspectsContent.Items.ZetAspectTinker;
			}

			return null;
		}

		public static bool HasAspectEquipment(CharacterBody self, BuffDef buffDef)
		{
			EquipmentDef equipDef = GetAspectEquipmentDef(buffDef);

			if (!equipDef) return false;

			EquipmentIndex equipIndex = equipDef.equipmentIndex;

			if (self.inventory.currentEquipmentIndex == equipIndex) return true;
			if (self.inventory.alternateEquipmentIndex == equipIndex) return true;

			return false;
		}

		public static EquipmentDef GetAspectEquipmentDef(BuffDef buffDef)
		{
			BuffDef modBuffDef;

			if (buffDef == RoR2Content.Buffs.AffixWhite) return RoR2Content.Equipment.AffixWhite;
			if (buffDef == RoR2Content.Buffs.AffixBlue) return RoR2Content.Equipment.AffixBlue;
			if (buffDef == RoR2Content.Buffs.AffixRed) return RoR2Content.Equipment.AffixRed;
			if (buffDef == RoR2Content.Buffs.AffixHaunted) return RoR2Content.Equipment.AffixHaunted;
			if (buffDef == RoR2Content.Buffs.AffixPoison) return RoR2Content.Equipment.AffixPoison;
			if (buffDef == RoR2Content.Buffs.AffixLunar) return RoR2Content.Equipment.AffixLunar;

			if (Catalog.EliteVariety.populated)
			{
				modBuffDef = Catalog.EliteVariety.Buffs.AffixArmored;
				if (modBuffDef && modBuffDef == buffDef) return Catalog.EliteVariety.Equipment.AffixArmored;
				modBuffDef = Catalog.EliteVariety.Buffs.AffixBuffing;
				if (modBuffDef && modBuffDef == buffDef) return Catalog.EliteVariety.Equipment.AffixBuffing;
				modBuffDef = Catalog.EliteVariety.Buffs.AffixImpPlane;
				if (modBuffDef && modBuffDef == buffDef) return Catalog.EliteVariety.Equipment.AffixImpPlane;
				modBuffDef = Catalog.EliteVariety.Buffs.AffixPillaging;
				if (modBuffDef && modBuffDef == buffDef) return Catalog.EliteVariety.Equipment.AffixPillaging;
				modBuffDef = Catalog.EliteVariety.Buffs.AffixSandstorm;
				if (modBuffDef && modBuffDef == buffDef) return Catalog.EliteVariety.Equipment.AffixSandstorm;
				modBuffDef = Catalog.EliteVariety.Buffs.AffixTinkerer;
				if (modBuffDef && modBuffDef == buffDef) return Catalog.EliteVariety.Equipment.AffixTinkerer;
			}

			return null;
		}

		public static ItemIndex ItemizeEliteEquipment(EquipmentIndex equipmentIndex)
		{
			if (equipmentIndex == EquipmentIndex.None) return ItemIndex.None;

			EquipmentDef equipDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);

			EquipmentDef modEquipDef;

			if (equipDef == RoR2Content.Equipment.AffixWhite) return ZetAspectsContent.Items.ZetAspectIce.itemIndex;
			if (equipDef == RoR2Content.Equipment.AffixBlue) return ZetAspectsContent.Items.ZetAspectLightning.itemIndex;
			if (equipDef == RoR2Content.Equipment.AffixRed) return ZetAspectsContent.Items.ZetAspectFire.itemIndex;
			if (equipDef == RoR2Content.Equipment.AffixHaunted) return ZetAspectsContent.Items.ZetAspectCelestial.itemIndex;
			if (equipDef == RoR2Content.Equipment.AffixPoison) return ZetAspectsContent.Items.ZetAspectMalachite.itemIndex;
			if (equipDef == RoR2Content.Equipment.AffixLunar) return ZetAspectsContent.Items.ZetAspectPerfect.itemIndex;

			if (Catalog.EliteVariety.populated)
			{
				modEquipDef = Catalog.EliteVariety.Equipment.AffixArmored;
				if (modEquipDef && modEquipDef == equipDef) return ZetAspectsContent.Items.ZetAspectArmor.itemIndex;
				modEquipDef = Catalog.EliteVariety.Equipment.AffixBuffing;
				if (modEquipDef && modEquipDef == equipDef) return ZetAspectsContent.Items.ZetAspectBanner.itemIndex;
				modEquipDef = Catalog.EliteVariety.Equipment.AffixImpPlane;
				if (modEquipDef && modEquipDef == equipDef) return ZetAspectsContent.Items.ZetAspectImpale.itemIndex;
				modEquipDef = Catalog.EliteVariety.Equipment.AffixPillaging;
				if (modEquipDef && modEquipDef == equipDef) return ZetAspectsContent.Items.ZetAspectGolden.itemIndex;
				modEquipDef = Catalog.EliteVariety.Equipment.AffixSandstorm;
				if (modEquipDef && modEquipDef == equipDef) return ZetAspectsContent.Items.ZetAspectCyclone.itemIndex;
				modEquipDef = Catalog.EliteVariety.Equipment.AffixTinkerer;
				if (modEquipDef && modEquipDef == equipDef) return ZetAspectsContent.Items.ZetAspectTinker.itemIndex;
			}

			return ItemIndex.None;
		}

		public static EliteDef GetEquipmentEliteDef(EquipmentDef equipDef)
		{
			if (equipDef == null) return null;
			if (equipDef.passiveBuffDef == null) return null;
			return equipDef.passiveBuffDef.eliteDef;
		}



		public static string FormatSeconds(float seconds)
		{
			string s = seconds == 1.0f ? "" : "s";
			return seconds + " second" + s;
		}

		public static void RegisterLanguageToken(string token, string text)
		{
			//LanguageAPI.Add(token, text);
			if (!LangTokens.ContainsKey(token))
			{
				LangTokens.Add(token, text);
			}
			else
			{
				LangTokens[token] = text;
			}
		}


		/*
		private static void DebugDrops()
		{
			bool debugDrops = true;

			if (debugDrops)
			{
				if (Input.GetKeyDown(KeyCode.F2))
				{
					var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

					if (!Configuration.DropAsEquipment())
					{
						CreateDroplet(ZetAspectsContent.Items.ZetAspectIce, transform.position + new Vector3(-5f,5f,5f));
						CreateDroplet(ZetAspectsContent.Items.ZetAspectLightning, transform.position + new Vector3(0f, 5f, 7.5f));
						CreateDroplet(ZetAspectsContent.Items.ZetAspectFire, transform.position + new Vector3(5f, 5f, 5f));
						CreateDroplet(ZetAspectsContent.Items.ZetAspectCelestial, transform.position + new Vector3(-5f, 5f, -5f));
						CreateDroplet(ZetAspectsContent.Items.ZetAspectMalachite, transform.position + new Vector3(0f, 5f, -7.5f));
						CreateDroplet(ZetAspectsContent.Items.ZetAspectPerfect, transform.position + new Vector3(5f, 5f, -5f));
					}
					else
					{
						CreateDroplet(RoR2Content.Equipment.AffixWhite, transform.position + new Vector3(-5f, 5f, 5f));
						CreateDroplet(RoR2Content.Equipment.AffixBlue, transform.position + new Vector3(0f, 5f, 7.5f));
						CreateDroplet(RoR2Content.Equipment.AffixRed, transform.position + new Vector3(5f, 5f, 5f));
						CreateDroplet(RoR2Content.Equipment.AffixHaunted, transform.position + new Vector3(-5f, 5f, -5f));
						CreateDroplet(RoR2Content.Equipment.AffixPoison, transform.position + new Vector3(0f, 5f, -7.5f));
						CreateDroplet(RoR2Content.Equipment.AffixLunar, transform.position + new Vector3(5f, 5f, -5f));
					}
				}

				if (Input.GetKeyDown(KeyCode.F3))
				{
					var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

					if (Catalog.EliteVariety.populated)
					{
						if (!Configuration.DropAsEquipment())
						{
							CreateDroplet(ZetAspectsContent.Items.ZetAspectArmor, transform.position + new Vector3(-5f, 5f, 5f));
							CreateDroplet(ZetAspectsContent.Items.ZetAspectBanner, transform.position + new Vector3(0f, 5f, 7.5f));
							CreateDroplet(ZetAspectsContent.Items.ZetAspectImpale, transform.position + new Vector3(5f, 5f, 5f));
							CreateDroplet(ZetAspectsContent.Items.ZetAspectGolden, transform.position + new Vector3(-5f, 5f, -5f));
							CreateDroplet(ZetAspectsContent.Items.ZetAspectCyclone, transform.position + new Vector3(0f, 5f, -7.5f));
							CreateDroplet(ZetAspectsContent.Items.ZetAspectTinker, transform.position + new Vector3(5f, 5f, -5f));
						}
						else
						{
							CreateDroplet(Catalog.EliteVariety.Equipment.AffixArmored, transform.position + new Vector3(-5f, 5f, 5f));
							CreateDroplet(Catalog.EliteVariety.Equipment.AffixBuffing, transform.position + new Vector3(0f, 5f, 7.5f));
							CreateDroplet(Catalog.EliteVariety.Equipment.AffixImpPlane, transform.position + new Vector3(5f, 5f, 5f));
							CreateDroplet(Catalog.EliteVariety.Equipment.AffixPillaging, transform.position + new Vector3(-5f, 5f, -5f));
							CreateDroplet(Catalog.EliteVariety.Equipment.AffixSandstorm, transform.position + new Vector3(0f, 5f, -7.5f));
							CreateDroplet(Catalog.EliteVariety.Equipment.AffixTinkerer, transform.position + new Vector3(5f, 5f, -5f));
						}
					}
				}

				if (Input.GetKeyDown(KeyCode.F4))
				{
					var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
					PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Items.ShieldOnly.itemIndex), transform.position, transform.forward * 30f);
					PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Items.RepeatHeal.itemIndex), transform.position, transform.forward * -30f);
					PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Items.HeadHunter.itemIndex), transform.position, transform.right * 30f);
					PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarDagger.itemIndex), transform.position, transform.right * -30f);
				}

				if (Input.GetKeyDown(KeyCode.F5))
				{
					var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
					PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Items.Knurl.itemIndex), transform.position, transform.forward * 30f);
				}
			}
		}

		private static void CreateDroplet(ItemDef def, Vector3 pos)
		{
			CreateDroplet(PickupCatalog.FindPickupIndex(def.itemIndex), pos);
		}

		private static void CreateDroplet(EquipmentDef def, Vector3 pos)
		{
			CreateDroplet(PickupCatalog.FindPickupIndex(def.equipmentIndex), pos);
		}

		private static void CreateDroplet(PickupIndex index, Vector3 pos)
		{
			PickupDropletController.CreatePickupDroplet(index, pos, Vector3.zero);
		}
		*/
	}
}
