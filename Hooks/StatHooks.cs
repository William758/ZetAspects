using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

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
			ArmorHook();
			AttackSpeedHook();
			CritChanceHook();
			FullShieldConversionHook();
			ShieldRegenHook();
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
			On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
			{
				orig(self);

				self.armor += GetArmorDelta(self);
			};
		}

		private static float GetArmorDelta(CharacterBody self)
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
			if (self.HasBuff(ZetAspectsContent.Buffs.ZetHeadHunter))
			{
				addedArmor += Configuration.HeadHunterBuffArmor.Value * self.GetBuffCount(ZetAspectsContent.Buffs.ZetHeadHunter);
			}

			float lostArmor = 0f;
			if (self.HasBuff(ZetAspectsContent.Buffs.ZetShredded))
			{
				lostArmor += Mathf.Abs(Configuration.AspectGhostShredArmor.Value);
			}
			if (self.teamComponent.teamIndex == TeamIndex.Player) lostArmor *= Configuration.AspectEffectPlayerDebuffMult.Value;

			return addedArmor - lostArmor;
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

		private static void FullShieldConversionHook()
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
					Debug.LogWarning("ZetAspects - Full Shield Conversion Hook Failed");
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
	}
}
