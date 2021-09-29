using BepInEx.Configuration;

namespace TPDespair.ZetAspects
{
	public static class Configuration
	{
		public static ConfigEntry<bool> AspectRedTier { get; set; }
		public static ConfigEntry<bool> AspectWorldUnique { get; set; }
		public static ConfigEntry<float> AspectDropChance { get; set; }
		public static ConfigEntry<float> AspectDropChanceDecay { get; set; }
		public static ConfigEntry<float> AspectDropChanceDecayLimit { get; set; }
		public static ConfigEntry<bool> AspectDropChanceCompensation { get; set; }
		public static ConfigEntry<bool> AspectShowDropText { get; set; }
		public static ConfigEntry<string> AspectDropText { get; set; }
		public static ConfigEntry<bool> AspectSkinApply { get; set; }


		public static ConfigEntry<bool> AspectEliteEquipment { get; set; }
		public static ConfigEntry<bool> AspectAbilitiesEliteEquipment { get; set; }
		public static ConfigEntry<float> AspectEquipmentEffect { get; set; }
		public static ConfigEntry<bool> AspectEquipmentAbsorb { get; set; }
		public static ConfigEntry<bool> AspectEquipmentConversion { get; set; }
		public static ConfigEntry<bool> AspectSkinEquipmentPriority { get; set; }


		public static ConfigEntry<int> LeechSeedHeal { get; set; }
		public static ConfigEntry<float> TranscendenceRegen { get; set; }


		public static ConfigEntry<int> HeadHunterCountExtra { get; set; }
		public static ConfigEntry<float> HeadHunterExtraEffect { get; set; }
		public static ConfigEntry<float> HeadHunterBaseDuration { get; set; }
		public static ConfigEntry<float> HeadHunterStackDuration { get; set; }


		public static ConfigEntry<bool> HeadHunterBuffEnable { get; set; }
		public static ConfigEntry<float> HeadHunterBuffHealth { get; set; }
		public static ConfigEntry<float> HeadHunterBuffArmor { get; set; }
		public static ConfigEntry<float> HeadHunterBuffMovementSpeed { get; set; }
		public static ConfigEntry<float> HeadHunterBuffDamage { get; set; }
		public static ConfigEntry<float> HeadHunterBuffAttackSpeed { get; set; }
		public static ConfigEntry<float> HeadHunterBuffCritChance { get; set; }


		public static ConfigEntry<float> AspectEffectMonsterDamageMult { get; set; }
		public static ConfigEntry<float> AspectEffectPlayerDebuffMult { get; set; }


		public static ConfigEntry<float> AspectWhiteFreezeChance { get; set; }
		public static ConfigEntry<float> AspectWhiteFreezeDuration { get; set; }
		public static ConfigEntry<float> AspectWhiteSlowDuration { get; set; }
		public static ConfigEntry<float> AspectWhiteBaseDamage { get; set; }
		public static ConfigEntry<float> AspectWhiteStackDamage { get; set; }


		public static ConfigEntry<float> AspectBlueSappedDuration { get; set; }
		public static ConfigEntry<float> AspectBlueSappedDamage { get; set; }
		public static ConfigEntry<float> AspectBlueHealthConverted { get; set; }
		public static ConfigEntry<float> AspectBlueBaseShieldGain { get; set; }
		public static ConfigEntry<float> AspectBlueStackShieldGain { get; set; }
		public static ConfigEntry<float> AspectBlueBombDuration { get; set; }
		public static ConfigEntry<float> AspectBlueBaseDamage { get; set; }
		public static ConfigEntry<float> AspectBlueStackDamage { get; set; }


		public static ConfigEntry<bool> AspectRedTrail { get; set; }
		public static ConfigEntry<bool> AspectRedExtraJump { get; set; }
		public static ConfigEntry<float> AspectRedBaseMovementGain { get; set; }
		public static ConfigEntry<float> AspectRedStackMovementGain { get; set; }
		public static ConfigEntry<bool> AspectRedUseBase { get; set; }
		public static ConfigEntry<float> AspectRedBurnDuration { get; set; }
		public static ConfigEntry<float> AspectRedBaseDamage { get; set; }
		public static ConfigEntry<float> AspectRedStackDamage { get; set; }


