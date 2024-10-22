using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static TPDespair.ZetAspects.ReflectionUtility;

using MonoOpCodes = Mono.Cecil.Cil.OpCodes;
using SysOpCodes = System.Reflection.Emit.OpCodes;

namespace TPDespair.ZetAspects.Compat
{
	internal static class PlasmaSpikeStrip
	{
		internal static Reflector Reflector;
		private static readonly string GUID = "com.plasmacore.PlasmaCoreSpikestripContent";
		private static readonly string identifier = "[PlasmaSpikeStripCompat]";

		private static Type RageEliteType;
		private static Type CloakEliteType;

		private static MethodInfo RageStatMethod;
		private static MethodInfo CloakOnHitMethod;

		private static MethodInfo CloakBehaviorStartMethod;
		private static MethodInfo CloakBehaviorFixedUpdateMethod;
		private static MethodInfo CloakBehaviorTakeDamageMethod;

		private static FieldInfo EliteBaseBuffArrayField;

		private static FieldInfo RageScriptObjectField;
		private static FieldInfo CloakScriptObjectField;
		private static FieldInfo CloakSmokebombField;

		private static GameObject SmokebombEffect;

		internal static bool rageStatHook = false;
		internal static bool cloakHitHook = false;
		internal static bool cloakStartHook = false;



		internal static void Init()
		{
			Reflector = new Reflector(GUID, identifier);

			if (!Reflector.FindPluginAssembly()) return;

			GatherInfos();
			HookMethods();
		}

		internal static void LateSetup()
		{
			if (Reflector.PluginAssembly == null) return;

			LateHookMethods();
		}



		private static void GatherInfos()
		{
			Type type = Reflector.GetType("PlasmaCoreSpikestripContent.Content.Elites.RagingElite");
			if (type != null)
			{
				RageEliteType = type;

				RageStatMethod = Reflector.GetMethod(type, "RecalculateStatsAPI_GetStatCoefficients");
				RageScriptObjectField = Reflector.GetField(type, "scriptableObject");
			}

			type = Reflector.GetType("PlasmaCoreSpikestripContent.Content.Elites.CloakedElite");
			if (type != null)
			{
				CloakEliteType = type;

				CloakOnHitMethod = Reflector.GetMethod(type, "GlobalEventManager_OnHitEnemy");
				CloakScriptObjectField = Reflector.GetField(type, "scriptableObject");
				CloakSmokebombField = Reflector.GetField(type, "SmokebombEffect");

				type = Reflector.GetType(type, "CloakedAffixBuffBehaviour");
				if (type != null)
				{
					CloakBehaviorStartMethod = Reflector.GetMethod(type, "Start");
					CloakBehaviorFixedUpdateMethod = Reflector.GetMethod(type, "FixedUpdate");
					CloakBehaviorTakeDamageMethod = Reflector.GetMethod(type, "OnTakeDamageServer");
				}
			}

			type = Reflector.GetType("PlasmaCoreSpikestripContent.Core.SSEliteBaseSO");
			if (type != null)
			{
				EliteBaseBuffArrayField = Reflector.GetField(type, "MiscBuffs");
			}
		}

		private static void HookMethods()
		{
			if (RageStatMethod != null)
			{
				HookEndpointManager.Modify(RageStatMethod, (ILContext.Manipulator)DisableStatHook);

				if (!rageStatHook)
				{
					HookEndpointManager.Unmodify(RageStatMethod, (ILContext.Manipulator)DisableStatHook);
					Logger.Warn(identifier + " - DisableStatHook Failed! - Removing Hook");
				}
			}
			
			if (CloakOnHitMethod != null)
			{
				HookEndpointManager.Modify(CloakOnHitMethod, (ILContext.Manipulator)DisableCloakHook);
			}
		}

		private static void LateHookMethods()
		{
			Harmony harmony = ZetAspectsPlugin.harmony;

			if (CloakSmokebombField != null)
			{
				SmokebombEffect = (GameObject)CloakSmokebombField.GetValue(null);
			}

			if (Catalog.veiledBuffer != BuffIndex.None)
			{
				if (CloakBehaviorStartMethod != null)
				{
					harmony.Patch(CloakBehaviorStartMethod, transpiler: new HarmonyMethod(typeof(PlasmaSpikeStrip), nameof(CloakBehaviorStartTranspiler)));
				}

				if (CloakBehaviorFixedUpdateMethod != null)
				{
					harmony.Patch(CloakBehaviorFixedUpdateMethod, prefix: new HarmonyMethod(typeof(PlasmaSpikeStrip), nameof(CloakBehaviorFixedUpdatePrefix)));
				}

				if (CloakBehaviorTakeDamageMethod != null)
				{
					harmony.Patch(CloakBehaviorTakeDamageMethod, prefix: new HarmonyMethod(typeof(PlasmaSpikeStrip), nameof(CloakBehaviorTakeDamagePrefix)));
				}
			}
		}



