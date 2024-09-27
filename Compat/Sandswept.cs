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
	internal static class Sandswept
	{
		internal static Reflector Reflector;
		private static readonly string GUID = "com.TeamSandswept.Sandswept";
		private static readonly string identifier = "[SandsweptCompat]";

		internal static FieldRefer<float> MotivatorMovement = new FieldRefer<float>(0.25f);
		internal static FieldRefer<float> MotivatorAtkspd = new FieldRefer<float>(0.25f);
		internal static FieldRefer<BuffDef> MotivatorEmpowerBuff = new FieldRefer<BuffDef>(null);
		internal static FieldRefer<float> MotivatorEmpowerDuration = new FieldRefer<float>(4f);

		internal static FieldRefer<float> OsmiumPlayerDamageFactor = new FieldRefer<float>(0.75f);

		private static MethodInfo MotivatorBoostMethod;
		private static MethodInfo MotivatorProcMethod;

		private static MethodInfo OsmiumDamageMethod;

		private static FieldInfo MotivatorControllerBodyField;

		private static FieldInfo OsmiumNearDamageFactorField;

		internal static bool motivatorDisableBoostHook = false;
		internal static bool motivatorBoostSelfHook = false;

		internal static bool osmiumDisableNearDamageHook = false;



		internal static void Init()
		{
			Reflector = new Reflector(GUID, identifier);

			if (!Reflector.FindPluginAssembly()) return;

			GatherInfos();
			HookMethods();
		}



		private static void GatherInfos()
		{
			Type type = Reflector.GetType("Sandswept.Elites.Motivating");
			if (type != null)
			{
				FieldInfo fieldInfo = Reflector.GetField(type, "passiveMovementSpeedBuff");
				if (fieldInfo != null)
				{
					MotivatorMovement.SetInfo(fieldInfo, null);
				}

				fieldInfo = Reflector.GetField(type, "passiveAttackSpeedBuff");
				if (fieldInfo != null)
				{
					MotivatorAtkspd.SetInfo(fieldInfo, null);
				}

				fieldInfo = Reflector.GetField(type, "warcryBuff");
				if (fieldInfo != null)
				{
					MotivatorEmpowerBuff.SetInfo(fieldInfo, null);
				}

				fieldInfo = Reflector.GetField(type, "onHitAttackSpeedBuffDuration");
				if (fieldInfo != null)
				{
					MotivatorEmpowerDuration.SetInfo(fieldInfo, null);
				}

				MotivatorBoostMethod = Reflector.GetMethod(type, "GlobalEventManager_onServerDamageDealt");
			}

			type = Reflector.GetType("Sandswept.Elites.MotivatorController");
			if (type != null)
			{
				MotivatorControllerBodyField = Reflector.GetField(type, "body");

				MotivatorProcMethod = Reflector.GetMethod(type, "Proc");
			}

			type = Reflector.GetType("Sandswept.Elites.Osmium");
			if (type != null)
			{
				FieldInfo fieldInfo = Reflector.GetField(type, "playerOutsideDamageTakenMultiplier");
				if (fieldInfo != null)
				{
					OsmiumPlayerDamageFactor.SetInfo(fieldInfo, null);
				}

				OsmiumNearDamageFactorField = Reflector.GetField(type, "insideDamageTakenMultiplier");

				OsmiumDamageMethod = Reflector.GetMethod(type, "HealthComponent_TakeDamage");
			}
		}

		private static void HookMethods()
		{
			if (Configuration.AspectMotivatorTweaks.Value)
			{
				if (MotivatorBoostMethod != null)
				{
					HookEndpointManager.Modify(MotivatorBoostMethod, (ILContext.Manipulator)GenericReturnHook);
					motivatorDisableBoostHook = true;
				}
			}

			if (!motivatorDisableBoostHook && Configuration.AspectMotivatorEmpowerSelf.Value)
			{
				if (MotivatorProcMethod != null && MotivatorControllerBodyField != null)
				{
					HookEndpointManager.Modify(MotivatorProcMethod, (ILContext.Manipulator)EmpowerSelfHook);
					motivatorBoostSelfHook = true;
				}
			}

			if (Configuration.AspectOsmiumPlayerNearbyNormal.Value)
			{
				if (OsmiumDamageMethod != null && OsmiumNearDamageFactorField != null)
				{
					HookEndpointManager.Modify(OsmiumDamageMethod, (ILContext.Manipulator)OsmiumDamageHook);
				}
			}
		}



		private static void EmpowerSelfHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			c.Emit(OpCodes.Ldarg, 0);
			c.Emit(OpCodes.Ldfld, MotivatorControllerBodyField);
			c.EmitDelegate<Action<CharacterBody>>((body) =>
			{
				if (body)
				{
					HealthComponent healthComponent = body.healthComponent;
					if (healthComponent && healthComponent.alive)
					{
						body.AddTimedBuff(MotivatorEmpowerBuff, MotivatorEmpowerDuration);
					}
				}
			});
		}

		private static void OsmiumDamageHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchLdsfld(OsmiumNearDamageFactorField)
			);

			if (found)
			{
				c.Index += 1;

				c.Emit(OpCodes.Ldarg, 2);
				c.EmitDelegate<Func<float, HealthComponent, float>>((value, hc) =>
				{
					if (hc.body.isPlayerControlled)
					{
						return 1f;
					}

					return value;
				});

				osmiumDisableNearDamageHook = true;
			}
			else
			{
				Logger.Warn(identifier + " - OsmiumDamageHook Failed");
			}
		}



		private static void GenericReturnHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			c.Emit(OpCodes.Ret);
		}
	}
}