		public static ConfigEntry<bool> AspectGhostSlowEffect { get; set; }
		public static ConfigEntry<float> AspectGhostShredDuration { get; set; }
		public static ConfigEntry<float> AspectGhostShredArmor { get; set; }
		public static ConfigEntry<float> AspectGhostAllyArmorGain { get; set; }
		public static ConfigEntry<float> AspectGhostBaseArmorGain { get; set; }
		public static ConfigEntry<float> AspectGhostStackArmorGain { get; set; }


		public static ConfigEntry<bool> AspectPoisonFireSpikes { get; set; }
		public static ConfigEntry<float> AspectPoisonNullDuration { get; set; }
		public static ConfigEntry<float> AspectPoisonNullDamageTaken { get; set; }
		public static ConfigEntry<float> AspectPoisonBaseHealthGain { get; set; }
		public static ConfigEntry<float> AspectPoisonStackHealthGain { get; set; }
		public static ConfigEntry<int> AspectPoisonBaseHeal { get; set; }
		public static ConfigEntry<int> AspectPoisonStackHeal { get; set; }


		public static ConfigEntry<bool> AspectLunarProjectiles { get; set; }
		public static ConfigEntry<float> AspectLunarCrippleDuration { get; set; }
		public static ConfigEntry<float> AspectLunarRegen { get; set; }
		public static ConfigEntry<float> AspectLunarBaseMovementGain { get; set; }
		public static ConfigEntry<float> AspectLunarStackMovementGain { get; set; }
		public static ConfigEntry<float> AspectLunarBaseShieldGain { get; set; }
		public static ConfigEntry<float> AspectLunarStackShieldGain { get; set; }


		public static ConfigEntry<float> AspectArmorBaseArmorGain { get; set; }
		public static ConfigEntry<float> AspectArmorStackArmorGain { get; set; }


		public static ConfigEntry<float> AspectBannerBaseAttackSpeedGain { get; set; }
		public static ConfigEntry<float> AspectBannerStackAttackSpeedGain { get; set; }


		public static ConfigEntry<float> AspectImpaleBaseDotAmp { get; set; }
		public static ConfigEntry<float> AspectImpaleStackDotAmp { get; set; }


		public static ConfigEntry<float> AspectGoldenBaseRegenGain { get; set; }
		public static ConfigEntry<float> AspectGoldenStackRegenGain { get; set; }
		public static ConfigEntry<float> AspectGoldenItemScoreFactor { get; set; }
		public static ConfigEntry<float> AspectGoldenItemScoreExponent { get; set; }
		public static ConfigEntry<float> AspectGoldenBaseScoredRegenGain { get; set; }
		public static ConfigEntry<float> AspectGoldenStackScoredRegenGain { get; set; }


		public static ConfigEntry<float> AspectCycloneBaseMovementGain { get; set; }
		public static ConfigEntry<float> AspectCycloneStackMovementGain { get; set; }


		public static ConfigEntry<float> AspectTinkerBaseDamageResistGain { get; set; }
		public static ConfigEntry<float> AspectTinkerStackDamageResistGain { get; set; }


		public static ConfigEntry<float> AspectLeechingBaseLeechGain { get; set; }
		public static ConfigEntry<float> AspectLeechingStackLeechGain { get; set; }
		public static ConfigEntry<float> AspectLeechingMonsterLeechMult { get; set; }


		public static ConfigEntry<float> AspectFrenziedBaseMovementGain { get; set; }
		public static ConfigEntry<float> AspectFrenziedStackMovementGain { get; set; }
		public static ConfigEntry<float> AspectFrenziedMonsterMovementMult { get; set; }
		public static ConfigEntry<float> AspectFrenziedBaseAttackSpeedGain { get; set; }
		public static ConfigEntry<float> AspectFrenziedStackAttackSpeedGain { get; set; }
		public static ConfigEntry<float> AspectFrenziedMonsterAttackSpeedMult { get; set; }
		public static ConfigEntry<float> AspectFrenziedBaseCooldownGain { get; set; }
		public static ConfigEntry<float> AspectFrenziedStackCooldownGain { get; set; }
		public static ConfigEntry<float> AspectFrenziedMonsterCooldownMult { get; set; }


		public static ConfigEntry<float> AspectSanguineBaseDotAmp { get; set; }
		public static ConfigEntry<float> AspectSanguineStackDotAmp { get; set; }
		public static ConfigEntry<float> AspectSanguineBleedDuration { get; set; }
		public static ConfigEntry<float> AspectSanguineBaseDamage { get; set; }
		public static ConfigEntry<float> AspectSanguineStackDamage { get; set; }


