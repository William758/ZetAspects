using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using RoR2.ContentManagement;
using RoR2.ExpansionManagement;

namespace TPDespair.ZetAspects
{
	public static partial class Catalog
	{
		public static AssetBundle Assets;



		public static bool menuVisited = false;
		public static bool setupComplete = false;
		public static bool setupIntermediate = false;
		public static bool setupCompat = false;
		public static bool setupInitialize = false;



		public static Dictionary<BuffIndex, ItemIndex> buffToItem = new Dictionary<BuffIndex, ItemIndex>();
		public static Dictionary<BuffIndex, EquipmentIndex> buffToEquip = new Dictionary<BuffIndex, EquipmentIndex>();

		public static Dictionary<EquipmentIndex, ItemIndex> equipToItem = new Dictionary<EquipmentIndex, ItemIndex>();

		public static List<ItemIndex> disabledItemIndexes = new List<ItemIndex>();

		public static List<ItemIndex> aspectItemIndexes = new List<ItemIndex>();
		public static List<BuffIndex> aspectBuffIndexes = new List<BuffIndex>();
		public static List<EquipmentIndex> aspectEquipIndexes = new List<EquipmentIndex>();

		public static List<ItemDef> transformableAspectItemDefs = new List<ItemDef>();



		internal static ItemTierDef BossItemTier;
		internal static ItemTierDef RedItemTier;

		internal static GameObject BossDropletPrefab;
		internal static GameObject LightningStakePrefab;
		internal static GameObject RejectTextPrefab;
		internal static GameObject CommandCubePrefab;
		internal static GameObject SmokeBombMiniPrefab;



		public static bool limitChillStacks = false;
		public static bool borboFrostBlade = false;
		public static bool shieldJump = false;
		public static bool aspectAbilities = false;
		public static bool immuneHealth = false;
		public static bool altIceActive = false;

		public static bool ChillCanStack => RoR2Content.Buffs.Slow80.canStack;



		public static bool dropWeightsAvailable = false;



		public static class Sprites
		{
			public static Sprite OutlineRed;
			public static Sprite OutlineOrange;
			public static Sprite OutlineYellow;
			public static Sprite OutlineBlue;
			public static Sprite OutlineVoid;

			public static Sprite NullOutlineRed;
			public static Sprite NullOutlineOrange;
			public static Sprite NullOutlineYellow;

			public static Sprite CrackedOutlineRed;
			public static Sprite CrackedOutlineOrange;
			public static Sprite CrackedOutlineYellow;

			public static Sprite AffixWhite;
			public static Sprite AffixBlue;
			public static Sprite AffixRed;
			public static Sprite AffixHaunted;
			public static Sprite AffixPoison;
			public static Sprite AffixLunar;

			public static Sprite AffixEarth;
			public static Sprite AffixVoid;

			public static Sprite AffixPlated;
			public static Sprite AffixWarped;
			public static Sprite AffixVeiled;
			public static Sprite AffixAragonite;

			public static Sprite AffixGold;

			public static Sprite AffixSanguine;

			public static Sprite AffixSepia;
			public static Sprite SepiaElite;

			public static Sprite AffixNullifier;

			public static Sprite AffixBlighted;

			public static Sprite AffixBackup;
			public static Sprite BackupDebuff;

			public static Sprite AffixPurity;

			public static Sprite AffixBarrier;
			public static Sprite AffixBlackHole;
			public static Sprite AffixMoney;
			public static Sprite AffixNight;
			public static Sprite AffixWater;
			public static Sprite AffixRealgar;

			public static Sprite HauntCloak;
			public static Sprite ZetHeadHunter;
			public static Sprite ZetSapped;
			public static Sprite ZetShredded;
			public static Sprite ZetPoached;
			public static Sprite ZetSepiaBlind;
			public static Sprite ZetElusive;



			public static void Load()
			{
				OutlineRed = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineRed.png");
				OutlineOrange = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineOrange.png");
				OutlineYellow = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineYellow.png");
				OutlineBlue = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineBlue.png");
				OutlineVoid = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineVoid.png");

				AffixWhite = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixWhite.png");
				AffixBlue = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBlue.png");
				AffixRed = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixRed.png");
				AffixHaunted = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixHaunted.png");
				AffixPoison = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPoison.png");
				AffixLunar = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixLunar.png");

				AffixEarth = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixEarth.png");
				AffixVoid = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixVoid.png");

				if (SpikeStrip.Enabled)
				{
					AffixPlated = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPlated.png");
					AffixWarped = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixWarped.png");
					AffixVeiled = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixVeiled.png");
					AffixAragonite = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixAragonite.png");
					ZetElusive = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffElusive.png");
				}

				if (GoldenCoastPlus.Enabled)
				{
					AffixGold = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPillaging.png");
				}
				
				if (Aetherium.Enabled)
				{
					AffixSanguine = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixSanguine.png");
				}

				if (Bubbet.Enabled)
				{
					AffixSepia = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixSepia.png");
					SepiaElite = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffAffixSepia.png");
					ZetSepiaBlind = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffSepiaBlind.png");
				}

				if (WarWisp.Enabled)
				{
					NullOutlineRed = Assets.LoadAsset<Sprite>("Assets/Icons/texNullOutlineRed.png");
					NullOutlineOrange = Assets.LoadAsset<Sprite>("Assets/Icons/texNullOutlineOrange.png");
					NullOutlineYellow = Assets.LoadAsset<Sprite>("Assets/Icons/texNullOutlineYellow.png");

					AffixNullifier = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixNullifier.png");
				}

				AffixBlighted = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBlighted.png");
				
				if (GOTCE.Enabled)
				{
					CrackedOutlineRed = Assets.LoadAsset<Sprite>("Assets/Icons/texCrackedOutlineRed.png");
					CrackedOutlineOrange = Assets.LoadAsset<Sprite>("Assets/Icons/texCrackedOutlineOrange.png");
					CrackedOutlineYellow = Assets.LoadAsset<Sprite>("Assets/Icons/texCrackedOutlineYellow.png");

					AffixBackup = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBackup.png");
					BackupDebuff = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffNoSecondary.png");
				}
				
				if (Thalasso.Enabled)
				{
					AffixPurity = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPurity.png");
				}

				if (RisingTides.Enabled)
				{
					AffixBarrier = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBarrier.png");
					AffixBlackHole = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBlackHole.png");
					AffixMoney = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixArmored.png");
					AffixNight = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixNight.png");
					AffixWater = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixWater.png");
					AffixRealgar = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixImpPlane.png");
				}

				HauntCloak = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffHauntCloak.png");
				ZetHeadHunter = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffHeadHunter.png");
				ZetSapped = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffSapped.png");
				ZetShredded = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffShredded.png");
				ZetPoached = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffPoached.png");
			}
		}

