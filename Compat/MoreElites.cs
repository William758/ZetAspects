using BepInEx;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using static TPDespair.ZetAspects.ReflectionUtility;

using MonoOpCodes = Mono.Cecil.Cil.OpCodes;
using SysOpCodes = System.Reflection.Emit.OpCodes;

namespace TPDespair.ZetAspects.Compat
{
	internal static class MoreElites
	{
		internal static Reflector Reflector;
		private static readonly string GUID = "com.Nuxlar.MoreElites";
		private static readonly string identifier = "[MoreElitesCompat]";

		private static MethodInfo FrenzyStatMethod;
		private static MethodInfo VolatileExplosionMethod;

		internal static bool frenzyStatHook = false;
		internal static bool explosionHook = false;



		internal static void Init()
		{
			Reflector = new Reflector(GUID, identifier);

			if (!Reflector.FindPluginAssembly()) return;

			Type type = Reflector.GetType("MoreElites.Frenzied");
			if (type != null)
			{
				FrenzyStatMethod = Reflector.GetMethod(type, "Frenzy");
			}

			type = Reflector.GetType("MoreElites.Volatile");
			if (type != null)
			{
				VolatileExplosionMethod = Reflector.GetMethod(type, "AddBehemoExplosion");
			}

			HookMethods();
		}



		private static void HookMethods()
		{
			if (FrenzyStatMethod != null)
			{
				HookEndpointManager.Modify(FrenzyStatMethod, (ILContext.Manipulator)DisableStatHook);

				if (!frenzyStatHook)
				{
					HookEndpointManager.Unmodify(FrenzyStatMethod, (ILContext.Manipulator)DisableStatHook);
					Logger.Warn("[" + identifier + "] - DisableStatHook Failed! - Removing Hook");
				}
			}

			if (VolatileExplosionMethod != null)
			{
				Harmony harmony = ZetAspectsPlugin.harmony;

				harmony.Patch(VolatileExplosionMethod, transpiler: new HarmonyMethod(typeof(MoreElites), nameof(VolatileExplosionTranspiler)));
			}
		}



		// Should be able to disable any RecalculateStatsAPI stats ???
		internal static void DisableStatHook(ILContext il)
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

					Logger.Info(identifier + " - DisableStatHook : DisableStat[" + counter + "] (" + nextInst + ") (" + type + ") at cursor index : " + statArgsIndex);
				}
				else
				{
					Logger.Warn(identifier + " - DisableStatHook : Could not find stat adjustment!");
				}
			}

			if (counter > 0 && !findFail)
			{
				frenzyStatHook = true;
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



		public static IEnumerable<CodeInstruction> VolatileExplosionTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo validateMethod = AccessTools.Method(typeof(MoreElites), nameof(ExplosionDamage));

			List<CodeInstruction> instructionList = instructions.ToList();

			CodeInstruction ld_body = null;

			for (int i = 0; i < instructionList.Count; i++)
			{
				CodeInstruction instruction = instructionList[i];
				yield return instruction;

				if (ld_body == null)
				{
					if (i - 2 >= 0)
					{
						if (instruction.ToString().Contains("HasBuff"))
						{
							Logger.Info(identifier + " - IL [VolatileExplosion] Found : ld_body!");
							ld_body = instructionList[i - 2];
						}
					}
				}
				else
				{
					if (i + 1 < instructionList.Count)
					{
						string nextInstStr = instructionList[i + 1].ToString();
						if (nextInstStr.Contains("stfld") && nextInstStr.Contains("baseDamage"))
						{
							Logger.Info(identifier + " - IL [VolatileExplosion] Match Found!");
							explosionHook = true;

							yield return new CodeInstruction(SysOpCodes.Pop);
							yield return new CodeInstruction(SysOpCodes.Ldarg, 3);
							yield return new CodeInstruction(ld_body.opcode, ld_body.operand);
							yield return new CodeInstruction(SysOpCodes.Call, validateMethod);
						}
					}
				}
			}
		}

		public static float ExplosionDamage(DamageInfo damageInfo, CharacterBody atkBody)
		{
			float damage = damageInfo.damage;
			float count = Catalog.GetStackMagnitude(atkBody, BuffDefOf.AffixVolatile);

			damage *= Configuration.AspectVolatileBaseDamage.Value + Configuration.AspectVolatileStackDamage.Value * (count - 1f);
			if (atkBody.teamComponent.teamIndex != TeamIndex.Player) damage *= Configuration.AspectVolatileMonsterDamageMult.Value;

			return damage;
		}
	}
}
