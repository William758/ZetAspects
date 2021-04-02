using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
    public static class ZetAspectMalachite
    {
		public static string identifier = "ZetAspectMalachite";

		internal static void Hooks()
        {
			SpikeballHook();
			NullDurationHook();
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
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPoisonIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixPoisonIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixPoison");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			EnigmaticThunder.Modules.Languages.Add("ITEM_" + locToken + "_NAME", "N'kuhana's Retort");
			EnigmaticThunder.Modules.Languages.Add("ITEM_" + locToken + "_PICKUP", "Become an aspect of corruption.");
			EnigmaticThunder.Modules.Languages.Add("ITEM_" + locToken + "_DESC", BuildDescription());
			EnigmaticThunder.Modules.Languages.Add("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Corruption</style> :";
			if (Configuration.AspectPoisonFireSpikes.Value)
			{
				output += "\nPeriodically releases spiked balls that sprout spike pits from where they land.";
			}
			output += "\nAttacks <style=cIsUtility>nullify</style> on hit for ";
			output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectPoisonNullDuration.Value);
			if (Configuration.AspectPoisonNullDamageTaken.Value != 0f)
			{
				output += ", increasing <style=cIsUtility>damage taken</style> by <style=cIsUtility>";
				output += Mathf.Abs(Configuration.AspectPoisonNullDamageTaken.Value) * 100f + "%</style>";
			}
			output += ".";
			if (Configuration.AspectPoisonBaseHealthGain.Value > 0f)
			{
				output += "\nIncrease <style=cIsHealing>maximum health</style> by <style=cIsHealing>";
				output += Configuration.AspectPoisonBaseHealthGain.Value + "</style>";
				if (Configuration.AspectPoisonStackHealthGain.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectPoisonStackHealthGain.Value + " per stack)</style>";
				}
				output += ".";
			}
			if (Configuration.AspectPoisonBaseHeal.Value > 0)
			{
				output += "\nDealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>";
				output += Configuration.AspectPoisonBaseHeal.Value + "</style>";
				if (Configuration.AspectPoisonStackHeal.Value != 0)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectPoisonStackHeal.Value + " per stack)</style>";
				}
				output += " <style=cIsHealing>health</style>.";
			}

			return output;
		}

		private static void SpikeballHook()
		{
			IL.RoR2.CharacterBody.UpdateAffixPoison += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdarg(0),
					x => x.MatchLdarg(0),
					x => x.MatchLdfld<CharacterBody>("poisonballTimer"),
					x => x.MatchLdarg(1),
					x => x.MatchAdd(),
					x => x.MatchStfld<CharacterBody>("poisonballTimer")
				);

				if (found)
				{
					c.Index += 4;

					c.Emit(OpCodes.Ldarg, 0);
					c.EmitDelegate<Func<float, CharacterBody, float>>((deltaTime, self) =>
					{
						if (self.teamComponent.teamIndex == TeamIndex.Player && !Configuration.AspectPoisonFireSpikes.Value) return 0f;

						return deltaTime;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Malachite - Spikeball Hook Failed");
				}
			};
		}

		private static void NullDurationHook()
		{
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdloc(1),
					x => x.MatchLdsfld(typeof(RoR2Content.Buffs).GetField("HealingDisabled")),
					x => x.MatchLdcR4(8f),
					x => x.MatchLdarg(1),
					x => x.MatchLdfld<DamageInfo>("procCoefficient"),
					x => x.MatchMul()
				);

				if (found)
				{
					c.Index += 6;

					c.Emit(OpCodes.Pop);
					c.Emit(OpCodes.Ldloc, 0);
					c.Emit(OpCodes.Ldarg, 1);
					c.EmitDelegate<Func<CharacterBody, DamageInfo, float>>((self, damageInfo) =>
					{
						float duration = Configuration.AspectPoisonNullDuration.Value;
						if (self.teamComponent.teamIndex != TeamIndex.Player) duration = 8f * damageInfo.procCoefficient;

						return duration;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Malachite - Nullified Duration Hook Failed");
				}
			};
		}
	}
}
