using System;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace TPDespair.ZetAspects
{
	internal static class StatHooks
	{
		public static int BaseShieldLocIndex = -1;
		public static int BaseRegenLocIndex = -1;



		internal static void Init()
		{
			MovementSpeedHook();
			JumpCountHook();
			DamageHook();
			ShieldHook();
			HealthHook();
			AttackSpeedHook();
			RegenHook();

			DirectStatHook();

			DisableOverloadingShieldConversionHook();



			if (BaseRegenLocIndex != -1)
			{
				ShieldConversionHook();
			}
			else
			{
				Logger.Warn("BaseRegenLocIndex Not Found - ShieldConversionHook Aborted");
			}

			if (BaseShieldLocIndex != -1)
			{
				FullShieldConversionHook();
			}
			else
			{
				Logger.Warn("BaseShieldLocIndex Not Found - FullShieldConversionHook Aborted");
			}
		}



		private static void MovementSpeedHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				int BaseSpeedLocIndex = -1;

				if (c.TryGotoNext(
					x => x.MatchLdfld<CharacterBody>("baseMoveSpeed"),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("levelMoveSpeed")
				))
				{
					if (!c.TryGotoNext(x => x.MatchStloc(out BaseSpeedLocIndex)))
					{
						Logger.Warn("MovementSpeedHook Failed - Could not find BaseSpeedLocIndex");
						return;
					}
				}



				int MultSpeedLocIndex = -1;
				int DivSpeedLocIndex = -1;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(BaseSpeedLocIndex),
					x => x.MatchLdloc(out MultSpeedLocIndex),
					x => x.MatchLdloc(out DivSpeedLocIndex),
					x => x.MatchDiv(),
					x => x.MatchMul(),
					x => x.MatchStloc(BaseSpeedLocIndex)
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Pop);

					// increase
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, MultSpeedLocIndex);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						float count;

						if (self.HasBuff(BuffDefOf.AffixRed) && Configuration.AspectRedBaseMovementGain.Value > 0f)
						{
							count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixRed);
							value += Configuration.AspectRedBaseMovementGain.Value + Configuration.AspectRedStackMovementGain.Value * (count - 1f);
						}

						if (self.HasBuff(BuffDefOf.AffixLunar))
						{
							value -= 0.3f;

							if (Configuration.AspectLunarBaseMovementGain.Value > 0f)
							{
								count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixLunar);
								value += Configuration.AspectLunarBaseMovementGain.Value + Configuration.AspectLunarStackMovementGain.Value * (count - 1f);
							}
						}

						if (self.HasBuff(BuffDefOf.ZetHeadHunter))
						{
							value += Configuration.HeadHunterBuffMovementSpeed.Value * self.GetBuffCount(BuffDefOf.ZetHeadHunter);
						}

						if (self.HasBuff(BuffDefOf.AffixVeiled))
						{
							if (Configuration.AspectVeiledBaseMovementGain.Value > 0f)
							{
								count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixVeiled);
								value += Configuration.AspectVeiledBaseMovementGain.Value + Configuration.AspectVeiledStackMovementGain.Value * (count - 1f);
							}

							if (self.HasBuff(BuffDefOf.ZetElusive) && Configuration.AspectVeiledElusiveMovementGain.Value > 0f)
							{
								bool nemCloak = Compat.NemSpikeStrip.VeiledEnabled && Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.VeiledHitToShowField, true);

								count = Mathf.Max(5f, self.GetBuffCount(BuffDefOf.ZetElusive));
								value += Configuration.AspectVeiledElusiveMovementGain.Value * (count / (nemCloak ? 40f : 20f));
							}
						}

						if (Compat.PlasmaSpikeStrip.rageStatHook)
						{
							if (self.HasBuff(BuffDefOf.AffixAragonite) && Configuration.AspectAragoniteBaseMovementGain.Value > 0f)
							{
								count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixAragonite);
								float effectValue = Configuration.AspectAragoniteBaseMovementGain.Value + Configuration.AspectAragoniteStackMovementGain.Value * (count - 1f);
								if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectAragoniteMonsterMovementMult.Value;
								value += effectValue;
							}
							else if (Catalog.rageAura != BuffIndex.None && self.HasBuff(Catalog.rageAura) && Configuration.AspectAragoniteAllyMovementGain.Value > 0f)
							{
								float effectValue = Configuration.AspectAragoniteAllyMovementGain.Value;
								if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectAragoniteMonsterMovementMult.Value;
								value += effectValue;
							}
						}

						if (self.HasBuff(BuffDefOf.AffixNight) && Configuration.AspectNightBaseMovementGain.Value > 0f)
						{
							count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixNight);
							float effectValue = Configuration.AspectNightBaseMovementGain.Value + Configuration.AspectNightStackMovementGain.Value * (count - 1f);
							if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectNightMonsterMovementMult.Value;
							value += effectValue;
						}

						if (Compat.RisingTides.nightSpeedStatHook)
						{
							if (self.HasBuff(BuffDefOf.NightSpeed) && Configuration.AspectNightBaseSafeMovementGain.Value > 0f)
							{
								count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixNight);
								float effectValue = Configuration.AspectNightBaseSafeMovementGain.Value + Configuration.AspectNightStackSafeMovementGain.Value * (count - 1f);
								if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectNightMonsterSafeMovementMult.Value;
								value += effectValue;
							}
						}

						if (self.HasBuff(BuffDefOf.AffixOppressive) && Configuration.AspectOppressiveBaseMovementGain.Value > 0f)
						{
							count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixOppressive);
							value += Configuration.AspectOppressiveBaseMovementGain.Value + Configuration.AspectOppressiveStackMovementGain.Value * (count - 1f);
						}

						if (Compat.MoreElites.frenzyStatHook)
						{
							if (self.HasBuff(BuffDefOf.AffixFrenzied) && Configuration.AspectFrenziedBaseMovementGain.Value > 0f)
							{
								count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixFrenzied);
								float effectValue = Configuration.AspectFrenziedBaseMovementGain.Value + Configuration.AspectFrenziedStackMovementGain.Value * (count - 1f);
								if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectFrenziedMonsterMovementMult.Value;
								value += effectValue;
							}
						}

						if (self.HasBuff(BuffDefOf.AffixSandstorm) && Configuration.AspectCycloneBaseMovementGain.Value > 0f)
						{
							count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixSandstorm);
							value += Configuration.AspectCycloneBaseMovementGain.Value + Configuration.AspectCycloneStackMovementGain.Value * (count - 1f);
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, MultSpeedLocIndex);

					c.Emit(OpCodes.Ldloc, BaseSpeedLocIndex);
				}
				else
				{
					Logger.Warn("MovementSpeedHook Failed");
				}
			};
		}

		private static void JumpCountHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				int featherCountIndex = -1;

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("baseJumpCount"),
					x => x.MatchLdloc(out featherCountIndex)
				);

				if (found)
				{
					// add
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, featherCountIndex);
					c.EmitDelegate<Func<CharacterBody, int, int>>((self, value) =>
					{
						if (self.teamComponent.teamIndex == TeamIndex.Player)
						{
							if (self.HasBuff(BuffDefOf.AffixRed) && Configuration.AspectRedExtraJump.Value) value++;

							if (self.HasBuff(BuffDefOf.AffixOppressive) && Configuration.AspectOppressiveExtraJump.Value) value++;

							if (self.HasBuff(BuffDefOf.AffixEmpowering) && Configuration.AspectEmpoweringExtraJump.Value) value++;

							if (self.HasBuff(BuffDefOf.AffixBuffing) && Configuration.AspectBannerExtraJump.Value) value++;

							if (self.HasBuff(BuffDefOf.AffixMotivator) && Configuration.AspectMotivatorExtraJump.Value) value++;
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, featherCountIndex);
				}
				else
				{
					Logger.Warn("JumpCountHook Failed");
				}
			};
		}

		private static void DamageHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				int BaseDamageLocIndex = -1;

				if (c.TryGotoNext(
					x => x.MatchLdfld<CharacterBody>("baseDamage"),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("levelDamage")
				))
				{
					if (!c.TryGotoNext(x => x.MatchStloc(out BaseDamageLocIndex)))
					{
						Logger.Warn("DamageHook Failed - Could not find BaseDamageLocIndex");
						return;
					}
				}



				int MultDamageLocIndex = -1;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(BaseDamageLocIndex),
					x => x.MatchLdloc(out MultDamageLocIndex),
					x => x.MatchMul(),
					x => x.MatchStloc(BaseDamageLocIndex)
				);

				if (found)
				{
					// increase
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, MultDamageLocIndex);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(BuffDefOf.ZetHeadHunter))
						{
							value += Configuration.HeadHunterBuffDamage.Value * self.GetBuffCount(BuffDefOf.ZetHeadHunter);
						}

						if (self.HasBuff(BuffDefOf.AffixVoid))
						{
							bool onPrimaryEquipment = false;

							Inventory inventory = self.inventory;
							if (inventory)
							{
								if (inventory.currentEquipmentIndex == DLC1Content.Equipment.EliteVoidEquipment.equipmentIndex)
								{
									onPrimaryEquipment = true;
								}
							}

							if (Compat.EliteReworks.eliteVoidEnabled)
							{
								float voidDamage = Compat.EliteReworks.eliteVoidDamage;

								if (voidDamage > 0f)
								{
									value -= voidDamage - 0.7f;
								}
							}

							if (onPrimaryEquipment)
							{
								value += 0.3f;
							}

							if (Configuration.AspectVoidBaseDamageGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixVoid);
								value += Configuration.AspectVoidBaseDamageGain.Value + Configuration.AspectVoidStackDamageGain.Value * (count - 1f);
							}
						}

						if (self.HasBuff(BuffDefOf.AffixBlighted))
						{
							if (Configuration.AspectBlightedBaseDamageGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixBlighted);
								value += Configuration.AspectBlightedBaseDamageGain.Value + Configuration.AspectBlightedStackDamageGain.Value * (count - 1f);
							}
						}

						if (self.HasBuff(BuffDefOf.AffixBlackHole) && Configuration.AspectBlackHoleBaseDamageGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixBlackHole);
							value += Configuration.AspectBlackHoleBaseDamageGain.Value + Configuration.AspectBlackHoleStackDamageGain.Value * (count - 1f);
						}

						if (self.HasBuff(BuffDefOf.AffixEmpowering))
						{
							if (Configuration.AspectEmpoweringBaseDamageGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixEmpowering);
								value += Configuration.AspectEmpoweringBaseDamageGain.Value + Configuration.AspectEmpoweringStackDamageGain.Value * (count - 1f);
							}
						}

						if (self.HasBuff(BuffDefOf.AffixBuffing) && Configuration.AspectBannerBaseDamageGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixBuffing);
							value += Configuration.AspectBannerBaseDamageGain.Value + Configuration.AspectBannerStackDamageGain.Value * (count - 1f);
						}

						if (AspectPackDefOf.EliteVariety.Enabled && Configuration.AspectTinkerBaseMinionDamageGain.Value > 0f)
						{
							CharacterMaster master = self.master;
							if (master && EffectHooks.IsValidDrone(master))
							{
								CharacterBody ownerBody = EffectHooks.GetMinionOwnerBody(master);
								if (ownerBody && ownerBody.HasBuff(BuffDefOf.AffixTinkerer))
								{
									float count = Catalog.GetStackMagnitude(ownerBody, BuffDefOf.AffixTinkerer);
									value += Configuration.AspectTinkerBaseMinionDamageGain.Value + Configuration.AspectTinkerStackMinionDamageGain.Value * (count - 1f);
								}
							}
						}

						if (self.HasBuff(BuffDefOf.AffixMotivator) && Configuration.AspectMotivatorBaseDamageGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixMotivator);
							value += Configuration.AspectMotivatorBaseDamageGain.Value + Configuration.AspectMotivatorStackDamageGain.Value * (count - 1f);
						}

						if (self.HasBuff(BuffDefOf.AffixEmpyrean))
						{
							if (Configuration.AspectEmpyreanBaseDamageGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixEmpyrean);
								value += Configuration.AspectEmpyreanBaseDamageGain.Value + Configuration.AspectEmpyreanStackDamageGain.Value * (count - 1f);
							}
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, MultDamageLocIndex);

					c.Index += 4;

					// multiplier
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, BaseDamageLocIndex);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(BuffDefOf.ZetSapped))
						{
							float delta = Mathf.Abs(Configuration.AspectBlueSappedDamage.Value);
							value *= 1f - Mathf.Min(0.9f, delta);
						}

						if (AspectPackDefOf.EliteVariety.Populated && Configuration.AspectTinkerTweaks.Value && self.bodyIndex == Catalog.tinkerDroneBodyIndex)
						{
							if (self.teamComponent.teamIndex != TeamIndex.Player) value *= Configuration.AspectTinkerMonsterDamageMult.Value;
							else value *= Configuration.AspectTinkerPlayerDamageMult.Value;
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, BaseDamageLocIndex);
				}
				else
				{
					Logger.Warn("DamageHook Failed");
				}
			};
		}

		private static void ShieldHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				if (c.TryGotoNext(
					x => x.MatchLdfld<CharacterBody>("baseMaxShield"),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("levelMaxShield")
				))
				{
					if (!c.TryGotoNext(x => x.MatchStloc(out BaseShieldLocIndex)))
					{
						Logger.Warn("ShieldHook Failed - Could not find BaseShieldLocIndex");
						return;
					}
				}



				bool found = c.TryGotoNext(
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("EngiShield")),
					x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff")
				);

				if (found)
				{
					// add
					c.Emit(OpCodes.Ldloc, BaseShieldLocIndex);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_maxHealth"));
					c.EmitDelegate<Func<CharacterBody, float, float, float>>((self, shield, health) =>
					{
						if (self.HasBuff(BuffDefOf.AffixBlue) && Configuration.AspectBlueBaseShieldGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixBlue);
							shield += health * (Configuration.AspectBlueBaseShieldGain.Value + Configuration.AspectBlueStackShieldGain.Value * (count - 1f));
						}

						if (Compat.WarWisp.shieldOverrideHook)
						{
							if (self.HasBuff(BuffDefOf.AffixNullifier) && Configuration.AspectNullifierBaseShieldGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixNullifier);
								shield += health * (Configuration.AspectNullifierBaseShieldGain.Value + Configuration.AspectNullifierStackShieldGain.Value * (count - 1f));
							}
						}

						return shield;
					});
					c.Emit(OpCodes.Stloc, BaseShieldLocIndex);

					c.Emit(OpCodes.Ldarg, 0);
				}
				else
				{
					Logger.Warn("ShieldHook Failed");
				}
			};
		}

		private static void HealthHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				int BaseHealthLocIndex = -1;

				if (c.TryGotoNext(
					x => x.MatchLdfld<CharacterBody>("baseMaxHealth"),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("levelMaxHealth")
				))
				{
					if (!c.TryGotoNext(x => x.MatchStloc(out BaseHealthLocIndex)))
					{
						Logger.Warn("HealthHook Failed - Could not find BaseHealthLocIndex");
						return;
					}
				}



				int MultHealthLocIndex = -1;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(BaseHealthLocIndex),
					x => x.MatchLdloc(out MultHealthLocIndex),
					x => x.MatchMul(),
					x => x.MatchStloc(BaseHealthLocIndex)
				);

				if (found)
				{
					// add
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, BaseHealthLocIndex);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(BuffDefOf.AffixPoison) && Configuration.AspectPoisonBaseHealthGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixPoison);
							value += Configuration.AspectPoisonBaseHealthGain.Value + Configuration.AspectPoisonStackHealthGain.Value * (count - 1f);
						}

						if (self.HasBuff(BuffDefOf.AffixPurity) && Configuration.AspectPurityBaseHealthGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixPurity);
							value += Configuration.AspectPurityBaseHealthGain.Value + Configuration.AspectPurityStackHealthGain.Value * (count - 1f);
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, BaseHealthLocIndex);

					// increase
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, MultHealthLocIndex);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(BuffDefOf.ZetHeadHunter))
						{
							value += Configuration.HeadHunterBuffHealth.Value * self.GetBuffCount(BuffDefOf.ZetHeadHunter);
						}

						if (self.HasBuff(BuffDefOf.AffixVoid))
						{
							bool onPrimaryEquipment = false;

							Inventory inventory = self.inventory;
							if (inventory)
							{
								if (inventory.currentEquipmentIndex == DLC1Content.Equipment.EliteVoidEquipment.equipmentIndex)
								{
									onPrimaryEquipment = true;
								}
							}

							if (onPrimaryEquipment)
							{
								value -= 0.5f;
							}

							if (Configuration.AspectVoidBaseHealthGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixVoid);
								value += Configuration.AspectVoidBaseHealthGain.Value + Configuration.AspectVoidStackHealthGain.Value * (count - 1f);
							}
						}

						if (self.HasBuff(BuffDefOf.AffixBlighted))
						{
							if (Configuration.AspectBlightedBaseHealthGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixBlighted);
								value += Configuration.AspectBlightedBaseHealthGain.Value + Configuration.AspectBlightedStackHealthGain.Value * (count - 1f);
							}
						}

						if (Compat.NemSpikeStrip.PlatedEnabled && self.HasBuff(BuffDefOf.AffixPlated) && !Configuration.AspectPlatedPlayerHealthReduction.Value && self.teamComponent.teamIndex == TeamIndex.Player)
						{
							float cfgValue = Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.PlatedHealthField, 0.2f);
							if (cfgValue != 1f)
							{
								value -= cfgValue - 1f;
							}
						}

						if (self.HasBuff(BuffDefOf.AffixEmpyrean))
						{
							if (Configuration.AspectEmpyreanBaseHealthGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixEmpyrean);
								value += Configuration.AspectEmpyreanBaseHealthGain.Value + Configuration.AspectEmpyreanStackHealthGain.Value * (count - 1f);
							}
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, MultHealthLocIndex);
					
					c.Index += 4;

					// multiplier
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, BaseHealthLocIndex);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						//BuffDef targetBuff = Catalog.nemBarrier ? Catalog.Buff.AffixBuffered : Catalog.Buff.AffixBarrier;
						BuffDef targetBuff = BuffDefOf.AffixBarrier;
						if (self.HasBuff(targetBuff) && !Configuration.AspectBarrierPlayerHealthReduction.Value && self.teamComponent.teamIndex == TeamIndex.Player)
						{
							float cfgValue = Compat.RisingTides.GetConfigValue(Compat.RisingTides.BarrierHealthReduction, 50f);

							value /= 1f - cfgValue / 100f;
						}

						if (AspectPackDefOf.EliteVariety.Populated && Configuration.AspectTinkerTweaks.Value && self.bodyIndex == Catalog.tinkerDroneBodyIndex)
						{
							if (self.teamComponent.teamIndex != TeamIndex.Player) value *= Configuration.AspectTinkerMonsterHealthMult.Value;
							else value *= Configuration.AspectTinkerPlayerHealthMult.Value;
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, BaseHealthLocIndex);
				}
				else
				{
					Logger.Warn("HealthHook Failed");
				}
			};
		}

		private static void AttackSpeedHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				int BaseAtkSpdLocIndex = -1;

				if (c.TryGotoNext(
					x => x.MatchLdfld<CharacterBody>("baseAttackSpeed"),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("levelAttackSpeed")
				))
				{
					if (!c.TryGotoNext(x => x.MatchStloc(out BaseAtkSpdLocIndex)))
					{
						Logger.Warn("AttackSpeedHook Failed - Could not find BaseAtkSpdLocIndex");
						return;
					}
				}



				int MultAtkSpdLocIndex = -1;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(BaseAtkSpdLocIndex),
					x => x.MatchLdloc(out MultAtkSpdLocIndex),
					x => x.MatchMul(),
					x => x.MatchStloc(BaseAtkSpdLocIndex)
				);

				if (found)
				{
					// increase
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, MultAtkSpdLocIndex);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(BuffDefOf.ZetHeadHunter))
						{
							value += Configuration.HeadHunterBuffAttackSpeed.Value * self.GetBuffCount(BuffDefOf.ZetHeadHunter);
						}

						if (Compat.PlasmaSpikeStrip.rageStatHook)
						{
							if (self.HasBuff(BuffDefOf.AffixAragonite) && Configuration.AspectAragoniteBaseAtkSpdGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixAragonite);
								float effectValue = Configuration.AspectAragoniteBaseAtkSpdGain.Value + Configuration.AspectAragoniteStackAtkSpdGain.Value * (count - 1f);
								if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectAragoniteMonsterAtkSpdMult.Value;
								value += effectValue;
							}
							else if (Catalog.rageAura != BuffIndex.None && self.HasBuff(Catalog.rageAura) && Configuration.AspectAragoniteAllyAtkSpdGain.Value > 0f)
							{
								float effectValue = Configuration.AspectAragoniteAllyAtkSpdGain.Value;
								if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectAragoniteMonsterAtkSpdMult.Value;
								value += effectValue;
							}
						}

						if (self.HasBuff(BuffDefOf.AffixNight) && Configuration.AspectNightBaseAtkSpdGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixNight);
							float effectValue = Configuration.AspectNightBaseAtkSpdGain.Value + Configuration.AspectNightStackAtkSpdGain.Value * (count - 1f);
							if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectNightMonsterAtkSpdMult.Value;
							value += effectValue;
						}

						if (Compat.RisingTides.nightSpeedStatHook)
						{
							if (self.HasBuff(BuffDefOf.NightSpeed) && Configuration.AspectNightBaseSafeAtkSpdGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixNight);
								float effectValue = Configuration.AspectNightBaseSafeAtkSpdGain.Value + Configuration.AspectNightStackSafeAtkSpdGain.Value * (count - 1f);
								if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectNightMonsterSafeAtkSpdMult.Value;
								value += effectValue;
							}
						}

						if (Compat.MoreElites.frenzyStatHook)
						{
							if (self.HasBuff(BuffDefOf.AffixFrenzied) && Configuration.AspectFrenziedBaseAtkSpdGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixFrenzied);
								float effectValue = Configuration.AspectFrenziedBaseAtkSpdGain.Value + Configuration.AspectFrenziedStackAtkSpdGain.Value * (count - 1f);
								if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectFrenziedMonsterAtkSpdMult.Value;
								value += effectValue;
							}
						}

						if (self.HasBuff(BuffDefOf.AffixSandstorm) && Configuration.AspectCycloneBaseAtkSpdGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixSandstorm);
							value += Configuration.AspectCycloneBaseAtkSpdGain.Value + Configuration.AspectCycloneStackAtkSpdGain.Value * (count - 1f);
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, MultAtkSpdLocIndex);

					c.Index += 4;

					// multiplier
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, BaseAtkSpdLocIndex);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(BuffDefOf.ZetPoached))
						{
							float delta = Mathf.Abs(Configuration.AspectEarthPoachedAttackSpeed.Value);
							value *= 1f - Mathf.Min(0.9f, delta);
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, BaseAtkSpdLocIndex);
				}
				else
				{
					Logger.Warn("AttackSpeedHook Failed");
				}
			};
		}
		
		private static void RegenHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				int findIndex = -1;

				if (c.TryGotoNext(
					x => x.MatchLdfld<CharacterBody>("baseRegen"),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("levelRegen")
				))
				{
					if (!c.TryGotoNext(x => x.MatchStloc(out BaseRegenLocIndex)))
					{
						Logger.Warn("RegenHook Failed - Could not find BaseRegenLocIndex");
						return;
					}
				}

				findIndex = c.Index;



				c.Index = 0;

				int KnurlCountLocIndex = -1;

				if (!c.TryGotoNext(
					x => x.MatchLdsfld(typeof(RoR2Content.Items).GetField("Knurl")),
					x => x.MatchCallOrCallvirt<Inventory>("GetItemCountEffective"),
					x => x.MatchStloc(out KnurlCountLocIndex)
				))
				{
					Logger.Warn("RegenHook Failed - Cound not find KnurlCountLocIndex");
					return;
				}



				c.Index = findIndex;

				int KnurlRegenLocIndex = -1;
				int LevelRegenScaleLocIndex = -1;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(KnurlCountLocIndex),
					x => x.MatchConvR4(),
					x => x.MatchLdcR4(out _),
					x => x.MatchMul(),
					x => x.MatchLdloc(out LevelRegenScaleLocIndex),
					x => x.MatchMul(),
					x => x.MatchStloc(out KnurlRegenLocIndex)
				);

				if (!found)
				{
					Logger.Warn("RegenHook - Could not find KnurlRegen region! , assuming indexes...");
					LevelRegenScaleLocIndex = BaseRegenLocIndex + 1;
					KnurlRegenLocIndex = -1;
				}
				else
				{
					Logger.Info("RegenHook - KnurlRegenLocIndex: " + KnurlRegenLocIndex + ", LevelRegenScaleLocIndex: " + LevelRegenScaleLocIndex);

					if (KnurlRegenLocIndex - LevelRegenScaleLocIndex > 1)
					{
						Logger.Warn("RegenHook - These indexes probably shouldnt be this far apart?");
					}

				}



				int CrocoRegenLocIndex = -1;

				found = c.TryGotoNext(
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("CrocoRegen")),
					x => x.MatchCallOrCallvirt<CharacterBody>("GetBuffCount")
				) && c.TryGotoNext(
					x => x.MatchStloc(out CrocoRegenLocIndex));

				if (!found)
				{
					Logger.Warn("RegenHook Failed - Could not find CrocoRegen Region!");
					return;
				}



				if (KnurlRegenLocIndex == -1)
				{
					KnurlRegenLocIndex = CrocoRegenLocIndex + 1;
				}



				found = c.TryGotoNext(
					x => x.MatchLdcR4(1f),
					x => x.MatchStloc(out _)
				);

				if (found)
				{
					// add (affected by lvl regen scaling and ignites)
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, KnurlRegenLocIndex);
					c.Emit(OpCodes.Ldloc, LevelRegenScaleLocIndex);
					c.EmitDelegate<Func<CharacterBody, float, float, float>>((self, value, scaling) =>
					{
						float amount = 0f;

						Inventory inventory = self.inventory;
						if (inventory)
						{
							bool goldEnabled = self.HasBuff(BuffDefOf.AffixGold) && Configuration.AspectGoldBaseScoredRegenGain.Value > 0f;
							bool pillageEnabled = self.HasBuff(BuffDefOf.AffixPillaging) && Configuration.AspectGoldenBaseScoredRegenGain.Value > 0f;

							if (goldEnabled || pillageEnabled)
							{
								float itemScore = GetItemScore(inventory);

								if (goldEnabled)
								{
									float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixGold);

									float scoredRegen = itemScore * Configuration.AspectGoldItemScoreFactor.Value;
									scoredRegen = Mathf.Pow(scoredRegen, Configuration.AspectGoldItemScoreExponent.Value);
									scoredRegen *= Configuration.AspectGoldBaseScoredRegenGain.Value + Configuration.AspectGoldStackScoredRegenGain.Value * (count - 1f);

									// itemscore regen does not benefit from lvl scaling
									value += scoredRegen * (1f + (Configuration.AspectGoldItemScoreLevelScaling.Value * (self.level - 1f)));
								}

								if (pillageEnabled)
								{
									float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixPillaging);

									float scoredRegen = itemScore * Configuration.AspectGoldenItemScoreFactor.Value;
									scoredRegen = Mathf.Pow(scoredRegen, Configuration.AspectGoldenItemScoreExponent.Value);
									scoredRegen *= Configuration.AspectGoldenBaseScoredRegenGain.Value + Configuration.AspectGoldenStackScoredRegenGain.Value * (count - 1f);

									// itemscore regen does not benefit from lvl scaling
									value += scoredRegen * (1f + (Configuration.AspectGoldenItemScoreLevelScaling.Value * (self.level - 1f)));
								}
							}
						}

						if (self.HasBuff(BuffDefOf.AffixEarth) && Configuration.AspectEarthRegeneration.Value > 0f)
						{
							amount += 1.6f;
						}

						if (self.HasBuff(BuffDefOf.AffixSepia) && Configuration.AspectSepiaBaseRegenGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixSepia);
							amount += Configuration.AspectSepiaBaseRegenGain.Value + Configuration.AspectSepiaStackRegenGain.Value * (count - 1f);
						}

						if (self.HasBuff(BuffDefOf.AffixPurity) && Configuration.AspectPurityBaseRegenGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixPurity);
							amount += Configuration.AspectPurityBaseRegenGain.Value + Configuration.AspectPurityStackRegenGain.Value * (count - 1f);
						}

						if (self.HasBuff(BuffDefOf.AffixMoney) && Configuration.AspectMoneyBaseRegenGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixMoney);
							amount += Configuration.AspectMoneyBaseRegenGain.Value + Configuration.AspectMoneyStackRegenGain.Value * (count - 1f);
						}

						if (amount != 0f)
						{
							if (!self.isPlayerControlled)
							{
								float bonusLevelValue = self.level - 1f;
								if (bonusLevelValue > 0f)
								{
									float increasedRegenPerLevel = (scaling - 1f) / bonusLevelValue;

									scaling = 1f + Mathf.Pow(bonusLevelValue, Configuration.MonsterRegenExponent.Value) * increasedRegenPerLevel;
								}
							}

							value += amount * scaling;
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, KnurlRegenLocIndex);

					// add percent (unaffected by lvl regen scaling and ignites)
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, CrocoRegenLocIndex);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						float amount = 0f;

						if (self.HasBuff(BuffDefOf.AffixEarth) && Configuration.AspectEarthRegeneration.Value > 0f)
						{
							amount += Configuration.AspectEarthRegeneration.Value;
						}

						if (amount > 0f && AllowPercentHealthRegen(self))
						{
							value += self.maxHealth * amount;
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, CrocoRegenLocIndex);
				}
				else
				{
					Logger.Warn("RegenHook Failed");
				}
			};
		}

		public static float GetItemScore(Inventory inventory)
		{
			float itemScore = 0f;

			itemScore += inventory.GetTotalItemCountOfTier(ItemTier.Tier1);
			itemScore += inventory.GetTotalItemCountOfTier(ItemTier.VoidTier1);
			itemScore += 3f * inventory.GetTotalItemCountOfTier(ItemTier.Tier2);
			itemScore += 3f * inventory.GetTotalItemCountOfTier(ItemTier.VoidTier2);
			itemScore += 9f * inventory.GetTotalItemCountOfTier(ItemTier.Tier3);
			itemScore += 9f * inventory.GetTotalItemCountOfTier(ItemTier.VoidTier3);
			itemScore += 9f * inventory.GetTotalItemCountOfTier(ItemTier.Boss);
			itemScore += 9f * inventory.GetTotalItemCountOfTier(ItemTier.VoidBoss);
			itemScore += 9f * inventory.GetTotalItemCountOfTier(ItemTier.Lunar);
			ItemTier lunarVoidTier = Catalog.lunarVoidTier;
			if (lunarVoidTier != ItemTier.AssignedAtRuntime)
			{
				itemScore += 9f * inventory.GetTotalItemCountOfTier(lunarVoidTier);
			}

			float equipmentEffect = 9f * Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);
			if (Catalog.ItemizeEliteEquipment(inventory.currentEquipmentIndex) != ItemIndex.None) itemScore += equipmentEffect;
			if (Catalog.ItemizeEliteEquipment(inventory.alternateEquipmentIndex) != ItemIndex.None) itemScore += equipmentEffect;

			return itemScore;
		}

		private static bool AllowPercentHealthRegen(CharacterBody body)
		{
			TeamIndex teamIndex = body.teamComponent.teamIndex;

			if (teamIndex == TeamIndex.Player || body.outOfDanger)
			{
				if (!DisplayHooks.HasInvulnBuff(body))
				{
					if (body.bodyIndex != Catalog.artifactShellBodyIndex && body.bodyIndex != Catalog.goldenTitanBodyIndex)
					{
						return true;
					}
				}
			}

			return false;
		}



		private static void DirectStatHook()
		{
			On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
			{
				orig(self);

				if (self)
				{
					if (self.HasBuff(BuffDefOf.ZetWarped))
					{
						self.moveSpeed *= Configuration.AspectWarpedAltSpeedMult.Value;
						self.jumpPower *= Configuration.AspectWarpedAltJumpMult.Value;
						self.acceleration *= Configuration.AspectWarpedAltAccelerationMult.Value;
					}

					self.armor += GetArmorDelta(self);

					ModifyCrit(self);

					SkillLocator skillLocator = self.skillLocator;
					if (skillLocator)
					{
						float mult = GetCooldownMultiplier(self);
						if (mult != 1f)
						{
							if (skillLocator.primary) skillLocator.primary.cooldownScale *= mult;
							if (skillLocator.secondary) skillLocator.secondary.cooldownScale *= mult;
							if (skillLocator.utility) skillLocator.utility.cooldownScale *= mult;
							if (skillLocator.special) skillLocator.special.cooldownScale *= mult;
						}

						if (skillLocator.secondary)
						{
							if (self.HasBuff(BuffDefOf.AffixBackup))
							{
								if (Compat.GOTCE.backupStatHook)
								{
									if (Configuration.AspectBackupBaseCooldownGain.Value > 0f)
									{
										float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixBackup);
										float coeff = Configuration.AspectBackupBaseCooldownGain.Value + Configuration.AspectBackupStackCooldownGain.Value * (count - 1f);

										coeff = 1f - (Util.ConvertAmplificationPercentageIntoReductionPercentage(coeff * 100f) / 100f);
										//Logger.Warn("Backup Cooldown Mult : " + coeff);
										skillLocator.secondary.cooldownScale *= coeff;
									}
								}

								if (Compat.GOTCE.chargeOverride)
								{
									if (Configuration.AspectBackupBaseChargesGain.Value > 0)
									{
										float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixBackup);
										int charges = Mathf.RoundToInt(Configuration.AspectBackupBaseChargesGain.Value + Configuration.AspectBackupStackChargesGain.Value * (count - 1f));

										//Logger.Warn("Backup Bonus Charges : " + charges);
										skillLocator.secondary.SetBonusStockFromBody(skillLocator.secondary.bonusStockFromBody + charges);
									}
								}
							}
						}
					}
				}
			};
		}

		private static float GetArmorDelta(CharacterBody self)
		{
			float addedArmor = 0f;
			float count;

			if (self.HasBuff(BuffDefOf.AffixHaunted) && Configuration.AspectHauntedBaseArmorGain.Value > 0f)
			{
				count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixHaunted);
				addedArmor += Configuration.AspectHauntedBaseArmorGain.Value + Configuration.AspectHauntedStackArmorGain.Value * (count - 1f);
			}
			else if (self.HasBuff(RoR2Content.Buffs.AffixHauntedRecipient) && Configuration.AspectHauntedAllyArmorGain.Value > 0f)
			{
				addedArmor += Configuration.AspectHauntedAllyArmorGain.Value;
			}

			if (!Compat.NemSpikeStrip.PlatedEnabled || Configuration.AspectPlatedAllowDefenceWithNem.Value)
			{
				if (self.HasBuff(BuffDefOf.AffixPlated) && Configuration.AspectPlatedBaseArmorGain.Value > 0f)
				{
					count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixPlated);
					addedArmor += Configuration.AspectPlatedBaseArmorGain.Value + Configuration.AspectPlatedStackArmorGain.Value * (count - 1f);
				}
			}

			if (self.HasBuff(BuffDefOf.AffixNullifier))
			{
				if (Configuration.AspectNullifierBaseArmorGain.Value > 0f)
				{
					count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixNullifier);
					addedArmor += Configuration.AspectNullifierBaseArmorGain.Value + Configuration.AspectNullifierStackArmorGain.Value * (count - 1f);
				}
			}
			else if (self.HasBuff(Catalog.nullifierRecipient))
			{
				if (Configuration.AspectNullifierOverrideAllyArmor.Value && Compat.WarWisp.cachedAllyArmorValue != -1000000f)
				{
					addedArmor -= Compat.WarWisp.cachedAllyArmorValue;
					addedArmor += Configuration.AspectNullifierAllyArmorGain.Value;
				}
			}

			if (self.HasBuff(BuffDefOf.ZetHeadHunter))
			{
				addedArmor += Configuration.HeadHunterBuffArmor.Value * self.GetBuffCount(BuffDefOf.ZetHeadHunter);
			}

			if (self.HasBuff(BuffDefOf.ZetShredded))
			{
				addedArmor -= Mathf.Abs(Configuration.AspectHauntedShredArmor.Value);
			}

			if (self.HasBuff(BuffDefOf.AffixArmored) && Configuration.AspectArmorBaseArmorGain.Value > 0f)
			{
				count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixArmored);
				addedArmor += Configuration.AspectArmorBaseArmorGain.Value + Configuration.AspectArmorStackArmorGain.Value * (count - 1f);
			}

			return addedArmor;
		}

		private static void ModifyCrit(CharacterBody self)
		{
			float addedCrit = 0f;

			if (self.HasBuff(BuffDefOf.ZetHeadHunter))
			{
				addedCrit += Configuration.HeadHunterBuffCritChance.Value * self.GetBuffCount(BuffDefOf.ZetHeadHunter);
			}

			if (addedCrit > 0f)
			{
				Inventory inventory = self.inventory;
				if (inventory)
				{
					if (inventory.GetItemCount(DLC1Content.Items.ConvertCritChanceToCritDamage) == 0)
					{
						self.crit += addedCrit;
					}
					else
					{
						self.critMultiplier += addedCrit * 0.01f;
					}
				}
			}
		}

		private static float GetCooldownMultiplier(CharacterBody self)
		{
			float mult = 1f;
			float additiveReduction = 0f;

			if (self.HasBuff(BuffDefOf.AffixWarped) && Configuration.AspectWarpedBaseCooldownGain.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixWarped);
				additiveReduction += Configuration.AspectWarpedBaseCooldownGain.Value + Configuration.AspectWarpedStackCooldownGain.Value * (count - 1f);
			}

			if (self.HasBuff(BuffDefOf.AffixSepia) && Configuration.AspectSepiaBaseCooldownGain.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixSepia);
				additiveReduction += Configuration.AspectSepiaBaseCooldownGain.Value + Configuration.AspectSepiaStackCooldownGain.Value * (count - 1f);
			}

			if (Compat.PlasmaSpikeStrip.rageStatHook)
			{
				if (self.HasBuff(BuffDefOf.AffixAragonite) && Configuration.AspectAragoniteBaseCooldownGain.Value > 0f)
				{
					float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixAragonite);
					float effectValue = Configuration.AspectAragoniteBaseCooldownGain.Value + Configuration.AspectAragoniteStackCooldownGain.Value * (count - 1f);
					if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectAragoniteMonsterCooldownMult.Value;
					additiveReduction += effectValue;
				}
				else if (Catalog.rageAura != BuffIndex.None && self.HasBuff(Catalog.rageAura) && Configuration.AspectAragoniteAllyCooldownGain.Value > 0f)
				{
					float effectValue = Configuration.AspectAragoniteAllyCooldownGain.Value;
					if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectAragoniteMonsterCooldownMult.Value;
					additiveReduction += effectValue;
				}
			}

			if (self.HasBuff(BuffDefOf.AffixBlighted) && Configuration.AspectBlightedBaseCooldownGain.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixBlighted);
				additiveReduction += Configuration.AspectBlightedBaseCooldownGain.Value + Configuration.AspectBlightedStackCooldownGain.Value * (count - 1f);
			}

			if (self.HasBuff(BuffDefOf.AffixWater) && Configuration.AspectWaterBaseCooldownGain.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixWater);
				additiveReduction += Configuration.AspectWaterBaseCooldownGain.Value + Configuration.AspectWaterStackCooldownGain.Value * (count - 1f);
			}

			if (Compat.MoreElites.frenzyStatHook)
			{
				if (self.HasBuff(BuffDefOf.AffixFrenzied) && Configuration.AspectFrenziedBaseCooldownGain.Value > 0f)
				{
					float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixFrenzied);
					float effectValue = Configuration.AspectFrenziedBaseCooldownGain.Value + Configuration.AspectFrenziedStackCooldownGain.Value * (count - 1f);
					if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectFrenziedMonsterCooldownMult.Value;
					additiveReduction += effectValue;
				}
			}

			if (self.HasBuff(BuffDefOf.AffixEcho) && Configuration.AspectEchoBaseCooldownGain.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixEcho);
				float effectValue = Configuration.AspectEchoBaseCooldownGain.Value + Configuration.AspectEchoStackCooldownGain.Value * (count - 1f);
				if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectEchoMonsterCooldownMult.Value;
				additiveReduction += effectValue;
			}

			if (self.HasBuff(BuffDefOf.AffixOsmium) && Configuration.AspectOsmiumBaseCooldownGain.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixOsmium);
				additiveReduction += Configuration.AspectOsmiumBaseCooldownGain.Value + Configuration.AspectOsmiumStackCooldownGain.Value * (count - 1f);
			}

			if (self.HasBuff(BuffDefOf.AffixEmpyrean) && Configuration.AspectEmpyreanBaseCooldownGain.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixEmpyrean);
				additiveReduction += Configuration.AspectEmpyreanBaseCooldownGain.Value + Configuration.AspectEmpyreanStackCooldownGain.Value * (count - 1f);
			}

			if (additiveReduction > 0f)
			{
				mult *= 1f - (Util.ConvertAmplificationPercentageIntoReductionPercentage(additiveReduction * 100f) / 100f);
			}

			return mult;
		}



		private static void DisableOverloadingShieldConversionHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("AffixBlue")),
					x => x.MatchCall<CharacterBody>("HasBuff")
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 0);
				}
				else
				{
					Logger.Warn("DisableOverloadingShieldConversionHook Failed");
				}
			};
		}

		private static void ShieldConversionHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(BaseRegenLocIndex)
				);

				if (!found)
				{
					Logger.Warn("ShieldConversionHook - BaseRegenLocIndex Failed");
					return;
				}



				found = c.TryGotoPrev(MoveType.After,
					x => x.MatchCallOrCallvirt<CharacterBody>("set_maxShield")
				);

				if (found)
				{
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Action<CharacterBody>>((body) =>
					{
						float healthRemaining = 1f;

						if (!Compat.EliteReworks.affixBlueEnabled || !Compat.EliteReworks.affixBlueRemoveShield)
						{
							if (body.HasBuff(BuffDefOf.AffixBlue) && Configuration.AspectBlueHealthConverted.Value > 0f)
							{
								healthRemaining *= 1f - Mathf.Clamp(Configuration.AspectBlueHealthConverted.Value, 0f, 1f);
							}
						}

						if (Compat.WarWisp.shieldOverrideHook)
						{
							if (body.HasBuff(BuffDefOf.AffixNullifier) && Configuration.AspectNullifierHealthConverted.Value > 0f)
							{
								healthRemaining *= 1f - Mathf.Clamp(Configuration.AspectNullifierHealthConverted.Value, 0f, 1f);
							}
						}

						if (healthRemaining < 1f)
						{
							float converted = body.maxHealth * (1f - healthRemaining);

							body.maxHealth = Mathf.Max(1f, body.maxHealth - converted);
							body.maxShield += converted;
						}
					});
				}
				else
				{
					Logger.Warn("ShieldConversionHook - FindSetShield Failed");
				}
			};
		}

		private static void FullShieldConversionHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				int ShieldOnlyCountLocIndex = -1;

				bool found = c.TryGotoNext(
					x => x.MatchLdsfld(typeof(RoR2Content.Items).GetField("ShieldOnly")),
					x => x.MatchCallOrCallvirt<Inventory>("GetItemCountEffective"),
					x => x.MatchStloc(out ShieldOnlyCountLocIndex)
				);

				if (!found)
				{
					Logger.Warn("FullShieldConversionHook - Find ShieldOnlyCountLocIndex Failed");

					return;
				}



				found = c.TryGotoNext(
					x => x.MatchLdcR4(0.25f),
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchStloc(BaseShieldLocIndex)
				);

				if (found)
				{
					c.Index += 5;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, BaseShieldLocIndex);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_maxHealth"));
					c.Emit(OpCodes.Ldloc, ShieldOnlyCountLocIndex);
					c.EmitDelegate<Func<CharacterBody, float, float, int, float>>((self, shield, health, so) =>
					{
						float mult = 1f;
						if (so > 0) mult += 0.25f + (0.25f * so);
						if (self.HasBuff(BuffDefOf.AffixLunar) && Configuration.AspectLunarBaseShieldGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, BuffDefOf.AffixLunar);
							mult += Configuration.AspectLunarBaseShieldGain.Value + Configuration.AspectLunarStackShieldGain.Value * (count - 1f);
						}
						shield += health * mult;

						return shield;
					});
				}
				else
				{
					Logger.Warn("FullShieldConversionHook Failed");
				}
			};
		}
	}
}
