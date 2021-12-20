using RoR2;
using RoR2.ContentManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public class ZetAspectsContent : IContentPackProvider
	{
		public ContentPack contentPack = new ContentPack();

		public string identifier
		{
			get { return "ZetAspectsContent"; }
		}

		public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
		{
			ZetAspectsPlugin.LoadAssets();

			Sprites.Load();

			Buffs.Create();
			Items.Create();

			contentPack.buffDefs.Add(Buffs.buffDefs);
			contentPack.itemDefs.Add(Items.itemDefs.ToArray());
			args.ReportProgress(1f);
			yield break;
		}

		public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
		{
			ContentPack.Copy(contentPack, args.output);
			args.ReportProgress(1f);
			yield break;
		}

		public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
		{
			args.ReportProgress(1f);
			yield break;
		}



		public static class Sprites
		{
			public static Sprite HauntCloak;

			public static Sprite OutlineRed;
			public static Sprite OutlineOrange;
			public static Sprite OutlineYellow;
			public static Sprite OutlineGreen;
			public static Sprite OutlineBlue;
			public static Sprite OutlineWhite;

			public static Sprite AffixWhite;
			public static Sprite AffixBlue;
			public static Sprite AffixRed;
			public static Sprite AffixHaunted;
			public static Sprite AffixPoison;
			public static Sprite AffixLunar;

			public static Sprite AffixArmored;
			public static Sprite AffixBuffing;
			public static Sprite AffixImpPlane;
			public static Sprite AffixPillaging;
			public static Sprite AffixSandstorm;
			public static Sprite AffixTinkerer;

			public static Sprite AffixLeeching;
			public static Sprite AffixFrenzied;
			public static Sprite AffixVolatile;
			public static Sprite AffixBlighted;

			public static Sprite AffixSanguine;

			public static void Load()
			{
				HauntCloak = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texBuffHauntCloakIcon.png");

				OutlineRed = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineRed.png");
				OutlineOrange = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineOrange.png");
				OutlineYellow = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineYellow.png");
				OutlineGreen = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineGreen.png");
				OutlineBlue = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineBlue.png");
				OutlineWhite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineWhite.png");

				AffixWhite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixWhite.png");
				AffixBlue = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBlue.png");
				AffixRed = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixRed.png");
				AffixHaunted = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixHaunted.png");
				AffixPoison = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPoison.png");
				AffixLunar = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixLunar.png");

				if (Catalog.EliteVariety.Enabled)
				{
					AffixArmored = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixArmored.png");
					AffixBuffing = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBuffing.png");
					AffixImpPlane = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixImpPlane.png");
					AffixPillaging = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPillaging.png");
					AffixSandstorm = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixSandstorm.png");
					AffixTinkerer = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixTinkerer.png");
				}

				if (Catalog.LostInTransit.Enabled)
				{
					AffixLeeching = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixLeeching.png");
					AffixFrenzied = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixFrenzied.png");
					AffixVolatile = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixVolatile.png");
					AffixBlighted = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBlighted.png");
				}

				if (Catalog.Aetherium.Enabled)
				{
					AffixSanguine = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixSanguine.png");
				}
			}
		}



		public static class Buffs
		{
			public static BuffDef ZetHeadHunter;
			public static BuffDef ZetSapped;
			public static BuffDef ZetShredded;

			public static BuffDef[] buffDefs;

			public static void Create()
			{
				ZetHeadHunter = ScriptableObject.CreateInstance<BuffDef>();
				ZetHeadHunter.name = "ZetHeadHunter";
				ZetHeadHunter.buffColor = new Color(0.5f, 0.5f, 0.35f);
				ZetHeadHunter.canStack = true;
				ZetHeadHunter.isDebuff = false;
				ZetHeadHunter.iconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texBuffHeadHunterIcon.png");

				ZetSapped = ScriptableObject.CreateInstance<BuffDef>();
				ZetSapped.name = "ZetSapped";
				ZetSapped.buffColor = new Color(0.5f, 0.75f, 1f);
				ZetSapped.canStack = false;
				ZetSapped.isDebuff = true;
				ZetSapped.iconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texBuffSappedIcon.png");

				ZetShredded = ScriptableObject.CreateInstance<BuffDef>();
				ZetShredded.name = "ZetShredded";
				ZetShredded.buffColor = new Color(0.185f, 0.75f, 0.465f);
				ZetShredded.canStack = false;
				ZetShredded.isDebuff = true;
				ZetShredded.iconSprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texBuffShreddedIcon.png");

				buffDefs = new BuffDef[] { ZetHeadHunter, ZetSapped, ZetShredded };
			}
		}

		public static class Items
		{
			public static ItemDef ZetAspectsDropCountTracker;
			public static ItemDef ZetAspectsUpdateInventory;

			public static ItemDef ZetAspectIce;
			public static ItemDef ZetAspectLightning;
			public static ItemDef ZetAspectFire;
			public static ItemDef ZetAspectCelestial;
			public static ItemDef ZetAspectMalachite;
			public static ItemDef ZetAspectPerfect;

			public static ItemDef ZetAspectArmor;
			public static ItemDef ZetAspectBanner;
			public static ItemDef ZetAspectImpale;
			public static ItemDef ZetAspectGolden;
			public static ItemDef ZetAspectCyclone;
			public static ItemDef ZetAspectTinker;

			public static ItemDef ZetAspectLeeching;
			public static ItemDef ZetAspectFrenzied;
			public static ItemDef ZetAspectVolatile;
			public static ItemDef ZetAspectBlighted;

			public static ItemDef ZetAspectSanguine;

			public static List<ItemDef> itemDefs = new List<ItemDef>();

			public static void Create()
			{
				ZetAspectsDropCountTracker = ScriptableObject.CreateInstance<ItemDef>();
				ZetAspectsDropCountTracker.name = "ZetAspectsDropCountTracker";
				ZetAspectsDropCountTracker.tier = ItemTier.NoTier;
				ZetAspectsDropCountTracker.AutoPopulateTokens();
				ZetAspectsDropCountTracker.hidden = true;
				ZetAspectsDropCountTracker.canRemove = false;

				ZetAspectsUpdateInventory = ScriptableObject.CreateInstance<ItemDef>();
				ZetAspectsUpdateInventory.name = "ZetAspectsUpdateInventory";
				ZetAspectsUpdateInventory.tier = ItemTier.NoTier;
				ZetAspectsUpdateInventory.AutoPopulateTokens();
				ZetAspectsUpdateInventory.hidden = true;
				ZetAspectsUpdateInventory.canRemove = false;

				ZetAspectIce = ZetAspects.ZetAspectIce.DefineItem();
				ZetAspectLightning = ZetAspects.ZetAspectLightning.DefineItem();
				ZetAspectFire = ZetAspects.ZetAspectFire.DefineItem();
				ZetAspectCelestial = ZetAspects.ZetAspectCelestial.DefineItem();
				ZetAspectMalachite = ZetAspects.ZetAspectMalachite.DefineItem();
				ZetAspectPerfect = ZetAspects.ZetAspectPerfect.DefineItem();

				itemDefs.AddRange(new ItemDef[] { ZetAspectsDropCountTracker, ZetAspectsUpdateInventory, ZetAspectIce, ZetAspectLightning, ZetAspectFire, ZetAspectCelestial, ZetAspectMalachite, ZetAspectPerfect });

				if (Catalog.EliteVariety.Enabled)
				{
					ZetAspectArmor = ZetAspects.ZetAspectArmor.DefineItem();
					ZetAspectBanner = ZetAspects.ZetAspectBanner.DefineItem();
					ZetAspectImpale = ZetAspects.ZetAspectImpale.DefineItem();
					ZetAspectGolden = ZetAspects.ZetAspectGolden.DefineItem();
					ZetAspectCyclone = ZetAspects.ZetAspectCyclone.DefineItem();
					ZetAspectTinker = ZetAspects.ZetAspectTinker.DefineItem();

					itemDefs.AddRange(new ItemDef[] { ZetAspectArmor, ZetAspectBanner, ZetAspectImpale, ZetAspectGolden, ZetAspectCyclone, ZetAspectTinker });
				}

				if (Catalog.LostInTransit.Enabled)
				{
					ZetAspectLeeching = ZetAspects.ZetAspectLeeching.DefineItem();
					ZetAspectFrenzied = ZetAspects.ZetAspectFrenzied.DefineItem();
					ZetAspectVolatile = ZetAspects.ZetAspectVolatile.DefineItem();
					ZetAspectBlighted = ZetAspects.ZetAspectBlighted.DefineItem();

					itemDefs.AddRange(new ItemDef[] { ZetAspectLeeching, ZetAspectFrenzied, ZetAspectVolatile, ZetAspectBlighted });
				}

				if (Catalog.Aetherium.Enabled)
				{
					ZetAspectSanguine = ZetAspects.ZetAspectSanguine.DefineItem();

					itemDefs.AddRange(new ItemDef[] { ZetAspectSanguine });
				}
			}
		}
	}
}
