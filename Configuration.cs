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
		public static ConfigEntry<bool> AspectDropWeightMaster { get; set; }
		public static ConfigEntry<int> AspectDropWeightLimit { get; set; }
		public static ConfigEntry<float> AspectDropWeightWhite { get; set; }
		public static ConfigEntry<float> AspectDropWeightBlue { get; set; }
		public static ConfigEntry<float> AspectDropWeightRed { get; set; }
		public static ConfigEntry<float> AspectDropWeightHaunted { get; set; }
		public static ConfigEntry<float> AspectDropWeightPoison { get; set; }
		public static ConfigEntry<float> AspectDropWeightLunar { get; set; }
		public static ConfigEntry<float> AspectDropWeightEarth { get; set; }
		public static ConfigEntry<float> AspectDropWeightVoid { get; set; }
		public static ConfigEntry<float> AspectDropWeightWarped { get; set; }
		public static ConfigEntry<float> AspectDropWeightPlated { get; set; }

		public static ConfigEntry<bool> AspectEliteEquipment { get; set; }
		public static ConfigEntry<bool> AspectAbilitiesEliteEquipment { get; set; }
		public static ConfigEntry<float> AspectEquipmentEffect { get; set; }
		public static ConfigEntry<bool> AspectEquipmentAbsorb { get; set; }
		public static ConfigEntry<bool> AspectEquipmentConversion { get; set; }

		public static ConfigEntry<float> TranscendenceRegen { get; set; }
		public static ConfigEntry<bool> RecolorHpBar { get; set; }
		public static ConfigEntry<bool> RecolorImmuneBar { get; set; }
		public static ConfigEntry<bool> RecolorShieldConvertBar { get; set; }

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
		public static ConfigEntry<float> AspectRedBaseBurnDamage { get; set; }
		public static ConfigEntry<float> AspectRedStackBurnDamage { get; set; }
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

		public static ConfigEntry<float> AspectEarthRegeneration { get; set; }
		public static ConfigEntry<float> AspectEarthPoachedDuration { get; set; }
		public static ConfigEntry<float> AspectEarthPoachedAttackSpeed { get; set; }
		public static ConfigEntry<float> AspectEarthPoachedLeech { get; set; }
		public static ConfigEntry<float> AspectEarthBaseLeech { get; set; }
		public static ConfigEntry<float> AspectEarthStackLeech { get; set; }
		public static ConfigEntry<float> AspectEarthMonsterLeechMult { get; set; }

		public static ConfigEntry<float> AspectVoidBaseHealthGain { get; set; }
		public static ConfigEntry<float> AspectVoidStackHealthGain { get; set; }
		public static ConfigEntry<float> AspectVoidBaseDamageGain { get; set; }
		public static ConfigEntry<float> AspectVoidStackDamageGain { get; set; }
		public static ConfigEntry<bool> AspectVoidUseBase { get; set; }
		public static ConfigEntry<float> AspectVoidBaseCollapseDamage { get; set; }
		public static ConfigEntry<float> AspectVoidStackCollapseDamage { get; set; }
		public static ConfigEntry<float> AspectVoidMonsterDamageMult { get; set; }

		public static ConfigEntry<float> AspectPlatedBaseArmorGain { get; set; }
		public static ConfigEntry<float> AspectPlatedStackArmorGain { get; set; }
		public static ConfigEntry<float> AspectWarpedBaseCooldownGain { get; set; }
		public static ConfigEntry<float> AspectWarpedStackCooldownGain { get; set; }



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
			AspectDropWeightMaster = Config.Bind(
				"0b-DropWeight", "aspectDropWeightMaster", false,
				"Enable this config to adjust the drop weight of each individual aspect when dropped by elites. Run the game again after enabling this config to generate drop weight configs."
			);
			if (AspectDropWeightMaster.Value)
			{
				AspectDropWeightLimit = Config.Bind(
					"0b-DropWeight", "aspectDropWeightLimit", 5,
					"Last stage to apply drop weights. 0 to always apply."
				);

				AspectDropWeightWhite = Config.Bind(
					"0b-DropWeight", "aspectDropWeightWhite", 1f,
					"Drop chance multiplier for AffixWhite"
				);
				AspectDropWeightBlue = Config.Bind(
					"0b-DropWeight", "aspectDropWeightBlue", 1f,
					"Drop chance multiplier for AffixBlue"
				);
				AspectDropWeightRed = Config.Bind(
					"0b-DropWeight", "aspectDropWeightRed", 1f,
					"Drop chance multiplier for AffixRed"
				);
				AspectDropWeightHaunted = Config.Bind(
					"0b-DropWeight", "aspectDropWeightHaunted", 1f,
					"Drop chance multiplier for AffixHaunted"
				);
				AspectDropWeightPoison = Config.Bind(
					"0b-DropWeight", "aspectDropWeightPoison", 1f,
					"Drop chance multiplier for AffixPoison"
				);
				AspectDropWeightLunar = Config.Bind(
					"0b-DropWeight", "aspectDropWeightLunar", 1f,
					"Drop chance multiplier for AffixLunar"
				);
				AspectDropWeightEarth = Config.Bind(
					"0b-DropWeight", "aspectDropWeightEarth", 1f,
					"Drop chance multiplier for AffixEarth"
				);
				AspectDropWeightVoid = Config.Bind(
					"0b-DropWeight", "aspectDropWeightVoid", 1f,
					"Drop chance multiplier for AffixVoid"
				);
				AspectDropWeightWarped = Config.Bind(
					"0b-DropWeight", "aspectDropWeightWarped", 1f,
					"Drop chance multiplier for AffixWarped"
				);
				AspectDropWeightPlated = Config.Bind(
					"0b-DropWeight", "aspectDropWeightPlated", 1f,
					"Drop chance multiplier for AffixPlated"
				);

				Catalog.dropWeightsAvailable = true;
			}



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
			RecolorHpBar = Config.Bind(
				"0d-Tweaks", "recolorHpBar", true,
				"Tweak the color of various hpbar conditions."
			);
			RecolorImmuneBar = Config.Bind(
				"0d-Tweaks", "recolorImmuneBar", false,
				"Change hpbar color while invulnerable."
			);
			RecolorShieldConvertBar = Config.Bind(
				"0d-Tweaks", "recolorShieldConvertBar", true,
				"Change hpbar color while all health is converted into shields."
			);
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
			RiskOfRainConfigs(Config);
			SpikeStripConfigs(Config);
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
				"Monster blade damage multiplier."
			);



			AspectBlueSappedDuration = Config.Bind(
				"2ab-AspectBlue", "blueSappedDuration", 4.0f,
				"Set sapped duration in seconds. Set to 0 to disable."
			);
			AspectBlueSappedDamage = Config.Bind(
				"2ab-AspectBlue", "blueSappedDamage", 0.10f,
				"Damage reduction of sapped."
			);
			AspectBlueHealthConverted = Config.Bind(
				"2ab-AspectBlue", "blueHealthConverted", 0.20f,
				"Health converted into shield. Set to 0 to disable."
			);
			AspectBlueBaseShieldGain = Config.Bind(
				"2ab-AspectBlue", "blueBaseShieldGained", 0.20f,
				"Shield gained from health. Set to 0 to disable."
			);
			AspectBlueStackShieldGain = Config.Bind(
				"2ab-AspectBlue", "blueAddedShieldGained", 0.10f,
				"Shield gained from health per stack."
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
				"Monster bomb damage multiplier."
			);



			AspectRedTrail = Config.Bind(
				"2ac-AspectRed", "redPlayerTrail", true,
				"Players leave fire trail."
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
			AspectRedBaseBurnDamage = Config.Bind(
				"2ac-AspectRed", "redBaseBurnDamage", 0.20f,
				"Base total damage of burn over duration. Set to 0 to disable."
			);
			AspectRedStackBurnDamage = Config.Bind(
				"2ac-AspectRed", "redAddedBurnDamage", 0.10f,
				"Added total damage of burn per stack."
			);
			AspectRedMonsterDamageMult = Config.Bind(
				"2ac-AspectRed", "redMonsterDamageMult", 1f,
				"Monster burn damage multiplier."
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
				"Players throw spike balls."
			);
			AspectPoisonNullDuration = Config.Bind(
				"2ae-AspectPoison", "poisonNullDuration", 4.0f,
				"Set ruin duration for players in seconds. Monster duration is 8 seconds."
			);
			AspectPoisonNullDamageTaken = Config.Bind(
				"2ae-AspectPoison", "poisonNullDamageTaken", 0.15f,
				"Damage taken increase from ruin. Set to 0 to disable."
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
				"Players shoot projectiles during combat."
			);
			AspectLunarCrippleDuration = Config.Bind(
				"2af-AspectLunar", "lunarCrippleDuration", 4.0f,
				"Set cripple duration in seconds. Set to 0 to disable."
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
				"Extra shield. Set to 0 to disable."
			);
			AspectLunarStackShieldGain = Config.Bind(
				"2af-AspectLunar", "lunarAddedShieldGained", 0.10f,
				"Extra shield per stack."
			);



			AspectEarthRegeneration = Config.Bind(
				"2ag-AspectEarth", "earthPercentRegeneration", 0.01f,
				"Percent health regen gained. Monsters need to be outOfCombat. Set to 0 to disable."
			);
			AspectEarthPoachedDuration = Config.Bind(
				"2ag-AspectEarth", "earthPoachedDuration", 4.0f,
				"Set poached duration in seconds. Set to 0 to disable."
			);
			AspectEarthPoachedAttackSpeed = Config.Bind(
				"2ag-AspectEarth", "earthPoachedAttackSpeed", 0.10f,
				"Attack speed reduction of poached. Set to 0 to disable."
			);
			AspectEarthPoachedLeech = Config.Bind(
				"2ag-AspectEarth", "earthPoachedLeech", 0.10f,
				"Percent damage leech amount from poached. Additive with aspect leech. Set to 0 to disable."
			);
			AspectEarthBaseLeech = Config.Bind(
				"2ag-AspectEarth", "earthBaseLeech", 0.10f,
				"Percent damage leech amount. Set to 0 to disable."
			);
			AspectEarthStackLeech = Config.Bind(
				"2ag-AspectEarth", "earthAddedLeech", 0.10f,
				"Percent damage leech amount per stack."
			);
			AspectEarthMonsterLeechMult = Config.Bind(
				"2ag-AspectEarth", "earthMonsterLeechMult", 5f,
				"Monster leech multiplier."
			);



			AspectVoidBaseHealthGain = Config.Bind(
				"2ah-AspectVoid", "voidBaseHealthGained", 0.20f,
				"Health gained. Set to 0 to disable."
			);
			AspectVoidStackHealthGain = Config.Bind(
				"2ah-AspectVoid", "voidAddedHealthGained", 0.10f,
				"Health gained per stack."
			);
			AspectVoidBaseDamageGain = Config.Bind(
				"2ah-AspectVoid", "voidBaseDamageGained", 0.20f,
				"Damage gained. Set to 0 to disable."
			);
			AspectVoidStackDamageGain = Config.Bind(
				"2ah-AspectVoid", "voidAddedDamageGained", 0.10f,
				"Damage gained per stack."
			);
			AspectVoidUseBase = Config.Bind(
				"2ah-AspectVoid", "voidUseBaseDamage", false,
				"Set whether collapse damage is based on BASE or TOTAL damage. Vanilla behavior is 400% BASE damage."
			);
			AspectVoidBaseCollapseDamage = Config.Bind(
				"2ah-AspectVoid", "voidBaseCollapseDamage", 0.20f,
				"Base total damage of collapse over duration. Set to 0 to disable."
			);
			AspectVoidStackCollapseDamage = Config.Bind(
				"2ah-AspectVoid", "voidAddedCollapseDamage", 0.10f,
				"Added total damage of collapse per stack."
			);
			AspectVoidMonsterDamageMult = Config.Bind(
				"2ah-AspectVoid", "voidMonsterDamageMult", 1f,
				"Monster collapse damage multiplier."
			);
		}

		private static void SpikeStripConfigs(ConfigFile Config)
		{
			AspectPlatedBaseArmorGain = Config.Bind(
				"2ba-AspectPlated", "platedBaseArmor", 30f,
				"Armor gained. Set to 0 to disable."
			);
			AspectPlatedStackArmorGain = Config.Bind(
				"2ba-AspectPlated", "platedAddedArmor", 15f,
				"Armor gained per stack."
			);



			AspectWarpedBaseCooldownGain = Config.Bind(
				"2bb-AspectWarped", "warpedBaseCooldown", 0.2f,
				"Cooldown reduction gained. Set to 0 to disable."
			);
			AspectWarpedStackCooldownGain = Config.Bind(
				"2bb-AspectWarped", "warpedAddedCooldown", 0.1f,
				"Cooldown reduction gained per stack."
			);
		}
	}
}
