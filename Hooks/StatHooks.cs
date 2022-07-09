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

			OverloadingShieldConversionHook();
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

						return value;
					});
					c.Emit(OpCodes.Stloc, multValue);
					/*
					c.Index += 4;

					// multiplier
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, baseValue);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						return value;
					});
					c.Emit(OpCodes.Stloc, baseValue);
					*/
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
				if (!body.HasBuff(RoR2Content.Buffs.HiddenInvincibility) && !body.HasBuff(RoR2Content.Buffs.Immune))
				{
					if (body.baseNameToken != "ARTIFACTSHELL_BODY_NAME" && body.baseNameToken != "TITANGOLD_BODY_NAME")
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
					self.armor += GetArmorDelta(self);

					ModifyCrit(self);

					float mult = GetCooldownMultiplier(self);
					if (mult != 1f)
					{
						SkillLocator skillLocator = self.skillLocator;

						if (skillLocator.primary)
						{
							skillLocator.primary.cooldownScale *= mult;
						}
						if (skillLocator.secondary)
						{
							skillLocator.secondary.cooldownScale *= mult;
						}
						if (skillLocator.utility)
						{
							skillLocator.utility.cooldownScale *= mult;
						}
						if (skillLocator.special)
						{
							skillLocator.special.cooldownScale *= mult;
						}
					}
				}
			};
		}

		private static float GetArmorDelta(CharacterBody self)
		{
			float addedArmor = 0f;
			float lostArmor = 0f;
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

			if (self.HasBuff(Catalog.Buff.AffixPlated) && Configuration.AspectPlatedBaseArmorGain.Value > 0f)
			{
				count = Catalog.GetStackMagnitude(self, Catalog.Buff.AffixPlated);
				addedArmor += Configuration.AspectPlatedBaseArmorGain.Value + Configuration.AspectPlatedStackArmorGain.Value * (count - 1f);
			}

			if (self.HasBuff(Catalog.Buff.ZetHeadHunter))
			{
				addedArmor += Configuration.HeadHunterBuffArmor.Value * self.GetBuffCount(Catalog.Buff.ZetHeadHunter);
			}

			if (self.HasBuff(Catalog.Buff.ZetShredded))
			{
				lostArmor += Mathf.Abs(Configuration.AspectHauntedShredArmor.Value);
			}
			//if (self.teamComponent.teamIndex == TeamIndex.Player) lostArmor *= Configuration.AspectEffectPlayerDebuffMult.Value;

			return addedArmor - lostArmor;
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

			if (additiveReduction > 0f)
			{
				mult *= 1f - (Util.ConvertAmplificationPercentageIntoReductionPercentage(additiveReduction * 100f) / 100f);
			}

			return mult;
		}



		private static void OverloadingShieldConversionHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("AffixBlue")),
					x => x.MatchCall<CharacterBody>("HasBuff")
				);

				if (found)
				{
					c.Index += 7;

					// Set health conversion factor
					c.Emit(OpCodes.Pop);
					c.EmitDelegate<Func<float>>(() =>
					{
						return Mathf.Abs(Configuration.AspectBlueHealthConverted.Value);
					});

					c.Index += 11;

					// Add converted health to shield
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc, 66);
				}
				else
				{
					Logger.Warn("OverloadingShieldConversionHook Failed");
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
