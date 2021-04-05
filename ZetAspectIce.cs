using RoR2;
using RoR2.Orbs;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
    public static class ZetAspectIce
    {
		public static string identifier = "ZetAspectIce";

		internal static void Hooks()
		{
			FreezeHook();
			SlowHook();
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
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixWhiteIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixWhiteIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Her Biting Embrace");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of ice.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Ice</style> :\nAttacks <style=cIsUtility>chill</style> on hit for ";
			output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectWhiteSlowDuration.Value);
			output += ", reducing <style=cIsUtility>movement speed</style> by <style=cIsUtility>80%</style>.";
			if (Configuration.AspectWhiteFreezeChance.Value > 0f)
			{
				output += "\nAttacks have a <style=cIsUtility>";
				output += Configuration.AspectWhiteFreezeChance.Value;
				output += "%</style> <style=cStack>(+";
				output += Configuration.AspectWhiteFreezeChance.Value;
				output += "% per stack)</style> chance to <style=cIsUtility>freeze</style> for ";
				output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectWhiteFreezeDuration.Value) + ".";
			}
			if (Configuration.AspectWhiteBaseDamage.Value > 0f)
			{
				output += "\nAttacks fire a <style=cIsDamage>blade</style> that deals <style=cIsDamage>";
				output += Configuration.AspectWhiteBaseDamage.Value * 100f + "%</style>";
				if (Configuration.AspectWhiteStackDamage.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectWhiteStackDamage.Value * 100f + "% per stack)</style>";
				}
				output += " TOTAL damage.";
			}

			return output;
		}

		private static void FreezeHook()
		{
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
					c.Emit(OpCodes.Ldarg, 1);
					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Action<DamageReport, SetStateOnHurt>>((damageReport, state) =>
					{
						CharacterBody attacker = damageReport.attackerBody;

						if (attacker == null) return;

						if (!attacker.HasBuff(RoR2Content.Buffs.AffixWhite) || Configuration.AspectWhiteFreezeChance.Value <= 0f) return;
						if (!state.canBeFrozen || attacker.teamComponent.teamIndex != TeamIndex.Player || damageReport.damageInfo.procCoefficient < 0.105f) return;

						float count = ZetAspectsPlugin.GetStackMagnitude(attacker, RoR2Content.Buffs.AffixWhite);
						float chance = Configuration.AspectWhiteFreezeChance.Value * count * damageReport.damageInfo.procCoefficient;
						chance = Util.ConvertAmplificationPercentageIntoReductionPercentage(chance);

						if (Util.CheckRoll(chance)) state.SetFrozen(Configuration.AspectWhiteFreezeDuration.Value * damageReport.damageInfo.procCoefficient);
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
					c.Emit(OpCodes.Ldloc, 0);
					c.Emit(OpCodes.Ldarg, 1);
					c.EmitDelegate<Func<CharacterBody, DamageInfo, float>>((self, damageInfo) =>
					{
						float duration = Configuration.AspectWhiteSlowDuration.Value;
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

		internal static void FireFrostBlade(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!self.HasBuff(RoR2Content.Buffs.AffixWhite)) return;
			if (Configuration.AspectWhiteBaseDamage.Value <= 0f) return;

			GameObject gameObject = self.gameObject;
			TeamIndex teamIndex = self.teamComponent.teamIndex;

			float damage = Util.OnHitProcDamage(damageInfo.damage, self.damage, 1f);
			float count = ZetAspectsPlugin.GetStackMagnitude(self, RoR2Content.Buffs.AffixWhite);

			damage *= Configuration.AspectWhiteBaseDamage.Value + Configuration.AspectWhiteStackDamage.Value * (count - 1f);
			if (self.teamComponent.teamIndex != TeamIndex.Player) damage *= Configuration.AspectEffectMonsterDamageMult.Value;

            LightningOrb lightningOrb = new LightningOrb
            {
                attacker = gameObject,
                bouncedObjects = null,
                bouncesRemaining = 0,
                damageCoefficientPerBounce = 1f,
                damageColorIndex = DamageColorIndex.Item,
                damageValue = damage,
                isCrit = damageInfo.crit,
                lightningType = LightningOrb.LightningType.RazorWire,
                origin = gameObject.transform.position,
                procChainMask = default,
                procCoefficient = 0f,
                range = 0f,
                teamIndex = teamIndex,
                target = victim.mainHurtBox
            };
            OrbManager.instance.AddOrb(lightningOrb);
		}
	}
}
