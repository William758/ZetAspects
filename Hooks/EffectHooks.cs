using System;
using System.Collections.Generic;
//using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
//using RoR2.UI;
//using TMPro;

namespace TPDespair.ZetAspects
{
	internal static class EffectHooks
	{
		//public static List<string> dronesList = new List<string> { "Drone", "Droid", "Robo", "Turret", "Missile", "Laser", "Beam" };

		internal static Dictionary<NetworkInstanceId, float> DestroyedBodies = new Dictionary<NetworkInstanceId, float>();
		internal static Dictionary<NetworkInstanceId, float> ShieldRegenAccumulator = new Dictionary<NetworkInstanceId, float>();

		//private static FieldInfo DamageInfoRejectedField;

		internal const float BuffCycleDuration = 5f;
		private static float FixedUpdateStopwatch = 0f;

		internal static bool preventedDefaultOverloadingBomb = false;
		internal static bool preventedDefaultBlazeBurn = false;
		internal static bool preventedDefaultLunarCripple = false;
		internal static bool preventedDefaultVoidCollapse = false;

		internal static bool useCustomOverloadBombs = false;
		internal static bool useCustomFracture = false;



		internal static void OnFixedUpdate()
		{
			FixedUpdateStopwatch += Time.fixedDeltaTime;

			if (FixedUpdateStopwatch >= 0.5f)
			{
				List<NetworkInstanceId> destroyedBodiesKeys = new List<NetworkInstanceId>(DestroyedBodies.Keys);

				foreach (var netId in destroyedBodiesKeys)
				{
					DestroyedBodies[netId] -= FixedUpdateStopwatch;
					if (DestroyedBodies[netId] <= 0f)
					{
						DestroyedBodies.Remove(netId);

						if (DisplayHooks.BodyEliteDisplay.ContainsKey(netId))
						{
							DisplayHooks.BodyEliteDisplay.Remove(netId);
						}
						if (ShieldRegenAccumulator.ContainsKey(netId))
						{
							ShieldRegenAccumulator.Remove(netId);
						}
					}
				}

				FixedUpdateStopwatch = 0f;
			}
		}



		internal static void Init()
		{
			OnDestroyHook();

			//DamageInfoRejectedField = typeof(DamageInfo).GetField("rejected");

			FreezeHook();
			PreventAspectChillHook();
			PreventOverloadingBombHook();
			FireTrailHook();
			PreventAspectBurnHook();
			PoisonBallHook();
			NullDurationHook();
			LunarProjectileHook();
			PreventAspectCrippleHook();
			PreventAspectCollapseHook();
			DamageTakenHook();
			HeadHunterBuffHook();
			//DotAmpHook();
			OnHitAllHook();
			OnHitEnemyHook();
			FixTimedChillApplication();
			ShieldRegenHook();

			PreventVoidEquipmentRemovalHook();
			EquipmentLostBuffHook();
			EquipmentGainedBuffHook();
			ApplyAspectBuffOnInventoryChangedHook();
			UpdateOnBuffLostHook();
			RefreshAspectBuffsHook();
		}



		internal static void ApplyVoidBearCooldownFix()
		{
			On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += VoidBearFix_AddTimedBuff;
			On.RoR2.CharacterBody.RemoveBuff_BuffIndex += VoidBearFix_RemoveBuff;
		}



		private static void OnDestroyHook()
		{
			On.RoR2.CharacterBody.OnDestroy += (orig, self) =>
			{
				// Prevent AffixVoid Displays
				DisplayHooks.OnBodyDeath(self);

				DestroyedBodies.Add(self.netId, 3.5f);

				orig(self);
			};
		}


