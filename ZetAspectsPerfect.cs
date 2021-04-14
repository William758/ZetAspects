using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
    public static class ZetAspectPerfect
	{
		public static string identifier = "ZetAspectPerfect";

		internal static void Hooks()
		{
			ProjectileHook();
			CrippleApplyHook();
		}

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };
			if (!Configuration.AspectRedTier.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			Sprite sprite;
			if (Configuration.AspectRedTier.Value)
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixLunarIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixLunarIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixLunar");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Shared Design");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of perfection.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			//ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Perfection</style> :";
			if (Configuration.AspectLunarProjectiles.Value)
			{
				output += "\nPeriodically fire projectiles while in combat.";
			}
			output += "\nAttacks <style=cIsUtility>cripple</style> on hit for ";
			output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectLunarCrippleDuration.Value);
			output += ", reducing <style=cIsUtility>armor</style> by <style=cIsUtility>20</style> and";
			output += " <style=cIsUtility>movement speed</style> by <style=cIsUtility>50%</style>.";
			if (Configuration.AspectLunarBaseMovementGain.Value > 0f)
			{
				output += "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>";
				output += Configuration.AspectLunarBaseMovementGain.Value * 100f + "%</style>";
				if (Configuration.AspectLunarStackMovementGain.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectLunarStackMovementGain.Value * 100f + "% per stack)</style>";
				}
				output += ".";
			}
			output += "\nConvert <style=cIsHealing>100%</style> of health into <style=cIsHealing>regenerating shields</style>.";
			if (Configuration.AspectLunarBaseShieldGain.Value > 0f)
			{
				output += "\nGain <style=cIsHealing>";
				output += Configuration.AspectLunarBaseShieldGain.Value * 100f + "%</style>";
				if (Configuration.AspectLunarStackShieldGain.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectLunarStackShieldGain.Value * 100f + "% per stack)</style>";
				}
				output += " extra <style=cIsHealing>shield</style> from conversion.";
			}
			if (Configuration.AspectLunarRegen.Value > 0f)
			{
				output += "\nAt least <style=cIsHealing>";
				output += Configuration.AspectLunarRegen.Value * 100f + "%</style>";
				output += " of <style=cIsHealing>health regeneration</style> applies to <style=cIsHealing>shields</style>.";
			}

			return output;
		}

		private static void ProjectileHook()
		{
			IL.RoR2.CharacterBody.UpdateAffixLunar += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchCall<CharacterBody>("HasBuff")
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<bool, CharacterBody, bool>>((hasBuff, self) =>
					{
						if (self.teamComponent.teamIndex == TeamIndex.Player && !Configuration.AspectLunarProjectiles.Value) return false;

						return hasBuff;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Perfect - Projectile Hook Failed");
				}
			};
		}

		private static void CrippleApplyHook()
		{
			IL.RoR2.HealthComponent.TakeDamage += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(1),
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("AffixLunar")),
					x => x.MatchCallvirt<CharacterBody>("HasBuff")
				);

				if (found)
				{
					c.Index += 3;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldc_I4, 0);
				}
				else
				{
					Debug.LogWarning("ZetAspect : Perfect - Cripple Apply Hook Failed");
				}
			};
		}

		internal static void ApplyCripple(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!self.HasBuff(RoR2Content.Buffs.AffixLunar)) return;

			float duration = Configuration.AspectLunarCrippleDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
			if (duration > 0.1f) victim.AddTimedBuff(RoR2Content.Buffs.Cripple, duration);
		}
	}
}