		internal static void Init(ConfigFile Config)
		{
			GeneralConfigs(Config);
			HeadHunterConfigs(Config);
			AspectConfigs(Config);
		}


		private static void GeneralConfigs(ConfigFile Config)
		{
			AspectRedTier = Config.Bind(
				"0a-General", "aspectRedTier", false,
				"Make aspects red items, otherwise yellow."
			);
			AspectWorldUnique = Config.Bind(
				"0a-General", "aspectWorldUnique", true,
				"Make aspects only drop from their respective elites and no where else."
			);
			AspectDropChance = Config.Bind(
				"0a-General", "aspectDropChance", 0.2f,
				"Percent chance that an elite drops its aspect. 0.2 is 0.2%"
			);
			AspectDropChanceDecay = Config.Bind(
				"0a-General", "aspectDropChanceDecay", 0.5f,
				"Multiply drop chance for every aspect that has dropped in a run."
			);
			AspectDropChanceDecayLimit = Config.Bind(
				"0a-General", "aspectDropChanceDecayLimit", 0.25f,
				"Drop chance decay lower limit. Drop chance decay multiplier will not go below this value."
			);
			AspectDropChanceCompensation = Config.Bind(
				"0a-General", "aspectDropChanceCompensation", true,
				"Greatly increase drop chance at certain dropcount vs stagecount thresholds."
			);
			AspectShowDropText = Config.Bind(
				"0a-General", "aspectEnableDropText", true,
				"Display a chat message when an elite drops an aspect."
			);
			AspectDropText = Config.Bind(
				"0a-General", "aspectDropText", "A slain enemy leaves some of its power behind...",
				"Chat message for when an elite drops an aspect."
			);
			AspectSkinApply = Config.Bind(
				"0a-General", "aspectEliteSkin", true,
				"Whether to apply elite skin with new items."
			);


			AspectEliteEquipment = Config.Bind(
				"0b-Equipment", "eliteEquipmentDrop", false,
				"Elites drop equipment instead of new items."
			);
			AspectAbilitiesEliteEquipment = Config.Bind(
				"0b-Equipment", "aspectAbilitiesForceEquipmentDrop", true,
				"Aspect Abilities forces elites to drop equipment."
			);
			AspectEquipmentEffect = Config.Bind(
				"0b-Equipment", "eliteEquipmentEffect", 3f,
				"Count equipment as this much stacks of aspects for players."
			);
			AspectEquipmentAbsorb = Config.Bind(
				"0b-Equipment", "eliteEquipmentAbsorb", true,
				"Picking up the same elite equipment will absorb as non-equipment item."
			);
			AspectEquipmentConversion = Config.Bind(
				"0b-Equipment", "eliteEquipmentConversion", true,
				"Clicking bottom-right equipment icon will convert into non-equipment item."
			);
			AspectSkinEquipmentPriority = Config.Bind(
				"0b-Equipment", "eliteEquipmentSkinPriority", true,
				"Whether elite equipment skin takes priority."
			);


			LeechSeedHeal = Config.Bind(
				"0c-Tweaks", "seedLifeGainOnHit", 2,
				"Health gained on hit from Leech Seed."
			);
			TranscendenceRegen = Config.Bind(
				"0c-Tweaks", "transcendenceShieldRegen", 0.20f,
				"Health regen converted into shield regen. Set to 0 to disable."
			);
		}


