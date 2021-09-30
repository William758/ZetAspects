using BepInEx;
using BepInEx.Bootstrap;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RoR2;

namespace TPDespair.ZetAspects
{
	internal static class EliteVarietyHooks
	{
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static Type SandstormBlindType;

		private static MethodInfo SandstormAwakeMethod;
		private static MethodInfo SandstormTickMethod;

		private static MethodInfo TinkerScrapMethod;

		private static FieldInfo BlindVisionRadiusField;
		private static FieldInfo BlindCameraEffectField;

		private static FieldInfo SandstormDamageField;
		private static FieldInfo SandstormFrequencyField;



		internal static void Init()
		{
			Plugin = Chainloader.PluginInfos["com.themysticsword.elitevariety"].Instance;
			PluginAssembly = Assembly.GetAssembly(Plugin.GetType());

			if (PluginAssembly != null)
			{
				FindStuff();
			}
			else
			{
				Debug.LogWarning("ZetAspect [EV] - Could Not Find Assembly");
			}

			if (Configuration.AspectCycloneTweaks.Value)
			{
				if (SandstormAwakeMethod != null && SandstormDamageField != null && SandstormFrequencyField != null)
				{
					HookEndpointManager.Modify(SandstormAwakeMethod, (ILContext.Manipulator)TickValueHook);
				}

				if (SandstormTickMethod != null)
				{
					HookEndpointManager.Modify(SandstormTickMethod, (ILContext.Manipulator)TickCritHook);
				}
			}

			if (Configuration.AspectTinkerTweaks.Value)
			{
				if (TinkerScrapMethod != null)
				{
					HookEndpointManager.Modify(TinkerScrapMethod, (ILContext.Manipulator)TinkerScrapHook);
				}
			}
		}



		private static void FindStuff()
		{
			Type type;

			type = Type.GetType("EliteVariety.Buffs.SandstormBlind, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				SandstormBlindType = type;

				BlindVisionRadiusField = type.GetField("maxVisionRadius", Flags);
				if (BlindVisionRadiusField == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Field : maxVisionRadius");

				BlindCameraEffectField = type.GetField("cameraEffect", Flags);
				if (BlindCameraEffectField == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Field : cameraEffect");
			}
			else
			{
				Debug.LogWarning("ZetAspect [LIT] - Could Not Find Type : EliteVariety.Buffs.SandstormBlind");
			}

			type = Type.GetType("EliteVariety.Buffs.AffixSandstorm, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				type = type.GetNestedType("EliteVarietySandstormBehavior", Flags);
				if (type != null)
				{
					SandstormAwakeMethod = type.GetMethod("Awake", Flags);
					if (SandstormAwakeMethod == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Method : Awake");

					SandstormTickMethod = type.GetMethod("Tick", Flags);
					if (SandstormTickMethod == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Method : Tick");

					SandstormDamageField = type.GetField("damage", Flags);
					if (SandstormDamageField == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Field : damage");

					SandstormFrequencyField = type.GetField("tickFrequency", Flags);
					if (SandstormFrequencyField == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Field : tickFrequency");
				}
				else
				{
					Debug.LogWarning("ZetAspect [EV] - Could Not Find NestedType : EliteVarietySandstormBehavior");
				}
			}
			else
			{
				Debug.LogWarning("ZetAspect [EV] - Could Not Find Type : EliteVariety.Buffs.AffixSandstorm");
			}

			type = Type.GetType("EliteVariety.Buffs.AffixTinkerer, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				TinkerScrapMethod = type.GetMethod("GenericGameEvents_OnHitEnemy", Flags);
				if (TinkerScrapMethod == null) Debug.LogWarning("ZetAspect [EV] - Could Not Find Method : GenericGameEvents_OnHitEnemy");
			}
			else
			{
				Debug.LogWarning("ZetAspect [EV] - Could Not Find Type : EliteVariety.Buffs.AffixTinkerer");
			}
		}



		internal static void LateSetup()
		{
			if (!Configuration.AspectCycloneTweaks.Value) return;

			float visionRange = 240f;

			if (BlindVisionRadiusField != null) BlindVisionRadiusField.SetValue(SandstormBlindType, visionRange);

			if (BlindCameraEffectField != null)
			{
				GameObject cameraEffect = (GameObject)BlindCameraEffectField.GetValue(SandstormBlindType);
				if (cameraEffect)
				{
					RampFog rampFog = cameraEffect.transform.Find("CameraEffect/PP").GetComponent<PostProcessVolume>().sharedProfile.GetSetting<RampFog>();

					rampFog.fogOne.Override(visionRange * 0.001f);
					rampFog.fogHeightStart.Override(-visionRange * 0.5f);
					rampFog.fogHeightEnd.Override(visionRange * 0.5f);

					BlindCameraEffectField.SetValue(SandstormBlindType, cameraEffect);
				}
				else
				{
					Debug.LogWarning("ZetAspect [EV] - Sandstorm CameraEffect is Null!");
				}
			}
		}



		private static void TickValueHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			c.Index = 0;

			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldc_R4, 10f);
			c.Emit(OpCodes.Stfld, SandstormDamageField);
			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldc_R4, 2f);
			c.Emit(OpCodes.Stfld, SandstormFrequencyField);
		}

		private static void TickCritHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			int critResult = 1;
			ILLabel label = null;

			bool found = c.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(out _),
				x => x.MatchCallOrCallvirt<NetworkedBodyAttachment>("get_attachedBody"),
				x => x.MatchCallOrCallvirt<CharacterBody>("RollCrit"),
				x => x.MatchStloc(out critResult)
			);

			if (found)
			{
				c.Index += 4;
				label = c.MarkLabel();
				c.Index -= 4;
				c.Emit(OpCodes.Ldc_I4, 0);
				c.Emit(OpCodes.Br, label);
			}
			else
			{
				Debug.LogWarning("ZetAspects [EV] - TickCritHook Failed");
			}
		}

		private static void TinkerScrapHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			c.Index = 0;

			c.Emit(OpCodes.Ret);
		}
	}
}