		public static class Prefabs
		{
			public static GameObject AffixVoid;
			public static GameObject AffixSepia;
			public static GameObject AffixPure;



			public static void Load()
			{
				AffixVoid = Assets.LoadAsset<GameObject>("Assets/Prefabs/prefabAffixVoid.prefab");

				if (Bubbet.Enabled)
				{
					AffixSepia = Assets.LoadAsset<GameObject>("Assets/Prefabs/prefabAffixSepia.prefab");
				}

				if (Thalasso.Enabled)
				{
					AffixPure = Assets.LoadAsset<GameObject>("Assets/Prefabs/prefabAffixPure.prefab");
				}
			}
		}

		public static Sprite CreateAspectSprite(Sprite baseSprite, Sprite outlineSprite)
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



		public static class Buff
		{
			public static BuffDef ZetHeadHunter;
			public static BuffDef ZetSapped;
			public static BuffDef ZetShredded;
			public static BuffDef ZetPoached;
			public static BuffDef ZetSepiaBlind;
			public static BuffDef ZetElusive;

			public static BuffDef BackupDebuff;
			public static BuffDef NightSpeed;
			public static BuffDef NightBlind;



			public static BuffDef AffixWhite;
			public static BuffDef AffixBlue;
			public static BuffDef AffixRed;
			public static BuffDef AffixHaunted;
			public static BuffDef AffixPoison;
			public static BuffDef AffixLunar;

			public static BuffDef AffixEarth;
			public static BuffDef AffixVoid;

			public static BuffDef AffixPlated;
			public static BuffDef AffixWarped;
			public static BuffDef AffixVeiled;
			public static BuffDef AffixAragonite;

			public static BuffDef AffixGold;

			public static BuffDef AffixSanguine;

			public static BuffDef AffixSepia;

			public static BuffDef AffixNullifier;

			public static BuffDef AffixBlighted;

			public static BuffDef AffixBackup;

			public static BuffDef AffixPurity;

			public static BuffDef AffixBarrier;
			public static BuffDef AffixBlackHole;
			public static BuffDef AffixMoney;
			public static BuffDef AffixNight;
			public static BuffDef AffixWater;
			public static BuffDef AffixRealgar;
		}

		public static class Equip
		{
			public static EquipmentDef AffixWhite;
			public static EquipmentDef AffixBlue;
			public static EquipmentDef AffixRed;
			public static EquipmentDef AffixHaunted;
			public static EquipmentDef AffixPoison;
			public static EquipmentDef AffixLunar;

			public static EquipmentDef AffixEarth;
			public static EquipmentDef AffixVoid;

			public static EquipmentDef AffixPlated;
			public static EquipmentDef AffixWarped;
			public static EquipmentDef AffixVeiled;
			public static EquipmentDef AffixAragonite;

			public static EquipmentDef AffixGold;

			public static EquipmentDef AffixSanguine;

			public static EquipmentDef AffixSepia;

			public static EquipmentDef AffixNullifier;

			public static EquipmentDef AffixBlighted;

			public static EquipmentDef AffixBackup;

			public static EquipmentDef AffixPurity;

			public static EquipmentDef AffixBarrier;
			public static EquipmentDef AffixBlackHole;
			public static EquipmentDef AffixMoney;
			public static EquipmentDef AffixNight;
			public static EquipmentDef AffixWater;
			public static EquipmentDef AffixRealgar;
		}

		public static class Item
		{
			public static ItemDef ZetAspectsDropCountTracker;
			public static ItemDef ZetAspectsUpdateInventory;



			public static ItemDef ZetAspectWhite;
			public static ItemDef ZetAspectBlue;
			public static ItemDef ZetAspectRed;
			public static ItemDef ZetAspectHaunted;
			public static ItemDef ZetAspectPoison;
			public static ItemDef ZetAspectLunar;

			public static ItemDef ZetAspectEarth;
			public static ItemDef ZetAspectVoid;

			public static ItemDef ZetAspectPlated;
			public static ItemDef ZetAspectWarped;
			public static ItemDef ZetAspectVeiled;
			public static ItemDef ZetAspectAragonite;

			public static ItemDef ZetAspectGold;

			public static ItemDef ZetAspectSanguine;

			public static ItemDef ZetAspectSepia;

			public static ItemDef ZetAspectNullifier;

			public static ItemDef ZetAspectBlighted;

			public static ItemDef ZetAspectBackup;

			public static ItemDef ZetAspectPurity;

			public static ItemDef ZetAspectBarrier;
			public static ItemDef ZetAspectBlackHole;
			public static ItemDef ZetAspectMoney;
			public static ItemDef ZetAspectNight;
			public static ItemDef ZetAspectWater;
			public static ItemDef ZetAspectRealgar;
		}

		public static EffectDef RejectTextDef;