		/*
		private static void EffectManagerHook()
		{
			On.RoR2.EffectManager.SpawnEffect_EffectIndex_EffectData_bool += (orig, index, data, transmit) =>
			{
				if ((int)index == 1758001 && !transmit)
				{
					if (data.genericUInt == 1u)
					{
						CreateMissText(data);
					}
					else
					{
						Debug.LogWarning("ZetAspects - Unknown SpawnEffect : " + data.genericUInt);
					}

					return;
				}

				orig(index, data, transmit);
			};
		}

		private static void CreateMissText(EffectData effectData)
		{
			EffectDef effectDef = Catalog.RejectTextDef;

			if (effectDef == null) return;

			if (!VFXBudget.CanAffordSpawn(effectDef.prefabVfxAttributes)) return;
			if (effectDef.cullMethod != null && !effectDef.cullMethod(effectData)) return;

			EffectData effectDataClone = effectData.Clone();
			GameObject gameObject = UnityEngine.Object.Instantiate(effectDef.prefab, effectDataClone.origin, effectDataClone.rotation);
			if (gameObject)
			{
				EffectComponent effectComponent = gameObject.GetComponent<EffectComponent>();
				if (effectComponent)
				{
					effectComponent.effectData = effectDataClone.Clone();

					Transform textTransform = effectComponent.transform.Find("TextMeshPro");
					if (textTransform)
					{
						TextMeshPro textMesh = textTransform.GetComponent<TextMeshPro>();
						if (textMesh)
						{
							LanguageTextMeshController controller = textMesh.gameObject.GetComponent<LanguageTextMeshController>();
							if (controller)
							{
								controller.token = "MISS";
								textMesh.text = "MISS";
								textMesh.fontSize = 1.75f;
							}
						}
					}
				}
			}
		}

		private static void DodgeHook()
		{
			IL.RoR2.HealthComponent.TakeDamage += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(1),
					x => x.MatchLdfld<DamageInfo>("rejected"),
					x => x.MatchBrtrue(out _),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<HealthComponent>("body"),
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("BodyArmor"))
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldarg, 1);
					c.EmitDelegate<Func<HealthComponent, DamageInfo, bool>>((hc, damageInfo) =>
					{
						if (damageInfo.rejected) return true;

						float effect = 0f;
						float rangeMult = 1f;
						bool attackerBlind = false;

						if (damageInfo.attacker)
						{
							CharacterBody attackBody = damageInfo.attacker.GetComponent<CharacterBody>();
							if (attackBody)
							{
								// attacker is player at close range
								if (Configuration.AspectCycloneDodgeNegate.Value && attackBody.teamComponent.teamIndex == TeamIndex.Player)
								{
									rangeMult = (attackBody.corePosition - damageInfo.position).sqrMagnitude;

									if (rangeMult < 400f) return false;

									if (rangeMult < 900f) rangeMult = 0.5f;
									else rangeMult = 1f;
								}

								if (attackBody.HasBuff(Catalog.EliteVariety.blindBuffIndex))
								{
									attackerBlind = true;

									if (Configuration.AspectCycloneBlindDodgeEffect.Value > 0f) effect += Configuration.AspectCycloneBlindDodgeEffect.Value;
								}
							}
						}

						CharacterBody body = hc.body;

						if (Configuration.AspectCycloneBaseDodgeGain.Value > 0f)
						{
							if (!Configuration.AspectCycloneDodgeBlindOnly.Value || attackerBlind)
							{
								if (body && body.HasBuff(Catalog.EliteVariety.Buffs.AffixSandstorm))
								{
									float count = ZetAspectsPlugin.GetStackMagnitude(body, Catalog.EliteVariety.Buffs.AffixSandstorm);
									effect += Configuration.AspectCycloneBaseDodgeGain.Value + Configuration.AspectCycloneStackDodgeGain.Value * (count - 1f);
								}
							}
						}

						if (effect > 0f)
						{
							effect = Util.ConvertAmplificationPercentageIntoReductionPercentage(effect * 100f);
							effect *= rangeMult;

							if (body)
							{
								if (body.bodyIndex == Catalog.RiskOfRain.mithrixBodyIndex) effect *= 0.5f;

								if (body.teamComponent.teamIndex == TeamIndex.Player)
								{
									if (Catalog.diluvianArtifactIndex != ArtifactIndex.None)
									{
										RunArtifactManager artifactManager = RunArtifactManager.instance;
										if (artifactManager && artifactManager.IsArtifactEnabled(Catalog.diluvianArtifactIndex)) effect *= 0.5f;
									}
								}
							}

							if (Util.CheckRoll(effect, 0f, null))
							{
								EffectManager.SpawnEffect((EffectIndex)1758001, new EffectData { genericUInt = 1u, origin = damageInfo.position }, true);

								return true;
							}
						}

						return false;
					});
					c.Emit(OpCodes.Stfld, DamageInfoRejectedField);

					c.Emit(OpCodes.Ldarg, 1);
				}
				else
				{
					Debug.LogWarning("ZetAspects : DodgeHook Failed");
				}
			};
		}
		*/


		private static void FreezeHook()
		{
			IL.RoR2.SetStateOnHurt.OnTakeDamageServer += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdcR4(2f),
					x => x.MatchLdloc(1),
					x => x.MatchLdfld<DamageInfo>("procCoefficient"),
					x => x.MatchMul()
				);

