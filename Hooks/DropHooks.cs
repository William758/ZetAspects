using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.UI;

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



	public static class DropHooks
	{
		internal static Dictionary<EquipmentIndex, float> DropWeight = new Dictionary<EquipmentIndex, float>();
		public static bool applyDropWeight = false;

		public static bool disableDrops = false;
		public static int runDropCount = 0;

		internal static void Init()
		{
			SceneDirector.onPostPopulateSceneServer += OnScenePopulated;
			SceneExitController.onBeginExit += OnSceneExit;

			On.RoR2.Artifacts.CommandArtifactManager.OnDropletHitGroundServer += CommandGroupInterceptHook;

			ResetRunDropCountHook();
			EarlyDropChanceHook();
			DisableDropChanceHook();
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

				// Prevent reading non-existant configs
				if (Catalog.dropWeightsAvailable)
				{
					int limit = Configuration.AspectDropWeightLimit.Value;
					applyDropWeight = limit <= 0 || Run.instance.stageClearCount < limit;

					if (applyDropWeight) UpdateDropWeights();
				}

				disableDrops = false;
				Logger.Info("ScenePopulated : Drops Enabled");

				Logger.Info("RunDropCount : " + runDropCount);
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
					if (inventory) highestCount = Math.Max(highestCount, inventory.GetItemCount(Catalog.Item.ZetAspectsDropCountTracker));
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
						int trackerCount = inventory.GetItemCount(Catalog.Item.ZetAspectsDropCountTracker);
						int dropCount = runDropCount;

						if (trackerCount < dropCount) inventory.GiveItem(Catalog.Item.ZetAspectsDropCountTracker, dropCount - trackerCount);
					}
				}
			}
		}

		private static void UpdateDropWeights()
		{
			SetDropWeight(Catalog.Equip.AffixWhite, Configuration.AspectDropWeightWhite.Value);
			SetDropWeight(Catalog.Equip.AffixBlue, Configuration.AspectDropWeightBlue.Value);
			SetDropWeight(Catalog.Equip.AffixRed, Configuration.AspectDropWeightRed.Value);
			SetDropWeight(Catalog.Equip.AffixHaunted, Configuration.AspectDropWeightHaunted.Value);
			SetDropWeight(Catalog.Equip.AffixPoison, Configuration.AspectDropWeightPoison.Value);
			SetDropWeight(Catalog.Equip.AffixLunar, Configuration.AspectDropWeightLunar.Value);

			SetDropWeight(Catalog.Equip.AffixEarth, Configuration.AspectDropWeightEarth.Value);
			SetDropWeight(Catalog.Equip.AffixVoid, Configuration.AspectDropWeightVoid.Value);

			if (Catalog.SpikeStrip.Enabled)
			{
				SetDropWeight(Catalog.Equip.AffixAragonite, Configuration.AspectDropWeightAragonite.Value);
				SetDropWeight(Catalog.Equip.AffixVeiled, Configuration.AspectDropWeightVeiled.Value);
				SetDropWeight(Catalog.Equip.AffixWarped, Configuration.AspectDropWeightWarped.Value);
				SetDropWeight(Catalog.Equip.AffixPlated, Configuration.AspectDropWeightPlated.Value);
			}

			if (Catalog.GoldenCoastPlus.Enabled)
			{
				SetDropWeight(Catalog.Equip.AffixGold, Configuration.AspectDropWeightGold.Value);
			}

			if (Catalog.Aetherium.Enabled)
			{
				SetDropWeight(Catalog.Equip.AffixSanguine, Configuration.AspectDropWeightSanguine.Value);
			}

			if (Catalog.Bubbet.Enabled)
			{
				SetDropWeight(Catalog.Equip.AffixSepia, Configuration.AspectDropWeightSepia.Value);
			}
		}

		private static void SetDropWeight(EquipmentDef equipDef, float value)
		{
			if (equipDef)
			{
				EquipmentIndex equipIndex = equipDef.equipmentIndex;
				if (equipIndex != EquipmentIndex.None)
				{
					if (DropWeight.ContainsKey(equipIndex)) DropWeight[equipIndex] = value;
					else DropWeight.Add(equipIndex, value);
				}
			}
		}

		private static void OnSceneExit(SceneExitController sceneExitController)
		{
			if (Run.instance)
			{
				disableDrops = true;
				Logger.Info("SceneExit : Drops Disabled");
			}
		}



		private static void CommandGroupInterceptHook(On.RoR2.Artifacts.CommandArtifactManager.orig_OnDropletHitGroundServer orig, ref GenericPickupController.CreatePickupInfo createPickupInfo, ref bool shouldSpawn)
		{
			if (!shouldSpawn) return;

			PickupIndex pickupIndex = createPickupInfo.pickupIndex;
			if (pickupIndex != PickupIndex.none)
			{
				PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
				if (pickupDef != null)
				{
					if (pickupDef.itemIndex != ItemIndex.None && Catalog.aspectItemIndexes.Contains(pickupDef.itemIndex))
					{
						if (Configuration.AspectWorldUnique.Value && Configuration.AspectCommandGroupItems.Value)
						{
							AspectCommandDroplet(pickupDef, createPickupInfo.position, createPickupInfo.rotation, ref shouldSpawn);
						}
					}

					if (pickupDef.equipmentIndex != EquipmentIndex.None && Catalog.aspectEquipIndexes.Contains(pickupDef.equipmentIndex))
					{
						if (Configuration.AspectCommandGroupEquip.Value)
						{
							AspectCommandDroplet(pickupDef, createPickupInfo.position, createPickupInfo.rotation, ref shouldSpawn);
						}
					}
				}
			}

			if (!shouldSpawn) return;

			orig(ref createPickupInfo, ref shouldSpawn);
		}

		private static void AspectCommandDroplet(PickupDef pickupDef, Vector3 position, Quaternion rotation, ref bool shouldSpawn)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Catalog.CommandCubePrefab, position, rotation);
			gameObject.GetComponent<PickupIndexNetworker>().NetworkpickupIndex = pickupDef.pickupIndex;
			SetPickupPickerControllerAspectOptions(gameObject.GetComponent<PickupPickerController>(), pickupDef);
			NetworkServer.Spawn(gameObject);

			shouldSpawn = false;
		}

		private static void SetPickupPickerControllerAspectOptions(PickupPickerController controller, PickupDef pickupDef)
		{
			List<PickupPickerController.Option> options = new List<PickupPickerController.Option>();

			if (pickupDef.itemIndex != ItemIndex.None)
			{
				foreach (ItemIndex itemIndex in Catalog.aspectItemIndexes)
				{
					bool valid = !Catalog.disabledItemIndexes.Contains(itemIndex);
					bool available = !Run.instance.IsItemExpansionLocked(itemIndex);

					if (valid && available)
					{
						PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(itemIndex);
						options.Add(new PickupPickerController.Option { available = true, pickupIndex = pickupIndex });
					}
				}
			}
			else if (pickupDef.equipmentIndex != EquipmentIndex.None)
			{
				foreach (EquipmentIndex equipIndex in Catalog.aspectEquipIndexes)
				{
					bool available = !Run.instance.IsEquipmentExpansionLocked(equipIndex);

					if (available)
					{
						PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(equipIndex);
						options.Add(new PickupPickerController.Option { available = true, pickupIndex = pickupIndex });
					}
				}
			}
			else
			{
				options.Add(new PickupPickerController.Option { available = true, pickupIndex = pickupDef.pickupIndex });
			}

			controller.SetOptionsServer(options.ToArray());
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

			string output = "BaseDropChance : ";
			output += $"{currentChance:0.###}% : ";
			output += "[base]" + $"{baseChance:0.###}% x ";
			output += "[decay]" + $"{decayMult:0.###} x ";
			output += "[player]" + $"{playerMult:0.###} x ";
			output += "[stage]" + $"{stageMult:0.###}";

			Logger.Info(output);
		}

		private static float GetDropChance(EquipmentIndex index)
		{
			float chance = Configuration.AspectDropChance.Value;

			// can only be set to true after a Catalog.dropWeightsAvailable check
			// set on scene populated : checks stage limit
			if (applyDropWeight)
			{
				if (DropWeight.ContainsKey(index)) chance *= DropWeight[index];
			}

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
			float value = Configuration.AspectDropChanceCompensation.Value;

			if (value <= 1f) return 1f;

			if (Run.instance)
			{
				int stagesCleared = Run.instance.stageClearCount;
				int dropCount = runDropCount;

				if (dropCount < 3)
				{
					if (stagesCleared >= (dropCount + 1) * 3) return value;
				}
				else
				{
					if (stagesCleared >= (dropCount * 5) - 1) return value;
				}
			}

			return 1f;
		}

		private static bool CheckDropRoll(float chance, CharacterMaster master)
		{
			bool detailedLogging = Configuration.AspectDropVerboseLogging.Value;

			float luck = master ? master.luck : 0f;

			int count = Mathf.CeilToInt(Mathf.Abs(luck));
			if (detailedLogging)
			{
				string text = "--Roll count based on luck : " + (count + 1);
				if (count > 0) text += (luck > 0f) ? " (favorable)" : " unfavorable";
				Logger.Info(text);
			}
			float roll = UnityEngine.Random.Range(0f, 100f);
			if (detailedLogging) Logger.Info("----Roll : " + roll);

			for (int i = 0; i < count; i++)
			{
				float r = UnityEngine.Random.Range(0f, 100f);
				if (detailedLogging) Logger.Info("----Roll : " + r);
				roll = (luck > 0f) ? Mathf.Min(roll, r) : Mathf.Max(roll, r);
			}

			bool drop = roll <= chance;
			if (detailedLogging) Logger.Info("--Result : " + roll + " <= " + chance + " ? " + drop);
			return drop;
		}

		private static void EarlyDropChanceHook()
		{
			On.RoR2.GlobalEventManager.OnCharacterDeath += (orig, self, damageReport) =>
			{
				if (NetworkServer.active && damageReport != null)
				{
					CharacterMaster atkMaster = damageReport.attackerMaster;
					CharacterBody vicBody = damageReport.victimBody;
					EquipmentDef equipDef = null;

					if (vicBody)
					{
						if (vicBody.equipmentSlot)
						{
							// some bodies return null equipDef even though they have an elite aspect
							equipDef = EquipmentCatalog.GetEquipmentDef(vicBody.equipmentSlot.equipmentIndex);
						}

						if (!equipDef && vicBody.isElite)
						{
							if (Configuration.AspectDropVerboseLogging.Value)
							{
								Logger.Warn("EarlyDropChanceHook - Victim is elite but equipment from slot is null, checking inventory!");
							}

							Inventory inventory = vicBody.inventory;
							if (inventory)
							{
								equipDef = EquipmentCatalog.GetEquipmentDef(inventory.currentEquipmentIndex);
							}
						}
					}

					//Logger.Info("EarlyDropChanceHook - Victim : " + vicBody.name + "[" + vicBody.netId + "] Equipment : " + (equipDef ? equipDef.name : "None"));

					if (equipDef && atkMaster && vicBody)
					{
						if (AttemptDrop(atkMaster, equipDef, vicBody))
						{
							GameObject vicObject = null;
							if (damageReport.victim) vicObject = damageReport.victim.gameObject;

							Vector3 position = Vector3.zero;
							Quaternion rotation = Quaternion.identity;
							Transform transform = vicObject.transform;
							if (transform)
							{
								position = transform.position;
								rotation = transform.rotation;
							}

							InputBankTest inputBankTest = null;
							if (vicBody) inputBankTest = vicBody.inputBank;
							Ray aimRay = inputBankTest ? inputBankTest.GetAimRay() : new Ray(position, rotation * Vector3.forward);

							PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(equipDef.equipmentIndex), position + Vector3.up * 1.5f, Vector3.up * 20f + aimRay.direction * 2f);
						}
					}
				}

				orig(self, damageReport);
			};
		}

		private static bool AttemptDrop(CharacterMaster atkMaster, EquipmentDef equipDef, CharacterBody vicBody)
		{
			EquipmentIndex index = equipDef.equipmentIndex;

			if (Catalog.GetEquipmentEliteDef(equipDef) == null) return false;
			if (Catalog.ItemizeEliteEquipment(index) == ItemIndex.None) return false;

			if (disableDrops) return false;

			float chance = GetDropChance(index);
			if (Configuration.AspectDropVerboseLogging.Value)
			{
				Logger.Info("Elite Killed - Rolling aspect drop chance!");
				Logger.Info("--Victim : " + vicBody.name + "[" + vicBody.netId + "] Equipment : " + equipDef.name);
				Logger.Info("--Aspect drop chance : " + chance);
			}
			if (chance <= 0f) return false;

			if (CheckDropRoll(chance, atkMaster))
			{
				runDropCount++;
				UpdateZetDropTracker();
				Logger.Info("RunDropCount : " + runDropCount);
				LogDropChance();

				if (Configuration.AspectShowDropText.Value)
				{
					Chat.SendBroadcastChat(new Chat.SimpleChatMessage
					{
						baseToken = "<color=#DDDDA0><size=120%>" + Configuration.AspectDropText.Value + "</color></size>"
					});
				}

				return true;
			}

			return false;
		}

		private static void DisableDropChanceHook()
		{
			IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdfld<EquipmentDef>("dropOnDeathChance"),
					x => x.MatchLdcR4(100f),
					x => x.MatchMul(),
					x => x.MatchLdloc(16),
					x => x.MatchCall("RoR2.Util", "CheckRoll")
				);

				if (found)
				{
					c.Index += 5;

					c.Emit(OpCodes.Ldloc, 12);// equipmentDef
					c.EmitDelegate<Func<bool, EquipmentDef, bool>>((result, equipDef) =>
					{
						EquipmentIndex index = equipDef.equipmentIndex;

						//if (index == EquipmentIndex.None) return false;
						//if (Catalog.GetEquipmentEliteDef(equipDef) == null) return false;
						if (Catalog.ItemizeEliteEquipment(index) == ItemIndex.None) return false;

						return result;
					});
				}
				else
				{
					Logger.Warn("DisableDropChanceHook Failed");
				}
			};
		}

		private static void InterceptAspectDropHook()
		{
			On.RoR2.PickupDropletController.CreatePickupDroplet_PickupIndex_Vector3_Vector3 += (orig, pickupIndex, position, velocity) =>
			{
				if (!DropAsEquipment())
				{
					EquipmentIndex equipIndex = PickupCatalog.GetPickupDef(pickupIndex).equipmentIndex;

					if (equipIndex != EquipmentIndex.None)
					{
						ItemIndex newIndex = Catalog.ItemizeEliteEquipment(equipIndex);

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

		public static bool CanObtainEquipment()
		{
			return DropAsEquipment();
		}

		public static bool CanObtainItem()
		{
			if (DropAsEquipment())
			{
				if (Configuration.AspectEquipmentConversion.Value) return true;
				if (Configuration.AspectEquipmentAbsorb.Value) return true;
				// cant get from killing elites but can still find in chests/printers
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
					x => x.MatchLdfld<GenericPickupController>("pickupIndex"),
					x => x.MatchCall("RoR2.PickupCatalog", "GetPickupDef"),
					x => x.MatchStloc(1)
				);

				if (found)
				{
					c.Index += 4;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldarg, 1);
					c.Emit(OpCodes.Ldloc, 1);
					c.EmitDelegate<Func<GenericPickupController, CharacterBody, PickupDef, PickupDef>>((controller, body, pickupDef) =>
					{
						if (Configuration.AspectEquipmentAbsorb.Value)
						{
							EquipmentIndex pickupEquip = pickupDef.equipmentIndex;

							if (pickupEquip != EquipmentIndex.None && pickupEquip == body.inventory.currentEquipmentIndex)
							{
								ItemIndex newIndex = Catalog.ItemizeEliteEquipment(pickupEquip);

								if (newIndex != ItemIndex.None)
								{
									controller.pickupIndex = PickupCatalog.FindPickupIndex(newIndex);
									return PickupCatalog.GetPickupDef(controller.pickupIndex);
								}
							}
						}

						return pickupDef;
					});
					c.Emit(OpCodes.Stloc, 1);
				}
				else
				{
					Logger.Warn("EquipmentAbsorbHook Failed");
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
					ItemIndex newIndex = Catalog.ItemizeEliteEquipment(equipIndex);

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

				string message = Chat.readOnlyLog.DefaultIfEmpty(null).Last();
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
						ItemIndex itemIndex = Catalog.ItemizeEliteEquipment(inventory.currentEquipmentIndex);
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

			ItemIndex itemIndex = Catalog.ItemizeEliteEquipment(inventory.currentEquipmentIndex);
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
