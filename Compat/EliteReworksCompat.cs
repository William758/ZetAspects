using System;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;

namespace TPDespair.ZetAspects.Compat
{
	internal static class EliteReworks
	{
		private static readonly string GUID = "com.Moffein.EliteReworks";
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static Type PluginType;
		private static Type TweakAffixBlueType;
		private static Type TweakEliteVoidType;

		private static FieldInfo AffixBlueEnabledField;
		private static FieldInfo AffixBlueRemoveShieldField;
		private static FieldInfo AffixBluePassiveField;
		private static FieldInfo AffixBlueOnHitReworkField;
		private static FieldInfo AffixBlueDamageCoeffField;
		private static FieldInfo AffixHauntedEnabledField;
		private static FieldInfo AffixPoisonEnabledField;
		private static FieldInfo EliteVoidEnabledField;
		private static FieldInfo EliteVoidDamageBonusField;

		internal static bool affixBlueEnabled = false;
		internal static bool affixBlueRemoveShield = false;
		internal static bool affixBluePassive = false;
		internal static bool affixBlueOnHit = false;
		internal static float affixBlueDamage = 0f;
		internal static bool affixHauntedEnabled = false;
		internal static bool affixPoisonEnabled = false;
		internal static bool eliteVoidEnabled = false;
		internal static float eliteVoidDamage = 0f;



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
				Logger.Warn("[EliteReworksCompat] - Could Not Find EliteReworks Assembly");
			}

			if (AffixBlueEnabledField != null) affixBlueEnabled = (bool)AffixBlueEnabledField.GetValue(PluginType);

			if (AffixBlueRemoveShieldField != null) affixBlueRemoveShield = (bool)AffixBlueRemoveShieldField.GetValue(PluginType);

			if (AffixHauntedEnabledField != null) affixHauntedEnabled = (bool)AffixHauntedEnabledField.GetValue(PluginType);

			if (AffixPoisonEnabledField != null) affixPoisonEnabled = (bool)AffixPoisonEnabledField.GetValue(PluginType);

			if (EliteVoidEnabledField != null) eliteVoidEnabled = (bool)EliteVoidEnabledField.GetValue(PluginType);

			if (AffixBluePassiveField != null) affixBluePassive = (bool)AffixBluePassiveField.GetValue(TweakAffixBlueType);

			if (AffixBlueOnHitReworkField != null) affixBlueOnHit = (bool)AffixBlueOnHitReworkField.GetValue(TweakAffixBlueType);

			if (AffixBlueDamageCoeffField != null) affixBlueDamage = (float)AffixBlueDamageCoeffField.GetValue(TweakAffixBlueType);

			if (EliteVoidDamageBonusField != null) eliteVoidDamage = (float)EliteVoidDamageBonusField.GetValue(TweakEliteVoidType);
		}



		private static void GatherInfos()
		{
			Type type;

			type = Plugin.GetType();
			if (type != null)
			{
				PluginType = type;

				AffixBlueEnabledField = type.GetField("affixBlueEnabled", Flags);
				if (AffixBlueEnabledField == null) Logger.Warn("[EliteReworksCompat] - Could Not Find Field : EliteReworksPlugin.affixBlueEnabled");

				AffixBlueRemoveShieldField = type.GetField("affixBlueRemoveShield", Flags);
				if (AffixBlueRemoveShieldField == null) Logger.Warn("[EliteReworksCompat] - Could Not Find Field : EliteReworksPlugin.affixBlueRemoveShield");

				AffixHauntedEnabledField = type.GetField("affixHauntedEnabled", Flags);
				if (AffixHauntedEnabledField == null) Logger.Warn("[EliteReworksCompat] - Could Not Find Field : EliteReworksPlugin.affixHauntedEnabled");

				AffixPoisonEnabledField = type.GetField("affixPoisonEnabled", Flags);
				if (AffixPoisonEnabledField == null) Logger.Warn("[EliteReworksCompat] - Could Not Find Field : EliteReworksPlugin.affixPoisonEnabled");

				EliteVoidEnabledField = type.GetField("eliteVoidEnabled", Flags);
				if (EliteVoidEnabledField == null) Logger.Warn("[EliteReworksCompat] - Could Not Find Field : EliteReworksPlugin.eliteVoidEnabled");
			}
			else
			{
				Logger.Warn("[EliteReworksCompat] - Could Not Find Type : EliteReworksPlugin");
			}

			type = Type.GetType("EliteReworks.Tweaks.T1.AffixBlue, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				TweakAffixBlueType = type;

				AffixBluePassiveField = type.GetField("enablePassiveLightning", Flags);
				if (AffixBluePassiveField == null) Logger.Warn("[EliteReworksCompat] - Could Not Find Field : AffixBlue.enablePassiveLightning");

				AffixBlueOnHitReworkField = type.GetField("enableOnHitRework", Flags);
				if (AffixBlueOnHitReworkField == null) Logger.Warn("[EliteReworksCompat] - Could Not Find Field : AffixBlue.enableOnHitRework");

				AffixBlueDamageCoeffField = type.GetField("lightningDamageCoefficient", Flags);
				if (AffixBlueDamageCoeffField == null) Logger.Warn("[EliteReworksCompat] - Could Not Find Field : AffixBlue.lightningDamageCoefficient");
			}
			else
			{
				Logger.Warn("[EliteReworksCompat] - Could Not Find Type : EliteReworks.Tweaks.T1.AffixBlue");
			}

			type = Type.GetType("EliteReworks.Tweaks.DLC1.EliteVoid, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				TweakEliteVoidType = type;

				EliteVoidDamageBonusField = type.GetField("damageBonus", Flags);
				if (EliteVoidDamageBonusField == null) Logger.Warn("[EliteReworksCompat] - Could Not Find Field : EliteVoid.damageBonus");
			}
			else
			{
				Logger.Warn("[EliteReworksCompat] - Could Not Find Type : EliteReworks.Tweaks.DLC1.EliteVoid");
			}
		}
	}
}
