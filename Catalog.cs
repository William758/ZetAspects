using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AddressableAssets;
using RoR2;
using RoR2.ContentManagement;
using RoR2.ExpansionManagement;

namespace TPDespair.ZetAspects
{
	public static class Catalog
	{
		public static AssetBundle Assets;



		public static bool setupComplete = false;
		public static bool menuVisited = false;



		public static Dictionary<BuffIndex, ItemIndex> buffToItem = new Dictionary<BuffIndex, ItemIndex>();
		public static Dictionary<BuffIndex, EquipmentIndex> buffToEquip = new Dictionary<BuffIndex, EquipmentIndex>();

		public static Dictionary<EquipmentIndex, ItemIndex> equipToItem = new Dictionary<EquipmentIndex, ItemIndex>();

		public static List<ItemIndex> disabledItemIndexes = new List<ItemIndex>();

		public static List<ItemIndex> aspectItemIndexes = new List<ItemIndex>();
		public static List<EquipmentIndex> aspectEquipIndexes = new List<EquipmentIndex>();



		internal static GameObject BossDropletPrefab;
		internal static GameObject LightningStakePrefab;
		internal static GameObject RejectTextPrefab;



		public static bool limitChillStacks = false;
		public static bool borboFrostBlade = false;
		public static bool shieldJump = false;
		public static bool aspectAbilities = false;
		public static bool immuneHealth = false;

		public static bool ChillCanStack => RoR2Content.Buffs.Slow80.canStack;



		public static class Sprites
		{
			public static Sprite OutlineRed;
			public static Sprite OutlineOrange;
			public static Sprite OutlineYellow;
			public static Sprite OutlineBlue;

			public static Sprite AffixWhite;
			public static Sprite AffixBlue;
			public static Sprite AffixRed;
			public static Sprite AffixHaunted;
			public static Sprite AffixPoison;
			public static Sprite AffixLunar;

			public static Sprite AffixEarth;

			public static Sprite AffixSanguine;

			public static Sprite HauntCloak;
			public static Sprite ZetHeadHunter;
			public static Sprite ZetSapped;
			public static Sprite ZetShredded;
			public static Sprite ZetPoached;



			public static void Load()
			{
				OutlineRed = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineRed.png");
				OutlineOrange = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineOrange.png");
				OutlineYellow = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineYellow.png");
				OutlineBlue = Assets.LoadAsset<Sprite>("Assets/Icons/texOutlineBlue.png");

				AffixWhite = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixWhite.png");
				AffixBlue = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBlue.png");
				AffixRed = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixRed.png");
				AffixHaunted = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixHaunted.png");
				AffixPoison = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPoison.png");
				AffixLunar = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixLunar.png");

				AffixEarth = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixEarth.png");
				/*
				if (Aetherium.Enabled)
				{
					AffixSanguine = Assets.LoadAsset<Sprite>("Assets/Icons/texAffixSanguine.png");
				}
				*/
				HauntCloak = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffHauntCloak.png");
				ZetHeadHunter = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffHeadHunter.png");
				ZetSapped = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffSapped.png");
				ZetShredded = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffShredded.png");
				ZetPoached = Assets.LoadAsset<Sprite>("Assets/Icons/texBuffPoached.png");
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



			public static BuffDef AffixWhite;
			public static BuffDef AffixBlue;
			public static BuffDef AffixRed;
			public static BuffDef AffixHaunted;
			public static BuffDef AffixPoison;
			public static BuffDef AffixLunar;

			public static BuffDef AffixEarth;

			public static BuffDef AffixSanguine;
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

			public static EquipmentDef AffixSanguine;
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

			public static ItemDef ZetAspectSanguine;
		}

		public static EffectDef RejectTextDef;

		public static ArtifactIndex diluvianArtifactIndex = ArtifactIndex.None;
		public static BodyIndex mithrixBodyIndex = BodyIndex.None;
		public static BuffIndex altSlow80 = BuffIndex.None;



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

			if (aspect > 0f && self.teamComponent.teamIndex == TeamIndex.Player)
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
			EquipmentIndex equipIndex = GetAspectEquipIndex(buffDef.buffIndex);

			if (equipIndex == EquipmentIndex.None) return 0;

			int count = 0;

			if (inventory.currentEquipmentIndex == equipIndex) count++;
			if (inventory.alternateEquipmentIndex == equipIndex) count++;

			return count;
		}