		private static void HeadHunterConfigs(ConfigFile Config)
		{
			HeadHunterCountExtra = Config.Bind(
				"1a-Headhunter", "hunterExtraCount", -1,
				"Count extra headhunters up to amount. -1 for no limit."
			);
			HeadHunterExtraEffect = Config.Bind(
				"1a-Headhunter", "hunterExtraEffect", 0.25f,
				"Count extra headhunters as this much extra stacks of aspect."
			);
			HeadHunterBaseDuration = Config.Bind(
				"1a-Headhunter", "hunterBaseBuffDuration", 20f,
				"Base duration of buff effect."
			);
			HeadHunterStackDuration = Config.Bind(
				"1a-Headhunter", "hunterAddedBuffDuration", 10f,
				"Added duration of buff effect per stack."
			);


			HeadHunterBuffEnable = Config.Bind(
				"1b-Headhunter Buff", "hunterBuffEnable", true,
				"Killing elites also grants stat buffs."
			);
			HeadHunterBuffHealth = Config.Bind(
				"1b-Headhunter Buff", "hunterIncreasedHealth", 0.05f,
				"Increased health per headhunter buff."
			);
			HeadHunterBuffArmor = Config.Bind(
				"1b-Headhunter Buff", "hunterIncreasedArmor", 2f,
				"Increased armor per headhunter buff."
			);
			HeadHunterBuffMovementSpeed = Config.Bind(
				"1b-Headhunter Buff", "hunterIncreasedMovementSpeed", 0.05f,
				"Increased movement speed per headhunter buff."
			);
			HeadHunterBuffDamage = Config.Bind(
				"1b-Headhunter Buff", "hunterIncreasedDamage", 0.02f,
				"Increased damage per headhunter buff."
			);
			HeadHunterBuffAttackSpeed = Config.Bind(
				"1b-Headhunter Buff", "hunterIncreasedAttackSpeed", 0.02f,
				"Increased attack speed per headhunter buff."
			);
			HeadHunterBuffCritChance = Config.Bind(
				"1b-Headhunter Buff", "hunterIncreasedCritChance", 2f,
				"Increased crit chance per headhunter buff."
			);
		}


		private static void AspectConfigs(ConfigFile Config)
		{
			AspectEffectMonsterDamageMult = Config.Bind(
				"20-Aspect Effects", "effectMonsterDamageMult", 1f,
				"Multiply damage value of aspect effects from monsters."
			);
			AspectEffectPlayerDebuffMult = Config.Bind(
				"20-Aspect Effects", "effectPlayerDebuffMult", 1f,
				"Multiply debuff effect of shredded and nullified applied to players."
			);


			RiskOfRainConfigs(Config);
			EliteVarietyConfigs(Config);
			LostInTransitConfigs(Config);
			AetheriumConfigs(Config);
		}


