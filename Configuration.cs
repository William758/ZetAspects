using BepInEx.Configuration;

namespace TPDespair.ZetAspects
{
	public static class Configuration
	{
		public static ConfigEntry<bool> AspectRedTier { get; set; }
		public static ConfigEntry<bool> AspectWorldUnique { get; set; }
		public static ConfigEntry<bool> AspectShowDropText { get; set; }
		public static ConfigEntry<string> AspectDropText { get; set; }
		public static ConfigEntry<bool> AspectSkinItemApply { get; set; }
		public static ConfigEntry<bool> AspectSkinEquipmentPriority { get; set; }

		public static ConfigEntry<float> AspectDropChance { get; set; }
		public static ConfigEntry<float> AspectDropChanceMultiplayerFactor { get; set; }
		public static ConfigEntry<float> AspectDropChanceMultiplayerExponent { get; set; }
		public static ConfigEntry<float> AspectDropChanceDecay { get; set; }
		public static ConfigEntry<float> AspectDropChanceDecayLimit { get; set; }
		public static ConfigEntry<float> AspectDropChanceCompensation { get; set; }

		public static ConfigEntry<bool> AspectEliteEquipment { get; set; }
		public static ConfigEntry<bool> AspectAbilitiesEliteEquipment { get; set; }
		public static ConfigEntry<float> AspectEquipmentEffect { get; set; }
		public static ConfigEntry<bool> AspectEquipmentAbsorb { get; set; }
		public static ConfigEntry<bool> AspectEquipmentConversion { get; set; }

		public static ConfigEntry<float> TranscendenceRegen { get; set; }
		//public static ConfigEntry<bool> RecolorImmuneHealth { get; set; }

		public static ConfigEntry<float> HeadHunterBaseDuration { get; set; }
		public static ConfigEntry<float> HeadHunterStackDuration { get; set; }

		public static ConfigEntry<bool> HeadHunterBuffEnable { get; set; }
		public static ConfigEntry<float> HeadHunterBuffHealth { get; set; }
		public static ConfigEntry<float> HeadHunterBuffArmor { get; set; }
		public static ConfigEntry<float> HeadHunterBuffMovementSpeed { get; set; }
		public static ConfigEntry<float> HeadHunterBuffDamage { get; set; }
		public static ConfigEntry<float> HeadHunterBuffAttackSpeed { get; set; }
		public static ConfigEntry<float> HeadHunterBuffCritChance { get; set; }

		//public static ConfigEntry<bool> AetheriumHooks { get; set; }

		public static ConfigEntry<float> AspectWhiteBaseFreezeChance { get; set; }
		public static ConfigEntry<float> AspectWhiteStackFreezeChance { get; set; }
		public static ConfigEntry<float> AspectWhiteFreezeDuration { get; set; }
		public static ConfigEntry<float> AspectWhiteSlowDuration { get; set; }
		public static ConfigEntry<float> AspectWhiteBaseDamage { get; set; }
		public static ConfigEntry<float> AspectWhiteStackDamage { get; set; }
		public static ConfigEntry<float> AspectWhiteMonsterDamageMult { get; set; }

		public static ConfigEntry<float> AspectBlueSappedDuration { get; set; }
		public static ConfigEntry<float> AspectBlueSappedDamage { get; set; }
		public static ConfigEntry<float> AspectBlueHealthConverted { get; set; }
		public static ConfigEntry<float> AspectBlueBaseShieldGain { get; set; }
		public static ConfigEntry<float> AspectBlueStackShieldGain { get; set; }
		public static ConfigEntry<float> AspectBlueBombDuration { get; set; }
		public static ConfigEntry<float> AspectBlueBaseDamage { get; set; }
		public static ConfigEntry<float> AspectBlueStackDamage { get; set; }
		public static ConfigEntry<float> AspectBlueMonsterDamageMult { get; set; }

		public static ConfigEntry<bool> AspectRedTrail { get; set; }
		public static ConfigEntry<bool> AspectRedExtraJump { get; set; }
		public static ConfigEntry<float> AspectRedBaseMovementGain { get; set; }
		public static ConfigEntry<float> AspectRedStackMovementGain { get; set; }
		public static ConfigEntry<bool> AspectRedUseBase { get; set; }
		public static ConfigEntry<float> AspectRedBurnDuration { get; set; }
		public static ConfigEntry<float> AspectRedBaseDamage { get; set; }
		public static ConfigEntry<float> AspectRedStackDamage { get; set; }
		public static ConfigEntry<float> AspectRedMonsterDamageMult { get; set; }