		public static bool HasAspectItem(Inventory inventory, BuffDef buffDef)
		{
			ItemIndex itemIndex = GetAspectItemIndex(buffDef.buffIndex);

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
			NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append(ZetAspectsPlugin.ModGuid + ":" + ZetAspectsPlugin.ModVer);

			ContentManager.collectContentPackProviders += AddContentPackProvider;

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
			BossDropletPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/ItemPickups/BossOrb");
			LightningStakePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/LightningStake");
			RejectTextPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/DamageRejected");
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
		}

		private static void CreateItems()
		{
			ItemDef ZetAspectsDropCountTracker = ScriptableObject.CreateInstance<ItemDef>();
			ZetAspectsDropCountTracker.name = "ZetAspectsDropCountTracker";
			ZetAspectsDropCountTracker.tier = ItemTier.NoTier;
			ZetAspectsDropCountTracker.AutoPopulateTokens();
			ZetAspectsDropCountTracker.hidden = true;
			ZetAspectsDropCountTracker.canRemove = false;
			Item.ZetAspectsDropCountTracker = ZetAspectsDropCountTracker;
			ZetAspectsContent.itemDefs.Add(ZetAspectsDropCountTracker);

			ItemDef ZetAspectsUpdateInventory = ScriptableObject.CreateInstance<ItemDef>();
			ZetAspectsUpdateInventory.name = "ZetAspectsUpdateInventory";
			ZetAspectsUpdateInventory.tier = ItemTier.NoTier;
			ZetAspectsUpdateInventory.AutoPopulateTokens();
			ZetAspectsUpdateInventory.hidden = true;
			ZetAspectsUpdateInventory.canRemove = false;
			Item.ZetAspectsUpdateInventory = ZetAspectsUpdateInventory;
			ZetAspectsContent.itemDefs.Add(ZetAspectsUpdateInventory);

			ItemDef ZetAspectWhite = Items.ZetAspectWhite.DefineItem();
			Item.ZetAspectWhite = ZetAspectWhite;
			ZetAspectsContent.itemDefs.Add(ZetAspectWhite);

			ItemDef ZetAspectBlue = Items.ZetAspectBlue.DefineItem();
			Item.ZetAspectBlue = ZetAspectBlue;
			ZetAspectsContent.itemDefs.Add(ZetAspectBlue);

			ItemDef ZetAspectRed = Items.ZetAspectRed.DefineItem();
			Item.ZetAspectRed = ZetAspectRed;
			ZetAspectsContent.itemDefs.Add(ZetAspectRed);

			ItemDef ZetAspectHaunted = Items.ZetAspectHaunted.DefineItem();
			Item.ZetAspectHaunted = ZetAspectHaunted;
			ZetAspectsContent.itemDefs.Add(ZetAspectHaunted);

			ItemDef ZetAspectPoison = Items.ZetAspectPoison.DefineItem();
			Item.ZetAspectPoison = ZetAspectPoison;
			ZetAspectsContent.itemDefs.Add(ZetAspectPoison);

			ItemDef ZetAspectLunar = Items.ZetAspectLunar.DefineItem();
			Item.ZetAspectLunar = ZetAspectLunar;
			ZetAspectsContent.itemDefs.Add(ZetAspectLunar);

			ItemDef ZetAspectEarth = Items.ZetAspectEarth.DefineItem();
			Item.ZetAspectEarth = ZetAspectEarth;
			ZetAspectsContent.itemDefs.Add(ZetAspectEarth);
			/*
			if (Aetherium.Enabled)
			{
				ItemDef ZetAspectSanguine = Items.ZetAspectSanguine.DefineItem();
				Item.ZetAspectSanguine = ZetAspectSanguine;
				ZetAspectsContent.itemDefs.Add(ZetAspectSanguine);
			}
			*/
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
			//Aetherium.PreInit();
		}

		private static void SetupCatalog()
		{
			if (setupComplete) return;

			if (PluginLoaded("com.Borbo.ArtificerExtended")) limitChillStacks = true;
			if (PluginLoaded("com.Borbo.BORBO")) borboFrostBlade = true;
			if (PluginLoaded("com.TransRights.RealisticTransgendence")) shieldJump = true;// Reflection Config
			if (PluginLoaded("com.TheMysticSword.AspectAbilities")) aspectAbilities = true;
			//if (PluginLoaded("com.DestroyedClone.HealthbarImmune")) immuneHealth = true;
			//if (Configuration.RecolorImmuneHealth.Value) immuneHealth = true;
			/*
			EffectIndex effectIndex = EffectCatalog.FindEffectIndexFromPrefab(RejectTextPrefab);
			RejectTextDef = EffectCatalog.GetEffectDef(effectIndex);
			*/
			diluvianArtifactIndex = ArtifactCatalog.FindArtifactIndex("ARTIFACT_DILUVIFACT");
			altSlow80 = BuffCatalog.FindBuffIndex("EliteReworksSlow80");

			RiskOfRain.Init();
			//Aetherium.Init();

			Language.ChangeText();

			RuleCatalogExcludeItemChoices();

			Logger.Info("Catalog Setup Complete");

			setupComplete = true;
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
			/*
			if (Aetherium.populated)
			{
				Aetherium.ItemEntries(true);
				Aetherium.EquipmentEntries(false);
			}
			*/
		}



