using RoR2;
using RoR2.ContentManagement;
using System.Collections;
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

			Buffs.Create();
			Items.Create();

			contentPack.buffDefs.Add(Buffs.buffDefs);
			contentPack.itemDefs.Add(Items.itemDefs);
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
			public static ItemDef ZetAspectIce;
			public static ItemDef ZetAspectLightning;
			public static ItemDef ZetAspectFire;
			public static ItemDef ZetAspectCelestial;
			public static ItemDef ZetAspectMalachite;
			public static ItemDef ZetAspectPerfect;

			public static ItemDef[] itemDefs;

			public static void Create()
			{
				ZetAspectIce = ZetAspects.ZetAspectIce.DefineItem();
				ZetAspectLightning = ZetAspects.ZetAspectLightning.DefineItem();
				ZetAspectFire = ZetAspects.ZetAspectFire.DefineItem();
				ZetAspectCelestial = ZetAspects.ZetAspectCelestial.DefineItem();
				ZetAspectMalachite = ZetAspects.ZetAspectMalachite.DefineItem();
				ZetAspectPerfect = ZetAspects.ZetAspectPerfect.DefineItem();

				itemDefs = new ItemDef[] { ZetAspectIce, ZetAspectLightning, ZetAspectFire, ZetAspectCelestial, ZetAspectMalachite, ZetAspectPerfect };
			}
		}
	}
}
