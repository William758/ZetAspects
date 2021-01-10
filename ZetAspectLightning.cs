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
			SetHook();
		}

		private static void DefineItem()
		{
			string st1 = ZetAspectsPlugin.ZetAspectBlueBombDurationCfg.Value == 1.0f ? "" : "s";
			string st2 = ZetAspectsPlugin.ZetAspectBlueSappedDurationCfg.Value == 1.0f ? "" : "s";

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
				descriptionToken = "<style=cDeath>Aspect of Lightning</style> :\nAttacks <style=cIsUtility>sap</style> on hit for " + ZetAspectsPlugin.ZetAspectBlueSappedDurationCfg.Value + " second" + st2 + ", reducing <style=cIsUtility>damage</style> by <style=cIsUtility>"+ (Mathf.Abs(ZetAspectsPlugin.ZetAspectBlueSappedDamageCfg.Value) * 100f) + "%</style>.\n<style=cIsHealing>Convert " + (ZetAspectsPlugin.ZetAspectBlueHealthConvertedCfg.Value * 100f) + "%</style> of <style=cIsHealing>health</style> into <style=cIsHealing>regenerating shields</style>.\nGain <style=cIsHealing>" + (ZetAspectsPlugin.ZetAspectBlueBaseShieldGainCfg.Value * 100f) + "%</style> <style=cStack>(+" + (ZetAspectsPlugin.ZetAspectBlueStackShieldGainCfg.Value * 100f) + "% per stack)</style> of health as extra <style=cIsHealing>shield</style>.\nAttacks attach a <style=cIsDamage>bomb</style> that explodes for <style=cIsDamage>" + (ZetAspectsPlugin.ZetAspectBlueBaseDamageCfg.Value * 100f) + "%</style> <style=cStack>(+" + (ZetAspectsPlugin.ZetAspectBlueStackDamageCfg.Value * 100f) + "% per stack)</style> TOTAL damage after " + ZetAspectsPlugin.ZetAspectBlueBombDurationCfg.Value + " second" + st1 + ".",
				loreToken = "Become an aspect of lightning.",
				tags = tags
			};

			ItemDisplayRuleDict disp = new ItemDisplayRuleDict(null);

			itemIndex = ItemAPI.Add(new CustomItem(itemDef, disp));
		}

		private static void SetHook()
		{
			// Lightning bomb creation
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

			// Convert health and increase shield - damage reduction
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				// - Gain health as extra shield
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
						if (self.HasBuff(BuffIndex.AffixBlue))
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
					Debug.LogWarning("ZetAspect : Lightning - Health As Extra Shield Hook Failed");
				}

				// - Change shield conversion
				found = c.TryGotoNext(
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

				// - Sapped damage effect
				found = c.TryGotoNext(
					x => x.MatchLdloc(57),
					x => x.MatchLdloc(58),
					x => x.MatchMul(),
					x => x.MatchStloc(57)
				);

				if (found)
				{
					c.Index += 4;

					// reduce damage stat
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
	}
}
