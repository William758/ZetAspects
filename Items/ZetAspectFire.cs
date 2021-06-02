using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class ZetAspectFire
	{
		public static string identifier = "ZetAspectFire";

		internal static void Hooks()
		{
			FiretrailHook();
			BurnApplicationHook();
		}

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Damage, ItemTag.Utility };
			if (!Configuration.AspectRedTier.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			Sprite sprite;
			if (Configuration.AspectRedTier.Value)
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixRedIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixRedIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixRed");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Ifrit's Distinction");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of fire.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Fire</style> :";
			if (Configuration.AspectRedTrail.Value)
			{
				output += "\nLeave behind a fiery trail that damages enemies on contact.";
			}
			if (Configuration.AspectRedExtraJump.Value)
			{
				output += "\nGain <style=cIsUtility>+1</style> maximum <style=cIsUtility>jump count</style>.";
			}
			if (Configuration.AspectRedBaseMovementGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>";
				output += Configuration.AspectRedBaseMovementGain.Value * 100f + "%</style>";
				if (Configuration.AspectRedStackMovementGain.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectRedStackMovementGain.Value * 100f + "% per stack)</style>";
				}
				output += ".";
			}
			if (Configuration.AspectRedBaseDamage.Value > 0f)
			{
				output += "\nAttacks <style=cIsDamage>burn</style> on hit for <style=cIsDamage>";
				output += Configuration.AspectRedBaseDamage.Value * 100f + "%</style>";
				if (Configuration.AspectRedStackDamage.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectRedStackDamage.Value * 100f + "% per stack)</style>";
				}
				output += Configuration.AspectRedUseBase.Value ? " base" : " TOTAL";
				output += " damage over ";
				output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectRedBurnDuration.Value) + ".";
			}

			return output;
		}

		private static void FiretrailHook()
		{
			IL.RoR2.CharacterBody.UpdateFireTrail += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchStloc(0)
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 0);
					c.EmitDelegate<Func<CharacterBody, bool, bool>>((self, hasBuff) =>
					{
						if (self.teamComponent.teamIndex == TeamIndex.Player && !Configuration.AspectRedTrail.Value) return false;

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

		private static void BurnApplicationHook()
		{
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

					// Prevent default
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 0);

					// Handle DOT application
					c.Emit(OpCodes.Ldloc, 0);
					c.Emit(OpCodes.Ldarg, 1);
					c.Emit(OpCodes.Ldarg, 2);
					c.EmitDelegate<Action<CharacterBody, DamageInfo, GameObject>>((self, damageInfo, victim) =>
					{
						// dots tick at the start of duration
						// 4s is 8 ticks - because the 1st tick is at 0s dot only lasts 3.5s
						float ticks, rTicks, damageMult;

						// Burn
						if ((damageInfo.damageType & DamageType.IgniteOnHit) > DamageType.Generic)
						{
							ticks = 2f * 4f * damageInfo.procCoefficient;
							rTicks = Mathf.Ceil(ticks);

							if (rTicks > 0f)
							{
								damageMult = ticks / rTicks;
								damageMult *= rTicks / (rTicks + 1f);
								DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.Burn, (rTicks * 0.5f) + 0.05f, damageMult);
							}
						}

						// Percent Burn
						// 5 ticks per second
						if ((damageInfo.damageType & DamageType.PercentIgniteOnHit) != DamageType.Generic)
						{
							ticks = 5f * 4f * damageInfo.procCoefficient;
							rTicks = Mathf.Ceil(ticks);

							if (rTicks > 0f)
							{
								damageMult = ticks / rTicks;
								damageMult *= rTicks / (rTicks + 1f);
								DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.PercentBurn, (rTicks * 0.2f) + 0.02f, damageMult);
							}
						}

						// Fire Aspect Burn
						if (self.HasBuff(RoR2Content.Buffs.AffixRed) && Configuration.AspectRedBaseDamage.Value > 0f)
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, RoR2Content.Buffs.AffixRed);
							float aspectDamageMult = Configuration.AspectRedBaseDamage.Value + Configuration.AspectRedStackDamage.Value * (count - 1f);
							float aspectDuration = Configuration.AspectRedBurnDuration.Value;

							float ignitePercentBaseDamage = 0f;
							float aspectPercentBaseDamage = 0f;

							if ((damageInfo.damageType & DamageType.PercentIgniteOnHit) != DamageType.Generic)
							{
								ignitePercentBaseDamage = 4f * damageInfo.procCoefficient * 0.5f;
							}

							if (Configuration.AspectRedUseBase.Value)
							{
								aspectPercentBaseDamage = aspectDamageMult;
							}
							else
							{
								aspectPercentBaseDamage = damageInfo.damage / self.damage;
								aspectPercentBaseDamage *= aspectDamageMult;
								if (damageInfo.crit)
								{
									aspectPercentBaseDamage *= 2f;
								}
							}

							if (self.teamComponent.teamIndex != TeamIndex.Player)
							{
								float monsterMult = Configuration.AspectEffectMonsterDamageMult.Value;
								aspectPercentBaseDamage *= monsterMult;
								aspectDuration *= monsterMult;
							}

							// Choose highest damage output
							if (ignitePercentBaseDamage > aspectPercentBaseDamage)
							{
								ticks = 2f * 4f * damageInfo.procCoefficient;
								rTicks = Mathf.Ceil(ticks);

								if (rTicks > 0f)
								{
									damageMult = ticks / rTicks;
									damageMult *= rTicks / (rTicks + 1f);
									DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.Burn, (rTicks * 0.5f) + 0.05f, damageMult);
								}
							}
							else
							{
								ticks = 2f * aspectDuration;
								rTicks = Mathf.Ceil(ticks);

								if (rTicks > 0f)
								{
									damageMult = aspectPercentBaseDamage / aspectDuration / 0.5f;
									damageMult *= ticks / rTicks;
									damageMult *= rTicks / (rTicks + 1f);
									DotController.InflictDot(victim, damageInfo.attacker, DotController.DotIndex.Burn, (rTicks * 0.5f) + 0.05f, damageMult);
									//Debug.LogWarning("BaseDamage : " + self.damage);
									//Debug.LogWarning("TotalDamage : " + damageInfo.damage);
									//Debug.LogWarning("ProcCoeff : " + damageInfo.procCoefficient);
									//Debug.LogWarning("Ticks : " + rTicks + "(+1) [" + (self.damage * 0.25f * damageMult) + "]");
								}
							}

							return;
						}
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Fire - DOT Application Hook Failed");
				}
			};
		}
	}
}
