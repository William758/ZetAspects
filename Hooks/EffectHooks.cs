using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using RoR2.UI;
using TMPro;
using HarmonyLib;

namespace TPDespair.ZetAspects
{
	internal static class EffectHooks
	{
		public static List<string> dronesList = new List<string> { "Drone", "Droid", "Robo", "Turret", "Missile", "Laser", "Beam" };

		internal static Dictionary<NetworkInstanceId, float> DestroyedBodies = new Dictionary<NetworkInstanceId, float>();
		internal static Dictionary<NetworkInstanceId, float> ShieldRegenAccumulator = new Dictionary<NetworkInstanceId, float>();

		public static BindingFlags Flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance;
		private static FieldInfo DamageInfoRejectedField;
		private static FieldInfo AffixLunarAvailibilityField;

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
						if (BlightedStateManager.Affixes.ContainsKey(netId))
						{
							BlightedStateManager.DestroyEntry(netId);
						}

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

			DamageInfoRejectedField = typeof(DamageInfo).GetField("rejected");
			Type type = typeof(CharacterBody).GetNestedType("ItemAvailability", Flags);
			if (type != null)
			{
				AffixLunarAvailibilityField = type.GetField("hasAffixLunar", Flags);
			}
			else
			{
				Logger.Warn("Failed to find type : CharacterBody.ItemAvailability");
			}

			EffectManagerHook();
			DodgeHook();

			PlatingHook();
			FallDamageHook();
			KnockBackHook();

			FreezeHook();
			PreventAspectChillHook();
			PreventOverloadingBombHook();
			FireTrailHook();
			PreventAspectBurnHook();
			PoisonBallHook();
			NullDurationHook();
			if (AffixLunarAvailibilityField != null) LunarProjectileHook();
			PreventAspectCrippleHook();
			PreventAspectCollapseHook();
			DamageTakenHook();
			//HealMultHook();
			HeadHunterBuffHook();
			DotAmpHook();
			OnHitAllHook();
			OnHitEnemyHook();
			//FixTimedChillApplication();
			ShieldRegenHook();
			GoldFromKillHook();

			//PreventVoidEquipmentRemovalHook();
			EquipmentLostBuffHook();
			EquipmentGainedBuffHook();
			ApplyAspectBuffOnInventoryChangedHook();
			UpdateOnBuffLostHook();

			On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += BuffInterceptHook;

			//RefreshAspectBuffsHook();
			CharacterBody.onBodyInventoryChangedGlobal += HandleEliteBuffManager;
		}