				if (found)
				{
					// Move to after default freeze check
					c.Index += 8;

					// Handle freeze chance
					c.Emit(OpCodes.Ldarg, 1);
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Action<DamageReport, SetStateOnHurt>>((damageReport, state) =>
					{
						CharacterBody attacker = damageReport.attackerBody;
						CharacterBody victim = damageReport.victimBody;

						if (attacker == null) return;
						if (victim && victim.bodyIndex == Catalog.mithrixBodyIndex) return;
						if (!attacker.HasBuff(RoR2Content.Buffs.AffixWhite) || Configuration.AspectWhiteBaseFreezeChance.Value <= 0f) return;
						if (!state.canBeFrozen || attacker.teamComponent.teamIndex != TeamIndex.Player || damageReport.damageInfo.procCoefficient < 0.105f) return;

						float count = Catalog.GetStackMagnitude(attacker, RoR2Content.Buffs.AffixWhite);
						float chance = Configuration.AspectWhiteBaseFreezeChance.Value + Configuration.AspectWhiteStackFreezeChance.Value * (count - 1f);
						chance = Util.ConvertAmplificationPercentageIntoReductionPercentage(chance * damageReport.damageInfo.procCoefficient);

						if (Util.CheckRoll(chance)) state.SetFrozen(Configuration.AspectWhiteFreezeDuration.Value * damageReport.damageInfo.procCoefficient);
					});
				}
				else
				{
					Logger.Warn("FreezeHook Failed");
				}
			};
		}

		private static void PreventAspectChillHook()
		{
			// still applies a 0 duration chill, another hook prevents it from going through
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcI4(2),
					x => x.MatchAdd(),
					x => x.MatchStloc(9),
					x => x.MatchLdloc(9)
				);

				if (found)
				{
					c.Index += 4;

					c.Emit(OpCodes.Ldc_I4, 0);
					c.Emit(OpCodes.Stloc, 9);
				}
				else
				{
					Logger.Warn("PreventAspectChillHook Failed");
				}
			};
		}

		private static void PreventOverloadingBombHook()
		{
			IL.RoR2.GlobalEventManager.OnHitAll += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(0),
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("AffixBlue")),
					x => x.MatchCallvirt<CharacterBody>("HasBuff")
				);

				if (found)
				{
					c.Index += 3;

					// Prevent default
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 0);

					preventedDefaultOverloadingBomb = true;
				}
				else
				{
					Logger.Warn("PreventOverloadingBombHook Failed");
				}
			};
		}

		private static void FireTrailHook()
		{
			IL.RoR2.CharacterBody.UpdateFireTrail += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(0)
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 0);
					c.EmitDelegate<Func<CharacterBody, bool, bool>>((self, hasBuff) =>
					{
						if (self.teamComponent.teamIndex == TeamIndex.Player && !Configuration.AspectRedTrail.Value) return false;

						return hasBuff;
					});
					c.Emit(OpCodes.Stloc, 0);
				}
				else
				{
					Logger.Warn("FireTrailHook Failed");
				}
			};
		}

		private static void PreventAspectBurnHook()
		{
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(1),
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("AffixRed")),
					x => x.MatchCallvirt<CharacterBody>("HasBuff")
				);

				if (found)
				{
					c.Index += 3;

					// Prevent default
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 0);

					preventedDefaultBlazeBurn = true;
				}
				else
				{
					Logger.Warn("PreventAspectBurnHook Failed");
				}
			};
		}

		private static void PoisonBallHook()
		{
			IL.RoR2.CharacterBody.UpdateAffixPoison += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("poisonballTimer"),
					x => x.MatchLdarg(1),
					x => x.MatchAdd(),
					x => x.MatchStfld<CharacterBody>("poisonballTimer")
				);

				if (found)
				{
					c.Index += 4;

					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<float, CharacterBody, float>>((deltaTime, self) =>
					{
						if (self.teamComponent.teamIndex == TeamIndex.Player && !Configuration.AspectPoisonFireSpikes.Value) return 0f;

						return deltaTime;
					});
				}
				else
				{
					Logger.Warn("PoisonBallHook Failed");
				}
			};
		}

		private static void NullDurationHook()
		{
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(2),
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("HealingDisabled")),
					x => x.MatchLdcR4(8f),
					x => x.MatchLdarg(1),
					x => x.MatchLdfld<DamageInfo>("procCoefficient"),
					x => x.MatchMul()
				);

				if (found)
				{
					c.Index += 6;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc, 1);
					c.Emit(OpCodes.Ldarg, 1);
					c.EmitDelegate<Func<CharacterBody, DamageInfo, float>>((self, damageInfo) =>
					{
						float duration = Configuration.AspectPoisonNullDuration.Value;
						if (self.teamComponent.teamIndex != TeamIndex.Player) duration = 8f * damageInfo.procCoefficient;

						return duration;
					});
				}
				else
				{
					Logger.Warn("NullDurationHook Failed");
				}
			};
		}

		private static void LunarProjectileHook()
		{
			IL.RoR2.CharacterBody.UpdateAffixLunar += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchCall<CharacterBody>("HasBuff")
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<bool, CharacterBody, bool>>((hasBuff, self) =>
					{
						if (self.teamComponent.teamIndex == TeamIndex.Player && !Configuration.AspectLunarProjectiles.Value) return false;

						return hasBuff;
					});
				}
				else
				{
					Logger.Warn("LunarProjectileHook Failed");
				}
			};
		}

		private static void PreventAspectCrippleHook()
		{
			IL.RoR2.HealthComponent.TakeDamage += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(1),
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("AffixLunar")),
					x => x.MatchCallvirt<CharacterBody>("HasBuff")
				);

				if (found)
				{
					c.Index += 3;

					// prevent default
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 0);

					preventedDefaultLunarCripple = true;
				}
				else
				{
					Logger.Warn("PreventAspectCrippleHook Failed");
				}
			};
		}

		private static void PreventAspectCollapseHook()
		{
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(1),
					x => x.MatchLdsfld(typeof(DLC1Content.Buffs).GetField("EliteVoid")),
					x => x.MatchCallvirt<CharacterBody>("HasBuff")
				);

				if (found)
				{
					c.Index += 3;

					// prevent default
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 0);

					preventedDefaultVoidCollapse = true;
				}
				else
				{
					Logger.Warn("PreventAspectCollapseHook Failed");
				}
			};
		}



		private static void DamageTakenHook()
		{
			IL.RoR2.HealthComponent.TakeDamage += (il) =>
			{
				ILCursor c = new ILCursor(il);

				// find : store damageinfo.damage into variable
				bool found = c.TryGotoNext(
					x => x.MatchLdarg(1),
					x => x.MatchLdfld<DamageInfo>("damage"),
					x => x.MatchStloc(6)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Ldloc, 6);
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<float, HealthComponent, float>>((damage, healthComponent) =>
					{
						CharacterBody self = healthComponent.body;

						if (!self) return damage;

						if (self.HasBuff(RoR2Content.Buffs.HealingDisabled) && Configuration.AspectPoisonNullDamageTaken.Value != 0f)
						{
							float extraDamage = Mathf.Abs(Configuration.AspectPoisonNullDamageTaken.Value);
							//if (self.teamComponent.teamIndex == TeamIndex.Player) extraDamage *= Configuration.AspectEffectPlayerDebuffMult.Value;
							damage *= 1f + extraDamage;
						}

						return damage;
					});
					c.Emit(OpCodes.Stloc, 6);
				}
				else
				{
					Logger.Warn("DamageTakenHook Failed");
				}
			};
		}
		/*
		internal static bool IsValidDrone(CharacterMaster minionMaster)
		{
			bool result = dronesList.Exists((droneSubstring) => { return minionMaster.name.Contains(droneSubstring); });
			//Debug.LogWarning("Checking Master Name : " + minionMaster.name + " - " + result);
			return result;
		}
		
		internal static CharacterBody GetMinionOwnerBody(CharacterMaster minionMaster)
		{
			MinionOwnership minionOwnership = minionMaster.minionOwnership;
			if (!minionOwnership) return null;
			CharacterMaster ownerMaster = minionOwnership.ownerMaster;
			if (!ownerMaster) return null;

			return ownerMaster.GetBody();
		}
		*/

		private static void HeadHunterBuffHook()
		{
			IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcR4(3f),
					x => x.MatchLdcR4(5f),
					x => x.MatchLdloc(57),
					x => x.MatchConvR4(),
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchStloc(59)
				);

				if (found)
				{
					c.Index += 6;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc, 57);
					c.EmitDelegate<Func<int, float>>((count) =>
					{
						return Configuration.HeadHunterBaseDuration.Value + Configuration.HeadHunterStackDuration.Value * (count - 1);
					});
					c.Emit(OpCodes.Dup);
					c.Emit(OpCodes.Ldloc, 14);
					c.EmitDelegate<Action<float, CharacterBody>>((duration, attacker) =>
					{
						if (Configuration.HeadHunterBuffEnable.Value) attacker.AddTimedBuff(Catalog.Buff.ZetHeadHunter, duration);
					});
				}
				else
				{
					Logger.Warn("HeadHunterBuffHook Failed");
				}
			};
		}
		/*
		private static void ModifyDot()
		{
			On.RoR2.DotController.InflictDot_GameObject_GameObject_DotIndex_float_float += (orig, attacker, victim, index, duration, damage) =>
			{
				DotController.DotIndex dotIndex = Catalog.EliteVariety.impaleDotIndex;
				if (dotIndex != DotController.DotIndex.None && index == dotIndex)
				{
					damage *= Configuration.AspectImpaleDamageMult.Value;

					if (Configuration.AspectImpaleTweaks.Value && Run.instance)
					{
						bool isPlayer = false;
						CharacterBody atkBody = attacker.GetComponent<CharacterBody>();
						if (atkBody && atkBody.teamComponent.teamIndex == TeamIndex.Player) isPlayer = true;

						float mult = Mathf.Clamp(Run.instance.ambientLevel / 90f, 0.05f, 1f);
						if (isPlayer)
						{
							mult = 1 - mult;
							mult *= mult;
							mult = 1 - mult;
						}

						damage *= mult;
						duration *= mult;

						duration = Mathf.Ceil(duration / 5f) * 5f + 0.1f;
					}
				}

				orig(attacker, victim, index, duration, damage);
			};
		}

		private static void ModifyDeploySlot()
		{
			On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += (orig, self, slot) =>
			{
				DeployableSlot deploySlot = Catalog.EliteVariety.tinkerDeploySlot;
				if (deploySlot != DeployableSlot.EngiMine && slot == deploySlot)
				{
					int output;

					if (self.teamIndex == TeamIndex.Player) output = Configuration.AspectTinkerPlayerLimit.Value;
					else output = Configuration.AspectTinkerMonsterLimit.Value;

					if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef)) output *= 2;

					return output;
				}
				else
				{
					return orig(self, slot);
				}
			};
		}
		*/
		
		private static void DotAmpHook()
		{
			On.RoR2.DotController.InflictDot_refInflictDotInfo += DotController_InflictDot_refInflictDotInfo;
		}

		private static void DotController_InflictDot_refInflictDotInfo(On.RoR2.DotController.orig_InflictDot_refInflictDotInfo orig, ref InflictDotInfo inflictDotInfo)
		{
			if (inflictDotInfo.attackerObject)
			{
				CharacterBody atkBody = inflictDotInfo.attackerObject.GetComponent<CharacterBody>();
				if (atkBody)
				{
					float mult = 1;
					/*
					if (atkBody.HasBuff(Catalog.EliteVariety.Buffs.AffixImpPlane) && Configuration.AspectImpaleBaseDotAmp.Value > 0f)
					{
						float count = ZetAspectsPlugin.GetStackMagnitude(atkBody, Catalog.EliteVariety.Buffs.AffixImpPlane);
						mult += Configuration.AspectImpaleBaseDotAmp.Value + Configuration.AspectImpaleStackDotAmp.Value * (count - 1f);
					}
					*/
					/*
					if (atkBody.HasBuff(Catalog.Buff.AffixSanguine) && Configuration.AspectSanguineBaseDotAmp.Value > 0f)
					{
						float count = Catalog.GetStackMagnitude(atkBody, Catalog.Buff.AffixSanguine);
						mult += Configuration.AspectSanguineBaseDotAmp.Value + Configuration.AspectSanguineStackDotAmp.Value * (count - 1f);
					}
					*/

					inflictDotInfo.damageMultiplier *= mult;
				}
			}

			orig(ref inflictDotInfo);
		}
		
		private static void OnHitAllHook()
		{
			On.RoR2.GlobalEventManager.OnHitAll += (orig, self, damageInfo, hitObject) =>
			{
				ApplyAspectOnHitAllEffects(damageInfo);

				orig(self, damageInfo, hitObject);
			};
		}

		private static void ApplyAspectOnHitAllEffects(DamageInfo damageInfo)
		{
			if (!NetworkServer.active) return;
			if (damageInfo.procCoefficient <= 0f || damageInfo.rejected) return;

			if (damageInfo.attacker)
			{
				CharacterBody attacker = damageInfo.attacker.GetComponent<CharacterBody>();

				if (attacker)
				{
					FireOverloadingBomb(attacker, damageInfo);
				}
			}
		}

		private static void FireOverloadingBomb(CharacterBody self, DamageInfo damageInfo)
		{
			if (!useCustomOverloadBombs) return;

			if (!self.HasBuff(RoR2Content.Buffs.AffixBlue)) return;
			if (Configuration.AspectBlueBaseDamage.Value <= 0f) return;

			float damage = damageInfo.damage;
			float count = Catalog.GetStackMagnitude(self, RoR2Content.Buffs.AffixBlue);

			damage *= Configuration.AspectBlueBaseDamage.Value + Configuration.AspectBlueStackDamage.Value * (count - 1f);
			if (self.teamComponent.teamIndex != TeamIndex.Player) damage *= Configuration.AspectBlueMonsterDamageMult.Value;

			//Logger.Warn("Damage : " + damageInfo.damage + " - " + "Overloading : " + damage);

			FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
			{
				projectilePrefab = Catalog.LightningStakePrefab,
				position = damageInfo.position,
				rotation = Quaternion.identity,
				owner = damageInfo.attacker,
				damage = damage,
				force = 0f,
				crit = damageInfo.crit,
				damageColorIndex = DamageColorIndex.Item,
				target = null,
				speedOverride = -1f,
				fuseOverride = Configuration.AspectBlueBombDuration.Value,
				damageTypeOverride = null
			};
			ProjectileManager.instance.FireProjectile(fireProjectileInfo);
		}

		private static void OnHitEnemyHook()
		{
			On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damageInfo, victimObject) =>
			{
				ApplyAspectOnHitEnemyEffects(damageInfo, victimObject);

				orig(self, damageInfo, victimObject);
			};
		}

		private static void ApplyAspectOnHitEnemyEffects(DamageInfo damageInfo, GameObject victimObject)
		{
			if (!NetworkServer.active) return;
			if (damageInfo.procCoefficient < 0.125f || damageInfo.rejected) return;

			if (damageInfo.attacker)
			{
				CharacterBody attacker = damageInfo.attacker.GetComponent<CharacterBody>();
				CharacterBody victim = victimObject ? victimObject.GetComponent<CharacterBody>() : null;

				if (attacker && victim)
				{
					CreateFrostBlade(attacker, victim, damageInfo);
					ApplyChill(attacker, victim, damageInfo);
					ApplySapped(attacker, victim, damageInfo);
					ApplyBurn(attacker, victim, damageInfo);
					ApplyShredded(attacker, victim, damageInfo);
					ApplyCripple(attacker, victim, damageInfo);
					HandlePoach(attacker, victim, damageInfo);
					ApplyCollapse(attacker, victim, damageInfo);
					//ApplyBleed(attacker, victim, damageInfo);
				}
			}
		}

		private static void CreateFrostBlade(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!self.HasBuff(RoR2Content.Buffs.AffixWhite)) return;
			if (Configuration.AspectWhiteBaseDamage.Value <= 0f) return;

			if (damageInfo.procChainMask.HasProc(ProcType.Thorns)) return;

			GameObject gameObject = self.gameObject;
			TeamIndex teamIndex = self.teamComponent.teamIndex;

			float damage = damageInfo.damage;
			float count = Catalog.GetStackMagnitude(self, RoR2Content.Buffs.AffixWhite);

			damage *= Configuration.AspectWhiteBaseDamage.Value + Configuration.AspectWhiteStackDamage.Value * (count - 1f);
			if (self.teamComponent.teamIndex != TeamIndex.Player) damage *= Configuration.AspectWhiteMonsterDamageMult.Value;

			var procMask = default(ProcChainMask);
			if (Catalog.borboFrostBlade) procMask.AddProc(ProcType.Thorns);

			//Logger.Warn("Damage : " + damageInfo.damage + " - " + "FrostBlade : " + damage);

			LightningOrb lightningOrb = new LightningOrb
			{
				attacker = gameObject,
				bouncedObjects = null,
				bouncesRemaining = 0,
				damageCoefficientPerBounce = 1f,
				damageColorIndex = DamageColorIndex.Item,
				damageValue = damage,
				isCrit = damageInfo.crit,
				lightningType = LightningOrb.LightningType.RazorWire,
				origin = gameObject.transform.position,
				procChainMask = procMask,
				procCoefficient = 0f,
				range = 0f,
				teamIndex = teamIndex,
				target = victim.mainHurtBox
			};
			OrbManager.instance.AddOrb(lightningOrb);
		}

		private static void FixTimedChillApplication()
		{
			On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += (orig, self, buffDef, duration) =>
			{
				if (NetworkServer.active)
				{
					if (Catalog.altSlow80 != BuffIndex.None && buffDef.buffIndex == Catalog.altSlow80)
					{
						ChillApplication(self, duration);
						return;
					}
					if (buffDef == RoR2Content.Buffs.Slow80)
					{
						if (duration < 0.1f) return;
						if (Catalog.limitChillStacks && self.GetBuffCount(RoR2Content.Buffs.Slow80) >= 10) return;
					}
				}

				orig(self, buffDef, duration);
			};
		}

		private static void ApplyChill(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!CanApplyAspectChill(self)) return;

			float duration = Configuration.AspectWhiteSlowDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;

			ChillApplication(victim, duration);
		}

		private static void ChillApplication(CharacterBody victim, float duration)
		{
			if (duration > 0.1f)
			{
				if (Catalog.ChillCanStack)
				{
					int chillStacks = victim.GetBuffCount(RoR2Content.Buffs.Slow80);

					if (chillStacks > 0)
					{
						RefreshChillStacks(victim, duration);
					}
					else
					{
						victim.AddTimedBuff(RoR2Content.Buffs.Slow80, duration);
					}
				}
				else
				{
					victim.AddTimedBuff(RoR2Content.Buffs.Slow80, duration);
				}
			}
		}

		private static bool CanApplyAspectChill(CharacterBody self)
		{
			if (self.HasBuff(RoR2Content.Buffs.AffixWhite)) return true;
			if (Configuration.AspectHauntedSlowEffect.Value && self.HasBuff(RoR2Content.Buffs.AffixHaunted)) return true;
			return false;
		}

		private static void RefreshChillStacks(CharacterBody self, float duration)
		{
			BuffIndex chillBuffIndex = RoR2Content.Buffs.Slow80.buffIndex;

			for (int i = 0; i < self.timedBuffs.Count; i++)
			{
				CharacterBody.TimedBuff timedBuff = self.timedBuffs[i];
				if (timedBuff.buffIndex == chillBuffIndex)
				{
					if (timedBuff.timer > 0.1f && timedBuff.timer < duration) timedBuff.timer = duration;
				}
			}
		}

		private static void ApplySapped(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!self.HasBuff(RoR2Content.Buffs.AffixBlue)) return;
			if (Catalog.Buff.ZetSapped.buffIndex == BuffIndex.None) return;

			float duration = Configuration.AspectBlueSappedDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
			if (duration > 0.1f) victim.AddTimedBuff(Catalog.Buff.ZetSapped, duration);
		}

		private static void ApplyBurn(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!self.HasBuff(RoR2Content.Buffs.AffixRed)) return;

			if (Configuration.AspectRedBaseBurnDamage.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixRed);
				float damage = Configuration.AspectRedBaseBurnDamage.Value + Configuration.AspectRedStackBurnDamage.Value * (count - 1f);
				float duration = Configuration.AspectRedBurnDuration.Value;
				DotController.DotIndex dotIndex = DotController.DotIndex.Burn;

				if (Configuration.AspectRedUseBase.Value)
				{
					damage *= self.damage;
				}
				else
				{
					damage *= damageInfo.damage;

					if (damageInfo.crit)
					{
						damage *= self.critMultiplier;
					}
				}

				damage *= damageInfo.procCoefficient;
				duration *= damageInfo.procCoefficient;

				if (self.teamComponent.teamIndex != TeamIndex.Player)
				{
					damage *= Configuration.AspectRedMonsterDamageMult.Value;
				}

				Inventory inventory = self.inventory;
				if (inventory)
				{
					count = inventory.GetItemCount(DLC1Content.Items.StrengthenBurn);
					if (count > 0f)
					{
						damage *= 1f + (3f * count);

						dotIndex = DotController.DotIndex.StrongerBurn;
					}
				}

				//Logger.Warn("Damage : " + damageInfo.damage + " - " + "Burning : " + damage + " - " + "duration : " + duration);

				InflictDotPrecise(victim.gameObject, damageInfo.attacker, dotIndex, duration, damage);
			}
		}

		private static void ApplyShredded(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (Compat.EliteReworks.affixHauntedEnabled) return;

			if (!self.HasBuff(RoR2Content.Buffs.AffixHaunted)) return;
			if (Catalog.Buff.ZetShredded.buffIndex == BuffIndex.None) return;

			float duration = Configuration.AspectHauntedShredDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
			if (duration > 0.1f) victim.AddTimedBuff(Catalog.Buff.ZetShredded, duration);
		}

		private static void ApplyCripple(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!self.HasBuff(RoR2Content.Buffs.AffixLunar)) return;

			float duration = Configuration.AspectLunarCrippleDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
			if (duration > 0.1f) victim.AddTimedBuff(RoR2Content.Buffs.Cripple, duration);
		}

		private static void HandlePoach(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (self.HasBuff(Catalog.Buff.AffixEarth))
			{
				float amount = 0f;

				float duration = Configuration.AspectEarthPoachedDuration.Value;
				if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
				if (duration > 0.1f)
				{
					victim.AddTimedBuff(Catalog.Buff.ZetPoached, duration);

					if (Configuration.AspectEarthPoachedLeech.Value > 0f)
					{
						amount += Configuration.AspectEarthPoachedLeech.Value;
					}
				}

				if (Configuration.AspectEarthBaseLeech.Value > 0f)
				{
					float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixEarth);
					amount += Configuration.AspectEarthBaseLeech.Value + Configuration.AspectEarthStackLeech.Value * (count - 1f);
				}

				if (amount > 0)
				{
					amount *= damageInfo.damage;

					if (damageInfo.crit)
					{
						amount *= self.critMultiplier;
					}

					if (self.teamComponent.teamIndex != TeamIndex.Player)
					{
						amount *= Configuration.AspectEarthMonsterLeechMult.Value;
					}

					//Logger.Warn("Damage : " + damageInfo.damage + " - " + "Healing : " + amount);

					self.healthComponent.Heal(amount, damageInfo.procChainMask, true);
				}
			}
			else if (victim.HasBuff(Catalog.Buff.ZetPoached))
			{
				if (Configuration.AspectEarthPoachedLeech.Value > 0f)
				{
					float amount = Configuration.AspectEarthPoachedLeech.Value;
					amount *= damageInfo.damage;

					if (damageInfo.crit)
					{
						amount *= self.critMultiplier;
					}

					if (self.teamComponent.teamIndex != TeamIndex.Player)
					{
						amount *= Configuration.AspectEarthMonsterLeechMult.Value;
					}

					//Logger.Warn("Damage : " + damageInfo.damage + " - " + "Healing : " + amount);

					self.healthComponent.Heal(amount, damageInfo.procChainMask, true);
				}
			}
		}

		private static void ApplyCollapse(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!useCustomFracture) return;

			if (!self.HasBuff(Catalog.Buff.AffixVoid)) return;

			if (Configuration.AspectVoidBaseCollapseDamage.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixVoid);
				float damage = Configuration.AspectVoidBaseCollapseDamage.Value + Configuration.AspectVoidStackCollapseDamage.Value * (count - 1f);

				if (Configuration.AspectVoidUseBase.Value)
				{
					damage *= self.damage;
				}
				else
				{
					damage *= damageInfo.damage;

					if (damageInfo.crit)
					{
						damage *= self.critMultiplier;
					}
				}

				if (self.teamComponent.teamIndex != TeamIndex.Player)
				{
					damage *= Configuration.AspectVoidMonsterDamageMult.Value;
				}

				//Logger.Warn("Damage : " + damageInfo.damage + " - " + "Fracture : " + damage);

				InflictFracturePrecise(victim.gameObject, damageInfo.attacker, damage);
			}
		}

		private static void ApplyBleed(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			/*
			if (!Compat.Aetherium.bleedHook) return;

			BuffDef buffDef = Catalog.Buff.AffixSanguine;

			if (!buffDef || !self.HasBuff(buffDef)) return;

			if (Configuration.AspectSanguineBaseDamage.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, buffDef);
				float damage = Configuration.AspectSanguineBaseDamage.Value + Configuration.AspectSanguineStackDamage.Value * (count - 1f);

				if (self.teamComponent.teamIndex != TeamIndex.Player) damage *= Configuration.AspectSanguineMonsterDamageMult.Value;

				InflictDotPrecise(victim.gameObject, damageInfo.attacker, DotController.DotIndex.Bleed, Configuration.AspectSanguineBleedDuration.Value, self.damage * damage);
			}
			*/
		}
		


		internal static void InflictDotPrecise(GameObject victim, GameObject attacker, DotController.DotIndex dotIndex, float duration, float damage)
		{
			DotController.DotDef dotDef = DotController.dotDefs[(int)dotIndex];

			float ticks = duration / dotDef.interval;
			float rTicks = Mathf.Ceil(ticks);

			if (rTicks > 0f)
			{
				CharacterBody atkBody = attacker.GetComponent<CharacterBody>();

				float dotBaseDPS = atkBody.damage * (dotDef.damageCoefficient / dotDef.interval);
				float targetDPS = damage / (rTicks * dotDef.interval);

				float damageMult = targetDPS / dotBaseDPS;
				damageMult *= ticks / (rTicks + 1f);

				float tickedDuration = (rTicks * dotDef.interval) - 0.01f;

				if (dotIndex == DotController.DotIndex.Burn || dotIndex == DotController.DotIndex.StrongerBurn)
				{
					CharacterBody vicBody = victim.GetComponent<CharacterBody>();

					// bypass burn damage being affected by target health
					float dotStackDamage = dotDef.damageCoefficient * atkBody.damage * damageMult;
					float dotStackDamageFromBurnOverride = Mathf.Min(dotStackDamage, vicBody.healthComponent.fullCombinedHealth * 0.01f * damageMult);
					if (dotStackDamage > dotStackDamageFromBurnOverride)
					{
						// will cause severe burns if another mod removes health effect within DotController.AddDot
						damageMult *= dotStackDamage / dotStackDamageFromBurnOverride;
					}

					InflictDotInfo inflictDotInfo = new InflictDotInfo
					{
						attackerObject = attacker,
						victimObject = victim,
						totalDamage = null,
						duration = tickedDuration,
						damageMultiplier = damageMult,
						dotIndex = dotIndex,
						maxStacksFromAttacker = null
					};
					DotController.InflictDot(ref inflictDotInfo);
				}
				else
				{
					DotController.InflictDot(victim, attacker, dotIndex, tickedDuration, damageMult);
				}
			}
		}

		internal static void InflictFracturePrecise(GameObject victim, GameObject attacker, float damage)
		{
			DotController.DotIndex dotIndex = DotController.DotIndex.Fracture;
			DotController.DotDef dotDef = DotController.dotDefs[(int)dotIndex];

			CharacterBody atkBody = attacker.GetComponent<CharacterBody>();

			float dotBaseDPS = atkBody.damage * (dotDef.damageCoefficient / dotDef.interval);
			float targetDPS = damage / dotDef.interval;

			float damageMult = targetDPS / dotBaseDPS;

			DotController.InflictDot(victim, attacker, dotIndex, dotDef.interval, damageMult);
		}



		private static void ShieldRegenHook()
		{
			On.RoR2.HealthComponent.ServerFixedUpdate += (orig, self) =>
			{
				HandleShieldRegen(self);

				orig(self);
			};
		}

		private static void HandleShieldRegen(HealthComponent self)
		{
			if (self.alive)
			{
				CharacterBody body = self.body;
				bool canRegenShield = self.shield >= 1f || body.outOfDangerStopwatch > 2.5f;

				if (self.shield < self.fullShield && body.regen > 0f && canRegenShield)
				{
					float shieldRegen = 0f;

					if (body.HasBuff(Catalog.Buff.AffixLunar)) shieldRegen = Mathf.Max(shieldRegen, Configuration.AspectLunarRegen.Value);

					Inventory inventory = body.inventory;
					if (inventory)
					{
						if (inventory.GetItemCount(RoR2Content.Items.ShieldOnly) > 0) shieldRegen = Mathf.Max(shieldRegen, Configuration.TranscendenceRegen.Value);

						//if (inventory.GetItemCount(RoR2Content.Items.PersonalShield) > 0) shieldRegen = Mathf.Max(shieldRegen, 10f);
					}

					if (shieldRegen > 0f)
					{
						if (!ShieldRegenAccumulator.ContainsKey(body.netId)) ShieldRegenAccumulator.Add(body.netId, 0f);

						float accumulator = ShieldRegenAccumulator[body.netId];

						shieldRegen *= body.regen;
						accumulator += shieldRegen * Time.fixedDeltaTime;

						if (accumulator > 1f)
						{
							float num = Mathf.Floor(accumulator);
							accumulator -= num;
							AddShieldToHealthComponent(self, num);
						}

						ShieldRegenAccumulator[body.netId] = accumulator;
					}
				}
			}
		}

		private static void AddShieldToHealthComponent(HealthComponent self, float value)
		{
			if (!NetworkServer.active)
			{
				Logger.Warn("[Server] function 'System.Void ZetAspects::AddShieldToHealthComponent(ROR2.HealthComponent, System.Single)' called on client");
				return;
			}
			if (!self.alive)
			{
				return;
			}
			if (self.shield < self.fullShield)
			{
				self.Networkshield = Mathf.Min(self.shield + value, self.fullShield);
			}
		}



		private static void PreventVoidEquipmentRemovalHook()
		{
			IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdsfld(typeof(DLC1Content.Elites).GetField("Void")),
					x => x.MatchLdfld<EliteDef>("eliteEquipmentDef"),
					x => x.MatchCallvirt<EquipmentDef>("get_equipmentIndex"),
					x => x.MatchBneUn(out _)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, -2);
				}
				else
				{
					Logger.Warn("PreventVoidEquipmentRemovalHook Failed");
				}
			};
		}

		private static void EquipmentLostBuffHook()
		{
			IL.RoR2.CharacterBody.OnEquipmentLost += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(1),
					x => x.MatchLdfld<EquipmentDef>("passiveBuffDef"),
					x => x.MatchBrfalse(out _)
				);

				if (found)
				{
					c.Index += 2;

					c.EmitDelegate<Func<BuffDef, BuffDef>>((buffDef) =>
					{
						if (!buffDef) return buffDef;

						if (Catalog.GetAspectEquipIndex(buffDef.buffIndex) != EquipmentIndex.None) return null;

						return buffDef;
					});
				}
				else
				{
					Logger.Warn("EquipmentLostBuffHook Failed");
				}
			};
		}

		private static void EquipmentGainedBuffHook()
		{
			IL.RoR2.CharacterBody.OnEquipmentGained += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(1),
					x => x.MatchLdfld<EquipmentDef>("passiveBuffDef"),
					x => x.MatchBrfalse(out _)
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<BuffDef, CharacterBody, BuffDef>>((buffDef, self) =>
					{
						if (!buffDef) return buffDef;

						if (Catalog.GetAspectEquipIndex(buffDef.buffIndex) != EquipmentIndex.None)
						{
							if (self && !DestroyedBodies.ContainsKey(self.netId))
							{
								self.AddTimedBuff(buffDef, BuffCycleDuration);
							}

							return null;
						}

						return buffDef;
					});
				}
				else
				{
					Logger.Warn("EquipmentGainedBuffHook Failed");
				}
			};
		}

		private static void ApplyAspectBuffOnInventoryChangedHook()
		{
			On.RoR2.CharacterBody.OnInventoryChanged += (orig, self) =>
			{
				if (NetworkServer.active && self)
				{
					if (!DestroyedBodies.ContainsKey(self.netId))
					{
						ApplyAspectBuffs(self);
					}
				}

				orig(self);
			};
		}

		private static void RefreshAspectBuffsHook()
		{
			On.RoR2.CharacterBody.UpdateBuffs += (orig, self, deltaTime) =>
			{
				orig(self, deltaTime);

				if (NetworkServer.active && self)
				{
					if (!DestroyedBodies.ContainsKey(self.netId))
					{
						ApplyAspectBuffs(self);
					}
				}
			};
		}

		private static void UpdateOnBuffLostHook()
		{
			On.RoR2.CharacterBody.OnBuffFinalStackLost += (orig, self, buffDef) =>
			{
				orig(self, buffDef);

				if (NetworkServer.active && Run.instance && self)
				{
					if (!DestroyedBodies.ContainsKey(self.netId))
					{
						Inventory inventory = self.inventory;

						if (inventory)
						{
							if (Catalog.HasAspectItemOrEquipment(inventory, buffDef))
							{
								self.AddTimedBuff(buffDef, BuffCycleDuration);
							}
							else
							{
								// update itemBehaviors and itemDisplays
								//self.OnInventoryChanged();
								inventory.GiveItem(Catalog.Item.ZetAspectsUpdateInventory);
							}
						}
					}
				}
			};
		}

		private static void ApplyAspectBuffs(CharacterBody self)
		{
			Inventory inventory = self.inventory;

			if (!inventory) return;

			ApplyAspectBuff(self, inventory, Catalog.Buff.AffixWhite, Catalog.Item.ZetAspectWhite, Catalog.Equip.AffixWhite);
			ApplyAspectBuff(self, inventory, Catalog.Buff.AffixBlue, Catalog.Item.ZetAspectBlue, Catalog.Equip.AffixBlue);
			ApplyAspectBuff(self, inventory, Catalog.Buff.AffixRed, Catalog.Item.ZetAspectRed, Catalog.Equip.AffixRed);
			ApplyAspectBuff(self, inventory, Catalog.Buff.AffixHaunted, Catalog.Item.ZetAspectHaunted, Catalog.Equip.AffixHaunted);
			ApplyAspectBuff(self, inventory, Catalog.Buff.AffixPoison, Catalog.Item.ZetAspectPoison, Catalog.Equip.AffixPoison);
			ApplyAspectBuff(self, inventory, Catalog.Buff.AffixLunar, Catalog.Item.ZetAspectLunar, Catalog.Equip.AffixLunar);

			ApplyAspectBuff(self, inventory, Catalog.Buff.AffixEarth, Catalog.Item.ZetAspectEarth, Catalog.Equip.AffixEarth);
			ApplyAspectBuff(self, inventory, Catalog.Buff.AffixVoid, Catalog.Item.ZetAspectVoid, Catalog.Equip.AffixVoid);
			/*
			if (Catalog.Aetherium.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixSanguine, Catalog.Item.ZetAspectSanguine, Catalog.Equip.AffixSanguine);
			}
			*/
		}

		private static void ApplyAspectBuff(CharacterBody body, Inventory inventory, BuffDef buffDef, ItemDef itemDef, EquipmentDef equipDef)
		{
			if (buffDef && !body.HasBuff(buffDef))
			{
				if (Catalog.HasAspectItemOrEquipment(inventory, itemDef, equipDef)) body.AddTimedBuff(buffDef, BuffCycleDuration);
			}
		}



		private static void VoidBearFix_AddTimedBuff(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
		{
			if (NetworkServer.active)
			{
				BuffIndex buff = DLC1Content.Buffs.BearVoidCooldown.buffIndex;

				if (buffDef.buffIndex == buff && self.GetBuffCount(buff) < 1)
				{
					self.ClearTimedBuffs(buff);
					self.SetBuffCount(buff, 0);
				}
			}

			orig(self, buffDef, duration);
		}

		private static void VoidBearFix_RemoveBuff(On.RoR2.CharacterBody.orig_RemoveBuff_BuffIndex orig, CharacterBody self, BuffIndex buffType)
		{
			if (NetworkServer.active)
			{
				BuffIndex buff = DLC1Content.Buffs.BearVoidCooldown.buffIndex;

				if (buffType == buff && self.GetBuffCount(buff) < 1) return;
			}

			orig(self, buffType);
		}
	}
}