		private static void RiskOfRainConfigs(ConfigFile Config)
		{
			AspectWhiteFreezeChance = Config.Bind(
				"2aa-Ice Aspect", "iceBaseFreezeChance", 6.0f,
				"Set freeze chance. Hyperbolic. Player Only. Set to 0 to disable."
			);
			AspectWhiteFreezeDuration = Config.Bind(
				"2aa-Ice Aspect", "iceFreezeDuration", 2.0f,
				"Set freeze duration in seconds."
			);
			AspectWhiteSlowDuration = Config.Bind(
				"2aa-Ice Aspect", "iceSlowDuration", 4.0f,
				"Set slow duration in seconds."
			);
			AspectWhiteBaseDamage = Config.Bind(
				"2aa-Ice Aspect", "iceBaseTotalDamage", 0.20f,
				"Base total damage of blades. Set to 0 to disable."
			);
			AspectWhiteStackDamage = Config.Bind(
				"2aa-Ice Aspect", "iceAddedTotalDamage", 0.10f,
				"Added total damage of blades per stack."
			);


			AspectBlueSappedDuration = Config.Bind(
				"2ab-Lightning Aspect", "lightningSappedDuration", 4.0f,
				"Set sapped duration in seconds. Set to 0 to disable."
			);
			AspectBlueSappedDamage = Config.Bind(
				"2ab-Lightning Aspect", "lightningSappedDamage", 0.10f,
				"Base Damage reduction of sapped."
			);
			AspectBlueHealthConverted = Config.Bind(
				"2ab-Lightning Aspect", "lightningHealthConverted", 0.20f,
				"Set health converted into shield. Set to 0 to disable."
			);
			AspectBlueBaseShieldGain = Config.Bind(
				"2ab-Lightning Aspect", "lightningBaseShieldGained", 0.20f,
				"Set shield gained from health. Set to 0 to disable."
			);
			AspectBlueStackShieldGain = Config.Bind(
				"2ab-Lightning Aspect", "lightningAddedShieldGained", 0.10f,
				"Set shield gained from health per stack."
			);
			AspectBlueBombDuration = Config.Bind(
				"2ab-Lightning Aspect", "lightningBombDuration", 1.0f,
				"Set detonation timer for bomb."
			);
			AspectBlueBaseDamage = Config.Bind(
				"2ab-Lightning Aspect", "lightningBaseTotalDamage", 0.20f,
				"Base total damage of bombs. Set to 0 to disable."
			);
			AspectBlueStackDamage = Config.Bind(
				"2ab-Lightning Aspect", "lightningAddedTotalDamage", 0.10f,
				"Added total damage of bombs per stack."
			);


			AspectRedTrail = Config.Bind(
				"2ac-Fire Aspect", "firePlayerTrail", true,
				"Set whether players leave fire trail."
			);
			AspectRedExtraJump = Config.Bind(
				"2ac-Fire Aspect", "fireExtraJump", true,
				"Extra jump. Player Only"
			);
			AspectRedBaseMovementGain = Config.Bind(
				"2ac-Fire Aspect", "fireBaseMovementGained", 0.20f,
				"Movement speed gained. Set to 0 to disable."
			);
			AspectRedStackMovementGain = Config.Bind(
				"2ac-Fire Aspect", "fireAddedMovementGained", 0.10f,
				"Movement speed gained per stack."
			);
			AspectRedUseBase = Config.Bind(
				"2ac-Fire Aspect", "fireUseBaseDamage", false,
				"Set whether burn damage is based on BASE or TOTAL damage. Vanilla behavior is 200% BASE damage over 4 seconds."
			);
			AspectRedBurnDuration = Config.Bind(
				"2ac-Fire Aspect", "fireBurnDuration", 4.0f,
				"Set burn duration in seconds."
			);
			AspectRedBaseDamage = Config.Bind(
				"2ac-Fire Aspect", "fireBaseTotalDamage", 0.20f,
				"Base total damage of burn over duration. Set to 0 to disable."
			);
			AspectRedStackDamage = Config.Bind(
				"2ac-Fire Aspect", "fireAddedTotalDamage", 0.10f,
				"Added total damage of burn per stack."
			);


			AspectGhostSlowEffect = Config.Bind(
				"2ad-Celestial Aspect", "celestialSlowEffect", false,
				"Set whether applies slow. Shares duration with ice aspect."
			);
			AspectGhostShredDuration = Config.Bind(
				"2ad-Celestial Aspect", "celestialShredDuration", 4.0f,
				"Set shred duration in seconds. Set to 0 to disable."
			);
			AspectGhostShredArmor = Config.Bind(
				"2ad-Celestial Aspect", "celestialShredArmor", 20f,
				"Armor reduction of shred."
			);
			AspectGhostAllyArmorGain = Config.Bind(
				"2ad-Celestial Aspect", "celestialAllyArmor", 20f,
				"Armor granted to nearby allies. Effect only applies to allies without aspect and does not stack. Set to 0 to disable."
			);
			AspectGhostBaseArmorGain = Config.Bind(
				"2ad-Celestial Aspect", "celestialBaseArmor", 60f,
				"Armor gained. Set to 0 to disable."
			);
			AspectGhostStackArmorGain = Config.Bind(
				"2ad-Celestial Aspect", "celestialAddedArmor", 30f,
				"Armor gained per stack."
			);


			AspectPoisonFireSpikes = Config.Bind(
				"2ae-Malachite Aspect", "malachitePlayerSpikes", true,
				"Set whether players throw spike balls."
			);
			AspectPoisonNullDuration = Config.Bind(
				"2ae-Malachite Aspect", "malachiteNullDuration", 4.0f,
				"Set nullification duration for players in seconds. Monsters is 8 seconds."
			);
			AspectPoisonNullDamageTaken = Config.Bind(
				"2ae-Malachite Aspect", "malachiteNullDamageTaken", 0.15f,
				"Damage taken increase from nullification. Set to 0 to disable."
			);
			AspectPoisonBaseHealthGain = Config.Bind(
				"2ae-Malachite Aspect", "malachiteBaseHealth", 400f,
				"Health gained. Set to 0 to disable."
			);
			AspectPoisonStackHealthGain = Config.Bind(
				"2ae-Malachite Aspect", "malachiteAddedHealth", 200f,
				"Health gained per stack."
			);
			AspectPoisonBaseHeal = Config.Bind(
				"2ae-Malachite Aspect", "malachiteBaseLifeGainOnHit", 8,
				"Health gained on hit. Set to 0 to disable."
			);
			AspectPoisonStackHeal = Config.Bind(
				"2ae-Malachite Aspect", "malachiteAddedLifeGainOnHit", 4,
				"Health gained on hit per stack."
			);


			AspectLunarProjectiles = Config.Bind(
				"2af-Perfect Aspect", "perfectPlayerProjectiles", true,
				"Set whether players shoot projectiles during combat."
			);
			AspectLunarCrippleDuration = Config.Bind(
				"2af-Perfect Aspect", "perfectCrippleDuration", 4.0f,
				"Set cripple duration in seconds."
			);
			AspectLunarRegen = Config.Bind(
				"2af-Perfect Aspect", "perfectShieldRegen", 0.50f,
				"Health regen converted into shield regen. Set to 0 to disable."
			);
			AspectLunarBaseMovementGain = Config.Bind(
				"2af-Perfect Aspect", "perfectBaseMovementGained", 0.20f,
				"Movement speed gained. Set to 0 to disable."
			);
			AspectLunarStackMovementGain = Config.Bind(
				"2af-Perfect Aspect", "perfectAddedMovementGained", 0.10f,
				"Movement speed gained per stack."
			);
			AspectLunarBaseShieldGain = Config.Bind(
				"2af-Perfect Aspect", "perfectBaseShieldGained", 0.20f,
				"Set extra shield. Set to 0 to disable."
			);
			AspectLunarStackShieldGain = Config.Bind(
				"2af-Perfect Aspect", "perfectAddedShieldGained", 0.10f,
				"Set extra shield per stack."
			);
		}


