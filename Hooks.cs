using System;
using System.Security;
using System.Security.Permissions;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace TPDespair.ZetAspects
{
	internal static class Hooks
	{
		internal static void Init()
		{
			StatHooks();
			EffectHooks();
			DropHooks();
			DisplayHooks();
		}

		private static void StatHooks()
		{
			MovementSpeedHook();
			JumpCountHook();
			DamageHook();
			ShieldHook();
			HealthHook();
			ArmorHook();
			AttackSpeedHook();
			CritChanceHook();
			ShieldConversionHook();
			ShieldRegenHook();
		}

		private static void EffectHooks()
		{
			ZetAspectIce.Hooks();
			ZetAspectLightning.Hooks();
			ZetAspectFire.Hooks();
			ZetAspectCelestial.Hooks();
			ZetAspectMalachite.Hooks();
			ZetAspectPerfect.Hooks();
			DamageTakenHook();
			LifeGainOnHitHook();
			HeadHunterBuffHook();
			OnHitEnemyHook();
			RefreshAspectBuffsHook();
		}

		private static void DropHooks()
		{
			DropChanceHook();
			InterceptAspectDropHook();
			EquipmentConversionHook();
		}

		private static void DisplayHooks()
		{
			CharacterOverlayHook();
			EquipmentDisplayHook();
			UpdateItemDisplayHook();
			EnableItemDisplayHook();
			DisableItemDisplayHook();
		}



		private static void MovementSpeedHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(62),
					x => x.MatchLdloc(63),
					x => x.MatchLdloc(64),
					x => x.MatchDiv(),
					x => x.MatchMul(),
					x => x.MatchStloc(62)
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Pop);

					// increase
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 63);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(RoR2Content.Buffs.AffixRed) && Configuration.AspectRedBaseMovementGain.Value > 0f)
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, RoR2Content.Buffs.AffixRed);
							float addedSpeed = Configuration.AspectRedBaseMovementGain.Value + Configuration.AspectRedStackMovementGain.Value * (count - 1f);
							value += addedSpeed;
						}

						if (self.HasBuff(RoR2Content.Buffs.AffixLunar))
						{
							value -= 0.3f;

							if (Configuration.AspectLunarBaseMovementGain.Value > 0f)
							{
								float count = ZetAspectsPlugin.GetStackMagnitude(self, RoR2Content.Buffs.AffixLunar);
								float addedSpeed = Configuration.AspectLunarBaseMovementGain.Value + Configuration.AspectLunarStackMovementGain.Value * (count - 1f);
								value += addedSpeed;
							}
						}

						if (self.HasBuff(ZetAspectsContent.Buffs.ZetHeadHunter))
						{
							value += Configuration.HeadHunterBuffMovementSpeed.Value * self.GetBuffCount(ZetAspectsContent.Buffs.ZetHeadHunter);
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, 63);

					c.Emit(OpCodes.Ldloc, 62);
				}
				else
				{
					Debug.LogWarning("ZetAspects - Movement Speed Hook Failed");
				}
			};
		}

		private static void JumpCountHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("baseJumpCount"),
					x => x.MatchLdloc(8)
				);

				if (found)
				{
					// add
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 8);
					c.EmitDelegate<Func<CharacterBody, int, int>>((self, value) =>
					{
						if (self.HasBuff(RoR2Content.Buffs.AffixRed) && Configuration.AspectRedExtraJump.Value)
						{
							if (self.teamComponent.teamIndex == TeamIndex.Player) value++;
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, 8);
				}
				else
				{
					Debug.LogWarning("ZetAspects - Jump Count Hook Failed");
				}
			};
		}

		private static void DamageHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(66),
					x => x.MatchLdloc(67),
					x => x.MatchMul(),
					x => x.MatchStloc(66)
				);

				if (found)
				{
					// increase
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 67);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(ZetAspectsContent.Buffs.ZetHeadHunter))
						{
							value += Configuration.HeadHunterBuffDamage.Value * self.GetBuffCount(ZetAspectsContent.Buffs.ZetHeadHunter);
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, 67);

					c.Index += 4;

					// multiplier
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 66);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(ZetAspectsContent.Buffs.ZetSapped))
						{
							float delta = Mathf.Abs(Configuration.AspectBlueSappedDamage.Value);
							value *= 1f - Mathf.Min(0.9f, delta);
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, 66);
				}
				else
				{
					Debug.LogWarning("ZetAspects - Damage Hook Failed");
				}
			};
		}

		private static void ShieldHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchStloc(52)
				);

				if (found)
				{
					c.Index += 3;

					// add
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 52);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_maxHealth"));
					c.EmitDelegate<Func<CharacterBody, float, float, float>>((self, shield, health) =>
					{
						if (self.HasBuff(RoR2Content.Buffs.AffixBlue) && Configuration.AspectBlueBaseShieldGain.Value > 0f)
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, RoR2Content.Buffs.AffixBlue);
							float addedShield = health * (Configuration.AspectBlueBaseShieldGain.Value + Configuration.AspectBlueStackShieldGain.Value * (count - 1f));
							shield += addedShield;
						}

						return shield;
					});
					c.Emit(OpCodes.Stloc, 52);
				}
				else
				{
					Debug.LogWarning("ZetAspects - Shield Gain Hook Failed");
				}
			};
		}

		private static void HealthHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(50),
					x => x.MatchLdloc(51),
					x => x.MatchMul(),
					x => x.MatchStloc(50)
				);

				if (found)
				{
					// add
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 50);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(RoR2Content.Buffs.AffixPoison) && Configuration.AspectPoisonBaseHealthGain.Value > 0f)
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, RoR2Content.Buffs.AffixPoison);
							float addedHealth = Configuration.AspectPoisonBaseHealthGain.Value + Configuration.AspectPoisonStackHealthGain.Value * (count - 1f);
							value += addedHealth;
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, 50);

					// increase
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 51);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(ZetAspectsContent.Buffs.ZetHeadHunter))
						{
							value += Configuration.HeadHunterBuffHealth.Value * self.GetBuffCount(ZetAspectsContent.Buffs.ZetHeadHunter);
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, 51);
				}
				else
				{
					Debug.LogWarning("ZetAspects - Health Hook Failed");
				}
			};
		}

		private static void ArmorHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(24),
					x => x.MatchConvR4(),
					x => x.MatchLdcR4(70f),
					x => x.MatchMul(),
					x => x.MatchAdd()
				);

				if (found)
				{
					c.Index += 5;

					// add
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<float, CharacterBody, float>>((value, self) =>
					{
						float addedArmor = 0f;
						if (self.HasBuff(RoR2Content.Buffs.AffixHaunted) && Configuration.AspectGhostBaseArmorGain.Value > 0f)
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, RoR2Content.Buffs.AffixHaunted);
							addedArmor += Configuration.AspectGhostBaseArmorGain.Value + Configuration.AspectGhostStackArmorGain.Value * (count - 1f);
						}
						else if (self.HasBuff(RoR2Content.Buffs.AffixHauntedRecipient) && Configuration.AspectGhostAllyArmorGain.Value > 0f)
						{
							addedArmor += Configuration.AspectGhostAllyArmorGain.Value;
						}
						value += addedArmor;

						float lostArmor = 0f;
						if (self.HasBuff(ZetAspectsContent.Buffs.ZetShredded))
						{
							lostArmor += Mathf.Abs(Configuration.AspectGhostShredArmor.Value);
						}
						if (self.teamComponent.teamIndex == TeamIndex.Player) lostArmor *= Configuration.AspectEffectPlayerDebuffMult.Value;
						value -= lostArmor;

						return value;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects - Armor Hook Failed");
				}
			};
		}

		private static void AttackSpeedHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(70),
					x => x.MatchLdloc(71),
					x => x.MatchMul(),
					x => x.MatchStloc(70)
				);

				if (found)
				{
					// increase
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 71);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(ZetAspectsContent.Buffs.ZetHeadHunter))
						{
							value += Configuration.HeadHunterBuffAttackSpeed.Value * self.GetBuffCount(ZetAspectsContent.Buffs.ZetHeadHunter);
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, 71);
				}
				else
				{
					Debug.LogWarning("ZetAspects - Attack Speed Hook Failed");
				}
			};
		}

		private static void CritChanceHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdloc(72),
					x => x.MatchCall<CharacterBody>("set_crit")
				);

				if (found)
				{
					// add
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 72);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(ZetAspectsContent.Buffs.ZetHeadHunter))
						{
							value += Configuration.HeadHunterBuffCritChance.Value * self.GetBuffCount(ZetAspectsContent.Buffs.ZetHeadHunter);
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, 72);
				}
				else
				{
					Debug.LogWarning("ZetAspects - Crit Chance Hook Failed");
				}
			};
		}

		private static void ShieldConversionHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcR4(0.25f),
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchStloc(52)
				);

				if (found)
				{
					c.Index += 5;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 52);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_maxHealth"));
					c.Emit(OpCodes.Ldloc, 14);
					c.EmitDelegate<Func<CharacterBody, float, float, int, float>>((self, shield, health, so) =>
					{
						float mult = 1f;
						if (so > 0) mult += 0.25f + (0.25f * so);
						if (self.HasBuff(RoR2Content.Buffs.AffixLunar) && Configuration.AspectLunarBaseShieldGain.Value > 0f)
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, RoR2Content.Buffs.AffixLunar);
							mult += Configuration.AspectLunarBaseShieldGain.Value + Configuration.AspectLunarStackShieldGain.Value * (count - 1f);
						}
						shield += health * mult;

						return shield;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects - Shield Conversion Hook Failed");
				}
			};
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
						if (healthComponent.body.HasBuff(RoR2Content.Buffs.HealingDisabled) && Configuration.AspectPoisonNullDamageTaken.Value != 0f)
						{
							float extraDamage = Mathf.Abs(Configuration.AspectPoisonNullDamageTaken.Value);
							if (healthComponent.body.teamComponent.teamIndex == TeamIndex.Player) extraDamage *= Configuration.AspectEffectPlayerDebuffMult.Value;
							damage *= 1f + extraDamage;
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
			if (damageInfo.procCoefficient == 0f || damageInfo.rejected) return;

			if (damageInfo.attacker)
			{
				CharacterBody attacker = damageInfo.attacker.GetComponent<CharacterBody>();
				CharacterBody victim = victimObject ? victimObject.GetComponent<CharacterBody>() : null;

				if (attacker && victim)
				{
					ZetAspectIce.FireFrostBlade(attacker, victim, damageInfo);
					ZetAspectLightning.ApplySapped(attacker, victim, damageInfo);
					ZetAspectCelestial.ApplyShredded(attacker, victim, damageInfo);
					ZetAspectPerfect.ApplyCripple(attacker, victim, damageInfo);
				}
			}
		}

		private static void RefreshAspectBuffsHook()
		{
			On.RoR2.CharacterBody.UpdateBuffs += (orig, self, deltaTime) =>
			{
				orig(self, deltaTime);

				if (self.inventory)
				{
					if (!self.HasBuff(RoR2Content.Buffs.AffixWhite))
					{
						if (self.inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectIce) > 0 || self.inventory.alternateEquipmentIndex == RoR2Content.Equipment.AffixWhite.equipmentIndex)
						{
							self.AddTimedBuff(RoR2Content.Buffs.AffixWhite, 5f);
						}
					}
					if (!self.HasBuff(RoR2Content.Buffs.AffixBlue))
					{
						if (self.inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectLightning) > 0 || self.inventory.alternateEquipmentIndex == RoR2Content.Equipment.AffixBlue.equipmentIndex)
						{
							self.AddTimedBuff(RoR2Content.Buffs.AffixBlue, 5f);
						}
					}
					if (!self.HasBuff(RoR2Content.Buffs.AffixRed))
					{
						if (self.inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectFire) > 0 || self.inventory.alternateEquipmentIndex == RoR2Content.Equipment.AffixRed.equipmentIndex)
						{
							self.AddTimedBuff(RoR2Content.Buffs.AffixRed, 5f);
						}
					}
					if (!self.HasBuff(RoR2Content.Buffs.AffixHaunted))
					{
						if (self.inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectCelestial) > 0 || self.inventory.alternateEquipmentIndex == RoR2Content.Equipment.AffixHaunted.equipmentIndex)
						{
							self.AddTimedBuff(RoR2Content.Buffs.AffixHaunted, 5f);
						}
					}
					if (!self.HasBuff(RoR2Content.Buffs.AffixPoison))
					{
						if (self.inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectMalachite) > 0 || self.inventory.alternateEquipmentIndex == RoR2Content.Equipment.AffixPoison.equipmentIndex)
						{
							self.AddTimedBuff(RoR2Content.Buffs.AffixPoison, 5f);
						}
					}
					if (!self.HasBuff(RoR2Content.Buffs.AffixLunar))
					{
						if (self.inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectPerfect) > 0 || self.inventory.alternateEquipmentIndex == RoR2Content.Equipment.AffixLunar.equipmentIndex)
						{
							self.AddTimedBuff(RoR2Content.Buffs.AffixLunar, 5f);
						}
					}
				}
			};
		}



		private static void DropChanceHook()
		{
			IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchPop(),
					x => x.MatchLdcR4(0.025f),
					x => x.MatchLdloc(14)
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Pop);
					c.EmitDelegate<Func<float>>(() =>
					{
						return Configuration.AspectDropChance.Value;
					});

					found = c.TryGotoNext(
						x => x.MatchLdloc(2),
						x => x.MatchCallOrCallvirt<CharacterBody>("get_isElite")
					);

					if (found)
					{
						c.Index += 2;

						c.Emit(OpCodes.Ldloc, 10);
						c.EmitDelegate<Func<bool, EquipmentIndex, bool>>((isElite, index) =>
						{
							if (isElite && index != EquipmentIndex.None)
							{
								if (Configuration.AspectShowDropText.Value && NetworkServer.active)
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

		private static void EquipmentConversionHook()
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
						if (!Configuration.AspectEquipmentConversion.Value) return pickupDef;

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
					Debug.LogWarning("ZetAspects - Equipment Conversion Hook Failed");
				}
			};
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
						if (GetEquipmentEliteDef(equipDef) == null) equipDef = null;

						if (equipDef != null && Configuration.AspectSkinEquipmentPriority.Value) return equipDef;
						if (!Configuration.AspectSkinApply.Value) return equipDef;

						CharacterBody body = model.body;

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

		private static EliteDef GetEquipmentEliteDef(EquipmentDef equipDef)
		{
			if (equipDef == null) return null;
			if (equipDef.passiveBuffDef == null) return null;
			return equipDef.passiveBuffDef.eliteDef;
		}

		private static EquipmentDef GetEquipmentDefToDisplay(Inventory inventory, EquipmentDef eliteEquipDef = null)
		{
			EquipmentDef targetEquipDef;
			bool eliteEquipDefNotNull = !(eliteEquipDef == null);

			targetEquipDef = RoR2Content.Equipment.AffixLunar;
			if (targetEquipDef != null)
			{
				if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectPerfect) > 0) return targetEquipDef;
				if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
			}
			targetEquipDef = RoR2Content.Equipment.AffixPoison;
			if (targetEquipDef != null)
			{
				if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectMalachite) > 0) return targetEquipDef;
				if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
			}
			targetEquipDef = RoR2Content.Equipment.AffixHaunted;
			if (targetEquipDef != null)
			{
				if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectCelestial) > 0) return targetEquipDef;
				if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
			}
			targetEquipDef = RoR2Content.Equipment.AffixRed;
			if (targetEquipDef != null)
			{
				if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectFire) > 0) return targetEquipDef;
				if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
			}
			targetEquipDef = RoR2Content.Equipment.AffixBlue;
			if (targetEquipDef != null)
			{
				if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectLightning) > 0) return targetEquipDef;
				if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
			}
			targetEquipDef = RoR2Content.Equipment.AffixWhite;
			if (targetEquipDef != null)
			{
				if (inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectIce) > 0) return targetEquipDef;
				if (eliteEquipDefNotNull && eliteEquipDef == targetEquipDef) return targetEquipDef;
			}

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

			if (equipDef == RoR2Content.Equipment.AffixLunar) return EquipmentIndex.None;
			if (equipDef == RoR2Content.Equipment.AffixPoison) return EquipmentIndex.None;
			if (equipDef == RoR2Content.Equipment.AffixHaunted) return EquipmentIndex.None;
			if (equipDef == RoR2Content.Equipment.AffixRed) return EquipmentIndex.None;
			if (equipDef == RoR2Content.Equipment.AffixBlue) return EquipmentIndex.None;
			if (equipDef == RoR2Content.Equipment.AffixWhite) return EquipmentIndex.None;

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
			if (GetEquipmentEliteDef(equipDef) == null) equipDef = null;

			if (equipDef != null && Configuration.AspectSkinEquipmentPriority.Value) displayDef = equipDef;
			else if (!Configuration.AspectSkinApply.Value) displayDef = equipDef;
			else displayDef = GetEquipmentDefToDisplay(inventory, equipDef);

			HandleAspectDisplay(model, displayDef, RoR2Content.Equipment.AffixLunar, ZetAspectsContent.Items.ZetAspectPerfect);
			HandleAspectDisplay(model, displayDef, RoR2Content.Equipment.AffixPoison, ZetAspectsContent.Items.ZetAspectMalachite);
			HandleAspectDisplay(model, displayDef, RoR2Content.Equipment.AffixHaunted, ZetAspectsContent.Items.ZetAspectCelestial);
			HandleAspectDisplay(model, displayDef, RoR2Content.Equipment.AffixRed, ZetAspectsContent.Items.ZetAspectFire);
			HandleAspectDisplay(model, displayDef, RoR2Content.Equipment.AffixBlue, ZetAspectsContent.Items.ZetAspectLightning);
			HandleAspectDisplay(model, displayDef, RoR2Content.Equipment.AffixWhite, ZetAspectsContent.Items.ZetAspectIce);
		}

		private static void HandleAspectDisplay(CharacterModel model, EquipmentDef display, EquipmentDef target, ItemDef item)
		{
			ItemMask list = model.enabledItemDisplays;
			ItemIndex index = item.itemIndex;
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

				orig(self, index);
			};
		}
	}
}
