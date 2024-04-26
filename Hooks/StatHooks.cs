using System;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace TPDespair.ZetAspects
{
	internal static class StatHooks
	{
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

			//IL.RoR2.CharacterBody.RecalculateStats += RecalculateStatsEmpyreanIL;
			DisableOverloadingShieldConversionHook();
			ShieldConversionHook();
			FullShieldConversionHook();
		}



		private static void MovementSpeedHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				const int baseValue = 74;
				const int multValue = 75;
				const int divValue = 76;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(baseValue),
					x => x.MatchLdloc(multValue),
					x => x.MatchLdloc(divValue),
					x => x.MatchDiv(),
					x => x.MatchMul(),
					x => x.MatchStloc(baseValue)
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Pop);

					// increase
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, multValue);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						float count;

						if (self.HasBuff(RoR2Content.Buffs.AffixRed) && Configuration.AspectRedBaseMovementGain.Value > 0f)
						{
							count = Catalog.GetStackMagnitude(self, RoR2Content.Buffs.AffixRed);
							value += Configuration.AspectRedBaseMovementGain.Value + Configuration.AspectRedStackMovementGain.Value * (count - 1f);
						}

						if (self.HasBuff(RoR2Content.Buffs.AffixLunar))
						{
							value -= 0.3f;

							if (Configuration.AspectLunarBaseMovementGain.Value > 0f)
							{
								count = Catalog.GetStackMagnitude(self, RoR2Content.Buffs.AffixLunar);
								value += Configuration.AspectLunarBaseMovementGain.Value + Configuration.AspectLunarStackMovementGain.Value * (count - 1f);
							}
						}

						if (self.HasBuff(Catalog.Buff.ZetHeadHunter))
						{
							value += Configuration.HeadHunterBuffMovementSpeed.Value * self.GetBuffCount(Catalog.Buff.ZetHeadHunter);
						}

						if (self.HasBuff(Catalog.Buff.AffixVeiled))
						{
							if (Configuration.AspectVeiledBaseMovementGain.Value > 0f)
							{
								count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixVeiled);
								value += Configuration.AspectVeiledBaseMovementGain.Value + Configuration.AspectVeiledStackMovementGain.Value * (count - 1f);
							}

							if (self.HasBuff(Catalog.Buff.ZetElusive) && Configuration.AspectVeiledElusiveMovementGain.Value > 0f)
							{
								bool nemCloak = Compat.NemSpikeStrip.VeiledEnabled && Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.VeiledHitToShowField, true);

								count = Mathf.Max(5f, self.GetBuffCount(Catalog.Buff.ZetElusive));
								value += Configuration.AspectVeiledElusiveMovementGain.Value * (count / (nemCloak ? 40f : 20f));
							}
						}

						if (Compat.PlasmaSpikeStrip.rageStatHook)
						{
							if (self.HasBuff(Catalog.Buff.AffixAragonite) && Configuration.AspectAragoniteBaseMovementGain.Value > 0f)
							{
								count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixAragonite);
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

						if (self.HasBuff(Catalog.Buff.AffixNight) && Configuration.AspectNightBaseMovementGain.Value > 0f)
						{
							count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixNight);
							float effectValue = Configuration.AspectNightBaseMovementGain.Value + Configuration.AspectNightStackMovementGain.Value * (count - 1f);
							if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectNightMonsterMovementMult.Value;
							value += effectValue;
						}

						if (Compat.RisingTides.nightSpeedStatHook)
						{
							if (self.HasBuff(Catalog.Buff.NightSpeed) && Configuration.AspectNightBaseSafeMovementGain.Value > 0f)
							{
								count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixNight);
								float effectValue = Configuration.AspectNightBaseSafeMovementGain.Value + Configuration.AspectNightStackSafeMovementGain.Value * (count - 1f);
								if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectNightMonsterSafeMovementMult.Value;
								value += effectValue;
							}
						}

						if (self.HasBuff(Catalog.Buff.AffixOppressive) && Configuration.AspectOppressiveBaseMovementGain.Value > 0f)
						{
							count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixOppressive);
							value += Configuration.AspectOppressiveBaseMovementGain.Value + Configuration.AspectOppressiveStackMovementGain.Value * (count - 1f);
						}

						if (Compat.MoreElites.frenzyStatHook)
						{
							if (self.HasBuff(Catalog.Buff.AffixFrenzied) && Configuration.AspectFrenziedBaseMovementGain.Value > 0f)
							{
								count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixFrenzied);
								float effectValue = Configuration.AspectFrenziedBaseMovementGain.Value + Configuration.AspectFrenziedStackMovementGain.Value * (count - 1f);
								if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectFrenziedMonsterMovementMult.Value;
								value += effectValue;
							}
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, multValue);

					c.Emit(OpCodes.Ldloc, baseValue);
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
						if (self.teamComponent.teamIndex == TeamIndex.Player)
						{
							if (self.HasBuff(RoR2Content.Buffs.AffixRed) && Configuration.AspectRedExtraJump.Value) value++;

							if (self.HasBuff(Catalog.Buff.AffixOppressive) && Configuration.AspectOppressiveExtraJump.Value) value++;
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, 8);
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

				const int baseValue = 78;
				const int multValue = 79;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(baseValue),
					x => x.MatchLdloc(multValue),
					x => x.MatchMul(),
					x => x.MatchStloc(baseValue)
				);

				if (found)
				{
					// increase
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, multValue);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(Catalog.Buff.ZetHeadHunter))
						{
							value += Configuration.HeadHunterBuffDamage.Value * self.GetBuffCount(Catalog.Buff.ZetHeadHunter);
						}

						if (self.HasBuff(Catalog.Buff.AffixVoid))
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
								float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixVoid);
								value += Configuration.AspectVoidBaseDamageGain.Value + Configuration.AspectVoidStackDamageGain.Value * (count - 1f);
							}
						}

						if (self.HasBuff(Catalog.Buff.AffixBlighted))
						{
							if (Configuration.AspectBlightedBaseDamageGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixBlighted);
								value += Configuration.AspectBlightedBaseDamageGain.Value + Configuration.AspectBlightedStackDamageGain.Value * (count - 1f);
							}
						}

						if (self.HasBuff(Catalog.Buff.AffixBlackHole) && Configuration.AspectBlackHoleBaseDamageGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixBlackHole);
							value += Configuration.AspectBlackHoleBaseDamageGain.Value + Configuration.AspectBlackHoleStackDamageGain.Value * (count - 1f);
						}

						if (self.HasBuff(Catalog.Buff.AffixEmpowering))
						{
							if (Configuration.AspectEmpoweringBaseDamageGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixEmpowering);
								value += Configuration.AspectEmpoweringBaseDamageGain.Value + Configuration.AspectEmpoweringStackDamageGain.Value * (count - 1f);
							}
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, multValue);

					c.Index += 4;

					// multiplier
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, baseValue);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(Catalog.Buff.ZetSapped))
						{
							float delta = Mathf.Abs(Configuration.AspectBlueSappedDamage.Value);
							value *= 1f - Mathf.Min(0.9f, delta);
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, baseValue);
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

				const int shieldValue = 64;

				bool found = c.TryGotoNext(
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchStloc(shieldValue)
				);

				if (found)
				{
					c.Index += 3;

					// add
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, shieldValue);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_maxHealth"));
					c.EmitDelegate<Func<CharacterBody, float, float, float>>((self, shield, health) =>
					{
						if (self.HasBuff(RoR2Content.Buffs.AffixBlue) && Configuration.AspectBlueBaseShieldGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, RoR2Content.Buffs.AffixBlue);
							shield += health * (Configuration.AspectBlueBaseShieldGain.Value + Configuration.AspectBlueStackShieldGain.Value * (count - 1f));
						}

						if (Compat.WarWisp.shieldOverrideHook)
						{
							if (self.HasBuff(Catalog.Buff.AffixNullifier) && Configuration.AspectNullifierBaseShieldGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixNullifier);
								shield += health * (Configuration.AspectNullifierBaseShieldGain.Value + Configuration.AspectNullifierStackShieldGain.Value * (count - 1f));
							}
						}

						return shield;
					});
					c.Emit(OpCodes.Stloc, shieldValue);
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

				const int baseValue = 62;
				const int multValue = 63;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(baseValue),
					x => x.MatchLdloc(multValue),
					x => x.MatchMul(),
					x => x.MatchStloc(baseValue)
				);

				if (found)
				{
					// add
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, baseValue);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(RoR2Content.Buffs.AffixPoison) && Configuration.AspectPoisonBaseHealthGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, RoR2Content.Buffs.AffixPoison);
							value += Configuration.AspectPoisonBaseHealthGain.Value + Configuration.AspectPoisonStackHealthGain.Value * (count - 1f);
						}

						if (self.HasBuff(Catalog.Buff.AffixPurity) && Configuration.AspectPurityBaseHealthGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixPurity);
							value += Configuration.AspectPurityBaseHealthGain.Value + Configuration.AspectPurityStackHealthGain.Value * (count - 1f);
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, baseValue);

					// increase
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, multValue);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(Catalog.Buff.ZetHeadHunter))
						{
							value += Configuration.HeadHunterBuffHealth.Value * self.GetBuffCount(Catalog.Buff.ZetHeadHunter);
						}

						if (self.HasBuff(Catalog.Buff.AffixVoid))
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
								float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixVoid);
								value += Configuration.AspectVoidBaseHealthGain.Value + Configuration.AspectVoidStackHealthGain.Value * (count - 1f);
							}
						}

						if (self.HasBuff(Catalog.Buff.AffixBlighted))
						{
							if (Configuration.AspectBlightedBaseHealthGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixBlighted);
								value += Configuration.AspectBlightedBaseHealthGain.Value + Configuration.AspectBlightedStackHealthGain.Value * (count - 1f);
							}
						}

						if (Compat.NemSpikeStrip.PlatedEnabled && self.HasBuff(Catalog.Buff.AffixPlated) && !Configuration.AspectPlatedPlayerHealthReduction.Value && self.teamComponent.teamIndex == TeamIndex.Player)
						{
							float cfgValue = Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.PlatedHealthField, 0.2f);
							if (cfgValue != 1f)
							{
								value -= cfgValue - 1f;
							}
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, multValue);
					
					c.Index += 4;

					// multiplier
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, baseValue);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						//BuffDef targetBuff = Catalog.nemBarrier ? Catalog.Buff.AffixBuffered : Catalog.Buff.AffixBarrier;
						BuffDef targetBuff = Catalog.Buff.AffixBarrier;
						if (self.HasBuff(targetBuff) && !Configuration.AspectBarrierPlayerHealthReduction.Value && self.teamComponent.teamIndex == TeamIndex.Player)
						{
							float cfgValue = Compat.RisingTides.GetConfigValue(Compat.RisingTides.BarrierHealthReduction, 50f);

							value /= 1f - cfgValue / 100f;
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, baseValue);
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

				const int baseValue = 82;
				const int multValue = 83;

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(baseValue),
					x => x.MatchLdloc(multValue),
					x => x.MatchMul(),
					x => x.MatchStloc(baseValue)
				);

				if (found)
				{
					// increase
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, multValue);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(Catalog.Buff.ZetHeadHunter))
						{
							value += Configuration.HeadHunterBuffAttackSpeed.Value * self.GetBuffCount(Catalog.Buff.ZetHeadHunter);
						}

						if (Compat.PlasmaSpikeStrip.rageStatHook)
						{
							if (self.HasBuff(Catalog.Buff.AffixAragonite) && Configuration.AspectAragoniteBaseAtkSpdGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixAragonite);
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

						if (self.HasBuff(Catalog.Buff.AffixNight) && Configuration.AspectNightBaseAtkSpdGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixNight);
							float effectValue = Configuration.AspectNightBaseAtkSpdGain.Value + Configuration.AspectNightStackAtkSpdGain.Value * (count - 1f);
							if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectNightMonsterAtkSpdMult.Value;
							value += effectValue;
						}

						if (Compat.RisingTides.nightSpeedStatHook)
						{
							if (self.HasBuff(Catalog.Buff.NightSpeed) && Configuration.AspectNightBaseSafeAtkSpdGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixNight);
								float effectValue = Configuration.AspectNightBaseSafeAtkSpdGain.Value + Configuration.AspectNightStackSafeAtkSpdGain.Value * (count - 1f);
								if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectNightMonsterSafeAtkSpdMult.Value;
								value += effectValue;
							}
						}

						if (Compat.MoreElites.frenzyStatHook)
						{
							if (self.HasBuff(Catalog.Buff.AffixFrenzied) && Configuration.AspectFrenziedBaseAtkSpdGain.Value > 0f)
							{
								float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixFrenzied);
								float effectValue = Configuration.AspectFrenziedBaseAtkSpdGain.Value + Configuration.AspectFrenziedStackAtkSpdGain.Value * (count - 1f);
								if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectFrenziedMonsterAtkSpdMult.Value;
								value += effectValue;
							}
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, multValue);

					c.Index += 4;

					// multiplier
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, baseValue);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(Catalog.Buff.ZetPoached))
						{
							float delta = Mathf.Abs(Configuration.AspectEarthPoachedAttackSpeed.Value);
							value *= 1f - Mathf.Min(0.9f, delta);
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, baseValue);
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

				const int lvlScaling = 66;
				const int knurlValue = 67;
				const int crocoValue = 70;
				const int multValue = 72;

				bool found = c.TryGotoNext(
					x => x.MatchLdcR4(1f),
					x => x.MatchStloc(multValue)
				);

				if (found)
				{
					// add (affected by lvl regen scaling and ignites)
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, knurlValue);
					c.Emit(OpCodes.Ldloc, lvlScaling);
					c.EmitDelegate<Func<CharacterBody, float, float, float>>((self, value, scaling) =>
					{
						float amount = 0f;

						if (self.HasBuff(Catalog.Buff.AffixEarth) && Configuration.AspectEarthRegeneration.Value > 0f)
						{
							amount += 1.6f;
						}

						if (self.HasBuff(Catalog.Buff.AffixGold))
						{
							float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixGold);

							if (Configuration.AspectGoldBaseRegenGain.Value > 0f)
							{
								amount += Configuration.AspectGoldBaseRegenGain.Value + Configuration.AspectGoldStackRegenGain.Value * (count - 1f);
							}

							if (Configuration.AspectGoldBaseScoredRegenGain.Value > 0f)
							{
								Inventory inventory = self.inventory;
								if (inventory)
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

									itemScore *= Configuration.AspectGoldItemScoreFactor.Value;
									itemScore = Mathf.Pow(itemScore, Configuration.AspectGoldItemScoreExponent.Value);
									itemScore *= Configuration.AspectGoldBaseScoredRegenGain.Value + Configuration.AspectGoldStackScoredRegenGain.Value * (count - 1f);

									// itemscore regen does not benefit from lvl scaling
									value += itemScore * (1f + (Configuration.AspectGoldItemScoreLevelScaling.Value * (self.level - 1f)));
								}
							}
						}

						if (self.HasBuff(Catalog.Buff.AffixSepia) && Configuration.AspectSepiaBaseRegenGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixSepia);
							amount += Configuration.AspectSepiaBaseRegenGain.Value + Configuration.AspectSepiaStackRegenGain.Value * (count - 1f);
						}

						if (self.HasBuff(Catalog.Buff.AffixPurity) && Configuration.AspectPurityBaseRegenGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixPurity);
							amount += Configuration.AspectPurityBaseRegenGain.Value + Configuration.AspectPurityStackRegenGain.Value * (count - 1f);
						}

						if (self.HasBuff(Catalog.Buff.AffixMoney) && Configuration.AspectMoneyBaseRegenGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixMoney);
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
					c.Emit(OpCodes.Stloc, knurlValue);

					// add percent (unaffected by lvl regen scaling and ignites)
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, crocoValue);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						float amount = 0f;

						if (self.HasBuff(Catalog.Buff.AffixEarth) && Configuration.AspectEarthRegeneration.Value > 0f)
						{
							amount += Configuration.AspectEarthRegeneration.Value;
						}

						if (amount > 0f && AllowPercentHealthRegen(self))
						{
							value += self.maxHealth * amount;
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, crocoValue);
				}
				else
				{
					Logger.Warn("RegenHook Failed");
				}
			};
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
					if (self.HasBuff(Catalog.Buff.ZetWarped))
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
							if (self.HasBuff(Catalog.Buff.AffixBackup))
							{
								if (Compat.GOTCE.backupStatHook)
								{
									if (Configuration.AspectBackupBaseCooldownGain.Value > 0f)
									{
										float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixBackup);
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
										float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixBackup);
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

			if (self.HasBuff(RoR2Content.Buffs.AffixHaunted) && Configuration.AspectHauntedBaseArmorGain.Value > 0f)
			{
				count = Catalog.GetStackMagnitude(self, RoR2Content.Buffs.AffixHaunted);
				addedArmor += Configuration.AspectHauntedBaseArmorGain.Value + Configuration.AspectHauntedStackArmorGain.Value * (count - 1f);
			}
			else if (self.HasBuff(RoR2Content.Buffs.AffixHauntedRecipient) && Configuration.AspectHauntedAllyArmorGain.Value > 0f)
			{
				addedArmor += Configuration.AspectHauntedAllyArmorGain.Value;
			}

			if (!Compat.NemSpikeStrip.PlatedEnabled || Configuration.AspectPlatedAllowDefenceWithNem.Value)
			{
				if (self.HasBuff(Catalog.Buff.AffixPlated) && Configuration.AspectPlatedBaseArmorGain.Value > 0f)
				{
					count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixPlated);
					addedArmor += Configuration.AspectPlatedBaseArmorGain.Value + Configuration.AspectPlatedStackArmorGain.Value * (count - 1f);
				}
			}

			if (self.HasBuff(Catalog.Buff.AffixNullifier))
			{
				if (Configuration.AspectNullifierBaseArmorGain.Value > 0f)
				{
					count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixNullifier);
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

			if (self.HasBuff(Catalog.Buff.ZetHeadHunter))
			{
				addedArmor += Configuration.HeadHunterBuffArmor.Value * self.GetBuffCount(Catalog.Buff.ZetHeadHunter);
			}

			if (self.HasBuff(Catalog.Buff.ZetShredded))
			{
				addedArmor -= Mathf.Abs(Configuration.AspectHauntedShredArmor.Value);
			}

			return addedArmor;
		}

		private static void ModifyCrit(CharacterBody self)
		{
			float addedCrit = 0f;

			if (self.HasBuff(Catalog.Buff.ZetHeadHunter))
			{
				addedCrit += Configuration.HeadHunterBuffCritChance.Value * self.GetBuffCount(Catalog.Buff.ZetHeadHunter);
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

			if (self.HasBuff(Catalog.Buff.AffixWarped) && Configuration.AspectWarpedBaseCooldownGain.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixWarped);
				additiveReduction += Configuration.AspectWarpedBaseCooldownGain.Value + Configuration.AspectWarpedStackCooldownGain.Value * (count - 1f);
			}

			if (self.HasBuff(Catalog.Buff.AffixSepia) && Configuration.AspectSepiaBaseCooldownGain.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixSepia);
				additiveReduction += Configuration.AspectSepiaBaseCooldownGain.Value + Configuration.AspectSepiaStackCooldownGain.Value * (count - 1f);
			}

			if (Compat.PlasmaSpikeStrip.rageStatHook)
			{
				if (self.HasBuff(Catalog.Buff.AffixAragonite) && Configuration.AspectAragoniteBaseCooldownGain.Value > 0f)
				{
					float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixAragonite);
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

			if (self.HasBuff(Catalog.Buff.AffixBlighted) && Configuration.AspectBlightedBaseCooldownGain.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixBlighted);
				additiveReduction += Configuration.AspectBlightedBaseCooldownGain.Value + Configuration.AspectBlightedStackCooldownGain.Value * (count - 1f);
			}

			if (self.HasBuff(Catalog.Buff.AffixWater) && Configuration.AspectWaterBaseCooldownGain.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixWater);
				additiveReduction += Configuration.AspectWaterBaseCooldownGain.Value + Configuration.AspectWaterStackCooldownGain.Value * (count - 1f);
			}

			if (Compat.MoreElites.frenzyStatHook)
			{
				if (self.HasBuff(Catalog.Buff.AffixFrenzied) && Configuration.AspectFrenziedBaseCooldownGain.Value > 0f)
				{
					float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixFrenzied);
					float effectValue = Configuration.AspectFrenziedBaseCooldownGain.Value + Configuration.AspectFrenziedStackCooldownGain.Value * (count - 1f);
					if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectFrenziedMonsterCooldownMult.Value;
					additiveReduction += effectValue;
				}
			}

			if (self.HasBuff(Catalog.Buff.AffixEcho) && Configuration.AspectEchoBaseCooldownGain.Value > 0f)
			{
				float count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixEcho);
				float effectValue = Configuration.AspectEchoBaseCooldownGain.Value + Configuration.AspectEchoStackCooldownGain.Value * (count - 1f);
				if (self.teamComponent.teamIndex != TeamIndex.Player) effectValue *= Configuration.AspectEchoMonsterCooldownMult.Value;
				additiveReduction += effectValue;
			}

			if (additiveReduction > 0f)
			{
				mult *= 1f - (Util.ConvertAmplificationPercentageIntoReductionPercentage(additiveReduction * 100f) / 100f);
			}

			return mult;
		}


		/*
		private static void RecalculateStatsEmpyreanIL(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool ILFound = c.TryGotoNext(MoveType.After,
				x => x.MatchLdsfld(typeof(RoR2Content.Buffs), nameof(RoR2Content.Buffs.AffixBlue)),
				x => x.MatchCallOrCallvirt<CharacterBody>(nameof(CharacterBody.HasBuff)),
				x => x.MatchBrfalse(out _),
				x => x.MatchLdarg(0),
				x => x.MatchCallOrCallvirt<CharacterBody>("get_maxHealth"),
				x => x.MatchLdcR4(0.5f)
			);

			if (ILFound)
			{
				c.Emit(OpCodes.Ldarg_0);
				c.EmitDelegate<Func<float, CharacterBody, float>>((defaultPercentage, body) =>
				{
					if (body.HasBuff(JunkContent.Buffs.MeatRegenBoost))
					{
						return 0.1f;
					}
					return defaultPercentage;
				});
				Debug.Log(il);
			}
			else
			{
				Debug.Log("Failed to find IL match for Empyrean hook 1!");
			}
		}
		*/
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
					x => x.MatchStloc(66)
				);

				if (found)
				{
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
								if (body.HasBuff(Catalog.Buff.AffixBlue) && Configuration.AspectBlueHealthConverted.Value > 0f)
								{
									healthRemaining *= 1f - Mathf.Clamp(Configuration.AspectBlueHealthConverted.Value, 0f, 1f);
								}
							}

							if (Compat.WarWisp.shieldOverrideHook)
							{
								if (body.HasBuff(Catalog.Buff.AffixNullifier) && Configuration.AspectNullifierHealthConverted.Value > 0f)
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
						Logger.Warn("ShieldConversionHook:FindSetShield Failed");
					}
				}
				else
				{
					Logger.Warn("ShieldConversionHook:FindRegenScaleVar Failed");
				}
			};
		}

		private static void FullShieldConversionHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				const int shieldValue = 64;

				bool found = c.TryGotoNext(
					x => x.MatchLdcR4(0.25f),
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchStloc(shieldValue)
				);

				if (found)
				{
					c.Index += 5;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, shieldValue);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_maxHealth"));
					c.Emit(OpCodes.Ldloc, 14);
					c.EmitDelegate<Func<CharacterBody, float, float, int, float>>((self, shield, health, so) =>
					{
						float mult = 1f;
						if (so > 0) mult += 0.25f + (0.25f * so);
						if (self.HasBuff(RoR2Content.Buffs.AffixLunar) && Configuration.AspectLunarBaseShieldGain.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(self, RoR2Content.Buffs.AffixLunar);
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
