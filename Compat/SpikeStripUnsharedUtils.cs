using System;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using RoR2;

namespace TPDespair.ZetAspects.Compat
{
	internal static class SpikeStripUnsharedUtils
	{
		private static readonly string GUID = "_com.groovesalad.GrooveUnsharedUtils";
		private static readonly string identifier = "[SpikeStripUnsharedUtilsCompat]";
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static MethodInfo CloakOnSpawnMethod;



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
				Logger.Warn(identifier + " - Could Not Find GrooveUnsharedUtils Assembly");
			}

			HookMethods();
		}



		private static void GatherInfos()
		{
			Type type;

			type = Plugin.GetType();
			if (type != null)
			{
				CloakOnSpawnMethod = type.GetMethod("CharacterBody_onBodyStartGlobal", Flags);
				if (CloakOnSpawnMethod == null) Logger.Warn(identifier + " - Could Not Find Method : Main.CharacterBody_onBodyStartGlobal");
			}
		}

		private static void HookMethods()
		{
			if (CloakOnSpawnMethod != null)
			{
				HookEndpointManager.Modify(CloakOnSpawnMethod, (ILContext.Manipulator)DisableCloakHook);
			}
		}

		private static void DisableCloakHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchLdloc(0),
				x => x.MatchBrfalse(out _)
			);

			if (found)
			{
				c.Index += 1;

				c.EmitDelegate<Func<bool, bool>>((flag) =>
				{
					if (!Configuration.AspectVeiledCloakOnSpawn.Value) return false;

					return flag;
				});
			}
			else
			{
				Logger.Warn(identifier + " - DisableCloakHook Failed!");
			}
		}
	}
}
