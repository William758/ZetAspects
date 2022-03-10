using System;
using System.Collections.Generic;
//using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Stats;
//using RoR2.UI;

namespace TPDespair.ZetAspects
{
	internal static class DisplayHooks
	{
		/*
		public static Color32 shieldConvertColor = new Color32(52, 111, 255, 255);
		public static Color32 shieldCriticalColor = new Color32(204, 219, 255, 255);
		public static Color32 shieldImmuneColor = new Color32(45, 225, 225, 255);
		public static Color32 healthImmuneColor = new Color32(225, 225, 45, 255);
		*/
		internal static Dictionary<NetworkInstanceId, EquipmentDef> BodyEliteDisplay = new Dictionary<NetworkInstanceId, EquipmentDef>();



		internal static void Init()
		{
			CharacterOverlayHook();
			EquipmentDisplayHook();
			UpdateItemDisplayHook();
			EnableItemDisplayHook();
			DisableItemDisplayHook();

			FixItemCountStatHook();

			//ShieldBarColorHook();
		}



		private static EquipmentDef GetBodyEliteDisplay(CharacterBody body)
		{
			if (BodyEliteDisplay.ContainsKey(body.netId)) return BodyEliteDisplay[body.netId];

			return null;
		}

		private static void UpdateBodyEliteDisplay(CharacterBody body)
		{
			if (EffectHooks.DestroyedBodies.ContainsKey(body.netId)) return;

			if (!BodyEliteDisplay.ContainsKey(body.netId)) BodyEliteDisplay.Add(body.netId, null);

			Inventory inventory = body.inventory;
			if (inventory)
			{
				EquipmentDef equipDef;
				bool aspectSkinItemApply = Configuration.AspectSkinItemApply.Value;

				if (Configuration.AspectSkinEquipmentPriority.Value || !aspectSkinItemApply)
				{
					equipDef = GetEliteDisplayFromEquipment(inventory);

					if (equipDef)
					{
						BodyEliteDisplay[body.netId] = equipDef;
						return;
					}
					else if (aspectSkinItemApply)
					{
						equipDef = GetEliteDisplayFromItem(inventory);

						if (equipDef)
						{
							BodyEliteDisplay[body.netId] = equipDef;
							return;
						}
					}
				}
				else
				{
					equipDef = GetEliteDisplayFromInventory(inventory);

					if (equipDef)
					{
						BodyEliteDisplay[body.netId] = equipDef;
						return;
					}
				}
			}

			BodyEliteDisplay[body.netId] = null;
		}

