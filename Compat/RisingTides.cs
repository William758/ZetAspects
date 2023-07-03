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
	internal static class RisingTides
	{
		private static readonly string GUID = "com.themysticsword.risingtides";
		private static readonly string identifier = "[RisingTidesCompat]";
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static MethodInfo NightSpeedStatMethod;

		internal static bool nightSpeedStatHook = false;



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
				Logger.Warn(identifier + " - Could Not Find Assembly");
			}

			HookMethods();
		}



		private static void GatherInfos()
		{
			Type type;

			type = Type.GetType("RisingTides.Buffs.NightSpeedBoost, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				NightSpeedStatMethod = type.GetMethod("RecalculateStatsAPI_GetStatCoefficients", Flags);
				if (NightSpeedStatMethod == null) Logger.Warn(identifier + " - Could Not Find Method : NightSpeedBoost.RecalculateStatsAPI_GetStatCoefficients");
			}
			else
			{
				Logger.Warn(identifier + " - Could Not Find Type : RisingTides.Buffs.NightSpeedBoost");
			}
		}

		private static void HookMethods()
		{
			if (NightSpeedStatMethod != null)
			{
				HookEndpointManager.Modify(NightSpeedStatMethod, (ILContext.Manipulator)DisableStatHook);

				if (!nightSpeedStatHook)
				{
					HookEndpointManager.Unmodify(NightSpeedStatMethod, (ILContext.Manipulator)DisableStatHook);
					Logger.Warn(identifier + " - DisableStatHook Failed! - Removing Hook");
				}
			}
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

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_R4, cancleValue);

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
				nightSpeedStatHook = true;
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
	}
}
