using BepInEx;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

using static TPDespair.ZetAspects.ReflectionUtility;

namespace TPDespair.ZetAspects.Compat
{
	internal static class AspectAbilities
	{
		internal static Reflector Reflector;
		private static readonly string GUID = "com.themysticsword.aspectabilities";
		private static readonly string identifier = "[AspectAbilitiesCompat]";

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;



		internal static void LateSetup()
		{
			Reflector = new Reflector(GUID, identifier);

			if (!Reflector.FindPluginAssembly()) return;

			Harmony harmony = ZetAspectsPlugin.harmony;

			Type type = Reflector.GetType("AspectAbilities.AspectAbilitiesPlugin");
			if (type != null)
			{
				Type[] par = new Type[] { typeof(EquipmentDef) };
				MethodInfo findAspectAbilityMethodInfo = type.GetMethod("FindAspectAbility", Flags, null, par, null);
				if (findAspectAbilityMethodInfo != null)
				{
					harmony.Patch(findAspectAbilityMethodInfo, prefix: new HarmonyMethod(typeof(AspectAbilities), nameof(FindAbilityPrefix)));
				}
			}
		}


		/*
		private static void ExampleHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			Type[] par = new Type[] { typeof(GameObject), typeof(Vector3), doTheRestOfThese };
			BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			MethodInfo methodInfo = typeof(EffectManager).GetMethod("SimpleImpactEffect", Flags, null, par, null);

			bool found = c.TryGotoNext(
				x => x.MatchCallOrCallvirt(methodInfo)
			);

			if (found)
			{
				c.Index += 1;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldc_I4, 0);
			}
			else
			{
				Logger.Warn("ExampleHook Failed");
			}
		}
		*/


		public static bool FindAbilityPrefix(EquipmentDef equipmentDef, ref object __result)
		{
			if (equipmentDef == null)
			{
				__result = null;

				return false;
			}

			return true;
		}
	}
}
