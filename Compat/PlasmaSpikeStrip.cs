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
	internal static class PlasmaSpikeStrip
	{
		private static readonly string GUID = "com.plasmacore.PlasmaCoreSpikestripContent";
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static Type RageEliteType;
		//private static Type CloakEliteType;

		private static MethodInfo RageStatMethod;
		private static MethodInfo CloakOnHitMethod;

		private static FieldInfo RageScriptObjectField;
		private static FieldInfo BuffArrayField;

		internal static bool rageStatHook = false;
		internal static bool cloakHook = false;



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
				Logger.Warn("[PlasmaSpikeStripCompat] - Could Not Find PlasmaSpikeStrip Assembly");
			}

			HookMethods();
		}



		private static void GatherInfos()
		{
			Type type;

			type = Type.GetType("PlasmaCoreSpikestripContent.Content.Elites.RagingElite, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				RageEliteType = type;

				RageStatMethod = type.GetMethod("RecalculateStatsAPI_GetStatCoefficients", Flags);
				if (RageStatMethod == null) Logger.Warn("[PlasmaSpikeStripCompat] - Could Not Find Method : RagingElite.RecalculateStatsAPI_GetStatCoefficients");

				RageScriptObjectField = type.GetField("scriptableObject", Flags);
				if (RageScriptObjectField == null) Logger.Warn("[PlasmaSpikeStripCompat] - Could Not Find Field : RagingElite.scriptableObject");
			}
			else
			{
				Logger.Warn("[PlasmaSpikeStripCompat] - Could Not Find Type : PlasmaCoreSpikestripContent.Content.Elites.RagingElite");
			}

			type = Type.GetType("PlasmaCoreSpikestripContent.Content.Elites.CloakedElite, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				//CloakEliteType = type;

				CloakOnHitMethod = type.GetMethod("GlobalEventManager_OnHitEnemy", Flags);
				if (CloakOnHitMethod == null) Logger.Warn("[PlasmaSpikeStripCompat] - Could Not Find Method : CloakedElite.GlobalEventManager_OnHitEnemy");
			}
			else
			{
				Logger.Warn("[PlasmaSpikeStripCompat] - Could Not Find Type : PlasmaCoreSpikestripContent.Content.Elites.CloakedElite");
			}

			type = Type.GetType("PlasmaCoreSpikestripContent.Core.SSEliteBaseSO, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				BuffArrayField = type.GetField("MiscBuffs", Flags);
				if (BuffArrayField == null) Logger.Warn("[PlasmaSpikeStripCompat] - Could Not Find Field : SSEliteBaseSO.MiscBuffs");
			}
			else
			{
				Logger.Warn("[PlasmaSpikeStripCompat] - Could Not Find Type : PlasmaCoreSpikestripContent.Core.SSEliteBaseSO");
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
					Logger.Warn("[PlasmaSpikeStripCompat] - DisableStatHook Failed! - Removing Hook");
				}
			}
			
			if (CloakOnHitMethod != null)
			{
				HookEndpointManager.Modify(CloakOnHitMethod, (ILContext.Manipulator)DisableCloakHook);
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

					Logger.Info("[PlasmaSpikeStripCompat] - DisableStatHook : DisableStat[" + counter + "] (" + nextInst+") (" + type + ") at cursor index : " + statArgsIndex);
				}
				else
				{
					Logger.Warn("[PlasmaSpikeStripCompat] - DisableStatHook : Could not find stat adjustment!");
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

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldc_R4, 999999f);

				cloakHook = true;
			}
			else
			{
				Logger.Warn("[PlasmaSpikeStripCompat] - DisableCloakHook Failed!");
			}
		}



		internal static BuffIndex GetRageBuffWardBuffIndex()
		{
			if (RageScriptObjectField != null && BuffArrayField != null)
			{
				BuffDef[] buffs = (BuffDef[])BuffArrayField.GetValue(RageScriptObjectField.GetValue(RageEliteType));
				BuffDef buff = buffs[0];
				if (buff)
				{
					BuffIndex index = buff.buffIndex;

					Logger.Info("[PlasmaSpikeStripCompat] - GetRageBuffWardBuffIndex : " + index);
					return index;
				}
			}

			Logger.Warn("[PlasmaSpikeStripCompat] - GetRageBuffWardBuffIndex : Could not find MiscBuffs[0]");
			return BuffIndex.None;
		}
	}
}
