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
		public static bool disableDrops = false;
		public static int runDropCount = 0;

		internal static void Init()
		{
			SceneDirector.onPostPopulateSceneServer += OnScenePopulated;
			SceneExitController.onBeginExit += OnSceneExit;

			ResetRunDropCountHook();
			DropChanceHook();
			InterceptAspectDropHook();
			EquipmentAbsorbHook();
			DiscoverAspectHook();
			EquipmentConvertHook();
			EquipmentIconHook();
		}



		private static void OnScenePopulated(SceneDirector sceneDirector)
		{
			if (Run.instance)
			{
				UpdateRunDropCount();
				UpdateZetDropTracker();

				disableDrops = false;
				Debug.LogWarning("ZetAspects - ScenePopulated : Drops Enabled");

				Debug.LogWarning("ZetAspects - RunDropCount : " + runDropCount);
				LogDropChance();
			}
		}

		private static void UpdateRunDropCount()
		{
			int highestCount = 0;

			foreach (PlayerCharacterMasterController pcmc in PlayerCharacterMasterController.instances)
			{
				CharacterMaster master = pcmc.master;
				if (master)
				{
					Inventory inventory = master.inventory;
					if (inventory) highestCount = Math.Max(highestCount, inventory.GetItemCount(ZetAspectsContent.Items.ZetDropTracker));
				}
			}

			runDropCount = Math.Max(runDropCount, highestCount);
		}

		private static void UpdateZetDropTracker()
		{
			foreach (PlayerCharacterMasterController pcmc in PlayerCharacterMasterController.instances)
			{
				CharacterMaster master = pcmc.master;
				if (master)
				{
					Inventory inventory = master.inventory;
					if (inventory)
					{
						int trackerCount = inventory.GetItemCount(ZetAspectsContent.Items.ZetDropTracker);
						int dropCount = runDropCount;

						if (trackerCount < dropCount) inventory.GiveItem(ZetAspectsContent.Items.ZetDropTracker, dropCount - trackerCount);
					}
				}
			}
		}

		private static void OnSceneExit(SceneExitController sceneExitController)
		{
			if (Run.instance)
			{
				disableDrops = true;
				Debug.LogWarning("ZetAspects - SceneExit : Drops Disabled");
			}
		}



		private static void ResetRunDropCountHook()
		{
			On.RoR2.Run.Start += (orig, self) =>
			{
				runDropCount = 0;

				orig(self);
			};

			On.RoR2.Run.OnDestroy += (orig, self) =>
			{
				runDropCount = 0;

				orig(self);
			};
		}



		private static void LogDropChance()
		{
			float baseChance = Configuration.AspectDropChance.Value;

			float decayMult = DecayChanceFactor();
			float playerMult = PlayerChanceFactor();
			float stageMult = StageChanceFactor();

			float currentChance = baseChance * decayMult * playerMult * stageMult;

			string output = "ZetAspects - DropChance : ";
			output += $"{currentChance:0.###}% : ";
			output += "[base]" + $"{baseChance:0.###}% x ";
			output += "[decay]" + $"{decayMult:0.###} x ";
			output += "[player]" + $"{playerMult:0.###} x ";
			output += "[stage]" + $"{stageMult:0.###}";

			Debug.LogWarning(output);
		}

		private static float GetDropChance()
		{
			float chance = Configuration.AspectDropChance.Value;

			chance *= DecayChanceFactor();
			chance *= PlayerChanceFactor();
			chance *= StageChanceFactor();

			return chance;
		}

		private static float DecayChanceFactor()
		{
			float decay = Mathf.Abs(Configuration.AspectDropChanceDecay.Value);
			if (decay < 1f) return Mathf.Max(Configuration.AspectDropChanceDecayLimit.Value, Mathf.Pow(decay, runDropCount));
			return 1f;
		}

		private static float PlayerChanceFactor()
		{
			float count = PlayerCharacterMasterController.instances.Count * Mathf.Abs(Configuration.AspectDropChanceMultiplayerFactor.Value);
			return Mathf.Max(0.001f, Mathf.Pow(count, Configuration.AspectDropChanceMultiplayerExponent.Value));
		}

		private static float StageChanceFactor()
		{
			if (!Configuration.AspectDropChanceCompensation.Value) return 1f;

			if (Run.instance)
			{
				int stagesCleared = Run.instance.stageClearCount;
				int dropCount = runDropCount;

				if (dropCount < 3)
				{
					if (stagesCleared >= (dropCount + 1) * 3) return 8f;
				}
				else
				{
					if (stagesCleared >= (dropCount * 5) - 1) return 8f;
				}
			}

			return 1f;
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
						if (ZetAspectsPlugin.GetEquipmentEliteDef(EquipmentCatalog.GetEquipmentDef(index)) == null) return false;

						// prevent blacklisted equipment from dropping
						EquipmentDef equipDef = Catalog.LostInTransit.Equipment.AffixBlighted;
						if (equipDef && index == equipDef.equipmentIndex) return false;

						if (disableDrops) return false;

						float chance = GetDropChance();
						if (chance <= 0f) return false;

						if (CheckDropRoll(chance, master))
						{
							runDropCount++;
							UpdateZetDropTracker();
							Debug.LogWarning("ZetAspects - RunDropCount : " + runDropCount);
							LogDropChance();
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
				if (!DropAsEquipment())
				{
					EquipmentIndex equipIndex = PickupCatalog.GetPickupDef(pickupIndex).equipmentIndex;

					if (equipIndex != EquipmentIndex.None)
					{
						ItemIndex newIndex = ZetAspectsPlugin.ItemizeEliteEquipment(equipIndex);

						if (newIndex != ItemIndex.None) pickupIndex = PickupCatalog.FindPickupIndex(newIndex);
					}
				}

				orig(pickupIndex, position, velocity);
			};
		}

		private static bool DropAsEquipment()
		{
			if (Catalog.aspectAbilities && Configuration.AspectAbilitiesEliteEquipment.Value) return true;
			if (Configuration.AspectEliteEquipment.Value) return true;
			return false;
		}

		internal static bool CanObtainEquipment()
		{
			return DropAsEquipment();
		}

		internal static bool CanObtainItem()
		{
			if (DropAsEquipment())
			{
				if (Configuration.AspectEquipmentConversion.Value) return true;
				if (Configuration.AspectEquipmentAbsorb.Value) return true;
				if (!Configuration.AspectWorldUnique.Value) return true;
				return false;
			}
			else return true;
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
						if (Configuration.AspectEquipmentAbsorb.Value)
						{
							EquipmentIndex pickupEquip = pickupDef.equipmentIndex;

							if (pickupEquip != EquipmentIndex.None && pickupEquip == body.inventory.currentEquipmentIndex)
							{
								ItemIndex newIndex = ZetAspectsPlugin.ItemizeEliteEquipment(pickupEquip);

								if (newIndex != ItemIndex.None)
								{
									gpc.pickupIndex = PickupCatalog.FindPickupIndex(newIndex);
									return PickupCatalog.GetPickupDef(gpc.pickupIndex);
								}
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

		private static void DiscoverAspectHook()
		{
			On.RoR2.UserProfile.DiscoverPickup += (orig, self, pickupIndex) =>
			{
				orig(self, pickupIndex);

				EquipmentIndex equipIndex = PickupCatalog.GetPickupDef(pickupIndex).equipmentIndex;

				if (equipIndex != EquipmentIndex.None)
				{
					ItemIndex newIndex = ZetAspectsPlugin.ItemizeEliteEquipment(equipIndex);

					if (newIndex != ItemIndex.None) orig(self, PickupCatalog.FindPickupIndex(newIndex));
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
						ItemIndex itemIndex = ZetAspectsPlugin.ItemizeEliteEquipment(inventory.currentEquipmentIndex);
						if (itemIndex != ItemIndex.None)
						{
							inventory.GiveItem(itemIndex);
							inventory.SetEquipmentIndex(EquipmentIndex.None);
						}
					}
				}
			};
		}

		private static Chat.UserChatMessage ReconstructMessage(string message)
		{
			Match match = Regex.Match(message, "<color=#e5eefc><noparse>(.+?)<\\/noparse>: <noparse>(.+?)<\\/noparse><\\/color>", RegexOptions.Compiled);
			if (!match.Success || match.Groups.Count < 3)
			{
				match = Regex.Match(message, "<color=#e5eefc>(.+?): (.+?)<\\/color>", RegexOptions.Compiled);
				if (!match.Success || match.Groups.Count < 3) return null;
			}

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

				if (self.equipmentIcons.Length > 0) AttachZetAspectsConvertHandler(self.equipmentIcons[0]);
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

			ItemIndex itemIndex = ZetAspectsPlugin.ItemizeEliteEquipment(inventory.currentEquipmentIndex);
			if (itemIndex != ItemIndex.None) 
			{
				PlayerCharacterMasterController pcmc = handler.GetPCMC();
				if (!pcmc) return;
				NetworkUser netUser = pcmc.networkUser;
				if (!netUser) return;

				RoR2.Console.instance.SubmitCmd(netUser, "say \"" + "ConvertAspect" + "\"", false);
			}
		}
	}
}
