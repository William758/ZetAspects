using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Stats;
using RoR2.UI;

namespace TPDespair.ZetAspects
{
	// this is attached to a buff display so that our changes only happens once
	internal class ZetAspectsBuffIconMarker : MonoBehaviour
	{
		
	}



	internal static class DisplayHooks
	{
		public static Color32 shieldVoidColor = new Color32(255, 75, 210, 255);

		public static Color32 shieldConvertColor = new Color32(50, 100, 255, 255);
		public static Color32 shieldConvertVoidColor = new Color32(255, 50, 150, 255);

		public static Color32 shieldCriticalColor = new Color32(200, 200, 200, 255);

		public static Color32 shieldImmuneColor = new Color32(55, 205, 225, 255);
		public static Color32 shieldImmuneVoidColor = new Color32(205, 55, 255, 255);
		public static Color32 healthImmuneColor = new Color32(225, 225, 45, 255);
		
		internal static Dictionary<NetworkInstanceId, EquipmentDef> BodyEliteDisplay = new Dictionary<NetworkInstanceId, EquipmentDef>();



		internal static void Init()
		{
			CharacterOverlayHook();
			EquipmentDisplayHook();
			UpdateItemDisplayHook();
			EnableItemDisplayHook();
			DisableItemDisplayHook();

			FixItemCountStatHook();
			FixDamageDealtStatHook();

			if (Configuration.RecolorHpBar.Value) HPBarColorHook();

			BuffIconDisplayHook();
		}



		internal static void OnBodyDeath(CharacterBody body)
		{
			if (GetBodyEliteDisplay(body) == Catalog.Equip.AffixVoid)
			{
				ModelLocator locator = body.GetComponent<ModelLocator>();
				if (locator && locator.modelTransform)
				{
					CharacterModel model = locator.modelTransform.GetComponent<CharacterModel>();
					if (model)
					{
						ApplyAspectDisplays(model, true);
						//model.UpdateOverlays();
					}
				}
			}
		}



		private static EquipmentDef GetBodyEliteDisplay(CharacterBody body)
		{
			if (BodyEliteDisplay.ContainsKey(body.netId)) return BodyEliteDisplay[body.netId];

			return null;
		}

		private static void UpdateBodyEliteDisplay(CharacterBody body, bool isDead = false)
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
					equipDef = GetEliteDisplayFromEquipment(inventory, isDead);

