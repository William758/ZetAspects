using BepInEx;
using BepInEx.Bootstrap;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Reflection;
using UnityEngine;
using RoR2;

namespace TPDespair.ZetAspects
{
	internal static class LostInTransitCompat
	{
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static MethodInfo LeechingHealMethod;
		private static MethodInfo FrenziedStatMethod;
		private static MethodInfo VolatileExplodeMethod;
		private static MethodInfo BlightedStatMethod;
		private static MethodInfo BlightedSetElitesMethod;
		private static MethodInfo BlightedOnDestroyMethod;
		private static MethodInfo BlightedStatStartMethod;
		private static MethodInfo BlightedStatDestroyMethod;

		private static FieldInfo FrenziedDoingAbilityField;
		private static FieldInfo BlightedBodyField;
		private static FieldInfo BlightedFirstBuffField;
		private static FieldInfo BlightedSecondBuffField;

		internal static bool leechHook = false;
		internal static int leechMatchedIndex = 0;
		internal static bool frenzyMSHook = false;
		internal static bool frenzyASHook = false;
		internal static int frenzyCDRHook = 0;
		internal static bool explodeHook = false;
		internal static int blightCDRHook = 0;
		internal static int blightSetEliteHook = 0;
		internal static int blightOnDestroyHook = 0;
		internal static bool blightBuffControl = false;
		internal static bool blightStatControl = false;



		internal static void Init()
		{
			if (!Configuration.LostInTransitHooks.Value) return;

			Plugin = Chainloader.PluginInfos["com.swuff.LostInTransit"].Instance;
			PluginAssembly = Assembly.GetAssembly(Plugin.GetType());

			if (PluginAssembly != null)
			{
				FindStuff();
			}
			else
			{
				Debug.LogWarning("ZetAspect [LIT] - Could Not Find LostInTransit Assembly");
			}

			if (LeechingHealMethod != null) HookEndpointManager.Modify(LeechingHealMethod, (ILContext.Manipulator)LeechAmountHook);

			if (FrenziedStatMethod != null)
			{
				HookEndpointManager.Modify(FrenziedStatMethod, (ILContext.Manipulator)FrenzyStatHook);

				if (FrenziedDoingAbilityField != null) HookEndpointManager.Modify(FrenziedStatMethod, (ILContext.Manipulator)FrenzyCooldownHook);
			}

			if (VolatileExplodeMethod != null) HookEndpointManager.Modify(VolatileExplodeMethod, (ILContext.Manipulator)ExplodeDamageHook);

			if (BlightedStatMethod != null) HookEndpointManager.Modify(BlightedStatMethod, (ILContext.Manipulator)BlightCooldownHook);

			if (BlightedBodyField != null && BlightedFirstBuffField != null && BlightedSecondBuffField != null)
			{
				if (BlightedSetElitesMethod != null && BlightedOnDestroyMethod != null)
				{
					HookEndpointManager.Modify(BlightedSetElitesMethod, (ILContext.Manipulator)BlightEliteHook);
					HookEndpointManager.Modify(BlightedOnDestroyMethod, (ILContext.Manipulator)BlightDestroyHook);

					if (blightSetEliteHook != 4 || blightOnDestroyHook != 2)
					{
						HookEndpointManager.Unmodify(BlightedSetElitesMethod, (ILContext.Manipulator)BlightEliteHook);
						HookEndpointManager.Unmodify(BlightedOnDestroyMethod, (ILContext.Manipulator)BlightDestroyHook);

						Debug.LogWarning("ZetAspect [LIT] - Could Not Fully Control Blight Buffs!");
					}
					else
					{
						blightBuffControl = true;
					}
				}
			}

			if (BlightedStatStartMethod != null && BlightedStatDestroyMethod != null)
			{
				HookEndpointManager.Modify(BlightedStatStartMethod, (ILContext.Manipulator)GenericReturnHook);
				HookEndpointManager.Modify(BlightedStatDestroyMethod, (ILContext.Manipulator)GenericReturnHook);
				blightStatControl = true;
			}
		}



