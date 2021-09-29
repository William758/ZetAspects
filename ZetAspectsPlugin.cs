using BepInEx;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine;
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
	[BepInDependency("com.KomradeSpectre.Aetherium", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.swuff.LostInTransit", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("com.themysticsword.elitevariety", BepInDependency.DependencyFlags.SoftDependency)]

	public class ZetAspectsPlugin : BaseUnityPlugin
	{
		public const string ModVer = "2.5.0";
		public const string ModName = "ZetAspects";
		public const string ModGuid = "com.TPDespair.ZetAspects";



		public static AssetBundle Assets;

		public static float MultiplayerChanceCompensation = 1f;
		public static bool DisableDrops = false;
		public static int RunDropCount = 0;



		public void Awake()
		{
			RoR2Application.isModded = true;
			NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append(ModGuid + ":" + ModVer);

			Configuration.Init(Config);
			ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;

			Catalog.SetOnLogBookControllerInit();

			StatHooks.Init();
			EffectHooks.Init();
			DropHooks.Init();
			DisplayHooks.Init();

			if (Catalog.LostInTransit.Enabled) LostInTransitHooks.Init();
			if (Catalog.Aetherium.Enabled) AetheriumHooks.Init();

			Language.Override();
			ChangeText();
		}



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



		internal static void InflictDotPrecise(GameObject victim, GameObject attacker, DotController.DotIndex dotIndex, float duration, float damage)
		{
			DotController.DotDef dotDef = DotController.dotDefs[(int)dotIndex];

			float ticks = duration / dotDef.interval;
			float rTicks = Mathf.Ceil(ticks);

			if (rTicks > 0f)
			{
				CharacterBody atkBody = attacker.GetComponent<CharacterBody>();

				float dotBaseDPS = atkBody.damage * (dotDef.damageCoefficient / dotDef.interval);
				float targetDPS = damage / (rTicks * dotDef.interval);

				float damageMult = targetDPS / dotBaseDPS;
				damageMult *= ticks / (rTicks + 1f);

				float tickedDuration = rTicks * dotDef.interval + (dotDef.interval - 0.01f);
				DotController.InflictDot(victim, attacker, dotIndex, tickedDuration, damageMult);
			}
		}



		internal static Sprite CreateAspectSprite(Sprite baseSprite, Sprite outlineSprite)
		{
			Color32[] basePixels = baseSprite.texture.GetPixels32();
			Color32[] outlinePixels = outlineSprite.texture.GetPixels32();

			// non-transparent outlinePixels overwrite basePixels
			for (var i = 0; i < outlinePixels.Length; ++i)
			{
				if (outlinePixels[i].a > 11) basePixels[i] = outlinePixels[i];
			}

			Texture2D newTexture = new Texture2D(128, 128, TextureFormat.RGBA32, false);

			newTexture.SetPixels32(basePixels);
			newTexture.Apply();

			return Sprite.Create(newTexture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f), 25.0f);
		}



		private static void ChangeText()
		{
			string text;
			Language.RegisterToken("ITEM_SEED_DESC", "Dealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>" + Configuration.LeechSeedHeal.Value + "</style> <style=cStack>(+" + Configuration.LeechSeedHeal.Value + " per stack)</style> <style=cIsHealing>health</style>.");
			Language.RegisterToken("ITEM_HEADHUNTER_DESC", "Gain the <style=cIsDamage>power</style> of any killed elite monster for <style=cIsDamage>" + Configuration.HeadHunterBaseDuration.Value + "s</style> <style=cStack>(+" + Configuration.HeadHunterStackDuration.Value + "s per stack)</style>.");
			text = "Convert <style=cIsHealing>100%</style> of health into <style=cIsHealing>regenerating shields</style>.\nGain <style=cIsHealing>50%</style> <style=cStack>(+25% per stack)</style> extra <style=cIsHealing>shield</style> from conversion.";
			if (Configuration.TranscendenceRegen.Value > 0f)
			{
				text += "\nAt least <style=cIsHealing>";
				text += Configuration.TranscendenceRegen.Value * 100f + "%</style>";
				text += " of <style=cIsHealing>health regeneration</style> applies to <style=cIsHealing>shields</style>.";
			}
			Language.RegisterToken("ITEM_SHIELDONLY_DESC", text);
		}



		public static float GetStackMagnitude(CharacterBody self, BuffDef buffDef)
		{
			Inventory inventory = self.inventory;
			if (inventory == null) return 1f;

			float aspect = inventory.GetItemCount(GetAspectItemDef(buffDef));
			float hunter = inventory.GetItemCount(RoR2Content.Items.HeadHunter);

			if (HasAspectEquipment(inventory, buffDef))
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

			if (Catalog.LostInTransit.populated)
			{
				modBuffDef = Catalog.LostInTransit.Buffs.AffixLeeching;
				if (modBuffDef && modBuffDef == buffDef) return ZetAspectsContent.Items.ZetAspectLeeching;
				modBuffDef = Catalog.LostInTransit.Buffs.AffixFrenzied;
				if (modBuffDef && modBuffDef == buffDef) return ZetAspectsContent.Items.ZetAspectFrenzied;
			}

			if (Catalog.Aetherium.populated)
			{
				modBuffDef = Catalog.Aetherium.Buffs.AffixSanguine;
				if (modBuffDef && modBuffDef == buffDef) return ZetAspectsContent.Items.ZetAspectSanguine;
			}

			return null;
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

			if (Catalog.LostInTransit.populated)
			{
				modBuffDef = Catalog.LostInTransit.Buffs.AffixLeeching;
				if (modBuffDef && modBuffDef == buffDef) return Catalog.LostInTransit.Equipment.AffixLeeching;
				modBuffDef = Catalog.LostInTransit.Buffs.AffixFrenzied;
				if (modBuffDef && modBuffDef == buffDef) return Catalog.LostInTransit.Equipment.AffixFrenzied;
			}

			if (Catalog.Aetherium.populated)
			{
				modBuffDef = Catalog.Aetherium.Buffs.AffixSanguine;
				if (modBuffDef && modBuffDef == buffDef) return Catalog.Aetherium.Equipment.AffixSanguine;
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

			if (Catalog.LostInTransit.populated)
			{
				modEquipDef = Catalog.LostInTransit.Equipment.AffixLeeching;
				if (modEquipDef && modEquipDef == equipDef) return ZetAspectsContent.Items.ZetAspectLeeching.itemIndex;
				modEquipDef = Catalog.LostInTransit.Equipment.AffixFrenzied;
				if (modEquipDef && modEquipDef == equipDef) return ZetAspectsContent.Items.ZetAspectFrenzied.itemIndex;
			}

			if (Catalog.Aetherium.populated)
			{
				modEquipDef = Catalog.Aetherium.Equipment.AffixSanguine;
				if (modEquipDef && modEquipDef == equipDef) return ZetAspectsContent.Items.ZetAspectSanguine.itemIndex;
			}

			return ItemIndex.None;
		}

		public static bool HasAspectItemOrEquipment(Inventory inventory, BuffDef buffDef)
		{
			if (HasAspectEquipment(inventory, buffDef)) return true;
			if (HasAspectItem(inventory, buffDef)) return true;

			return false;
		}

		public static bool HasAspectItemOrEquipment(Inventory inventory, ItemDef itemDef, EquipmentDef equipDef)
		{
			if (itemDef && inventory.GetItemCount(itemDef) > 0) return true;

			if (equipDef)
			{
				EquipmentIndex equipIndex = equipDef.equipmentIndex;

				if (inventory.currentEquipmentIndex == equipIndex) return true;
				if (inventory.alternateEquipmentIndex == equipIndex) return true;
			}

			return false;
		}

		public static bool HasAspectEquipment(Inventory inventory, BuffDef buffDef)
		{
			EquipmentDef equipDef = GetAspectEquipmentDef(buffDef);

			if (!equipDef) return false;

			EquipmentIndex equipIndex = equipDef.equipmentIndex;

			if (inventory.currentEquipmentIndex == equipIndex) return true;
			if (inventory.alternateEquipmentIndex == equipIndex) return true;

			return false;
		}

		public static bool HasAspectItem(Inventory inventory, BuffDef buffDef)
		{
			ItemDef itemDef = GetAspectItemDef(buffDef);

			if (!itemDef) return false;

			if (inventory.GetItemCount(itemDef) > 0) return true;

			return false;
		}

		public static EliteDef GetEquipmentEliteDef(EquipmentDef equipDef)
		{
			if (equipDef == null) return null;
			if (equipDef.passiveBuffDef == null) return null;
			return equipDef.passiveBuffDef.eliteDef;
		}
	}
}