					if (equipDef)
					{
						BodyEliteDisplay[body.netId] = equipDef;
						return;
					}
					else if (aspectSkinItemApply)
					{
						equipDef = GetEliteDisplayFromItem(inventory, isDead);

						if (equipDef)
						{
							BodyEliteDisplay[body.netId] = equipDef;
							return;
						}
					}
				}
				else
				{
					equipDef = GetEliteDisplayFromInventory(inventory, isDead);

					if (equipDef)
					{
						BodyEliteDisplay[body.netId] = equipDef;
						return;
					}
				}
			}

			BodyEliteDisplay[body.netId] = null;
		}

		private static EquipmentDef GetEliteDisplayFromInventory(Inventory inventory, bool isDead = false)
		{
			EquipmentDef currentEquipDef = EquipmentCatalog.GetEquipmentDef(inventory.currentEquipmentIndex);
			EquipmentDef alternateEquipDef = EquipmentCatalog.GetEquipmentDef(inventory.alternateEquipmentIndex);

			EquipmentDef targetEquipDef;

			if (Catalog.Blighted.populated)
			{
				targetEquipDef = Catalog.Equip.AffixBlighted;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectBlighted) > 0) return targetEquipDef;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}

			if (!isDead)
			{
				targetEquipDef = Catalog.Equip.AffixVoid;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectVoid) > 0) return targetEquipDef;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}

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
			
			if (Catalog.Aetherium.populated)
			{
				targetEquipDef = Catalog.Equip.AffixSanguine;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectSanguine) > 0) return targetEquipDef;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}

			if (Catalog.WarWisp.populated)
			{
				targetEquipDef = Catalog.Equip.AffixNullifier;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectNullifier) > 0) return targetEquipDef;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}

			if (Catalog.SpikeStrip.populated)
			{
				targetEquipDef = Catalog.Equip.AffixAragonite;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectAragonite) > 0) return targetEquipDef;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

				targetEquipDef = Catalog.Equip.AffixVeiled;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectVeiled) > 0) return targetEquipDef;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

				targetEquipDef = Catalog.Equip.AffixWarped;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectWarped) > 0) return targetEquipDef;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

				targetEquipDef = Catalog.Equip.AffixPlated;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectPlated) > 0) return targetEquipDef;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}

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

			if (Catalog.Bubbet.populated)
			{
				targetEquipDef = Catalog.Equip.AffixSepia;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectSepia) > 0) return targetEquipDef;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}

			if (Catalog.GoldenCoastPlus.populated)
			{
				targetEquipDef = Catalog.Equip.AffixGold;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectGold) > 0) return targetEquipDef;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}

			return null;
		}

		private static EquipmentDef GetEliteDisplayFromEquipment(Inventory inventory, bool isDead = false)
		{
			EquipmentDef currentEquipDef = EquipmentCatalog.GetEquipmentDef(inventory.currentEquipmentIndex);
			EquipmentDef alternateEquipDef = EquipmentCatalog.GetEquipmentDef(inventory.alternateEquipmentIndex);

			EquipmentDef targetEquipDef;

			if (Catalog.Blighted.populated)
			{
				targetEquipDef = Catalog.Equip.AffixBlighted;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}

			if (!isDead)
			{
				targetEquipDef = Catalog.Equip.AffixVoid;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}

			targetEquipDef = Catalog.Equip.AffixLunar;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixPoison;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixHaunted;
			if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
			if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			
			if (Catalog.Aetherium.populated)
			{
				targetEquipDef = Catalog.Equip.AffixSanguine;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}

			if (Catalog.WarWisp.populated)
			{
				targetEquipDef = Catalog.Equip.AffixNullifier;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}

			if (Catalog.SpikeStrip.populated)
			{
				targetEquipDef = Catalog.Equip.AffixAragonite;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

				targetEquipDef = Catalog.Equip.AffixVeiled;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

				targetEquipDef = Catalog.Equip.AffixWarped;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;

				targetEquipDef = Catalog.Equip.AffixPlated;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}

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

			if (Catalog.Bubbet.populated)
			{
				targetEquipDef = Catalog.Equip.AffixSepia;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}

			if (Catalog.GoldenCoastPlus.populated)
			{
				targetEquipDef = Catalog.Equip.AffixGold;
				if (currentEquipDef && currentEquipDef == targetEquipDef) return targetEquipDef;
				if (alternateEquipDef && alternateEquipDef == targetEquipDef) return targetEquipDef;
			}

			return null;
		}

		private static EquipmentDef GetEliteDisplayFromItem(Inventory inventory, bool isDead = false)
		{
			EquipmentDef targetEquipDef;

			if (Catalog.Blighted.populated)
			{
				targetEquipDef = Catalog.Equip.AffixBlighted;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectBlighted) > 0) return targetEquipDef;
			}

			if (!isDead)
			{
				targetEquipDef = Catalog.Equip.AffixVoid;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectVoid) > 0) return targetEquipDef;
			}

			targetEquipDef = Catalog.Equip.AffixLunar;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectLunar) > 0) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixPoison;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectPoison) > 0) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixHaunted;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectHaunted) > 0) return targetEquipDef;
			
			if (Catalog.Aetherium.populated)
			{
				targetEquipDef = Catalog.Equip.AffixSanguine;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectSanguine) > 0) return targetEquipDef;
			}

			if (Catalog.WarWisp.populated)
			{
				targetEquipDef = Catalog.Equip.AffixNullifier;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectNullifier) > 0) return targetEquipDef;
			}

			if (Catalog.SpikeStrip.populated)
			{
				targetEquipDef = Catalog.Equip.AffixAragonite;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectAragonite) > 0) return targetEquipDef;

				targetEquipDef = Catalog.Equip.AffixVeiled;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectVeiled) > 0) return targetEquipDef;

				targetEquipDef = Catalog.Equip.AffixWarped;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectWarped) > 0) return targetEquipDef;

				targetEquipDef = Catalog.Equip.AffixPlated;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectPlated) > 0) return targetEquipDef;
			}

			targetEquipDef = Catalog.Equip.AffixEarth;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectEarth) > 0) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixRed;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectRed) > 0) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixBlue;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectBlue) > 0) return targetEquipDef;

			targetEquipDef = Catalog.Equip.AffixWhite;
			if (inventory.GetItemCount(Catalog.Item.ZetAspectWhite) > 0) return targetEquipDef;

			if (Catalog.Bubbet.populated)
			{
				targetEquipDef = Catalog.Equip.AffixSepia;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectSepia) > 0) return targetEquipDef;
			}

			if (Catalog.GoldenCoastPlus.populated)
			{
				targetEquipDef = Catalog.Equip.AffixGold;
				if (inventory.GetItemCount(Catalog.Item.ZetAspectGold) > 0) return targetEquipDef;
			}

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

							//if (equipDef && !equipDefFromBody && equipDef == Catalog.Equip.AffixVoid) return null;
							if (equipDefFromBody) return equipDefFromBody;
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

		private static void ApplyAspectDisplays(CharacterModel model, bool isDead = false)
		{
			if (!model.itemDisplayRuleSet) return;

			EquipmentDef displayDef = null;
			CharacterBody body = model.body;
			if (body)
			{
				UpdateBodyEliteDisplay(body, isDead);

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
			HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixVoid, Catalog.Item.ZetAspectVoid);

			if (Catalog.SpikeStrip.Enabled)
			{
				HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixAragonite, Catalog.Item.ZetAspectAragonite);
				HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixVeiled, Catalog.Item.ZetAspectVeiled);
				HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixWarped, Catalog.Item.ZetAspectWarped);
				HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixPlated, Catalog.Item.ZetAspectPlated);
			}

			if (Catalog.GoldenCoastPlus.Enabled)
			{
				HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixGold, Catalog.Item.ZetAspectGold);
			}
			
			if (Catalog.Aetherium.Enabled)
			{
				HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixSanguine, Catalog.Item.ZetAspectSanguine);
			}

			if (Catalog.Bubbet.Enabled)
			{
				HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixSepia, Catalog.Item.ZetAspectSepia);
			}

			if (Catalog.WarWisp.Enabled)
			{
				HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixNullifier, Catalog.Item.ZetAspectNullifier);
			}

			if (Catalog.Blighted.Enabled)
			{
				HandleAspectDisplay(model, displayDef, Catalog.Equip.AffixBlighted, Catalog.Item.ZetAspectBlighted);
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

		private static void FixDamageDealtStatHook()
		{
			IL.RoR2.Stats.StatManager.ProcessDamageEvents += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(1)
				);

				if (found)
				{
					c.Emit(OpCodes.Ldloc, 0);
					c.EmitDelegate<Func<ulong, StatManager.DamageEvent, ulong>>((value, damageEvent) =>
					{
						ulong damageLimit = 10000000u;

						CharacterMaster atkMaster = damageEvent.attackerMaster;
						if (atkMaster)
						{
							CharacterBody atkBody = atkMaster.GetBody();
							if (atkBody)
							{
								// ignore damage values over 10 million times higher than body basedamage
								damageLimit = (ulong)(1e7 * (double)atkBody.damage);
							}
						}

						if (value > damageLimit)
						{
							return 0u;
						}
						else
						{
							return value;
						}
					});
				}
				else
				{
					Logger.Warn("FixDamageDealtStatHook Failed");
				}
			};
		}



		private static void HPBarColorHook()
		{
			On.RoR2.UI.HealthBar.UpdateBarInfos += (orig, self) =>
			{
				orig(self);

				HPBarColorOverride(self);
			};
		}

		private static void HPBarColorOverride(HealthBar hpBar)
		{
			bool critical = false, immune = false, convertShield = false, voidShield = false;

			if (hpBar.healthCritical && hpBar.style.flashOnHealthCritical)
			{
				if (Mathf.FloorToInt(Time.fixedTime * 10f) % 2 == 0) critical = true;
			}

			HealthComponent healthComponent = hpBar._source;
			if (healthComponent)
			{
				if (healthComponent.godMode) immune = true;

				CharacterBody body = healthComponent.body;
				if (body)
				{
					if (body.HasBuff(RoR2Content.Buffs.HiddenInvincibility) || body.HasBuff(RoR2Content.Buffs.Immune)) immune = true;

					if (body.HasBuff(RoR2Content.Buffs.AffixLunar)) convertShield = true;

					if (body.HasBuff(Catalog.Buff.AffixNullifier) && Configuration.AspectNullifierRegen.Value > 0.001f) convertShield = true;

					Inventory inventory = body.inventory;
					if (inventory)
					{
						int count = inventory.GetItemCount(RoR2Content.Items.ShieldOnly);
						if (count > 0) convertShield = true;

						count = inventory.GetItemCount(DLC1Content.Items.MissileVoid);
						if (count > 0) voidShield = true;
					}
				}
			}

			if (critical)
			{
				SetShieldColor(hpBar, shieldCriticalColor);
			}
			else if (immune && (Catalog.immuneHealth || Configuration.RecolorImmuneBar.Value))
			{
				SetHealthColor(hpBar, healthImmuneColor);

				if (voidShield) SetShieldColor(hpBar, shieldImmuneVoidColor);
				else SetShieldColor(hpBar, shieldImmuneColor);
			}
			else if (convertShield && Configuration.RecolorShieldConvertBar.Value)
			{
				if (voidShield) SetShieldColor(hpBar, shieldConvertVoidColor);
				else SetShieldColor(hpBar, shieldConvertColor);
			}
			else if (voidShield)
			{
				SetShieldColor(hpBar, shieldVoidColor);
			}
		}

		private static void SetShieldColor(HealthBar hpBar, Color32 color)
		{
			hpBar.barInfoCollection.shieldBarInfo.color = color;
		}

		private static void SetHealthColor(HealthBar hpBar, Color32 color)
		{
			hpBar.barInfoCollection.trailingOverHealthbarInfo.color = color;
		}



		private static void BuffIconDisplayHook()
		{
			On.RoR2.UI.BuffIcon.UpdateIcon += (orig, self) =>
			{
				orig(self);

				BuffDef elusiveBuff = Catalog.Buff.ZetElusive;
				if (self.buffDef && elusiveBuff != null && self.buffDef == elusiveBuff)
				{
					BuffIcon.sharedStringBuilder.Clear();
					BuffIcon.sharedStringBuilder.AppendInt(Mathf.Max(self.buffCount * 5, 25), 1U, uint.MaxValue);
					BuffIcon.sharedStringBuilder.Append("%");

					ZetAspectsBuffIconMarker marker = self.stackCount.GetComponent<ZetAspectsBuffIconMarker>();
					if (!marker)
					{
						self.stackCount.gameObject.AddComponent<ZetAspectsBuffIconMarker>();
						self.stackCount.fontSize *= 0.8f;
					}

					self.stackCount.SetText(BuffIcon.sharedStringBuilder);
				}
			};
		}
	}
}
