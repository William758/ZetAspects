using UnityEngine;
using RoR2;
using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace TPDespair.ZetAspects
{
	public enum PopState
	{
		None = 0,
		Equip = 1,
		Buff = 2,
		Item = 4,
		Replace = 8,
		Populated = 16
	}

	public static class FlagExt
	{
		public static void SetFlag(this ref PopState flag, PopState other)
		{
			flag |= other;
		}

		public static bool CheckFlag(this PopState flag, PopState other)
		{
			return (flag & other) != 0;
		}
	}

	public class AspectDef
	{
		/// <summary>A unique identifier must be provided for AspectDef to load.</summary>
		public string identifier = "";

		public AspectPack pack;

		public EquipmentDef equipmentDef;
		public BuffDef buffDef;
		public ItemDef itemDef;

		/// <summary>Name to search for associated EquipmentDef.</summary>
		public string equipmentName = "";
		/// <summary>Name to search for associated BuffDef.</summary>
		public string buffName = "";
		/// <summary>Name to search for associated ItemDef.</summary>
		public string itemName = "";

		/// <summary>Custom function to search for associated EquipmentDef.</summary>
		public Func<EquipmentDef> FindEquipmentDef;
		/// <summary>Custom function to search for associated BuffDef.</summary>
		public Func<BuffDef> FindBuffDef;
		/// <summary>Custom function to search for associated ItemDef.</summary>
		public Func<ItemDef> FindItemDef;

		/// <summary>Called when associated EquipmentDef is found. Also called when setup fails, EquipmentDef will be null.</summary>
		public Action<EquipmentDef> RefEquipmentDef;
		/// <summary>Called when associated BuffDef is found. Also called when setup fails, BuffDef will be null.</summary>
		public Action<BuffDef> RefBuffDef;
		/// <summary>Called when associated ItemDef is found. Also called when setup fails, ItemDef will be null.</summary>
		public Action<ItemDef> RefItemDef;

		public Func<Sprite> BaseIcon;
		public Func<Sprite> OutlineIcon;

		/// <summary>Whether to copy the pickup prefab of the assoiciated equipment. Otherwise apply the pickup prefab of the item to the associated equipment.</summary>
		public bool copyEquipmentPrefab = true;

		/// <summary>Determines the priority in which elite displays are applied. Higher values have higher priority.</summary>
		public int displayPriority = 1;

		/// <summary>Remove item displays from character model on death.</summary>
		public bool disableDisplayOnDeath = false;

		/// <summary>Called during setup or when associated configs are changed.</summary>
		public Action SetupTokens;

		//public Action<AspectDef> PostInit;

		/// <summary>Prevent elite buff from being applied to blacklisted bodies.</summary>
		public List<string> bodyBlacklist = new List<string>();
		public List<BodyIndex> blacklistedIndexes = new List<BodyIndex>();

		/// <summary>Custom function that must return true to allow elite buff on CharacterBody.</summary>
		public Func<CharacterBody, Inventory, bool> AllowAffix;

		/// <summary>Called before elite buff is granted from : OnEquipmentGained and OnInventoryChanged. Return true to grant the elite buff.</summary>
		public Func<CharacterBody, Inventory, bool> PreBuffGrant;

		/// <summary>Allows EliteBuffManager to refresh other buffs.</summary>
		public Func<CharacterBody, IEnumerable<BuffIndex>> Promoter;

		/// <summary>Called whenever EliteBuffManager refreshes buff duration.</summary>
		public Action<CharacterBody> OnRefresh;

		/// <summary>Called whenever buff fails to refresh and expires.</summary>
		public Action<CharacterBody> OnExpire;

		public bool invalid = false;

		public bool PackPopulated
		{
			get
			{
				if (pack != null)
				{
					return pack.Populated;
				}

				return false;
			}
		}

		public bool PackEnabled
		{
			get
			{
				if (pack != null)
				{
					return pack.Enabled;
				}

				return false;
			}
		}

		public ConfigEntry<float> DropWeight;

		internal void CreateDropWeightConfig(ConfigFile config)
		{
			DropWeight = config.Bind(
				"0b-DropWeight", "aspectDropWeight" + identifier, 1f,
				"Drop chance multiplier for " + identifier
			);
		}
	}

	public class AspectPack
	{
		/// <summary>A unique identifier must be provided for AspectPack to load.</summary>
		public string identifier = "";

		/// <summary>Plugin GUID that needs to be installed for AspectPack to load.</summary>
		public string dependency = "";

		/// <summary>Plugin GUIDs that needs to be installed for AspectPack to load.</summary>
		public List<string> dependencies = new List<string>();

		/// <summary>Custom function that must return true for AspectPack to load.</summary>
		public Func<bool> CustomDependency = () => true;

		/// <summary>Used when target plugin has dependency on current plugin. AspectPack will always go through validation steps.</summary>
		public bool alwaysValidate = false;

		public Action PostInit;



		private int _enabled = -1;
		public bool Enabled
		{
			get
			{
				if (_enabled == -1)
				{
					if (dependency == "")
					{
						if (dependencies.Count == 0)
						{
							_enabled = CustomDependency() ? 1 : 0;
						}
						else
						{
							foreach (string dependency in dependencies)
							{
								if (!Catalog.PluginLoaded(dependency))
								{
									_enabled = 0;

									return false;
								}
							}

							_enabled = 1;
						}
					}
					else
					{
						_enabled = Catalog.PluginLoaded(dependency) ? 1 : 0;
					}
				}

				return _enabled == 1;
			}
		}

		private PopState PopState = PopState.None;

		public bool Populated
		{
			get
			{
				return PopState.CheckFlag(PopState.Populated);
			}
		}

		public List<AspectDef> aspectDefs = new List<AspectDef>();



		public AspectPack Register()
		{
			Catalog.aspectPacks.Add(this);

			foreach (AspectDef aspectDef in aspectDefs)
			{
				Catalog.aspectDefs.Add(aspectDef);
			}

			return this;
		}



		internal void Entrada()
		{
			if (Enabled)
			{
				AssignPack();
				PopulateEquipment();
				PopulateItems();
				ValidateDefs();
				DisableInactiveItems();

				ApplyIcons();
			}
			else if (alwaysValidate)
			{
				AssignPack();
				PopulateEquipment();
				PopulateItems();
				ValidateDefs();
				DisableInactiveItems();
			}
		}

		internal void Init()
		{
			if (Enabled)
			{
				PopulateEquipment();
				PopulateBuffs();
				PopulateItems();
				ValidateDefs();
				DisableInactiveItems();

				ResolveBlacklist();

				CopyExpansionReq();
				CopyModelPrefabs();
				ApplyIcons();
				EquipmentColor();
				SetupTokens();

				if (DropHooks.CanObtainEquipment()) EquipmentEntries(true);
				ItemEntries(DropHooks.CanObtainItem());

				FillEqualities();

				PopState.SetFlag(PopState.Populated);
			}
			else if (alwaysValidate)
			{
				PopulateEquipment();
				PopulateItems();
				ValidateDefs();
				DisableInactiveItems();
			}
		}

		internal void Finale()
		{
			if (Populated)
			{
				ItemEntries(true);
				EquipmentEntries(false);
			}
		}



		private void AssignPack()
		{
			foreach (AspectDef aspectDef in aspectDefs)
			{
				aspectDef.pack = this;
			}
		}



		private void PopulateEquipment()
		{
			if (PopState.CheckFlag(PopState.Equip)) return;

			foreach (AspectDef aspectDef in aspectDefs)
			{
				EquipmentDef equipDef = null;

				if (aspectDef.equipmentName != "")
				{
					EquipmentIndex index = EquipmentCatalog.FindEquipmentIndex(aspectDef.equipmentName);
					if (index != EquipmentIndex.None)
					{
						equipDef = EquipmentCatalog.GetEquipmentDef(index);
					}

					if (!equipDef)
					{
						Logger.Warn("Failed to find EquipmentDef named : " + aspectDef.equipmentName);
					}
				}

				if (!equipDef)
				{
					equipDef = aspectDef.FindEquipmentDef?.Invoke();
				}

				if (equipDef)
				{
					aspectDef.equipmentDef = equipDef;

					aspectDef.RefEquipmentDef?.Invoke(equipDef);
				}

				if (!aspectDef.equipmentDef)
				{
					aspectDef.invalid = true;
					Logger.Warn("Failed to find EquipmentDef for : " + aspectDef.identifier);
				}
			}

			PopState.SetFlag(PopState.Equip);
		}

		private void PopulateBuffs()
		{
			if (PopState.CheckFlag(PopState.Buff)) return;

			foreach (AspectDef aspectDef in aspectDefs)
			{
				BuffDef buffDef = null;

				if (aspectDef.buffName != "")
				{
					BuffIndex index = BuffCatalog.FindBuffIndex(aspectDef.buffName);
					if (index != BuffIndex.None)
					{
						buffDef = BuffCatalog.GetBuffDef(index);
					}

					if (!buffDef)
					{
						Logger.Warn("Failed to find BuffDef named : " + aspectDef.buffName);
					}
				}

				if (!buffDef)
				{
					buffDef = aspectDef.FindBuffDef?.Invoke();
				}

				if (!buffDef && aspectDef.equipmentDef != null)
				{
					buffDef = aspectDef.equipmentDef.passiveBuffDef;

					if (!buffDef)
					{
						Logger.Error("EquipmentDef of " + aspectDef.identifier + " does not have a passiveBuffDef!");
					}
				}

				if (buffDef)
				{
					aspectDef.buffDef = buffDef;

					aspectDef.RefBuffDef?.Invoke(buffDef);
				}

				if (!aspectDef.buffDef)
				{
					aspectDef.invalid = true;
					Logger.Warn("Failed to find BuffDef for : " + aspectDef.identifier);
				}
			}

			PopState.SetFlag(PopState.Buff);
		}

		private void PopulateItems()
		{
			if (PopState.CheckFlag(PopState.Item)) return;

			foreach (AspectDef aspectDef in aspectDefs)
			{
				ItemDef itemDef = null;

				if (aspectDef.itemName != "")
				{
					ItemIndex index = ItemCatalog.FindItemIndex(aspectDef.itemName);
					if (index != ItemIndex.None)
					{
						itemDef = ItemCatalog.GetItemDef(index);
					}

					if (!itemDef)
					{
						Logger.Warn("Failed to find ItemDef named : " + aspectDef.itemName);
					}
				}

				if (!itemDef)
				{
					itemDef = aspectDef.FindItemDef?.Invoke();
				}

				if (itemDef)
				{
					aspectDef.itemDef = itemDef;

					aspectDef.RefItemDef?.Invoke(itemDef);
				}

				if (!aspectDef.itemDef)
				{
					aspectDef.invalid = true;
					Logger.Warn("Failed to find ItemDef for : " + aspectDef.identifier);
				}
			}

			PopState.SetFlag(PopState.Item);
		}

		private void ValidateDefs()
		{
			if ((PopState & (PopState)7) == 0)
			{
				Logger.Warn("Tried to ValidateDefs of " + identifier + " but nothing is populated!");

				return;
			}

			foreach (AspectDef aspectDef in aspectDefs)
			{
				if (PopState.CheckFlag(PopState.Equip))
				{
					if (aspectDef.equipmentDef)
					{
						if (aspectDef.equipmentDef.equipmentIndex == EquipmentIndex.None)
						{
							aspectDef.invalid = true;
							Logger.Warn(aspectDef.identifier + " : associated EquipmentIndex is [none]!");
						}
					}
					else
					{
						aspectDef.invalid = true;
						Logger.Warn(aspectDef.identifier + " : associated Equipment is null!");
					}
				}

				if (PopState.CheckFlag(PopState.Buff))
				{
					if (aspectDef.buffDef)
					{
						if (aspectDef.buffDef.buffIndex == BuffIndex.None)
						{
							aspectDef.invalid = true;
							Logger.Warn(aspectDef.identifier + " : associated BuffIndex is [none]!");
						}
					}
					else
					{
						aspectDef.invalid = true;
						Logger.Warn(aspectDef.identifier + " : associated Buff is null!");
					}
				}

				if (PopState.CheckFlag(PopState.Item))
				{
					if (aspectDef.itemDef)
					{
						if (aspectDef.itemDef.itemIndex == ItemIndex.None)
						{
							aspectDef.invalid = true;
							Logger.Warn(aspectDef.identifier + " : associated ItemIndex is [none]!");
						}
					}
					else
					{
						aspectDef.invalid = true;
						Logger.Warn(aspectDef.identifier + " : associated Item is null!");
					}
				}
			}
		}

		private void DisableInactiveItems()
		{
			foreach (AspectDef aspectDef in aspectDefs)
			{
				if (aspectDef.invalid)
				{
					DeactivateItem(aspectDef);

					aspectDef.RefEquipmentDef?.Invoke(null);
					aspectDef.RefBuffDef?.Invoke(null);
					aspectDef.RefItemDef?.Invoke(null);
				}
			}
		}



		private void ResolveBlacklist()
		{
			foreach (AspectDef aspectDef in aspectDefs)
			{
				if (aspectDef.bodyBlacklist.Count > 0)
				{
					foreach (string name in aspectDef.bodyBlacklist)
					{
						BodyIndex bodyIndex = BodyCatalog.FindBodyIndex(name);
						if (bodyIndex != BodyIndex.None)
						{
							aspectDef.blacklistedIndexes.Add(bodyIndex);
						}
					}
				}
			}
		}



		private void CopyExpansionReq()
		{
			foreach (AspectDef aspectDef in aspectDefs)
			{
				CopyExpansion(aspectDef);
			}
		}

		private void CopyModelPrefabs()
		{
			foreach (AspectDef aspectDef in aspectDefs)
			{
				CopyModelPrefab(aspectDef);
			}
		}

		private void ApplyIcons()
		{
			if (PopState.CheckFlag(PopState.Replace)) return;

			foreach (AspectDef aspectDef in aspectDefs)
			{
				SetupIcons(aspectDef);
			}

			PopState.SetFlag(PopState.Replace);
		}

		private void EquipmentColor()
		{
			foreach (AspectDef aspectDef in aspectDefs)
			{
				if (aspectDef.equipmentDef)
				{
					ColorEquipmentDroplet(aspectDef.equipmentDef);
				}
			}
		}

		private void SetupTokens()
		{
			foreach (AspectDef aspectDef in aspectDefs)
			{
				aspectDef.SetupTokens?.Invoke();
			}
		}



		internal void ItemEntries(bool shown)
		{
			foreach (AspectDef aspectDef in aspectDefs)
			{
				SetItemState(aspectDef, shown);
			}
		}

		internal void EquipmentEntries(bool shown)
		{
			foreach (AspectDef aspectDef in aspectDefs)
			{
				SetEquipmentState(aspectDef, shown);
			}
		}



		internal void FillEqualities()
		{
			if ((PopState & (PopState)7) == (PopState)7)
			{
				foreach (AspectDef aspectDef in aspectDefs)
				{
					if (!aspectDef.invalid)
					{
						CreateEquality(aspectDef);
					}
				}
			}
			else
			{
				Logger.Error("Tried to FillEqualities for : " + identifier + " but defs have not been fully validated!");
				Logger.Warn("---- PopState: " + PopState);
			}
		}







		private static void DeactivateItem(AspectDef aspectDef)
		{
			ItemDef itemDef = aspectDef.itemDef;

			if (!itemDef)
			{
				Logger.Warn("Tried to disable ItemDef of " + aspectDef.identifier + " but it is null!");

				return;
			}

			if (Catalog.disabledItemIndexes.Contains(itemDef.itemIndex))
			{
				Logger.Warn(itemDef.name + " : is already disabled!");
			}

			Logger.Warn("Deactivating : " + itemDef.name);

			if (itemDef._itemTierDef == Catalog.BossItemTier)
			{
				if (ItemCatalog.tier3ItemList.Contains(itemDef.itemIndex))
				{
					ItemCatalog.tier3ItemList.Remove(itemDef.itemIndex);
				}
			}

			itemDef.tier = ItemTier.NoTier;
			Catalog.AssignDepricatedTier(itemDef, ItemTier.NoTier);
			itemDef.hidden = true;
			if (itemDef.DoesNotContainTag(ItemTag.WorldUnique))
			{
				ItemTag[] tags = itemDef.tags;
				int index = tags.Length;

				Array.Resize(ref tags, index + 1);
				tags[index] = ItemTag.WorldUnique;

				itemDef.tags = tags;
			}

			if (!Catalog.disabledItemIndexes.Contains(itemDef.itemIndex))
			{
				Catalog.disabledItemIndexes.Add(itemDef.itemIndex);
			}
		}



		private static void CopyExpansion(AspectDef aspectDef)
		{
			if (aspectDef.equipmentDef && aspectDef.itemDef)
			{
				aspectDef.itemDef.requiredExpansion = aspectDef.equipmentDef.requiredExpansion;

				return;
			}

			Logger.Warn("Could not copy expansion requirement for " + aspectDef.identifier);

			LogNullInfo(aspectDef);
		}

		private static void CopyModelPrefab(AspectDef aspectDef)
		{
			if (aspectDef.equipmentDef && aspectDef.itemDef)
			{
				if (aspectDef.copyEquipmentPrefab)
				{
					CopyEquipmentPrefab(aspectDef.itemDef, aspectDef.equipmentDef);
				}
				else
				{
					CopyItemPrefab(aspectDef.itemDef, aspectDef.equipmentDef);
				}

				return;
			}

			if (aspectDef.copyEquipmentPrefab)
			{
				Logger.Warn("Could not copy model prefab [EQUIP] --> [ITEM] for : " + aspectDef.identifier);
			}
			else
			{
				Logger.Warn("Could not copy model prefab [ITEM] --> [EQUIP] for : " + aspectDef.identifier);
			}

			LogNullInfo(aspectDef);
		}

		private static void LogNullInfo(AspectDef aspectDef)
		{
			if (!aspectDef.equipmentDef && !aspectDef.itemDef)
			{
				Logger.Warn("---- EquipmentDef and ItemDef is null!");
			}
			else if (!aspectDef.itemDef)
			{
				Logger.Warn("---- ItemDef is null!");
			}
			else if (!aspectDef.itemDef)
			{
				Logger.Warn("---- EquipmentDef is null!");
			}
		}

		private static void CopyEquipmentPrefab(ItemDef itemDef, EquipmentDef equipDef)
		{
			itemDef.pickupModelPrefab = equipDef.pickupModelPrefab;
			PickupDef pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(itemDef.itemIndex));
			pickupDef.displayPrefab = equipDef.pickupModelPrefab;
		}

		private static void CopyItemPrefab(ItemDef itemDef, EquipmentDef equipDef)
		{
			equipDef.pickupModelPrefab = itemDef.pickupModelPrefab;
			PickupDef pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(equipDef.equipmentIndex));
			pickupDef.displayPrefab = itemDef.pickupModelPrefab;
		}

		private static void SetupIcons(AspectDef aspectDef)
		{
			Sprite baseSprite = aspectDef.BaseIcon != null ? aspectDef.BaseIcon() : null;
			Sprite outlineSprite = aspectDef.OutlineIcon != null ? aspectDef.OutlineIcon() : null;

			if (baseSprite && outlineSprite)
			{
				EquipmentDef equipDef = aspectDef.equipmentDef;
				if (equipDef)
				{
					Color32 outlineColor = aspectDef.equipmentDef.isLunar ? new Color32(100, 225, 250, 255) : new Color32(250, 150, 50, 255);
					equipDef.pickupIconSprite = Catalog.CreateAspectSprite(baseSprite, outlineSprite, outlineColor);

					PickupDef equipPickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(equipDef.equipmentIndex));
					equipPickupDef.iconSprite = equipDef.pickupIconSprite;
					equipPickupDef.iconTexture = equipDef.pickupIconSprite.texture;
				}

				ItemDef itemDef = aspectDef.itemDef;
				if (itemDef)
				{
					Color32 outlineColor = Configuration.AspectRedTier.Value ? new Color32(230, 70, 60, 255) : new Color32(225, 250, 40, 255);
					if (Catalog.AsHighlander) outlineColor = new Color32(225, 220, 165, 255);
					if (outlineSprite == Catalog.Sprites.OutlineVoid) outlineColor = new Color32(255, 255, 255, 255);
					itemDef.pickupIconSprite = Catalog.CreateAspectSprite(baseSprite, outlineSprite, outlineColor);

					PickupDef itemPickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(itemDef.itemIndex));
					itemPickupDef.iconSprite = itemDef.pickupIconSprite;
					itemPickupDef.iconTexture = itemDef.pickupIconSprite.texture;
				}
			}
			else
			{
				Logger.Warn("SetupIcons failed for : " + aspectDef.identifier);

				if (baseSprite == null)
				{
					Logger.Warn("---- BaseSprite is null!");
				}

				if (outlineSprite == null)
				{
					Logger.Warn("---- OutlineSprite is null!");
				}
			}
		}

		private static void ColorEquipmentDroplet(EquipmentDef equipDef)
		{
			equipDef.isBoss = true;
			equipDef.colorIndex = ColorCatalog.ColorIndex.Artifact;

			PickupDef pickupDef = PickupCatalog.GetPickupDef(PickupCatalog.FindPickupIndex(equipDef.equipmentIndex));

			pickupDef.isBoss = true;
			if (Catalog.BossDropletPrefab) pickupDef.dropletDisplayPrefab = Catalog.BossDropletPrefab;
			pickupDef.baseColor = new Color(0.9f, 0.7f, 0.75f);
			pickupDef.darkColor = new Color(0.9f, 0.7f, 0.75f);
		}



		private static void SetItemState(AspectDef aspectDef, bool shown)
		{
			ItemDef itemDef = aspectDef.itemDef;

			if (!itemDef)
			{
				Logger.Warn("Tried to set ItemState of " + aspectDef.identifier + " but it is null!");

				return;
			}

			if (!Catalog.aspectItemIndexes.Contains(itemDef.itemIndex))
			{
				Catalog.aspectItemIndexes.Add(itemDef.itemIndex);
			}

			if (!itemDef.hidden)
			{
				if (itemDef == ItemDefOf.ZetAspectVoid && Configuration.AspectVoidContagiousItem.Value)
				{
					if (!shown) itemDef.tier = ItemTier.NoTier;
					else itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.VoidTier3 : ItemTier.VoidBoss;
				}
				else
				{
					if (!shown) itemDef.tier = ItemTier.NoTier;
					else itemDef._itemTierDef = Configuration.AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
				}

				if (Catalog.AsHighlander && Catalog.highlanderTier != ItemTier.AssignedAtRuntime && (itemDef._itemTierDef == Catalog.BossItemTier || itemDef._itemTierDef == Catalog.RedItemTier))
				{
					itemDef._itemTierDef = Catalog.HighlanderItemTier;
				}
			}
		}

		private static void SetEquipmentState(AspectDef aspectDef, bool canDrop)
		{
			EquipmentDef equipDef = aspectDef.equipmentDef;

			if (!equipDef)
			{
				Logger.Warn("Tried to set EquipmentState of " + aspectDef.identifier + " but it is null!");

				return;
			}

			if (!Catalog.aspectEquipIndexes.Contains(equipDef.equipmentIndex))
			{
				Catalog.aspectEquipIndexes.Add(equipDef.equipmentIndex);
			}

			equipDef.canDrop = canDrop;
		}

		

		private static void CreateEquality(AspectDef aspectDef)
		{
			EquipmentDef equipDef = aspectDef.equipmentDef;
			BuffDef buffDef = aspectDef.buffDef;
			ItemDef itemDef = aspectDef.itemDef;

			Logger.Info("CreateEquality : " + (equipDef ? equipDef.name : "null") + " , " + (buffDef ? ("[" + buffDef.buffIndex + "] " + buffDef.name) : "null") + " , " + (itemDef ? itemDef.name : "null"));

			if (!itemDef)
			{
				Logger.Warn("CreateEquality called for : " + aspectDef.identifier + " but its associated item is null!");

				return;
			}
			else if (Catalog.disabledItemIndexes.Contains(itemDef.itemIndex))
			{
				Logger.Warn("CreateEquality called for disabled aspect : " + aspectDef.identifier);

				return;
			}

			if (equipDef && buffDef)
			{
				if (!Catalog.aspectBuffIndexes.Contains(buffDef.buffIndex))
				{
					Catalog.aspectBuffIndexes.Add(buffDef.buffIndex);
				}



				if (aspectDef.Promoter != null)
				{
					if (!Catalog.aspectsWithPromoters.Contains(aspectDef))
					{
						Catalog.aspectsWithPromoters.Add(aspectDef);

						Logger.Info("Registered promoter: " + aspectDef.identifier);
					}
				}



				if (!Catalog.buffToAspect.ContainsKey(buffDef.buffIndex))
				{
					Catalog.buffToAspect.Add(buffDef.buffIndex, aspectDef);
				}
				else
				{
					Logger.Warn("buffToAspect already contains key for : [" + buffDef.buffIndex + "] - " + buffDef.name);
					Logger.Warn("---- current aspect : " + aspectDef.identifier);
					Logger.Warn("---- associated aspect : " + Catalog.buffToAspect[buffDef.buffIndex].identifier);
				}



				if (!Catalog.buffToItem.ContainsKey(buffDef.buffIndex))
				{
					Catalog.buffToItem.Add(buffDef.buffIndex, itemDef.itemIndex);
				}
				else
				{
					Logger.Warn("buffToItem already contains key for : [" + buffDef.buffIndex + "] - " + buffDef.name);
					Logger.Warn("---- current item : " + itemDef.name);
					Logger.Warn("---- associated item : " + ItemCatalog.GetItemDef(Catalog.buffToItem[buffDef.buffIndex]).name);
				}

				if (!Catalog.buffToEquip.ContainsKey(buffDef.buffIndex))
				{
					Catalog.buffToEquip.Add(buffDef.buffIndex, equipDef.equipmentIndex);
				}
				else
				{
					Logger.Warn("buffToEquip already contains key for : [" + buffDef.buffIndex + "] - " + buffDef.name);
					Logger.Warn("---- current equipment : " + equipDef.name);
					Logger.Warn("---- associated equipment : " + EquipmentCatalog.GetEquipmentDef(Catalog.buffToEquip[buffDef.buffIndex]).name);
				}

				if (!Catalog.equipToItem.ContainsKey(equipDef.equipmentIndex))
				{
					Catalog.equipToItem.Add(equipDef.equipmentIndex, itemDef.itemIndex);
				}
				else
				{
					Logger.Warn("equipToItem already contains key for : " + equipDef.name);
					Logger.Warn("---- current item : " + itemDef.name);
					Logger.Warn("---- associated item : " + ItemCatalog.GetItemDef(Catalog.equipToItem[equipDef.equipmentIndex]).name);
				}
			}
		}
	}
}
