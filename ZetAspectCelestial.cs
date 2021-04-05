using RoR2;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace TPDespair.ZetAspects
{
    public static class ZetAspectCelestial
    {
		public static string identifier = "ZetAspectCelestial";

		internal static void Hooks()
        {
			SlowHook();
			ItemBehaviorHook();
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
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixHauntedIconRed.png");
			}
			else
			{
				sprite = ZetAspectsPlugin.Assets.LoadAsset<Sprite>("Assets/Icons/texAffixHauntedIconYellow.png");
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef.tier = Configuration.AspectRedTier.Value ? ItemTier.Tier3 : ItemTier.Boss;
			itemDef.pickupModelPrefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupAffixHaunted");
			itemDef.pickupIconSprite = sprite;

			itemDef.AutoPopulateTokens();

			string locToken = identifier.ToUpperInvariant();

			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_NAME", "Spectral Circlet");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_PICKUP", "Become an aspect of incorporeality.");
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_DESC", BuildDescription());
			ZetAspectsPlugin.RegisterLanguageToken("ITEM_" + locToken + "_LORE", "...");

			return itemDef;
		}

		public static string BuildDescription()
		{
			string output = "<style=cDeath>Aspect of Incorporeality</style> :";
			if (Configuration.AspectGhostSlowEffect.Value)
			{
				output += "\nAttacks <style=cIsUtility>chill</style> on hit for ";
				output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectWhiteSlowDuration.Value);
				output += ", reducing <style=cIsUtility>movement speed</style> by <style=cIsUtility>80%</style>.";
			}
			if (Configuration.AspectGhostShredDuration.Value > 0f)
			{
				output += "\nAttacks <style=cIsUtility>shred</style> on hit for ";
				output += ZetAspectsPlugin.FormatSeconds(Configuration.AspectGhostShredDuration.Value);
				output += ", reducing <style=cIsUtility>armor</style> by <style=cIsUtility>";
				output += Mathf.Abs(Configuration.AspectGhostShredArmor.Value) + "</style>.";
			}
			if (Configuration.AspectGhostBaseArmorGain.Value > 0f)
			{
				output += "\nIncrease <style=cIsHealing>armor</style> by <style=cIsHealing>";
				output += Configuration.AspectGhostBaseArmorGain.Value + "</style>";
				if (Configuration.AspectGhostStackArmorGain.Value != 0f)
				{
					output += " <style=cStack>(+";
					output += Configuration.AspectGhostStackArmorGain.Value + " per stack)</style>";
				}
				output += ".";
			}
			if (Configuration.AspectGhostAllyArmorGain.Value > 0f)
			{
				output += "\nGrants allies inside its spherical aura <style=cIsHealing>";
				output += Configuration.AspectGhostAllyArmorGain.Value + " armor</style>.";
			}

			return output;
		}

		private static void SlowHook()
		{
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcI4(2),
					x => x.MatchAdd(),
					x => x.MatchStloc(8)
				);

				if (found)
				{
					c.Index += 1;

					// Celestial slow
					c.EmitDelegate<Func<int, int>>((value) =>
					{
						if (!Configuration.AspectGhostSlowEffect.Value) return 0;
						return value;
					});
				}
				else
				{
					Debug.LogWarning("ZetAspect : Celestial - Slow Hook Failed");
				}
			};
		}

		private static void ItemBehaviorHook()
		{
			On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
			{
				self.AddItemBehavior<CharacterBody.AffixHauntedBehavior>(self.HasBuff(RoR2Content.Buffs.AffixHaunted) ? 1 : 0);

				orig(self);
			};
		}

		internal static void ApplyShredded(CharacterBody self, CharacterBody victim, DamageInfo damageInfo)
		{
			if (!self.HasBuff(RoR2Content.Buffs.AffixHaunted)) return;
			if (ZetAspectsContent.Buffs.ZetShredded.buffIndex == BuffIndex.None) return;

			float duration = Configuration.AspectGhostShredDuration.Value;
			if (self.teamComponent.teamIndex != TeamIndex.Player) duration *= damageInfo.procCoefficient;
			if (duration > 0.1f) victim.AddTimedBuff(ZetAspectsContent.Buffs.ZetShredded, duration);
		}
	}
}
