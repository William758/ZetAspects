using RoR2;
using RoR2.Orbs;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class ZetAspectIce
	{
		public static ItemIndex itemIndex;

		internal static void Init()
		{
			DefineItem();
			FreezeHook();
			SlowHook();
			ProjectileHook();
		}

		private static void DefineItem()
		{
			ItemTag[] tags = { ItemTag.Damage, ItemTag.Utility };

			if (!ZetAspectsPlugin.ZetAspectRedTierCfg.Value) 
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = new ItemDef
			{
				name = "ZetAspectIce",
				tier = ZetAspectsPlugin.ZetAspectRedTierCfg.Value ? ItemTier.Tier3 : ItemTier.Boss,
				pickupModelPath = "Prefabs/PickupModels/PickupAffixWhite",
				pickupIconPath = "Textures/ItemIcons/texAffixWhiteIcon",
				nameToken = "Her Biting Embrace",
				pickupToken = "Become an aspect of ice.",
				descriptionToken = BuildDescription(),
				loreToken = "Become an aspect of ice.",
				tags = tags
			};

			ItemDisplayRuleDict disp = new ItemDisplayRuleDict(null);

			itemIndex = ItemAPI.Add(new CustomItem(itemDef, disp));
		}

		public static string BuildDescription()
        {
			string output = "<style=cDeath>Aspect of Ice</style> :\nAttacks <style=cIsUtility>chill</style> on hit for ";
			output += ZetAspectsPlugin.FormatSeconds(ZetAspectsPlugin.ZetAspectWhiteSlowDurationCfg.Value);
			output += ", reducing <style=cIsUtility>movement speed</style> by <style=cIsUtility>80%</style>.";
			if (ZetAspectsPlugin.ZetAspectWhiteFreezeChanceCfg.Value > 0f)
            {
				output += "\nAttacks have a <style=cIsUtility>";
				output += ZetAspectsPlugin.ZetAspectWhiteFreezeChanceCfg.Value;
				output += "%</style> <style=cStack>(+";
				output += ZetAspectsPlugin.ZetAspectWhiteFreezeChanceCfg.Value;
				output += "% per stack)</style> chance to <style=cIsUtility>freeze</style> for ";
				output += ZetAspectsPlugin.FormatSeconds(ZetAspectsPlugin.ZetAspectWhiteFreezeDurationCfg.Value) + ".";
			}
			if (ZetAspectsPlugin.ZetAspectWhiteBaseDamageCfg.Value > 0f)
            {
				output += "\nAttacks fire a <style=cIsDamage>blade</style> that deals <style=cIsDamage>";
				output += ZetAspectsPlugin.ZetAspectWhiteBaseDamageCfg.Value * 100f + "%</style>";
				if (ZetAspectsPlugin.ZetAspectWhiteStackDamageCfg.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += ZetAspectsPlugin.ZetAspectWhiteStackDamageCfg.Value * 100f + "% per stack)</style>";
				}
				output += " TOTAL damage.";
			}

			return output;
		}

		private static void FreezeHook()
        {
			// Add freeze chance
			IL.RoR2.SetStateOnHurt.OnTakeDamageServer += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdcR4(2f),
					x => x.MatchLdloc(1),
					x => x.MatchLdfld<DamageInfo>("procCoefficient"),
					x => x.MatchMul()
				);

				if (found)
				{
					// Move to after default freeze check
					c.Index += 8;

					// Handle freeze chance
					c.Emit(OpCodes.Ldarg_1);
					c.Emit(OpCodes.Ldarg_0);
					c.EmitDelegate<Action<DamageReport, SetStateOnHurt>>((damageReport, state) =>
					{
						CharacterBody attacker = damageReport.attackerBody;

						if (!attacker.HasBuff(BuffIndex.AffixWhite)) return;
						if (ZetAspectsPlugin.ZetAspectWhiteFreezeChanceCfg.Value <= 0f) return;

						if (attacker == null || attacker.teamComponent.teamIndex != TeamIndex.Player || !state.canBeFrozen || damageReport.damageInfo.procCoefficient < 0.15f) return;

						float count = ZetAspectsPlugin.GetStackMagnitude(attacker, itemIndex);
						float chance = ZetAspectsPlugin.ZetAspectWhiteFreezeChanceCfg.Value * count * damageReport.damageInfo.procCoefficient;
						chance = Util.ConvertAmplificationPercentageIntoReductionPercentage(chance);

						if (Util.CheckRoll(chance)) state.SetFrozen(ZetAspectsPlugin.ZetAspectWhiteFreezeDurationCfg.Value * damageReport.damageInfo.procCoefficient);
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Ice - Freeze Hook Failed");
				}
			};
		}

		private static void SlowHook()
		{
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcR4(1.5f),
					x => x.MatchLdarg(1),
					x => x.MatchLdfld<DamageInfo>("procCoefficient"),
					x => x.MatchMul(),
					x => x.MatchLdloc(8),
					x => x.MatchConvR4(),
					x => x.MatchMul()
				);

				if (found)
				{
					c.Index += 7;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc_0);
					c.Emit(OpCodes.Ldarg_1);
					c.EmitDelegate<Func<CharacterBody, DamageInfo, float>>((self, damageInfo) =>
					{
						float duration = ZetAspectsPlugin.ZetAspectWhiteSlowDurationCfg.Value;
						if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;

						return duration;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Ice - Slow Hook Failed");
				}
			};
		}

		private static void ProjectileHook()
		{
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcR4(1.5f),
					x => x.MatchLdarg(1),
					x => x.MatchLdfld<DamageInfo>("procCoefficient"),
					x => x.MatchMul(),
					x => x.MatchLdloc(8),
					x => x.MatchConvR4(),
					x => x.MatchMul()
				);

				if (found)
				{
					c.Index += 7;

					c.Emit(OpCodes.Ldloc_0);
					c.Emit(OpCodes.Ldarg_1);
					c.EmitDelegate<Action<CharacterBody, DamageInfo>>((self, damageInfo) =>
					{
						if (!self.HasBuff(BuffIndex.AffixWhite)) return;
						if (ZetAspectsPlugin.ZetAspectWhiteBaseDamageCfg.Value <= 0f) return;

						GameObject gameObject = self.gameObject;
						TeamIndex teamIndex = self.teamComponent.teamIndex;

						HurtBox[] hurtBoxes = new SphereSearch
						{
							origin = damageInfo.position,
							radius = 10f,
							mask = LayerIndex.entityPrecise.mask,
							queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
						}.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamIndex)).OrderCandidatesByDistance().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes();

						if (hurtBoxes.Length > 0)
						{
							float damage = Util.OnHitProcDamage(damageInfo.damage, self.damage, 1f);
							float count = ZetAspectsPlugin.GetStackMagnitude(self, itemIndex);

							damage *= ZetAspectsPlugin.ZetAspectWhiteBaseDamageCfg.Value + ZetAspectsPlugin.ZetAspectWhiteStackDamageCfg.Value * (count - 1f);
							if (self.teamComponent.teamIndex != TeamIndex.Player) damage *= ZetAspectsPlugin.ZetAspectEffectMonsterDamageMultCfg.Value;

							LightningOrb lightningOrb = new LightningOrb();
							lightningOrb.attacker = gameObject;
							lightningOrb.bouncedObjects = null;
							lightningOrb.bouncesRemaining = 0;
							lightningOrb.damageCoefficientPerBounce = 1f;
							lightningOrb.damageColorIndex = DamageColorIndex.Item;
							lightningOrb.damageValue = damage;
							lightningOrb.isCrit = damageInfo.crit;
							lightningOrb.lightningType = LightningOrb.LightningType.RazorWire;
							lightningOrb.origin = gameObject.transform.position;
							lightningOrb.procChainMask = default(ProcChainMask);
							lightningOrb.procCoefficient = 0f;
							lightningOrb.range = 0f;
							lightningOrb.teamIndex = teamIndex;
							lightningOrb.target = hurtBoxes[0];
							OrbManager.instance.AddOrb(lightningOrb);
						}
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Ice - Projectile Hook Failed");
				}
			};
		}
	}
}
