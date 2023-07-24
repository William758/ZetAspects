using System;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using RoR2;
using UnityEngine;

namespace TPDespair.ZetAspects.Compat
{
	internal static class GrooveSpikeStrip
	{
		private static readonly string GUID = "com.groovesalad.GrooveSaladSpikestripContent";
		private static readonly string identifier = "[GrooveSpikeStripCompat]";
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static FieldInfo GravityBuffField;
		private static FieldInfo LiftForceField;

		private static FieldInfo StifleBuffField;
		private static MethodInfo TakeDamageMethod;
		private static MethodInfo MaxPlatingMethod;

		private static FieldInfo ItemBehaviorBodyField;


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
				Logger.Warn(identifier + " - Could Not Find PlasmaSpikeStrip Assembly");
			}

			HookMethods();
		}



		private static void GatherInfos()
		{
			Type type;

			type = Type.GetType("GrooveSaladSpikestripContent.Content.WarpedElite, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				GravityBuffField = type.GetField("gravityBuff", Flags);
				if (GravityBuffField == null) Logger.Warn(identifier + " - Could Not Find Field : WarpedElite.gravityBuff");

				type = type.GetNestedType("GravityBuffBehaviour", Flags);
				if (type != null)
				{
					LiftForceField = type.GetField("antiGravCoef", Flags);
					if (LiftForceField == null) Logger.Warn(identifier + " - Could Not Find Field : GravityBuffBehaviour.antiGravCoef");
				}
				else
				{
					Logger.Warn(identifier + " - Could Not Find NestedType : WarpedElite.GravityBuffBehaviour");
				}
			}
			else
			{
				Logger.Warn(identifier + " - Could Not Find Type : GrooveSaladSpikestripContent.Content.WarpedElite");
			}

			type = Type.GetType("GrooveSaladSpikestripContent.Content.PlatedElite, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				StifleBuffField = type.GetField("damageReductionBuff", Flags);
				if (StifleBuffField == null) Logger.Warn(identifier + " - Could Not Find Field : PlatedElite.damageReductionBuff");

				TakeDamageMethod = type.GetMethod("HealthComponent_TakeDamage", Flags);
				if (TakeDamageMethod == null) Logger.Warn(identifier + " - Could Not Find Method : PlatedElite.HealthComponent_TakeDamage");

				type = type.GetNestedType("PlatedAffixBuffBehaviour", Flags);
				if (type != null)
				{
					MaxPlatingMethod = type.GetMethod("get_maxPlatingCount", Flags);
					if (MaxPlatingMethod == null) Logger.Warn(identifier + " - Could Not Find Method : PlatedAffixBuffBehaviour.get_maxPlatingCount");
				}
				else
				{
					Logger.Warn(identifier + " - Could Not Find NestedType : PlatedElite.PlatedAffixBuffBehaviour");
				}
			}
			else
			{
				Logger.Warn(identifier + " - Could Not Find Type : GrooveSaladSpikestripContent.Content.PlatedElite");
			}
		}

		private static void HookMethods()
		{
			if (TakeDamageMethod != null && StifleBuffField != null)
			{
				HookEndpointManager.Modify(TakeDamageMethod, (ILContext.Manipulator)ModifyDamageReduction);
			}

			ItemBehaviorBodyField = typeof(CharacterBody.ItemBehavior).GetField("body");
			if (MaxPlatingMethod != null && ItemBehaviorBodyField != null)
			{
				HookEndpointManager.Modify(MaxPlatingMethod, (ILContext.Manipulator)OverridePlating);
			}

			if (LiftForceField != null)
			{
				Configuration.AspectWarpedLiftForce.SettingChanged += AspectWarpedLiftForce_SettingChanged;

				LiftForceField.SetValue(null, Configuration.AspectWarpedLiftForce.Value);
			}
		}

		private static void AspectWarpedLiftForce_SettingChanged(object sender, EventArgs e)
		{
			ConfigEntry<float> configEntry = (ConfigEntry<float>)sender;
			LiftForceField.SetValue(null, configEntry.Value);
		}



		private static void ModifyDamageReduction(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchLdsfld(StifleBuffField),
				x => x.MatchCallOrCallvirt<CharacterBody>("GetBuffCount"),
				x => x.MatchConvR4()
			);

			if (found)
			{
				c.Index += 3;

				c.EmitDelegate<Func<float, float>>((factor) =>
				{
					factor = Mathf.Pow(factor, Configuration.AspectPlatedStifleExponent.Value);
					factor *= Configuration.AspectPlatedStifleMult.Value;

					return factor;
				});
			}
			else
			{
				Logger.Warn(identifier + " - ModifyDamageReduction Failed!");
			}
		}

		private static void OverridePlating(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			c.Index = 0;

			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldfld, ItemBehaviorBodyField);
			c.EmitDelegate<Func<CharacterBody, int>>((body) =>
			{
				float mult = 3f * ((body.teamComponent.teamIndex == TeamIndex.Player) ? Configuration.AspectPlatedPlayerPlateCountMult.Value : Configuration.AspectPlatedMonsterPlateCountMult.Value);

				return Mathf.Clamp((int)(body.bestFitRadius * mult), 2, 10);
			});
			c.Emit(OpCodes.Ret);
		}



		internal static BuffIndex GetGravityBuffIndex()
		{
			if (GravityBuffField != null)
			{
				BuffDef buffDef = (BuffDef)GravityBuffField.GetValue(null);
				if (buffDef)
				{
					BuffIndex buffIndex = buffDef.buffIndex;
					Logger.Info(identifier + " - gravityBuff : " + buffIndex);
					return buffIndex;
				}
			}

			Logger.Warn(identifier + " - GetGravityBuffIndex Failed!");
			return BuffIndex.None;
		}
	}
}
