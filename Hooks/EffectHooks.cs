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
		private static FieldInfo HealthComponentItemCountsField;
		private static FieldInfo AffixLunarAvailibilityField;

		private static float FixedUpdateStopwatch = 0f;

		internal static bool preventedDefaultOverloadingBomb = false;
		internal static bool preventedDefaultBlazeBurn = false;
		internal static bool preventedDefaultLunarCripple = false;
		internal static bool preventedDefaultVoidCollapse = false;

		internal static bool useCustomOverloadBombs = false;
		internal static bool useCustomFracture = false;



		private static int TakeDamageProcess_DamageLocIndex = -1;

		private static void FindDamageLocIndex(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchLdfld<TeamDef>("friendlyFireScaling")
			);

			if (!found)
			{
				Logger.Warn("FindDamageLocIndex - cound not find TeamDef.friendlyFireScaling");
			}
			else
			{
				goto findStloc;
			}



			found = c.TryGotoNext(
				x => x.MatchCallOrCallvirt<CharacterBody>("get_critMultiplier")
			);

			if (!found)
			{
				Logger.Warn("FindDamageLocIndex - cound not find CharacterBody.get_critMultiplier");
				return;
			}



			findStloc:

			int findIndex = c.Index;

			found = c.TryGotoNext(
				x => x.MatchStloc(out TakeDamageProcess_DamageLocIndex)
			);

			if (!found || (found && c.Index - findIndex > 12))
			{
				Logger.Error("FindDamageLocIndex - could not reliably find DamageLocIndex");
				if (found) Logger.Error("FindOffset:" + (c.Index - findIndex));
				return;
			}



			Logger.Info("FindDamageLocIndex: " + TakeDamageProcess_DamageLocIndex);
		}



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
						foreach (IAspectProvider aspectProvider in EliteBuffManager.Providers)
						{
							aspectProvider.OnBodyDiscard(netId);
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
			HealthComponentItemCountsField = typeof(HealthComponent).GetField("itemCounts", Flags);

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
			ShieldRegenHook();
			GoldFromKillHook();

			On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += BuffInterceptHook;
		}

		internal static void LateSetup()
		{
			if (AspectPackDefOf.EliteVariety.Enabled)
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

		public static bool IsBodyDestroyed(CharacterBody body)
		{
			return IsBodyDestroyed(body.netId);
		}

		public static bool IsBodyDestroyed(NetworkInstanceId netId)
		{
			return DestroyedBodies.ContainsKey(netId);
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
					x => x.MatchLdfld<DamageInfo>("rejected"),
					x => x.MatchBrtrue(out _),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<HealthComponent>("body"),
					x => x.MatchLdsfld(typeof(JunkContent.Buffs).GetField("BodyArmor"))
				);

				if (found)
				{
					// loc.1 DamageInfo on stack
					c.Emit(OpCodes.Dup);
					c.Emit(OpCodes.Dup);
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<DamageInfo, HealthComponent, bool>>((damageInfo, healthComponent) =>
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

								if (atkBody.HasBuff(BuffDefOf.ZetSepiaBlind))
								{
									if (Configuration.AspectSepiaBlindDodgeEffect.Value > 0f)
									{
										avoidChance += Configuration.AspectSepiaBlindDodgeEffect.Value;
									}
								}

								if (atkBody.HasBuff(BuffDefOf.NightBlind))
								{
									if (Configuration.AspectNightBlindDodgeEffect.Value > 0f)
									{
										avoidChance += Configuration.AspectNightBlindDodgeEffect.Value;
									}
								}

								if (atkBody.HasBuff(BuffDefOf.SandBlind))
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
							if (vicBody.HasBuff(BuffDefOf.AffixHaunted) && Configuration.AspectHauntedBaseDodgeGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(vicBody, BuffDefOf.AffixHaunted);
								avoidChance += Configuration.AspectHauntedBaseDodgeGain.Value + Configuration.AspectHauntedStackDodgeGain.Value * (count - 1f);
							}
							else if (vicBody.HasBuff(RoR2Content.Buffs.AffixHauntedRecipient) && Configuration.AspectHauntedAllyDodgeGain.Value > 0f)
							{
								avoidChance += Configuration.AspectHauntedAllyDodgeGain.Value;
							}

							if (vicBody.HasBuff(BuffDefOf.AffixSepia) && Configuration.AspectSepiaBaseDodgeGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(vicBody, BuffDefOf.AffixSepia);
								avoidChance += Configuration.AspectSepiaBaseDodgeGain.Value + Configuration.AspectSepiaStackDodgeGain.Value * (count - 1f);
							}

							if (vicBody.HasBuff(BuffDefOf.AffixVeiled))
							{
								if (Configuration.AspectVeiledBaseDodgeGain.Value > 0f)
								{
									float count = Catalog.GetStackMagnitude(vicBody, BuffDefOf.AffixVeiled);
									avoidChance += Configuration.AspectVeiledBaseDodgeGain.Value + Configuration.AspectVeiledStackDodgeGain.Value * (count - 1f);
								}

								if (vicBody.HasBuff(BuffDefOf.ZetElusive) && Configuration.AspectVeiledElusiveDodgeGain.Value > 0f)
								{
									bool nemCloak = Compat.NemSpikeStrip.VeiledEnabled && Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.VeiledHitToShowField, true);

									float count = Mathf.Max(5f, vicBody.GetBuffCount(BuffDefOf.ZetElusive));
									avoidChance += Configuration.AspectVeiledElusiveDodgeGain.Value * (count / (nemCloak ? 40f : 20f));
								}
							}

							if (vicBody.HasBuff(BuffDefOf.AffixSandstorm) && Configuration.AspectCycloneBaseDodgeGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(vicBody, BuffDefOf.AffixSandstorm);
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
				if (TakeDamageProcess_DamageLocIndex == -1)
				{
					FindDamageLocIndex(il);
				}

				if (TakeDamageProcess_DamageLocIndex == -1)
				{
					Logger.Warn("PlatingHook Cannot Proceed without DamageLocIndex");
					return;
				}



				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdflda(HealthComponentItemCountsField),
					x => x.MatchLdfld<HealthComponent.ItemCounts>("armorPlate")
				);

				if (found)
				{
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, TakeDamageProcess_DamageLocIndex);
					c.EmitDelegate<Func<HealthComponent, float, float>>((healthComponent, damage) =>
					{
						float plating = 0f;

						CharacterBody vicBody = healthComponent.body;
						if (vicBody)
						{
							if (!Compat.NemSpikeStrip.PlatedEnabled || Configuration.AspectPlatedAllowDefenceWithNem.Value)
							{
								if (vicBody.HasBuff(BuffDefOf.AffixPlated) && Configuration.AspectPlatedBasePlatingGain.Value > 0f)
								{
									float count = Catalog.GetStackMagnitude(vicBody, BuffDefOf.AffixPlated);
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
					c.Emit(OpCodes.Stloc, TakeDamageProcess_DamageLocIndex);
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
						if (body.HasBuff(BuffDefOf.AffixWarped) && Configuration.AspectWarpedBaseFallReductionGain.Value > 0f)
						{
							float effect = Catalog.GetStackMagnitude(body, BuffDefOf.AffixWarped);
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
							if (body.HasBuff(BuffDefOf.AffixWarped) && Configuration.AspectWarpedBaseForceResistGain.Value > 0f)
							{
								float effect = Catalog.GetStackMagnitude(body, BuffDefOf.AffixWarped);
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
						if (!attacker.HasBuff(BuffDefOf.AffixWhite) || Configuration.AspectWhiteBaseFreezeChance.Value <= 0f) return;
						if (!state.canBeFrozen || attacker.teamComponent.teamIndex != TeamIndex.Player || damageReport.damageInfo.procCoefficient < 0.105f) return;

						float count = Catalog.GetStackMagnitude(attacker, BuffDefOf.AffixWhite);
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
			IL.RoR2.GlobalEventManager.ProcessHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				int cursorIndex = -1;
				bool found = c.TryGotoNext(
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("Slow80"))
				);

				if (found)
				{
					int slowDurIndex = -1;
					cursorIndex = c.Index;

					found = c.TryGotoNext(
						x => x.MatchLdfld<DamageInfo>("procCoefficient"),
						x => x.MatchMul(),
						x => x.MatchLdloc(out slowDurIndex)
					);

					if (found && c.Index - cursorIndex < 15)
					{
						c.Index = 0;

						found = c.TryGotoNext(
							x => x.MatchLdloc(slowDurIndex),
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
							Logger.Warn("PreventAspectChillHook Failed - Could not find duration check");
						}
					}
					else
					{
						if (found)
						{
							Logger.Warn("PreventAspectChillHook Failed - loc finder advanced too far");
						}
						else
						{
							Logger.Warn("PreventAspectChillHook Failed - Could not find loc index");
						}
					}
				}
				else
				{
					Logger.Warn("PreventAspectChillHook Failed - Could not find Slow80");
				}
			};
		}

		private static void PreventOverloadingBombHook()
		{
			IL.RoR2.GlobalEventManager.OnHitAllProcess += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(out _),
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("AffixBlue")),
					x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff")
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
					x => x.MatchLdloc(out _),
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("AffixRed")),
					x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff")
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
					x => x.MatchLdloc(out _),
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("HealingDisabled")),
					x => x.MatchLdcR4(out _),
					x => x.MatchLdarg(1),
					x => x.MatchLdfld<DamageInfo>("procCoefficient"),
					x => x.MatchMul()
				);

				if (!found)
				{
					Logger.Warn("NullDurationHook Failed - Could not find BuffApplication");
					return;
				}

				int cursorIndex = c.Index += 6;

				int AtkBodyLocIndex = -1;

				found = c.TryGotoPrev(
					x => x.MatchLdloc(out AtkBodyLocIndex),
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("AffixPoison"))
				);

				if (!found)
				{
					Logger.Warn("NullDurationHook Failed - Could not find AtkBodyLocIndex");
					return;
				}



				c.Index = cursorIndex;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldloc, AtkBodyLocIndex);
				c.Emit(OpCodes.Ldarg, 1);
				c.EmitDelegate<Func<CharacterBody, DamageInfo, float>>((self, damageInfo) =>
				{
					float duration = Configuration.AspectPoisonNullDuration.Value;
					if (self.teamComponent.teamIndex != TeamIndex.Player) duration = 8f * damageInfo.procCoefficient;

					return duration;
				});
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
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("AffixLunar")),
					x => x.MatchCallvirt<CharacterBody>("HasBuff")
				);

				if (found)
				{
					c.Index += 2;

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
					x => x.MatchLdloc(out _),
					x => x.MatchLdsfld(typeof(DLC1Content.Buffs).GetField("EliteVoid")),
					x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff")
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
				if (TakeDamageProcess_DamageLocIndex == -1)
				{
					FindDamageLocIndex(il);
				}

				if (TakeDamageProcess_DamageLocIndex == -1)
				{
					Logger.Warn("DamageTakenHook Cannot Proceed without DamageLocIndex");
					return;
				}



				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(TakeDamageProcess_DamageLocIndex)
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldloc, TakeDamageProcess_DamageLocIndex);
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<float, HealthComponent, float>>((damage, healthComponent) =>
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

							if ((!Catalog.nemBarrier && vicBody.HasBuff(BuffDefOf.AffixBarrier)) || (Catalog.nemBarrier && vicBody.HasBuff(BuffDefOf.AffixBuffered)))
							{
								if (healthComponent.barrier > 0f)
								{
									float cfgValue = Compat.RisingTides.GetConfigValue(Compat.RisingTides.BarrierDamageResistance, 50f);

									damage /= 1f - cfgValue / 100f;
								}
							}

							if (vicBody.HasBuff(BuffDefOf.AffixBarrier))
							{
								if (healthComponent.barrier > 0f)
								{
									if (Configuration.AspectBarrierBaseBarrierDamageReductionGain.Value > 0f)
									{
										float count = Catalog.GetStackMagnitude(vicBody, BuffDefOf.AffixBarrier);
										float effectValue = Configuration.AspectBarrierBaseBarrierDamageReductionGain.Value + Configuration.AspectBarrierStackBarrierDamageReductionGain.Value * (count - 1f);

										reduction += effectValue;
									}
								}

								if (Configuration.AspectBarrierBaseDamageReductionGain.Value > 0f)
								{
									float count = Catalog.GetStackMagnitude(vicBody, BuffDefOf.AffixBarrier);
									float effectValue = Configuration.AspectBarrierBaseDamageReductionGain.Value + Configuration.AspectBarrierStackDamageReductionGain.Value * (count - 1f);

									reduction += effectValue;
								}
							}

							if (vicBody.HasBuff(BuffDefOf.AffixBuffered))
							{
								if (healthComponent.barrier > 0f)
								{
									if (Configuration.AspectBufferedBaseBarrierDamageReductionGain.Value > 0f)
									{
										float count = Catalog.GetStackMagnitude(vicBody, BuffDefOf.AffixBuffered);
										float effectValue = Configuration.AspectBufferedBaseBarrierDamageReductionGain.Value + Configuration.AspectBufferedStackBarrierDamageReductionGain.Value * (count - 1f);

										reduction += effectValue;
									}
								}

								if (Configuration.AspectBufferedBaseDamageReductionGain.Value > 0f)
								{
									float count = Catalog.GetStackMagnitude(vicBody, BuffDefOf.AffixBuffered);
									float effectValue = Configuration.AspectBufferedBaseDamageReductionGain.Value + Configuration.AspectBufferedStackDamageReductionGain.Value * (count - 1f);

									reduction += effectValue;
								}
							}

							bool echoEnabled = AspectPackDefOf.MoreElites.Enabled && Configuration.AspectEchoBaseMinionDamageResistGain.Value > 0f;
							bool tinkerEnabled = AspectPackDefOf.EliteVariety.Enabled && Configuration.AspectTinkerBaseMinionDamageResistGain.Value > 0f;

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
											if (ownerBody.HasBuff(BuffDefOf.AffixEcho))
											{
												float count = Catalog.GetStackMagnitude(ownerBody, BuffDefOf.AffixEcho);
												float effectValue = Configuration.AspectEchoBaseMinionDamageResistGain.Value + Configuration.AspectEchoStackMinionDamageResistGain.Value * (count - 1f);

												reduction += effectValue;
											}

											if (isDrone && ownerBody.HasBuff(BuffDefOf.AffixTinkerer))
											{
												float count = Catalog.GetStackMagnitude(ownerBody, BuffDefOf.AffixTinkerer);
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
					c.Emit(OpCodes.Stloc, TakeDamageProcess_DamageLocIndex);
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

				int AtkBodyLocIndex = -1;
				int VicBodyLocIndex = -1;



				bool found = c.TryGotoNext(
					x => x.MatchLdarg(out _),
					x => x.MatchLdfld(typeof(DamageReport).GetField("attackerBody")),
					x => x.MatchStloc(out AtkBodyLocIndex)
				);

				if (!found)
				{
					Logger.Warn("HeadHunterBuffHook:Find AtkBodyLocIndex Failed");

					return;
				}



				c.Index = 0;

				found = c.TryGotoNext(
					x => x.MatchLdarg(out _),
					x => x.MatchLdfld(typeof(DamageReport).GetField("victimBody")),
					x => x.MatchStloc(out VicBodyLocIndex)
				);

				if (!found)
				{
					Logger.Warn("HeadHunterBuffHook:Find VicBodyLocIndex Failed");

					return;
				}



				found = c.TryGotoNext(
					x => x.MatchLdsfld(typeof(RoR2Content.Items).GetField("HeadHunter")),
					x => x.MatchCallOrCallvirt<Inventory>("GetItemCountEffective")
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Ldloc, AtkBodyLocIndex);
					c.Emit(OpCodes.Ldloc, VicBodyLocIndex);
					c.EmitDelegate<Action<int, CharacterBody, CharacterBody>>((count, atkBody, vicBody) =>
					{
						if (count > 0)
						{
							float duration = Configuration.HeadHunterBaseDuration.Value + Configuration.HeadHunterStackDuration.Value * (count - 1);

							for (int k = 0; k < BuffCatalog.eliteBuffIndices.Length; k++)
							{
								BuffIndex buffIndex = BuffCatalog.eliteBuffIndices[k];
								if (vicBody.HasBuff(buffIndex))
								{
									atkBody.AddTimedBuff(buffIndex, duration);
								}
							}

							if (Configuration.HeadHunterBuffEnable.Value)
							{
								atkBody.AddTimedBuff(BuffDefOf.ZetHeadHunter, duration);
							}
						}
					});
					c.Emit(OpCodes.Ldc_I4, 0);

					return;
				}



				if (!Configuration.HeadHunterBuffEnable.Value)
				{
					Logger.Warn("HeadHunterBuffHook Failed");

					return;
				}
				else
				{
					Logger.Warn("HeadHunterBuffHook Failed - Attempting Fallback");
				}



				c.Index = 0;

				found = c.TryGotoNext(
					x => x.MatchLdsfld(typeof(BuffCatalog).GetField("eliteBuffIndices"))
				);

				if (!found)
				{
					Logger.Warn("HeadHunterBuffHookFallback:Find eliteBuffIndices Failed");

					return;
				}



				found = c.TryGotoPrev(MoveType.After,
					x => x.MatchLdfld(typeof(DamageReport).GetField("victimIsElite"))
				);

				if (!found)
				{
					Logger.Warn("HeadHunterBuffHookFallback:Find victimIsElite Failed");

					return;
				}



				c.Emit(OpCodes.Ldloc, AtkBodyLocIndex);
				c.EmitDelegate<Func<bool, CharacterBody, bool>>((eliteVictim, atkBody) =>
				{
					if (eliteVictim)
					{
						int count = atkBody.inventory.GetItemCountEffective(RoR2Content.Items.HeadHunter);
						if (count > 0)
						{
							float duration = Configuration.HeadHunterBaseDuration.Value + Configuration.HeadHunterStackDuration.Value * (count - 1);

							atkBody.AddTimedBuff(BuffDefOf.ZetHeadHunter, duration);
						}
					}

					return eliteVictim;
				});
			};
		}
		/*
		private static void HeadHunterBuffHook()
		{
			IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcR4(3f),
					x => x.MatchLdcR4(5f),
					x => x.MatchLdloc(76),
					x => x.MatchConvR4(),
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchStloc(78)
				);

				if (found)
				{
					c.Index += 6;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc, 76);
					c.EmitDelegate<Func<int, float>>((count) =>
					{
						return Configuration.HeadHunterBaseDuration.Value + Configuration.HeadHunterStackDuration.Value * (count - 1);
					});
					c.Emit(OpCodes.Dup);
					c.Emit(OpCodes.Ldloc, 15);
					c.EmitDelegate<Action<float, CharacterBody>>((duration, attacker) =>
					{
						if (Configuration.HeadHunterBuffEnable.Value) attacker.AddTimedBuff(BuffDefOf.ZetHeadHunter, duration);
					});
				}
				else
				{
					Logger.Warn("HeadHunterBuffHook Failed");
				}
			};
		}
		*/
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
					
					if (atkBody.HasBuff(BuffDefOf.AffixRealgar) && Configuration.AspectRealgarBaseDotAmp.Value > 0f)
					{
						float count = Catalog.GetStackMagnitude(atkBody, BuffDefOf.AffixRealgar);
						mult += Configuration.AspectRealgarBaseDotAmp.Value + Configuration.AspectRealgarStackDotAmp.Value * (count - 1f);
					}
					
					if (atkBody.HasBuff(BuffDefOf.AffixSanguine) && Configuration.AspectSanguineBaseDotAmp.Value > 0f)
					{
						float count = Catalog.GetStackMagnitude(atkBody, BuffDefOf.AffixSanguine);
						mult += Configuration.AspectSanguineBaseDotAmp.Value + Configuration.AspectSanguineStackDotAmp.Value * (count - 1f);
					}

					if (atkBody.HasBuff(BuffDefOf.AffixImpPlane) && Configuration.AspectImpaleBaseDotAmp.Value > 0f)
					{
						float count = Catalog.GetStackMagnitude(atkBody, BuffDefOf.AffixImpPlane);
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

			if (!self.HasBuff(BuffDefOf.AffixBlue)) return;
			if (Configuration.AspectBlueBaseDamage.Value <= 0f) return;

			float damage = damageInfo.damage;
			float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixBlue);

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
			if (!self.HasBuff(BuffDefOf.AffixWhite)) return;
			if (Configuration.AspectWhiteBaseDamage.Value <= 0f) return;

			if (damageInfo.procChainMask.HasProc(ProcType.Thorns)) return;

			GameObject gameObject = self.gameObject;
			TeamIndex teamIndex = self.teamComponent.teamIndex;

			float damage = damageInfo.damage;
			float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixWhite);

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
			if (self.HasBuff(BuffDefOf.AffixWhite)) return true;
			if (Configuration.AspectHauntedSlowEffect.Value && self.HasBuff(BuffDefOf.AffixHaunted)) return true;
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
			if (!self.HasBuff(BuffDefOf.AffixBlue)) return;
			if (!BuffDefOf.ZetSapped) return;

			float duration = Configuration.AspectBlueSappedDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
			if (duration > 0.1f) victim.AddTimedBuff(BuffDefOf.ZetSapped, duration);
		}

		private static void ApplyBurn(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!self.HasBuff(BuffDefOf.AffixRed)) return;

			if (Configuration.AspectRedBaseBurnDamage.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixRed);
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
					count = inventory.GetItemCountEffective(DLC1Content.Items.StrengthenBurn);
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
				if (!self.HasBuff(BuffDefOf.AffixHaunted)) return;
				if (BuffDefOf.ZetShredded.buffIndex == BuffIndex.None) return;

				float duration = Configuration.AspectHauntedShredDuration.Value;
				if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
				if (duration > 0.1f) victim.AddTimedBuff(BuffDefOf.ZetShredded, duration);
			}
		}

		private static void ApplyCripple(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!self.HasBuff(BuffDefOf.AffixLunar)) return;

			float duration = Configuration.AspectLunarCrippleDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
			if (duration > 0.1f) victim.AddTimedBuff(RoR2Content.Buffs.Cripple, duration);
		}

		private static void HandlePoach(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			bool atkMending = self.HasBuff(BuffDefOf.AffixEarth);
			bool vicPoached = victim.HasBuff(BuffDefOf.ZetPoached);

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
					victim.AddTimedBuff(BuffDefOf.ZetPoached, duration);

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
						float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixEarth);
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

			if (!self.HasBuff(BuffDefOf.AffixVoid)) return;

			if (Configuration.AspectVoidBaseCollapseDamage.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixVoid);
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

			BuffDef buffDef = BuffDefOf.AffixSanguine;

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
			BuffDef buffDef = BuffDefOf.AffixSepia;
			if (!buffDef || !self.HasBuff(buffDef)) return;

			BuffDef appliedBuff = BuffDefOf.ZetSepiaBlind;
			if (!appliedBuff) return;

			float duration = Configuration.AspectSepiaBlindDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
			if (duration > 0.1f) victim.AddTimedBuff(appliedBuff, duration);
		}

		private static void ApplyElusive(CharacterBody self, DamageInfo damageInfo)
		{
			if (!Compat.PlasmaSpikeStrip.cloakHitHook) return;

			if (damageInfo.procCoefficient < 0.245f) return;

			BuffDef eliteBuff = BuffDefOf.AffixVeiled;
			if (!eliteBuff || !self.HasBuff(eliteBuff)) return;

			bool nemCloak = Compat.NemSpikeStrip.VeiledEnabled && Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.VeiledHitToShowField, true);
			bool cloakOnly = Configuration.AspectVeiledCloakOnly.Value;

			if (nemCloak && (cloakOnly || self.HasBuff(Catalog.veiledCooldown))) return;
			if (Configuration.AspectVeiledCloakPassive.Value && !self.HasBuff(Catalog.veiledBuffer)) return;

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

			BuffDef elusiveBuff = BuffDefOf.ZetElusive;
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

			BuffDef eliteBuff = BuffDefOf.AffixVeiled;
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
				BuffDef buffDef = BuffDefOf.AffixWarped;
				if (!buffDef || !self.HasBuff(buffDef)) return;

				BuffDef appliedBuff = BuffDefOf.ZetWarped;
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
					DotController.InflictDot(victim, attacker, null, dotIndex, tickedDuration, damageMult);
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

			DotController.InflictDot(victim, attacker, null, dotIndex, dotDef.interval, damageMult);
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

						if (atkBody.HasBuff(BuffDefOf.AffixMoney) && Configuration.AspectMoneyStackGoldMult.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(atkBody, BuffDefOf.AffixMoney);
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

					if (body.HasBuff(BuffDefOf.AffixNullifier)) shieldRegen = Mathf.Max(shieldRegen, Configuration.AspectNullifierRegen.Value);
					if (body.HasBuff(BuffDefOf.AffixLunar)) shieldRegen = Mathf.Max(shieldRegen, Configuration.AspectLunarRegen.Value);

					Inventory inventory = body.inventory;
					if (inventory)
					{
						if (inventory.GetItemCountEffective(RoR2Content.Items.ShieldOnly) > 0) shieldRegen = Mathf.Max(shieldRegen, Configuration.TranscendenceRegen.Value);

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