		public static class RiskOfRain
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;



			internal static void PreInit()
			{
				PopulateEquipment();
				ApplyEquipmentIcons();
			}

			internal static void Init()
			{
				mithrixBodyIndex = BodyCatalog.FindBodyIndex("BrotherBody");

				PopulateEquipment();
				PopulateBuffs();

				SetupText();
				ItemEntries(DropHooks.CanObtainItem());

				CopyExpansionReq();
				CopyModelPrefabs();

				ApplyEquipmentIcons();
				if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
				EquipmentColor();

				BuffDef buffDef = RoR2Content.Buffs.AffixHauntedRecipient;
				buffDef.buffColor = Color.white;
				buffDef.iconSprite = Sprites.HauntCloak;

				FillEqualities();

				populated = true;
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				Equip.AffixWhite = RoR2Content.Equipment.AffixWhite;
				Equip.AffixBlue = RoR2Content.Equipment.AffixBlue;
				Equip.AffixRed = RoR2Content.Equipment.AffixRed;
				Equip.AffixHaunted = RoR2Content.Equipment.AffixHaunted;
				Equip.AffixPoison = RoR2Content.Equipment.AffixPoison;
				Equip.AffixLunar = RoR2Content.Equipment.AffixLunar;

				Equip.AffixEarth = EquipmentCatalog.GetEquipmentDef(EquipmentCatalog.FindEquipmentIndex("EliteEarthEquipment"));

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				Buff.AffixWhite = RoR2Content.Buffs.AffixWhite;
				Buff.AffixBlue = RoR2Content.Buffs.AffixBlue;
				Buff.AffixRed = RoR2Content.Buffs.AffixRed;
				Buff.AffixHaunted = RoR2Content.Buffs.AffixHaunted;
				Buff.AffixPoison = RoR2Content.Buffs.AffixPoison;
				Buff.AffixLunar = RoR2Content.Buffs.AffixLunar;

				Buff.AffixEarth = DLC1Content.Buffs.EliteEarth;

				buffDefPopulated = true;
			}



			private static void SetupText()
			{
				Items.ZetAspectWhite.SetupTokens();
				Items.ZetAspectBlue.SetupTokens();
				Items.ZetAspectRed.SetupTokens();
				Items.ZetAspectHaunted.SetupTokens();
				Items.ZetAspectPoison.SetupTokens();
				Items.ZetAspectLunar.SetupTokens();

				Items.ZetAspectEarth.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectWhite, shown);
				SetItemState(Item.ZetAspectBlue, shown);
				SetItemState(Item.ZetAspectRed, shown);
				SetItemState(Item.ZetAspectHaunted, shown);
				SetItemState(Item.ZetAspectPoison, shown);
				SetItemState(Item.ZetAspectLunar, shown);

				SetItemState(Item.ZetAspectEarth, shown);
			}

