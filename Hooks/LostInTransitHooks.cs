using BepInEx;
using BepInEx.Bootstrap;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using RoR2;
using System;
using System.Reflection;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	internal static class LostInTransitHooks
	{
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static MethodInfo LeechingHealMethod;
		private static MethodInfo FrenziedStatMethod;

		private static FieldInfo FrenziedDoingAbilityField;

		internal static bool leechHook = false;
		internal static bool frenzyMSHook = false;
		internal static bool frenzyASHook = false;
		internal static int frenzyCDRHook = 0;



		internal static void Init()
		{
			Plugin = Chainloader.PluginInfos["com.swuff.LostInTransit"].Instance;
			PluginAssembly = Assembly.GetAssembly(Plugin.GetType());

			if (PluginAssembly != null)
			{
				FindStuff();
			}
			else
			{
				Debug.LogWarning("ZetAspect [LIT] - Could Not Find Assembly");
			}

			if (LeechingHealMethod != null) HookEndpointManager.Modify(LeechingHealMethod, (ILContext.Manipulator)LeechAmountHook);

			if (FrenziedStatMethod != null)
			{
				HookEndpointManager.Modify(FrenziedStatMethod, (ILContext.Manipulator)FrenzyStatHook);

				if (FrenziedDoingAbilityField != null) HookEndpointManager.Modify(FrenziedStatMethod, (ILContext.Manipulator)FrenzyCooldownHook);
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
					if (LeechingHealMethod == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Method : OnDamageDealtServer");
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
					if (FrenziedStatMethod == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Method : RecalculateStatsEnd");

					FrenziedDoingAbilityField = type.GetField("doingAbility", Flags);
					if (FrenziedDoingAbilityField == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Field : doingAbility");
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
		}



		private static void LeechAmountHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchLdloca(1),
				x => x.MatchInitobj<ProcChainMask>()
			);

			if (found)
			{
				leechHook = true;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldarg, 1);
				c.EmitDelegate<Func<DamageReport, float>>((damageReport) =>
				{
					CharacterBody attacker = damageReport.attackerBody;
					BuffDef AffixLeeching = Catalog.LostInTransit.Buffs.AffixLeeching;

					float count = AffixLeeching ? ZetAspectsPlugin.GetStackMagnitude(attacker, AffixLeeching) : 1f;
					float value = Configuration.AspectLeechingBaseLeechGain.Value + Configuration.AspectLeechingStackLeechGain.Value * (count - 1f);

					if (attacker.teamComponent.teamIndex != TeamIndex.Player) value *= Configuration.AspectLeechingMonsterLeechMult.Value;

					return damageReport.damageDealt * damageReport.damageInfo.procCoefficient * value;
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
			if (doingAbility) return -1f;
			else
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

					if (body.teamComponent.teamIndex != TeamIndex.Player) value *= Configuration.AspectFrenziedMonsterCooldownMult.Value;

					return scale * (1f / (1f + value));
				}
			}
		}
	}
}