		internal static void LateSetup()
		{
			if (Catalog.EliteVariety.Enabled)
			{
				ModifyDot();
				ModifyDeploySlot();
			}
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
						Logger.Warn("Unknown SpawnEffect : " + data.genericUInt);
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
			IL.RoR2.HealthComponent.TakeDamageProcess += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(1),
					x => x.MatchLdfld<DamageInfo>("rejected"),
					x => x.MatchBrtrue(out _),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<HealthComponent>("body"),
					x => x.MatchLdsfld(typeof(JunkContent.Buffs).GetField("BodyArmor"))
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldarg, 1);
					c.EmitDelegate<Func<HealthComponent, DamageInfo, bool>>((healthComponent, damageInfo) =>
					{
						if (damageInfo.rejected) return true;

						float avoidChance = 0f;
						float dodgeMultiplier = 1f;

						if (damageInfo.attacker)
						{
							CharacterBody atkBody = damageInfo.attacker.GetComponent<CharacterBody>();
							if (atkBody)
							{
								// attacker is player at close range
								float dodgeRange = Configuration.NearbyPlayerDodgeBypass.Value;
								if (dodgeRange > 1f && atkBody.teamComponent.teamIndex == TeamIndex.Player)
								{
									float distMag = (atkBody.corePosition - damageInfo.position).sqrMagnitude;

									float falloffMag = dodgeRange * dodgeRange;
									if (distMag < falloffMag) return false;

									float halveMag = dodgeRange * 1.5f;
									halveMag *= halveMag;
									if (distMag < halveMag) dodgeMultiplier *= 0.5f;
								}

								if (atkBody.HasBuff(Catalog.Buff.ZetSepiaBlind))
								{
									if (Configuration.AspectSepiaBlindDodgeEffect.Value > 0f)
									{
										avoidChance += Configuration.AspectSepiaBlindDodgeEffect.Value;
									}
								}

								if (atkBody.HasBuff(Catalog.Buff.NightBlind))
								{
									if (Configuration.AspectNightBlindDodgeEffect.Value > 0f)
									{
										avoidChance += Configuration.AspectNightBlindDodgeEffect.Value;
									}
								}

								if (atkBody.HasBuff(Catalog.Buff.SandBlind))
								{
									if (Configuration.AspectCycloneBlindDodgeEffect.Value > 0f)
									{
										avoidChance += Configuration.AspectCycloneBlindDodgeEffect.Value;
									}
								}
							}
						}

						CharacterBody vicBody = healthComponent.body;
						if (vicBody)
						{
							if (vicBody.HasBuff(RoR2Content.Buffs.AffixHaunted) && Configuration.AspectHauntedBaseDodgeGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(vicBody, RoR2Content.Buffs.AffixHaunted);
								avoidChance += Configuration.AspectHauntedBaseDodgeGain.Value + Configuration.AspectHauntedStackDodgeGain.Value * (count - 1f);
							}
							else if (vicBody.HasBuff(RoR2Content.Buffs.AffixHauntedRecipient) && Configuration.AspectHauntedAllyDodgeGain.Value > 0f)
							{
								avoidChance += Configuration.AspectHauntedAllyDodgeGain.Value;
							}

							if (vicBody.HasBuff(Catalog.Buff.AffixSepia) && Configuration.AspectSepiaBaseDodgeGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(vicBody, Catalog.Buff.AffixSepia);
								avoidChance += Configuration.AspectSepiaBaseDodgeGain.Value + Configuration.AspectSepiaStackDodgeGain.Value * (count - 1f);
							}

							if (vicBody.HasBuff(Catalog.Buff.AffixVeiled))
							{
								if (Configuration.AspectVeiledBaseDodgeGain.Value > 0f)
								{
									float count = Catalog.GetStackMagnitude(vicBody, Catalog.Buff.AffixVeiled);
									avoidChance += Configuration.AspectVeiledBaseDodgeGain.Value + Configuration.AspectVeiledStackDodgeGain.Value * (count - 1f);
								}

								if (vicBody.HasBuff(Catalog.Buff.ZetElusive) && Configuration.AspectVeiledElusiveDodgeGain.Value > 0f)
								{
									bool nemCloak = Compat.NemSpikeStrip.VeiledEnabled && Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.VeiledHitToShowField, true);

									float count = Mathf.Max(5f, vicBody.GetBuffCount(Catalog.Buff.ZetElusive));
									avoidChance += Configuration.AspectVeiledElusiveDodgeGain.Value * (count / (nemCloak ? 40f : 20f));
								}
							}

							if (vicBody.HasBuff(Catalog.Buff.AffixSandstorm) && Configuration.AspectCycloneBaseDodgeGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(vicBody, Catalog.Buff.AffixSandstorm);
								avoidChance += Configuration.AspectCycloneBaseDodgeGain.Value + Configuration.AspectCycloneStackDodgeGain.Value * (count - 1f);
							}

							if (vicBody.bodyIndex == Catalog.mithrixBodyIndex || vicBody.bodyIndex == Catalog.voidlingBodyIndex) dodgeMultiplier *= 0.65f;

							if (Catalog.diluvianArtifactIndex != ArtifactIndex.None && vicBody.teamComponent.teamIndex == TeamIndex.Player)
							{
								RunArtifactManager artifactManager = RunArtifactManager.instance;
								if (artifactManager && artifactManager.IsArtifactEnabled(Catalog.diluvianArtifactIndex)) dodgeMultiplier *= 0.5f;
							}
						}

						if (avoidChance > 0f)
						{
							avoidChance = Util.ConvertAmplificationPercentageIntoReductionPercentage(avoidChance);
							avoidChance *= dodgeMultiplier;

							if (Util.CheckRoll(avoidChance, 0f, null))
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
					Logger.Warn("DodgeHook Failed");
				}
			};
		}



		private static void PlatingHook()
		{
			IL.RoR2.HealthComponent.TakeDamageProcess += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcR4(1f),
					x => x.MatchLdloc(7),
					x => x.MatchLdloc(39),
					x => x.MatchMul(),
					x => x.MatchCall<Mathf>("Max"),
					x => x.MatchStloc(7)
				);

				if (found)
				{
					c.Index += 6;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 7);
					c.EmitDelegate<Func<HealthComponent, float, float>>((healthComponent, damage) =>
					{
						float plating = 0f;

						CharacterBody vicBody = healthComponent.body;
						if (vicBody)
						{
							if (!Compat.NemSpikeStrip.PlatedEnabled || Configuration.AspectPlatedAllowDefenceWithNem.Value)
							{
								if (vicBody.HasBuff(Catalog.Buff.AffixPlated) && Configuration.AspectPlatedBasePlatingGain.Value > 0f)
								{
									float count = Catalog.GetStackMagnitude(vicBody, Catalog.Buff.AffixPlated);
									float effectValue = Configuration.AspectPlatedBasePlatingGain.Value + Configuration.AspectPlatedStackPlatingGain.Value * (count - 1f);
									if (vicBody.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectPlatedMonsterPlatingMult.Value;
									plating += effectValue;
								}
							}
						}

						if (plating > 0f)
						{
							damage = Mathf.Max(1f, damage - plating);
						}

						return damage;
					});
					c.Emit(OpCodes.Stloc, 7);
				}
				else
				{
					Logger.Warn("PlatingHook Failed");
				}
			};
		}

		private static void FallDamageHook()
		{
			IL.RoR2.GlobalEventManager.OnCharacterHitGroundServer += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(7)
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldloc, 6);
					c.Emit(OpCodes.Ldarg, 1);
					c.EmitDelegate<Func<float, CharacterBody, float>>((damage, body) =>
					{
						if (body.HasBuff(Catalog.Buff.AffixWarped) && Configuration.AspectWarpedBaseFallReductionGain.Value > 0f)
						{
							float effect = Catalog.GetStackMagnitude(body, Catalog.Buff.AffixWarped);
							effect = Configuration.AspectWarpedBaseFallReductionGain.Value + Configuration.AspectWarpedStackFallReductionGain.Value * (effect - 1f);
							effect = 1f - (0.01f * Util.ConvertAmplificationPercentageIntoReductionPercentage(100f * effect));
							if (effect <= 0.001f) effect /= body.maxHealth;

							damage *= effect;
						}

						return damage;
					});
					c.Emit(OpCodes.Stloc, 6);
				}
				else
				{
					Logger.Warn("FallDamageHook Failed");
				}
			};
		}

		private static void KnockBackHook()
		{
			On.RoR2.HealthComponent.TakeDamageProcess += (orig, self, damageInfo) =>
			{
				if (self)
				{
					if (damageInfo.canRejectForce)
					{
						CharacterBody body = self.body;
						if (body)
						{
							if (body.HasBuff(Catalog.Buff.AffixWarped) && Configuration.AspectWarpedBaseForceResistGain.Value > 0f)
							{
								float effect = Catalog.GetStackMagnitude(body, Catalog.Buff.AffixWarped);
								effect = Configuration.AspectWarpedBaseForceResistGain.Value + Configuration.AspectWarpedStackForceResistGain.Value * (effect - 1f);
								effect = 1f - (0.01f * Util.ConvertAmplificationPercentageIntoReductionPercentage(100f * effect));
								if (effect <= 0.001f) effect = 0f;

								damageInfo.force *= effect;
							}
						}
					}
				}

				orig(self, damageInfo);
			};
		}



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
			/*
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
			*/

			IL.RoR2.GlobalEventManager.ProcessHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(10),
					x => x.MatchLdcI4(0),
					x => x.MatchBle(out _)
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 999);
				}
				else
				{
					Logger.Warn("PreventAspectChillHook Failed");
				}
			};
		}

		private static void PreventOverloadingBombHook()
		{
			IL.RoR2.GlobalEventManager.OnHitAllProcess += (il) =>
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
			IL.RoR2.GlobalEventManager.ProcessHitEnemy += (il) =>
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
			IL.RoR2.GlobalEventManager.ProcessHitEnemy += (il) =>
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
					x => x.MatchLdfld(AffixLunarAvailibilityField)
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
			IL.RoR2.HealthComponent.TakeDamageProcess += (il) =>
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
			IL.RoR2.GlobalEventManager.ProcessHitEnemy += (il) =>
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
			IL.RoR2.HealthComponent.TakeDamageProcess += (il) =>
			{
				ILCursor c = new ILCursor(il);

				const int damageValue = 7;

				// find : store damageinfo.damage into variable
				bool found = c.TryGotoNext(
					x => x.MatchLdarg(1),
					x => x.MatchLdfld<DamageInfo>("damage"),
					x => x.MatchStloc(damageValue)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Ldloc, damageValue);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldarg, 1);
					c.EmitDelegate<Func<float, HealthComponent, DamageInfo, float>>((damage, healthComponent, damageInfo) =>
					{
						CharacterBody vicBody = healthComponent.body;
						if (vicBody)
						{
							if (vicBody.HasBuff(RoR2Content.Buffs.HealingDisabled) && Configuration.AspectPoisonNullDamageTaken.Value != 0f)
							{
								float extraDamage = Mathf.Abs(Configuration.AspectPoisonNullDamageTaken.Value);
								damage *= 1f + extraDamage;
							}

							float reduction = 0f;

							if ((!Catalog.nemBarrier && vicBody.HasBuff(Catalog.Buff.AffixBarrier)) || (Catalog.nemBarrier && vicBody.HasBuff(Catalog.Buff.AffixBuffered)))
							{
								if (healthComponent.barrier > 0f)
								{
									float cfgValue = Compat.RisingTides.GetConfigValue(Compat.RisingTides.BarrierDamageResistance, 50f);

									damage /= 1f - cfgValue / 100f;
								}
							}

							if (vicBody.HasBuff(Catalog.Buff.AffixBarrier))
							{
								if (healthComponent.barrier > 0f)
								{
									if (Configuration.AspectBarrierBaseBarrierDamageReductionGain.Value > 0f)
									{
										float count = Catalog.GetStackMagnitude(vicBody, Catalog.Buff.AffixBarrier);
										float effectValue = Configuration.AspectBarrierBaseBarrierDamageReductionGain.Value + Configuration.AspectBarrierStackBarrierDamageReductionGain.Value * (count - 1f);

										reduction += effectValue;
									}
								}

								if (Configuration.AspectBarrierBaseDamageReductionGain.Value > 0f)
								{
									float count = Catalog.GetStackMagnitude(vicBody, Catalog.Buff.AffixBarrier);
									float effectValue = Configuration.AspectBarrierBaseDamageReductionGain.Value + Configuration.AspectBarrierStackDamageReductionGain.Value * (count - 1f);

									reduction += effectValue;
								}
							}

							if (vicBody.HasBuff(Catalog.Buff.AffixBuffered))
							{
								if (healthComponent.barrier > 0f)
								{
									if (Configuration.AspectBufferedBaseBarrierDamageReductionGain.Value > 0f)
									{
										float count = Catalog.GetStackMagnitude(vicBody, Catalog.Buff.AffixBuffered);
										float effectValue = Configuration.AspectBufferedBaseBarrierDamageReductionGain.Value + Configuration.AspectBufferedStackBarrierDamageReductionGain.Value * (count - 1f);

										reduction += effectValue;
									}
								}

								if (Configuration.AspectBufferedBaseDamageReductionGain.Value > 0f)
								{
									float count = Catalog.GetStackMagnitude(vicBody, Catalog.Buff.AffixBuffered);
									float effectValue = Configuration.AspectBufferedBaseDamageReductionGain.Value + Configuration.AspectBufferedStackDamageReductionGain.Value * (count - 1f);

									reduction += effectValue;
								}
							}

							bool echoEnabled = Catalog.MoreElites.Enabled && Configuration.AspectEchoBaseMinionDamageResistGain.Value > 0f;
							bool tinkerEnabled = Catalog.EliteVariety.Enabled && Configuration.AspectTinkerBaseMinionDamageResistGain.Value > 0f;

							// should we check minion owners for damage reduction sources?
							if (echoEnabled || tinkerEnabled)
							{
								CharacterMaster master = vicBody.master;
								if (master)
								{
									// echo will check all minions , if tinker only ; only check drones
									bool isDrone = tinkerEnabled && IsValidDrone(master);
									if (echoEnabled || isDrone)
									{
										CharacterBody ownerBody = GetMinionOwnerBody(master);
										if (ownerBody)
										{
											if (ownerBody.HasBuff(Catalog.Buff.AffixEcho))
											{
												float count = Catalog.GetStackMagnitude(ownerBody, Catalog.Buff.AffixEcho);
												float effectValue = Configuration.AspectEchoBaseMinionDamageResistGain.Value + Configuration.AspectEchoStackMinionDamageResistGain.Value * (count - 1f);

												reduction += effectValue;
											}

											if (isDrone && ownerBody.HasBuff(Catalog.Buff.AffixTinkerer))
											{
												float count = Catalog.GetStackMagnitude(ownerBody, Catalog.Buff.AffixTinkerer);
												float effectValue = Configuration.AspectTinkerBaseMinionDamageResistGain.Value + Configuration.AspectTinkerStackMinionDamageResistGain.Value * (count - 1f);

												reduction += effectValue;
											}
										}
									}
								}
							}

							if (reduction > 0f)
							{
								damage *= 1f / (1f + reduction);
							}
						}

						return damage;
					});
					c.Emit(OpCodes.Stloc, damageValue);
				}
				else
				{
					Logger.Warn("DamageTakenHook Failed");
				}
			};
		}
		
		internal static bool IsValidDrone(CharacterMaster minionMaster)
		{
			return dronesList.Exists((droneSubstring) => { return minionMaster.name.Contains(droneSubstring); });
		}
		
		internal static CharacterBody GetMinionOwnerBody(CharacterMaster minionMaster)
		{
			MinionOwnership minionOwnership = minionMaster.minionOwnership;
			if (!minionOwnership) return null;
			CharacterMaster ownerMaster = minionOwnership.ownerMaster;
			if (!ownerMaster) return null;

			return ownerMaster.GetBody();
		}
		

		/*
		private static void HealMultHook()
		{
			IL.RoR2.HealthComponent.Heal += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdflda<HealthComponent>("itemCounts"),
					x => x.MatchLdfld(typeof(HealthComponent.ItemCounts).GetField("increaseHealing"))
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldarg, 1);
					c.EmitDelegate<Func<HealthComponent, float, float>>((healthComponent, healAmount) =>
					{
						float mult = 1f;

						return healAmount *= mult;
					});
					c.Emit(OpCodes.Starg, 1);

					c.Emit(OpCodes.Ldarg, 0);
				}
				else
				{
					Logger.Warn("HealMultHook Failed!");
				}
			};
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
					x => x.MatchLdloc(75),
					x => x.MatchConvR4(),
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchStloc(77)
				);

				if (found)
				{
					c.Index += 6;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc, 75);
					c.EmitDelegate<Func<int, float>>((count) =>
					{
						return Configuration.HeadHunterBaseDuration.Value + Configuration.HeadHunterStackDuration.Value * (count - 1);
					});
					c.Emit(OpCodes.Dup);
					c.Emit(OpCodes.Ldloc, 15);
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
		
		private static void ModifyDot()
		{
			On.RoR2.DotController.InflictDot_GameObject_GameObject_DotIndex_float_float_Nullable1 += (orig, attacker, victim, index, duration, damage, wat) =>
			{
				DotController.DotIndex dotIndex = Catalog.impaleDotIndex;
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

				orig(attacker, victim, index, duration, damage, wat);
			};
		}

		private static void ModifyDeploySlot()
		{
			On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += (orig, self, slot) =>
			{
				DeployableSlot deploySlot = Catalog.tinkerDeploySlot;
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
					
					if (atkBody.HasBuff(Catalog.Buff.AffixRealgar) && Configuration.AspectRealgarBaseDotAmp.Value > 0f)
					{
						float count = Catalog.GetStackMagnitude(atkBody, Catalog.Buff.AffixRealgar);
						mult += Configuration.AspectRealgarBaseDotAmp.Value + Configuration.AspectRealgarStackDotAmp.Value * (count - 1f);
					}
					
					if (atkBody.HasBuff(Catalog.Buff.AffixSanguine) && Configuration.AspectSanguineBaseDotAmp.Value > 0f)
					{
						float count = Catalog.GetStackMagnitude(atkBody, Catalog.Buff.AffixSanguine);
						mult += Configuration.AspectSanguineBaseDotAmp.Value + Configuration.AspectSanguineStackDotAmp.Value * (count - 1f);
					}

					if (atkBody.HasBuff(Catalog.Buff.AffixImpPlane) && Configuration.AspectImpaleBaseDotAmp.Value > 0f)
					{
						float count = Catalog.GetStackMagnitude(atkBody, Catalog.Buff.AffixImpPlane);
						mult += Configuration.AspectImpaleBaseDotAmp.Value + Configuration.AspectImpaleStackDotAmp.Value * (count - 1f);
					}

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
				fuseOverride = Configuration.AspectBlueBombDuration.Value
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
					ApplyBleed(attacker, victim, damageInfo);
					ApplySepiaBlind(attacker, victim, damageInfo);
					ApplyElusive(attacker, damageInfo);
					ApplyNemCloak(attacker, damageInfo);
					ApplyWarped(attacker, victim, damageInfo);
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
			if (Catalog.borboFrostBlade || Configuration.AspectWhiteThornsProc.Value) procMask.AddProc(ProcType.Thorns);

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
		/*
		 * moved to BuffInterceptHook
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
		*/
		private static void ApplyChill(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!CanApplyAspectChill(self)) return;

			float duration = Configuration.AspectWhiteSlowDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;

			if (duration > 0.1f)
			{
				EffectManager.SimpleImpactEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/AffixWhiteImpactEffect"), damageInfo.position, Vector3.up, true);
				ChillApplication(victim, duration);
			}
		}

		private static void ChillApplication(CharacterBody victim, float duration)
		{
			if (duration < 0.25f) duration = 0.25f;

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
			if (!Catalog.Buff.ZetSapped) return;

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
			if (!Compat.EliteReworks.affixHauntedEnabled || !Compat.EliteReworks.affixHauntedOnHit)
			{
				if (!self.HasBuff(RoR2Content.Buffs.AffixHaunted)) return;
				if (Catalog.Buff.ZetShredded.buffIndex == BuffIndex.None) return;

				float duration = Configuration.AspectHauntedShredDuration.Value;
				if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
				if (duration > 0.1f) victim.AddTimedBuff(Catalog.Buff.ZetShredded, duration);
			}
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
			bool atkMending = self.HasBuff(Catalog.Buff.AffixEarth);
			bool vicPoached = victim.HasBuff(Catalog.Buff.ZetPoached);

			if (!(atkMending || vicPoached)) return;

			//Logger.Warn("--------");

			float healAmount = 0f;
			float damage = damageInfo.damage;
			// LifeStealBuff does not account for any damage boosts
			//if (damageInfo.crit) damage *= self.critMultiplier;

			if (atkMending)
			{
				float duration = Configuration.AspectEarthPoachedDuration.Value;
				if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
				if (duration > 0.1f)
				{
					victim.AddTimedBuff(Catalog.Buff.ZetPoached, duration);

					vicPoached = true;
				}

				float leechFraction = Configuration.AspectEarthBaseLeech.Value;
				if (leechFraction > 0f)
				{
					//Logger.Warn("--Apply BaseLeech");
					healAmount += GetModifiedHealAmountFromDamage(damage, leechFraction);
					//Logger.Warn("--HealPool : " + healAmount);

					float stackFraction = Configuration.AspectEarthStackLeech.Value;
					if (stackFraction > 0f)
					{
						float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixEarth);
						healAmount *= (leechFraction + stackFraction * (count - 1f)) / leechFraction;

						//Logger.Warn("--Apply EffectiveLeechMultiplier");
						//Logger.Warn("--HealPool : " + healAmount);
					}
				}
			}

			if (vicPoached)
			{
				float poachFraction = Configuration.AspectEarthPoachedLeech.Value;
				if (poachFraction > 0f)
				{
					//Logger.Warn("--Apply PoachLeech");
					healAmount += GetModifiedHealAmountFromDamage(damage, poachFraction);
					//Logger.Warn("--HealPool : " + healAmount);
				}
			}

			if (healAmount > 0f)
			{
				if (self.teamComponent.teamIndex != TeamIndex.Player)
				{
					healAmount *= Configuration.AspectEarthMonsterLeechMult.Value;
				}

				self.healthComponent.Heal(healAmount, damageInfo.procChainMask, true);
			}
		}

		private static float GetModifiedHealAmountFromDamage(float damage, float baseFraction)
		{
			float amount = damage * baseFraction;

			//Logger.Warn("--GettingModifiedHealFromDamage");
			//Logger.Warn("----Amount : " + damage + " * " + baseFraction + " = " + amount);

			if (amount >= 1.25f)
			{
				int leechMod = Configuration.AspectEarthLeechModifier.Value;
				if (leechMod > 0)
				{
					float modMult = Configuration.AspectEarthLeechModMult.Value;
					float modParameter = Configuration.AspectEarthLeechParameter.Value;
					float postModMult = Configuration.AspectEarthLeechPostModMult.Value;

					if (leechMod == 1)
					{
						amount = Mathf.Max(1.25f, amount * modMult);
						amount = Mathf.Pow(amount, modParameter) * postModMult;
					}
					if (leechMod == 2)
					{
						amount = Mathf.Max(1.25f, amount * modMult);
						amount = Mathf.Log(amount, modParameter) * postModMult;
					}

					amount = Mathf.Max(1.25f, amount);
				}
			}

			//Logger.Warn("--ApplyModifiedHealFormula");
			//Logger.Warn("----Amount : " + amount);

			return amount;
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
		}

		private static void ApplySepiaBlind(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			BuffDef buffDef = Catalog.Buff.AffixSepia;
			if (!buffDef || !self.HasBuff(buffDef)) return;

			BuffDef appliedBuff = Catalog.Buff.ZetSepiaBlind;
			if (!appliedBuff) return;

			float duration = Configuration.AspectSepiaBlindDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
			if (duration > 0.1f) victim.AddTimedBuff(appliedBuff, duration);
		}

		private static void ApplyElusive(CharacterBody self, DamageInfo damageInfo)
		{
			if (!Compat.PlasmaSpikeStrip.cloakHook) return;

			if (damageInfo.procCoefficient < 0.245f) return;

			BuffDef eliteBuff = Catalog.Buff.AffixVeiled;
			if (!eliteBuff || !self.HasBuff(eliteBuff)) return;

			bool nemCloak = Compat.NemSpikeStrip.VeiledEnabled && Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.VeiledHitToShowField, true);
			bool cloakOnly = Configuration.AspectVeiledCloakOnly.Value;

			if (nemCloak && (cloakOnly || self.HasBuff(Catalog.veiledCooldown))) return;

			float duration = Configuration.AspectVeiledElusiveDuration.Value;
			bool refresh = Configuration.AspectVeiledElusiveRefresh.Value;
			if (cloakOnly)
			{
				if (duration > 0.1f && (refresh || !self.HasBuff(RoR2Content.Buffs.Cloak)))
				{
					ApplyCloak(self, damageInfo, duration);
				}

				return;
			}

			if (!Configuration.ValidElusiveModifier) return;

			BuffDef elusiveBuff = Catalog.Buff.ZetElusive;
			if (!elusiveBuff) return;

			if (!refresh && self.HasBuff(elusiveBuff)) return;

			if (duration > 0.1f)
			{
				float effect = Catalog.GetStackMagnitude(self, eliteBuff);
				effect = 1f + Configuration.AspectVeiledElusiveStackEffect.Value * (effect - 1f);
				int count = Mathf.RoundToInt(effect * 20f);

				bool effectDecay = Configuration.AspectVeiledElusiveDecay.Value;
				float decayInterval = duration / 20f;

				float buffDuration = effectDecay ? (decayInterval * count) : duration;
				if (buffDuration - GetFirstBuffTimer(self, elusiveBuff) >= 0.5f)
				{
					self.ClearTimedBuffs(elusiveBuff.buffIndex);
					self.SetBuffCount(elusiveBuff.buffIndex, 0);

					for (int i = 0; i < count; i++)
					{
						buffDuration = effectDecay ? (decayInterval * (count - i)) : duration;

						self.AddTimedBuff(elusiveBuff.buffIndex, buffDuration);
					}

					if (!nemCloak)
					{
						float cloakDuration = effectDecay ? (decayInterval * count) : duration;

						ApplyCloak(self, damageInfo, cloakDuration);
					}
				}
			}
		}

		private static float GetFirstBuffTimer(CharacterBody body, BuffDef buffDef)
		{
			for (int i = 0; i < body.timedBuffs.Count; i++)
			{
				CharacterBody.TimedBuff timedBuff = body.timedBuffs[i];
				if (timedBuff.buffIndex == buffDef.buffIndex)
				{
					return timedBuff.timer;
				}
			}

			return 0f;
		}

		private static void ApplyCloak(CharacterBody self, DamageInfo damageInfo, float duration)
		{
			BuffDef cloakBuff = RoR2Content.Buffs.Cloak;
			if (!self.HasBuff(cloakBuff))
			{
				EffectManager.SpawnEffect(Catalog.SmokeBombMiniPrefab, new EffectData
				{
					origin = damageInfo.attacker.transform.position,
					rotation = damageInfo.attacker.transform.rotation,
					scale = self.bestFitRadius * 0.2f
				}, true);
			}

			self.AddTimedBuff(cloakBuff.buffIndex, duration);
		}

		private static void ApplyNemCloak(CharacterBody self, DamageInfo damageInfo)
		{
			if (!Compat.NemSpikeStrip.VeiledEnabled) return;

			BuffDef eliteBuff = Catalog.Buff.AffixVeiled;
			if (!eliteBuff || !self.HasBuff(eliteBuff)) return;

			// - NemSpikeStrip OnHitEnemy Prefix does not seem to be working ??? - also doesnt account for being on cooldown ?
			if (Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.VeiledHitToShowField, true))
			{
				BuffDef cloakBuff = RoR2Content.Buffs.Cloak;
				if (!self.HasBuff(cloakBuff) && !self.HasBuff(Catalog.veiledCooldown))
				{
					self.AddBuff(cloakBuff.buffIndex);
					EffectManager.SpawnEffect(Catalog.SmokeBombMiniPrefab, new EffectData
					{
						origin = damageInfo.attacker.transform.position,
						rotation = damageInfo.attacker.transform.rotation,
						scale = self.bestFitRadius * 0.2f
					}, true);
				}
			}
		}

		private static void ApplyWarped(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			int config = Configuration.AspectWarpedAltDebuff.Value;
			if (config <= 0) return;

			if (config >= 2 || victim.teamComponent.teamIndex == TeamIndex.Player)
			{
				BuffDef buffDef = Catalog.Buff.AffixWarped;
				if (!buffDef || !self.HasBuff(buffDef)) return;

				BuffDef appliedBuff = Catalog.Buff.ZetWarped;
				if (!appliedBuff) return;

				float duration = 4f * damageInfo.procCoefficient;
				if (duration > 0.1f) victim.AddTimedBuff(appliedBuff, duration);
			}
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



		private static void GoldFromKillHook()
		{
			IL.RoR2.DeathRewards.OnKilledServer += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(2)
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldloc, 0);
					c.Emit(OpCodes.Ldloc, 2);
					c.EmitDelegate<Func<CharacterBody, uint, uint>>((atkBody, reward) =>
					{
						float mult = 1f;

						if (atkBody.HasBuff(Catalog.Buff.AffixMoney) && Configuration.AspectMoneyStackGoldMult.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(atkBody, Catalog.Buff.AffixMoney);
							mult += Configuration.AspectMoneyBaseGoldMult.Value + Configuration.AspectMoneyStackGoldMult.Value * (count - 1);
						}

						if (mult > 1f)
						{
							return (uint)(reward * mult);
						}

						return reward;
					});
					c.Emit(OpCodes.Stloc, 2);
				}
				else
				{
					Logger.Warn("GoldFromKillHook Failed");
				}
			};
		}



		private static void ShieldRegenHook()
		{
			On.RoR2.HealthComponent.ServerFixedUpdate += (orig, self, deltaTime) =>
			{
				HandleShieldRegen(self, deltaTime);

				orig(self, deltaTime);
			};
		}

		private static void HandleShieldRegen(HealthComponent self, float deltaTime)
		{
			if (self.alive)
			{
				CharacterBody body = self.body;
				bool destroyed = DestroyedBodies.ContainsKey(body.netId);
				bool canRegenShield = self.shield >= 1f || body.outOfDangerStopwatch > Configuration.ShieldRegenBreakDelay.Value;

				if (self.shield < self.fullShield && body.regen > 0f && canRegenShield && !destroyed)
				{
					float shieldRegen = 0f;

					if (body.HasBuff(Catalog.Buff.AffixNullifier)) shieldRegen = Mathf.Max(shieldRegen, Configuration.AspectNullifierRegen.Value);
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
						//accumulator += shieldRegen * Time.fixedDeltaTime;
						accumulator += shieldRegen * deltaTime;

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



		/*
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
		*/

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
								if (Catalog.Buff.AffixEcho && buffDef == Catalog.Buff.AffixEcho)
								{
									self.AddTimedBuff(Catalog.Buff.ZetEchoPrimer, 0.1f);
								}
								else
								{
									if (BodyAllowedAffix(self, buffDef))
									{
										self.AddTimedBuff(buffDef, 1.25f);
									}
								}
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
						EliteBuffManager manager = self.GetComponent<EliteBuffManager>();
						if (manager)
						{
							manager.RefreshEliteBuffs();
						}

						BlightedStateManager.ApplyBlightedAspectBuffs(self);

						ApplyAspectBuffs(self);
					}
				}

				orig(self);
			};
		}

		private static void HandleEliteBuffManager(CharacterBody body)
		{
			if (NetworkServer.active)
			{
				int count = DestroyedBodies.ContainsKey(body.netId) ? 0 : body.eliteBuffCount;
				body.AddItemBehavior<EliteBuffManager>(count);

				if (body.HasBuff(Catalog.Buff.AffixVeiled))
				{
					bool nemCloak = Compat.NemSpikeStrip.VeiledEnabled && Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.VeiledHitToShowField, true);
					if (nemCloak)
					{
						if (!body.HasBuff(Catalog.veiledCooldown) && !body.HasBuff(RoR2Content.Buffs.Cloak))
						{
							body.AddTimedBuff(Catalog.veiledCooldown, 0.1f);
						}

						if (body.HasBuff(Catalog.veiledCooldown) && body.HasBuff(Catalog.Buff.ZetElusive))
						{
							body.ClearTimedBuffs(Catalog.Buff.ZetElusive);
						}
					}
				}

				/*
				count = body.HasBuff(Catalog.Buff.AffixMoney) ? Mathf.RoundToInt(100f * Catalog.GetStackMagnitude(body, Catalog.Buff.AffixMoney)) : 0 ;
				body.AddItemBehavior<GoldPowerBehavior>(count);
				*/
			}
		}

		/*
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
		*/

		private static void UpdateOnBuffLostHook()
		{
			On.RoR2.CharacterBody.OnBuffFinalStackLost += (orig, self, buffDef) =>
			{
				orig(self, buffDef);

				if (!self || buffDef.buffIndex == BuffIndex.None || !NetworkServer.active || !Run.instance) return;

				if (!DestroyedBodies.ContainsKey(self.netId))
				{
					Inventory inventory = self.inventory;
					if (inventory)
					{
						bool aspectBuff = false;
						BuffDef buffToCheck = buffDef;

						if (Catalog.Buff.ZetEchoPrimer && buffDef == Catalog.Buff.ZetEchoPrimer) buffToCheck = Catalog.Buff.AffixEcho;

						if (buffToCheck.buffIndex == Catalog.lampBuff) return;

						if (Catalog.aspectBuffIndexes.Contains(buffToCheck.buffIndex))
						{
							aspectBuff = true;

							if (BodyAllowedAffix(self, buffToCheck) && Catalog.HasAspectItemOrEquipment(inventory, buffToCheck))
							{
								self.AddTimedBuff(buffToCheck, BuffCycleDuration);
							}
						}

						// update itemBehaviors and itemDisplays
						int updateMode = Configuration.UpdateInventoryFromBuff.Value;
						if (updateMode > 0 && (updateMode > 1 || aspectBuff))
						{
							//Logger.Warn("UpdateInventory : [" + buffDef.buffIndex + "] " + buffDef.name);
							inventory.GiveItem(Catalog.Item.ZetAspectsUpdateInventory);
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
			if (self.bodyIndex != Catalog.urchinTurretBodyIndex && self.bodyIndex != Catalog.urchinOrbitalBodyIndex)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixPoison, Catalog.Item.ZetAspectPoison, Catalog.Equip.AffixPoison);
			}
			ApplyAspectBuff(self, inventory, Catalog.Buff.AffixLunar, Catalog.Item.ZetAspectLunar, Catalog.Equip.AffixLunar);
			if (self.bodyIndex != Catalog.healOrbBodyIndex)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixEarth, Catalog.Item.ZetAspectEarth, Catalog.Equip.AffixEarth);
			}
			ApplyAspectBuff(self, inventory, Catalog.Buff.AffixVoid, Catalog.Item.ZetAspectVoid, Catalog.Equip.AffixVoid);

			if (Catalog.SpikeStrip.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixAragonite, Catalog.Item.ZetAspectAragonite, Catalog.Equip.AffixAragonite);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixVeiled, Catalog.Item.ZetAspectVeiled, Catalog.Equip.AffixVeiled);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixWarped, Catalog.Item.ZetAspectWarped, Catalog.Equip.AffixWarped);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixPlated, Catalog.Item.ZetAspectPlated, Catalog.Equip.AffixPlated);
			}

			if (Catalog.GoldenCoastPlus.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixGold, Catalog.Item.ZetAspectGold, Catalog.Equip.AffixGold);
			}
			
			if (Catalog.Aetherium.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixSanguine, Catalog.Item.ZetAspectSanguine, Catalog.Equip.AffixSanguine);
			}

			if (Catalog.Bubbet.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixSepia, Catalog.Item.ZetAspectSepia, Catalog.Equip.AffixSepia);
			}

			if (Catalog.WarWisp.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixNullifier, Catalog.Item.ZetAspectNullifier, Catalog.Equip.AffixNullifier);
			}

			if (Catalog.Blighted.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixBlighted, Catalog.Item.ZetAspectBlighted, Catalog.Equip.AffixBlighted);
			}

			if (Catalog.GOTCE.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixBackup, Catalog.Item.ZetAspectBackup, Catalog.Equip.AffixBackup);
			}

			if (Catalog.Thalasso.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixPurity, Catalog.Item.ZetAspectPurity, Catalog.Equip.AffixPurity);
			}

			if (Catalog.RisingTides.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixBarrier, Catalog.Item.ZetAspectBarrier, Catalog.Equip.AffixBarrier);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixBlackHole, Catalog.Item.ZetAspectBlackHole, Catalog.Equip.AffixBlackHole);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixMoney, Catalog.Item.ZetAspectMoney, Catalog.Equip.AffixMoney);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixNight, Catalog.Item.ZetAspectNight, Catalog.Equip.AffixNight);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixWater, Catalog.Item.ZetAspectWater, Catalog.Equip.AffixWater);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixRealgar, Catalog.Item.ZetAspectRealgar, Catalog.Equip.AffixRealgar);
			}

			if (Catalog.NemRisingTides.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixBuffered, Catalog.Item.ZetAspectBuffered, Catalog.Equip.AffixBuffered);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixOppressive, Catalog.Item.ZetAspectOppressive, Catalog.Equip.AffixOppressive);
			}

			if (Catalog.MoreElites.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixEmpowering, Catalog.Item.ZetAspectEmpowering, Catalog.Equip.AffixEmpowering);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixFrenzied, Catalog.Item.ZetAspectFrenzied, Catalog.Equip.AffixFrenzied);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixVolatile, Catalog.Item.ZetAspectVolatile, Catalog.Equip.AffixVolatile);
				if (inventory.GetItemCount(Catalog.summonedEcho) == 0)
				{
					if (Catalog.Buff.AffixEcho && !self.HasBuff(Catalog.Buff.AffixEcho))
					{
						if (Catalog.HasAspectItemOrEquipment(inventory, Catalog.Item.ZetAspectEcho, Catalog.Equip.AffixEcho)) self.AddTimedBuff(Catalog.Buff.ZetEchoPrimer, 0.1f);
					}
				}
			}

			if (Catalog.EliteVariety.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixArmored, Catalog.Item.ZetAspectArmor, Catalog.Equip.AffixArmored);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixBuffing, Catalog.Item.ZetAspectBanner, Catalog.Equip.AffixBuffing);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixImpPlane, Catalog.Item.ZetAspectImpale, Catalog.Equip.AffixImpPlane);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixPillaging, Catalog.Item.ZetAspectGolden, Catalog.Equip.AffixPillaging);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixSandstorm, Catalog.Item.ZetAspectCyclone, Catalog.Equip.AffixSandstorm);
				if (self.bodyIndex != Catalog.tinkerDroneBodyIndex)
				{
					ApplyAspectBuff(self, inventory, Catalog.Buff.AffixTinkerer, Catalog.Item.ZetAspectTinker, Catalog.Equip.AffixTinkerer);
				}
			}

			if (Catalog.Augmentum.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixAdaptive, Catalog.Item.ZetAspectAdaptive, Catalog.Equip.AffixAdaptive);
			}

			if (Catalog.Sandswept.populated)
			{
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixMotivator, Catalog.Item.ZetAspectMotivator, Catalog.Equip.AffixMotivator);
				ApplyAspectBuff(self, inventory, Catalog.Buff.AffixOsmium, Catalog.Item.ZetAspectOsmium, Catalog.Equip.AffixOsmium);
			}
		}

		private static void ApplyAspectBuff(CharacterBody body, Inventory inventory, BuffDef buffDef, ItemDef itemDef, EquipmentDef equipDef)
		{
			if (buffDef && !body.HasBuff(buffDef))
			{
				if (Catalog.HasAspectItemOrEquipment(inventory, itemDef, equipDef)) body.AddTimedBuff(buffDef, BuffCycleDuration);
			}
		}

		private static bool BodyAllowedAffix(CharacterBody body, BuffDef buffDef)
		{
			BodyIndex bodyIndex = body.bodyIndex;

			if ((bodyIndex == Catalog.urchinTurretBodyIndex || bodyIndex == Catalog.urchinOrbitalBodyIndex) && buffDef == Catalog.Buff.AffixPoison) return false;
			if (bodyIndex == Catalog.healOrbBodyIndex && buffDef == Catalog.Buff.AffixEarth) return false;
			if (bodyIndex == Catalog.tinkerDroneBodyIndex && buffDef == Catalog.Buff.AffixTinkerer) return false;

			Inventory inventory = body.inventory;
			if (inventory != null)
			{
				if (Catalog.Buff.AffixEcho && buffDef == Catalog.Buff.AffixEcho && inventory.GetItemCount(Catalog.summonedEcho) != 0) return false;
			}

			return true;
		}



		private static void BuffInterceptHook(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
		{
			if (NetworkServer.active)
			{
				if (buffDef == RoR2Content.Buffs.Slow80)
				{
					if (duration < 0.1f) return;
					if (Catalog.limitChillStacks && self.GetBuffCount(RoR2Content.Buffs.Slow80) >= 10) return;
				}

				BuffIndex buffIndex = buffDef.buffIndex;
				if (buffIndex != BuffIndex.None)
				{
					if (buffIndex == Catalog.altSlow80)
					{
						if (duration < 0.1f) return;
						ChillApplication(self, duration);
						return;
					}

					if (buffIndex == Catalog.antiGrav)
					{
						int config = Configuration.AspectWarpedAltDebuff.Value;
						if (self.teamComponent.teamIndex == TeamIndex.Player)
						{
							if (config > 0) return;
						}
						else
						{
							if (config > 1) return;
						}
					}
				}
			}

			orig(self, buffDef, duration);
		}



		private static void VoidBearFix_AddTimedBuff(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
		{
			if (buffDef == null) return;

			if (NetworkServer.active)
			{
				BuffIndex buffIndex = DLC1Content.Buffs.BearVoidCooldown.buffIndex;
				if (buffDef.buffIndex == buffIndex)
				{
					if (self.GetBuffCount(buffIndex) < 1)
					{
						self.ClearTimedBuffs(buffIndex);
						self.SetBuffCount(buffIndex, 0);
					}

					if (duration > 0f)
					{
						float remainingDuration = duration;
						while (remainingDuration > 0f)
						{
							orig(self, buffDef, remainingDuration);
							remainingDuration -= 1f;
						}
					}

					return;
				}
			}

			orig(self, buffDef, duration);
		}

		private static void VoidBearFix_RemoveBuff(On.RoR2.CharacterBody.orig_RemoveBuff_BuffIndex orig, CharacterBody self, BuffIndex buffType)
		{
			if (NetworkServer.active)
			{
				BuffIndex buffIndex = DLC1Content.Buffs.BearVoidCooldown.buffIndex;

				if (buffType == buffIndex && self.GetBuffCount(buffIndex) < 1) return;
			}

			orig(self, buffType);
		}
	}
}
