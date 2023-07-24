using System;
using System.Reflection;
using UnityEngine;
using BepInEx;
using BepInEx.Bootstrap;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using RoR2;

namespace TPDespair.ZetAspects.Compat
{
	internal static class NemSpikeStrip
	{
		private static readonly string GUID = "prodzpod.NemesisSpikestrip";
		private static readonly string identifier = "[NemSpikeStripCompat]";
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static FieldInfo PlatedEnabledField;
		internal static FieldInfo PlatedHealthField;

		private static FieldInfo VeiledEnabledField;
		internal static FieldInfo VeiledHitToShowField;
		internal static FieldInfo VeiledCooldownBuffField;

		private static FieldInfo WarpedEnabledField;
		internal static FieldInfo WarpedDurationField;

		internal static bool PlatedEnabled = false;
		internal static bool VeiledEnabled = false;
		internal static bool WarpedEnabled = false;



		internal static bool GetConfigValue(FieldInfo fieldInfo, bool defaultValue)
		{
			object fieldValue = fieldInfo.GetValue(null);
			Type type = fieldValue.GetType();
			PropertyInfo propInfo = type.GetProperty("Value", Flags);
			if (propInfo != null)
			{
				return (bool)propInfo.GetValue(fieldValue);
			}
			else
			{
				Logger.Warn(identifier + " - Could Not Get Value For : " + fieldInfo.Name);
				return defaultValue;
			}
		}

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



		internal static void LateSetup()
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
				Logger.Warn(identifier + " - Could Not Find NemSpikeStrip Assembly");
			}

			HookMethods();
		}



		private static void GatherInfos()
		{
			Type type;

			type = Type.GetType("NemesisSpikestrip.Changes.Plated, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				PlatedEnabledField = type.GetField("enabled", Flags);
				if (PlatedEnabledField == null) Logger.Warn(identifier + " - Could Not Find Field : Plated.enabled");

				PlatedHealthField = type.GetField("MaxHPPenalty", Flags);
				if (PlatedHealthField == null) Logger.Warn(identifier + " - Could Not Find Field : Plated.MaxHPPenalty");
			}
			else
			{
				Logger.Warn(identifier + " - Could Not Find Type : NemesisSpikestrip.Changes.Veiled");
			}

			type = Type.GetType("NemesisSpikestrip.Changes.Veiled, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				VeiledEnabledField = type.GetField("enabled", Flags);
				if (VeiledEnabledField == null) Logger.Warn(identifier + " - Could Not Find Field : Veiled.enabled");

				VeiledHitToShowField = type.GetField("HitToShow", Flags);
				if (VeiledHitToShowField == null) Logger.Warn(identifier + " - Could Not Find Field : Veiled.HitToShow");

				VeiledCooldownBuffField = type.GetField("Cooldown", Flags);
				if (VeiledCooldownBuffField == null) Logger.Warn(identifier + " - Could Not Find Field : Veiled.Cooldown");
			}
			else
			{
				Logger.Warn(identifier + " - Could Not Find Type : NemesisSpikestrip.Changes.Veiled");
			}

			type = Type.GetType("NemesisSpikestrip.Changes.Warped, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				WarpedEnabledField = type.GetField("enabled", Flags);
				if (WarpedEnabledField == null) Logger.Warn(identifier + " - Could Not Find Field : Warped.enabled");

				WarpedDurationField = type.GetField("Duration", Flags);
				if (WarpedDurationField == null) Logger.Warn(identifier + " - Could Not Find Field : Warped.HitToShow");
			}
			else
			{
				Logger.Warn(identifier + " - Could Not Find Type : NemesisSpikestrip.Changes.Veiled");
			}
		}

		private static void HookMethods()
		{
			PlatedEnabled = (bool)PlatedEnabledField.GetValue(null);

			VeiledEnabled = (bool)VeiledEnabledField.GetValue(null);

			WarpedEnabled = (bool)WarpedEnabledField.GetValue(null);



			Catalog.veiledCooldown = GetVeiledCooldownBuffIndex();
		}



		private static BuffIndex GetVeiledCooldownBuffIndex()
		{
			if (VeiledCooldownBuffField != null)
			{
				BuffDef buffDef = (BuffDef)VeiledCooldownBuffField.GetValue(null);
				if (buffDef)
				{
					BuffIndex buffIndex = buffDef.buffIndex;
					Logger.Info(identifier + " - veiledCooldownBuff : " + buffIndex);
					return buffIndex;
				}
			}

			Logger.Warn(identifier + " - GetVeiledCooldownBuffIndex Failed!");
			return BuffIndex.None;
		}
	}
}
