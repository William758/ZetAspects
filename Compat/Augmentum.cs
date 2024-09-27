using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using UnityEngine;
using RoR2;

using static TPDespair.ZetAspects.ReflectionUtility;

namespace TPDespair.ZetAspects.Compat
{
	internal static class Augmentum
	{
		internal static Reflector Reflector;
		private static readonly string GUID = "com.BrandonRosa.Augmentum";
		private static readonly string identifier = "[AugmentumCompat]";

		private static MethodInfo AdaptiveCompatUpdateMethod;
		private static MethodInfo AdaptiveCompatDropletInterceptMethod;

		internal static FieldRefer<int> AdaptivePreHitArmor = new FieldRefer<int>(150);
		internal static FieldRefer<bool> AdaptiveInvisibility = new FieldRefer<bool>(true);
		internal static FieldRefer<float> AdaptiveInvisibilityDuration = new FieldRefer<float>(3f);
		internal static FieldRefer<float> AdaptiveBoostDuration = new FieldRefer<float>(12f);
		internal static FieldRefer<float> AdaptiveLacerationDuration = new FieldRefer<float>(7.5f);


		internal static void LateSetup()
		{
			Reflector = new Reflector(GUID, identifier);

			if (!Reflector.FindPluginAssembly()) return;

			Type type = Reflector.GetType("BransItems.Modules.Pickups.EliteEquipments.AffixAdaptive");
			if (type != null)
			{
				FieldInfo fieldInfo = Reflector.GetField(type, "PreHitArmorAdd");
				if (fieldInfo != null)
				{
					AdaptivePreHitArmor.SetInfo(fieldInfo, null);
				}

				fieldInfo = Reflector.GetField(type, "EnableInvisibility");
				if (fieldInfo != null)
				{
					AdaptiveInvisibility.SetInfo(fieldInfo, null);
				}

				fieldInfo = Reflector.GetField(type, "InvisibleTimer");
				if (fieldInfo != null)
				{
					AdaptiveInvisibilityDuration.SetInfo(fieldInfo, null);
				}

				fieldInfo = Reflector.GetField(type, "AdaptiveBoostTimer");
				if (fieldInfo != null)
				{
					AdaptiveBoostDuration.SetInfo(fieldInfo, null);
				}

				fieldInfo = Reflector.GetField(type, "LacerationDuration");
				if (fieldInfo != null)
				{
					AdaptiveLacerationDuration.SetInfo(fieldInfo, null);
				}
			}





			AdaptiveCompatUpdateMethod = Reflector.GetMethod("BransItems.Modules.Pickups.EliteEquipments.ZetAdaptiveDrop", "CharacterBody_FixedUpdate");

			type = Reflector.GetType("BransItems.Modules.Compatability.ModCompatability", "ZetAspectsCompat");
			if (type != null)
			{
				AdaptiveCompatDropletInterceptMethod = Reflector.GetMatchingMethod(Reflector.GetType(type, "<>c"), new Type[] { null, typeof(PickupIndex), typeof(Vector3), typeof(Vector3) });
				if (AdaptiveCompatDropletInterceptMethod == null) Logger.Warn(identifier + " - Could Not Find AdaptiveCompatDropletInterceptMethod");
			}

			HookMethods();
		}



		private static void HookMethods()
		{
			if (AdaptiveCompatUpdateMethod != null)
			{
				HookEndpointManager.Modify(AdaptiveCompatUpdateMethod, (ILContext.Manipulator)DisableAdaptiveCompatUpdateHook);
			}

			if (AdaptiveCompatDropletInterceptMethod != null)
			{
				HookEndpointManager.Modify(AdaptiveCompatDropletInterceptMethod, (ILContext.Manipulator)DisableAdaptiveCompatDropletInterceptHook);
			}
		}



		private static void DisableAdaptiveCompatUpdateHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchStloc(1)
			);

			if (found)
			{
				c.Index += 1;

				c.Emit(OpCodes.Ldc_I4, 0);
				c.Emit(OpCodes.Stloc, 1);
			}
			else
			{
				Logger.Warn(identifier + " - DisableAdaptiveCompatUpdateHook Failed");
			}
		}

		private static void DisableAdaptiveCompatDropletInterceptHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchStloc(0)
			);

			if (found)
			{
				c.Index += 1;

				c.Emit(OpCodes.Ldc_I4, 0);
				c.Emit(OpCodes.Stloc, 0);
			}
			else
			{
				Logger.Warn(identifier + " - DisableAdaptiveCompatDropletInterceptHook Failed");
			}
		}
	}
}
