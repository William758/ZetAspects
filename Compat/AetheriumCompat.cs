using BepInEx;
using BepInEx.Bootstrap;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Reflection;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	internal static class AetheriumCompat
	{
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static MethodInfo ApplyBleedMethod;

		internal static bool bleedHook = false;



		internal static void Init()
		{
			if (!Configuration.AetheriumHooks.Value) return;

			Plugin = Chainloader.PluginInfos["com.KomradeSpectre.Aetherium"].Instance;
			PluginAssembly = Assembly.GetAssembly(Plugin.GetType());

			if (PluginAssembly != null)
			{
				FindStuff();
			}
			else
			{
				Debug.LogWarning("ZetAspect [AETH] - Could Not Find Aetherium Assembly");
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
				if (ApplyBleedMethod == null) Debug.LogWarning("ZetAspect [LIT] - Could Not Find Method : AffixSanguine.BleedOnHit");
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
