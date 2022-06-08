using System;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using RoR2;
using UnityEngine;

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
		private static Type ComponentPassiveAffixBlueType;
		private static Type TweakEliteVoidType;
		private static Type TweakAffixHauntedType;

		private static FieldInfo AffixBlueEnabledField;
		private static FieldInfo AffixBlueRemoveShieldField;
		private static FieldInfo AffixBluePassiveField;
		private static FieldInfo AffixBlueOnHitReworkField;
		private static FieldInfo AffixBlueDamageCoeffField;
		private static FieldInfo AffixBlueScatterBombField;
		private static FieldInfo AffixHauntedEnabledField;
		private static FieldInfo AffixPoisonEnabledField;
		private static FieldInfo EliteVoidEnabledField;
		private static FieldInfo EliteVoidDamageBonusField;
		private static FieldInfo AffixHauntedReplaceOnHitField;

		private static MethodInfo OnHitAllMethod;

		internal static bool affixBlueEnabled = false;
		internal static bool affixBlueRemoveShield = false;
		internal static bool affixBluePassive = false;
		internal static bool affixBlueOnHit = false;
		internal static float affixBlueDamage = 0f;
		internal static bool affixBlueScatter = false;
		internal static bool affixHauntedEnabled = false;
		internal static bool affixPoisonEnabled = false;
		internal static bool eliteVoidEnabled = false;
		internal static float eliteVoidDamage = 0f;
		internal static bool affixHauntedOnHit = false;

		internal static int affixBlueDamageHook = 0;
		internal static bool overrideAffixBlue = false;



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

			ReadValues();
			HookMethods();
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

			type = Type.GetType("EliteReworks.Tweaks.T1.Components.AffixBluePassiveLightning, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				ComponentPassiveAffixBlueType = type;

				AffixBlueScatterBombField = type.GetField("scatterBombs", Flags);
				if (AffixBlueScatterBombField == null) Logger.Warn("[EliteReworksCompat] - Could Not Find Field : AffixBluePassiveLightning.scatterBombs");
			}
			else
			{
				Logger.Warn("[EliteReworksCompat] - Could Not Find Type : EliteReworks.Tweaks.T1.Components.AffixBluePassiveLightning");
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

			type = Type.GetType("EliteReworks.Tweaks.T2.AffixHaunted, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				TweakAffixHauntedType = type;

				AffixHauntedReplaceOnHitField = type.GetField("replaceOnHitEffect", Flags);
				if (AffixHauntedReplaceOnHitField == null) Logger.Warn("[EliteReworksCompat] - Could Not Find Field : AffixHaunted.replaceOnHitEffect");
			}
			else
			{
				Logger.Warn("[EliteReworksCompat] - Could Not Find Type : EliteReworks.Tweaks.T2.AffixHaunted");
			}

			type = Type.GetType("EliteReworks.SharedHooks.OnHitAll, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				OnHitAllMethod = type.GetMethod("TriggerOnHitAllEffects", Flags);
				if (OnHitAllMethod == null) Logger.Warn("[EliteReworksCompat] - Could Not Find Method : OnHitAll.TriggerOnHitAllEffects");
			}
			else
			{
				Logger.Warn("[EliteReworksCompat] - Could Not Find Type : EliteReworks.SharedHooks.OnHitAll");
			}
		}

		private static void ReadValues()
		{
			if (AffixBlueEnabledField != null) affixBlueEnabled = (bool)AffixBlueEnabledField.GetValue(PluginType);

			if (AffixBlueRemoveShieldField != null) affixBlueRemoveShield = (bool)AffixBlueRemoveShieldField.GetValue(PluginType);

			if (AffixHauntedEnabledField != null) affixHauntedEnabled = (bool)AffixHauntedEnabledField.GetValue(PluginType);

			if (AffixPoisonEnabledField != null) affixPoisonEnabled = (bool)AffixPoisonEnabledField.GetValue(PluginType);

			if (EliteVoidEnabledField != null) eliteVoidEnabled = (bool)EliteVoidEnabledField.GetValue(PluginType);

			if (AffixBluePassiveField != null) affixBluePassive = (bool)AffixBluePassiveField.GetValue(TweakAffixBlueType);

			if (AffixBlueOnHitReworkField != null) affixBlueOnHit = (bool)AffixBlueOnHitReworkField.GetValue(TweakAffixBlueType);

			if (AffixBlueDamageCoeffField != null) affixBlueDamage = (float)AffixBlueDamageCoeffField.GetValue(TweakAffixBlueType);

			if (AffixBlueScatterBombField != null) affixBlueScatter = (bool)AffixBlueScatterBombField.GetValue(ComponentPassiveAffixBlueType);

			if (EliteVoidDamageBonusField != null) eliteVoidDamage = (float)EliteVoidDamageBonusField.GetValue(TweakEliteVoidType);

			if (AffixHauntedReplaceOnHitField != null) affixHauntedOnHit = (bool)AffixHauntedReplaceOnHitField.GetValue(TweakAffixHauntedType);
		}

		private static void HookMethods()
		{
			if (AffixBlueDamageCoeffField != null)
			{
				if (OnHitAllMethod != null)
				{
					HookEndpointManager.Modify(OnHitAllMethod, (ILContext.Manipulator)OverrideAffixBlueDamage);

					if (affixBlueDamageHook != 2)
					{
						HookEndpointManager.Unmodify(OnHitAllMethod, (ILContext.Manipulator)OverrideAffixBlueDamage);

						Debug.LogWarning("[EliteReworksCompat] - Failed to successfully hook OnHitAll.TriggerOnHitAllEffects, Unhooking!");
					}
					else
					{
						overrideAffixBlue = true;
					}
				}
			}
		}



		private static void OverrideAffixBlueDamage(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			int bodyIndex = -1;

			bool found = c.TryGotoNext(
				x => x.MatchLdarg(2),
				x => x.MatchLdfld<DamageInfo>("attacker"),
				x => x.MatchCallOrCallvirt<GameObject>("GetComponent"),
				x => x.MatchStloc(out bodyIndex)
			);

			if (bodyIndex != -1)
			{
				found = c.TryGotoNext(
					x => x.MatchLdsfld(AffixBlueDamageCoeffField)
				);

				if (found)
				{
					affixBlueDamageHook++;

					Logger.Info("[EliteReworksCompat] - AffixBlueDamageCoeffField Index : " + c.Index);

					c.Index += 1;

					c.Emit(OpCodes.Ldloc, bodyIndex);
					c.EmitDelegate<Func<float, CharacterBody, float>>((mult, body) =>
					{
						if (Configuration.AspectBlueEliteReworksDamage.Value && Configuration.AspectBlueBaseDamage.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(body, RoR2Content.Buffs.AffixBlue);

							mult = Configuration.AspectBlueBaseDamage.Value + Configuration.AspectBlueStackDamage.Value * (count - 1f);
							if (body.teamComponent.teamIndex != TeamIndex.Player) mult *= Configuration.AspectBlueMonsterDamageMult.Value;
						}

						return mult;
					});
				}
				else
				{
					Logger.Warn("[EliteReworksCompat] - OverrideAffixBlueDamage - Failed to find AffixBlueDamageCoefficientField");
				}

				found = c.TryGotoNext(
					x => x.MatchLdsfld(AffixBlueDamageCoeffField)
				);

				if (found)
				{
					affixBlueDamageHook++;

					Logger.Info("[EliteReworksCompat] - AffixBlueDamageCoeffField Index : " + c.Index);

					c.Index += 1;

					c.Emit(OpCodes.Ldloc, bodyIndex);
					c.EmitDelegate<Func<float, CharacterBody, float>>((mult, body) =>
					{
						if (Configuration.AspectBlueEliteReworksDamage.Value && Configuration.AspectBlueBaseDamage.Value > 0f)
						{
							float count = Catalog.GetStackMagnitude(body, RoR2Content.Buffs.AffixBlue);

							mult = Configuration.AspectBlueBaseDamage.Value + Configuration.AspectBlueStackDamage.Value * (count - 1f);
							if (body.teamComponent.teamIndex != TeamIndex.Player) mult *= Configuration.AspectBlueMonsterDamageMult.Value;
						}

						return mult;
					});
				}
				else
				{
					Logger.Warn("[EliteReworksCompat] - OverrideAffixBlueDamage - Failed to find AffixBlueDamageCoefficientField");
				}
			}
			else
			{
				Logger.Warn("[EliteReworksCompat] - OverrideAffixBlueDamage - Failed to find CharacterBody Local Variable Index");
			}
		}
	}
}
