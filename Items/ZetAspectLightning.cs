using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class ZetAspectLightning
	{
		public static GameObject lightningStake = Resources.Load<GameObject>("Prefabs/Projectiles/LightningStake");

		public static string identifier = "ZetAspectLightning";

		internal static void Hooks()
		{
			BombHook();
			ShieldConversionHook();
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
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBlueIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixBlueIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixBlue");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Silence Between Two Strikes");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of lightning.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Lightning</style> :";
			if (Configuration.AspectBlueSappedDuration.Value > 0f)
			{
				output += "\nAttacks <style=cIsUtility>sap</style> on hit for ";
				output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectBlueSappedDuration.Value);
				output += ", reducing <style=cIsUtility>damage</style> by <style=cIsUtility>";
				output += Mathf.Abs(Configuration.AspectBlueSappedDamage.Value) * 100f + "%</style>.";
			}
			output += "\nConvert <style=cIsHealing>";
			output += Configuration.AspectBlueHealthConverted.Value * 100f;
			output += "%</style> of health into <style=cIsHealing>regenerating shields</style>.";
			if (Configuration.AspectBlueBaseShieldGain.Value > 0f)
			{
				output += "\nGain <style=cIsHealing>";
				output += Configuration.AspectBlueBaseShieldGain.Value * 100f + "%</style>";
				if (Configuration.AspectBlueStackShieldGain.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectBlueStackShieldGain.Value * 100f + "% per stack)</style>";
				}
				output += " of health as extra <style=cIsHealing>shield</style>.";
			}
			if (Configuration.AspectBlueBaseDamage.Value > 0f)
			{
				output += "\nAttacks attach a <style=cIsDamage>bomb</style> that explodes for <style=cIsDamage>";
				output += Configuration.AspectBlueBaseDamage.Value * 100f + "%</style>";
				if (Configuration.AspectBlueStackDamage.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectBlueStackDamage.Value * 100f + "% per stack)</style>";
				}
				output += " TOTAL damage after ";
				output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectBlueBombDuration.Value) + ".";
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
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("AffixBlue")),
					x => x.MatchCallvirt<CharacterBody>("HasBuff")
				);

				if (found)
				{
					c.Index += 3;

					// Prevent default
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 0);

					// Handle lightning bomb creation
					c.Emit(OpCodes.Ldloc, 0);
					c.Emit(OpCodes.Ldarg, 1);
					c.EmitDelegate<Action<CharacterBody, DamageInfo>>((self, damageInfo) =>
					{
						if (!self.HasBuff(RoR2Content.Buffs.AffixBlue)) return;
						if (Configuration.AspectBlueBaseDamage.Value <= 0f) return;

						float damage = Util.OnHitProcDamage(damageInfo.damage, self.damage, 1f);
						float count = ZetAspectsPlugin.GetStackMagnitude(self, RoR2Content.Buffs.AffixBlue);

						damage *= Configuration.AspectBlueBaseDamage.Value + Configuration.AspectBlueStackDamage.Value * (count - 1f);
						if (self.teamComponent.teamIndex != TeamIndex.Player) damage *= Configuration.AspectEffectMonsterDamageMult.Value;

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
							fuseOverride = Configuration.AspectBlueBombDuration.Value,
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

		private static void ShieldConversionHook()
		{
			IL.RoR2.CharacterBody.RecalculateStats += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("AffixBlue")),
					x => x.MatchCall<CharacterBody>("HasBuff")
				);

				if (found)
				{
					c.Index += 7;

					// Set health conversion factor
					c.Emit(OpCodes.Pop);
					c.EmitDelegate<Func<float>>(() =>
					{
						return Configuration.AspectBlueHealthConverted.Value;
					});

					c.Index += 11;

					// Add converted health to shield
					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc, 53);
				}
				else
				{
					Debug.LogWarning("ZetAspect : Lightning - Shield Conversion Hook Failed");
				}
			};
		}

		internal static void ApplySapped(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!self.HasBuff(RoR2Content.Buffs.AffixBlue)) return;
			if (ZetAspectsContent.Buffs.ZetSapped.buffIndex == BuffIndex.None) return;

			float duration = Configuration.AspectBlueSappedDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
			if (duration > 0.1f) victim.AddTimedBuff(ZetAspectsContent.Buffs.ZetSapped, duration);
		}
	}
}