			private static void CopyExpansionReq()
			{
				CopyExpansion(Item.ZetAspectWhite, Equip.AffixWhite);
				CopyExpansion(Item.ZetAspectBlue, Equip.AffixBlue);
				CopyExpansion(Item.ZetAspectRed, Equip.AffixRed);
				CopyExpansion(Item.ZetAspectHaunted, Equip.AffixHaunted);
				CopyExpansion(Item.ZetAspectPoison, Equip.AffixPoison);
				CopyExpansion(Item.ZetAspectLunar, Equip.AffixLunar);

				CopyExpansion(Item.ZetAspectEarth, Equip.AffixEarth);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectEarth, Equip.AffixEarth);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixWhite, Sprites.AffixWhite, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixBlue, Sprites.AffixBlue, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixRed, Sprites.AffixRed, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixHaunted, Sprites.AffixHaunted, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixPoison, Sprites.AffixPoison, Sprites.OutlineOrange);
				ReplaceEquipmentIcon(Equip.AffixLunar, Sprites.AffixLunar, Sprites.OutlineBlue);

				ReplaceEquipmentIcon(Equip.AffixEarth, Sprites.AffixEarth, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixWhite, shown);
				SetEquipmentState(Equip.AffixBlue, shown);
				SetEquipmentState(Equip.AffixRed, shown);
				SetEquipmentState(Equip.AffixHaunted, shown);
				SetEquipmentState(Equip.AffixPoison, shown);
				SetEquipmentState(Equip.AffixLunar, shown);

				SetEquipmentState(Equip.AffixEarth, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixWhite);
				ColorEquipmentDroplet(Equip.AffixBlue);
				ColorEquipmentDroplet(Equip.AffixRed);
				ColorEquipmentDroplet(Equip.AffixHaunted);
				ColorEquipmentDroplet(Equip.AffixPoison);
				ColorEquipmentDroplet(Equip.AffixLunar);

				ColorEquipmentDroplet(Equip.AffixEarth);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixWhite, Buff.AffixWhite, Item.ZetAspectWhite);
				CreateEquality(Equip.AffixBlue, Buff.AffixBlue, Item.ZetAspectBlue);
				CreateEquality(Equip.AffixRed, Buff.AffixRed, Item.ZetAspectRed);
				CreateEquality(Equip.AffixHaunted, Buff.AffixHaunted, Item.ZetAspectHaunted);
				CreateEquality(Equip.AffixPoison, Buff.AffixPoison, Item.ZetAspectPoison);
				CreateEquality(Equip.AffixLunar, Buff.AffixLunar, Item.ZetAspectLunar);

				CreateEquality(Equip.AffixEarth, Buff.AffixEarth, Item.ZetAspectEarth);
			}
		}
		/*
		public static class Aetherium
		{
			private static bool equipDefPopulated = false;
			private static bool buffDefPopulated = false;
			private static bool iconsReplaced = false;

			public static bool populated = false;

			private static int state = -1;
			public static bool Enabled
			{
				get
				{
					if (state == -1)
					{
						if (PluginLoaded("com.KomradeSpectre.Aetherium")) state = 1;
						else state = 0;
					}
					return state == 1;
				}
			}



			internal static void PreInit()
			{
				if (Enabled)
				{
					PopulateEquipment();
					DisableInactiveItems();
					ApplyEquipmentIcons();
				}
			}

			internal static void Init()
			{
				if (Enabled)
				{
					PopulateEquipment();
					PopulateBuffs();

					DisableInactiveItems();
					SetupText();
					ItemEntries(DropHooks.CanObtainItem());

					CopyModelPrefabs();

					ApplyEquipmentIcons();
					if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
					EquipmentColor();

					FillEqualities();

					populated = true;
				}
			}



			private static void PopulateEquipment()
			{
				if (equipDefPopulated) return;

				EquipmentIndex index;

				index = EquipmentCatalog.FindEquipmentIndex("AETHERIUM_ELITE_EQUIPMENT_AFFIX_SANGUINE");
				if (index != EquipmentIndex.None) Equip.AffixSanguine = EquipmentCatalog.GetEquipmentDef(index);

				equipDefPopulated = true;
			}

			private static void PopulateBuffs()
			{
				if (buffDefPopulated) return;

				BuffIndex index;

				index = BuffCatalog.FindBuffIndex("AFFIX_SANGUINE");
				if (index != BuffIndex.None) Buff.AffixSanguine = BuffCatalog.GetBuffDef(index);

				buffDefPopulated = true;
			}



			private static void DisableInactiveItems()
			{
				int state = GetPopulatedState(equipDefPopulated, buffDefPopulated);

				DisableInactiveItem(Item.ZetAspectSanguine, ref Equip.AffixSanguine, ref Buff.AffixSanguine, state);
			}

			private static void SetupText()
			{
				Items.ZetAspectSanguine.SetupTokens();
			}

			internal static void ItemEntries(bool shown)
			{
				SetItemState(Item.ZetAspectSanguine, shown);
			}

			private static void CopyModelPrefabs()
			{
				CopyEquipmentPrefab(Item.ZetAspectSanguine, Equip.AffixSanguine);
			}

			private static void ApplyEquipmentIcons()
			{
				if (iconsReplaced) return;

				ReplaceEquipmentIcon(Equip.AffixSanguine, Sprites.AffixSanguine, Sprites.OutlineOrange);

				iconsReplaced = true;
			}

			internal static void EquipmentEntries(bool shown)
			{
				SetEquipmentState(Equip.AffixSanguine, shown);
			}

			internal static void EquipmentColor()
			{
				ColorEquipmentDroplet(Equip.AffixSanguine);
			}

			internal static void FillEqualities()
			{
				CreateEquality(Equip.AffixSanguine, Buff.AffixSanguine, Item.ZetAspectSanguine);
			}
		}
		*/


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

			if (itemDef.tier == ItemTier.Tier3)
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
					if (!shown) itemDef.tier = ItemTier.NoTier;
					else itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
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