		private static EquipmentDef GetEliteDisplayFromInventory(Inventory inventory)
		{
			EquipmentDef currentEquipDef = EquipmentCatalog.GetEquipmentDef(inventory.currentEquipmentIndex);
			EquipmentDef alternateEquipDef = EquipmentCatalog.GetEquipmentDef(inventory.alternateEquipmentIndex);

			EquipmentDef targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixLunar;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectLunar) > 0) return targetEquipDef;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixPoison;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectPoison) > 0) return targetEquipDef;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixHaunted;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectHaunted) > 0) return targetEquipDef;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			/*
			if (Catalog.Aetherium.populated)
			{
				targetEquipDef = Catalog.Equip.AffixSanguine;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectSanguine) > 0) return targetEquipDef;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}
			*/
			targetEquipDef = Catalog.Equip.AffixEarth;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectEarth) > 0) return targetEquipDef;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixRed;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectRed) > 0) return targetEquipDef;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixBlue;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectBlue) > 0) return targetEquipDef;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixWhite;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectWhite) > 0) return targetEquipDef;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

			return null;
		}

		private static EquipmentDef GetEliteDisplayFromEquipment(Inventory inventory)
		{
			EquipmentDef currentEquipDef = EquipmentCatalog.GetEquipmentDef(inventory.currentEquipmentIndex);
			EquipmentDef alternateEquipDef = EquipmentCatalog.GetEquipmentDef(inventory.alternateEquipmentIndex);

			EquipmentDef targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixLunar;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixPoison;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixHaunted;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			/*
			if (Catalog.Aetherium.populated)
			{
				targetEquipDef = Catalog.Equip.AffixSanguine;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}
			*/
			targetEquipDef = Catalog.Equip.AffixEarth;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixRed;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixBlue;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixWhite;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

			return null;
		}

		private static EquipmentDef GetEliteDisplayFromItem(Inventory inventory)
		{
			EquipmentDef targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixLunar;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectLunar) > 0) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixPoison;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectPoison) > 0) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixHaunted;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectHaunted) > 0) return targetEquipDef;
			/*
			if (Catalog.Aetherium.populated)
			{
				targetEquipDef = Catalog.Equip.AffixSanguine;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectSanguine) > 0) return targetEquipDef;
			}
			*/
			targetEquipDef = Catalog.Equip.AffixEarth;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectEarth) > 0) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixRed;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectRed) > 0) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixBlue;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectBlue) > 0) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixWhite;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectWhite) > 0) return targetEquipDef;

			return null;
		}



		private static void CharacterOverlayHook()
		{
			IL.RoR2.CharacterModel.UpdateOverlays += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterModel>("inventoryEquipmentIndex"),
					x => x.MatchCall("RoR2.EquipmentCatalog", "GetEquipmentDef"),
					x => x.MatchStloc(0)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<EquipmentDef, CharacterModel, EquipmentDef>>((equipDef, model) =>
					{
						CharacterBody body = model.body;
						if (body)
						{
							EquipmentDef equipDefFromBody = GetBodyEliteDisplay(body);
							if (equipDefFromBody) equipDef = equipDefFromBody;
						}

						return equipDef;
					});
				}
				else
				{
					Logger.Warn("Character Overlay Hook Failed");
				}
			};
		}

		private static void EquipmentDisplayHook()
		{
			On.RoR2.CharacterModel.SetEquipmentDisplay += (orig, self, index) =>
			{
				if (Catalog.aspectEquipIndexes.Contains(index)) index = EquipmentIndex.None;

				orig(self, index);
			};
		}

		private static void UpdateItemDisplayHook()
		{
			On.RoR2.CharacterModel.UpdateItemDisplay += (orig, self, inventory) =>
			{
				orig(self, inventory);

				ApplyAspectDisplays(self);
			};
		}

		private static void ApplyAspectDisplays(CharacterModel model)
		{
			if (!model.itemDisplayRuleSet) return;

			EquipmentDef displayDef = null;
			CharacterBody body = model.body;
			if (body)
			{
				UpdateBodyEliteDisplay(body);

				EquipmentDef equipDefFromBody = GetBodyEliteDisplay(body);
				if (equipDefFromBody) displayDef = equipDefFromBody;
			}

			HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixLunar, Catalog.Item.ZetAspectLunar);
			HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixPoison, Catalog.Item.ZetAspectPoison);
			HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixHaunted, Catalog.Item.ZetAspectHaunted);
			HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixRed, Catalog.Item.ZetAspectRed);
			HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixBlue, Catalog.Item.ZetAspectBlue);
			HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixWhite, Catalog.Item.ZetAspectWhite);

			HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixEarth, Catalog.Item.ZetAspectEarth);
			/*
			if (Catalog.Aetherium.Enabled)
			{
				HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixSanguine, Catalog.Item.ZetAspectSanguine);
			}
			*/
		}

		private static void HandleAspectDisplay(CharacterModel model, EquipmentDef display, EquipmentDef target, ItemDef item)
		{
			ItemMask list = model.enabledItemDisplays;
			ItemIndex index = item.itemIndex;

			if (!target) return;

			if (display == target)
			{
				if (!list.Contains(index))
				{
					list.Add(index);
					DisplayRuleGroup drg = model.itemDisplayRuleSet.GetEquipmentDisplayRuleGroup(target.equipmentIndex);
					model.InstantiateDisplayRuleGroup(drg, index, EquipmentIndex.None);
				}
			}
			else
			{
				if (list.Contains(index))
				{
					list.Remove(index);
					RemoveAspectDisplay(model, index);
				}
			}
		}

		private static void RemoveAspectDisplay(CharacterModel model, ItemIndex index)
		{
			for (int i = model.parentedPrefabDisplays.Count - 1; i >= 0; i--)
			{
				if (model.parentedPrefabDisplays[i].itemIndex == index)
				{
					model.parentedPrefabDisplays[i].Undo();
					model.parentedPrefabDisplays.RemoveAt(i);
				}
			}
			for (int j = model.limbMaskDisplays.Count - 1; j >= 0; j--)
			{
				if (model.limbMaskDisplays[j].itemIndex == index)
				{
					model.limbMaskDisplays[j].Undo(model);
					model.limbMaskDisplays.RemoveAt(j);
				}
			}
		}

		private static void EnableItemDisplayHook()
		{
			On.RoR2.CharacterModel.EnableItemDisplay += (orig, self, index) =>
			{
				if (Catalog.aspectItemIndexes.Contains(index)) return;

				orig(self, index);
			};
		}

		private static void DisableItemDisplayHook()
		{
			On.RoR2.CharacterModel.DisableItemDisplay += (orig, self, index) =>
			{
				if (Catalog.aspectItemIndexes.Contains(index)) return;

				orig(self, index);
			};
		}



		private static void FixItemCountStatHook()
		{
			IL.RoR2.Stats.StatManager.ProcessItemCollectedEvents += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(0),
					x => x.MatchLdfld("RoR2.Stats.StatManager/ItemCollectedEvent", "quantity")
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Ldloc, 0);
					c.EmitDelegate<Func<int, StatManager.ItemCollectedEvent, int>>((count, ice) =>
					{
						ItemDef itemDef = ItemCatalog.GetItemDef(ice.itemIndex);
						if (itemDef.hidden) return 0;
						return count;
					});
				}
				else
				{
					Logger.Warn("FixItemCountStatHook Failed");
				}
			};
		}


		/*
		private static void ShieldBarColorHook()
		{
			On.RoR2.UI.HealthBar.UpdateBarInfos += (orig, self) =>
			{
				orig(self);

				int shieledColor = GetShieldColorIndex(self);
				if (shieledColor != 0)
				{
					if (shieledColor == 3)
					{
						SetShieldColor(self, shieldImmuneColor);
						SetHealthColor(self, healthImmuneColor);
					}
					if (shieledColor == 2) SetShieldColor(self, shieldCriticalColor);
					if (shieledColor == 1) SetShieldColor(self, shieldConvertColor);
				}
			};
		}

		private static int GetShieldColorIndex(HealthBar hpBar)
		{
			int output = 0;

			if (hpBar.healthCritical && hpBar.style.flashOnHealthCritical)
			{
				if (Mathf.FloorToInt(Time.fixedTime * 10f) % 2 == 0) output = 2;
			}

			HealthComponent hc = hpBar._source;
			if (hc)
			{
				CharacterBody body = hc.body;
				if (body)
				{
					if (Catalog.immuneHealth)
					{
						if (hc.godMode || body.HasBuff(RoR2Content.Buffs.HiddenInvincibility) || body.HasBuff(RoR2Content.Buffs.Immune)) output = Math.Max(output, 3);
					}

					if (body.HasBuff(RoR2Content.Buffs.AffixLunar))
					{
						output = Math.Max(output, 1);
					}
					else
					{
						Inventory inventory = body.inventory;
						if (inventory)
						{
							int count = inventory.GetItemCount(RoR2Content.Items.ShieldOnly);
							if (count > 0) output = Math.Max(output, 1);
						}
					}
				}
			}

			return output;
		}

		private static void SetShieldColor(HealthBar hpBar, Color32 color)
		{
			hpBar.barInfoCollection.shieldBarInfo.color = color;
		}

		private static void SetHealthColor(HealthBar hpBar, Color32 color)
		{
			hpBar.barInfoCollection.healthBarInfo.color = color;
		}
		*/
	}
}