		public static ConfigEntry<bool> AspectHauntedSlowEffect { get; set; }
		public static ConfigEntry<float> AspectHauntedShredDuration { get; set; }
		public static ConfigEntry<float> AspectHauntedShredArmor { get; set; }
		public static ConfigEntry<float> AspectHauntedAllyArmorGain { get; set; }
		public static ConfigEntry<float> AspectHauntedBaseArmorGain { get; set; }
		public static ConfigEntry<float> AspectHauntedStackArmorGain { get; set; }

		public static ConfigEntry<bool> AspectPoisonFireSpikes { get; set; }
		public static ConfigEntry<float> AspectPoisonNullDuration { get; set; }
		public static ConfigEntry<float> AspectPoisonNullDamageTaken { get; set; }
		public static ConfigEntry<float> AspectPoisonBaseHealthGain { get; set; }
		public static ConfigEntry<float> AspectPoisonStackHealthGain { get; set; }

		public static ConfigEntry<bool> AspectLunarProjectiles { get; set; }
		public static ConfigEntry<float> AspectLunarCrippleDuration { get; set; }
		public static ConfigEntry<float> AspectLunarRegen { get; set; }
		public static ConfigEntry<float> AspectLunarBaseMovementGain { get; set; }
		public static ConfigEntry<float> AspectLunarStackMovementGain { get; set; }
		public static ConfigEntry<float> AspectLunarBaseShieldGain { get; set; }
		public static ConfigEntry<float> AspectLunarStackShieldGain { get; set; }
		/*
		public static ConfigEntry<float> AspectSanguineBaseDotAmp { get; set; }
		public static ConfigEntry<float> AspectSanguineStackDotAmp { get; set; }
		public static ConfigEntry<float> AspectSanguineBleedDuration { get; set; }
		public static ConfigEntry<float> AspectSanguineBaseDamage { get; set; }
		public static ConfigEntry<float> AspectSanguineStackDamage { get; set; }
		public static ConfigEntry<float> AspectSanguineMonsterDamageMult { get; set; }
		*/


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
				"Make aspects only drop from their respective elites and no where else. Set to false to add them to the drop pool."
			);
			AspectShowDropText = Config.Bind(
				"0a-General", "aspectEnableDropText", true,
				"Display a chat message when an elite drops an aspect."
			);
			AspectDropText = Config.Bind(
				"0a-General", "aspectDropText", "A slain enemy leaves some of its power behind...",
				"Chat message for when an elite drops an aspect."
			);
			AspectSkinItemApply = Config.Bind(
				"0a-General", "aspectItemEliteSkin", true,
				"Whether new items can apply elite skin."
			);
			AspectSkinEquipmentPriority = Config.Bind(
				"0a-General", "eliteEquipmentSkinPriority", true,
				"Whether elite equipment skin takes priority."
			);



			AspectDropChance = Config.Bind(
				"0b-DropChance", "aspectDropChance", 0.2f,
				"Percent chance that an elite drops its aspect. 0.2 is 0.2%"
			);
			AspectDropChanceMultiplayerFactor = Config.Bind(
				"0b-DropChance", "aspectDropChanceMultiplayerFactor", 1f,
				"Multiply playercount used in drop chance calculation. Applies before playercount exponent."
			);
			AspectDropChanceMultiplayerExponent = Config.Bind(
				"0b-DropChance", "aspectDropChanceMultiplayerExponent", 0.5f,
				"Drop chance is multiplied by the playercount to the power of value. -1 to divide chance by playercount, 0 to have playercount have no effect on chance, 1 to multiply chance by playercount."
			);
			AspectDropChanceDecay = Config.Bind(
				"0b-DropChance", "aspectDropChanceDecay", 0.5f,
				"Multiply drop chance for every aspect that has dropped in a run."
			);
			AspectDropChanceDecayLimit = Config.Bind(
				"0b-DropChance", "aspectDropChanceDecayLimit", 0.25f,
				"Drop chance decay lower limit. Drop chance decay multiplier will not go below this value."
			);
			AspectDropChanceCompensation = Config.Bind(
				"0b-DropChance", "aspectDropChanceCompensation", 4f,
				"Multiply drop chance at certain dropcount vs stagecount thresholds. 1 drop by stage 4, 2 by 7, 3 by 10, 4 by 15, 5 by 20..."
			);



