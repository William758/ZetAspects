using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class ZetAspectFire
	{
		public static ItemIndex itemIndex;

		internal static void Init()
		{
			DefineItem();
			SetHook();
		}

		private static void DefineItem()
		{
			string st1 = ZetAspectsPlugin.ZetAspectRedBurnDurationCfg.Value == 1.0f ? "" : "s";
			string lim = ZetAspectsPlugin.ZetAspectRedMovementGainCapCfg.Value > 0f ? " up to <style=cIsUtility>" + (ZetAspectsPlugin.ZetAspectRedMovementGainCapCfg.Value * 100f) + "%</style>" : "";
			string trail = ZetAspectsPlugin.ZetAspectRedTrailCfg.Value ? "\nLeave behind a fiery trail that damages enemies on contact." : "";
			string jump = ZetAspectsPlugin.ZetAspectRedExtraJumpCfg.Value ? "\nGain <style=cIsUtility>+1</style> maximum <style=cIsUtility>jump count</style>." : "";

			ItemTag[] tags = { ItemTag.Damage, ItemTag.Utility };

			if (!ZetAspectsPlugin.ZetAspectRedTierCfg.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = new ItemDef
			{
				name = "ZetAspectFire",
				tier = ZetAspectsPlugin.ZetAspectRedTierCfg.Value ? ItemTier.Tier3 : ItemTier.Boss,
				pickupModelPath = "Prefabs/PickupModels/PickupAffixRed",
				pickupIconPath = "Textures/ItemIcons/texAffixRedIcon",
				nameToken = "Ifrit's Distinction",
				pickupToken = "Become an aspect of fire.",
				descriptionToken = "<style=cDeath>Aspect of Fire</style> :" + trail + jump + "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + (ZetAspectsPlugin.ZetAspectRedBaseMovementGainCfg.Value*100f) + "%</style> <style=cStack>(+" + (ZetAspectsPlugin.ZetAspectRedStackMovementGainCfg.Value * 100f) + "% per stack)</style>" + lim + ".\nAttacks <style=cIsDamage>burn</style> on hit for <style=cIsDamage>" + (ZetAspectsPlugin.ZetAspectRedBaseDamageCfg.Value * 100f) + "%</style> <style=cStack>(+" + (ZetAspectsPlugin.ZetAspectRedStackDamageCfg.Value * 100f) + "% per stack)</style> TOTAL damage over "+ ZetAspectsPlugin.ZetAspectRedBurnDurationCfg.Value + " second"+ st1 + ".",
				loreToken = "Become an aspect of fire.",
				tags = tags
			};

			ItemDisplayRuleDict disp = new ItemDisplayRuleDict(null);

			itemIndex = ItemAPI.Add(new CustomItem(itemDef, disp));
		}

		private static void SetHook()
		{
			// Seperate burn and ignite
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(7),
					x => x.MatchLdloc(7),
					x => x.MatchOr()
				);

				if (found)
				{
					c.Index += 3;

					// Disable default 
					// Cause branch to after ignite code
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 0);

					// Handle DOT application
					c.Emit(OpCodes.Ldloc_0);
					c.Emit(OpCodes.Ldarg_1);
					c.Emit(OpCodes.Ldarg_2);
					c.EmitDelegate<Action<CharacterBody, DamageInfo, GameObject>>((self, damageInfo, victim) =>
					{
						// Burn
						if ((damageInfo.damageType & DamageType.IgniteOnHit) > DamageType.Generic)
						{
							DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.Burn, 4f * damageInfo.procCoefficient, 1f);
						}

						// Percent Ignite
						if (self.HasBuff(BuffIndex.AffixRed))
						{
							float baseDuration = ZetAspectsPlugin.ZetAspectRedBurnDurationCfg.Value;

							float igniteDamage = 0f;
							float aspectDamage = 0f;

							if ((damageInfo.damageType & DamageType.PercentIgniteOnHit) != DamageType.Generic)
							{
							// base damage over duration / 5 ticks
							igniteDamage = 4f * damageInfo.procCoefficient;
							}

							float damage = Util.OnHitProcDamage(damageInfo.damage, self.damage, 1f);
							damage /= self.damage;// Multiplied in DotController
							damage *= 2;// DotDef Interval / Coefficient
							damage /= baseDuration;

							// crit damage multiplier
							damage *= GetCritMult(self, damageInfo);

							float count = ZetAspectsPlugin.GetStackMagnitude(self, itemIndex);
							damage *= ZetAspectsPlugin.ZetAspectRedBaseDamageCfg.Value + ZetAspectsPlugin.ZetAspectRedStackDamageCfg.Value * (count - 1f);

							// halve monster burn duration (and total damage output)
							if (self.teamComponent.teamIndex != TeamIndex.Player) baseDuration *= ZetAspectsPlugin.ZetAspectEffectMonsterDamageMultCfg.Value;

							// base damage over duration / 5 ticks
							aspectDamage = baseDuration * damage;

							// choose highest damage output
							if (igniteDamage > aspectDamage)
							{
								DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.PercentBurn, 4f * damageInfo.procCoefficient, 1f);
							}
							else
							{
								DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.PercentBurn, baseDuration, damage);
							}
						}
						else
						{
							if ((damageInfo.damageType & DamageType.PercentIgniteOnHit) != DamageType.Generic)
							{
								DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.PercentBurn, 4f * damageInfo.procCoefficient, 1f);
							}
						}
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Fire - DOT Application Hook Failed");
				}
			};

			// Remove percent burn damage limit for players
			IL.RoR2.DotController.AddDot += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchCallvirt<HealthComponent>("get_fullCombinedHealth"),
					x => x.MatchLdcR4(0.01f),
					x => x.MatchMul(),
					x => x.MatchCallOrCallvirt<Mathf>("Min")
				);

				if (found)
				{
					c.Index += 2;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc_0);
					c.EmitDelegate<Func<TeamIndex, float>>((teamIndex) =>
					{
						return teamIndex == TeamIndex.Player ? 1f : 0.01f;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Fire - Burn Delimiter Hook Failed");
				}
			};

			// Increase movement speed - additional jump
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(53),
					x => x.MatchLdloc(54),
					x => x.MatchLdloc(55),
					x => x.MatchDiv(),
					x => x.MatchMul(),
					x => x.MatchStloc(53)
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Pop);

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 54);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, value) =>
					{
						if (self.HasBuff(BuffIndex.AffixRed))
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, itemIndex);
							float cap = ZetAspectsPlugin.ZetAspectRedMovementGainCapCfg.Value;
							float addedSpeed = ZetAspectsPlugin.ZetAspectRedBaseMovementGainCfg.Value + ZetAspectsPlugin.ZetAspectRedStackMovementGainCfg.Value * (count - 1f);
							if (cap > 0f) addedSpeed = Mathf.Min(addedSpeed, cap);
							//if (self.teamComponent.teamIndex != TeamIndex.Player) addedSpeed *= 0.5f;
							value += addedSpeed;
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, 54);

					c.Emit(OpCodes.Ldloc, 53);
				}
				else
				{
					Debug.LogWarning("ZetAspect : Fire - Movespeed Hook Failed");
				}

				found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("baseJumpCount"),
					x => x.MatchLdloc(5)
				);

				if (found)
				{
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 5);
					c.EmitDelegate<Func<CharacterBody, int, int>>((self, value) =>
					{
						if (self.HasBuff(BuffIndex.AffixRed))
						{
							if (ZetAspectsPlugin.ZetAspectRedExtraJumpCfg.Value)
							{
								if (self.teamComponent.teamIndex == TeamIndex.Player) value++;
							}
						}

						return value;
					});
					c.Emit(OpCodes.Stloc, 5);
				}
				else
				{
					Debug.LogWarning("ZetAspect : Fire - Jump Hook Failed");
				}
			};

			// Set Whether players leave fire trail
			IL.RoR2.CharacterBody.UpdateFireTrail += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(0)
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldarg_0);
					c.Emit(OpCodes.Ldloc, 0);
					c.EmitDelegate<Func<CharacterBody, bool, bool>>((self, hasBuff) =>
					{
						if (self.teamComponent.teamIndex == TeamIndex.Player && !ZetAspectsPlugin.ZetAspectRedTrailCfg.Value) return false;

						return hasBuff;
					});
					c.Emit(OpCodes.Stloc, 0);
				}
				else
				{
					Debug.LogWarning("ZetAspect : Fire - Firetrail Hook Failed");
				}
			};
		}

		private static float GetCritMult(CharacterBody characterBody,DamageInfo damageInfo)
		{
			if (!damageInfo.crit) return 1f;

			if (characterBody && ZetAspectsPlugin.ZetHypercritEnabledCfg.Value)
			{
				//Amount of crit chance past 100%
				var extraCritChance = Mathf.Max(characterBody.crit - 100f, 0f);
				//Amount of crit chance for the final crit
				var lastCritChance = extraCritChance % 100f;

				float critValue = ZetAspectsPlugin.ZetHypercritBaseCfg.Value;
				float critMult = ZetAspectsPlugin.ZetHypercritMultCfg.Value;

				int extraCrits = Mathf.FloorToInt(extraCritChance / 100f) + (Util.CheckRoll(lastCritChance) ? 1 : 0);
				if (ZetAspectsPlugin.ZetHypercritModeCfg.Value == 1) extraCrits = Math.Min(extraCrits, (int)(100 / critMult));

				switch (ZetAspectsPlugin.ZetHypercritModeCfg.Value)
				{
					case 0:// Linear
						critValue += critMult * extraCrits;
						break;
					case 2:// Asymptotic
						critValue += critMult * (1f - Mathf.Pow(2, -extraCrits / ZetAspectsPlugin.ZetHypercritDecayCfg.Value));
						break;
					default:// Exponential
						critValue *= Mathf.Pow(critMult, extraCrits);
						break;
				}

				return critValue;
			}

			return 2f;
		}
	}
}
