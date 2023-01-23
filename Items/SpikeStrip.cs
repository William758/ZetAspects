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

			Sprite outlineSprite;
			if (AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixPlated, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetLanguage = "default";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_PLATED_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_PLATED_PICKUP"));
			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXPLATED_DESCRIPTION", EquipmentDescription(desc, TextFragment("AFFIX_PLATED_ACTIVE")));



			targetLanguage = "pt-BR";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_PLATED_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_PLATED_PICKUP"));
			desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXPLATED_NAME", TextFragment("AFFIX_PLATED_NAME"));
			RegisterToken("EQUIPMENT_AFFIXPLATED_PICKUP", TextFragment("AFFIX_PLATED_PICKUP"));
			RegisterToken("EQUIPMENT_AFFIXPLATED_DESCRIPTION", EquipmentDescription(desc, TextFragment("AFFIX_PLATED_ACTIVE")));



			targetLanguage = "ko";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_PLATED_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_PLATED_PICKUP"));
			desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXPLATED_NAME", TextFragment("AFFIX_PLATED_NAME"));
			RegisterToken("EQUIPMENT_AFFIXPLATED_PICKUP", TextFragment("AFFIX_PLATED_PICKUP"));
			RegisterToken("EQUIPMENT_AFFIXPLATED_DESCRIPTION", EquipmentDescription(desc, TextFragment("AFFIX_PLATED_ACTIVE")));



			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_ENDURANCE");
			output += TextFragment("PASSIVE_DEFENSE_PLATING");
			output += String.Format(
				TextFragment("DAMAGEREDUCTION_ON_HIT"),
				SecondText(8f, "for")
			);
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

			Sprite outlineSprite;
			if (AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixWarped, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetLanguage = "default";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_WARPED_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_WARPED_PICKUP"));

			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXWARPED_DESCRIPTION", EquipmentDescription(desc, TextFragment("AFFIX_WARPED_ACTIVE")));



			targetLanguage = "pt-BR";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_WARPED_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_WARPED_PICKUP"));

			desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXWARPED_NAME", TextFragment("AFFIX_WARPED_NAME"));
			RegisterToken("EQUIPMENT_AFFIXWARPED_PICKUP", TextFragment("AFFIX_WARPED_PICKUP"));
			RegisterToken("EQUIPMENT_AFFIXWARPED_DESCRIPTION", EquipmentDescription(desc, TextFragment("AFFIX_WARPED_ACTIVE")));



			targetLanguage = "ko";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_WARPED_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_WARPED_PICKUP"));

			desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXWARPED_NAME", TextFragment("AFFIX_WARPED_NAME"));
			RegisterToken("EQUIPMENT_AFFIXWARPED_PICKUP", TextFragment("AFFIX_WARPED_PICKUP"));
			RegisterToken("EQUIPMENT_AFFIXWARPED_DESCRIPTION", EquipmentDescription(desc, TextFragment("AFFIX_WARPED_ACTIVE")));



			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			string output = TextFragment("ASPECT_OF_GRAVITY");
			output += TextFragment("PASSIVE_DEFLECT_PROJ");
			output += String.Format(
				TextFragment("LEVITATE_ON_HIT"),
				SecondText(4f, "for")
			);
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

			Sprite outlineSprite;
			if (AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixVeiled, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetLanguage = "default";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_VEILED_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_VEILED_PICKUP"));
			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXVEILED_DESCRIPTION", EquipmentDescription(desc, TextFragment("AFFIX_VEILED_ACTIVE")));



			targetLanguage = "pt-BR";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_VEILED_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_VEILED_PICKUP"));
			desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXVEILED_NAME", TextFragment("AFFIX_VEILED_NAME"));
			RegisterToken("EQUIPMENT_AFFIXVEILED_PICKUP", TextFragment("AFFIX_VEILED_PICKUP"));
			RegisterToken("EQUIPMENT_AFFIXVEILED_DESCRIPTION", EquipmentDescription(desc, TextFragment("AFFIX_VEILED_ACTIVE")));



			targetLanguage = "ko";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_VEILED_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_VEILED_PICKUP"));
			desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXVEILED_NAME", TextFragment("AFFIX_VEILED_NAME"));
			RegisterToken("EQUIPMENT_AFFIXVEILED_PICKUP", TextFragment("AFFIX_VEILED_PICKUP"));
			RegisterToken("EQUIPMENT_AFFIXVEILED_DESCRIPTION", EquipmentDescription(desc, TextFragment("AFFIX_VEILED_ACTIVE")));



			targetLanguage = "";
		}

		public static string BuildDescription(bool combine)
		{
			bool attemptDisableCloak = true;

			string output = TextFragment("ASPECT_OF_OBFUSCATION");

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

			if (!Compat.PlasmaSpikeStrip.cloakHook && attemptDisableCloak) 
			{
				output += TextFragment("CLOAK_ON_HIT");
			}
			else
			{
				float duration = AspectVeiledElusiveDuration.Value;

				if (AspectVeiledCloakOnly.Value)
				{
					if (duration > 0.1f)
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
						if (!attemptDisableCloak) output += TextFragment("CLOAK_ON_HIT");
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

			Sprite outlineSprite;
			if (AspectRedTier.Value) outlineSprite = Catalog.Sprites.OutlineRed;
			else outlineSprite = Catalog.Sprites.OutlineYellow;

			ItemDef itemDef = ScriptableObject.CreateInstance<ItemDef>();
			itemDef.name = identifier;
			itemDef.tags = tags;
			itemDef._itemTierDef = AspectRedTier.Value ? Catalog.RedItemTier : Catalog.BossItemTier;
			itemDef.pickupModelPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/PickupModels/PickupAffixWhite");
			itemDef.pickupIconSprite = Catalog.CreateAspectSprite(Catalog.Sprites.AffixAragonite, outlineSprite);

			itemDef.AutoPopulateTokens();

			return itemDef;
		}

		public static void SetupTokens()
		{
			string locToken = identifier.ToUpperInvariant();

			targetLanguage = "default";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_ARAGONITE_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_ARAGONITE_PICKUP"));
			string desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXARAGONITE_DESCRIPTION", EquipmentDescription(desc, TextFragment("AFFIX_ARAGONITE_ACTIVE")));



			targetLanguage = "pt-BR";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_ARAGONITE_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_ARAGONITE_PICKUP"));
			desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXARAGONITE_NAME", TextFragment("AFFIX_ARAGONITE_NAME"));
			RegisterToken("EQUIPMENT_AFFIXARAGONITE_PICKUP", TextFragment("AFFIX_ARAGONITE_PICKUP"));
			RegisterToken("EQUIPMENT_AFFIXARAGONITE_DESCRIPTION", EquipmentDescription(desc, TextFragment("AFFIX_ARAGONITE_ACTIVE")));



			targetLanguage = "ko";

			RegisterToken("ITEM_" + locToken + "_NAME", TextFragment("AFFIX_ARAGONITE_NAME"));
			RegisterToken("ITEM_" + locToken + "_PICKUP", TextFragment("AFFIX_ARAGONITE_PICKUP"));
			desc = BuildDescription(false);
			RegisterToken("ITEM_" + locToken + "_DESC", desc);
			if (!DropHooks.CanObtainItem()) desc = BuildDescription(true);
			RegisterToken("EQUIPMENT_AFFIXARAGONITE_NAME", TextFragment("AFFIX_ARAGONITE_NAME"));
			RegisterToken("EQUIPMENT_AFFIXARAGONITE_PICKUP", TextFragment("AFFIX_ARAGONITE_PICKUP"));
			RegisterToken("EQUIPMENT_AFFIXARAGONITE_DESCRIPTION", EquipmentDescription(desc, TextFragment("AFFIX_ARAGONITE_ACTIVE")));



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

			if (baseMS == baseAS && stackMS == stackAS)
			{
				if (baseMS > 0f)
				{
					output += String.Format(
						TextFragment("STAT_BOTHSPEED"),
						ScalingText(baseMS, stackMS, "percent", "cIsDamage", combine)
					);
				}

				if (allyMS == allyAS)
				{
					if (allyMS > 0f)
					{
						output += String.Format(
							TextFragment("ANGRY_BOTHSPD"),
							ScalingText(allyMS, "percent", "cIsDamage")
						);
					}
				}
				else
				{
					if (allyMS > 0f)
					{
						output += String.Format(
							TextFragment("ANGRY_MOVSPD"),
							ScalingText(allyMS, "percent", "cIsUtility")
						);
					}
					if (allyAS > 0f)
					{
						output += String.Format(
							TextFragment("ANGRY_ATKSPD"),
							ScalingText(allyAS, "percent", "cIsDamage")
						);
					}
				}
			}
			else
			{
				if (allyMS == allyAS)
				{
					if (baseMS > 0f)
					{
						output += String.Format(
							TextFragment("STAT_MOVESPEED"),
							ScalingText(baseMS, stackMS, "percent", "cIsUtility", combine)
						);
					}
					if (baseAS > 0f)
					{
						output += String.Format(
							TextFragment("STAT_ATTACKSPEED"),
							ScalingText(baseAS, stackAS, "percent", "cIsDamage", combine)
						);
					}
					if (allyMS > 0f)
					{
						output += String.Format(
							TextFragment("ANGRY_BOTHSPD"),
							ScalingText(allyMS, "percent", "cIsDamage")
						);
					}
				}
				else
				{
					if (baseMS > 0f)
					{
						output += String.Format(
							TextFragment("STAT_MOVESPEED"),
							ScalingText(baseMS, stackMS, "percent", "cIsUtility", combine)
						);
					}
					if (allyMS > 0f)
					{
						output += String.Format(
							TextFragment("ANGRY_MOVSPD"),
							ScalingText(allyMS, "percent", "cIsUtility")
						);
					}
					if (baseAS > 0f)
					{
						output += String.Format(
							TextFragment("STAT_ATTACKSPEED"),
							ScalingText(baseAS, stackAS, "percent", "cIsDamage", combine)
						);
					}
					if (allyAS > 0f)
					{
						output += String.Format(
							TextFragment("ANGRY_ATKSPD"),
							ScalingText(allyAS, "percent", "cIsDamage")
						);
					}
				}
			}

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
