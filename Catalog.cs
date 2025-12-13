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

		public static BindingFlags Flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance;



		public static Dictionary<BuffIndex, ItemIndex> buffToItem = new Dictionary<BuffIndex, ItemIndex>();
		public static Dictionary<BuffIndex, EquipmentIndex> buffToEquip = new Dictionary<BuffIndex, EquipmentIndex>();

		public static Dictionary<EquipmentIndex, ItemIndex> equipToItem = new Dictionary<EquipmentIndex, ItemIndex>();

		public static List<ItemIndex> disabledItemIndexes = new List<ItemIndex>();

		public static List<ItemIndex> aspectItemIndexes = new List<ItemIndex>();
		public static List<BuffIndex> aspectBuffIndexes = new List<BuffIndex>();
		public static List<EquipmentIndex> aspectEquipIndexes = new List<EquipmentIndex>();

		public static List<ItemDef> transformableAspectItemDefs = new List<ItemDef>();

		public static List<AspectPack> aspectPacks = new List<AspectPack>();
		public static List<AspectDef> aspectDefs = new List<AspectDef>();

		public static Dictionary<BuffIndex, AspectDef> buffToAspect = new Dictionary<BuffIndex, AspectDef>();

		public static List<AspectDef> aspectsWithPromoters = new List<AspectDef>();



		internal static ItemTierDef BossItemTier;
		internal static ItemTierDef RedItemTier;

		internal static GameObject WhiteAspectPrefab;

		internal static GameObject BossDropletPrefab;
		internal static GameObject LightningStakePrefab;
		internal static GameObject RejectTextPrefab;
		internal static GameObject CommandCubePrefab;
		internal static GameObject SmokeBombMiniPrefab;



		public static int barrierDecayMode = 0;

		public static bool lateHooksDone = false;
		public static bool appliedVoidBearFix = false;
		public static bool limitChillStacks = false;
		public static bool borboFrostBlade = false;
		public static bool shieldJump = false;
		public static bool aspectAbilities = false;
		public static bool immuneHealth = false;
		public static bool altIceActive = false;
		public static bool nemBarrier = false;


		public static bool ChillCanStack => RoR2Content.Buffs.Slow80.canStack;



		public static bool dropWeightsAvailable = false;



		public static class Sprites
		{
			public static Sprite OutlineStandard;
			public static Sprite OutlineVoid;
			public static Sprite OutlineNull;
			public static Sprite OutlineCracked;
			/*
			public static Sprite OutlineRed;
			public static Sprite OutlineOrange;
			public static Sprite OutlineYellow;
			public static Sprite OutlineBlue;

			public static Sprite NullOutlineRed;
			public static Sprite NullOutlineOrange;
			public static Sprite NullOutlineYellow;

			public static Sprite CrackedOutlineRed;
			public static Sprite CrackedOutlineOrange;
			public static Sprite CrackedOutlineYellow;
			*/
			public static Sprite AffixUnknown;

			public static Sprite AffixWhite;
			public static Sprite AffixBlue;
			public static Sprite AffixRed;
			public static Sprite AffixHaunted;
			public static Sprite AffixPoison;
			public static Sprite AffixLunar;

			public static Sprite AffixEarth;
			public static Sprite AffixVoid;

			public static Sprite AffixAurelionite;
			public static Sprite AffixBead;

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

			public static Sprite AffixBuffered;
			public static Sprite AffixOppressive;

			public static Sprite AffixBuffing;
			public static Sprite AffixFrenzied;
			public static Sprite AffixVolatile;
			public static Sprite AffixEcho;

			public static Sprite AffixArmored;
			public static Sprite AffixBuffing_EV;
			public static Sprite AffixImpPlane;
			public static Sprite AffixPillaging;
			public static Sprite AffixSandstorm;
			public static Sprite AffixTinkerer;

			public static Sprite AffixAdaptive;

			public static Sprite AffixMotivator;
			public static Sprite AffixOsmium;

			public static Sprite AffixEmpyrean;

			public static Sprite HauntCloak;
			public static Sprite ZetHeadHunter;
			public static Sprite ZetSapped;
			public static Sprite ZetShredded;
			public static Sprite ZetPoached;
			public static Sprite ZetSepiaBlind;
			public static Sprite ZetElusive;
			public static Sprite ZetWarped;



			public static void Load()
			{
				OutlineStandard = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineWhite.png");
				OutlineVoid = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineVoid.png");
				/*
				OutlineRed = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineRed.png");
				OutlineOrange = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineOrange.png");
				OutlineYellow = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineYellow.png");
				OutlineBlue = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineBlue.png");
				OutlineVoid = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineVoid.png");
				OutlineWhite = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineWhite.png");
				*/
				AffixUnknown = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixUnknown.png");

				AffixWhite = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixWhite.png");
				AffixBlue = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBlue.png");
				AffixRed = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixRed.png");
				AffixHaunted = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixHaunted.png");
				AffixPoison = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPoison.png");
				AffixLunar = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixLunar.png");

				AffixEarth = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixEarth.png");
				AffixVoid = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixVoid.png");

				AffixAurelionite = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixAurelionite.png");
				AffixBead = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBead.png");

				if (AspectPackDefOf.SpikeStrip.Enabled)
				{
					AffixPlated = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPlated.png");
					AffixWarped = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixWarped.png");
					AffixVeiled = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixVeiled.png");
					AffixAragonite = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixAragonite.png");
					ZetElusive = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffElusive.png");
					ZetWarped = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Slow80").iconSprite;
				}
				
				if (AspectPackDefOf.GoldenCoastPlus.Enabled)
				{
					AffixGold = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPillaging.png");
				}

				if (AspectPackDefOf.Aetherium.Enabled)
				{
					AffixSanguine = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixSanguine.png");
				}

				if (AspectPackDefOf.Bubbet.Enabled)
				{
					AffixSepia = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixSepia.png");
					SepiaElite = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffAffixSepia.png");
					ZetSepiaBlind = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffSepiaBlind.png");
				}
				
				if (AspectPackDefOf.WarWisp.Enabled)
				{
					OutlineNull = Assets.LoadAsset<Sprite>("Assets/Icons/texNullOutlineWhite.png");
					/*
					NullOutlineRed = Assets.LoadAsset<Sprite>("Assets/Icons/texNullOutlineRed.png");
					NullOutlineOrange = Assets.LoadAsset<Sprite>("Assets/Icons/texNullOutlineOrange.png");
					NullOutlineYellow = Assets.LoadAsset<Sprite>("Assets/Icons/texNullOutlineYellow.png");
					*/
					AffixNullifier = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixNullifier.png");
				}

				AffixBlighted = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBlighted.png");

				if (AspectPackDefOf.GOTCE.Enabled)
				{
					OutlineCracked = Assets.LoadAsset<Sprite>("Assets/Icons/texCrackedOutlineWhite.png");
					/*
					CrackedOutlineRed = Assets.LoadAsset<Sprite>("Assets/Icons/texCrackedOutlineRed.png");
					CrackedOutlineOrange = Assets.LoadAsset<Sprite>("Assets/Icons/texCrackedOutlineOrange.png");
					CrackedOutlineYellow = Assets.LoadAsset<Sprite>("Assets/Icons/texCrackedOutlineYellow.png");
					*/
					AffixBackup = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBackup.png");
					BackupDebuff = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffNoSecondary.png");
				}

				if (AspectPackDefOf.Thalasso.Enabled)
				{
					AffixPurity = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPurity.png");
				}

				if (AspectPackDefOf.RisingTides.Enabled)
				{
					AffixBarrier = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBarrier.png");
					AffixBlackHole = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBlackHole.png");
					AffixMoney = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixArmored.png");
					AffixNight = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixNight.png");
					AffixWater = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixWater.png");
					AffixRealgar = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixImpPlane.png");
				}

				AffixBuffered = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBuffered.png");
				AffixOppressive = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixOppressive.png");

				if (AspectPackDefOf.MoreElites.Enabled)
				{
					AffixBuffing = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBuffing.png");
					AffixFrenzied = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixFrenzied.png");
					AffixVolatile = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixVolatile.png");
					AffixEcho = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixEcho.png");
				}

				if (AspectPackDefOf.EliteVariety.Enabled)
				{
					AffixArmored = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixArmored.png");
					AffixBuffing_EV = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBuffing.png");
					AffixImpPlane = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixImpPlane.png");
					AffixPillaging = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPillaging.png");
					AffixSandstorm = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixSandstorm.png");
					AffixTinkerer = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixTinkerer.png");
				}

				if (AspectPackDefOf.Augmentum.Enabled)
				{
					AffixAdaptive = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixAdaptive.png");
				}

				if (AspectPackDefOf.Sandswept.Enabled)
				{
					AffixMotivator = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixMotivator.png");
					AffixOsmium = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixOsmium.png");
				}

				AffixEmpyrean = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixEmpyrean.png");



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
			public static GameObject AffixGold;



			public static void Load()
			{
				AffixVoid = Assets.LoadAsset<GameObject>("Assets/Prefabs/prefabAffixVoid.prefab");
				AffixVoid.AddComponent<ModelPanelParameters>();

				if (AspectPackDefOf.Bubbet.Enabled)
				{
					AffixSepia = Assets.LoadAsset<GameObject>("Assets/Prefabs/prefabAffixSepia.prefab");
					AffixSepia.AddComponent<ModelPanelParameters>();
				}

				if (AspectPackDefOf.Thalasso.Enabled)
				{
					AffixPure = Assets.LoadAsset<GameObject>("Assets/Prefabs/prefabAffixPure.prefab");
					AffixPure.AddComponent<ModelPanelParameters>();
				}

				if (AspectPackDefOf.GoldenCoastPlus.Enabled)
				{
					AffixGold = Assets.LoadAsset<GameObject>("Assets/Prefabs/prefabAffixGold.prefab");
					AffixGold.AddComponent<ModelPanelParameters>();
				}
			}
		}

		public static Sprite CreateAspectSprite(Sprite baseSprite, Sprite outlineSprite)
		{
			return CreateAspectSprite(baseSprite, outlineSprite, new Color32(255, 255, 255, 255));
		}

		public static Sprite CreateAspectSprite(Sprite baseSprite, Sprite outlineSprite, Color32 outlineColor)
		{
			Color32[] basePixels = baseSprite.texture.GetPixels32();
			Color32[] outlinePixels = outlineSprite.texture.GetPixels32();

			// non-transparent outlinePixels overwrite basePixels
			for (var i = 0; i < outlinePixels.Length; ++i)
			{
				if (outlinePixels[i].a > 11)
				{
					outlinePixels[i].r = (byte)((outlinePixels[i].r * outlineColor.r) / 255);
					outlinePixels[i].g = (byte)((outlinePixels[i].g * outlineColor.g) / 255);
					outlinePixels[i].b = (byte)((outlinePixels[i].b * outlineColor.b) / 255);
					outlinePixels[i].a = (byte)((outlinePixels[i].a * outlineColor.a) / 255);

					basePixels[i] = outlinePixels[i];
				}
			}

			Texture2D newTexture = new Texture2D(128, 128, TextureFormat.RGBA32, false);

			newTexture.SetPixels32(basePixels);
			newTexture.Apply();

			return Sprite.Create(newTexture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f), 25.0f);
		}

		public static Sprite TryUseSprite(Sprite sprite)
		{
			if (sprite.texture.isReadable) return sprite;

			Texture2D newTexture = DuplicateTexture(sprite.texture);
			return Sprite.Create(newTexture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f), 25.0f);
		}

		public static Texture2D DuplicateTexture(Texture2D source)
		{
			RenderTexture renderTex = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
			Graphics.Blit(source, renderTex);

			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = renderTex;



			Texture2D readableText = new Texture2D(source.width, source.height);

			readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
			readableText.Apply();

			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(renderTex);

			return readableText;
		}



		public static EffectDef RejectTextDef;

		public static ArtifactIndex diluvianArtifactIndex = ArtifactIndex.None;

		public static BodyIndex mithrixBodyIndex = BodyIndex.None;
		public static BodyIndex voidlingBodyIndex = BodyIndex.None;
		//public static BodyIndex urchinTurretBodyIndex = BodyIndex.None;
		//public static BodyIndex urchinOrbitalBodyIndex = BodyIndex.None;
		//public static BodyIndex healOrbBodyIndex = BodyIndex.None;
		public static BodyIndex artifactShellBodyIndex = BodyIndex.None;
		public static BodyIndex goldenTitanBodyIndex = BodyIndex.None;
		public static BodyIndex tinkerDroneBodyIndex = BodyIndex.None;

		public static BuffIndex altSlow80 = BuffIndex.None;
		public static BuffIndex antiGrav = BuffIndex.None;
		public static BuffIndex rageAura = BuffIndex.None;
		public static BuffIndex veiledBuffer = BuffIndex.None;
		public static BuffIndex veiledCooldown = BuffIndex.None;
		public static BuffIndex nullifierRecipient = BuffIndex.None;
		public static BuffIndex waterInvuln = BuffIndex.None;
		public static BuffIndex reactorInvuln = BuffIndex.None;
		public static BuffIndex lampBuff = BuffIndex.None;

		public static ItemIndex summonedEcho = ItemIndex.None;

		public static ItemTier lunarVoidTier = ItemTier.AssignedAtRuntime;
		public static ItemTier highlanderTier = ItemTier.AssignedAtRuntime;

		public static DeployableSlot tinkerDeploySlot = DeployableSlot.EngiMine;
		public static DotController.DotIndex impaleDotIndex = DotController.DotIndex.None;



		private static ItemTierDef _HighlanderTier;
		public static ItemTierDef HighlanderItemTier
		{
			get
			{
				if (_HighlanderTier == null)
				{
					if (highlanderTier == ItemTier.AssignedAtRuntime)
					{
						Logger.Warn("Checking ItemTierCatalog for : Highlander");
						ItemTierDef itemTierDef = ItemTierCatalog.FindTierDef("Highlander");
						if (itemTierDef)
						{
							Logger.Warn("ItemTierDef Highlander Found!");
							highlanderTier = itemTierDef.tier;
						}
					}

					_HighlanderTier = ItemTierCatalog.GetItemTierDef(highlanderTier);
				}

				return _HighlanderTier;
			}
		}

		private static int _HighlanderState = -1;
		public static bool AsHighlander
		{
			get
			{
				if (_HighlanderState == -1)
				{
					Logger.Warn("Checking for mod : ZetAspectHighlander");
					if (PluginLoaded("prodzpod.ZetAspectHighlander"))
					{
						_HighlanderState = 1;
						Logger.Warn("ZetAspectHighlander Found!");
					}
					else
					{
						_HighlanderState = 0;
					}
				}

				return _HighlanderState == 1;
			}
		}



		public static ItemTierDef AspectItemTier
		{
			get
			{
				return Configuration.AspectRedTier.Value ? RedItemTier : BossItemTier;
			}
		}

		public static bool AspectVoidContagious
		{
			get
			{
				return Configuration.AspectVoidContagiousItem.Value;
			}
		}



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

		public static AspectDef GetAspectDef(BuffIndex buffIndex)
		{
			if (buffToAspect.ContainsKey(buffIndex)) return buffToAspect[buffIndex];
			return null;
		}



		public static float GetStackMagnitude(CharacterBody self, BuffDef buffDef)
		{
			float aspect = 0f;

			Inventory inventory = self.inventory;
			if (inventory)
			{
				aspect = CountAspectEquipment(inventory, buffDef);

				if (aspect > 0f && self.isPlayerControlled)
				{
					aspect *= Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);
				}

				aspect += inventory.GetItemCountEffective(GetAspectItemIndex(buffDef.buffIndex));
			}

			foreach (IAspectProvider provider in EliteBuffManager.Providers)
			{
				float provStack = provider.StackCount(self);

				if (provStack > 0f && provider.HasAspect(self, buffDef.buffIndex))
				{
					aspect += provStack;
				}
			}

			return Mathf.Max(1f, aspect);
		}



		public static bool HasAspectFromProviders(CharacterBody body, BuffDef buffDef)
		{
			foreach (IAspectProvider provider in EliteBuffManager.Providers)
			{
				if (provider.HasAspect(body, buffDef.buffIndex)) return true;
			}

			return false;
		}

		public static bool HasAspectFromProviders(CharacterBody body, BuffIndex buffIndex)
		{
			foreach (IAspectProvider provider in EliteBuffManager.Providers)
			{
				if (provider.HasAspect(body, buffIndex)) return true;
			}

			return false;
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
			if (itemDef && inventory.GetItemCountEffective(itemDef) > 0) return true;

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

			if (inventory.GetItemCountEffective(itemIndex) > 0) return true;

			return false;
		}

		public static EliteDef GetEquipmentEliteDef(EquipmentDef equipDef)
		{
			if (equipDef == null) return null;
			if (equipDef.passiveBuffDef == null) return null;

			return equipDef.passiveBuffDef.eliteDef;
		}



		public static bool BodyAllowedAffix(CharacterBody body, AspectDef aspectDef)
		{
			BodyIndex bodyIndex = body.bodyIndex;

			if (aspectDef.blacklistedIndexes.Count > 0)
			{
				foreach (BodyIndex forbidIndex in aspectDef.blacklistedIndexes)
				{
					if (bodyIndex == forbidIndex) return false;
				}
			}

			if (aspectDef.AllowAffix != null)
			{
				return aspectDef.AllowAffix(body, body.inventory);
			}

			return true;
		}

		public static bool BodyAllowedAffix(CharacterBody body, BuffDef buffDef)
		{
			AspectDef aspectDef = GetAspectDef(buffDef.buffIndex);

			if (aspectDef != null)
			{
				return BodyAllowedAffix(body, aspectDef);
			}

			return false;
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

			WhiteAspectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");

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
			BuffDefOf.ZetHeadHunter = ZetHeadHunter;
			ZetAspectsContent.buffDefs.Add(ZetHeadHunter);

			BuffDef ZetSapped = ScriptableObject.CreateInstance<BuffDef>();
			ZetSapped.name = "ZetSapped";
			ZetSapped.buffColor = new Color(0.5f, 0.75f, 1f);
			ZetSapped.canStack = false;
			ZetSapped.isDebuff = true;
			ZetSapped.iconSprite = Sprites.ZetSapped;
			BuffDefOf.ZetSapped = ZetSapped;
			ZetAspectsContent.buffDefs.Add(ZetSapped);

			BuffDef ZetShredded = ScriptableObject.CreateInstance<BuffDef>();
			ZetShredded.name = "ZetShredded";
			ZetShredded.buffColor = new Color(0.185f, 0.75f, 0.465f);
			ZetShredded.canStack = false;
			ZetShredded.isDebuff = true;
			ZetShredded.iconSprite = Sprites.ZetShredded;
			BuffDefOf.ZetShredded = ZetShredded;
			ZetAspectsContent.buffDefs.Add(ZetShredded);

			BuffDef ZetPoached = ScriptableObject.CreateInstance<BuffDef>();
			ZetPoached.name = "ZetPoached";
			ZetPoached.buffColor = new Color(0.5f, 0.75f, 0.185f);
			ZetPoached.canStack = false;
			ZetPoached.isDebuff = true;
			ZetPoached.iconSprite = Sprites.ZetPoached;
			BuffDefOf.ZetPoached = ZetPoached;
			ZetAspectsContent.buffDefs.Add(ZetPoached);

			if (AspectPackDefOf.Bubbet.Enabled)
			{
				BuffDef ZetSepiaBlind = ScriptableObject.CreateInstance<BuffDef>();
				ZetSepiaBlind.name = "ZetSepiaBlind";
				ZetSepiaBlind.buffColor = Color.white;
				ZetSepiaBlind.canStack = false;
				ZetSepiaBlind.isDebuff = true;
				ZetSepiaBlind.iconSprite = Sprites.ZetSepiaBlind;
				BuffDefOf.ZetSepiaBlind = ZetSepiaBlind;
				ZetAspectsContent.buffDefs.Add(ZetSepiaBlind);
			}

			if (AspectPackDefOf.SpikeStrip.Enabled)
			{
				BuffDef ZetElusive = ScriptableObject.CreateInstance<BuffDef>();
				ZetElusive.name = "ZetElusive";
				ZetElusive.buffColor = new Color(0.185f, 0.465f, 0.75f);
				ZetElusive.canStack = true;
				ZetElusive.isDebuff = false;
				ZetElusive.iconSprite = Sprites.ZetElusive;
				ZetElusive.stackingDisplayMethod = BuffDef.StackingDisplayMethod.Percentage;
				BuffDefOf.ZetElusive = ZetElusive;
				ZetAspectsContent.buffDefs.Add(ZetElusive);

				BuffDef ZetWarped = ScriptableObject.CreateInstance<BuffDef>();
				ZetWarped.name = "ZetWarped";
				ZetWarped.buffColor = new Color(0.65f, 0.5f, 0.75f);
				ZetWarped.canStack = false;
				ZetWarped.isDebuff = true;
				ZetWarped.iconSprite = Sprites.ZetWarped;
				BuffDefOf.ZetWarped = ZetWarped;
				ZetAspectsContent.buffDefs.Add(ZetWarped);
			}

			if (AspectPackDefOf.MoreElites.Enabled)
			{
				BuffDef ZetEchoPrimer = ScriptableObject.CreateInstance<BuffDef>();
				ZetEchoPrimer.name = "ZetEchoPrimer";
				ZetEchoPrimer.buffColor = Color.white;
				ZetEchoPrimer.canStack = false;
				ZetEchoPrimer.isDebuff = false;
				ZetEchoPrimer.isHidden = true;
				ZetEchoPrimer.iconSprite = Sprites.ZetElusive;
				BuffDefOf.ZetEchoPrimer = ZetEchoPrimer;
				ZetAspectsContent.buffDefs.Add(ZetEchoPrimer);
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
			ItemDefOf.ZetAspectsDropCountTracker = ZetAspectsDropCountTracker;
			ZetAspectsContent.itemDefs.Add(ZetAspectsDropCountTracker);

			ItemDef ZetAspectsUpdateInventory = ScriptableObject.CreateInstance<ItemDef>();
			ZetAspectsUpdateInventory.name = "ZetAspectsUpdateInventory";
			AssignDepricatedTier(ZetAspectsUpdateInventory, ItemTier.NoTier);
			ZetAspectsUpdateInventory.AutoPopulateTokens();
			ZetAspectsUpdateInventory.hidden = true;
			ZetAspectsUpdateInventory.canRemove = false;
			ItemDefOf.ZetAspectsUpdateInventory = ZetAspectsUpdateInventory;
			ZetAspectsContent.itemDefs.Add(ZetAspectsUpdateInventory);

			ItemDef ZetAspectWhite = Items.ZetAspectWhite.DefineItem();
			ItemDefOf.ZetAspectWhite = ZetAspectWhite;
			ZetAspectsContent.itemDefs.Add(ZetAspectWhite);
			transformableAspectItemDefs.Add(ZetAspectWhite);

			ItemDef ZetAspectBlue = Items.ZetAspectBlue.DefineItem();
			ItemDefOf.ZetAspectBlue = ZetAspectBlue;
			ZetAspectsContent.itemDefs.Add(ZetAspectBlue);
			transformableAspectItemDefs.Add(ZetAspectBlue);

			ItemDef ZetAspectRed = Items.ZetAspectRed.DefineItem();
			ItemDefOf.ZetAspectRed = ZetAspectRed;
			ZetAspectsContent.itemDefs.Add(ZetAspectRed);
			transformableAspectItemDefs.Add(ZetAspectRed);

			ItemDef ZetAspectHaunted = Items.ZetAspectHaunted.DefineItem();
			ItemDefOf.ZetAspectHaunted = ZetAspectHaunted;
			ZetAspectsContent.itemDefs.Add(ZetAspectHaunted);
			transformableAspectItemDefs.Add(ZetAspectHaunted);

			ItemDef ZetAspectPoison = Items.ZetAspectPoison.DefineItem();
			ItemDefOf.ZetAspectPoison = ZetAspectPoison;
			ZetAspectsContent.itemDefs.Add(ZetAspectPoison);
			transformableAspectItemDefs.Add(ZetAspectPoison);

			ItemDef ZetAspectLunar = Items.ZetAspectLunar.DefineItem();
			ItemDefOf.ZetAspectLunar = ZetAspectLunar;
			ZetAspectsContent.itemDefs.Add(ZetAspectLunar);
			transformableAspectItemDefs.Add(ZetAspectLunar);

			ItemDef ZetAspectEarth = Items.ZetAspectEarth.DefineItem();
			ItemDefOf.ZetAspectEarth = ZetAspectEarth;
			ZetAspectsContent.itemDefs.Add(ZetAspectEarth);
			transformableAspectItemDefs.Add(ZetAspectEarth);

			ItemDef ZetAspectVoid = Items.ZetAspectVoid.DefineItem();
			ItemDefOf.ZetAspectVoid = ZetAspectVoid;
			ZetAspectsContent.itemDefs.Add(ZetAspectVoid);

			ItemDef ZetAspectAurelionite = Items.ZetAspectAurelionite.DefineItem();
			ItemDefOf.ZetAspectAurelionite = ZetAspectAurelionite;
			ZetAspectsContent.itemDefs.Add(ZetAspectAurelionite);
			transformableAspectItemDefs.Add(ZetAspectAurelionite);

			ItemDef ZetAspectBead = Items.ZetAspectBead.DefineItem();
			ItemDefOf.ZetAspectBead = ZetAspectBead;
			ZetAspectsContent.itemDefs.Add(ZetAspectBead);
			transformableAspectItemDefs.Add(ZetAspectBead);

			if (AspectPackDefOf.SpikeStrip.Enabled)
			{
				ItemDef ZetAspectPlated = Items.ZetAspectPlated.DefineItem();
				ItemDefOf.ZetAspectPlated = ZetAspectPlated;
				ZetAspectsContent.itemDefs.Add(ZetAspectPlated);
				transformableAspectItemDefs.Add(ZetAspectPlated);

				ItemDef ZetAspectWarped = Items.ZetAspectWarped.DefineItem();
				ItemDefOf.ZetAspectWarped = ZetAspectWarped;
				ZetAspectsContent.itemDefs.Add(ZetAspectWarped);
				transformableAspectItemDefs.Add(ZetAspectWarped);

				ItemDef ZetAspectVeiled = Items.ZetAspectVeiled.DefineItem();
				ItemDefOf.ZetAspectVeiled = ZetAspectVeiled;
				ZetAspectsContent.itemDefs.Add(ZetAspectVeiled);
				transformableAspectItemDefs.Add(ZetAspectVeiled);

				ItemDef ZetAspectAragonite = Items.ZetAspectAragonite.DefineItem();
				ItemDefOf.ZetAspectAragonite = ZetAspectAragonite;
				ZetAspectsContent.itemDefs.Add(ZetAspectAragonite);
				transformableAspectItemDefs.Add(ZetAspectAragonite);
			}

			if (AspectPackDefOf.GoldenCoastPlus.Enabled)
			{
				ItemDef ZetAspectGold = Items.ZetAspectGold.DefineItem();
				ItemDefOf.ZetAspectGold = ZetAspectGold;
				ZetAspectsContent.itemDefs.Add(ZetAspectGold);
				transformableAspectItemDefs.Add(ZetAspectGold);
			}
			
			if (AspectPackDefOf.Aetherium.Enabled)
			{
				ItemDef ZetAspectSanguine = Items.ZetAspectSanguine.DefineItem();
				ItemDefOf.ZetAspectSanguine = ZetAspectSanguine;
				ZetAspectsContent.itemDefs.Add(ZetAspectSanguine);
				transformableAspectItemDefs.Add(ZetAspectSanguine);
			}

			if (AspectPackDefOf.Bubbet.Enabled)
			{
				ItemDef ZetAspectSepia = Items.ZetAspectSepia.DefineItem();
				ItemDefOf.ZetAspectSepia = ZetAspectSepia;
				ZetAspectsContent.itemDefs.Add(ZetAspectSepia);
				transformableAspectItemDefs.Add(ZetAspectSepia);
			}

			if (AspectPackDefOf.WarWisp.Enabled)
			{
				ItemDef ZetAspectNullifier = Items.ZetAspectNullifier.DefineItem();
				ItemDefOf.ZetAspectNullifier = ZetAspectNullifier;
				ZetAspectsContent.itemDefs.Add(ZetAspectNullifier);
				transformableAspectItemDefs.Add(ZetAspectNullifier);
			}

			ItemDef ZetAspectBlighted = Items.ZetAspectBlighted.DefineItem();
			ItemDefOf.ZetAspectBlighted = ZetAspectBlighted;
			ZetAspectsContent.itemDefs.Add(ZetAspectBlighted);
			transformableAspectItemDefs.Add(ZetAspectBlighted);
			
			if (AspectPackDefOf.GOTCE.Enabled)
			{
				ItemDef ZetAspectBackup = Items.ZetAspectBackup.DefineItem();
				ItemDefOf.ZetAspectBackup = ZetAspectBackup;
				ZetAspectsContent.itemDefs.Add(ZetAspectBackup);
				transformableAspectItemDefs.Add(ZetAspectBackup);
			}

			if (AspectPackDefOf.Thalasso.Enabled)
			{
				ItemDef ZetAspectPurity = Items.ZetAspectPurity.DefineItem();
				ItemDefOf.ZetAspectPurity = ZetAspectPurity;
				ZetAspectsContent.itemDefs.Add(ZetAspectPurity);
				transformableAspectItemDefs.Add(ZetAspectPurity);
			}

			if (AspectPackDefOf.RisingTides.Enabled)
			{
				ItemDef ZetAspectBarrier = Items.ZetAspectBarrier.DefineItem();
				ItemDefOf.ZetAspectBarrier = ZetAspectBarrier;
				ZetAspectsContent.itemDefs.Add(ZetAspectBarrier);
				transformableAspectItemDefs.Add(ZetAspectBarrier);

				ItemDef ZetAspectBlackHole = Items.ZetAspectBlackHole.DefineItem();
				ItemDefOf.ZetAspectBlackHole = ZetAspectBlackHole;
				ZetAspectsContent.itemDefs.Add(ZetAspectBlackHole);
				transformableAspectItemDefs.Add(ZetAspectBlackHole);

				ItemDef ZetAspectMoney = Items.ZetAspectMoney.DefineItem();
				ItemDefOf.ZetAspectMoney = ZetAspectMoney;
				ZetAspectsContent.itemDefs.Add(ZetAspectMoney);
				transformableAspectItemDefs.Add(ZetAspectMoney);

				ItemDef ZetAspectNight = Items.ZetAspectNight.DefineItem();
				ItemDefOf.ZetAspectNight = ZetAspectNight;
				ZetAspectsContent.itemDefs.Add(ZetAspectNight);
				transformableAspectItemDefs.Add(ZetAspectNight);

				ItemDef ZetAspectWater = Items.ZetAspectWater.DefineItem();
				ItemDefOf.ZetAspectWater = ZetAspectWater;
				ZetAspectsContent.itemDefs.Add(ZetAspectWater);
				transformableAspectItemDefs.Add(ZetAspectWater);

				ItemDef ZetAspectRealgar = Items.ZetAspectRealgar.DefineItem();
				ItemDefOf.ZetAspectRealgar = ZetAspectRealgar;
				ZetAspectsContent.itemDefs.Add(ZetAspectRealgar);
				transformableAspectItemDefs.Add(ZetAspectRealgar);
			}

			ItemDef ZetAspectBuffered = Items.ZetAspectBuffered.DefineItem();
			ItemDefOf.ZetAspectBuffered = ZetAspectBuffered;
			ZetAspectsContent.itemDefs.Add(ZetAspectBuffered);
			transformableAspectItemDefs.Add(ZetAspectBuffered);

			ItemDef ZetAspectOppressive = Items.ZetAspectOppressive.DefineItem();
			ItemDefOf.ZetAspectOppressive = ZetAspectOppressive;
			ZetAspectsContent.itemDefs.Add(ZetAspectOppressive);
			transformableAspectItemDefs.Add(ZetAspectOppressive);

			if (AspectPackDefOf.MoreElites.Enabled)
			{
				ItemDef ZetAspectEmpowering = Items.ZetAspectEmpowering.DefineItem();
				ItemDefOf.ZetAspectEmpowering = ZetAspectEmpowering;
				ZetAspectsContent.itemDefs.Add(ZetAspectEmpowering);
				transformableAspectItemDefs.Add(ZetAspectEmpowering);

				ItemDef ZetAspectFrenzied = Items.ZetAspectFrenzied.DefineItem();
				ItemDefOf.ZetAspectFrenzied = ZetAspectFrenzied;
				ZetAspectsContent.itemDefs.Add(ZetAspectFrenzied);
				transformableAspectItemDefs.Add(ZetAspectFrenzied);

				ItemDef ZetAspectVolatile = Items.ZetAspectVolatile.DefineItem();
				ItemDefOf.ZetAspectVolatile = ZetAspectVolatile;
				ZetAspectsContent.itemDefs.Add(ZetAspectVolatile);
				transformableAspectItemDefs.Add(ZetAspectVolatile);

				ItemDef ZetAspectEcho = Items.ZetAspectEcho.DefineItem();
				ItemDefOf.ZetAspectEcho = ZetAspectEcho;
				ZetAspectsContent.itemDefs.Add(ZetAspectEcho);
				transformableAspectItemDefs.Add(ZetAspectEcho);
			}

			if (AspectPackDefOf.EliteVariety.Enabled)
			{
				ItemDef ZetAspectArmor = Items.ZetAspectArmor.DefineItem();
				ItemDefOf.ZetAspectArmor = ZetAspectArmor;
				ZetAspectsContent.itemDefs.Add(ZetAspectArmor);
				transformableAspectItemDefs.Add(ZetAspectArmor);

				ItemDef ZetAspectBanner = Items.ZetAspectBanner.DefineItem();
				ItemDefOf.ZetAspectBanner = ZetAspectBanner;
				ZetAspectsContent.itemDefs.Add(ZetAspectBanner);
				transformableAspectItemDefs.Add(ZetAspectBanner);

				ItemDef ZetAspectImpale = Items.ZetAspectImpale.DefineItem();
				ItemDefOf.ZetAspectImpale = ZetAspectImpale;
				ZetAspectsContent.itemDefs.Add(ZetAspectImpale);
				transformableAspectItemDefs.Add(ZetAspectImpale);

				ItemDef ZetAspectGolden = Items.ZetAspectGolden.DefineItem();
				ItemDefOf.ZetAspectGolden = ZetAspectGolden;
				ZetAspectsContent.itemDefs.Add(ZetAspectGolden);
				transformableAspectItemDefs.Add(ZetAspectGolden);

				ItemDef ZetAspectCyclone = Items.ZetAspectCyclone.DefineItem();
				ItemDefOf.ZetAspectCyclone = ZetAspectCyclone;
				ZetAspectsContent.itemDefs.Add(ZetAspectCyclone);
				transformableAspectItemDefs.Add(ZetAspectCyclone);

				ItemDef ZetAspectTinker = Items.ZetAspectTinker.DefineItem();
				ItemDefOf.ZetAspectTinker = ZetAspectTinker;
				ZetAspectsContent.itemDefs.Add(ZetAspectTinker);
				transformableAspectItemDefs.Add(ZetAspectTinker);
			}

			if (AspectPackDefOf.Augmentum.Enabled)
			{
				ItemDef ZetAspectAdaptive = Items.ZetAspectAdaptive.DefineItem();
				ItemDefOf.ZetAspectAdaptive = ZetAspectAdaptive;
				ZetAspectsContent.itemDefs.Add(ZetAspectAdaptive);
				transformableAspectItemDefs.Add(ZetAspectAdaptive);
			}

			if (AspectPackDefOf.Sandswept.Enabled)
			{
				ItemDef ZetAspectMotivator = Items.ZetAspectMotivator.DefineItem();
				ItemDefOf.ZetAspectMotivator = ZetAspectMotivator;
				ZetAspectsContent.itemDefs.Add(ZetAspectMotivator);
				transformableAspectItemDefs.Add(ZetAspectMotivator);

				ItemDef ZetAspectOsmium = Items.ZetAspectOsmium.DefineItem();
				ItemDefOf.ZetAspectOsmium = ZetAspectOsmium;
				ZetAspectsContent.itemDefs.Add(ZetAspectOsmium);
				transformableAspectItemDefs.Add(ZetAspectOsmium);
			}

			ItemDef ZetAspectEmpyrean = Items.ZetAspectEmpyrean.DefineItem();
			ItemDefOf.ZetAspectEmpyrean = ZetAspectEmpyrean;
			ZetAspectsContent.itemDefs.Add(ZetAspectEmpyrean);
			transformableAspectItemDefs.Add(ZetAspectEmpyrean);
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
					transformationTable.Add(new ItemDef.Pair() { itemDef1 = itemDef, itemDef2 = ItemDefOf.ZetAspectVoid });
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
			On.RoR2.PickupTransmutationManager.RebuildPickupGroups += (orig) =>
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
			On.RoR2.UI.LogBook.LogBookController.BuildStaticData += (orig) =>
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
					if (Configuration.LogbookHideItem.Value) return false;

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
					if (Configuration.LogbookHideEquipment.Value) return false;

					if (DropHooks.CanObtainEquipment() && HasRequiredExpansion(equipDef.requiredExpansion, expAva))
					{
						ItemIndex itemIndex = ItemizeEliteEquipment(equipDef.equipmentIndex);

						if (itemIndex != ItemIndex.None && !disabledItemIndexes.Contains(itemIndex)) return true;
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
			foreach (AspectPack aspectPack in aspectPacks)
			{
				try
				{
					aspectPack.Entrada();
				}
				catch (Exception ex)
				{
					Logger.Error(ex);
				}
			}
		}

		private static void SetupCatalog()
		{
			if (setupComplete) return;

			Logger.Info("Catalog Setup Initialized");

			if (!lateHooksDone)
			{
				lateHooksDone = true;

				EffectHooks.LateSetup();
				DropHooks.LateSetup();
				//DisplayHooks.LateSetup();

				BuffDefOf.ZetElusive.stackingDisplayMethod = BuffDef.StackingDisplayMethod.Percentage;
			}

			// TODO should actually check and see if settings are enabled - 0 is EliteVariety setting rate on recalc stats
			if (PluginLoaded("com.zombieseatflesh7.dynamicbarrierdecay")) barrierDecayMode = 1; // character fixedupdate
			else if (PluginLoaded("com.RiskyLives.RiskyMod")) barrierDecayMode = 1; // server fixedupdate override
			else if (PluginLoaded("com.TPDespair.StatAdjustment")) barrierDecayMode = 1; // server fixedupdate override

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

			if (PluginLoaded("com.groovesalad.GrooveSaladSpikestripContent"))
			{
				antiGrav = Compat.GrooveSpikeStrip.GetGravityBuffIndex();
			}

			if (PluginLoaded("com.plasmacore.PlasmaCoreSpikestripContent"))
			{
				rageAura = Compat.PlasmaSpikeStrip.GetRageBuffWardBuffIndex();
				veiledBuffer = Compat.PlasmaSpikeStrip.GetVeiledBufferBuffIndex();
			}

			if (PluginLoaded("com.PopcornFactory.WispMod"))
			{
				nullifierRecipient = BuffCatalog.FindBuffIndex("Nullifier Armour Buff");
			}

			if (PluginLoaded("com.TeamMoonstorm.Starstorm2-Nightly") || PluginLoaded("com.TeamMoonstorm") || PluginLoaded("com.TeamMoonstorm.Starstorm2"))
			{
				reactorInvuln = BuffCatalog.FindBuffIndex("BuffReactor");
				lampBuff = BuffCatalog.FindBuffIndex("bdLampBuff");
			}

			if (PluginLoaded("com.themysticsword.risingtides"))
			{
				waterInvuln = BuffCatalog.FindBuffIndex("RisingTides_WaterInvincibility");
			}

			if (EquipmentCatalog.FindEquipmentIndex("NemesisRisingTides_AffixBuffered") != EquipmentIndex.None)
			{
				nemBarrier = true;
			}
			/*
			if (aspectAbilities)
			{
				urchinOrbitalBodyIndex = BodyCatalog.FindBodyIndex("AspectAbilitiesMalachiteUrchinOrbitalBody");
			}
			*/
			summonedEcho = ItemCatalog.FindItemIndex("SummonedEcho");

			ItemTierDef itemTierDef = ItemTierCatalog.FindTierDef("VoidLunarTierDef");
			if (itemTierDef)
			{
				lunarVoidTier = itemTierDef.tier;
			}

			//SetupCompat();
			setupCompat = true;

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

			DisplayHooks.SetupRenderPriority();

			DumpInfo();

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

			setupCompat = true;
		}

		private static void SetupIntermediate()
		{
			if (setupIntermediate)
			{
				Logger.Warn("Catalog SetupIntermediate Called Again!");
				return;
			}

			if (!appliedVoidBearFix)
			{
				appliedVoidBearFix = true;
				DLC1Content.Buffs.BearVoidCooldown.canStack = true;
				EffectHooks.ApplyVoidBearCooldownFix();
			}

			setupIntermediate = true;

			if (PluginLoaded("com.Moffein.EliteReworks") && Configuration.EliteReworksHooks.Value) Compat.EliteReworks.LateSetup();

			if (PluginLoaded("com.plasmacore.PlasmaCoreSpikestripContent")) Compat.PlasmaSpikeStrip.LateSetup();

			if (PluginLoaded("prodzpod.NemesisSpikestrip")) Compat.NemSpikeStrip.LateSetup();

			//if (PluginLoaded("prodzpod.NemesisRisingTides")) Compat.NemRisingTides.LateSetup();

			if (PluginLoaded("com.themysticsword.aspectabilities")) Compat.AspectAbilities.LateSetup();

			foreach (AspectPack aspectPack in aspectPacks)
			{
				try
				{
					aspectPack.Init();

					if (aspectPack.Enabled) aspectPack.PostInit?.Invoke();
				}
				catch (Exception ex)
				{
					Logger.Error(ex);
				}
			}
			
			Language.ChangeText();

			RuleCatalogExcludeItemChoices();
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

		private static void DumpInfo()
		{
			Logger.Info("Catalog - Dump Info");

			try
			{
				Logger.Info("-------------------");
				Logger.Info("buffIndexes: " + aspectBuffIndexes.Count);

				foreach (BuffIndex buffIndex in aspectBuffIndexes)
				{
					BuffDef buffDef = BuffCatalog.GetBuffDef(buffIndex);
					EquipmentDef equipDef = EquipmentCatalog.GetEquipmentDef(buffToEquip[buffIndex]);
					ItemDef itemDef = ItemCatalog.GetItemDef(buffToItem[buffIndex]);

					Logger.Info("[" + buffIndex + "] " + buffDef.name);
					Logger.Info("-- item: " + itemDef.name + " equip: " + equipDef.name);
				}

				Logger.Info("-------------------");
				Logger.Info("disabledAspects: " + disabledItemIndexes.Count);

				foreach (ItemIndex itemIndex in disabledItemIndexes)
				{
					ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
					Logger.Info("[" + itemIndex + "] " + itemDef.name);
				}
			}
			catch (Exception ex)
			{
				Logger.Warn("Failed To Dump Info!");
				Logger.Error(ex);
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

			int jankBuff = Configuration.UpdateInventoryFromBuff.Value;
			if (jankBuff < 2)
			{
				Logger.Info("UpdateInventoryFromBuff : " + jankBuff);

				if (jankBuff == 0) Logger.Warn(" -- Setting this to 0 may cause some elite buff effects to linger after they expire.");
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
			foreach (AspectPack aspectPack in aspectPacks)
			{
				aspectPack.Finale();
			}
		}



		// patch target for ZetAspectHighlander
		public static void SetItemState(ItemDef itemDef, bool shown)
		{
			
		}



		public static bool PluginLoaded(string key)
		{
			return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(key);
		}
	}
}