		public static IEnumerable<CodeInstruction> CloakBehaviorStartTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo validateMethod = AccessTools.Method(typeof(PlasmaSpikeStrip), nameof(CloakBehaviorStart));

			int counter = 0;
			List<CodeInstruction> instructionList = instructions.ToList();

			for (int i = 0; i < instructionList.Count; i++)
			{
				CodeInstruction instruction = instructionList[i];
				string instr = instruction.ToString();

				if (instr.Contains("AddTimedBuff"))
				{
					counter++;

					yield return new CodeInstruction(SysOpCodes.Pop);
					yield return new CodeInstruction(SysOpCodes.Pop);
					yield return new CodeInstruction(SysOpCodes.Ldnull);
					yield return new CodeInstruction(SysOpCodes.Ldc_R4, 0f);
				}

				if (instr.Contains("AddBuff"))
				{
					counter++;

					yield return new CodeInstruction(SysOpCodes.Pop);
					yield return new CodeInstruction(SysOpCodes.Ldnull);
				}

				yield return instruction;

				if (counter == 3)
				{
					counter = 0;
					cloakStartHook = true;

					yield return new CodeInstruction(SysOpCodes.Ldarg, 0);
					yield return new CodeInstruction(SysOpCodes.Ldfld, typeof(CharacterBody.ItemBehavior).GetField("body"));
					yield return new CodeInstruction(SysOpCodes.Call, validateMethod);
				}
			}
		}

		public static void CloakBehaviorStart(CharacterBody body)
		{
			if (!Configuration.AspectVeiledCloakPassive.Value) return;

			if (Configuration.AspectVeiledCloakOnSpawn.Value)
			{
				body.AddTimedBuff(RoR2Content.Buffs.Cloak, 5f);

				if (Configuration.AspectVeiledCloakPassive.Value)
				{
					body.AddBuff(Catalog.veiledBuffer);
					body.AddBuff(Catalog.veiledBuffer);
				}
			}
			else
			{
				body.outOfCombatStopwatch = 0f;
				body.healthComponent.lastHitTime = Run.FixedTimeStamp.now;
			}
		}



		public static bool CloakBehaviorFixedUpdatePrefix(CharacterBody ___body)
		{
			if (!NetworkServer.active) return false;

			if (!Configuration.AspectVeiledCloakPassive.Value) return false;

			if (___body.outOfCombatStopwatch >= 5f && !___body.HasBuff(RoR2Content.Buffs.Cloak) && ___body.healthComponent.timeSinceLastHit >= 10f)
			{
				for (int i = 0; i < 2; i++)
				{
					if (___body.HasBuff(Catalog.veiledBuffer))
					{
						___body.RemoveBuff(Catalog.veiledBuffer);
					}
				}

				___body.AddTimedBuff(RoR2Content.Buffs.Cloak, 5f);
				___body.AddBuff(Catalog.veiledBuffer);
				___body.AddBuff(Catalog.veiledBuffer);

				EffectManager.SpawnEffect(SmokebombEffect, new EffectData
				{
					_origin = ___body.transform.position,
					rotation = ___body.transform.rotation,
					scale = ___body.bestFitRadius * 0.4f
				}, true);
			}

			return false;
		}

		public static bool CloakBehaviorTakeDamagePrefix(DamageReport damageReport, CharacterBody ___body)
		{
			if (___body.HasBuff(Catalog.veiledBuffer))
			{
				___body.RemoveBuff(Catalog.veiledBuffer);

				EffectManager.SpawnEffect(SmokebombEffect, new EffectData
				{
					_origin = damageReport.damageInfo.position,
					rotation = damageReport.victimBody.transform.rotation,
					scale = ___body.bestFitRadius * 0.3f
				}, true);

				if (!___body.HasBuff(Catalog.veiledBuffer))
				{
					if (___body.HasBuff(RoR2Content.Buffs.Cloak))
					{
						___body.ClearTimedBuffs(RoR2Content.Buffs.Cloak.buffIndex);
					}

					if (___body.HasBuff(BuffDefOf.ZetElusive))
					{
						___body.ClearTimedBuffs(BuffDefOf.ZetElusive);
					}
				}
			}

			return false;
		}



		// Should be able to disable any RecalculateStatsAPI stats ???
		private static void DisableStatHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			int counter = 0;
			bool findFail = false;