		public static ArtifactIndex diluvianArtifactIndex = ArtifactIndex.None;
		public static BodyIndex mithrixBodyIndex = BodyIndex.None;
		public static BodyIndex voidlingBodyIndex = BodyIndex.None;
		public static BodyIndex urchinTurretBodyIndex = BodyIndex.None;
		public static BodyIndex urchinOrbitalBodyIndex = BodyIndex.None;
		public static BodyIndex healOrbBodyIndex = BodyIndex.None;
		public static BuffIndex altSlow80 = BuffIndex.None;
		public static BuffIndex rageAura = BuffIndex.None;
		public static BuffIndex nullifierRecipient = BuffIndex.None;
		public static BuffIndex waterInvuln = BuffIndex.None;
		public static BuffIndex reactorInvuln = BuffIndex.None;
		public static ItemTier lunarVoidTier = ItemTier.AssignedAtRuntime;



		public static ItemIndex GetAspectItemIndex(BuffIndex buffIndex)
		{
			if (buffToItem.ContainsKey(buffIndex)) return buffToItem[buffIndex];
			return ItemIndex.None;
		}

		public static EquipmentIndex GetAspectEquipIndex(BuffIndex buffIndex)
		{
			if (buffToEquip.ContainsKey(buffIndex)) return buffToEquip[buffIndex];
			return EquipmentIndex.None;
		}

		public static ItemIndex ItemizeEliteEquipment(EquipmentIndex equipIndex)
		{
			if (equipToItem.ContainsKey(equipIndex)) return equipToItem[equipIndex];
			return ItemIndex.None;
		}



		public static float GetStackMagnitude(CharacterBody self, BuffDef buffDef)
		{
			Inventory inventory = self.inventory;
			if (!inventory) return 1f;

			float aspect = CountAspectEquipment(inventory, buffDef);

			if (aspect > 0f && self.isPlayerControlled)
			{
				aspect *= Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);
			}

			aspect += inventory.GetItemCount(GetAspectItemIndex(buffDef.buffIndex));

			return Mathf.Max(1f, aspect);
		}



		public static bool HasAspectItemOrEquipment(Inventory inventory, BuffDef buffDef)
		{
			if (CountAspectEquipment(inventory, buffDef) > 0) return true;
			if (HasAspectItem(inventory, buffDef)) return true;

			return false;
		}

