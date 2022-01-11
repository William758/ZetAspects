using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace TPDespair.ZetAspects
{
	public static class BlightedStateManager
	{
		public static Dictionary<CharacterBody, BuffIndex[]> Entries = new Dictionary<CharacterBody, BuffIndex[]>();



		internal static void SetElites(CharacterBody body, BuffIndex firstBuff, BuffIndex secondBuff)
		{
			Debug.LogWarning("ZetAspects - BlightedStateManager - SetElites : " + body.name + " - " + body.netId);
			Debug.LogWarning("Setting EliteBuffs : " + BuffCatalog.GetBuffDef(firstBuff).name + " - " + BuffCatalog.GetBuffDef(secondBuff).name);

			if (!Entries.ContainsKey(body)) CreateEntry(body);

			BuffIndex oldFirstBuff = Entries[body][0];
			BuffIndex oldSecondBuff = Entries[body][1];

			Entries[body][0] = firstBuff;
			Entries[body][1] = secondBuff;

			if (NetworkServer.active)
			{
				bool clearBuffs = true;

				Inventory inventory = body.inventory;
				if (inventory && inventory.GetItemCount(RoR2Content.Items.HeadHunter) > 0) clearBuffs = false;
				bool destroyed = EffectHooks.DestroyedBodies.ContainsKey(body.netId);
				if (destroyed) clearBuffs = false;

				if (clearBuffs)
				{
					if (oldFirstBuff != BuffIndex.None) body.ClearTimedBuffs(oldFirstBuff);
					if (oldSecondBuff != BuffIndex.None) body.ClearTimedBuffs(oldSecondBuff);
				}

				if (!destroyed) ApplyBlightedAspectBuffs(body);
			}
		}

		internal static void Destroyed(CharacterBody body)
		{
			if (Entries.ContainsKey(body))
			{
				//Debug.LogWarning("BlightedStateManager - Destroyed : " + body.name + " - " + body.netId);

				BuffIndex firstBuff = Entries[body][0];
				BuffIndex secondBuff = Entries[body][1];

				Entries[body][0] = BuffIndex.None;
				Entries[body][1] = BuffIndex.None;

				if (NetworkServer.active)
				{
					bool clearBuffs = true;

					Inventory inventory = body.inventory;
					if (inventory && inventory.GetItemCount(RoR2Content.Items.HeadHunter) > 0) clearBuffs = false;
					if (EffectHooks.DestroyedBodies.ContainsKey(body.netId)) clearBuffs = false;

					if (clearBuffs)
					{
						//Debug.LogWarning("Clearing EliteBuffs : " + BuffCatalog.GetBuffDef(firstBuff).name + " - " + BuffCatalog.GetBuffDef(secondBuff).name);

						if (firstBuff != BuffIndex.None) body.ClearTimedBuffs(firstBuff);
						if (secondBuff != BuffIndex.None) body.ClearTimedBuffs(secondBuff);
					}
				}

				Entries.Remove(body);
			}
		}



		private static void CreateEntry(CharacterBody body)
		{
			BuffIndex[] buffs = new BuffIndex[] { BuffIndex.None, BuffIndex.None };
			Entries.Add(body, buffs);
		}



		public static bool HasAspectFromBlighted(CharacterBody body, BuffDef buffDef)
		{
			if (Entries.ContainsKey(body))
			{
				BuffIndex buffIndex = buffDef.buffIndex;

				if (buffIndex == BuffIndex.None) return false;
				if (Entries[body][0] == buffIndex) return true;
				if (Entries[body][1] == buffIndex) return true;
			}

			return false;
		}

		internal static void ApplyBlightedAspectBuffs(CharacterBody body)
		{
			if (Entries.ContainsKey(body))
			{
				BuffIndex targetBuff;

				targetBuff = Entries[body][0];
				if (targetBuff != BuffIndex.None && !body.HasBuff(targetBuff))
				{
					body.AddTimedBuff(targetBuff, EffectHooks.BuffCycleDuration);
				}
				targetBuff = Entries[body][1];
				if (targetBuff != BuffIndex.None && !body.HasBuff(targetBuff))
				{
					body.AddTimedBuff(targetBuff, EffectHooks.BuffCycleDuration);
				}
			}
		}
	}



	internal static class EffectHooks
	{
		public static GameObject lightningStake = Resources.Load<GameObject>("Prefabs/Projectiles/LightningStake");

		public static List<string> dronesList = new List<string> { "Drone", "Droid", "Robo", "Turret", "Missile", "Laser", "Beam" };

		internal static Dictionary<NetworkInstanceId, float> DestroyedBodies = new Dictionary<NetworkInstanceId, float>();

		private static FieldInfo DamageInfoRejectedField;

		internal const float BuffCycleDuration = 5f;
		private static float FixedUpdateStopwatch = 0f;



		internal static void OnFixedUpdate()
		{
			if (!NetworkServer.active) return;

			FixedUpdateStopwatch += Time.fixedDeltaTime;

			if (FixedUpdateStopwatch >= 0.333f)
			{
				List<NetworkInstanceId> destroyedBodiesKeys = new List<NetworkInstanceId>(DestroyedBodies.Keys);

				foreach (var netId in destroyedBodiesKeys)
				{
					DestroyedBodies[netId] -= FixedUpdateStopwatch;
					if (DestroyedBodies[netId] <= 0f) DestroyedBodies.Remove(netId);
				}

				FixedUpdateStopwatch = 0f;
			}
		}



		internal static void Init()
		{
			OnDestroyHook();

			DamageInfoRejectedField = typeof(DamageInfo).GetField("rejected");

			FreezeHook();
			PreventAspectChillHook();
			OverloadingBombHook();
			FireTrailHook();
			BurnApplicationHook();
			PoisonBallHook();
			NullDurationHook();
			LunarProjectileHook();
			PreventAspectCrippleHook();
			DamageTakenHook();
			LifeGainOnHitHook();
			HeadHunterBuffHook();
			DotAmpHook();
			OnHitEnemyHook();
			FixTimedChillApplication();
			ShieldRegenHook();

			EquipmentLostBuffHook();
			EquipmentGainedBuffHook();
			ApplyAspectBuffOnInventoryChangedHook();
			UpdateOnBuffLostHook();
			RefreshAspectBuffsHook();
		}

		internal static void LateSetup(){
			if (Catalog.EliteVariety.Enabled)
			{
				EffectManagerHook();
				DodgeHook();

				ModifyDot();
				ModifyDeploySlot();
			}
		}



		private static void OnDestroyHook()
		{
			On.RoR2.CharacterBody.OnDestroy += (orig, self) =>
			{
				if (NetworkServer.active) DestroyedBodies.Add(self.netId, 3f);

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
						if (victim && victim.bodyIndex == Catalog.RiskOfRain.mithrixBodyIndex) return;
						if (!attacker.HasBuff(RoR2Content.Buffs.AffixWhite) || Configuration.AspectWhiteBaseFreezeChance.Value <= 0f) return;
						if (!state.canBeFrozen || attacker.teamComponent.teamIndex != TeamIndex.Player || damageReport.damageInfo.procCoefficient < 0.105f) return;

						float count = ZetAspectsPlugin.GetStackMagnitude(attacker, RoR2Content.Buffs.AffixWhite);
						float chance = Configuration.AspectWhiteBaseFreezeChance.Value + Configuration.AspectWhiteStackFreezeChance.Value * (count - 1f);
						chance = Util.ConvertAmplificationPercentageIntoReductionPercentage(chance * damageReport.damageInfo.procCoefficient);

						if (Util.CheckRoll(chance)) state.SetFrozen(Configuration.AspectWhiteFreezeDuration.Value * damageReport.damageInfo.procCoefficient);
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects : Freeze Hook Failed");
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
					x => x.MatchStloc(8),
					x => x.MatchLdloc(8)
				);

				if (found)
				{
					c.Index += 4;

					c.Emit(OpCodes.Ldc_I4, 0);
					c.Emit(OpCodes.Stloc, 8);
				}
				else
				{
					Debug.LogWarning("ZetAspects : Chill Hook Failed");
				}
			};
		}

		private static void OverloadingBombHook()
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

					// Handle lightning bomb creation
					c.Emit(OpCodes.Ldloc, 0);
					c.Emit(OpCodes.Ldarg, 1);
					c.EmitDelegate<Action<CharacterBody, DamageInfo>>((self, damageInfo) =>
					{
						if (!self.HasBuff(RoR2Content.Buffs.AffixBlue)) return;
						if (Configuration.AspectBlueBaseDamage.Value <= 0f) return;

						float damage = Util.OnHitProcDamage(damageInfo.damage, self.damage, 1f);
						float count = ZetAspectsPlugin.GetStackMagnitude(self, RoR2Content.Buffs.AffixBlue);

						damage *= Configuration.AspectBlueBaseDamage.Value + Configuration.AspectBlueStackDamage.Value * (count - 1f);
						if (self.teamComponent.teamIndex != TeamIndex.Player) damage *= Configuration.AspectEffectMonsterDamageMult.Value;

						FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
						{
							projectilePrefab = lightningStake,
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
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects : Overloading Bomb Hook Failed");
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
					Debug.LogWarning("ZetAspects : Firetrail Hook Failed");
				}
			};
		}

		private static void BurnApplicationHook()
		{
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(7),
					x => x.MatchLdloc(7),
					x => x.MatchOr()
				);

				if (found)
				{
					c.Index += 3;

					// Prevent default
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 0);

					// Handle DOT application
					c.Emit(OpCodes.Ldloc, 0);
					c.Emit(OpCodes.Ldarg, 1);
					c.Emit(OpCodes.Ldarg, 2);
					c.EmitDelegate<Action<CharacterBody, DamageInfo, GameObject>>((self, damageInfo, victim) =>
					{
						// dots tick at the start of duration
						// 4s is 8 ticks - because the 1st tick is at 0s dot only lasts 3.5s
						float ticks, rTicks, damageMult;

						// Burn
						if ((damageInfo.damageType & DamageType.IgniteOnHit) > DamageType.Generic)
						{
							ticks = 2f * 4f * damageInfo.procCoefficient;
							rTicks = Mathf.Ceil(ticks);

							if (rTicks > 0f)
							{
								damageMult = ticks / rTicks;
								damageMult *= rTicks / (rTicks + 1f);
								DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.Burn, (rTicks * 0.5f) + 0.49f, damageMult);
							}
						}

						// Percent Burn
						// 5 ticks per second
						if ((damageInfo.damageType & DamageType.PercentIgniteOnHit) != DamageType.Generic)
						{
							ticks = 5f * 4f * damageInfo.procCoefficient;
							rTicks = Mathf.Ceil(ticks);

							if (rTicks > 0f)
							{
								damageMult = ticks / rTicks;
								damageMult *= rTicks / (rTicks + 1f);
								DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.PercentBurn, (rTicks * 0.2f) + 0.19f, damageMult);
							}
						}

						// Fire Aspect Burn
						if (self.HasBuff(RoR2Content.Buffs.AffixRed) && Configuration.AspectRedBaseDamage.Value > 0f)
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, RoR2Content.Buffs.AffixRed);
							float aspectDamageMult = Configuration.AspectRedBaseDamage.Value + Configuration.AspectRedStackDamage.Value * (count - 1f);
							float aspectDuration = Configuration.AspectRedBurnDuration.Value;

							float ignitePercentBaseDamage = 0f;
							float aspectPercentBaseDamage = 0f;

							if ((damageInfo.damageType & DamageType.PercentIgniteOnHit) != DamageType.Generic)
							{
								ignitePercentBaseDamage = 4f * damageInfo.procCoefficient * 0.5f;
							}

							if (Configuration.AspectRedUseBase.Value)
							{
								aspectPercentBaseDamage = aspectDamageMult;
							}
							else
							{
								aspectPercentBaseDamage = damageInfo.damage / self.damage;
								aspectPercentBaseDamage *= aspectDamageMult;
								if (damageInfo.crit)
								{
									aspectPercentBaseDamage *= 2f;
								}
							}

							if (self.teamComponent.teamIndex != TeamIndex.Player)
							{
								float monsterMult = Configuration.AspectEffectMonsterDamageMult.Value;
								aspectPercentBaseDamage *= monsterMult;
								aspectDuration *= monsterMult;
							}

							// Choose highest damage output
							if (ignitePercentBaseDamage > aspectPercentBaseDamage)
							{
								ticks = 2f * 4f * damageInfo.procCoefficient;
								rTicks = Mathf.Ceil(ticks);

								if (rTicks > 0f)
								{
									damageMult = ticks / rTicks;
									damageMult *= rTicks / (rTicks + 1f);
									DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.Burn, (rTicks * 0.5f) + 0.49f, damageMult);
								}
							}
							else
							{
								ticks = 2f * aspectDuration;
								rTicks = Mathf.Ceil(ticks);

								if (rTicks > 0f)
								{
									damageMult = aspectPercentBaseDamage / aspectDuration / 0.5f;
									damageMult *= ticks / rTicks;
									damageMult *= rTicks / (rTicks + 1f);
									DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.Burn, (rTicks * 0.5f) + 0.49f, damageMult);
									//Debug.LogWarning("BaseDamage : " + self.damage);
									//Debug.LogWarning("TotalDamage : " + damageInfo.damage);
									//Debug.LogWarning("ProcCoeff : " + damageInfo.procCoefficient);
									//Debug.LogWarning("Ticks : " + rTicks + "(+1) [" + (self.damage * 0.25f * damageMult) + "]");
								}
							}

							return;
						}
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects : Burn Application Hook Failed");
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
					Debug.LogWarning("ZetAspects : Poison Ball Hook Failed");
				}
			};
		}

		private static void NullDurationHook()
		{
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(1),
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
					c.Emit(OpCodes.Ldloc, 0);
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
					Debug.LogWarning("ZetAspects : Nullified Duration Hook Failed");
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
					Debug.LogWarning("ZetAspects : Lunar Projectile Hook Failed");
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

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 0);
				}
				else
				{
					Debug.LogWarning("ZetAspects : Prevent Cripple Apply Hook Failed");
				}
			};
		}



		private static void DamageTakenHook()
		{
			IL.RoR2.HealthComponent.TakeDamage += (il) =>
			{
				ILCursor c = new ILCursor(il);

				// find : store damageinfo.damge into variable
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
							if (self.teamComponent.teamIndex == TeamIndex.Player) extraDamage *= Configuration.AspectEffectPlayerDebuffMult.Value;
							damage *= 1f + extraDamage;
						}

						if (Catalog.EliteVariety.Enabled && Configuration.AspectTinkerBaseDamageResistGain.Value > 0f)
						{
							CharacterMaster master = self.master;
							if (master && IsValidDrone(master))
							{
								CharacterBody ownerBody = GetMinionOwnerBody(master);
								if (ownerBody && ownerBody.HasBuff(Catalog.EliteVariety.Buffs.AffixTinkerer))
								{
									float count = ZetAspectsPlugin.GetStackMagnitude(ownerBody, Catalog.EliteVariety.Buffs.AffixTinkerer);
									float damageReduction = Configuration.AspectTinkerBaseDamageResistGain.Value + Configuration.AspectTinkerStackDamageResistGain.Value * (count - 1f);
									damageReduction = Util.ConvertAmplificationPercentageIntoReductionPercentage(damageReduction * 100f) * 0.01f;
									damage *= 1f - damageReduction;
								}
							}
						}

						return damage;
					});
					c.Emit(OpCodes.Stloc, 6);
				}
				else
				{
					Debug.LogWarning("ZetAspects - Damage Taken Hook Failed");
				}
			};
		}

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

		private static void LifeGainOnHitHook()
		{
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(3),
					x => x.MatchLdsfld(typeof(RoR2Content.Items).GetField("Seed")),
					x => x.MatchCallvirt<Inventory>("GetItemCount"),
					x => x.MatchStloc(19)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Ldloc, 0);
					c.EmitDelegate<Func<int, CharacterBody, int>>((amount, self) =>
					{
						if (Configuration.LeechSeedHeal.Value > 1)
						{
							int seed = self.inventory.GetItemCount(RoR2Content.Items.Seed);
							amount += seed * (Configuration.LeechSeedHeal.Value - 1);
						}

						if (self.HasBuff(RoR2Content.Buffs.AffixPoison) && Configuration.AspectPoisonBaseHeal.Value > 0)
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, RoR2Content.Buffs.AffixPoison);
							amount += Configuration.AspectPoisonBaseHeal.Value + (int)(Configuration.AspectPoisonStackHeal.Value * (count - 1f) + 0.55f);
						}

						return amount;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects - LGOH Hook Failed");
				}
			};
		}

		private static void HeadHunterBuffHook()
		{
			IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcR4(3f),
					x => x.MatchLdcR4(5f),
					x => x.MatchLdloc(54),
					x => x.MatchConvR4(),
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchStloc(56)
				);

				if (found)
				{
					c.Index += 6;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc, 54);
					c.EmitDelegate<Func<int, float>>((count) =>
					{
						return Configuration.HeadHunterBaseDuration.Value + Configuration.HeadHunterStackDuration.Value * (count - 1);
					});
					c.Emit(OpCodes.Dup);
					c.Emit(OpCodes.Ldloc, 13);
					c.EmitDelegate<Action<float, CharacterBody>>((duration, attacker) =>
					{
						if (Configuration.HeadHunterBuffEnable.Value) attacker.AddTimedBuff(ZetAspectsContent.Buffs.ZetHeadHunter, duration);
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects - HH Duration And Buff Hook Failed");
				}
			};
		}

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

					if (atkBody.HasBuff(Catalog.EliteVariety.Buffs.AffixImpPlane) && Configuration.AspectImpaleBaseDotAmp.Value > 0f)
					{
						float count = ZetAspectsPlugin.GetStackMagnitude(atkBody, Catalog.EliteVariety.Buffs.AffixImpPlane);
						mult += Configuration.AspectImpaleBaseDotAmp.Value + Configuration.AspectImpaleStackDotAmp.Value * (count - 1f);
					}
					if (atkBody.HasBuff(Catalog.Aetherium.Buffs.AffixSanguine) && Configuration.AspectSanguineBaseDotAmp.Value > 0f)
					{
						float count = ZetAspectsPlugin.GetStackMagnitude(atkBody, Catalog.Aetherium.Buffs.AffixSanguine);
						mult += Configuration.AspectSanguineBaseDotAmp.Value + Configuration.AspectSanguineStackDotAmp.Value * (count - 1f);
					}

					inflictDotInfo.damageMultiplier *= mult;
				}
			}

			orig(ref inflictDotInfo);
		}

		private static void OnHitEnemyHook()
		{
			On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damageInfo, victimObject) =>
			{
				ApplyAspectOnHitEffects(damageInfo, victimObject);

				orig(self, damageInfo, victimObject);
			};
		}

		private static void ApplyAspectOnHitEffects(DamageInfo damageInfo, GameObject victimObject)
		{
			if (!NetworkServer.active) return;
			if (damageInfo.procCoefficient <= 0f || damageInfo.rejected) return;

			if (damageInfo.attacker)
			{
				CharacterBody attacker = damageInfo.attacker.GetComponent<CharacterBody>();
				CharacterBody victim = victimObject ? victimObject.GetComponent<CharacterBody>() : null;

				if (attacker && victim)
				{
					FireFrostBlade(attacker, victim, damageInfo);
					ApplyChill(attacker, victim, damageInfo);
					ApplySapped(attacker, victim, damageInfo);
					ApplyShredded(attacker, victim, damageInfo);
					ApplyCripple(attacker, victim, damageInfo);
					ApplyBleed(attacker, victim, damageInfo);
				}
			}
		}

		private static void FireFrostBlade(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!self.HasBuff(RoR2Content.Buffs.AffixWhite)) return;
			if (Configuration.AspectWhiteBaseDamage.Value <= 0f) return;

			if (damageInfo.procCoefficient < 0.105f) return;
			if (damageInfo.procChainMask.HasProc(ProcType.Thorns)) return;

			GameObject gameObject = self.gameObject;
			TeamIndex teamIndex = self.teamComponent.teamIndex;

			float damage = Util.OnHitProcDamage(damageInfo.damage, self.damage, 1f);
			float count = ZetAspectsPlugin.GetStackMagnitude(self, RoR2Content.Buffs.AffixWhite);

			damage *= Configuration.AspectWhiteBaseDamage.Value + Configuration.AspectWhiteStackDamage.Value * (count - 1f);
			if (self.teamComponent.teamIndex != TeamIndex.Player) damage *= Configuration.AspectEffectMonsterDamageMult.Value;

			var procMask = default(ProcChainMask);
			if (Catalog.borboFrostBlade) procMask.AddProc(ProcType.Thorns);

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
				if (buffDef == RoR2Content.Buffs.Slow80)
				{
					if (duration < 0.1f) return;
					if (Catalog.limitChillStacks && self.GetBuffCount(RoR2Content.Buffs.Slow80) >= 10) return;
				}

				orig(self, buffDef, duration);
			};
		}

		private static void ApplyChill(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!CanApplyAspectChill(self)) return;

			float duration = Configuration.AspectWhiteSlowDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
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
			if (Configuration.AspectGhostSlowEffect.Value && self.HasBuff(RoR2Content.Buffs.AffixHaunted)) return true;
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
			if (ZetAspectsContent.Buffs.ZetSapped.buffIndex == BuffIndex.None) return;

			float duration = Configuration.AspectBlueSappedDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
			if (duration > 0.1f) victim.AddTimedBuff(ZetAspectsContent.Buffs.ZetSapped, duration);
		}

		private static void ApplyShredded(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!self.HasBuff(RoR2Content.Buffs.AffixHaunted)) return;
			if (ZetAspectsContent.Buffs.ZetShredded.buffIndex == BuffIndex.None) return;

			float duration = Configuration.AspectGhostShredDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
			if (duration > 0.1f) victim.AddTimedBuff(ZetAspectsContent.Buffs.ZetShredded, duration);
		}

		private static void ApplyCripple(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!self.HasBuff(RoR2Content.Buffs.AffixLunar)) return;

			float duration = Configuration.AspectLunarCrippleDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
			if (duration > 0.1f) victim.AddTimedBuff(RoR2Content.Buffs.Cripple, duration);
		}

		private static void ApplyBleed(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!AetheriumHooks.bleedHook) return;

			BuffDef buffDef = Catalog.Aetherium.Buffs.AffixSanguine;

			if (!buffDef || !self.HasBuff(buffDef)) return;

			if (Configuration.AspectSanguineBaseDamage.Value > 0f)
			{
				float count = ZetAspectsPlugin.GetStackMagnitude(self, buffDef);
				float damage = Configuration.AspectSanguineBaseDamage.Value + Configuration.AspectSanguineStackDamage.Value * (count - 1f);

				if (self.teamComponent.teamIndex != TeamIndex.Player) damage *= Configuration.AspectEffectMonsterDamageMult.Value;

				InflictDotPrecise(victim.gameObject, damageInfo.attacker, DotController.DotIndex.Bleed, Configuration.AspectSanguineBleedDuration.Value, self.damage * damage);
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

				float tickedDuration = rTicks * dotDef.interval + (dotDef.interval - 0.01f);
				DotController.InflictDot(victim, attacker, dotIndex, tickedDuration, damageMult);
			}
		}



		private static void ShieldRegenHook()
		{
			IL.RoR2.HealthComponent.ServerFixedUpdate += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchStfld<HealthComponent>("regenAccumulator")
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<float, HealthComponent, float>>((regenAccumulator, healthComponent) =>
					{
						float toShield = 0f;
						Inventory inventory = healthComponent.body.inventory;

						if (healthComponent.body.HasBuff(RoR2Content.Buffs.AffixLunar)) toShield = Mathf.Max(toShield, Configuration.AspectLunarRegen.Value);
						if (inventory && inventory.GetItemCount(RoR2Content.Items.ShieldOnly) > 0) toShield = Mathf.Max(toShield, Configuration.TranscendenceRegen.Value);

						if (toShield <= 0f) return regenAccumulator;
						if (healthComponent.shield >= healthComponent.fullShield) return regenAccumulator;

						if (regenAccumulator > 1f)
						{
							float num = Mathf.Floor(regenAccumulator);
							regenAccumulator -= num;
							num *= toShield;
							AddShieldToHealthComponent(healthComponent, num);
						}

						return regenAccumulator;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects - Shield Regen Hook Failed");
				}
			};
		}

		private static void AddShieldToHealthComponent(HealthComponent healthComponent, float value)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void ZetAspects::AddShieldToHealthComponent(ROR2.HealthComponent, System.Single)' called on client");
				return;
			}
			if (!healthComponent.alive)
			{
				return;
			}
			if (healthComponent.shield < healthComponent.fullShield)
			{
				healthComponent.Networkshield = Mathf.Min(healthComponent.shield + value, healthComponent.fullShield);
			}
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

						if (ZetAspectsPlugin.GetAspectEquipmentDef(buffDef)) return null;

						return buffDef;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects - Equipment Lost Buff Hook Failed");
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

						if (ZetAspectsPlugin.GetAspectEquipmentDef(buffDef))
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
					Debug.LogWarning("ZetAspects - Equipment Gained Buff Hook Failed");
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
						BlightedStateManager.ApplyBlightedAspectBuffs(self);
					}
				}
			};
		}

		private static void UpdateOnBuffLostHook()
		{
			On.RoR2.CharacterBody.OnBuffFinalStackLost += (orig, self, buffDef) =>
			{
				orig(self, buffDef);

				if (NetworkServer.active && self)
				{
					if (!DestroyedBodies.ContainsKey(self.netId))
					{
						if (BlightedStateManager.HasAspectFromBlighted(self, buffDef))
						{
							self.AddTimedBuff(buffDef, BuffCycleDuration);
						}
						else
						{
							Inventory inventory = self.inventory;

							if (inventory)
							{
								if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, buffDef))
								{
									self.AddTimedBuff(buffDef, BuffCycleDuration);
								}
								else
								{
									// update item behaviors
									//self.OnInventoryChanged();
									inventory.GiveItem(ZetAspectsContent.Items.ZetAspectsUpdateInventory);
								}
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

			BuffDef targetBuff;

			targetBuff = RoR2Content.Buffs.AffixWhite;
			if (!self.HasBuff(targetBuff))
			{
				if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectIce, RoR2Content.Equipment.AffixWhite)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
			}
			targetBuff = RoR2Content.Buffs.AffixBlue;
			if (!self.HasBuff(targetBuff))
			{
				if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectLightning, RoR2Content.Equipment.AffixBlue)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
			}
			targetBuff = RoR2Content.Buffs.AffixRed;
			if (!self.HasBuff(targetBuff))
			{
				if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectFire, RoR2Content.Equipment.AffixRed)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
			}
			targetBuff = RoR2Content.Buffs.AffixHaunted;
			if (!self.HasBuff(targetBuff))
			{
				if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectCelestial, RoR2Content.Equipment.AffixHaunted)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
			}
			targetBuff = RoR2Content.Buffs.AffixPoison;
			if (!self.HasBuff(targetBuff))
			{
				if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectMalachite, RoR2Content.Equipment.AffixPoison)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
			}
			targetBuff = RoR2Content.Buffs.AffixLunar;
			if (!self.HasBuff(targetBuff))
			{
				if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectPerfect, RoR2Content.Equipment.AffixLunar)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
			}

			if (Catalog.EliteVariety.populated)
			{
				targetBuff = Catalog.EliteVariety.Buffs.AffixArmored;
				if (targetBuff && !self.HasBuff(targetBuff))
				{
					if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectArmor, Catalog.EliteVariety.Equipment.AffixArmored)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
				}
				targetBuff = Catalog.EliteVariety.Buffs.AffixBuffing;
				if (targetBuff && !self.HasBuff(targetBuff))
				{
					if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectBanner, Catalog.EliteVariety.Equipment.AffixBuffing)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
				}
				targetBuff = Catalog.EliteVariety.Buffs.AffixImpPlane;
				if (targetBuff && !self.HasBuff(targetBuff))
				{
					if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectImpale, Catalog.EliteVariety.Equipment.AffixImpPlane)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
				}
				targetBuff = Catalog.EliteVariety.Buffs.AffixPillaging;
				if (targetBuff && !self.HasBuff(targetBuff))
				{
					if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectGolden, Catalog.EliteVariety.Equipment.AffixPillaging)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
				}
				targetBuff = Catalog.EliteVariety.Buffs.AffixSandstorm;
				if (targetBuff && !self.HasBuff(targetBuff))
				{
					if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectCyclone, Catalog.EliteVariety.Equipment.AffixSandstorm)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
				}
				targetBuff = Catalog.EliteVariety.Buffs.AffixTinkerer;
				if (targetBuff && self.bodyIndex != Catalog.EliteVariety.tinkerDroneBodyIndex && !self.HasBuff(targetBuff))
				{
					if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectTinker, Catalog.EliteVariety.Equipment.AffixTinkerer)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
				}
			}

			if (Catalog.LostInTransit.populated)
			{
				targetBuff = Catalog.LostInTransit.Buffs.AffixLeeching;
				if (targetBuff && !self.HasBuff(targetBuff))
				{
					if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectLeeching, Catalog.LostInTransit.Equipment.AffixLeeching)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
				}
				targetBuff = Catalog.LostInTransit.Buffs.AffixFrenzied;
				if (targetBuff && !self.HasBuff(targetBuff))
				{
					if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectFrenzied, Catalog.LostInTransit.Equipment.AffixFrenzied)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
				}
				targetBuff = Catalog.LostInTransit.Buffs.AffixVolatile;
				if (targetBuff && !self.HasBuff(targetBuff))
				{
					if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectVolatile, Catalog.LostInTransit.Equipment.AffixVolatile)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
				}
				targetBuff = Catalog.LostInTransit.Buffs.AffixBlighted;
				if (targetBuff && !self.HasBuff(targetBuff))
				{
					if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectBlighted, Catalog.LostInTransit.Equipment.AffixBlighted)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
				}
			}

			if (Catalog.Aetherium.populated)
			{
				targetBuff = Catalog.Aetherium.Buffs.AffixSanguine;
				if (targetBuff && !self.HasBuff(targetBuff))
				{
					if (ZetAspectsPlugin.HasAspectItemOrEquipment(inventory, ZetAspectsContent.Items.ZetAspectSanguine, Catalog.Aetherium.Equipment.AffixSanguine)) self.AddTimedBuff(targetBuff, BuffCycleDuration);
				}
			}
		}
	}
}
