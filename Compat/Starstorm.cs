using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

using static TPDespair.ZetAspects.ReflectionUtility;

using MonoOpCodes = Mono.Cecil.Cil.OpCodes;
using SysOpCodes = System.Reflection.Emit.OpCodes;

namespace TPDespair.ZetAspects.Compat
{
	internal static class Starstorm
	{
		internal static Reflector Reflector;
		private static readonly string GUID = "com.TeamMoonstorm";
		private static readonly string identifier = "[StarstormCompat]";

		internal static Reflector MSU_Reflector;
		private static readonly string MSU_GUID = "com.TeamMoonstorm.MSU";

		private static MethodInfo BuffBehaviorGetBodyMethod;

		private static MethodInfo EmpyreanBuffGainMethod;
		private static MethodInfo EmpyreanBuffLostMethod;

		private static FieldInfo EmpyreanWhitelistField;



		public static EmpyreanAspectProvider EmpyreanProviderInstance;



		internal static void LateSetup()
		{
			MSU_Reflector = new Reflector(MSU_GUID, identifier);

			if (!MSU_Reflector.FindPluginAssembly()) return;



			Reflector = new Reflector(GUID, identifier);

			if (!Reflector.FindPluginAssembly()) return;

			GatherInfos();

			if (BuffBehaviorGetBodyMethod != null && EmpyreanWhitelistField != null && EmpyreanBuffGainMethod != null && EmpyreanBuffLostMethod != null)
			{
				HookMethods();

				EmpyreanProviderInstance = new EmpyreanAspectProvider();
				EliteBuffManager.AddProvider(EmpyreanProviderInstance);
			}
			else
			{
				Logger.Info(identifier + " - Aborted!");
			}
		}



		private static void GatherInfos()
		{
			Type type = MSU_Reflector.GetType("MSU.BaseBuffBehaviour");
			if (type != null)
			{
				BuffBehaviorGetBodyMethod = MSU_Reflector.GetMethod(type, "get_characterBody");
			}



			type = Reflector.GetType("SS2.Equipments.AffixEmpyrean");
			if (type != null)
			{
				EmpyreanWhitelistField = Reflector.GetField(type, "whitelistedEliteDefs");
			}

			type = Reflector.GetType("SS2.Equipments.AffixEmpyreanBehavior");
			if (type != null)
			{
				EmpyreanBuffGainMethod = Reflector.GetMethod(type, "OnFirstStackGained");
				EmpyreanBuffLostMethod = Reflector.GetMethod(type, "OnAllStacksLost");
			}
		}

		private static void HookMethods()
		{
			Harmony harmony = ZetAspectsPlugin.harmony;



			harmony.Patch(EmpyreanBuffGainMethod, transpiler: new HarmonyMethod(typeof(Starstorm), nameof(BuffGainTranspiler)));
			harmony.Patch(EmpyreanBuffGainMethod, postfix: new HarmonyMethod(typeof(Starstorm), nameof(BuffGainPostfix)));

			harmony.Patch(EmpyreanBuffLostMethod, transpiler: new HarmonyMethod(typeof(Starstorm), nameof(BuffLostTranspiler)));
			harmony.Patch(EmpyreanBuffLostMethod, postfix: new HarmonyMethod(typeof(Starstorm), nameof(BuffLostPostfix)));
		}



		public static IEnumerable<BuffIndex> EmpyreanAffixes()
		{
			List<EliteDef> elites = (List<EliteDef>)EmpyreanWhitelistField.GetValue(null);
			if (elites == null) return null;

			List<BuffIndex> buffs = new List<BuffIndex>();

			foreach (EliteDef eliteDef in EliteCatalog.eliteDefs)
			{
				if (eliteDef.IsAvailable() && elites.Contains(eliteDef))
				{
					EquipmentDef eliteEquipDef = eliteDef.eliteEquipmentDef;
					if (eliteEquipDef != null && eliteEquipDef.passiveBuffDef != null)
					{
						BuffIndex eliteBuff = eliteEquipDef.passiveBuffDef.buffIndex;
						if (!buffs.Contains(eliteBuff))
						{
							buffs.Add(eliteBuff);
						}
					}
				}
			}

			return buffs;
		}



		public static IEnumerable<CodeInstruction> BuffGainTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> instructionList = instructions.ToList();

			for (int i = 0; i < instructionList.Count; i++)
			{
				CodeInstruction instruction = instructionList[i];
				yield return instruction;

				if (instruction.ToString().Contains("IsAvailable"))
				{
					Logger.Info(identifier + " - IL [EmpyBuffGain] Match Found!");
					yield return new CodeInstruction(SysOpCodes.Pop);
					yield return new CodeInstruction(SysOpCodes.Ldc_I4, 0);
				}
			}
		}

		public static void BuffGainPostfix(object __instance)
		{
			CharacterBody body = (CharacterBody)BuffBehaviorGetBodyMethod.Invoke(__instance, null);
			if (body != null)
			{
				EmpyreanAspectProvider.Activated(body);
			}
		}

		public static IEnumerable<CodeInstruction> BuffLostTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> instructionList = instructions.ToList();

			for (int i = 0; i < instructionList.Count; i++)
			{
				CodeInstruction instruction = instructionList[i];
				yield return instruction;

				if (instruction.ToString().Contains("HasBuff"))
				{
					Logger.Info(identifier + " - IL [EmpyBuffLost] Match Found!");
					yield return new CodeInstruction(SysOpCodes.Pop);
					yield return new CodeInstruction(SysOpCodes.Ldc_I4, 0);
				}
			}
		}

		public static void BuffLostPostfix(object __instance)
		{
			CharacterBody body = (CharacterBody)BuffBehaviorGetBodyMethod.Invoke(__instance, null);
			if (body != null)
			{
				EmpyreanAspectProvider.Deactivated(body);
			}
		}
	}



	public class EmpyreanAspectProvider : IAspectProvider
	{
		public float StackCount()
		{
			return 1f;
		}

		public IEnumerable<BuffIndex> Aspects(CharacterBody body)
		{
			if (body.HasBuff(BuffDefOf.AffixEmpyrean))
			{
				return Starstorm.EmpyreanAffixes();
			}

			return null;
		}



		public static void Activated(CharacterBody body)
		{
			if (NetworkServer.active)
			{
				NetworkInstanceId netId = body.netId;

				if (!EffectHooks.IsBodyDestroyed(netId))
				{
					EliteBuffManager.ApplyBuffs(body, Starstorm.EmpyreanAffixes());
				}
			}
		}

		public static void Deactivated(CharacterBody body)
		{
			if (NetworkServer.active)
			{
				NetworkInstanceId netId = body.netId;

				if (!EffectHooks.IsBodyDestroyed(netId))
				{
					EliteBuffManager.CheckSustains(body, Starstorm.EmpyreanAffixes());
				}
			}
		}
	}
}