		private static void FindStuff()
		{
			Type type;

			type = Type.GetType("LostInTransit.Buffs.AffixLeeching, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				type = type.GetNestedType("AffixLeechingBehavior", Flags);
				if (type != null)
				{
					LeechingHealMethod = type.GetMethod("OnDamageDealtServer", Flags);
					if (LeechingHealMethod == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Method : AffixLeechingBehavior.OnDamageDealtServer");
				}
				else
				{
					Debug.LogWarning("ZetAspect [LIT] - Could Not Find NestedType : AffixLeechingBehavior");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspect [LIT] - Could Not Find Type : LostInTransit.Buffs.AffixLeeching");
			}

			type = Type.GetType("LostInTransit.Buffs.AffixFrenzied, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				type = type.GetNestedType("AffixFrenziedBehavior", Flags);
				if (type != null)
				{
					FrenziedStatMethod = type.GetMethod("RecalculateStatsEnd", Flags);
					if (FrenziedStatMethod == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Method : AffixFrenziedBehavior.RecalculateStatsEnd");

					FrenziedDoingAbilityField = type.GetField("doingAbility", Flags);
					if (FrenziedDoingAbilityField == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Field : AffixFrenziedBehavior.doingAbility");
				}
				else
				{
					Debug.LogWarning("ZetAspect [LIT] - Could Not Find NestedType : AffixFrenziedBehavior");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspect [LIT] - Could Not Find Type : LostInTransit.Buffs.AffixFrenzied");
			}

			type = Type.GetType("LostInTransit.Buffs.AffixVolatile, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				type = type.GetNestedType("AffixVolatileBehavior", Flags);
				if (type != null)
				{
					VolatileExplodeMethod = type.GetMethod("OnHitAll", Flags);
					if (VolatileExplodeMethod == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Method : AffixVolatileBehavior.OnHitAll");
				}
				else
				{
					Debug.LogWarning("ZetAspect [LIT] - Could Not Find NestedType : AffixVolatileBehavior");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspect [LIT] - Could Not Find Type : LostInTransit.Buffs.AffixVolatile");
			}



			type = Type.GetType("LostInTransit.Buffs.AffixBlighted, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				type = type.GetNestedType("AffixBlightedBehavior", Flags);
				if (type != null)
				{
					BlightedStatMethod = type.GetMethod("RecalculateStatsEnd", Flags);
					if (BlightedStatMethod == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Method : AffixBlightedBehavior.RecalculateStatsEnd");
					BlightedSetElitesMethod = type.GetMethod("SetElites", Flags);
					if (BlightedSetElitesMethod == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Method : AffixBlightedBehavior.SetElites");
					BlightedOnDestroyMethod = type.GetMethod("OnDestroy", Flags);
					if (BlightedOnDestroyMethod == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Method : AffixBlightedBehavior.OnDestroy");

					BlightedBodyField = type.GetField("body", Flags);
					if (BlightedBodyField == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Field : AffixFrenziedBehavior.body");
					BlightedFirstBuffField = type.GetField("firstBuff", Flags);
					if (BlightedFirstBuffField == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Field : AffixBlightedBehavior.firstBuff");
					BlightedSecondBuffField = type.GetField("secondBuff", Flags);
					if (BlightedSecondBuffField == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Field : AffixBlightedBehavior.secondBuff");
				}
				else
				{
					Debug.LogWarning("ZetAspect [LIT] - Could Not Find NestedType : AffixBlightedBehavior");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspect [LIT] - Could Not Find Type : LostInTransit.Buffs.AffixBlighted");
			}

			type = Type.GetType("LostInTransit.Equipments.AffixBlighted, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				type = type.GetNestedType("BlightStatIncrease", Flags);
				if (type != null)
				{
					BlightedStatStartMethod = type.GetMethod("Start", Flags);
					if (BlightedStatStartMethod == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Method : BlightStatIncrease.Start");
					BlightedStatDestroyMethod = type.GetMethod("OnDestroy", Flags);
					if (BlightedStatDestroyMethod == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Method : BlightStatIncrease.OnDestroy");
				}
				else
				{
					Debug.LogWarning("ZetAspect [LIT] - Could Not Find NestedType : BlightStatIncrease");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspect [LIT] - Could Not Find Type : LostInTransit.Equipments.AffixBlighted");
			}
		}



		private static void LeechAmountHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			int localIndex = 0;

			bool found = c.TryGotoNext(
				x => x.MatchLdloca(out localIndex),
				x => x.MatchInitobj<ProcChainMask>()
			);

			if (found)
			{
				leechHook = true;
				leechMatchedIndex = localIndex;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldarg, 1);
				c.EmitDelegate<Func<DamageReport, float>>((damageReport) =>
				{
					CharacterBody attacker = damageReport.attackerBody;
					BuffDef AffixLeeching = Catalog.LostInTransit.Buffs.AffixLeeching;

					float count = AffixLeeching ? ZetAspectsPlugin.GetStackMagnitude(attacker, AffixLeeching) : 1f;
					float value = Configuration.AspectLeechingBaseLeechGain.Value + Configuration.AspectLeechingStackLeechGain.Value * (count - 1f);

					if (attacker.teamComponent.teamIndex != TeamIndex.Player) value *= Configuration.AspectLeechingMonsterLeechMult.Value;

					return Mathf.Max(1f, damageReport.damageDealt * damageReport.damageInfo.procCoefficient * value);
				});
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - LeechAmountHook Failed");
			}
		}



		private static void FrenzyStatHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchCallOrCallvirt<CharacterBody>("get_moveSpeed"),
				x => x.MatchLdcR4(out _),
				x => x.MatchMul(),
				x => x.MatchCallOrCallvirt<CharacterBody>("set_moveSpeed")
			);

			if (found)
			{
				frenzyMSHook = true;

				c.Index += 3;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Dup);
				c.EmitDelegate<Func<CharacterBody, float>>((self) =>
				{
					BuffDef AffixFrenzied = Catalog.LostInTransit.Buffs.AffixFrenzied;

					float count = AffixFrenzied ? ZetAspectsPlugin.GetStackMagnitude(self, AffixFrenzied) : 1f;
					float value = Configuration.AspectFrenziedBaseMovementGain.Value + Configuration.AspectFrenziedStackMovementGain.Value * (count - 1f);

					if (self.teamComponent.teamIndex != TeamIndex.Player) value *= Configuration.AspectFrenziedMonsterMovementMult.Value;

					return self.moveSpeed * (1f + value);
				});
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - FrenzyStatHook:MoveSpeed Failed");
			}

			c.Index = 0;

			found = c.TryGotoNext(
				x => x.MatchCallOrCallvirt<CharacterBody>("get_attackSpeed"),
				x => x.MatchLdcR4(out _),
				x => x.MatchMul(),
				x => x.MatchCallOrCallvirt<CharacterBody>("set_attackSpeed")
			);

			if (found)
			{
				frenzyASHook = true;

				c.Index += 3;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Dup);
				c.EmitDelegate<Func<CharacterBody, float>>((self) =>
				{
					BuffDef AffixFrenzied = Catalog.LostInTransit.Buffs.AffixFrenzied;

					float count = AffixFrenzied ? ZetAspectsPlugin.GetStackMagnitude(self, AffixFrenzied) : 1f;
					float value = Configuration.AspectFrenziedBaseAttackSpeedGain.Value + Configuration.AspectFrenziedStackAttackSpeedGain.Value * (count - 1f);

					if (self.teamComponent.teamIndex != TeamIndex.Player) value *= Configuration.AspectFrenziedMonsterAttackSpeedMult.Value;

					return self.attackSpeed * (1f + value);
				});
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - FrenzyStatHook:AttackSpeed Failed");
			}
		}

		private static void FrenzyCooldownHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			int index = 0;

			bool found = c.TryGotoNext(
				x => x.MatchLdfld<SkillLocator>("primary"),
				x => x.MatchDup(),
				x => x.MatchCallOrCallvirt<GenericSkill>("get_cooldownScale")
			);

			if (found)
			{
				index = c.Index;

				found = c.TryGotoNext(
					x => x.MatchCallOrCallvirt<GenericSkill>("set_cooldownScale")
				);

				if (found && c.Index - index < 12)
				{
					frenzyCDRHook++;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Dup);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldfld, FrenziedDoingAbilityField);
					c.EmitDelegate<Func<GenericSkill, bool, float>>((skill, doingAbility) =>
					{
						return GetFrenziedCooldownReduction(skill, doingAbility);
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects [LIT] - FrenzyCooldownHook:SetPrimary Failed");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - FrenzyCooldownHook:GetPrimary Failed");
			}

			index = 0;
			c.Index = 0;

			found = c.TryGotoNext(
				x => x.MatchLdfld<SkillLocator>("secondary"),
				x => x.MatchDup(),
				x => x.MatchCallOrCallvirt<GenericSkill>("get_cooldownScale")
			);

			if (found)
			{
				index = c.Index;

				found = c.TryGotoNext(
					x => x.MatchCallOrCallvirt<GenericSkill>("set_cooldownScale")
				);

				if (found && c.Index - index < 12)
				{
					frenzyCDRHook++;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Dup);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldfld, FrenziedDoingAbilityField);
					c.EmitDelegate<Func<GenericSkill, bool, float>>((skill, doingAbility) =>
					{
						return GetFrenziedCooldownReduction(skill, doingAbility);
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects [LIT] - FrenzyCooldownHook:SetSecondary Failed");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - FrenzyCooldownHook:GetSecondary Failed");
			}

			index = 0;
			c.Index = 0;

			found = c.TryGotoNext(
				x => x.MatchLdfld<SkillLocator>("utility"),
				x => x.MatchDup(),
				x => x.MatchCallOrCallvirt<GenericSkill>("get_cooldownScale")
			);

			if (found)
			{
				index = c.Index;

				found = c.TryGotoNext(
					x => x.MatchCallOrCallvirt<GenericSkill>("set_cooldownScale")
				);

				if (found && c.Index - index < 12)
				{
					frenzyCDRHook++;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Dup);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldfld, FrenziedDoingAbilityField);
					c.EmitDelegate<Func<GenericSkill, bool, float>>((skill, doingAbility) =>
					{
						return GetFrenziedCooldownReduction(skill, doingAbility);
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects [LIT] - FrenzyCooldownHook:SetUtility Failed");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - FrenzyCooldownHook:GetUtility Failed");
			}

			index = 0;
			c.Index = 0;

			found = c.TryGotoNext(
				x => x.MatchLdfld<SkillLocator>("special"),
				x => x.MatchDup(),
				x => x.MatchCallOrCallvirt<GenericSkill>("get_cooldownScale")
			);

			if (found)
			{
				index = c.Index;

				found = c.TryGotoNext(
					x => x.MatchCallOrCallvirt<GenericSkill>("set_cooldownScale")
				);

				if (found && c.Index - index < 12)
				{
					frenzyCDRHook++;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Dup);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldfld, FrenziedDoingAbilityField);
					c.EmitDelegate<Func<GenericSkill, bool, float>>((skill, doingAbility) =>
					{
						return GetFrenziedCooldownReduction(skill, doingAbility);
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects [LIT] - FrenzyCooldownHook:SetSpecial Failed");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - FrenzyCooldownHook:GetSpecial Failed");
			}
		}

		private static float GetFrenziedCooldownReduction(GenericSkill skill, bool doingAbility)
		{
			CharacterBody body = skill.characterBody;
			float scale = skill.cooldownScale;

			if (!body)
			{
				return scale;
			}
			else
			{
				BuffDef AffixFrenzied = Catalog.LostInTransit.Buffs.AffixFrenzied;

				float count = AffixFrenzied ? ZetAspectsPlugin.GetStackMagnitude(body, AffixFrenzied) : 1f;
				float value = Mathf.Abs(Configuration.AspectFrenziedBaseCooldownGain.Value) + Mathf.Abs(Configuration.AspectFrenziedStackCooldownGain.Value) * (count - 1f);
				if (doingAbility) value *= 2;

				if (body.teamComponent.teamIndex != TeamIndex.Player) value *= Configuration.AspectFrenziedMonsterCooldownMult.Value;

				return scale * (1f / (1f + value));
			}
		}



		private static void ExplodeDamageHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchStfld<BlastAttack>("baseDamage")
			);

			if (found)
			{
				explodeHook = true;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldarg, 1);
				c.EmitDelegate<Func<DamageInfo, float>>((damageInfo) =>
				{
					float count = 1f;
					float value = Configuration.AspectVolatileBaseDamage.Value;

					GameObject attacker = damageInfo.attacker;
					if (attacker)
					{
						CharacterBody atkBody = attacker.GetComponent<CharacterBody>();
						if (atkBody)
						{
							BuffDef AffixVolatile = Catalog.LostInTransit.Buffs.AffixVolatile;
							count = AffixVolatile ? ZetAspectsPlugin.GetStackMagnitude(atkBody, AffixVolatile) : 1f;
						}

						value += Configuration.AspectVolatileStackDamage.Value * (count - 1f);
						if (atkBody && atkBody.teamComponent.teamIndex != TeamIndex.Player) value *= Configuration.AspectEffectMonsterDamageMult.Value;
					}

					return Mathf.Max(1f, damageInfo.damage * value);
				});
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - ExplodeDamageHook Failed");
			}
		}



		private static void BlightCooldownHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			int index = 0;

			bool found = c.TryGotoNext(
				x => x.MatchLdfld<SkillLocator>("primary"),
				x => x.MatchDup(),
				x => x.MatchCallOrCallvirt<GenericSkill>("get_cooldownScale")
			);

			if (found)
			{
				index = c.Index;

				found = c.TryGotoNext(
					x => x.MatchCallOrCallvirt<GenericSkill>("set_cooldownScale")
				);

				if (found && c.Index - index < 12)
				{
					blightCDRHook++;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Dup);
					c.EmitDelegate<Func<GenericSkill, float>>((skill) =>
					{
						return GetBlightedCooldownReduction(skill);
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects [LIT] - BlightCooldownHook:SetPrimary Failed");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - BlightCooldownHook:GetPrimary Failed");
			}

			index = 0;
			c.Index = 0;

			found = c.TryGotoNext(
				x => x.MatchLdfld<SkillLocator>("secondary"),
				x => x.MatchDup(),
				x => x.MatchCallOrCallvirt<GenericSkill>("get_cooldownScale")
			);

			if (found)
			{
				index = c.Index;

				found = c.TryGotoNext(
					x => x.MatchCallOrCallvirt<GenericSkill>("set_cooldownScale")
				);

				if (found && c.Index - index < 12)
				{
					blightCDRHook++;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Dup);
					c.EmitDelegate<Func<GenericSkill, float>>((skill) =>
					{
						return GetBlightedCooldownReduction(skill);
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects [LIT] - BlightCooldownHook:SetSecondary Failed");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - BlightCooldownHook:GetSecondary Failed");
			}

			index = 0;
			c.Index = 0;

			found = c.TryGotoNext(
				x => x.MatchLdfld<SkillLocator>("utility"),
				x => x.MatchDup(),
				x => x.MatchCallOrCallvirt<GenericSkill>("get_cooldownScale")
			);

			if (found)
			{
				index = c.Index;

				found = c.TryGotoNext(
					x => x.MatchCallOrCallvirt<GenericSkill>("set_cooldownScale")
				);

				if (found && c.Index - index < 12)
				{
					blightCDRHook++;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Dup);
					c.EmitDelegate<Func<GenericSkill, float>>((skill) =>
					{
						return GetBlightedCooldownReduction(skill);
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects [LIT] - BlightCooldownHook:SetUtility Failed");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - BlightCooldownHook:GetUtility Failed");
			}

			index = 0;
			c.Index = 0;

			found = c.TryGotoNext(
				x => x.MatchLdfld<SkillLocator>("special"),
				x => x.MatchDup(),
				x => x.MatchCallOrCallvirt<GenericSkill>("get_cooldownScale")
			);

			if (found)
			{
				index = c.Index;

				found = c.TryGotoNext(
					x => x.MatchCallOrCallvirt<GenericSkill>("set_cooldownScale")
				);

				if (found && c.Index - index < 12)
				{
					blightCDRHook++;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Dup);
					c.EmitDelegate<Func<GenericSkill, float>>((skill) =>
					{
						return GetBlightedCooldownReduction(skill);
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects [LIT] - BlightCooldownHook:SetSpecial Failed");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - BlightCooldownHook:GetSpecial Failed");
			}
		}

		private static float GetBlightedCooldownReduction(GenericSkill skill)
		{
			CharacterBody body = skill.characterBody;
			float scale = skill.cooldownScale;

			if (!body)
			{
				return scale;
			}
			else
			{
				BuffDef AffixBlighted = Catalog.LostInTransit.Buffs.AffixBlighted;

				float count = AffixBlighted ? ZetAspectsPlugin.GetStackMagnitude(body, AffixBlighted) : 1f;
				float value = Mathf.Abs(Configuration.AspectBlightedBaseCooldownGain.Value) + Mathf.Abs(Configuration.AspectBlightedStackCooldownGain.Value) * (count - 1f);

				if (body.teamComponent.teamIndex != TeamIndex.Player) value *= Configuration.AspectBlightedMonsterCooldownMult.Value;

				return scale * (1f / (1f + value));
			}
		}

		private static void BlightEliteHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			c.Index = 0;

			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldfld, BlightedBodyField);
			c.Emit(OpCodes.Ldarg, 1);
			c.Emit(OpCodes.Ldarg, 2);
			c.EmitDelegate<Action<CharacterBody, int, int>>((body, firstElite, secondElite) =>
			{
				EliteDef firstEliteDef = EliteCatalog.GetEliteDef((EliteIndex)firstElite);
				EliteDef secondEliteDef = EliteCatalog.GetEliteDef((EliteIndex)secondElite);

				BlightedStateManager.SetElites(body, firstEliteDef.eliteEquipmentDef.passiveBuffDef.buffIndex, secondEliteDef.eliteEquipmentDef.passiveBuffDef.buffIndex);
			});

			bool found = c.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(BlightedFirstBuffField),
				x => x.MatchCallOrCallvirt<CharacterBody>("RemoveBuff")
			);

			if (found)
			{
				blightSetEliteHook++;

				c.Index += 2;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldnull);
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - BlightEliteHook:PreventFirstRemoveBuff Failed");
			}

			found = c.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(BlightedSecondBuffField),
				x => x.MatchCallOrCallvirt<CharacterBody>("RemoveBuff")
			);

			if (found)
			{
				blightSetEliteHook++;

				c.Index += 2;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldnull);
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - BlightEliteHook:PreventSecondRemoveBuff Failed");
			}

			c.Index = 0;

			found = c.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(BlightedFirstBuffField),
				x => x.MatchCallOrCallvirt<CharacterBody>("AddBuff")
			);

			if (found)
			{
				blightSetEliteHook++;

				c.Index += 2;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldnull);
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - BlightEliteHook:PreventFirstAddBuff Failed");
			}

			found = c.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(BlightedSecondBuffField),
				x => x.MatchCallOrCallvirt<CharacterBody>("AddBuff")
			);

			if (found)
			{
				blightSetEliteHook++;

				c.Index += 2;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldnull);
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - BlightEliteHook:PreventSecondAddBuff Failed");
			}
		}

		private static void BlightDestroyHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			c.Index = 0;

			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldfld, BlightedBodyField);
			c.EmitDelegate<Action<CharacterBody>>((body) =>
			{
				BlightedStateManager.Destroyed(body);
			});

			bool found = c.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(BlightedFirstBuffField),
				x => x.MatchCallOrCallvirt<CharacterBody>("RemoveBuff")
			);

			if (found)
			{
				blightOnDestroyHook++;

				c.Index += 2;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldnull);
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - BlightDestroyHook:PreventFirstRemoveBuff Failed");
			}

			found = c.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(BlightedSecondBuffField),
				x => x.MatchCallOrCallvirt<CharacterBody>("RemoveBuff")
			);

			if (found)
			{
				blightOnDestroyHook++;

				c.Index += 2;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldnull);
			}
			else
			{
				Debug.LogWarning("ZetAspects [LIT] - BlightDestroyHook:PreventSecondRemoveBuff Failed");
			}
		}



		private static void GenericReturnHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			c.Index = 0;

			c.Emit(OpCodes.Ret);
		}
	}
}