			AspectEliteEquipment = Config.Bind(
				"0c-Equipment", "eliteEquipmentDrop", false,
				"Elites drop aspects as equipment. Otherwise they drop aspects as items."
			);
			AspectAbilitiesEliteEquipment = Config.Bind(
				"0c-Equipment", "aspectAbilitiesForceEquipmentDrop", true,
				"Elites drop aspects as equipment instead of items if Aspect Abilities is installed."
			);
			AspectEquipmentEffect = Config.Bind(
				"0c-Equipment", "eliteEquipmentEffect", 3f,
				"Count equipment as this many stacks of aspects for players."
			);
			AspectEquipmentAbsorb = Config.Bind(
				"0c-Equipment", "eliteEquipmentAbsorb", false,
				"Picking up the same elite equipment will absorb as non-equipment item."
			);
			AspectEquipmentConversion = Config.Bind(
				"0c-Equipment", "eliteEquipmentConversion", true,
				"Clicking bottom-right equipment icon will convert into non-equipment item."
			);



			TranscendenceRegen = Config.Bind(
				"0d-Tweaks", "transcendenceShieldRegen", 0.50f,
				"Health regen gained as shield regen. Set to 0 to disable."
			);
			/*
			RecolorImmuneHealth = Config.Bind(
				"0d-Tweaks", "recolorImmuneHealth", false,
				"Change healthbar color while immune."
			);
			*/
		}

		private static void HeadHunterConfigs(ConfigFile Config)
		{
			HeadHunterBaseDuration = Config.Bind(
				"1a-Headhunter", "hunterBaseBuffDuration", 20f,
				"Base duration of buff effect."
			);
			HeadHunterStackDuration = Config.Bind(
				"1a-Headhunter", "hunterAddedBuffDuration", 10f,
				"Added duration of buff effect per stack."
			);



			HeadHunterBuffEnable = Config.Bind(
				"1b-HeadhunterBuff", "hunterBuffEnable", true,
				"Killing elites also grants stat buffs."
			);
			HeadHunterBuffHealth = Config.Bind(
				"1b-HeadhunterBuff", "hunterIncreasedHealth", 0.05f,
				"Increased health per headhunter buff."
			);
			HeadHunterBuffArmor = Config.Bind(
				"1b-HeadhunterBuff", "hunterIncreasedArmor", 2f,
				"Increased armor per headhunter buff."
			);
			HeadHunterBuffMovementSpeed = Config.Bind(
				"1b-HeadhunterBuff", "hunterIncreasedMovementSpeed", 0.05f,
				"Increased movement speed per headhunter buff."
			);
			HeadHunterBuffDamage = Config.Bind(
				"1b-HeadhunterBuff", "hunterIncreasedDamage", 0.02f,
				"Increased damage per headhunter buff."
			);
			HeadHunterBuffAttackSpeed = Config.Bind(
				"1b-HeadhunterBuff", "hunterIncreasedAttackSpeed", 0.02f,
				"Increased attack speed per headhunter buff."
			);
			HeadHunterBuffCritChance = Config.Bind(
				"1b-HeadhunterBuff", "hunterIncreasedCritChance", 2f,
				"Increased crit chance per headhunter buff."
			);
		}

		private static void AspectConfigs(ConfigFile Config)
		{
			/*
			AetheriumHooks = Config.Bind(
				"20-ModCompatibility", "aetheriumHooks", true,
				"Allows modification of functions and values within the Aetherium mod."
			);
			*/


			RiskOfRainConfigs(Config);
			//AetheriumConfigs(Config);
		}

		private static void RiskOfRainConfigs(ConfigFile Config)
		{
			AspectWhiteBaseFreezeChance = Config.Bind(
				"2aa-AspectWhite", "whiteBaseFreezeChance", 6.0f,
				"Freeze chance. Hyperbolic. Player Only. Set to 0 to disable."
			);
			AspectWhiteStackFreezeChance = Config.Bind(
				"2aa-AspectWhite", "whiteAddedFreezeChance", 6.0f,
				"Freeze chance per stack."
			);
			AspectWhiteFreezeDuration = Config.Bind(
				"2aa-AspectWhite", "whiteFreezeDuration", 2.0f,
				"Set freeze duration in seconds."
			);
			AspectWhiteSlowDuration = Config.Bind(
				"2aa-AspectWhite", "whiteSlowDuration", 4.0f,
				"Set slow duration in seconds."
			);
			AspectWhiteBaseDamage = Config.Bind(
				"2aa-AspectWhite", "whiteBaseTotalDamage", 0.20f,
				"Base total damage of blades. Set to 0 to disable."
			);
			AspectWhiteStackDamage = Config.Bind(
				"2aa-AspectWhite", "whiteAddedTotalDamage", 0.10f,
				"Added total damage of blades per stack."
			);
			AspectWhiteMonsterDamageMult = Config.Bind(
				"2aa-AspectWhite", "whiteMonsterDamageMult", 1f,
				"Multiply damage of aspect effects from monsters."
			);



			AspectBlueSappedDuration = Config.Bind(
				"2ab-AspectBlue", "blueSappedDuration", 4.0f,
				"Set sapped duration in seconds. Set to 0 to disable."
			);
			AspectBlueSappedDamage = Config.Bind(
				"2ab-AspectBlue", "blueSappedDamage", 0.10f,
				"Base Damage reduction of sapped."
			);
			AspectBlueHealthConverted = Config.Bind(
				"2ab-AspectBlue", "blueHealthConverted", 0.20f,
				"Set health converted into shield. Set to 0 to disable."
			);
			AspectBlueBaseShieldGain = Config.Bind(
				"2ab-AspectBlue", "blueBaseShieldGained", 0.20f,
				"Set shield gained from health. Set to 0 to disable."
			);
			AspectBlueStackShieldGain = Config.Bind(
				"2ab-AspectBlue", "blueAddedShieldGained", 0.10f,
				"Set shield gained from health per stack."
			);
			AspectBlueBombDuration = Config.Bind(
				"2ab-AspectBlue", "blueBombDuration", 1.0f,
				"Set detonation timer for bomb."
			);
			AspectBlueBaseDamage = Config.Bind(
				"2ab-AspectBlue", "blueBaseTotalDamage", 0.20f,
				"Base total damage of bombs. Set to 0 to disable."
			);
			AspectBlueStackDamage = Config.Bind(
				"2ab-AspectBlue", "blueAddedTotalDamage", 0.10f,
				"Added total damage of bombs per stack."
			);
			AspectBlueMonsterDamageMult = Config.Bind(
				"2ab-AspectBlue", "blueMonsterDamageMult", 1f,
				"Multiply damage of aspect effects from monsters."
			);



			AspectRedTrail = Config.Bind(
				"2ac-AspectRed", "redPlayerTrail", true,
				"Set whether players leave fire trail."
			);
			AspectRedExtraJump = Config.Bind(
				"2ac-AspectRed", "redExtraJump", true,
				"Extra jump. Player Only"
			);
			AspectRedBaseMovementGain = Config.Bind(
				"2ac-AspectRed", "redBaseMovementGained", 0.20f,
				"Movement speed gained. Set to 0 to disable."
			);
			AspectRedStackMovementGain = Config.Bind(
				"2ac-AspectRed", "redAddedMovementGained", 0.10f,
				"Movement speed gained per stack."
			);
			AspectRedUseBase = Config.Bind(
				"2ac-AspectRed", "redUseBaseDamage", false,
				"Set whether burn damage is based on BASE or TOTAL damage. Vanilla behavior is 200% BASE damage over 4 seconds."
			);
			AspectRedBurnDuration = Config.Bind(
				"2ac-AspectRed", "redBurnDuration", 4.0f,
				"Set burn duration in seconds."
			);
			AspectRedBaseDamage = Config.Bind(
				"2ac-AspectRed", "redBaseTotalDamage", 0.20f,
				"Base total damage of burn over duration. Set to 0 to disable."
			);
			AspectRedStackDamage = Config.Bind(
				"2ac-AspectRed", "redAddedTotalDamage", 0.10f,
				"Added total damage of burn per stack."
			);
			AspectRedMonsterDamageMult = Config.Bind(
				"2ac-AspectRed", "redMonsterDamageMult", 1f,
				"Multiply damage of aspect effects from monsters."
			);



			AspectHauntedSlowEffect = Config.Bind(
				"2ad-AspectHaunted", "hauntedSlowEffect", false,
				"Set whether applies slow. Shares duration with ice aspect."
			);
			AspectHauntedShredDuration = Config.Bind(
				"2ad-AspectHaunted", "hauntedShredDuration", 4.0f,
				"Set shred duration in seconds. Set to 0 to disable."
			);
			AspectHauntedShredArmor = Config.Bind(
				"2ad-AspectHaunted", "hauntedShredArmor", 20f,
				"Armor reduction of shred."
			);
			AspectHauntedAllyArmorGain = Config.Bind(
				"2ad-AspectHaunted", "hauntedAllyArmor", 20f,
				"Armor granted to nearby allies. Effect only applies to allies without aspect and does not stack. Set to 0 to disable."
			);
			AspectHauntedBaseArmorGain = Config.Bind(
				"2ad-AspectHaunted", "hauntedBaseArmor", 60f,
				"Armor gained. Set to 0 to disable."
			);
			AspectHauntedStackArmorGain = Config.Bind(
				"2ad-AspectHaunted", "hauntedAddedArmor", 30f,
				"Armor gained per stack."
			);



			AspectPoisonFireSpikes = Config.Bind(
				"2ae-AspectPoison", "poisonPlayerSpikes", true,
				"Set whether players throw spike balls."
			);
			AspectPoisonNullDuration = Config.Bind(
				"2ae-AspectPoison", "poisonNullDuration", 4.0f,
				"Set nullification duration for players in seconds. Monsters is 8 seconds."
			);
			AspectPoisonNullDamageTaken = Config.Bind(
				"2ae-AspectPoison", "poisonNullDamageTaken", 0.15f,
				"Damage taken increase from nullification. Set to 0 to disable."
			);
			AspectPoisonBaseHealthGain = Config.Bind(
				"2ae-AspectPoison", "poisonBaseHealth", 400f,
				"Health gained. Set to 0 to disable."
			);
			AspectPoisonStackHealthGain = Config.Bind(
				"2ae-AspectPoison", "poisonAddedHealth", 200f,
				"Health gained per stack."
			);



			AspectLunarProjectiles = Config.Bind(
				"2af-AspectLunar", "lunarPlayerProjectiles", true,
				"Set whether players shoot projectiles during combat."
			);
			AspectLunarCrippleDuration = Config.Bind(
				"2af-AspectLunar", "lunarCrippleDuration", 4.0f,
				"Set cripple duration in seconds."
			);
			AspectLunarRegen = Config.Bind(
				"2af-AspectLunar", "lunarShieldRegen", 1.00f,
				"Health regen gained as shield regen. Set to 0 to disable."
			);
			AspectLunarBaseMovementGain = Config.Bind(
				"2af-AspectLunar", "lunarBaseMovementGained", 0.20f,
				"Movement speed gained. Set to 0 to disable."
			);
			AspectLunarStackMovementGain = Config.Bind(
				"2af-AspectLunar", "lunarAddedMovementGained", 0.10f,
				"Movement speed gained per stack."
			);
			AspectLunarBaseShieldGain = Config.Bind(
				"2af-AspectLunar", "lunarBaseShieldGained", 0.20f,
				"Set extra shield. Set to 0 to disable."
			);
			AspectLunarStackShieldGain = Config.Bind(
				"2af-AspectLunar", "lunarAddedShieldGained", 0.10f,
				"Set extra shield per stack."
			);
		}

		private static void AetheriumConfigs(ConfigFile Config)
		{
			/*
			AspectSanguineBaseDotAmp = Config.Bind(
				"2ba-AspectSanguine", "sanguineBaseDotAmp", 0.20f,
				"DOT damage multiplier gained. Set to 0 to disable."
			);
			AspectSanguineStackDotAmp = Config.Bind(
				"2ba-AspectSanguine", "sanguineAddedDotAmp", 0.10f,
				"DOT damage multiplier gained per stack."
			);
			AspectSanguineBleedDuration = Config.Bind(
				"2ba-AspectSanguine", "sanguineBleedDuration", 4.0f,
				"Set bleed duration in seconds."
			);
			AspectSanguineBaseDamage = Config.Bind(
				"2ba-AspectSanguine", "sanguineBaseTotalDamage", 1.2f,
				"Base total damage of bleed over duration. Set to 0 to disable."
			);
			AspectSanguineStackDamage = Config.Bind(
				"2ba-AspectSanguine", "sanguineAddedTotalDamage", 0.6f,
				"Added total damage of bleed per stack."
			);
			AspectSanguineMonsterDamageMult = Config.Bind(
				"2ba-AspectSanguine", "sanguineMonsterDamageMult", 1f,
				"Multiply damage of aspect effects from monsters."
			);
			*/
		}
	}
}
