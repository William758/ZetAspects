using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace TPDespair.ZetAspects
{
	internal static class EffectHooks
	{
		internal static void Init()
		{
			ZetAspectIce.Hooks();
			ZetAspectLightning.Hooks();
			ZetAspectFire.Hooks();
			ZetAspectCelestial.Hooks();
			ZetAspectMalachite.Hooks();
			ZetAspectPerfect.Hooks();
			DamageTakenHook();
			LifeGainOnHitHook();
			HeadHunterBuffHook();
			OnHitEnemyHook();
			RefreshAspectBuffsHook();
		}



		private static void DamageTakenHook()
		{
			IL.RoR2.HealthComponent.TakeDamage += (il) =>
			{
				ILCursor c = new ILCursor(il);

				// find : store damageinfo.damge into variable
				bool found = c.TryGotoNext(
					x => x.MatchLdarg(1),
					x => x.MatchLdfld<DamageInfo>("damage"),
					x => x.MatchStloc(6)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Ldloc, 6);
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<float, HealthComponent, float>>((damage, healthComponent) =>
					{
						if (healthComponent.body.HasBuff(RoR2Content.Buffs.HealingDisabled) && Configuration.AspectPoisonNullDamageTaken.Value != 0f)
						{
							float extraDamage = Mathf.Abs(Configuration.AspectPoisonNullDamageTaken.Value);
							if (healthComponent.body.teamComponent.teamIndex == TeamIndex.Player) extraDamage *= Configuration.AspectEffectPlayerDebuffMult.Value;
							damage *= 1f + extraDamage;
						}

						return damage;
					});
					c.Emit(OpCodes.Stloc, 6);
				}
				else
				{
					Debug.LogWarning("ZetAspects - Damage Taken Hook Failed");
				}
			};
		}

		private static void LifeGainOnHitHook()
		{
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(3),
					x => x.MatchLdsfld(typeof(RoR2Content.Items).GetField("Seed")),
					x => x.MatchCallvirt<Inventory>("GetItemCount"),
					x => x.MatchStloc(19)
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Ldloc, 0);
					c.EmitDelegate<Func<int, CharacterBody, int>>((amount, self) =>
					{
						if (Configuration.LeechSeedHeal.Value > 1)
						{
							int seed = self.inventory.GetItemCount(RoR2Content.Items.Seed);
							amount += seed * (Configuration.LeechSeedHeal.Value - 1);
						}

						if (self.HasBuff(RoR2Content.Buffs.AffixPoison) && Configuration.AspectPoisonBaseHeal.Value > 0)
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, RoR2Content.Buffs.AffixPoison);
							amount += Configuration.AspectPoisonBaseHeal.Value + (int)(Configuration.AspectPoisonStackHeal.Value * (count - 1f) + 0.55f);
						}

						return amount;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects - LGOH Hook Failed");
				}
			};
		}

		private static void HeadHunterBuffHook()
		{
			IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcR4(3f),
					x => x.MatchLdcR4(5f),
					x => x.MatchLdloc(54),
					x => x.MatchConvR4(),
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchStloc(56)
				);

				if (found)
				{
					c.Index += 6;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc, 54);
					c.EmitDelegate<Func<int, float>>((count) =>
					{
						return Configuration.HeadHunterBaseDuration.Value + Configuration.HeadHunterStackDuration.Value * (count - 1);
					});
					c.Emit(OpCodes.Dup);
					c.Emit(OpCodes.Ldloc, 13);
					c.EmitDelegate<Action<float, CharacterBody>>((duration, attacker) =>
					{
						if (Configuration.HeadHunterBuffEnable.Value) attacker.AddTimedBuff(ZetAspectsContent.Buffs.ZetHeadHunter, duration);
					});
				}
				else
				{
					Debug.LogWarning("ZetAspects - HH Duration And Buff Hook Failed");
				}
			};
		}

		private static void OnHitEnemyHook()
		{
			On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damageInfo, victimObject) =>
			{
				ApplyAspectOnHitEffects(damageInfo, victimObject);

				orig(self, damageInfo, victimObject);
			};
		}

		private static void ApplyAspectOnHitEffects(DamageInfo damageInfo, GameObject victimObject)
		{
			if (!NetworkServer.active) return;
			if (damageInfo.procCoefficient == 0f || damageInfo.rejected) return;

			if (damageInfo.attacker)
			{
				CharacterBody attacker = damageInfo.attacker.GetComponent<CharacterBody>();
				CharacterBody victim = victimObject ? victimObject.GetComponent<CharacterBody>() : null;

				if (attacker && victim)
				{
					ZetAspectIce.FireFrostBlade(attacker, victim, damageInfo);
					ZetAspectLightning.ApplySapped(attacker, victim, damageInfo);
					ZetAspectCelestial.ApplyShredded(attacker, victim, damageInfo);
					ZetAspectPerfect.ApplyCripple(attacker, victim, damageInfo);
				}
			}
		}

		private static void RefreshAspectBuffsHook()
		{
			On.RoR2.CharacterBody.UpdateBuffs += (orig, self, deltaTime) =>
			{
				orig(self, deltaTime);

				if (self.inventory)
				{
					if (!self.HasBuff(RoR2Content.Buffs.AffixWhite))
					{
						if (self.inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectIce) > 0 || self.inventory.alternateEquipmentIndex == RoR2Content.Equipment.AffixWhite.equipmentIndex)
						{
							self.AddTimedBuff(RoR2Content.Buffs.AffixWhite, 5f);
						}
					}
					if (!self.HasBuff(RoR2Content.Buffs.AffixBlue))
					{
						if (self.inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectLightning) > 0 || self.inventory.alternateEquipmentIndex == RoR2Content.Equipment.AffixBlue.equipmentIndex)
						{
							self.AddTimedBuff(RoR2Content.Buffs.AffixBlue, 5f);
						}
					}
					if (!self.HasBuff(RoR2Content.Buffs.AffixRed))
					{
						if (self.inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectFire) > 0 || self.inventory.alternateEquipmentIndex == RoR2Content.Equipment.AffixRed.equipmentIndex)
						{
							self.AddTimedBuff(RoR2Content.Buffs.AffixRed, 5f);
						}
					}
					if (!self.HasBuff(RoR2Content.Buffs.AffixHaunted))
					{
						if (self.inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectCelestial) > 0 || self.inventory.alternateEquipmentIndex == RoR2Content.Equipment.AffixHaunted.equipmentIndex)
						{
							self.AddTimedBuff(RoR2Content.Buffs.AffixHaunted, 5f);
						}
					}
					if (!self.HasBuff(RoR2Content.Buffs.AffixPoison))
					{
						if (self.inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectMalachite) > 0 || self.inventory.alternateEquipmentIndex == RoR2Content.Equipment.AffixPoison.equipmentIndex)
						{
							self.AddTimedBuff(RoR2Content.Buffs.AffixPoison, 5f);
						}
					}
					if (!self.HasBuff(RoR2Content.Buffs.AffixLunar))
					{
						if (self.inventory.GetItemCount(ZetAspectsContent.Items.ZetAspectPerfect) > 0 || self.inventory.alternateEquipmentIndex == RoR2Content.Equipment.AffixLunar.equipmentIndex)
						{
							self.AddTimedBuff(RoR2Content.Buffs.AffixLunar, 5f);
						}
					}
				}
			};
		}
	}
}
