using BepInEx;
using BepInEx.Bootstrap;
using System;
using System.Reflection;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	internal static class EliteReworksCompat
	{
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static Type PluginType;

		private static FieldInfo AffixBlueEnabledField;
		private static FieldInfo AffixBlueShieldField;
		private static FieldInfo AffixHauntedEnabledField;
		private static FieldInfo AffixPoisonEnabledField;

		internal static bool affixBlueEnabled = false;
		internal static bool affixBlueShield = false;
		internal static bool affixHauntedEnabled = false;
		internal static bool affixPoisonEnabled = false;

		internal static void LateSetup()
		{
			Plugin = Chainloader.PluginInfos["com.Moffein.EliteReworks"].Instance;
			PluginAssembly = Assembly.GetAssembly(Plugin.GetType());

			if (PluginAssembly != null)
			{
				FindStuff();
			}
			else
			{
				Debug.LogWarning("ZetAspect - Could Not Find EliteReworks Assembly");
			}

			if (AffixBlueEnabledField != null) affixBlueEnabled = (bool)AffixBlueEnabledField.GetValue(PluginType);

			if (AffixBlueShieldField != null) affixBlueShield = (bool)AffixBlueShieldField.GetValue(PluginType);

			if (AffixHauntedEnabledField != null) affixHauntedEnabled = (bool)AffixHauntedEnabledField.GetValue(PluginType);

			if (AffixPoisonEnabledField != null) affixPoisonEnabled = (bool)AffixPoisonEnabledField.GetValue(PluginType);
		}

		private static void FindStuff()
		{
			Type type;

			type = Plugin.GetType();
			if (type != null)
			{
				PluginType = type;

				AffixBlueEnabledField = type.GetField("affixBlueEnabled", Flags);
				if (AffixBlueEnabledField == null) Debug.LogWarning("ZetAspect [ER] - Could Not Find Field : EliteReworksPlugin.affixBlueEnabled");

				AffixBlueShieldField = type.GetField("affixBlueRemoveShield", Flags);
				if (AffixBlueShieldField == null) Debug.LogWarning("ZetAspect [ER] - Could Not Find Field : EliteReworksPlugin.affixBlueRemoveShield");

				AffixHauntedEnabledField = type.GetField("affixHauntedEnabled", Flags);
				if (AffixHauntedEnabledField == null) Debug.LogWarning("ZetAspect [ER] - Could Not Find Field : EliteReworksPlugin.affixHauntedEnabled");

				AffixPoisonEnabledField = type.GetField("affixPoisonEnabled", Flags);
				if (AffixPoisonEnabledField == null) Debug.LogWarning("ZetAspect [ER] - Could Not Find Field : EliteReworksPlugin.affixPoisonEnabled");
			}
			else
			{
				Debug.LogWarning("ZetAspect [ER] - Could Not Find Type : EliteReworksPlugin");
			}
		}
	}
}
