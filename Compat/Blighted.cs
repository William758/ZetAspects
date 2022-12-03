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
	internal static class Blighted
	{
		private static readonly string GUID = "com.Moffein.BlightedElites";
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static MethodInfo ActivateMethod;
		private static MethodInfo DeactivateMethod;

		private static FieldInfo BodyField;
		private static FieldInfo ActiveField;
		private static FieldInfo Buff1Field;
		private static FieldInfo Buff2Field;

		internal static int blightActivationHook = 0;
		internal static int blightDeactivationHook = 0;
		internal static bool blightBuffControl = false;



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
				Logger.Warn("[BlightedCompat] - Could Not Find Blighted Assembly");
			}

			HookMethods();
		}



		private static void GatherInfos()
		{
			Type type;

			type = Type.GetType("BlightedElites.Components.AffixBlightedComponent, " + PluginAssembly.FullName, false);
			if (type != null)
			{
				ActivateMethod = type.GetMethod("Activate", Flags);
				if (ActivateMethod == null) Logger.Warn("[BlightedCompat] - Could Not Find Method : AffixBlightedComponent.Activate");

				DeactivateMethod = type.GetMethod("Deactivate", Flags);
				if (DeactivateMethod == null) Logger.Warn("[BlightedCompat] - Could Not Find Method : AffixBlightedComponent.Deactivate");

				BodyField = type.GetField("characterBody", Flags);
				if (BodyField == null) Logger.Warn("[BlightedCompat] - Could Not Find Field : AffixBlightedComponent.characterBody");

				ActiveField = type.GetField("active", Flags);
				if (ActiveField == null) Logger.Warn("[BlightedCompat] - Could Not Find Field : AffixBlightedComponent.active");

				Buff1Field = type.GetField("buff1", Flags);
				if (Buff1Field == null) Logger.Warn("[BlightedCompat] - Could Not Find Field : AffixBlightedComponent.buff1");

				Buff2Field = type.GetField("buff2", Flags);
				if (Buff2Field == null) Logger.Warn("[BlightedCompat] - Could Not Find Field : AffixBlightedComponent.buff2");
			}
			else
			{
				Logger.Warn("[BlightedCompat] - Could Not Find Type : BlightedElites.Components.AffixBlightedComponent");
			}
		}

		private static void HookMethods()
		{
			if (BodyField != null && ActiveField != null && Buff1Field != null && Buff2Field != null)
			{
				if (ActivateMethod != null && DeactivateMethod != null)
				{
					HookEndpointManager.Modify(ActivateMethod, (ILContext.Manipulator)ActivateHook);
					HookEndpointManager.Modify(DeactivateMethod, (ILContext.Manipulator)DeactivateHook);

					if (blightActivationHook != 3 || blightDeactivationHook != 3)
					{
						HookEndpointManager.Unmodify(ActivateMethod, (ILContext.Manipulator)ActivateHook);
						HookEndpointManager.Unmodify(DeactivateMethod, (ILContext.Manipulator)DeactivateHook);

						Logger.Warn("[BlightedCompat] - Could Not Fully Control Blight Buffs!");
					}
					else
					{
						blightBuffControl = true;
					}
				}
				else
				{
					Logger.Warn("[BlightedCompat] - Failed to find required Methods!");
				}
			}
			else
			{
				Logger.Warn("[BlightedCompat] - Failed to find required Fields!");
			}
		}



		private static void ActivateHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdcI4(1),
				x => x.MatchStfld(ActiveField)
			);

			if (found)
			{
				blightActivationHook++;

				c.Index += 3;

				c.Emit(OpCodes.Ldarg, 0);
				c.Emit(OpCodes.Ldfld, BodyField);
				c.Emit(OpCodes.Ldarg, 0);
				c.Emit(OpCodes.Ldfld, Buff1Field);
				c.Emit(OpCodes.Ldarg, 0);
				c.Emit(OpCodes.Ldfld, Buff2Field);
				c.EmitDelegate<Action<CharacterBody, BuffDef, BuffDef>>((body, firstElite, secondElite) =>
				{
					BlightedStateManager.Activated(body, firstElite.buffIndex, secondElite.buffIndex);
				});
			}
			else
			{
				Logger.Warn("[BlightedCompat] - ActivateHook : Activation Failed!");
			}

			c.Index = 0;

			found = c.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(Buff1Field),
				x => x.MatchCallOrCallvirt<CharacterBody>("AddBuff")
			);

			if (found)
			{
				blightActivationHook++;

				c.Index += 2;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldnull);
			}
			else
			{
				Logger.Warn("[BlightedCompat] - ActivateHook : AddBuff1 Failed!");
			}

			c.Index = 0;

			found = c.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(Buff2Field),
				x => x.MatchCallOrCallvirt<CharacterBody>("AddBuff")
			);

			if (found)
			{
				blightActivationHook++;

				c.Index += 2;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldnull);
			}
			else
			{
				Logger.Warn("[BlightedCompat] - DeactivateHook : AddBuff2 Failed!");
			}
		}

		private static void DeactivateHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			c.Index = 0;

			bool found = c.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(Buff1Field),
				x => x.MatchCallOrCallvirt<CharacterBody>("RemoveBuff")
			);

			if (found)
			{
				blightDeactivationHook++;

				c.Index += 2;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldnull);
			}
			else
			{
				Logger.Warn("[BlightedCompat] - DeactivateHook : RemoveBuff1 Failed!");
			}

			c.Index = 0;

			found = c.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdfld(Buff2Field),
				x => x.MatchCallOrCallvirt<CharacterBody>("RemoveBuff")
			);

			if (found)
			{
				blightDeactivationHook++;

				c.Index += 2;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldnull);
			}
			else
			{
				Logger.Warn("[BlightedCompat] - DeactivateHook : RemoveBuff2 Failed!");
			}

			found = c.TryGotoNext(
				x => x.MatchLdarg(0),
				x => x.MatchLdcI4(0),
				x => x.MatchStfld(ActiveField)
			);

			if (found)
			{
				blightDeactivationHook++;

				c.Index += 3;

				c.Emit(OpCodes.Ldarg, 0);
				c.Emit(OpCodes.Ldfld, BodyField);
				c.EmitDelegate<Action<CharacterBody>>((body) =>
				{
					BlightedStateManager.Deactivated(body);
				});
			}
			else
			{
				Logger.Warn("[BlightedCompat] - DeactivateHook : Deactivation Failed!");
			}
		}
	}
}
