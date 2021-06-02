using System;
using System.Linq;
using System.Text.RegularExpressions;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace TPDespair.ZetAspects
{
	internal class ZetAspectsConvertHandler : MonoBehaviour, IPointerClickHandler
	{
		public Func<Inventory> GetInventory { get; set; }
		public Func<PlayerCharacterMasterController> GetPCMC { get; set; }

		public void OnPointerClick(PointerEventData eventData)
		{
			DropHooks.HandleConvertPointerClick(this);
		}
	}



	internal static class DropHooks
	{
		internal static void Init()
		{
			ResetRunDropCountHook();
			DropChanceHook();
			InterceptAspectDropHook();
			EquipmentAbsorbHook();
			EquipmentConvertHook();
			EquipmentIconHook();
		}



		private static void ResetRunDropCountHook()
		{
			On.RoR2.Run.Start += (orig, self) =>
			{
				ZetAspectsPlugin.RunDropCount = 0;
				Debug.LogWarning("ZetAspects - RunDropCount : 0");

				orig(self);
			};

			On.RoR2.Run.OnDestroy += (orig, self) =>
			{
				ZetAspectsPlugin.RunDropCount = 0;
				Debug.LogWarning("ZetAspects - RunDropCount : 0");

				orig(self);
			};
		}

		private static bool CheckDropRoll(float chance, CharacterMaster master)
		{
			if (chance <= 0f) return false;

			float luck = master ? master.luck : 0f;

			int count = Mathf.CeilToInt(Mathf.Abs(luck));
			float roll = UnityEngine.Random.Range(0f, 100f);

			for (int i = 0; i < count; i++)
			{
				float r = UnityEngine.Random.Range(0f, 100f);
				roll = (luck > 0f) ? Mathf.Min(roll, r) : Mathf.Max(roll, r);
			}

			return roll <= chance;
		}

		private static void DropChanceHook()
		{
			IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchPop(),
					x => x.MatchLdcR4(0.025f),
					x => x.MatchLdloc(14),
					x => x.MatchCall("RoR2.Util", "CheckRoll")
				);

				if (found)
				{
					c.Index += 4;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc, 14);
					c.Emit(OpCodes.Ldloc, 10);
					c.EmitDelegate<Func<CharacterMaster, EquipmentIndex, bool>>((master, index) =>
					{
						if (index == EquipmentIndex.None) return false;

						float chance = Configuration.AspectDropChance.Value;
						if (chance <= 0f) return false;

						float decay = Mathf.Abs(Configuration.AspectDropChanceDecay.Value);
						if (decay < 1f) chance *= Mathf.Pow(decay, ZetAspectsPlugin.RunDropCount);
						if (CheckDropRoll(chance, master))
						{
							ZetAspectsPlugin.RunDropCount++;
							Debug.LogWarning("ZetAspects - RunDropCount : " + ZetAspectsPlugin.RunDropCount);
							return true;
						}

						return false;
					});

					found = c.TryGotoNext(
						x => x.MatchLdloc(2),
						x => x.MatchCallOrCallvirt<CharacterBody>("get_isElite")
					);

					if (found)
					{
						c.Index += 2;

						c.EmitDelegate<Func<bool, bool>>((isElite) =>
						{
							if (isElite)
							{
								if (Configuration.AspectShowDropText.Value)
								{
									Chat.SendBroadcastChat(new Chat.SimpleChatMessage
									{
										baseToken = "<color=#DDDDA0>" + Configuration.AspectDropText.Value + "</color>"
									});
								}
							}

							return isElite;
						});
					}
					else
					{
						Debug.LogWarning("ZetAspects - Elite Aspect Drop Message Hook Failed");
					}
				}
				else
				{
					Debug.LogWarning("ZetAspects - Elite Aspect Drop Chance And Message Hook Failed");
				}
			};
		}

		private static void InterceptAspectDropHook()
		{
			On.RoR2.PickupDropletController.CreatePickupDroplet += (orig, pickupIndex, position, velocity) =>
			{
				if (!Configuration.AspectEliteEquipment.Value)
				{
					PickupIndex newIndex = PickupIndex.none;

					if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Equipment.AffixWhite.equipmentIndex))
					{
						newIndex = PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectIce.itemIndex);
					}
					else if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Equipment.AffixBlue.equipmentIndex))
					{
						newIndex = PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectLightning.itemIndex);
					}
					else if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Equipment.AffixRed.equipmentIndex))
					{
						newIndex = PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectFire.itemIndex);
					}
					else if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Equipment.AffixHaunted.equipmentIndex))
					{
						newIndex = PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectCelestial.itemIndex);
					}
					else if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Equipment.AffixPoison.equipmentIndex))
					{
						newIndex = PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectMalachite.itemIndex);
					}
					else if (pickupIndex == PickupCatalog.FindPickupIndex(RoR2Content.Equipment.AffixLunar.equipmentIndex))
					{
						newIndex = PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectPerfect.itemIndex);
					}

					if (newIndex != PickupIndex.none) pickupIndex = newIndex;
				}

				orig(pickupIndex, position, velocity);
			};
		}

		private static void EquipmentAbsorbHook()
		{
			IL.RoR2.GenericPickupController.AttemptGrant += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdcI4(1),
					x => x.MatchStfld<GenericPickupController>("consumed"),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<GenericPickupController>("pickupIndex")
				);

				if (found)
				{
					c.Index += 7;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldarg, 1);
					c.Emit(OpCodes.Ldloc, 2);
					c.EmitDelegate<Func<GenericPickupController, CharacterBody, PickupDef, PickupDef>>((gpc, body, pickupDef) =>
					{
						if (!Configuration.AspectEquipmentAbsorb.Value) return pickupDef;

						EquipmentIndex pickupEquip = pickupDef.equipmentIndex;

						if (pickupEquip != EquipmentIndex.None)
						{
							EquipmentIndex currentEquip = body.inventory.currentEquipmentIndex;
							PickupIndex newIndex = PickupIndex.none;

							if (pickupEquip == RoR2Content.Equipment.AffixWhite.equipmentIndex)
							{
								if (currentEquip == pickupEquip)
								{
									newIndex = PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectIce.itemIndex);
								}
							}
							else if (pickupEquip == RoR2Content.Equipment.AffixBlue.equipmentIndex)
							{
								if (currentEquip == pickupEquip)
								{
									newIndex = PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectLightning.itemIndex);
								}
							}
							else if (pickupEquip == RoR2Content.Equipment.AffixRed.equipmentIndex)
							{
								if (currentEquip == pickupEquip)
								{
									newIndex = PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectFire.itemIndex);
								}
							}
							else if (pickupEquip == RoR2Content.Equipment.AffixHaunted.equipmentIndex)
							{
								if (currentEquip == pickupEquip)
								{
									newIndex = PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectCelestial.itemIndex);
								}
							}
							else if (pickupEquip == RoR2Content.Equipment.AffixPoison.equipmentIndex)
							{
								if (currentEquip == pickupEquip)
								{
									newIndex = PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectMalachite.itemIndex);
								}
							}
							else if (pickupEquip == RoR2Content.Equipment.AffixLunar.equipmentIndex)
							{
								if (currentEquip == pickupEquip)
								{
									newIndex = PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectPerfect.itemIndex);
								}
							}

							if (newIndex != PickupIndex.none)
							{
								gpc.pickupIndex = newIndex;
								return PickupCatalog.GetPickupDef(newIndex);
							}
						}

						return pickupDef;
					});
					c.Emit(OpCodes.Stloc, 2);
				}
				else
				{
					Debug.LogWarning("ZetAspects - Equipment Absorb Hook Failed");
				}
			};
		}

		private static void EquipmentConvertHook()
		{
			Chat.onChatChanged += delegate ()
			{
				if (!NetworkServer.active) return;

				if (!Configuration.AspectEquipmentConversion.Value) return;

				string message = Chat.log.DefaultIfEmpty(null).Last();
				if (message == null) return;
				Chat.UserChatMessage userChatMessage = ReconstructMessage(message);
				if (userChatMessage == null) return;

				if (userChatMessage.text.ToLower() == "convertaspect")
				{
					CharacterMaster master = userChatMessage.sender.GetComponent<NetworkUser>().master;
					if (!master) return;

					Inventory inventory = master.inventory;
					if (inventory)
					{
						ItemIndex itemIndex = ZetAspectsPlugin.GetEquipmentAspectIndex(inventory.currentEquipmentIndex);
						if (itemIndex != ItemIndex.None)
						{
							inventory.SetEquipmentIndex(EquipmentIndex.None);
							inventory.GiveItem(itemIndex);
						}
					}
				}
			};
		}

		private static Chat.UserChatMessage ReconstructMessage(string message)
		{
			Match match = Regex.Match(message, "<color=#e5eefc><noparse>(.+?)<\\/noparse>: <noparse>(.+?)<\\/noparse><\\/color>", RegexOptions.Compiled);
			if (!match.Success || match.Groups.Count < 3) return null;

			GameObject gameObject = null;
			foreach (NetworkUser networkUser in NetworkUser.readOnlyInstancesList)
			{
				if (networkUser.userName == match.Groups[1].Value.Trim())
				{
					gameObject = networkUser.gameObject;
					break;
				}
			}

			if (gameObject == null) return null;

			return new Chat.UserChatMessage
			{
				sender = gameObject,
				text = match.Groups[2].Value.Trim()
			};
		}

		private static void EquipmentIconHook()
		{
			On.RoR2.UI.HUD.Update += (orig, self) =>
			{
				orig(self);

				if (self.equipmentIcons.Length > 0)
				{
					AttachZetAspectsConvertHandler(self.equipmentIcons[0]);
				}
			};
		}

		private static void AttachZetAspectsConvertHandler(EquipmentIcon icon)
		{
			if (icon.GetComponent<ZetAspectsConvertHandler>() != null) return;

			ZetAspectsConvertHandler handler = icon.transform.gameObject.AddComponent<ZetAspectsConvertHandler>();
			handler.GetInventory = () => icon.targetInventory;
			handler.GetPCMC = () => icon.playerCharacterMasterController;
		}

		internal static void HandleConvertPointerClick(ZetAspectsConvertHandler handler)
		{
			if (!Configuration.AspectEquipmentConversion.Value) return;

			Inventory inventory = handler.GetInventory();
			if (!inventory) return;
			if (!inventory.hasAuthority) return;

			ItemIndex itemIndex = ZetAspectsPlugin.GetEquipmentAspectIndex(inventory.currentEquipmentIndex);
			if (itemIndex != ItemIndex.None) 
			{
				PlayerCharacterMasterController pcmc = handler.GetPCMC();
				if (!pcmc) return;
				NetworkUser netUser = pcmc.networkUser;
				if (!netUser) return;
				GameObject gameObject = netUser.gameObject;
				if (!gameObject) return;

				Chat.SendBroadcastChat(
					new Chat.UserChatMessage { sender = gameObject, text = "ConvertAspect" }
				);
			}
		}
	}
}
