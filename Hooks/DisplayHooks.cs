using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	internal static class DisplayHooks
	{
		internal static void Init()
		{
			CharacterOverlayHook();
			EquipmentDisplayHook();
			UpdateItemDisplayHook();
			EnableItemDisplayHook();
			DisableItemDisplayHook();
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

						if (ZetAspectsPlugin.GetEquipmentEliteDef(equipDef) == null)
						{
							if (body && body.inventory)
							{
								equipDef = EquipmentCatalog.GetEquipmentDef(body.inventory.alternateEquipmentIndex);

								if (ZetAspectsPlugin.GetEquipmentEliteDef(equipDef) == null) equipDef = null;
							}
							else
							{
								equipDef = null;
							}
						}

						if (equipDef != null && Configuration.AspectSkinEquipmentPriority.Value) return equipDef;
						if (!Configuration.AspectSkinApply.Value) return equipDef;

						if (body && body.isElite && body.inventory)
						{
							return GetEquipmentDefToDisplay(body.inventory, equipDef);
						}

						return equipDef;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects - Character Overlay Hook Failed");
				}
			};
		}

		private static EquipmentDef GetEquipmentDefToDisplay(Inventory inventory, EquipmentDef eliteEquipDef = null)
		{
			EquipmentDef targetEquipDef;
			bool eliteEquipDefNotNull = !(eliteEquipDef == null);

			if (Catalog.EliteVariety.populated)
			{
				targetEquipDef = Catalog.EliteVariety.Equipment.AffixImpPlane;
				if (targetEquipDef)
				{
					if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectImpale) > 0) return targetEquipDef;
					if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
				}
			}

			targetEquipDef = RoR2Content.Equipment.AffixLunar;
			if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectPerfect) > 0) return targetEquipDef;
			if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
			targetEquipDef = RoR2Content.Equipment.AffixPoison;
			if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectMalachite) > 0) return targetEquipDef;
			if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
			targetEquipDef = RoR2Content.Equipment.AffixHaunted;
			if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectCelestial) > 0) return targetEquipDef;
			if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;

			if (Catalog.EliteVariety.populated)
			{
				targetEquipDef = Catalog.EliteVariety.Equipment.AffixArmored;
				if (targetEquipDef)
				{
					if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectArmor) > 0) return targetEquipDef;
					if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
				}
				targetEquipDef = Catalog.EliteVariety.Equipment.AffixBuffing;
				if (targetEquipDef)
				{
					if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectBanner) > 0) return targetEquipDef;
					if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
				}
				targetEquipDef = Catalog.EliteVariety.Equipment.AffixPillaging;
				if (targetEquipDef)
				{
					if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectGolden) > 0) return targetEquipDef;
					if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
				}
				targetEquipDef = Catalog.EliteVariety.Equipment.AffixSandstorm;
				if (targetEquipDef)
				{
					if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectCyclone) > 0) return targetEquipDef;
					if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
				}
				targetEquipDef = Catalog.EliteVariety.Equipment.AffixTinkerer;
				if (targetEquipDef)
				{
					if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectTinker) > 0) return targetEquipDef;
					if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
				}
			}

			targetEquipDef = RoR2Content.Equipment.AffixRed;
			if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectFire) > 0) return targetEquipDef;
			if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
			targetEquipDef = RoR2Content.Equipment.AffixBlue;
			if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectLightning) > 0) return targetEquipDef;
			if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
			targetEquipDef = RoR2Content.Equipment.AffixWhite;
			if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectIce) > 0) return targetEquipDef;
			if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;

			return eliteEquipDef;
		}

		private static void EquipmentDisplayHook()
		{
			On.RoR2.CharacterModel.SetEquipmentDisplay += (orig, self, index) =>
			{
				index = PreventDefaultAspectEquipment(index);
				orig(self, index);
			};
		}

		private static EquipmentIndex PreventDefaultAspectEquipment(EquipmentIndex index)
		{
			if (index == EquipmentIndex.None) return index;

			EquipmentDef equipDef = EquipmentCatalog.GetEquipmentDef(index);

			EquipmentDef modEquipDef;

			if (equipDef == RoR2Content.Equipment.AffixLunar) return EquipmentIndex.None;
			if (equipDef == RoR2Content.Equipment.AffixPoison) return EquipmentIndex.None;
			if (equipDef == RoR2Content.Equipment.AffixHaunted) return EquipmentIndex.None;
			if (equipDef == RoR2Content.Equipment.AffixRed) return EquipmentIndex.None;
			if (equipDef == RoR2Content.Equipment.AffixBlue) return EquipmentIndex.None;
			if (equipDef == RoR2Content.Equipment.AffixWhite) return EquipmentIndex.None;

			if (Catalog.EliteVariety.populated)
			{
				modEquipDef = Catalog.EliteVariety.Equipment.AffixArmored;
				if (modEquipDef && modEquipDef == equipDef) return EquipmentIndex.None;
				modEquipDef = Catalog.EliteVariety.Equipment.AffixBuffing;
				if (modEquipDef && modEquipDef == equipDef) return EquipmentIndex.None;
				modEquipDef = Catalog.EliteVariety.Equipment.AffixImpPlane;
				if (modEquipDef && modEquipDef == equipDef) return EquipmentIndex.None;
				modEquipDef = Catalog.EliteVariety.Equipment.AffixPillaging;
				if (modEquipDef && modEquipDef == equipDef) return EquipmentIndex.None;
				modEquipDef = Catalog.EliteVariety.Equipment.AffixSandstorm;
				if (modEquipDef && modEquipDef == equipDef) return EquipmentIndex.None;
				modEquipDef = Catalog.EliteVariety.Equipment.AffixTinkerer;
				if (modEquipDef && modEquipDef == equipDef) return EquipmentIndex.None;
			}

			return index;
		}

		private static void UpdateItemDisplayHook()
		{
			On.RoR2.CharacterModel.UpdateItemDisplay += (orig, self, inventory) =>
			{
				orig(self, inventory);
				ApplyAspectDisplays(self, inventory);
			};
		}

		private static void ApplyAspectDisplays(CharacterModel model, Inventory inventory)
		{
			if (!model.itemDisplayRuleSet) return;

			EquipmentDef displayDef;
			EquipmentDef equipDef = EquipmentCatalog.GetEquipmentDef(inventory.currentEquipmentIndex);
			if (ZetAspectsPlugin.GetEquipmentEliteDef(equipDef) == null)
			{
				equipDef = EquipmentCatalog.GetEquipmentDef(inventory.alternateEquipmentIndex);

				if (ZetAspectsPlugin.GetEquipmentEliteDef(equipDef) == null) equipDef = null;
			}

			if (equipDef != null && Configuration.AspectSkinEquipmentPriority.Value) displayDef = equipDef;
			else if (!Configuration.AspectSkinApply.Value) displayDef = equipDef;
			else displayDef = GetEquipmentDefToDisplay(inventory, equipDef);

			HandleAspectDisplay(model, displayDef, RoR2Content.Equipment.AffixLunar, ZetAspectsContent.Items.ZetAspectPerfect);
			HandleAspectDisplay(model, displayDef, RoR2Content.Equipment.AffixPoison, ZetAspectsContent.Items.ZetAspectMalachite);
			HandleAspectDisplay(model, displayDef, RoR2Content.Equipment.AffixHaunted, ZetAspectsContent.Items.ZetAspectCelestial);
			HandleAspectDisplay(model, displayDef, RoR2Content.Equipment.AffixRed, ZetAspectsContent.Items.ZetAspectFire);
			HandleAspectDisplay(model, displayDef, RoR2Content.Equipment.AffixBlue, ZetAspectsContent.Items.ZetAspectLightning);
			HandleAspectDisplay(model, displayDef, RoR2Content.Equipment.AffixWhite, ZetAspectsContent.Items.ZetAspectIce);

			if (Catalog.EliteVariety.populated)
			{
				HandleAspectDisplay(model, displayDef, Catalog.EliteVariety.Equipment.AffixArmored, ZetAspectsContent.Items.ZetAspectArmor);
				HandleAspectDisplay(model, displayDef, Catalog.EliteVariety.Equipment.AffixBuffing, ZetAspectsContent.Items.ZetAspectBanner);
				HandleAspectDisplay(model, displayDef, Catalog.EliteVariety.Equipment.AffixImpPlane, ZetAspectsContent.Items.ZetAspectImpale);
				HandleAspectDisplay(model, displayDef, Catalog.EliteVariety.Equipment.AffixPillaging, ZetAspectsContent.Items.ZetAspectGolden);
				HandleAspectDisplay(model, displayDef, Catalog.EliteVariety.Equipment.AffixSandstorm, ZetAspectsContent.Items.ZetAspectCyclone);
				HandleAspectDisplay(model, displayDef, Catalog.EliteVariety.Equipment.AffixTinkerer, ZetAspectsContent.Items.ZetAspectTinker);
			}
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
				if (index == ZetAspectsContent.Items.ZetAspectPerfect.itemIndex) return;
				if (index == ZetAspectsContent.Items.ZetAspectMalachite.itemIndex) return;
				if (index == ZetAspectsContent.Items.ZetAspectCelestial.itemIndex) return;
				if (index == ZetAspectsContent.Items.ZetAspectFire.itemIndex) return;
				if (index == ZetAspectsContent.Items.ZetAspectLightning.itemIndex) return;
				if (index == ZetAspectsContent.Items.ZetAspectIce.itemIndex) return;

				if (Catalog.EliteVariety.populated)
				{
					if (index == ZetAspectsContent.Items.ZetAspectArmor.itemIndex) return;
					if (index == ZetAspectsContent.Items.ZetAspectBanner.itemIndex) return;
					if (index == ZetAspectsContent.Items.ZetAspectImpale.itemIndex) return;
					if (index == ZetAspectsContent.Items.ZetAspectGolden.itemIndex) return;
					if (index == ZetAspectsContent.Items.ZetAspectCyclone.itemIndex) return;
					if (index == ZetAspectsContent.Items.ZetAspectTinker.itemIndex) return;
				}

				orig(self, index);
			};
		}

		private static void DisableItemDisplayHook()
		{
			On.RoR2.CharacterModel.DisableItemDisplay += (orig, self, index) =>
			{
				if (index == ZetAspectsContent.Items.ZetAspectPerfect.itemIndex) return;
				if (index == ZetAspectsContent.Items.ZetAspectMalachite.itemIndex) return;
				if (index == ZetAspectsContent.Items.ZetAspectCelestial.itemIndex) return;
				if (index == ZetAspectsContent.Items.ZetAspectFire.itemIndex) return;
				if (index == ZetAspectsContent.Items.ZetAspectLightning.itemIndex) return;
				if (index == ZetAspectsContent.Items.ZetAspectIce.itemIndex) return;

				if (Catalog.EliteVariety.populated)
				{
					if (index == ZetAspectsContent.Items.ZetAspectArmor.itemIndex) return;
					if (index == ZetAspectsContent.Items.ZetAspectBanner.itemIndex) return;
					if (index == ZetAspectsContent.Items.ZetAspectImpale.itemIndex) return;
					if (index == ZetAspectsContent.Items.ZetAspectGolden.itemIndex) return;
					if (index == ZetAspectsContent.Items.ZetAspectCyclone.itemIndex) return;
					if (index == ZetAspectsContent.Items.ZetAspectTinker.itemIndex) return;
				}

				orig(self, index);
			};
		}
	}
}
