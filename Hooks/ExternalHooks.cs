using BepInEx;
using BepInEx.Bootstrap;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RoR2;

namespace TPDespair.ZetAspects
{
	internal static class EliteVarietyHooks
	{
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static Type ImpPlaneImpaledType;
		private static Type SandstormBlindType;

		private static MethodInfo SandstormAwakeMethod;
		private static MethodInfo SandstormTickMethod;

		private static MethodInfo TinkerScrapMethod;

		private static FieldInfo ImpaleDotIndexField;

		private static FieldInfo BlindVisionRadiusField;
		private static FieldInfo BlindCameraEffectField;

		private static FieldInfo SandstormDamageField;
		private static FieldInfo SandstormFrequencyField;



		internal static void Init()
		{
			Plugin = Chainloader.PluginInfos["com.themysticsword.elitevariety"].Instance;
			PluginAssembly = Assembly.GetAssembly(Plugin.GetType());

			if (PluginAssembly != null)
			{
				FindStuff();
			}
			else
			{
				Debug.LogWarning("ZetAspect [EV] - Could Not Find Assembly");
			}

			if (Configuration.AspectCycloneTweaks.Value)
			{
				if (SandstormAwakeMethod != null && SandstormDamageField != null && SandstormFrequencyField != null)
				{
					HookEndpointManager.Modify(SandstormAwakeMethod, (ILContext.Manipulator)TickValueHook);
				}

				if (SandstormTickMethod != null)
				{
					HookEndpointManager.Modify(SandstormTickMethod, (ILContext.Manipulator)TickCritHook);
				}
			}

			if (Configuration.AspectTinkerTweaks.Value)
			{
				if (TinkerScrapMethod != null)
				{
					HookEndpointManager.Modify(TinkerScrapMethod, (ILContext.Manipulator)TinkerScrapHook);
				}
			}
		}



