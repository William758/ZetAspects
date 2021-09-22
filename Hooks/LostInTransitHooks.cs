using BepInEx;
using BepInEx.Bootstrap;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using RoR2;
using System;
using System.Reflection;
using UnityEngine;

namespace TPDespair.ZetAspects
{
    internal static class LostInTransitHooks
    {
		private static BaseUnityPlugin Plugin;
		private static Assembly PluginAssembly;

		private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static MethodInfo LeechingHealMethod;
		private static MethodInfo FrenziedStatMethod;

		internal static bool leechHook = false;
		internal static bool frenzyMSHook = false;
		internal static bool frenzyASHook = false;



		internal static void Init()
		{
			Plugin = Chainloader.PluginInfos["com.swuff.LostInTransit"].Instance;
			PluginAssembly = Assembly.GetAssembly(Plugin.GetType());

			if (PluginAssembly != null)
			{
				Debug.LogWarning("LostInTransit Assembly Found");

				Type type;

				type = Type.GetType("LostInTransit.Buffs.AffixLeeching, " + PluginAssembly.FullName, false);
				if (type != null)
				{
					Debug.LogWarning("LostInTransit.Buffs.AffixLeeching Type Found");

					type = type.GetNestedType("AffixLeechingBehavior", Flags);
					if (type != null)
					{
						Debug.LogWarning("AffixLeechingBehavior Type Found");

						LeechingHealMethod = type.GetMethod("OnDamageDealtServer", Flags);
						if (LeechingHealMethod != null)
						{
							Debug.LogWarning("OnDamageDealtServer Method Found");

							HookEndpointManager.Modify(LeechingHealMethod, (ILContext.Manipulator)LeechAmountHook);
						}
					}
				}

				type = Type.GetType("LostInTransit.Buffs.AffixFrenzied, " + PluginAssembly.FullName, false);
				if (type != null)
				{
					Debug.LogWarning("LostInTransit.Buffs.AffixFrenzied Type Found");

					type = type.GetNestedType("AffixFrenziedBehavior", Flags);
					if (type != null)
					{
						Debug.LogWarning("AffixFrenziedBehavior Type Found");

						FrenziedStatMethod = type.GetMethod("RecalculateStatsEnd", Flags);
						if (FrenziedStatMethod != null)
						{
							Debug.LogWarning("RecalculateStatsEnd Method Found");

							HookEndpointManager.Modify(FrenziedStatMethod, (ILContext.Manipulator)FrenzyStatHook);
						}
					}
				}
			}
		}



		private static void LeechAmountHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchLdloca(1),
				x => x.MatchInitobj<ProcChainMask>()
			);

			if (found)
			{
				leechHook = true;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Ldarg, 1);
				c.EmitDelegate<Func<DamageReport, float>>((damageReport) =>
				{
					CharacterBody attacker = damageReport.attackerBody;
					BuffDef AffixLeeching = Catalog.LostInTransit.Buffs.AffixLeeching;

					float count = AffixLeeching ? ZetAspectsPlugin.GetStackMagnitude(attacker, AffixLeeching) : 1f;
					float value = Configuration.AspectLeechingBaseLeechGain.Value + Configuration.AspectLeechingStackLeechGain.Value * (count - 1f);

					if (attacker.teamComponent.teamIndex != TeamIndex.Player) value *= Configuration.AspectLeechingMonsterLeechMult.Value;

					return damageReport.damageDealt * damageReport.damageInfo.procCoefficient * value;
				});
			}
			else
			{
				Debug.LogWarning("ZetAspects - LIT : LeechAmountHook Failed");
			}
		}

		private static void FrenzyStatHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchCallOrCallvirt<CharacterBody>("get_moveSpeed"),
				x => x.MatchLdcR4(out _),
				x => x.MatchMul(),
				x => x.MatchCallOrCallvirt<CharacterBody>("set_moveSpeed")
			);

			if (found)
			{
				frenzyMSHook = true;

				c.Index += 3;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Dup);
				c.EmitDelegate<Func<CharacterBody, float>>((self) =>
				{
					BuffDef AffixFrenzied = Catalog.LostInTransit.Buffs.AffixFrenzied;

					float count = AffixFrenzied ? ZetAspectsPlugin.GetStackMagnitude(self, AffixFrenzied) : 1f;
					float value = Configuration.AspectFrenziedBaseMovementGain.Value + Configuration.AspectFrenziedStackMovementGain.Value * (count - 1f);

					if (self.teamComponent.teamIndex != TeamIndex.Player) value *= Configuration.AspectFrenziedMonsterMovementMult.Value;

					return self.moveSpeed * (1f + value);
				});
			}
			else
			{
				Debug.LogWarning("ZetAspects - LIT : FrenzyStatHook - MoveSpeed Failed");
			}

			c.Index = 0;

			found = c.TryGotoNext(
				x => x.MatchCallOrCallvirt<CharacterBody>("get_attackSpeed"),
				x => x.MatchLdcR4(out _),
				x => x.MatchMul(),
				x => x.MatchCallOrCallvirt<CharacterBody>("set_attackSpeed")
			);

			if (found)
			{
				frenzyASHook = true;

				c.Index += 3;

				c.Emit(OpCodes.Pop);
				c.Emit(OpCodes.Dup);
				c.EmitDelegate<Func<CharacterBody, float>>((self) =>
				{
					BuffDef AffixFrenzied = Catalog.LostInTransit.Buffs.AffixFrenzied;

					float count = AffixFrenzied ? ZetAspectsPlugin.GetStackMagnitude(self, AffixFrenzied) : 1f;
					float value = Configuration.AspectFrenziedBaseAttackSpeedGain.Value + Configuration.AspectFrenziedStackAttackSpeedGain.Value * (count - 1f);

					if (self.teamComponent.teamIndex != TeamIndex.Player) value *= Configuration.AspectFrenziedMonsterAttackSpeedMult.Value;

					return self.attackSpeed * (1f + value);
				});
			}
			else
			{
				Debug.LogWarning("ZetAspects - LIT : FrenzyStatHook - AttackSpeed Failed");
			}
		}
	}
}