		private static void EliteVarietyConfigs(ConfigFile Config)
		{
			AspectArmorBaseArmorGain = Config.Bind(
				"2ba-Armor Aspect", "armorBaseArmor", 30f,
				"Armor gained. Set to 0 to disable."
			);
			AspectArmorStackArmorGain = Config.Bind(
				"2ba-Armor Aspect", "armorAddedArmor", 15f,
				"Armor gained per stack."
			);


			AspectBannerBaseAttackSpeedGain = Config.Bind(
				"2bb-Banner Aspect", "bannerBaseAttackSpeedGained", 0.20f,
				"Attack speed gained. Set to 0 to disable."
			);
			AspectBannerStackAttackSpeedGain = Config.Bind(
				"2bb-Banner Aspect", "bannerAddedAttackSpeedGained", 0.10f,
				"Attack speed gained per stack."
			);


			AspectImpaleBaseDotAmp = Config.Bind(
				"2bc-Impale Aspect", "impaleBaseDotAmp", 0.20f,
				"DOT damage multiplier gained. Set to 0 to disable."
			);
			AspectImpaleStackDotAmp = Config.Bind(
				"2bc-Impale Aspect", "impaleAddedDotAmp", 0.10f,
				"DOT damage multiplier gained per stack."
			);


			AspectGoldenBaseRegenGain = Config.Bind(
				"2bd-Golden Aspect", "goldenBaseRegen", 12f,
				"Health regen gained. Set to 0 to disable."
			);
			AspectGoldenStackRegenGain = Config.Bind(
				"2bd-Golden Aspect", "goldenAddedRegen", 6f,
				"Health regen gained per stack."
			);
			AspectGoldenItemScoreFactor = Config.Bind(
				"2bd-Golden Aspect", "goldenItemScoreFactor", 4.0f,
				"Itemscore factor. white = 1x, green = 3x, other = 9x. Applies before itemscore exponent."
			);
			AspectGoldenItemScoreExponent = Config.Bind(
				"2bd-Golden Aspect", "goldenItemScoreExponent", 0.65f,
				"Itemscore exponent. Raise itemscore to the power of exponent."
			);
			AspectGoldenBaseScoredRegenGain = Config.Bind(
				"2bd-Golden Aspect", "goldenBaseScoredRegen", 1.0f,
				"ScoredRegen multiplier gained. Applies after itemscore exponent. Set to 0 to disable."
			);
			AspectGoldenStackScoredRegenGain = Config.Bind(
				"2bd-Golden Aspect", "goldenStackScoredRegen", 0.5f,
				"ScoredRegen multiplier gained per stack."
			);


			AspectCycloneBaseMovementGain = Config.Bind(
				"2be-Cyclone Aspect", "cycloneBaseMovementGained", 0.20f,
				"Movement speed gained. Set to 0 to disable."
			);
			AspectCycloneStackMovementGain = Config.Bind(
				"2be-Cyclone Aspect", "cycloneAddedMovementGained", 0.10f,
				"Movement speed gained per stack."
			);


			AspectTinkerBaseDamageResistGain = Config.Bind(
				"2bf-Tinker Aspect", "tinkerBaseDroneDamageResist", 0.20f,
				"Drone damage taken reduction. Hyperbolic. Set to 0 to disable."
			);
			AspectTinkerStackDamageResistGain = Config.Bind(
				"2bf-Tinker Aspect", "tinkerAddedDroneDamageResist", 0.10f,
				"Drone damage taken reduction per stack. Hyperbolic."
			);
		}