		private static void FindStuff()
		{
			Type type;

			type = Type.GetType("EliteVariety.Buffs.ImpPlaneImpaled, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				ImpPlaneImpaledType = type;

				ImpaleDotIndexField = type.GetField("dotIndex", Flags);
				if (ImpaleDotIndexField == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Field : ImpPlaneImpaled.dotIndex");
			}
			else
			{
				Debug.LogWarning("ZetArtifact [ZetLoopifact] - Could Not Find Type : EliteVariety.Buffs.ImpPlaneImpaled");
			}

			type = Type.GetType("EliteVariety.Buffs.SandstormBlind, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				SandstormBlindType = type;

				BlindVisionRadiusField = type.GetField("maxVisionRadius", Flags);
				if (BlindVisionRadiusField == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Field : SandstormBlind.maxVisionRadius");

				BlindCameraEffectField = type.GetField("cameraEffect", Flags);
				if (BlindCameraEffectField == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Field : SandstormBlind.cameraEffect");
			}
			else
			{
				Debug.LogWarning("ZetAspect [LIT] - Could Not Find Type : EliteVariety.Buffs.SandstormBlind");
			}

			type = Type.GetType("EliteVariety.Buffs.AffixSandstorm, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				type = type.GetNestedType("EliteVarietySandstormBehavior", Flags);
				if (type != null)
				{
					SandstormAwakeMethod = type.GetMethod("Awake", Flags);
					if (SandstormAwakeMethod == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Method : EliteVarietySandstormBehavior.Awake");

					SandstormTickMethod = type.GetMethod("Tick", Flags);
					if (SandstormTickMethod == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Method : EliteVarietySandstormBehavior.Tick");

					SandstormDamageField = type.GetField("damage", Flags);
					if (SandstormDamageField == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Field : EliteVarietySandstormBehavior.damage");

					SandstormFrequencyField = type.GetField("tickFrequency", Flags);
					if (SandstormFrequencyField == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Field : EliteVarietySandstormBehavior.tickFrequency");
				}
				else
				{
					Debug.LogWarning("ZetAspect [EV] - Could Not Find NestedType : EliteVarietySandstormBehavior");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspect [EV] - Could Not Find Type : EliteVariety.Buffs.AffixSandstorm");
			}

			type = Type.GetType("EliteVariety.Buffs.AffixTinkerer, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				TinkerScrapMethod = type.GetMethod("GenericGameEvents_OnHitEnemy", Flags);
				if (TinkerScrapMethod == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Method : AffixTinkerer.GenericGameEvents_OnHitEnemy");
			}
			else
			{
				Debug.LogWarning("ZetAspect [EV] - Could Not Find Type : EliteVariety.Buffs.AffixTinkerer");
			}
		}



		internal static void LateSetup()
		{
			if (Configuration.AspectImpaleTweaks.Value)
			{
				Catalog.EliteVariety.impaleDotIndex = (DotController.DotIndex)ImpaleDotIndexField.GetValue(ImpPlaneImpaledType);
				if (Catalog.PluginLoaded("com.TPDespair.ZetArtifacts")) DisableZetArtifactsImpaleReduction();
			}
			if (Configuration.AspectCycloneTweaks.Value) ModifyCameraEffect();
		}

		private static void DisableZetArtifactsImpaleReduction()
		{
			BaseUnityPlugin ZetPlugin = Chainloader.PluginInfos["com.TPDespair.ZetArtifacts"].Instance;
			Assembly ZetPluginAssembly = Assembly.GetAssembly(ZetPlugin.GetType());

			if (ZetPluginAssembly != null)
			{
				Type type = Type.GetType("TPDespair.ZetArtifacts.ZetLoopifact, " + ZetPluginAssembly.FullName, false);
				if (type != null)
				{
					FieldInfo impaleReductionField = type.GetField("impaleReduction", Flags);
					if (impaleReductionField != null)
					{
						impaleReductionField.SetValue(type, false);
						Debug.LogWarning("ZetAspect [EV] - Field ZetLoopifact.impaleReduction => false");
					}
					else
					{
						Debug.LogWarning("ZetAspect [EV] - Could Not Find Field : ZetLoopifact.impaleReduction");
					}
				}
				else
				{
					Debug.LogWarning("ZetAspect [EV] - Could Not Find Type : TPDespair.ZetArtifacts.ZetLoopifact");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspect [EV] - Could Not Find ZetArtifacts Assembly");
			}
		}

		private static void ModifyCameraEffect()
		{
			float visionRange = 240f;

			if (BlindVisionRadiusField != null) BlindVisionRadiusField.SetValue(SandstormBlindType, visionRange);

			if (BlindCameraEffectField != null)
			{
				GameObject cameraEffect = (GameObject)BlindCameraEffectField.GetValue(SandstormBlindType);
				if (cameraEffect)
				{
					RampFog rampFog = cameraEffect.transform.Find("CameraEffect/PP").GetComponent<PostProcessVolume>().sharedProfile.GetSetting<RampFog>();

					rampFog.fogOne.Override(visionRange * 0.001f);
					rampFog.fogHeightStart.Override(-visionRange * 0.5f);
					rampFog.fogHeightEnd.Override(visionRange * 0.5f);

					BlindCameraEffectField.SetValue(SandstormBlindType, cameraEffect);
				}
				else
				{
					Debug.LogWarning("ZetAspect [EV] - Sandstorm CameraEffect is Null!");
				}
			}
		}



		private static void TickValueHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			c.Index = 0;

			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldc_R4, 10f);
			c.Emit(OpCodes.Stfld, SandstormDamageField);
			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldc_R4, 2f);
			c.Emit(OpCodes.Stfld, SandstormFrequencyField);
		}

		private static void TickCritHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			int critResult = 1;
			ILLabel label = null;

			bool found = c.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(out _),
				x => x.MatchCallOrCallvirt<NetworkedBodyAttachment>("get_attachedBody"),
				x => x.MatchCallOrCallvirt<CharacterBody>("RollCrit"),
				x => x.MatchStloc(out critResult)
			);

			if (found)
			{
				c.Index += 4;
				label = c.MarkLabel();
				c.Index -= 4;
				c.Emit(OpCodes.Ldc_I4, 0);
				c.Emit(OpCodes.Br, label);
			}
			else
			{
				Debug.LogWarning("ZetAspects [EV] - TickCritHook Failed");
			}
		}

		private static void TinkerScrapHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			c.Index = 0;

			c.Emit(OpCodes.Ret);
		}
	}



	internal static class LostInTransitHooks
	{
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static MethodInfo LeechingHealMethod;
		private static MethodInfo FrenziedStatMethod;
		private static MethodInfo VolatileExplodeMethod;

		private static FieldInfo FrenziedDoingAbilityField;

		internal static bool leechHook = false;
		internal static bool frenzyMSHook = false;
		internal static bool frenzyASHook = false;
		internal static int frenzyCDRHook = 0;
		internal static bool explodeHook = false;



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

			if (VolatileExplodeMethod != null) HookEndpointManager.Modify(VolatileExplodeMethod, (ILContext.Manipulator)ExplodeDamageHook);
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
	}



	internal static class AetheriumHooks
	{
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static MethodInfo ApplyBleedMethod;

		internal static bool bleedHook = false;



		internal static void Init()
		{
			Plugin = Chainloader.PluginInfos["com.KomradeSpectre.Aetherium"].Instance;
			PluginAssembly = Assembly.GetAssembly(Plugin.GetType());

			if (PluginAssembly != null)
			{
				FindStuff();
			}
			else
			{
				Debug.LogWarning("ZetAspect [AETH] - Could Not Find Assembly");
			}

			if (ApplyBleedMethod != null) HookEndpointManager.Modify(ApplyBleedMethod, (ILContext.Manipulator)DisableBleedHook);
		}



		private static void FindStuff()
		{
			Type type;

			type = Type.GetType("Aetherium.Equipment.EliteEquipment.AffixSanguine, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				ApplyBleedMethod = type.GetMethod("BleedOnHit", Flags);
				if (ApplyBleedMethod == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Method : BleedOnHit");
			}
			else
			{
				Debug.LogWarning("ZetAspect [AETH] - Could Not Find Type : Aetherium.Equipment.EliteEquipment.AffixSanguine");
			}
		}



		private static void DisableBleedHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchLdloc(0),
				x => x.MatchBrfalse(out _)
			);

			if (found)
			{
				bleedHook = true;

				c.Index += 1;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldc_I4, 0);
			}
			else
			{
				Debug.LogWarning("ZetAspects [AETH] - DisableBleedHook Failed");
			}
		}
	}
}