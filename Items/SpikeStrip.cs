using System;
using UnityEngine;
using RoR2;

using static TPDespair.ZetAspects.Configuration;
using static TPDespair.ZetAspects.Language;

namespace TPDespair.ZetAspects.Items
{
	public static class ZetAspectPlated
	{
		public static string identifier = "ZetAspectPlated";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };
			if (AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = Catalog.WhiteAspectPrefab;
			itemDef.pickupIconSprite = Catalog.Sprites.AffixUnknown;

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "PLATED";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixPlated;
				if (equipDef)
				{
					RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
					RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
					RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
				}
			}

			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_ENDURANCE");

			if (Compat.NemSpikeStrip.PlatedEnabled)
			{
				string temp = TextFragment("PASSIVE_DEFENSE_PLATING");
				if (!temp.Contains("\n")) temp = "\n" + temp;
				output += temp;
			}
			else
			{
				output += TextFragment("PASSIVE_DEFENSE_PLATING");
			}

			output += String.Format(
				TextFragment("DAMAGEREDUCTION_ON_HIT"),
				SecondText(8f, "for")
			);

			if (Compat.NemSpikeStrip.PlatedEnabled && AspectPlatedPlayerHealthReduction.Value)
			{
				float cfgValue = Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.PlatedHealthField, 0.2f);
				if (cfgValue != 1f)
				{
					output += String.Format(
						TextFragment("STAT_HEALTH_REDUCTION"),
						ScalingText(1f - cfgValue, "percent", "cIsHealth")
					);
				}
			}

			if (!Compat.NemSpikeStrip.PlatedEnabled || AspectPlatedAllowDefenceWithNem.Value)
			{
				if (AspectPlatedBaseArmorGain.Value > 0f)
				{
					output += String.Format(
						TextFragment("STAT_ARMOR"),
						ScalingText(AspectPlatedBaseArmorGain.Value, AspectPlatedStackArmorGain.Value, "flat", "cIsHealing", combine)
					);
				}
				if (AspectPlatedBasePlatingGain.Value > 0f)
				{
					output += String.Format(
						TextFragment("PLATING_EFFECT"),
						ScalingText(AspectPlatedBasePlatingGain.Value, AspectPlatedStackPlatingGain.Value, "flat", "cIsDamage", combine)
					);
					output += TextFragment("PLATING_DETAIL");
				}
			}

			return output;
		}
	}



	public static class ZetAspectWarped
	{
		public static string identifier = "ZetAspectWarped";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };
			if (AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = Catalog.WhiteAspectPrefab;
			itemDef.pickupIconSprite = Catalog.Sprites.AffixUnknown;

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "WARPED";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixWarped;
				if (equipDef)
				{
					RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
					RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
					RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
				}
			}

			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_GRAVITY");
			output += TextFragment("PASSIVE_DEFLECT_PROJ");

			if (AspectWarpedAltDebuff.Value >= 2 && Catalog.antiGrav != BuffIndex.None)
			{
				output += String.Format(
					TextFragment("WARPED_ON_HIT"),
					SecondText(4f, "for")
				);
			}
			else
			{
				float duration = 4f;
				if (Compat.NemSpikeStrip.WarpedEnabled)
				{
					duration = Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.WarpedDurationField, 4f);
				}

				output += String.Format(
					TextFragment("LEVITATE_ON_HIT"),
					SecondText(duration, "for")
				);
			}

			if (AspectWarpedBaseCooldownGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_COOLDOWN"),
					ScalingText(AspectWarpedBaseCooldownGain.Value, AspectWarpedStackCooldownGain.Value, "percent", "cIsUtility", combine)
				);
			}

			float baseValue = AspectWarpedBaseFallReductionGain.Value;
			if (baseValue > 999f)
			{
				output += TextFragment("FALL_IMMUNE");
			}
			else if (baseValue > 0f)
			{
				output += String.Format(
					TextFragment("FALL_REDUCTION"),
					ScalingText(baseValue, AspectWarpedStackFallReductionGain.Value, "percent", "cIsUtility", combine)
				);
			}

			baseValue = AspectWarpedBaseForceResistGain.Value;
			if (baseValue > 999f)
			{
				output += TextFragment("FORCE_IMMUNE");
			}
			else if (baseValue > 0f)
			{
				output += String.Format(
					TextFragment("FORCE_REDUCTION"),
					ScalingText(baseValue, AspectWarpedStackForceResistGain.Value, "percent", "cIsUtility", combine)
				);
			}

			return output;
		}
	}



	public static class ZetAspectVeiled
	{
		public static string identifier = "ZetAspectVeiled";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Healing, ItemTag.Utility };
			if (AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = Catalog.WhiteAspectPrefab;
			itemDef.pickupIconSprite = Catalog.Sprites.AffixUnknown;

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "VEILED";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixVeiled;
				if (equipDef)
				{
					RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
					RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
					RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
				}
			}

			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			bool attemptDisableCloak = true;

			string output = TextFragment("ASPECT_OF_OBFUSCATION");

			bool nemCloak = Compat.NemSpikeStrip.VeiledEnabled && Compat.NemSpikeStrip.GetConfigValue(Compat.NemSpikeStrip.VeiledHitToShowField, true);

			if (nemCloak || AspectVeiledCloakPassive.Value)
			{
				output += TextFragment("CLOAKING_PASSIVE");
				output += TextFragment("DECLOAK_WHEN_HIT");
			}

			if (AspectVeiledBaseDodgeGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("DODGE_CHANCE"),
					ScalingText(AspectVeiledBaseDodgeGain.Value, AspectVeiledStackDodgeGain.Value, "chance", "cIsHealing", combine)
				);

				output += TextFragment("DODGE_DETAIL");
			}

			if (AspectVeiledBaseMovementGain.Value > 0f)
			{
				output += String.Format(
					TextFragment("STAT_MOVESPEED"),
					ScalingText(AspectVeiledBaseMovementGain.Value, AspectVeiledStackMovementGain.Value, "percent", "cIsUtility", combine)
				);
			}

			if (!Compat.PlasmaSpikeStrip.cloakHitHook && attemptDisableCloak && !nemCloak) 
			{
				output += TextFragment("CLOAK_ON_HIT");
			}
			else
			{
				float duration = AspectVeiledElusiveDuration.Value;

				if (AspectVeiledCloakOnly.Value)
				{
					if (nemCloak)
					{
						output += TextFragment("CLOAK_ON_HIT");
					}
					else if (duration > 0.1f)
					{
						output += String.Format(
							TextFragment("CLOAK_ON_HIT_TIMED"),
							SecondText(4f, "for")
						);
					}
				}
				else
				{
					float movSpd = AspectVeiledElusiveMovementGain.Value;
					float dodge = AspectVeiledElusiveDodgeGain.Value;

					if (nemCloak)
					{
						float mult = AspectVeiledElusiveEffectMultWithNem.Value;

						movSpd *= mult;
						dodge *= mult;
					}

					if (duration > 0.1f && ValidElusiveModifier)
					{
						output += TextFragment("ELUSIVE_ON_HIT");

						if (movSpd > 0f && dodge > 0f)
						{
							output += String.Format(
								TextFragment("ELUSIVE_EFFECT_BOTH_DETAIL"),
								ScalingText(movSpd, "percent"),
								ScalingText(dodge, "chance")
							);
						}
						else if (movSpd > 0f)
						{
							output += String.Format(
								TextFragment("ELUSIVE_EFFECT_MOVE_DETAIL"),
								ScalingText(movSpd, "percent")
							);
						}
						else if (dodge > 0f)
						{
							output += String.Format(
								TextFragment("ELUSIVE_EFFECT_DODGE_DETAIL"),
								ScalingText(dodge, "chance")
							);
						}

						output += String.Format(
							TextFragment("ELUSIVE_EFFECT"),
							ScalingText(1f, AspectVeiledElusiveStackEffect.Value, "percent", "cIsUtility", combine)
						);

						if (AspectVeiledElusiveDecay.Value)
						{
							output += String.Format(
								TextFragment("ELUSIVE_DECAY_DETAIL"),
								ScalingText((1f / duration), "percent")
							);
						}
					}
					else
					{
						if (!attemptDisableCloak || nemCloak) output += TextFragment("CLOAK_ON_HIT");
					}
				}
			}

			return output;
		}
	}

	public static class ZetAspectAragonite
	{
		public static string identifier = "ZetAspectAragonite";

		internal static ItemDef DefineItem()
		{
			ItemTag[] tags = { ItemTag.Damage, ItemTag.Utility };
			if (AspectWorldUnique.Value)
			{
				Array.Resize(ref tags, 3);
				tags[2] = ItemTag.WorldUnique;
			}

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = Catalog.WhiteAspectPrefab;
			itemDef.pickupIconSprite = Catalog.Sprites.AffixUnknown;

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();
			string affix = "ARAGONITE";

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

				RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_" + affix + "_NAME"));
				RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_" + affix + "_PICKUP"));
				string desc = BuildDescription(false);
				RegisterToken("ITEM_" + locToken + "_DESC", desc);
				if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);

				EquipmentDef equipDef = EquipDefOf.AffixAragonite;
				if (equipDef)
				{
					RegisterToken(equipDef.nameToken, TextFragment("AFFIX_" + affix + "_NAME"));
					RegisterToken(equipDef.pickupToken, TextFragment("AFFIX_" + affix + "_PICKUP"));
					RegisterToken(equipDef.descriptionToken, EquipmentDescription(desc, TextFragment("AFFIX_" + affix + "_ACTIVE")));
				}
			}

			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			// aura buff not enabled , maybe it just doenst work on player team for some reason ???
			bool auraEnabled = true;

			string output = TextFragment("ASPECT_OF_FURY");

			output += TextFragment("PASSIVE_ANGRY_HIT");
			output += auraEnabled ? TextFragment("PASSIVE_ANGRY_AURA") : TextFragment("PASSIVE_UNKNOWN_AURA");

			float baseMS = 0.6f;
			float stackMS = 0f;
			float baseAS = 0.33f;
			float stackAS = 0f;
			float allyMS = 0.33f;
			float allyAS = 0.33f;

			if (Compat.PlasmaSpikeStrip.rageStatHook)
			{
				baseMS = AspectAragoniteBaseMovementGain.Value;
				stackMS = AspectAragoniteStackMovementGain.Value;
				baseAS = AspectAragoniteBaseAtkSpdGain.Value;
				stackAS = AspectAragoniteStackAtkSpdGain.Value;
				allyMS = AspectAragoniteAllyMovementGain.Value;
				allyAS = AspectAragoniteAllyAtkSpdGain.Value;

				if (Catalog.rageAura == BuffIndex.None)
				{
					allyMS = 0f;
					allyAS = 0f;
				}
			}

			if (!auraEnabled)
			{
				allyMS = 0f;
				allyAS = 0f;
			}

			output += HasteText(baseMS, stackMS, baseAS, stackAS, combine);
			output += HasteAuraText(allyMS, allyAS, false);

			if (Compat.PlasmaSpikeStrip.rageStatHook)
			{
				if (AspectAragoniteBaseCooldownGain.Value > 0f)
				{
					output += String.Format(
						TextFragment("STAT_COOLDOWN"),
						ScalingText(AspectAragoniteBaseCooldownGain.Value, AspectAragoniteStackCooldownGain.Value, "percent", "cIsUtility", combine)
					);
				}
				if (auraEnabled && AspectAragoniteAllyCooldownGain.Value > 0f)
				{
					output += String.Format(
						TextFragment("ANGRY_COOLDOWN"),
						ScalingText(AspectAragoniteAllyCooldownGain.Value, "percent", "cIsUtility")
					);
				}
			}
			else
			{
				output += String.Format(
					TextFragment("STAT_COOLDOWN"),
					ScalingText(0.5f, "percent", "cIsUtility")
				);
				if (auraEnabled)
				{
					output += String.Format(
						TextFragment("ANGRY_COOLDOWN"),
						ScalingText(0.5f, "percent", "cIsDamage")
					);
				}
			}

			return output;
		}
	}
}