		public static bool HasAspectItemOrEquipment(Inventory inventory, BuffIndex buffIndex)
		{
			if (CountAspectEquipment(inventory, buffIndex) > 0) return true;
			if (HasAspectItem(inventory, buffIndex)) return true;

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

		public static int CountAspectEquipment(Inventory inventory, BuffDef buffDef)
		{
			return CountAspectEquipment(inventory, buffDef.buffIndex);
		}

		public static int CountAspectEquipment(Inventory inventory, BuffIndex buffIndex)
		{
			EquipmentIndex equipIndex = GetAspectEquipIndex(buffIndex);

			if (equipIndex == EquipmentIndex.None) return 0;

			int count = 0;

			if (inventory.currentEquipmentIndex == equipIndex) count++;
			if (inventory.alternateEquipmentIndex == equipIndex) count++;

			return count;
		}

		public static bool HasAspectItem(Inventory inventory, BuffDef buffDef)
		{
			return HasAspectItem(inventory, buffDef.buffIndex);
		}

		public static bool HasAspectItem(Inventory inventory, BuffIndex buffIndex)
		{
			ItemIndex itemIndex = GetAspectItemIndex(buffIndex);

			if (itemIndex == ItemIndex.None) return false;

			if (inventory.GetItemCount(itemIndex) > 0) return true;

			return false;
		}

		public static EliteDef GetEquipmentEliteDef(EquipmentDef equipDef)
		{
			if (equipDef == null) return null;
			if (equipDef.passiveBuffDef == null) return null;

			return equipDef.passiveBuffDef.eliteDef;
		}



		internal static void OnAwake()
		{
			RoR2Application.isModded = true;
			string modString = ZetAspectsPlugin.ModGuid + ":" + ZetAspectsPlugin.ModVer;
			NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append(modString);
			Logger.Info("Adding " + modString + " to the networkModList.");

			ContentManager.collectContentPackProviders += AddContentPackProvider;

			if (Configuration.AspectVoidContagiousItem.Value) SetupItemTransformations();
			SetupListeners();

			ItemEntrySelectableHook();
			EquipmentEntrySelectableHook();
		}

		private static void AddContentPackProvider(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
		{
			addContentPackProvider(new ZetAspectsContent());
		}



		internal static void OnContentLoadStart()
		{
			LoadAssets();
			LoadResources();
			Sprites.Load();
			Prefabs.Load();

			CreateBuffs();
			CreateItems();
		}

		private static void LoadAssets()
		{
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TPDespair.ZetAspects.zetaspectbundle"))
			{
				Assets = AssetBundle.LoadFromStream(stream);
			}
		}

		private static void LoadResources()
		{
			BossItemTier = LegacyResourcesAPI.Load<ItemTierDef>("ItemTierDefs/BossTierDef");
			RedItemTier = LegacyResourcesAPI.Load<ItemTierDef>("ItemTierDefs/Tier3Def");

			BossDropletPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/ItemPickups/BossOrb");
			LightningStakePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/LightningStake");
			RejectTextPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/DamageRejected");
			CommandCubePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/CommandCube");
			SmokeBombMiniPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/Bandit2SmokeBombMini");
		}

		private static void CreateBuffs()
		{
			BuffDef ZetHeadHunter = ScriptableObject.CreateInstance<BuffDef>();
			ZetHeadHunter.name = "ZetHeadHunter";
			ZetHeadHunter.buffColor = new Color(0.5f, 0.5f, 0.35f);
			ZetHeadHunter.canStack = true;
			ZetHeadHunter.isDebuff = false;
			ZetHeadHunter.iconSprite = Sprites.ZetHeadHunter;
			Buff.ZetHeadHunter = ZetHeadHunter;
			ZetAspectsContent.buffDefs.Add(ZetHeadHunter);

			BuffDef ZetSapped = ScriptableObject.CreateInstance<BuffDef>();
			ZetSapped.name = "ZetSapped";
			ZetSapped.buffColor = new Color(0.5f, 0.75f, 1f);
			ZetSapped.canStack = false;
			ZetSapped.isDebuff = true;
			ZetSapped.iconSprite = Sprites.ZetSapped;
			Buff.ZetSapped = ZetSapped;
			ZetAspectsContent.buffDefs.Add(ZetSapped);

			BuffDef ZetShredded = ScriptableObject.CreateInstance<BuffDef>();
			ZetShredded.name = "ZetShredded";
			ZetShredded.buffColor = new Color(0.185f, 0.75f, 0.465f);
			ZetShredded.canStack = false;
			ZetShredded.isDebuff = true;
			ZetShredded.iconSprite = Sprites.ZetShredded;
			Buff.ZetShredded = ZetShredded;
			ZetAspectsContent.buffDefs.Add(ZetShredded);

			BuffDef ZetPoached = ScriptableObject.CreateInstance<BuffDef>();
			ZetPoached.name = "ZetPoached";
			ZetPoached.buffColor = new Color(0.5f, 0.75f, 0.185f);
			ZetPoached.canStack = false;
			ZetPoached.isDebuff = true;
			ZetPoached.iconSprite = Sprites.ZetPoached;
			Buff.ZetPoached = ZetPoached;
			ZetAspectsContent.buffDefs.Add(ZetPoached);

			if (Bubbet.Enabled)
			{
				BuffDef ZetSepiaBlind = ScriptableObject.CreateInstance<BuffDef>();
				ZetSepiaBlind.name = "ZetSepiaBlind";
				ZetSepiaBlind.buffColor = Color.white;
				ZetSepiaBlind.canStack = false;
				ZetSepiaBlind.isDebuff = true;
				ZetSepiaBlind.iconSprite = Sprites.ZetSepiaBlind;
				Buff.ZetSepiaBlind = ZetSepiaBlind;
				ZetAspectsContent.buffDefs.Add(ZetSepiaBlind);
			}

			if (SpikeStrip.Enabled)
			{
				BuffDef ZetElusive = ScriptableObject.CreateInstance<BuffDef>();
				ZetElusive.name = "ZetElusive";
				ZetElusive.buffColor = new Color(0.185f, 0.465f, 0.75f);
				ZetElusive.canStack = true;
				ZetElusive.isDebuff = false;
				ZetElusive.iconSprite = Sprites.ZetElusive;
				Buff.ZetElusive = ZetElusive;
				ZetAspectsContent.buffDefs.Add(ZetElusive);
			}
		}

		private static void CreateItems()
		{
			ItemDef ZetAspectsDropCountTracker = ScriptableObject.CreateInstance<ItemDef>();
			ZetAspectsDropCountTracker.name = "ZetAspectsDropCountTracker";
			AssignDepricatedTier(ZetAspectsDropCountTracker, ItemTier.NoTier);
			ZetAspectsDropCountTracker.AutoPopulateTokens();
			ZetAspectsDropCountTracker.hidden = true;
			ZetAspectsDropCountTracker.canRemove = false;
			Item.ZetAspectsDropCountTracker = ZetAspectsDropCountTracker;
			ZetAspectsContent.itemDefs.Add(ZetAspectsDropCountTracker);

			ItemDef ZetAspectsUpdateInventory = ScriptableObject.CreateInstance<ItemDef>();
			ZetAspectsUpdateInventory.name = "ZetAspectsUpdateInventory";
			AssignDepricatedTier(ZetAspectsUpdateInventory, ItemTier.NoTier);
			ZetAspectsUpdateInventory.AutoPopulateTokens();
			ZetAspectsUpdateInventory.hidden = true;
			ZetAspectsUpdateInventory.canRemove = false;
			Item.ZetAspectsUpdateInventory = ZetAspectsUpdateInventory;
			ZetAspectsContent.itemDefs.Add(ZetAspectsUpdateInventory);

			ItemDef ZetAspectWhite = Items.ZetAspectWhite.DefineItem();
			Item.ZetAspectWhite = ZetAspectWhite;
			ZetAspectsContent.itemDefs.Add(ZetAspectWhite);
			transformableAspectItemDefs.Add(ZetAspectWhite);

			ItemDef ZetAspectBlue = Items.ZetAspectBlue.DefineItem();
			Item.ZetAspectBlue = ZetAspectBlue;
			ZetAspectsContent.itemDefs.Add(ZetAspectBlue);
			transformableAspectItemDefs.Add(ZetAspectBlue);

			ItemDef ZetAspectRed = Items.ZetAspectRed.DefineItem();
			Item.ZetAspectRed = ZetAspectRed;
			ZetAspectsContent.itemDefs.Add(ZetAspectRed);
			transformableAspectItemDefs.Add(ZetAspectRed);

			ItemDef ZetAspectHaunted = Items.ZetAspectHaunted.DefineItem();
			Item.ZetAspectHaunted = ZetAspectHaunted;
			ZetAspectsContent.itemDefs.Add(ZetAspectHaunted);
			transformableAspectItemDefs.Add(ZetAspectHaunted);

			ItemDef ZetAspectPoison = Items.ZetAspectPoison.DefineItem();
			Item.ZetAspectPoison = ZetAspectPoison;
			ZetAspectsContent.itemDefs.Add(ZetAspectPoison);
			transformableAspectItemDefs.Add(ZetAspectPoison);

			ItemDef ZetAspectLunar = Items.ZetAspectLunar.DefineItem();
			Item.ZetAspectLunar = ZetAspectLunar;
			ZetAspectsContent.itemDefs.Add(ZetAspectLunar);
			transformableAspectItemDefs.Add(ZetAspectLunar);

			ItemDef ZetAspectEarth = Items.ZetAspectEarth.DefineItem();
			Item.ZetAspectEarth = ZetAspectEarth;
			ZetAspectsContent.itemDefs.Add(ZetAspectEarth);
			transformableAspectItemDefs.Add(ZetAspectEarth);

			ItemDef ZetAspectVoid = Items.ZetAspectVoid.DefineItem();
			Item.ZetAspectVoid = ZetAspectVoid;
			ZetAspectsContent.itemDefs.Add(ZetAspectVoid);

			if (SpikeStrip.Enabled)
			{
				ItemDef ZetAspectPlated = Items.ZetAspectPlated.DefineItem();
				Item.ZetAspectPlated = ZetAspectPlated;
				ZetAspectsContent.itemDefs.Add(ZetAspectPlated);
				transformableAspectItemDefs.Add(ZetAspectPlated);

				ItemDef ZetAspectWarped = Items.ZetAspectWarped.DefineItem();
				Item.ZetAspectWarped = ZetAspectWarped;
				ZetAspectsContent.itemDefs.Add(ZetAspectWarped);
				transformableAspectItemDefs.Add(ZetAspectWarped);

				ItemDef ZetAspectVeiled = Items.ZetAspectVeiled.DefineItem();
				Item.ZetAspectVeiled = ZetAspectVeiled;
				ZetAspectsContent.itemDefs.Add(ZetAspectVeiled);
				transformableAspectItemDefs.Add(ZetAspectVeiled);

				ItemDef ZetAspectAragonite = Items.ZetAspectAragonite.DefineItem();
				Item.ZetAspectAragonite = ZetAspectAragonite;
				ZetAspectsContent.itemDefs.Add(ZetAspectAragonite);
				transformableAspectItemDefs.Add(ZetAspectAragonite);
			}

			if (GoldenCoastPlus.Enabled)
			{
				ItemDef ZetAspectGold = Items.ZetAspectGold.DefineItem();
				Item.ZetAspectGold = ZetAspectGold;
				ZetAspectsContent.itemDefs.Add(ZetAspectGold);
				transformableAspectItemDefs.Add(ZetAspectGold);
			}
			
			if (Aetherium.Enabled)
			{
				ItemDef ZetAspectSanguine = Items.ZetAspectSanguine.DefineItem();
				Item.ZetAspectSanguine = ZetAspectSanguine;
				ZetAspectsContent.itemDefs.Add(ZetAspectSanguine);
				transformableAspectItemDefs.Add(ZetAspectSanguine);
			}

			if (Bubbet.Enabled)
			{
				ItemDef ZetAspectSepia = Items.ZetAspectSepia.DefineItem();
				Item.ZetAspectSepia = ZetAspectSepia;
				ZetAspectsContent.itemDefs.Add(ZetAspectSepia);
				transformableAspectItemDefs.Add(ZetAspectSepia);
			}

			if (WarWisp.Enabled)
			{
				ItemDef ZetAspectNullifier = Items.ZetAspectNullifier.DefineItem();
				Item.ZetAspectNullifier = ZetAspectNullifier;
				ZetAspectsContent.itemDefs.Add(ZetAspectNullifier);
				transformableAspectItemDefs.Add(ZetAspectNullifier);
			}

			ItemDef ZetAspectBlighted = Items.ZetAspectBlighted.DefineItem();
			Item.ZetAspectBlighted = ZetAspectBlighted;
			ZetAspectsContent.itemDefs.Add(ZetAspectBlighted);
			transformableAspectItemDefs.Add(ZetAspectBlighted);
			
			if (GOTCE.Enabled)
			{
				ItemDef ZetAspectBackup = Items.ZetAspectBackup.DefineItem();
				Item.ZetAspectBackup = ZetAspectBackup;
				ZetAspectsContent.itemDefs.Add(ZetAspectBackup);
				transformableAspectItemDefs.Add(ZetAspectBackup);
			}

			if (Thalasso.Enabled)
			{
				ItemDef ZetAspectPurity = Items.ZetAspectPurity.DefineItem();
				Item.ZetAspectPurity = ZetAspectPurity;
				ZetAspectsContent.itemDefs.Add(ZetAspectPurity);
				transformableAspectItemDefs.Add(ZetAspectPurity);
			}

			if (RisingTides.Enabled)
			{
				ItemDef ZetAspectBarrier = Items.ZetAspectBarrier.DefineItem();
				Item.ZetAspectBarrier = ZetAspectBarrier;
				ZetAspectsContent.itemDefs.Add(ZetAspectBarrier);
				transformableAspectItemDefs.Add(ZetAspectBarrier);

				ItemDef ZetAspectBlackHole = Items.ZetAspectBlackHole.DefineItem();
				Item.ZetAspectBlackHole = ZetAspectBlackHole;
				ZetAspectsContent.itemDefs.Add(ZetAspectBlackHole);
				transformableAspectItemDefs.Add(ZetAspectBlackHole);

				ItemDef ZetAspectMoney = Items.ZetAspectMoney.DefineItem();
				Item.ZetAspectMoney = ZetAspectMoney;
				ZetAspectsContent.itemDefs.Add(ZetAspectMoney);
				transformableAspectItemDefs.Add(ZetAspectMoney);

				ItemDef ZetAspectNight = Items.ZetAspectNight.DefineItem();
				Item.ZetAspectNight = ZetAspectNight;
				ZetAspectsContent.itemDefs.Add(ZetAspectNight);
				transformableAspectItemDefs.Add(ZetAspectNight);

				ItemDef ZetAspectWater = Items.ZetAspectWater.DefineItem();
				Item.ZetAspectWater = ZetAspectWater;
				ZetAspectsContent.itemDefs.Add(ZetAspectWater);
				transformableAspectItemDefs.Add(ZetAspectWater);

				ItemDef ZetAspectRealgar = Items.ZetAspectRealgar.DefineItem();
				Item.ZetAspectRealgar = ZetAspectRealgar;
				ZetAspectsContent.itemDefs.Add(ZetAspectRealgar);
				transformableAspectItemDefs.Add(ZetAspectRealgar);
			}
		}

		internal static void AssignDepricatedTier(ItemDef itemDef, ItemTier itemTier)
		{
			#pragma warning disable CS0618 // Type or member is obsolete
			itemDef.deprecatedTier = itemTier;
			#pragma warning restore CS0618 // Type or member is obsolete
		}



		private static void SetupItemTransformations()
		{
			On.RoR2.Items.ContagiousItemManager.Init += (orig) =>
			{
				List<ItemDef.Pair> transformationTable = ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem].ToList();

				foreach (ItemDef itemDef in transformableAspectItemDefs)
				{
					transformationTable.Add(new ItemDef.Pair() { itemDef1 = itemDef, itemDef2 = Item.ZetAspectVoid });
					Logger.Info("Successfully added aspect transformation for " + itemDef.name);
				}

				ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = transformationTable.ToArray();

				orig();
			};
		}



		private static void SetupListeners()
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
					PreInitValidation();
				}
				catch (Exception ex)
				{
					Logger.Error(ex);
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
					PreInitValidation();
				}
				catch (Exception ex)
				{
					Logger.Error(ex);
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
					Logger.Warn("Failed To Setup Catalog!");
					Logger.Error(ex);
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
					OnMenuEnteredFirstTime();
				}
				catch (Exception ex)
				{
					Logger.Error(ex);
				}
			};
		}



		private static void ItemEntrySelectableHook()
		{
			On.RoR2.UI.LogBook.LogBookController.CanSelectItemEntry += (orig, itemDef, expAva) =>
			{
				if (itemDef && aspectItemIndexes.Contains(itemDef.itemIndex))
				{
					if (DropHooks.CanObtainItem() && HasRequiredExpansion(itemDef.requiredExpansion, expAva))
					{
						if (!disabledItemIndexes.Contains(itemDef.itemIndex)) return true;
					}

					return false;
				}

				return orig(itemDef, expAva);
			};
		}

		private static void EquipmentEntrySelectableHook()
		{
			On.RoR2.UI.LogBook.LogBookController.CanSelectEquipmentEntry += (orig, equipDef, expAva) =>
			{
				if (equipDef && aspectEquipIndexes.Contains(equipDef.equipmentIndex))
				{
					if (DropHooks.CanObtainEquipment() && HasRequiredExpansion(equipDef.requiredExpansion, expAva))
					{
						ItemIndex itemIndex = ItemizeEliteEquipment(equipDef.equipmentIndex);

						if (!disabledItemIndexes.Contains(itemIndex)) return true;
					}

					return false;
				}

				return orig(equipDef, expAva);
			};
		}

		private static bool HasRequiredExpansion(ExpansionDef expansionDef, Dictionary<ExpansionDef, bool> expansionAvailability)
		{
			return expansionDef == null || !expansionAvailability.ContainsKey(expansionDef) || expansionAvailability[expansionDef];
		}



		// disable item if equipment not found else set equipment icon
		private static void PreInitValidation()
		{
			RiskOfRain.PreInit();
			SpikeStrip.PreInit();
			GoldenCoastPlus.PreInit();
			Aetherium.PreInit();
			Bubbet.PreInit();
			WarWisp.PreInit();
			Blighted.PreInit();
			GOTCE.PreInit();
			Thalasso.PreInit();
			RisingTides.PreInit();
		}

		private static void SetupCatalog()
		{
			if (setupComplete) return;

			Logger.Info("Catalog Setup Initialized");

			setupInitialize = true;

			if (PluginLoaded("com.Borbo.ArtificerExtended")) limitChillStacks = true;
			if (PluginLoaded("com.Borbo.BORBO")) borboFrostBlade = true;
			if (PluginLoaded("com.TransRights.RealisticTransgendence")) shieldJump = true;// Reflection Config
			if (PluginLoaded("com.themysticsword.aspectabilities")) aspectAbilities = true;
			if (PluginLoaded("com.DestroyedClone.HealthbarImmune")) immuneHealth = true;
			if (PluginLoaded("com.TPDespair.AlternateIceAbility")) altIceActive = true;

			EffectIndex effectIndex = EffectCatalog.FindEffectIndexFromPrefab(RejectTextPrefab);
			RejectTextDef = EffectCatalog.GetEffectDef(effectIndex);
			
			diluvianArtifactIndex = ArtifactCatalog.FindArtifactIndex("ARTIFACT_DILUVIFACT");
			altSlow80 = BuffCatalog.FindBuffIndex("EliteReworksSlow80");

			if (PluginLoaded("com.plasmacore.PlasmaCoreSpikestripContent"))
			{
				rageAura = Compat.PlasmaSpikeStrip.GetRageBuffWardBuffIndex();
			}

			if (PluginLoaded("com.PopcornFactory.WispMod"))
			{
				nullifierRecipient = BuffCatalog.FindBuffIndex("Nullifier Armour Buff");
			}

			if (PluginLoaded("com.TeamMoonstorm.Starstorm2-Nightly"))
			{
				reactorInvuln = BuffCatalog.FindBuffIndex("BuffReactor");
			}

			if (PluginLoaded("com.themysticsword.risingtides"))
			{
				waterInvuln = BuffCatalog.FindBuffIndex("RisingTides_WaterInvincibility");
			}

			if (aspectAbilities)
			{
				urchinOrbitalBodyIndex = BodyCatalog.FindBodyIndex("AspectAbilitiesMalachiteUrchinOrbitalBody");
			}

			ItemTierDef itemTierDef = ItemTierCatalog.FindTierDef("VoidLunarTierDef");
			if (itemTierDef)
			{
				lunarVoidTier = itemTierDef.tier;
			}

			SetupCompat();

			if (EffectHooks.preventedDefaultOverloadingBomb)
			{
				if (!(Compat.EliteReworks.affixBlueEnabled && Compat.EliteReworks.affixBlueOnHit))
				{
					EffectHooks.useCustomOverloadBombs = true;
				}
			}
			if (EffectHooks.preventedDefaultVoidCollapse)
			{
				if (!Compat.EliteReworks.eliteVoidEnabled)
				{
					EffectHooks.useCustomFracture = true;
				}
			}

			SetupIntermediate();

			Logger.Info("Catalog Setup Complete");

			setupComplete = true;
		}

		private static void SetupCompat()
		{
			if (setupCompat)
			{
				Logger.Warn("Catalog SetupCompat Called Again!");
				return;
			}

			if (PluginLoaded("com.Moffein.EliteReworks") && Configuration.EliteReworksHooks.Value) Compat.EliteReworks.LateSetup();

			if (PluginLoaded("com.Moffein.BlightedElites") && Configuration.BlightedHooks.Value) Compat.Blighted.LateSetup();

			setupCompat = true;
		}

		private static void SetupIntermediate()
		{
			if (setupIntermediate)
			{
				Logger.Warn("Catalog SetupIntermediate Called Again!");
				return;
			}

			DLC1Content.Buffs.BearVoidCooldown.canStack = true;
			EffectHooks.ApplyVoidBearCooldownFix();

			RiskOfRain.Init();
			SpikeStrip.Init();
			GoldenCoastPlus.Init();
			Aetherium.Init();
			Bubbet.Init();
			WarWisp.Init();
			Blighted.Init();
			GOTCE.Init();
			Thalasso.Init();
			RisingTides.Init();

			Language.ChangeText();

			RuleCatalogExcludeItemChoices();

			setupIntermediate = true;
		}

		private static void RuleCatalogExcludeItemChoices()
		{
			foreach (RuleDef ruleDef in RuleCatalog.allRuleDefs)
			{
				if (ruleDef.choices != null && ruleDef.choices.Count > 0)
				{
					ItemIndex itemIndex = ruleDef.choices[0].itemIndex;

					if (itemIndex != ItemIndex.None)
					{
						if (disabledItemIndexes.Contains(itemIndex) || (Configuration.AspectWorldUnique.Value && aspectItemIndexes.Contains(itemIndex)))
						{
							foreach (RuleChoiceDef ruleDefChoice in ruleDef.choices)
							{
								ruleDefChoice.excludeByDefault = true;
							}

							Logger.Info("Catalog - Hiding RuleCatalog Entry For : " + ItemCatalog.GetItemDef(itemIndex).name);
						}
					}
				}
			}
		}

		private static void OnMenuEnteredFirstTime()
		{
			if (menuVisited) return;

			if (!setupComplete)
			{
				Logger.Warn("OnMenuEnteredFirstTime - Logbook Initialization Failed!");
				Logger.Warn("Attempting Catalog Setup Fallback");

				try
				{
					SetupCatalog();
				}
				catch (Exception ex)
				{
					Logger.Warn("Failed To Setup Catalog!");
					Logger.Error(ex);
				}
			}

			bool obtainEquip = DropHooks.CanObtainEquipment();
			Logger.Info("EquipObtainable : " + obtainEquip);

			bool obtainItems = DropHooks.CanObtainItem();
			Logger.Info("ItemObtainable : " + obtainItems);

			bool convertEquip = Configuration.AspectEquipmentConversion.Value;
			bool absorbEquip = Configuration.AspectEquipmentAbsorb.Value;
			string msg = "EquipConvert : " + (convertEquip || absorbEquip);
			if (convertEquip) msg += " [Click]";
			if (absorbEquip) msg += " [Absorb]";
			Logger.Info(msg);

			bool dropDefault = Configuration.AspectEliteEquipment.Value;
			bool dropAbility = aspectAbilities && Configuration.AspectAbilitiesEliteEquipment.Value;
			msg = "DropAsEquipment : " + (dropDefault || dropAbility);
			if (dropDefault) msg += " [Default]";
			if (dropAbility) msg += " [AspectAbilities]";
			Logger.Info(msg);

			Logger.Info("ItemWorldUnique : " + Configuration.AspectWorldUnique.Value);

			FinalizeEntryStates();

			Logger.Info("FirstMenuVisit - Finalized Catalog Entries");

			menuVisited = true;
		}

		// set items to their actual tier if not disabled and sets equipment canDrop to false
		private static void FinalizeEntryStates()
		{
			RiskOfRain.ItemEntries(true);
			RiskOfRain.EquipmentEntries(false);

			if (SpikeStrip.populated)
			{
				SpikeStrip.ItemEntries(true);
				SpikeStrip.EquipmentEntries(false);
			}

			if (GoldenCoastPlus.populated)
			{
				GoldenCoastPlus.ItemEntries(true);
				GoldenCoastPlus.EquipmentEntries(false);
			}
			
			if (Aetherium.populated)
			{
				Aetherium.ItemEntries(true);
				Aetherium.EquipmentEntries(false);
			}

			if (Bubbet.populated)
			{
				Bubbet.ItemEntries(true);
				Bubbet.EquipmentEntries(false);
			}

			if (WarWisp.populated)
			{
				WarWisp.ItemEntries(true);
				WarWisp.EquipmentEntries(false);
			}

			if (Blighted.populated)
			{
				Blighted.ItemEntries(true);
				Blighted.EquipmentEntries(false);
			}

			if (GOTCE.populated)
			{
				GOTCE.ItemEntries(true);
				GOTCE.EquipmentEntries(false);
			}

			if (Thalasso.populated)
			{
				Thalasso.ItemEntries(true);
				Thalasso.EquipmentEntries(false);
			}

			if (RisingTides.populated)
			{
				RisingTides.ItemEntries(true);
				RisingTides.EquipmentEntries(false);
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
				Logger.Warn("Tried to disable " + itemDef.name + " without any BuffDefs or EquipmentDefs populated!");
			}

			if (disabledItemIndexes.Contains(itemDef.itemIndex))
			{
				if (equipDef != null) equipDef = null;
				if (buffDeff != null) buffDeff = null;
			}
		}

		private static void DeactivateItem(ItemDef itemDef, EquipmentDef equipDef)
		{
			if (disabledItemIndexes.Contains(itemDef.itemIndex)) return;

			if (!equipDef)
			{
				Logger.Warn(itemDef.name + " : associated equipment not found!");
				DeactivateItem(itemDef);
			}
		}

		private static void DeactivateItem(ItemDef itemDef, BuffDef buffDeff)
		{
			if (disabledItemIndexes.Contains(itemDef.itemIndex)) return;

			if (!buffDeff)
			{
				Logger.Warn(itemDef.name + " : associated buff not found!");
				DeactivateItem(itemDef);
			}
		}

		private static void DeactivateItem(ItemDef itemDef)
		{
			if (disabledItemIndexes.Contains(itemDef.itemIndex)) return;

			Logger.Warn("Deactivating : " + itemDef.name);

			if (itemDef._itemTierDef == BossItemTier)
			{
				if (ItemCatalog.tier3ItemList.Contains(itemDef.itemIndex))
				{
					ItemCatalog.tier3ItemList.Remove(itemDef.itemIndex);
				}
			}

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

			disabledItemIndexes.Add(itemDef.itemIndex);
		}



		private static void SetItemState(ItemDef itemDef, bool shown)
		{
			if (itemDef)
			{
				if (!aspectItemIndexes.Contains(itemDef.itemIndex))
				{
					aspectItemIndexes.Add(itemDef.itemIndex);
				}

				if (!itemDef.hidden)
				{
					if (itemDef == Item.ZetAspectVoid && Configuration.AspectVoidContagiousItem.Value)
					{
						if (!shown) itemDef.tier = ItemTier.NoTier;
						else itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.VoidTier3 : ItemTier.VoidBoss;
					}
					else
					{
						if (!shown) itemDef.tier = ItemTier.NoTier;
						else itemDef._itemTierDef = Configuration.AspectRedTier.Value ? RedItemTier : BossItemTier;
					}
				}
			}
		}

		private static void CopyExpansion(ItemDef itemDef, EquipmentDef equipDef)
		{
			if (!itemDef) return;

			if (!equipDef)
			{
				Logger.Warn("Could not copy expansion requirement for " + itemDef.name + " because its associated equipment was not found!");
				return;
			}

			itemDef.requiredExpansion = equipDef.requiredExpansion;
		}

		public static void CopyEquipmentPrefab(ItemDef itemDef, EquipmentDef equipDef)
		{
			if (!itemDef) return;

			if (!equipDef)
			{
				Logger.Warn("Could not copy model prefab for " + itemDef.name + " because its associated equipment was not found!");
				return;
			}

			itemDef.pickupModelPrefab = equipDef.pickupModelPrefab;
			PickupDef pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(itemDef.itemIndex));
			pickupDef.displayPrefab = equipDef.pickupModelPrefab;
		}

		public static void CopyItemPrefab(ItemDef itemDef, EquipmentDef equipDef)
		{
			equipDef.pickupModelPrefab = itemDef.pickupModelPrefab;
			PickupDef pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(equipDef.equipmentIndex));
			pickupDef.displayPrefab = itemDef.pickupModelPrefab;
		}

		public static void ReplaceEquipmentIcon(EquipmentDef equipDef, Sprite baseSprite, Sprite outlineSprite)
		{
			if (equipDef)
			{
				equipDef.pickupIconSprite = CreateAspectSprite(baseSprite, outlineSprite);

				PickupDef pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(equipDef.equipmentIndex));
				pickupDef.iconSprite = equipDef.pickupIconSprite;
				pickupDef.iconTexture = equipDef.pickupIconSprite.texture;
			}
		}

		private static void SetEquipmentState(EquipmentDef equipDef, bool canDrop)
		{
			if (equipDef)
			{
				if (!aspectEquipIndexes.Contains(equipDef.equipmentIndex))
				{
					aspectEquipIndexes.Add(equipDef.equipmentIndex);
				}

				equipDef.canDrop = canDrop;
			}
		}

		private static void ColorEquipmentDroplet(EquipmentDef equipDef)
		{
			if (equipDef)
			{
				equipDef.isBoss = true;
				equipDef.colorIndex = ColorCatalog.ColorIndex.Artifact;

				PickupDef pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(equipDef.equipmentIndex));

				pickupDef.isBoss = true;
				if (BossDropletPrefab) pickupDef.dropletDisplayPrefab = BossDropletPrefab;
				pickupDef.baseColor = new Color(0.9f, 0.7f, 0.75f);
				pickupDef.darkColor = new Color(0.9f, 0.7f, 0.75f);
				//pickupDef.darkColor = new Color(0.5f, 0.385f, 0.425f);
			}
		}



		private static void CreateEquality(EquipmentDef equipDef, BuffDef buffDef, ItemDef itemDef)
		{
			if (equipDef && buffDef)
			{
				if (!aspectBuffIndexes.Contains(buffDef.buffIndex))
				{
					aspectBuffIndexes.Add(buffDef.buffIndex);
				}



				if (!buffToItem.ContainsKey(buffDef.buffIndex))
				{
					buffToItem.Add(buffDef.buffIndex, itemDef.itemIndex);
				}
				else
				{
					Logger.Warn("buffToItem already contains key for : " + buffDef.name);
				}

				if (!buffToEquip.ContainsKey(buffDef.buffIndex))
				{
					buffToEquip.Add(buffDef.buffIndex, equipDef.equipmentIndex);
				}
				else
				{
					Logger.Warn("buffToEquip already contains key for : " + buffDef.name);
				}

				if (!equipToItem.ContainsKey(equipDef.equipmentIndex))
				{
					equipToItem.Add(equipDef.equipmentIndex, itemDef.itemIndex);
				}
				else
				{
					Logger.Warn("equipToItem already contains key for : " + equipDef.name);
				}
			}
		}



		public static bool PluginLoaded(string key)
		{
			return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(key);
		}
	}
}
