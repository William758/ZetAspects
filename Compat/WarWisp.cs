using System;
using System.Reflection;
using UnityEngine;
using BepInEx;
using BepInEx.Bootstrap;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using RoR2;
using BepInEx.Configuration;

namespace TPDespair.ZetAspects.Compat
{
	internal static class WarWisp
	{
		private static readonly string GUID = "com.PopcornFactory.WispMod";
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static MethodInfo EquipGainedMethod;
		private static MethodInfo EquipRemovedMethod;
		private static MethodInfo BuffGainedMethod;

		private static MethodInfo ControllerStartMethod;
		private static MethodInfo ControllerUpdateMethod;
		private static MethodInfo ControllerFixedUpdateMethod;
		private static FieldInfo ShieldBufferField;
		private static FieldInfo RadiusField;

		private static Type ConfigType;
		private static FieldInfo BlockCfgField;
		private static FieldInfo AllyArmorCfgField;
		private static FieldInfo MoveDebuffCfgField;

		private static bool shieldGainHook = false;
		private static bool shieldBufferHook = false;
		internal static bool shieldOverrideHook = false;
		internal static bool shieldRecoveryHook = false;

		internal static float cachedAllyArmorValue = -1000000f;



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
				Logger.Warn("[WarWispCompat] - Could Not Find WarWisp Assembly");
			}

			HookMethods();

