using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using RoR2;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

using static TPDespair.ZetAspects.ReflectionUtility;

namespace TPDespair.ZetAspects.Compat
{
	internal static class EliteVariety
	{
		internal static Reflector Reflector;
		private static readonly string GUID = "com.themysticsword.elitevariety";
		private static readonly string identifier = "[EliteVarietyCompat]";

		//private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static Type ImpPlaneImpaledType;
		private static Type SandstormBlindType;
		private static Type AffixTinkererType;

		private static MethodInfo ImpaleDotBehaviorMethod;

		private static MethodInfo SandstormAwakeMethod;
		private static MethodInfo SandstormTickMethod;

		private static MethodInfo TinkerScrapMethod;

		private static MethodInfo BannerBoostMethod;

		private static FieldInfo ImpaleDotIndexField;

		private static FieldInfo BlindVisionRadiusField;
		private static FieldInfo BlindCameraEffectField;

		private static FieldInfo SandstormDamageField;
		private static FieldInfo SandstormFrequencyField;
		private static FieldInfo SandstormProcField;

		private static FieldInfo TinkerDeploySlotField;

		internal static bool cycloneBlindHook = false;

		internal static bool tinkerStealHook = false;
		internal static bool tinkerLimitHook = false;



		internal static void Init()
		{
			Reflector = new Reflector(GUID, identifier);

			if (!Reflector.FindPluginAssembly()) return;

			GatherInfos();
			HookMethods();
		}

		internal static void LateSetup()
		{
			if (!Configuration.EliteVarietyHooks.Value) return;

			if (Configuration.AspectImpaleTweaks.Value)
			{
				if (ImpPlaneImpaledType != null && ImpaleDotIndexField != null)
				{
					Catalog.impaleDotIndex = (DotController.DotIndex)ImpaleDotIndexField.GetValue(ImpPlaneImpaledType);
					//if (Catalog.PluginLoaded("com.TPDespair.ZetArtifacts")) DisableZetArtifactsImpaleReduction();
				}
			}

			if (Configuration.AspectCycloneTweaks.Value)
			{
				if (SandstormBlindType != null)
				{
					ModifyCameraEffect();
				}
			}

			if (Configuration.AspectTinkerTweaks.Value)
			{
				if (AffixTinkererType != null && TinkerDeploySlotField != null)
				{
					Catalog.tinkerDeploySlot = (DeployableSlot)TinkerDeploySlotField.GetValue(AffixTinkererType);
					tinkerLimitHook = true;
				}
			}
		}



		private static void GatherInfos()
		{
			Type type;

			ImpPlaneImpaledType = Reflector.GetType("EliteVariety.Buffs.ImpPlaneImpaled");
			if (ImpPlaneImpaledType != null)
			{
				ImpaleDotIndexField = Reflector.GetField(ImpPlaneImpaledType, "dotIndex");

				ImpaleDotBehaviorMethod = Reflector.GetMatchingMethod(Reflector.GetType(ImpPlaneImpaledType, "<>c"), new Type[] { typeof(DotController), typeof(DotController.DotStack) });
				if (ImpaleDotBehaviorMethod == null) Logger.Warn(identifier + " - Could Not Find ImpaleDotBehaviorMethod");
			}



			SandstormBlindType = Reflector.GetType("EliteVariety.Buffs.SandstormBlind");
			if (SandstormBlindType != null)
			{
				BlindVisionRadiusField = Reflector.GetField(SandstormBlindType, "maxVisionRadius");
				BlindCameraEffectField = Reflector.GetField(SandstormBlindType, "cameraEffect");
			}

			type = Reflector.GetType("EliteVariety.Buffs.AffixSandstorm", "EliteVarietySandstormBehavior");
			if (type != null)
			{
				SandstormAwakeMethod = Reflector.GetMethod(type, "Awake");
				SandstormTickMethod = Reflector.GetMethod(type, "Tick");

				SandstormDamageField = Reflector.GetField(type, "damage");
				SandstormFrequencyField = Reflector.GetField(type, "tickFrequency");
				SandstormProcField = Reflector.GetField(type, "procCoefficient");
			}

			AffixTinkererType = Reflector.GetType("EliteVariety.Buffs.AffixTinkerer");

			Reflector mysticUtilReflector = new Reflector("com.themysticsword.mysticsrisky2utils", identifier);
			if (mysticUtilReflector.FindPluginAssembly())
			{
				Type charInfoType = mysticUtilReflector.GetType("MysticsRisky2Utils.MysticsRisky2UtilsPlugin", "GenericCharacterInfo");

				if (charInfoType != null)
				{
					if (AffixTinkererType != null)
					{
						TinkerScrapMethod = Reflector.GetMatchingMethod(AffixTinkererType, new Type[] { typeof(DamageInfo), charInfoType, charInfoType });
						if (TinkerScrapMethod == null) Logger.Warn(identifier + " - Could Not Find TinkerScrapStealMethod");
					}

					type = Reflector.GetType("EliteVariety.Buffs.AffixBuffing");
					if (type != null)
					{
						BannerBoostMethod = Reflector.GetMatchingMethod(Reflector.GetType(type, "<>c__DisplayClass6_0"), new Type[] { typeof(DamageInfo), charInfoType, charInfoType });
						if (TinkerScrapMethod == null) Logger.Warn(identifier + " - Could Not Find BannerWardBoostMethod");
					}
				}
			}

			if (AffixTinkererType != null) TinkerDeploySlotField = Reflector.GetField(AffixTinkererType, "deployableSlot");
		}

		private static void HookMethods()
		{
			if (Configuration.AspectImpaleTweaks.Value)
			{
				if (ImpaleDotBehaviorMethod != null)
				{
					HookEndpointManager.Modify(ImpaleDotBehaviorMethod, (ILContext.Manipulator)ImpaleBehaviorHook);
				}
			}

			if (Configuration.AspectCycloneTweaks.Value)
			{
				if (SandstormAwakeMethod != null && SandstormDamageField != null && SandstormFrequencyField != null && SandstormProcField != null)
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
					HookEndpointManager.Modify(TinkerScrapMethod, (ILContext.Manipulator)GenericReturnHook);
					tinkerStealHook = true;
				}
			}

			if (Configuration.AspectBannerTweaks.Value)
			{
				if (BannerBoostMethod != null)
				{
					HookEndpointManager.Modify(BannerBoostMethod, (ILContext.Manipulator)GenericReturnHook);
				}
			}
		}






		private static void ModifyCameraEffect()
		{
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
					Logger.Warn(identifier + " - Sandstorm CameraEffect is Null!");
				}
			}
		}



		private static void ImpaleBehaviorHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchCallOrCallvirt<HealthComponent>("get_fullCombinedHealth"),
				x => x.MatchLdcR4(out _)
			);

			if (found)
			{
				c.Index += 2;

				c.EmitDelegate<Func<float, float>>((value) =>
				{
					float mult = Run.instance ? Mathf.Clamp(Run.instance.ambientLevel / 90f, 0.05f, 1f) : 0.5f;

					return Mathf.Min(value, 0.33f * mult);
				});
			}
			else
			{
				Logger.Warn(identifier + " - ImpaleBehaviorHook Failed");
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
			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldc_R4, Configuration.AspectCycloneProc.Value);
			c.Emit(OpCodes.Stfld, SandstormProcField);
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
				Logger.Warn(identifier + " - TickCritHook Failed");
			}
		}

		private static void GenericReturnHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			c.Index = 0;

			c.Emit(OpCodes.Ret);
		}
	}
}
