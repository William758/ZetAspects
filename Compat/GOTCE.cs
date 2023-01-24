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
	internal static class GOTCE
	{
		private static readonly string GUID = "com.TheBestAssociatedLargelyLudicrousSillyheadGroup.GOTCE";
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static MethodInfo BackupStatMethod;

		internal static bool backupStatHook = false;
		internal static bool chargeOverride = true; // added charges from GOTCE seem to not work



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
				Logger.Warn("[GotceCompat] - Could Not Find GOTCE Assembly");
			}

			HookMethods();
		}



		private static void GatherInfos()
		{
			Type type;

			type = Type.GetType("GOTCE.Equipment.EliteEquipment.BackupMag, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				BackupStatMethod = type.GetMethod("Backup", Flags);
				if (BackupStatMethod == null) Logger.Warn("[GotceCompat] - Could Not Find Method : BackupMag.Backup");
			}
			else
			{
				Logger.Warn("[GotceCompat] - Could Not Find Type : GOTCE.Equipment.EliteEquipment.BackupMag");
			}
		}

		private static void HookMethods()
		{
			if (BackupStatMethod != null)
			{
				HookEndpointManager.Modify(BackupStatMethod, (ILContext.Manipulator)DisableStatHook);

				if (!backupStatHook)
				{
					HookEndpointManager.Unmodify(BackupStatMethod, (ILContext.Manipulator)DisableStatHook);
					Logger.Warn("[GotceCompat] - DisableStatHook Failed! - Removing Hook");
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

					Logger.Info("[GotceCompat] - DisableStatHook : DisableStat[" + counter + "] (" + nextInst + ") (" + type + ") at cursor index : " + statArgsIndex);
				}
				else
				{
					Logger.Warn("[GotceCompat] - DisableStatHook : Could not find stat adjustment!");
				}
			}

			if (counter > 0 && !findFail)
			{
				backupStatHook = true;
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