			while (true)
			{
				bool found = c.TryGotoNext(
					x => x.MatchLdarg(2),
					x => x.MatchDup()
				);

				if (!found) break;

				int statArgsIndex = c.Index;
				int valueFound = -1;

				int nextIndex = FindNextIndex(c, statArgsIndex, out valueFound);

				if (valueFound != -1 && c.Index - statArgsIndex <= 3)
				{
					c.Index = nextIndex + 1;

					bool additive = valueFound == 0;
					string type = additive ? "Add/Sub" : "Mul/Div";
					float cancleValue = additive ? 0f : 1f;

					c.Emit(MonoOpCodes.Pop);
					c.Emit(MonoOpCodes.Ldc_R4, cancleValue);

					c.Index++;

					string nextInst = c.Next.ToString();

					counter++;

					Logger.Info(identifier + " - DisableStatHook : DisableStat[" + counter + "] (" + nextInst+") (" + type + ") at cursor index : " + statArgsIndex);
				}
				else
				{
					Logger.Warn(identifier + " - DisableStatHook : Could not find stat adjustment!");
				}
			}

			if (counter > 0 && !findFail)
			{
				rageStatHook = true;
			}
		}

		private static int FindNextIndex(ILCursor c, int index, out int value)
		{
			int closestMatch = 99999;
			int newValue = -1;

			c.Index = index;
			if (c.TryGotoNext(x => x.MatchLdcR4(out _), x => x.MatchAdd()))
			{
				if (c.Index < closestMatch)
				{
					closestMatch = c.Index;
					newValue = 0;
				}
			}

			c.Index = index;
			if (c.TryGotoNext(x => x.MatchLdcR4(out _), x => x.MatchSub()))
			{
				if (c.Index < closestMatch)
				{
					closestMatch = c.Index;
					newValue = 0;
				}
			}

			c.Index = index;
			if (c.TryGotoNext(x => x.MatchLdcR4(out _), x => x.MatchMul()))
			{
				if (c.Index < closestMatch)
				{
					closestMatch = c.Index;
					newValue = 1;
				}
			}

			c.Index = index;
			if (c.TryGotoNext(x => x.MatchLdcR4(out _), x => x.MatchDiv()))
			{
				if (c.Index < closestMatch)
				{
					closestMatch = c.Index;
					newValue = 1;
				}
			}

			value = newValue;

			return closestMatch;
		}

		private static void DisableCloakHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchLdarg(3),
				x => x.MatchLdfld<DamageInfo>("procCoefficient"),
				x => x.MatchLdcR4(out _)
			);

			if (found)
			{
				c.Index += 3;

				c.Emit(MonoOpCodes.Pop);
				c.Emit(MonoOpCodes.Ldc_R4, 999999f);

				cloakHitHook = true;
			}
			else
			{
				Logger.Warn(identifier + " - DisableCloakHook Failed!");
			}
		}



		internal static BuffIndex GetRageBuffWardBuffIndex()
		{
			if (RageScriptObjectField != null && EliteBaseBuffArrayField != null)
			{
				try
				{
					BuffDef[] buffs = (BuffDef[])EliteBaseBuffArrayField.GetValue(RageScriptObjectField.GetValue(RageEliteType));
					BuffDef buff = buffs[0];
					if (buff)
					{
						BuffIndex index = buff.buffIndex;

						Logger.Info(identifier + " - GetRageBuffWardBuffIndex : " + index);
						return index;
					}
				}
				catch (Exception e)
				{
					Logger.Warn(identifier + " - Error on GetRageBuffWardBuffIndex");
					Logger.Warn(e);
					return BuffIndex.None;
				}
			}

			Logger.Warn(identifier + " - GetRageBuffWardBuffIndex : Could not find MiscBuffs[0]");
			return BuffIndex.None;
		}

		internal static BuffIndex GetVeiledBufferBuffIndex()
		{
			if (CloakScriptObjectField != null && EliteBaseBuffArrayField != null)
			{
				try
				{
					BuffDef[] buffs = (BuffDef[])EliteBaseBuffArrayField.GetValue(CloakScriptObjectField.GetValue(CloakEliteType));
					BuffDef buff = buffs[0];
					if (buff)
					{
						BuffIndex index = buff.buffIndex;

						Logger.Info(identifier + " - GetVeiledBufferBuffIndex : " + index);
						return index;
					}
				}
				catch (Exception e)
				{
					Logger.Warn(identifier + " - Error on GetVeiledBufferBuffIndex");
					Logger.Warn(e);
					return BuffIndex.None;
				}
			}

			Logger.Warn(identifier + " - GetVeiledBufferBuffIndex : Could not find MiscBuffs[0]");
			return BuffIndex.None;
		}
	}
}