			SyncAllyArmorValue();
		}



		private static void GatherInfos()
		{
			Type type;

			type = Type.GetType("WispMod.Modules.Elite.AffixNullifier, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				EquipGainedMethod = type.GetMethod("GiveNullifierController", Flags);
				if (EquipGainedMethod == null) Logger.Warn("[WarWispCompat] - Could Not Find Method : AffixNullifier.GiveNullifierController");

				EquipRemovedMethod = type.GetMethod("RemoveNullifierController", Flags);
				if (EquipRemovedMethod == null) Logger.Warn("[WarWispCompat] - Could Not Find Method : AffixNullifier.RemoveNullifierController");

				BuffGainedMethod = type.GetMethod("GiveNullifierControllerOnBuff", Flags);
				if (BuffGainedMethod == null) Logger.Warn("[WarWispCompat] - Could Not Find Method : AffixNullifier.GiveNullifierControllerOnBuff");

				type = type.GetNestedType("NullifierController");
				if (type != null)
				{
					ControllerStartMethod = type.GetMethod("Start", Flags);
					if (ControllerStartMethod == null) Logger.Warn("[WarWispCompat] - Could Not Find Method : NullifierController.Start");

					ControllerUpdateMethod = type.GetMethod("Update", Flags);
					if (ControllerUpdateMethod == null) Logger.Warn("[WarWispCompat] - Could Not Find Method : NullifierController.Update");

					ControllerFixedUpdateMethod = type.GetMethod("FixedUpdate", Flags);
					if (ControllerFixedUpdateMethod == null) Logger.Warn("[WarWispCompat] - Could Not Find Method : NullifierController.FixedUpdate");

					ShieldBufferField = type.GetField("originalMaxShield", Flags);
					if (ShieldBufferField == null) Logger.Warn("[WarWispCompat] - Could Not Find Field : NullifierController.originalMaxShield");

					RadiusField = type.GetField("radius", Flags);
					if (RadiusField == null) Logger.Warn("[WarWispCompat] - Could Not Find Field : NullifierController.radius");
				}
				else
				{
					Logger.Warn("[WarWispCompat] - Could Not Find NestedType : AffixNullifier.NullifierController");
				}
			}
			else
			{
				Logger.Warn("[WarWispCompat] - Could Not Find Type : WispMod.Modules.Elite.AffixNullifier");
			}

			type = Type.GetType("WispMod.Modules.Config, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				ConfigType = type;

				BlockCfgField = type.GetField("nullifierBlockChance", Flags);
				if (BlockCfgField == null) Logger.Warn("[WarWispCompat] - Could Not Find Field : Config.nullifierBlockChance");

				AllyArmorCfgField = type.GetField("nullifierArmourBuff", Flags);
				if (AllyArmorCfgField == null) Logger.Warn("[WarWispCompat] - Could Not Find Field : Config.nullifierArmourBuff");

				MoveDebuffCfgField = type.GetField("nullifierMoveSpeedDebuffOnEnemy", Flags);
				if (MoveDebuffCfgField == null) Logger.Warn("[WarWispCompat] - Could Not Find Field : Config.nullifierMoveSpeedDebuffOnEnemy");
			}
			else
			{
				Logger.Warn("[WarWispCompat] - Could Not Find Type : WispMod.Modules.Config");
			}
		}

		private static void HookMethods()
		{
			if (EquipGainedMethod != null)
			{
				Logger.Info("[WarWispCompat] - Hooking : EquipGainedMethod");
				HookEndpointManager.Modify(EquipGainedMethod, (ILContext.Manipulator)DisableFirstFlagHook);
			}

			if (EquipRemovedMethod != null)
			{
				Logger.Info("[WarWispCompat] - Hooking : EquipRemovedMethod");
				HookEndpointManager.Modify(EquipRemovedMethod, (ILContext.Manipulator)DisableFirstFlagHook);
			}

			if (BuffGainedMethod != null)
			{
				HookEndpointManager.Modify(BuffGainedMethod, (ILContext.Manipulator)BypassFirstPlayerCheckHook);
			}



			if (Configuration.AspectNullifierOverrideShield.Value && ControllerStartMethod != null && ShieldBufferField != null)
			{
				HookEndpointManager.Modify(ControllerStartMethod, (ILContext.Manipulator)DisableBaseShieldHook);
				HookEndpointManager.Modify(ControllerStartMethod, (ILContext.Manipulator)DisableShieldBufferHook);
				HookEndpointManager.Modify(ControllerStartMethod, (ILContext.Manipulator)DisableShieldSetHook);

				if (shieldGainHook && shieldBufferHook) shieldOverrideHook = true;
			}

			if (ControllerUpdateMethod != null && RadiusField != null)
			{
				HookEndpointManager.Modify(ControllerUpdateMethod, (ILContext.Manipulator)BoundRadiusHook);
			}

			if (ControllerFixedUpdateMethod != null)
			{
				HookEndpointManager.Modify(ControllerFixedUpdateMethod, (ILContext.Manipulator)BoundColliderRadiusHook);
			}

			if (Configuration.AspectNullifierShieldRecharge.Value && ControllerFixedUpdateMethod != null)
			{
				HookEndpointManager.Modify(ControllerFixedUpdateMethod, (ILContext.Manipulator)CombatStateHook);
			}
		}

		private static void SyncAllyArmorValue()
		{
			if (AllyArmorCfgField != null)
			{
				ConfigEntry<float> config = (ConfigEntry<float>)AllyArmorCfgField.GetValue(ConfigType);
				config.SettingChanged += Config_SettingChanged;
				cachedAllyArmorValue = config.Value;
				Logger.Info("[WarWispCompat] - cachedAllyArmorValue : " + cachedAllyArmorValue);
			}
		}

		private static void Config_SettingChanged(object sender, EventArgs e)
		{
			ConfigEntry<float> config = (ConfigEntry<float>)sender;
			cachedAllyArmorValue = config.Value;
			Logger.Info("[WarWispCompat] - cachedAllyArmorValue : " + cachedAllyArmorValue);
		}



		internal static float GetBlockChance()
		{
			if (BlockCfgField != null)
			{
				ConfigEntry<float> config = (ConfigEntry<float>)BlockCfgField.GetValue(ConfigType);

				return config.Value;
			}

			return -1000000f;
		}

		internal static float GetAllyArmor()
		{
			if (AllyArmorCfgField != null)
			{
				ConfigEntry<float> config = (ConfigEntry<float>)AllyArmorCfgField.GetValue(ConfigType);

				return config.Value;
			}

			return -1000000f;
		}

		internal static float GetMovementMult()
		{
			if (MoveDebuffCfgField != null)
			{
				ConfigEntry<float> config = (ConfigEntry<float>)MoveDebuffCfgField.GetValue(ConfigType);

				return config.Value;
			}

			return 1f;
		}



		private static void DisableFirstFlagHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchLdloc(0),
				x => x.MatchBrfalse(out _)
			);

			if (found)
			{
				c.Index += 1;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldc_I4, 0);
			}
			else
			{
				Logger.Warn("[WarWispCompat] - DisableFirstFlagHook Failed");
			}
		}

		private static void BypassFirstPlayerCheckHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchLdloc(0),
				x => x.MatchBrfalse(out _)
			);

			if (found)
			{
				found = c.TryGotoPrev( MoveType.After,
					x => x.MatchCallOrCallvirt<CharacterBody>("get_isPlayerControlled")
				);

				if (found)
				{
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 0);
				}
				else
				{
					Logger.Warn("[WarWispCompat] - BypassFirstPlayerCheckHook:BackToPlayerCheck Failed");
				}
			}
			else
			{
				Logger.Warn("[WarWispCompat] - BypassFirstPlayerCheckHook:FindFirstFlag Failed");
			}
		}

		private static void DisableBaseShieldHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchStfld<CharacterBody>("baseMaxShield")
			);

			if (found)
			{
				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Pop);
				c.Remove();

				shieldGainHook = true;
			}
			else
			{
				Logger.Warn("[WarWispCompat] - DisableBaseShieldHook Failed");
			}
		}

		private static void DisableShieldBufferHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchStfld(ShieldBufferField)
			);

			if (found)
			{
				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldc_R4, 0f);

				shieldBufferHook = true;
			}
			else
			{
				Logger.Warn("[WarWispCompat] - DisableShieldBufferHook Failed");
			}
		}

		private static void DisableShieldSetHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchStfld<HealthComponent>("shield")
			);

			if (found)
			{
				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Pop);
				c.Remove();
			}
			else
			{
				Logger.Warn("[WarWispCompat] - DisableShieldSetHook Failed");
			}
		}

		private static void BoundRadiusHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchStfld(RadiusField)
			);

			if (found)
			{
				c.EmitDelegate<Func<float, float>>((radius) =>
				{
					if (float.IsNaN(radius)) radius = 0f;

					radius = Mathf.Clamp(radius, 0f, 1f);

					return radius;
				});
			}
			else
			{
				Logger.Warn("[WarWispCompat] - BoundRadiusHook Failed");
			}
		}

		private static void BoundColliderRadiusHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchCallOrCallvirt<CharacterBody>("get_maxShield"),
				x => x.MatchDiv()
			);

			if (found)
			{
				c.Index += 2;

				c.EmitDelegate<Func<float, float>>((radius) =>
				{
					if (float.IsNaN(radius)) radius = 0f;

					radius = Mathf.Clamp(radius, 0f, 1f);

					return radius;
				});
			}
			else
			{
				Logger.Warn("[WarWispCompat] - BoundRadiusHook Failed");
			}
		}

		private static void CombatStateHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchStfld<CharacterBody>("outOfDangerStopwatch")
			);

			if (found)
			{
				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Pop);
				c.Remove();

				shieldRecoveryHook = true;
			}
			else
			{
				Logger.Warn("[WarWispCompat] - CombatStateHook Failed");
			}
		}
	}
}
