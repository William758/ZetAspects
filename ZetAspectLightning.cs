using RoR2;
using RoR2.Projectile;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class ZetAspectLightning
	{
		public static ItemIndex itemIndex;

		public static GameObject lightningStake = Resources.Load<GameObject>("Prefabs/Projectiles/LightningStake");

		internal static void Init()
		{
			DefineItem();
			BombHook();
			SappedDamageHook();
			ShieldGainHook();
			ShieldConversionHook();
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
				name = "ZetAspectLightning",
				tier = ZetAspectsPlugin.ZetAspectRedTierCfg.Value ? ItemTier.Tier3 : ItemTier.Boss,
				pickupModelPath = "Prefabs/PickupModels/PickupAffixBlue",
				pickupIconPath = "Textures/ItemIcons/texAffixBlueIcon",
				nameToken = "Silence Between Two Strikes",
				pickupToken = "Become an aspect of lightning.",
				descriptionToken = BuildDescription(),
				loreToken = "Become an aspect of lightning.",
				tags = tags
			};

			ItemDisplayRuleDict disp = new ItemDisplayRuleDict(null);

			itemIndex = ItemAPI.Add(new CustomItem(itemDef, disp));
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Lightning</style> :";
			if (ZetAspectsPlugin.ZetAspectBlueSappedDurationCfg.Value > 0f)
            {
				output += "\nAttacks <style=cIsUtility>sap</style> on hit for ";
				output += ZetAspectsPlugin.FormatSeconds(ZetAspectsPlugin.ZetAspectBlueSappedDurationCfg.Value);
				output += ", reducing <style=cIsUtility>damage</style> by <style=cIsUtility>";
				output += Mathf.Abs(ZetAspectsPlugin.ZetAspectBlueSappedDamageCfg.Value) * 100f + "%</style>.";
			}
			output += "\nConvert <style=cIsHealing>";
			output += ZetAspectsPlugin.ZetAspectBlueHealthConvertedCfg.Value * 100f;
			output += "%</style> of health into <style=cIsHealing>regenerating shields</style>.";
			if (ZetAspectsPlugin.ZetAspectBlueBaseShieldGainCfg.Value > 0f)
			{
				output += "\nGain <style=cIsHealing>";
				output += ZetAspectsPlugin.ZetAspectBlueBaseShieldGainCfg.Value * 100f + "%</style>";
				if (ZetAspectsPlugin.ZetAspectBlueStackShieldGainCfg.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += ZetAspectsPlugin.ZetAspectBlueStackShieldGainCfg.Value * 100f + "% per stack)</style>";
				}
				output += " of health as extra <style=cIsHealing>shield</style>.";
			}
			if (ZetAspectsPlugin.ZetAspectBlueBaseDamageCfg.Value > 0f)
			{
				output += "\nAttacks attach a <style=cIsDamage>bomb</style> that explodes for <style=cIsDamage>";
				output += ZetAspectsPlugin.ZetAspectBlueBaseDamageCfg.Value * 100f + "%</style>";
				if (ZetAspectsPlugin.ZetAspectBlueStackDamageCfg.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += ZetAspectsPlugin.ZetAspectBlueStackDamageCfg.Value * 100f + "% per stack)</style>";
				}
				output += " TOTAL damage after ";
				output += ZetAspectsPlugin.FormatSeconds(ZetAspectsPlugin.ZetAspectBlueBombDurationCfg.Value) + ".";
			}

			return output;
		}

		private static void BombHook()
		{
			IL.RoR2.GlobalEventManager.OnHitAll += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(0),
					x => x.MatchLdcI4(0x1C),
					x => x.MatchCallvirt<CharacterBody>("HasBuff")
				);

				if (found)
				{
					c.Index += 3;

					// prevent default
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 0);

					// Handle lightning bomb creation
					c.Emit(OpCodes.Ldloc_0);
					c.Emit(OpCodes.Ldarg_1);
					c.EmitDelegate<Action<CharacterBody, DamageInfo>>((self, damageInfo) =>
					{
						if (!self.HasBuff(BuffIndex.AffixBlue)) return;
						if (ZetAspectsPlugin.ZetAspectBlueBaseDamageCfg.Value <= 0f) return;

						float damage = Util.OnHitProcDamage(damageInfo.damage, self.damage, 1f);
						float count = ZetAspectsPlugin.GetStackMagnitude(self, itemIndex);

						damage *= ZetAspectsPlugin.ZetAspectBlueBaseDamageCfg.Value + ZetAspectsPlugin.ZetAspectBlueStackDamageCfg.Value * (count - 1f);
						if (self.teamComponent.teamIndex != TeamIndex.Player) damage *= ZetAspectsPlugin.ZetAspectEffectMonsterDamageMultCfg.Value;

						FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
						{
							projectilePrefab = lightningStake,
							position = damageInfo.position,
							rotation = Quaternion.identity,
							owner = damageInfo.attacker,
							damage = damage,
							force = 0f,
							crit = damageInfo.crit,
							damageColorIndex = DamageColorIndex.Item,
							target = null,
							speedOverride = -1f,
							fuseOverride = ZetAspectsPlugin.ZetAspectBlueBombDurationCfg.Value,
							damageTypeOverride = null
						};
						ProjectileManager.instance.FireProjectile(fireProjectileInfo);
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Lightning - Bomb Hook Failed");
				}
			};
		}

		private static void SappedDamageHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(57),
					x => x.MatchLdloc(58),
					x => x.MatchMul(),
					x => x.MatchStloc(57)
				);

				if (found)
				{
					c.Index += 4;

					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 57);
					c.EmitDelegate<Func<CharacterBody, float, float>>((self, damage) =>
					{
						if (self.HasBuff(ZetAspectsPlugin.ZetSappedDebuff))
						{
							float addedDamage = -Mathf.Abs(ZetAspectsPlugin.ZetAspectBlueSappedDamageCfg.Value);
							//if (self.teamComponent.teamIndex == TeamIndex.Player) addedDamage *= 0.5f;
							addedDamage = Mathf.Max(-0.99f, addedDamage);
							return damage * (1f + addedDamage);
						}
						else
						{
							return damage;
						}
					});
					c.Emit(OpCodes.Stloc, 57);
				}
				else
				{
					Debug.LogWarning("ZetAspect : Lightning - Sapped Damage Hook Failed");
				}
			};
		}

		private static void ShieldGainHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchMul(),
					x => x.MatchAdd(),
					x => x.MatchStloc(43)
				);

				if (found)
				{
					c.Index += 3;

					// Add extra shield
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Ldloc, 43);
					c.Emit(OpCodes.Ldarg, 0);
					c.Emit(OpCodes.Callvirt, typeof(CharacterBody).GetMethod("get_maxHealth"));
					c.EmitDelegate<Func<CharacterBody, float, float, float>>((self, shield, health) =>
					{
						if (self.HasBuff(BuffIndex.AffixBlue) && ZetAspectsPlugin.ZetAspectBlueBaseShieldGainCfg.Value > 0f)
						{
							float count = ZetAspectsPlugin.GetStackMagnitude(self, itemIndex);
							float addedShield = health * (ZetAspectsPlugin.ZetAspectBlueBaseShieldGainCfg.Value + ZetAspectsPlugin.ZetAspectBlueStackShieldGainCfg.Value * (count - 1f));
							//if (self.teamComponent.teamIndex != TeamIndex.Player) addedShield *= 0.5f;
							shield += addedShield;
						}

						return shield;
					});
					c.Emit(OpCodes.Stloc, 43);
				}
				else
				{
					Debug.LogWarning("ZetAspect : Lightning - Shield Gain Hook Failed");
				}
			};
		}

		private static void ShieldConversionHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdcI4(0x1C),
					x => x.MatchCallvirt<CharacterBody>("HasBuff")
				);

				if (found)
				{
					c.Index += 7;

					// Set health conversion factor
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_R4, ZetAspectsPlugin.ZetAspectBlueHealthConvertedCfg.Value);

					c.Index += 11;

					// Add converted health to shield
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc, 44);
				}
				else
				{
					Debug.LogWarning("ZetAspect : Lightning - Shield Conversion Hook Failed");
				}
			};
		}
	}
}