		private static void LostInTransitConfigs(ConfigFile Config)
		{
			AspectLeechingBaseLeechGain = Config.Bind(
				"2ca-Leeching Aspect", "leechingBaseLeech", 0.2f,
				"Damage leeched. Set to 0 to disable."
			);
			AspectLeechingStackLeechGain = Config.Bind(
				"2ca-Leeching Aspect", "leechingAddedLeech", 0.1f,
				"Damage leeched per stack."
			);
			AspectLeechingMonsterLeechMult = Config.Bind(
				"2ca-Leeching Aspect", "leechingMonsterLeechMult", 5f,
				"Monster leech multiplier."
			);


			AspectFrenziedBaseMovementGain = Config.Bind(
				"2cb-Frenzied Aspect", "frenziedBaseMovementGained", 0.2f,
				"Movement speed gained. Set to 0 to disable."
			);
			AspectFrenziedStackMovementGain = Config.Bind(
				"2cb-Frenzied Aspect", "frenziedAddedMovementGained", 0.1f,
				"Movement speed gained per stack."
			);
			AspectFrenziedMonsterMovementMult = Config.Bind(
				"2cb-Frenzied Aspect", "frenziedMonsterMovementMult", 5f,
				"Monster movement speed multiplier."
			);
			AspectFrenziedBaseAttackSpeedGain = Config.Bind(
				"2cb-Frenzied Aspect", "frenziedBaseAttackSpeedGained", 0.2f,
				"Attack speed gained. Set to 0 to disable."
			);
			AspectFrenziedStackAttackSpeedGain = Config.Bind(
				"2cb-Frenzied Aspect", "frenziedAddedAttackSpeedGained", 0.1f,
				"Attack speed gained per stack."
			);
			AspectFrenziedMonsterAttackSpeedMult = Config.Bind(
				"2cb-Frenzied Aspect", "frenziedMonsterAttackSpeedMult", 5f,
				"Monster attack speed multiplier."
			);
			AspectFrenziedBaseCooldownGain = Config.Bind(
				"2cb-Frenzied Aspect", "frenziedBaseCooldownGained", 0.2f,
				"Cooldown gained. Set to 0 to disable."
			);
			AspectFrenziedStackCooldownGain = Config.Bind(
				"2cb-Frenzied Aspect", "frenziedAddedCooldownGained", 0.1f,
				"Cooldown gained per stack."
			);
			AspectFrenziedMonsterCooldownMult = Config.Bind(
				"2cb-Frenzied Aspect", "frenziedMonsterCooldownMult", 5f,
				"Monster cooldown multiplier."
			);
		}


		private static void AetheriumConfigs(ConfigFile Config)
		{
			AspectSanguineBaseDotAmp = Config.Bind(
				"2da-Sanguine Aspect", "sanguineBaseDotAmp", 0.20f,
				"DOT damage multiplier gained. Set to 0 to disable."
			);
			AspectSanguineStackDotAmp = Config.Bind(
				"2da-Sanguine Aspect", "sanguineAddedDotAmp", 0.10f,
				"DOT damage multiplier gained per stack."
			);
			AspectSanguineBleedDuration = Config.Bind(
				"2da-Sanguine Aspect", "sanguineBleedDuration", 4.0f,
				"Set bleed duration in seconds."
			);
			AspectSanguineBaseDamage = Config.Bind(
				"2da-Sanguine Aspect", "sanguineBaseTotalDamage", 2.4f,
				"Base total damage of bleed over duration. Set to 0 to disable."
			);
			AspectSanguineStackDamage = Config.Bind(
				"2da-Sanguine Aspect", "sanguineAddedTotalDamage", 0.8f,
				"Added total damage of bleed per stack."
			);
		}
	}
}
