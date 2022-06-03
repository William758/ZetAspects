using System;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;

namespace TPDespair.ZetAspects.Compat
{
	internal static class Aetherium
	{
		private static readonly string GUID = "com.KomradeSpectre.Aetherium";
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static MethodInfo ApplyBleedMethod;

		internal static bool bleedHook = false;



		internal static void Init()
		{
			if (!Chainloader.PluginInfos.ContainsKey(GUID)) return;

			Plugin = Chainloader.PluginInfos[GUID].Instance;
			PluginAssembly = Assembly.GetAssembly(Plugin.GetType());

			if (PluginAssembly != null)
			{
				GatherInfos();
			}
			else
			{
				Logger.Warn("[AetheriumCompat] - Could Not Find Aetherium Assembly");
			}

			HookMethods();
		}



		private static void GatherInfos()
		{
			Type type;

			type = Type.GetType("Aetherium.Equipment.EliteEquipment.AffixSanguine, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				ApplyBleedMethod = type.GetMethod("BleedOnHit", Flags);
				if (ApplyBleedMethod == null) Logger.Warn("[AetheriumCompat] - Could Not Find Method : AffixSanguine.BleedOnHit");
			}
			else
			{
				Logger.Warn("[AetheriumCompat] - Could Not Find Type : Aetherium.Equipment.EliteEquipment.AffixSanguine");
			}
		}

		private static void HookMethods()
		{
			if (ApplyBleedMethod != null)
			{
				HookEndpointManager.Modify(ApplyBleedMethod, (ILContext.Manipulator)DisableBleedHook);
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
				Logger.Warn("[AetheriumCompat] - DisableBleedHook Failed");
			}
		}
	}
}
