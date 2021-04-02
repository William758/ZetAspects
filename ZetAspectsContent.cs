
using RoR2;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class ZetAspectsContent
	{
		public static ContentPack contentPack;

		internal static void Init()
		{
			Buffs.Create();
			Items.Create();

			contentPack = new ContentPack
			{
				buffDefs = Buffs.buffDefs,
				itemDefs = Items.itemDefs,
			};

			AddContentPack(contentPack);
		}

		private static void AddContentPack(ContentPack contentPack)
		{
			On.RoR2.ContentManager.SetContentPacks += (orig, contentPackList) =>
			{
				contentPackList.Add(contentPack);
				orig(contentPackList);
			};
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
				ZetHeadHunter.buffColor = Color.grey;
				ZetHeadHunter.canStack = true;
				ZetHeadHunter.isDebuff = false;
				ZetHeadHunter.iconSprite = Resources.Load<Sprite>("textures/bufficons/texBuffAttackSpeedOnCritIcon");

				ZetSapped = ScriptableObject.CreateInstance<BuffDef>();
				ZetSapped.name = "ZetSapped";
				ZetSapped.buffColor = new Color(0.185f, 0.465f, 0.75f);
				ZetSapped.canStack = false;
				ZetSapped.isDebuff = true;
				ZetSapped.iconSprite = Resources.Load<Sprite>("textures/bufficons/texBuffNullifiedIcon");
				
				ZetShredded = ScriptableObject.CreateInstance<BuffDef>();
				ZetShredded.name = "ZetShredded";
				ZetShredded.buffColor = new Color(0.185f, 0.75f, 0.465f);
				ZetShredded.canStack = false;
				ZetShredded.isDebuff = true;
				ZetShredded.iconSprite = Resources.Load<Sprite>("textures/bufficons/texBuffPulverizeIcon");

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

			public static ItemDef[] itemDefs;

			public static void Create()
			{
				ZetAspectIce = ZetAspects.ZetAspectIce.DefineItem();
				ZetAspectLightning = ZetAspects.ZetAspectLightning.DefineItem();
				ZetAspectFire = ZetAspects.ZetAspectFire.DefineItem();
				ZetAspectCelestial = ZetAspects.ZetAspectCelestial.DefineItem();
				ZetAspectMalachite = ZetAspects.ZetAspectMalachite.DefineItem();

				itemDefs = new ItemDef[] { ZetAspectIce, ZetAspectLightning, ZetAspectFire, ZetAspectCelestial, ZetAspectMalachite };
			}
		}
	}
}
