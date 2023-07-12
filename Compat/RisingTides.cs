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

		internal static FieldInfo BarrierDamageResistance;
		internal static FieldInfo BarrierHealthReduction;

		internal static FieldInfo BlackHoleDamageScale;

		internal static FieldInfo NightBlindRange;



		private static MethodInfo NightSpeedStatMethod;

		internal static bool nightSpeedStatHook = false;



		internal static FieldInfo GetField(string typeName, string fieldName)
		{
			Type type = Type.GetType(typeName + ", " + PluginAssembly.FullName, false);
			if (type != null)
			{
				FieldInfo fieldInfo = type.GetField(fieldName, Flags);
				if (fieldInfo == null) Logger.Warn(identifier + " - Could Not Find Field : " + typeName + "." + fieldName);
				return fieldInfo;
			}
			else
			{
				Logger.Warn(identifier + " - Could Not Find Type : " + typeName);
				return null;
			}
		}
		/*
		internal static FieldInfo GetField(string typeName, string nestedTypeName, string fieldName)
		{
			Type type = Type.GetType(typeName + ", " + PluginAssembly.FullName, false);
			if (type != null)
			{
				type = type.GetNestedType(nestedTypeName, Flags);
				if (type != null)
				{
					FieldInfo fieldInfo = type.GetField(fieldName, Flags);
					if (fieldInfo == null) Logger.Warn(identifier + " - Could Not Find Field : " + nestedTypeName + "." + fieldName);
					return fieldInfo;
				}
				else
				{
					Logger.Warn(identifier + " - Could Not Find NestedType : " + typeName + "." + nestedTypeName);
					return null;
				}
			}
			else
			{
				Logger.Warn(identifier + " - Could Not Find Type : " + typeName);
				return null;
			}
		}
		*/
		internal static float GetConfigValue(FieldInfo fieldInfo, float defaultValue)
		{
			object fieldValue = fieldInfo.GetValue(null);
			Type type = fieldValue.GetType();
			PropertyInfo propInfo = type.GetProperty("Value", Flags);
			if (propInfo != null)
			{
				return (float)propInfo.GetValue(fieldValue);
			}
			else 
			{
				Logger.Warn(identifier + " - Could Not Get Value For : " + fieldInfo.Name);
				return defaultValue;
			}
		}



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



			BarrierDamageResistance = GetField("RisingTides.Buffs.AffixBarrier", "barrierDamageResistance");
			BarrierHealthReduction = GetField("RisingTides.Buffs.AffixBarrier", "healthReduction");

			BlackHoleDamageScale = GetField("RisingTides.Buffs.AffixBlackHole", "markBaseDamage");

			NightBlindRange = GetField("RisingTides.Buffs.NightReducedVision", "visionDistance");
		}

		private static void HookMethods()
		{
			if (NightSpeedStatMethod != null)
			{
				HookEndpointManager.Modify(NightSpeedStatMethod, (ILContext.Manipulator)DisableStatHook);
			}
		}



		private static void DisableStatHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchStloc(0)
			);

			if (found)
			{
				nightSpeedStatHook = true;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldc_I4, 0);
			}
			else
			{
				Logger.Warn(identifier + " - DisableStatHook Failed");
			}
		}
	}
}
