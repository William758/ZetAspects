using BepInEx.Configuration;

namespace TPDespair.ZetAspects
{
	public static class Configuration
	{
		public static ConfigEntry<bool> AspectRedTier { get; set; }
		public static ConfigEntry<bool> AspectWorldUnique { get; set; }
		public static ConfigEntry<bool> AspectCommandGroupItems { get; set; }
		public static ConfigEntry<bool> AspectCommandGroupEquip { get; set; }
		public static ConfigEntry<bool> AspectShowDropText { get; set; }
		public static ConfigEntry<string> AspectDropText { get; set; }
		public static ConfigEntry<bool> AspectSkinItemApply { get; set; }
		public static ConfigEntry<bool> AspectSkinEquipmentPriority { get; set; }

		public static ConfigEntry<bool> LogbookHideItem { get; set; }
		public static ConfigEntry<bool> LogbookHideEquipment { get; set; }

		public static ConfigEntry<bool> AspectDropVerboseLogging { get; set; }
		public static ConfigEntry<float> AspectDropChance { get; set; }
		public static ConfigEntry<float> AspectDropChanceMultiplayerFactor { get; set; }
		public static ConfigEntry<float> AspectDropChanceMultiplayerExponent { get; set; }
		public static ConfigEntry<float> AspectDropChanceDecay { get; set; }
		public static ConfigEntry<float> AspectDropChanceDecayLimit { get; set; }
		public static ConfigEntry<float> AspectDropChanceCompensation { get; set; }
		public static ConfigEntry<bool> AspectDropWeightMaster { get; set; }
		public static ConfigEntry<int> AspectDropWeightLimit { get; set; }

		public static ConfigEntry<bool> AspectEliteEquipment { get; set; }
		public static ConfigEntry<bool> AspectAbilitiesEliteEquipment { get; set; }
		public static ConfigEntry<float> AspectEquipmentEffect { get; set; }
		public static ConfigEntry<bool> AspectEquipmentAbsorb { get; set; }
		public static ConfigEntry<bool> AspectEquipmentConversion { get; set; }

		public static ConfigEntry<float> MonsterRegenExponent { get; set; }
		public static ConfigEntry<float> TranscendenceRegen { get; set; }
		public static ConfigEntry<float> ShieldRegenBreakDelay { get; set; }
		public static ConfigEntry<float> NearbyPlayerDodgeBypass { get; set; }
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

		public static ConfigEntry<int> UpdateInventoryFromBuff { get; set; }
		public static ConfigEntry<bool> AetheriumHooks { get; set; }
		public static ConfigEntry<bool> EliteReworksHooks { get; set; }
		public static ConfigEntry<bool> SpikeStripHooks { get; set; }
		public static ConfigEntry<bool> WarWispHooks { get; set; }
		public static ConfigEntry<bool> BlightedHooks { get; set; }
		public static ConfigEntry<bool> GotceHooks { get; set; }
		public static ConfigEntry<bool> RisingTidesHooks { get; set; }
		public static ConfigEntry<bool> MoreElitesHooks { get; set; }
		public static ConfigEntry<bool> EliteVarietyHooks { get; set; }
		public static ConfigEntry<bool> AugmentumHooks { get; set; }
		public static ConfigEntry<bool> SandsweptHooks { get; set; }
		public static ConfigEntry<bool> StarstormHooks { get; set; }

		public static ConfigEntry<float> AspectWhiteBaseFreezeChance { get; set; }
		public static ConfigEntry<float> AspectWhiteStackFreezeChance { get; set; }
		public static ConfigEntry<float> AspectWhiteFreezeDuration { get; set; }
		public static ConfigEntry<float> AspectWhiteSlowDuration { get; set; }
		public static ConfigEntry<float> AspectWhiteBaseDamage { get; set; }
		public static ConfigEntry<float> AspectWhiteStackDamage { get; set; }
		public static ConfigEntry<bool> AspectWhiteThornsProc { get; set; }
		public static ConfigEntry<float> AspectWhiteMonsterDamageMult { get; set; }

		public static ConfigEntry<float> AspectBlueSappedDuration { get; set; }
		public static ConfigEntry<float> AspectBlueSappedDamage { get; set; }
		public static ConfigEntry<float> AspectBlueHealthConverted { get; set; }
		public static ConfigEntry<float> AspectBlueBaseShieldGain { get; set; }
		public static ConfigEntry<float> AspectBlueStackShieldGain { get; set; }
		public static ConfigEntry<float> AspectBlueBombDuration { get; set; }
		public static ConfigEntry<float> AspectBlueBaseDamage { get; set; }
		public static ConfigEntry<float> AspectBlueStackDamage { get; set; }
		public static ConfigEntry<bool> AspectBlueEliteReworksDamage { get; set; }
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
		public static ConfigEntry<float> AspectHauntedAllyDodgeGain { get; set; }
		public static ConfigEntry<float> AspectHauntedBaseArmorGain { get; set; }
		public static ConfigEntry<float> AspectHauntedStackArmorGain { get; set; }
		public static ConfigEntry<float> AspectHauntedBaseDodgeGain { get; set; }
		public static ConfigEntry<float> AspectHauntedStackDodgeGain { get; set; }

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
		public static ConfigEntry<int> AspectEarthLeechModifier { get; set; }
		public static ConfigEntry<float> AspectEarthLeechModMult { get; set; }
		public static ConfigEntry<float> AspectEarthLeechParameter { get; set; }
		public static ConfigEntry<float> AspectEarthLeechPostModMult { get; set; }
		public static ConfigEntry<float> AspectEarthPoachedLeech { get; set; }
		public static ConfigEntry<float> AspectEarthBaseLeech { get; set; }
		public static ConfigEntry<float> AspectEarthStackLeech { get; set; }
		public static ConfigEntry<float> AspectEarthMonsterLeechMult { get; set; }

		public static ConfigEntry<bool> AspectVoidContagiousItem { get; set; }
		public static ConfigEntry<float> AspectVoidBaseHealthGain { get; set; }
		public static ConfigEntry<float> AspectVoidStackHealthGain { get; set; }
		public static ConfigEntry<float> AspectVoidBaseDamageGain { get; set; }
		public static ConfigEntry<float> AspectVoidStackDamageGain { get; set; }
		public static ConfigEntry<bool> AspectVoidUseBase { get; set; }
		public static ConfigEntry<float> AspectVoidBaseCollapseDamage { get; set; }
		public static ConfigEntry<float> AspectVoidStackCollapseDamage { get; set; }
		public static ConfigEntry<float> AspectVoidMonsterDamageMult { get; set; }

		public static ConfigEntry<float> AspectPlatedStifleMult { get; set; }
		public static ConfigEntry<float> AspectPlatedStifleExponent { get; set; }
		public static ConfigEntry<float> AspectPlatedPlayerPlateCountMult { get; set; }
		public static ConfigEntry<float> AspectPlatedMonsterPlateCountMult { get; set; }
		public static ConfigEntry<float> AspectPlatedBaseArmorGain { get; set; }
		public static ConfigEntry<float> AspectPlatedStackArmorGain { get; set; }
		public static ConfigEntry<float> AspectPlatedBasePlatingGain { get; set; }
		public static ConfigEntry<float> AspectPlatedStackPlatingGain { get; set; }
		public static ConfigEntry<float> AspectPlatedMonsterPlatingMult { get; set; }
		public static ConfigEntry<bool> AspectPlatedPlayerHealthReduction { get; set; }
		public static ConfigEntry<bool> AspectPlatedAllowDefenceWithNem { get; set; }

		public static ConfigEntry<float> AspectWarpedLiftForce { get; set; }
		public static ConfigEntry<int> AspectWarpedAltDebuff { get; set; }
		public static ConfigEntry<float> AspectWarpedAltJumpMult { get; set; }
		public static ConfigEntry<float> AspectWarpedAltSpeedMult { get; set; }
		public static ConfigEntry<float> AspectWarpedAltAccelerationMult { get; set; }
		public static ConfigEntry<float> AspectWarpedBaseCooldownGain { get; set; }
		public static ConfigEntry<float> AspectWarpedStackCooldownGain { get; set; }
		public static ConfigEntry<float> AspectWarpedBaseFallReductionGain { get; set; }
		public static ConfigEntry<float> AspectWarpedStackFallReductionGain { get; set; }
		public static ConfigEntry<float> AspectWarpedBaseForceResistGain { get; set; }
		public static ConfigEntry<float> AspectWarpedStackForceResistGain { get; set; }

		public static ConfigEntry<float> AspectVeiledBaseDodgeGain { get; set; }
		public static ConfigEntry<float> AspectVeiledStackDodgeGain { get; set; }
		public static ConfigEntry<float> AspectVeiledBaseMovementGain { get; set; }
		public static ConfigEntry<float> AspectVeiledStackMovementGain { get; set; }
		public static ConfigEntry<bool> AspectVeiledCloakOnSpawn { get; set; }
		public static ConfigEntry<bool> AspectVeiledCloakPassive { get; set; }
		public static ConfigEntry<bool> AspectVeiledCloakOnly { get; set; }
		public static ConfigEntry<float> AspectVeiledElusiveDuration { get; set; }
		public static ConfigEntry<bool> AspectVeiledElusiveDecay { get; set; }
		public static ConfigEntry<bool> AspectVeiledElusiveRefresh { get; set; }
		public static ConfigEntry<float> AspectVeiledElusiveEffectMultWithNem { get; set; }
		public static ConfigEntry<float> AspectVeiledElusiveMovementGain { get; set; }
		public static ConfigEntry<float> AspectVeiledElusiveDodgeGain { get; set; }
		public static ConfigEntry<float> AspectVeiledElusiveStackEffect { get; set; }

		public static ConfigEntry<float> AspectAragoniteBaseMovementGain { get; set; }
		public static ConfigEntry<float> AspectAragoniteStackMovementGain { get; set; }
		public static ConfigEntry<float> AspectAragoniteAllyMovementGain { get; set; }
		public static ConfigEntry<float> AspectAragoniteMonsterMovementMult { get; set; }
		public static ConfigEntry<float> AspectAragoniteBaseAtkSpdGain { get; set; }
		public static ConfigEntry<float> AspectAragoniteStackAtkSpdGain { get; set; }
		public static ConfigEntry<float> AspectAragoniteAllyAtkSpdGain { get; set; }
		public static ConfigEntry<float> AspectAragoniteMonsterAtkSpdMult { get; set; }
		public static ConfigEntry<float> AspectAragoniteBaseCooldownGain { get; set; }
		public static ConfigEntry<float> AspectAragoniteStackCooldownGain { get; set; }
		public static ConfigEntry<float> AspectAragoniteAllyCooldownGain { get; set; }
		public static ConfigEntry<float> AspectAragoniteMonsterCooldownMult { get; set; }

		public static ConfigEntry<float> AspectGoldBaseRegenGain { get; set; }
		public static ConfigEntry<float> AspectGoldStackRegenGain { get; set; }
		public static ConfigEntry<float> AspectGoldItemScoreFactor { get; set; }
		public static ConfigEntry<float> AspectGoldItemScoreExponent { get; set; }
		public static ConfigEntry<float> AspectGoldItemScoreLevelScaling { get; set; }
		public static ConfigEntry<float> AspectGoldBaseScoredRegenGain { get; set; }
		public static ConfigEntry<float> AspectGoldStackScoredRegenGain { get; set; }

		public static ConfigEntry<float> AspectSanguineBaseDotAmp { get; set; }
		public static ConfigEntry<float> AspectSanguineStackDotAmp { get; set; }
		public static ConfigEntry<float> AspectSanguineBleedDuration { get; set; }
		public static ConfigEntry<float> AspectSanguineBaseDamage { get; set; }
		public static ConfigEntry<float> AspectSanguineStackDamage { get; set; }
		public static ConfigEntry<float> AspectSanguineMonsterDamageMult { get; set; }

		public static ConfigEntry<float> AspectSepiaBaseCooldownGain { get; set; }
		public static ConfigEntry<float> AspectSepiaStackCooldownGain { get; set; }
		public static ConfigEntry<float> AspectSepiaBaseRegenGain { get; set; }
		public static ConfigEntry<float> AspectSepiaStackRegenGain { get; set; }
		public static ConfigEntry<float> AspectSepiaBaseDodgeGain { get; set; }
		public static ConfigEntry<float> AspectSepiaStackDodgeGain { get; set; }
		public static ConfigEntry<float> AspectSepiaBlindDuration { get; set; }
		public static ConfigEntry<float> AspectSepiaBlindDodgeEffect { get; set; }

		public static ConfigEntry<bool> AspectNullifierOverrideShield { get; set; }
		public static ConfigEntry<bool> AspectNullifierShieldRecharge { get; set; }
		public static ConfigEntry<float> AspectNullifierHealthConverted { get; set; }
		public static ConfigEntry<float> AspectNullifierRegen { get; set; }
		public static ConfigEntry<float> AspectNullifierBaseShieldGain { get; set; }
		public static ConfigEntry<float> AspectNullifierStackShieldGain { get; set; }
		public static ConfigEntry<bool> AspectNullifierOverrideAllyArmor { get; set; }
		public static ConfigEntry<float> AspectNullifierAllyArmorGain { get; set; }
		public static ConfigEntry<float> AspectNullifierBaseArmorGain { get; set; }
		public static ConfigEntry<float> AspectNullifierStackArmorGain { get; set; }

		public static ConfigEntry<float> AspectBlightedBaseCooldownGain { get; set; }
		public static ConfigEntry<float> AspectBlightedStackCooldownGain { get; set; }
		public static ConfigEntry<float> AspectBlightedBaseHealthGain { get; set; }
		public static ConfigEntry<float> AspectBlightedStackHealthGain { get; set; }
		public static ConfigEntry<float> AspectBlightedBaseDamageGain { get; set; }
		public static ConfigEntry<float> AspectBlightedStackDamageGain { get; set; }

		public static ConfigEntry<int> AspectBackupBaseChargesGain { get; set; }
		public static ConfigEntry<int> AspectBackupStackChargesGain { get; set; }
		public static ConfigEntry<float> AspectBackupBaseCooldownGain { get; set; }
		public static ConfigEntry<float> AspectBackupStackCooldownGain { get; set; }

		public static ConfigEntry<float> AspectPurityBaseHealthGain { get; set; }
		public static ConfigEntry<float> AspectPurityStackHealthGain { get; set; }
		public static ConfigEntry<float> AspectPurityBaseRegenGain { get; set; }
		public static ConfigEntry<float> AspectPurityStackRegenGain { get; set; }

		public static ConfigEntry<bool> AspectBarrierPlayerHealthReduction { get; set; }
		public static ConfigEntry<float> AspectBarrierBaseDamageReductionGain { get; set; }
		public static ConfigEntry<float> AspectBarrierStackDamageReductionGain { get; set; }
		public static ConfigEntry<float> AspectBarrierBaseBarrierDamageReductionGain { get; set; }
		public static ConfigEntry<float> AspectBarrierStackBarrierDamageReductionGain { get; set; }

		public static ConfigEntry<float> AspectBlackHoleBaseDamageGain { get; set; }
		public static ConfigEntry<float> AspectBlackHoleStackDamageGain { get; set; }

		public static ConfigEntry<float> AspectMoneyBaseRegenGain { get; set; }
		public static ConfigEntry<float> AspectMoneyStackRegenGain { get; set; }
		public static ConfigEntry<float> AspectMoneyBaseGoldMult { get; set; }
		public static ConfigEntry<float> AspectMoneyStackGoldMult { get; set; }

		public static ConfigEntry<float> AspectNightBlindDodgeEffect { get; set; }
		public static ConfigEntry<float> AspectNightBaseMovementGain { get; set; }
		public static ConfigEntry<float> AspectNightStackMovementGain { get; set; }
		public static ConfigEntry<float> AspectNightMonsterMovementMult { get; set; }
		public static ConfigEntry<float> AspectNightBaseAtkSpdGain { get; set; }
		public static ConfigEntry<float> AspectNightStackAtkSpdGain { get; set; }
		public static ConfigEntry<float> AspectNightMonsterAtkSpdMult { get; set; }
		public static ConfigEntry<float> AspectNightBaseSafeMovementGain { get; set; }
		public static ConfigEntry<float> AspectNightStackSafeMovementGain { get; set; }
		public static ConfigEntry<float> AspectNightMonsterSafeMovementMult { get; set; }
		public static ConfigEntry<float> AspectNightBaseSafeAtkSpdGain { get; set; }
		public static ConfigEntry<float> AspectNightStackSafeAtkSpdGain { get; set; }
		public static ConfigEntry<float> AspectNightMonsterSafeAtkSpdMult { get; set; }

		public static ConfigEntry<float> AspectWaterBaseCooldownGain { get; set; }
		public static ConfigEntry<float> AspectWaterStackCooldownGain { get; set; }

		public static ConfigEntry<float> AspectRealgarBaseDotAmp { get; set; }
		public static ConfigEntry<float> AspectRealgarStackDotAmp { get; set; }

		public static ConfigEntry<bool> AspectOppressiveExtraJump { get; set; }
		public static ConfigEntry<float> AspectOppressiveBaseMovementGain { get; set; }
		public static ConfigEntry<float> AspectOppressiveStackMovementGain { get; set; }

		public static ConfigEntry<float> AspectBufferedBaseDamageReductionGain { get; set; }
		public static ConfigEntry<float> AspectBufferedStackDamageReductionGain { get; set; }
		public static ConfigEntry<float> AspectBufferedBaseBarrierDamageReductionGain { get; set; }
		public static ConfigEntry<float> AspectBufferedStackBarrierDamageReductionGain { get; set; }

		public static ConfigEntry<bool> AspectEmpoweringExtraJump { get; set; }
		public static ConfigEntry<float> AspectEmpoweringBaseDamageGain { get; set; }
		public static ConfigEntry<float> AspectEmpoweringStackDamageGain { get; set; }

		public static ConfigEntry<float> AspectFrenziedBaseMovementGain { get; set; }
		public static ConfigEntry<float> AspectFrenziedStackMovementGain { get; set; }
		public static ConfigEntry<float> AspectFrenziedMonsterMovementMult { get; set; }
		public static ConfigEntry<float> AspectFrenziedBaseAtkSpdGain { get; set; }
		public static ConfigEntry<float> AspectFrenziedStackAtkSpdGain { get; set; }
		public static ConfigEntry<float> AspectFrenziedMonsterAtkSpdMult { get; set; }
		public static ConfigEntry<float> AspectFrenziedBaseCooldownGain { get; set; }
		public static ConfigEntry<float> AspectFrenziedStackCooldownGain { get; set; }
		public static ConfigEntry<float> AspectFrenziedMonsterCooldownMult { get; set; }

		public static ConfigEntry<float> AspectVolatileBaseDamage { get; set; }
		public static ConfigEntry<float> AspectVolatileStackDamage { get; set; }
		public static ConfigEntry<float> AspectVolatileMonsterDamageMult { get; set; }

		public static ConfigEntry<float> AspectEchoBaseMinionDamageResistGain { get; set; }
		public static ConfigEntry<float> AspectEchoStackMinionDamageResistGain { get; set; }
		public static ConfigEntry<float> AspectEchoBaseCooldownGain { get; set; }
		public static ConfigEntry<float> AspectEchoStackCooldownGain { get; set; }
		public static ConfigEntry<float> AspectEchoMonsterCooldownMult { get; set; }

		public static ConfigEntry<float> AspectArmorBaseArmorGain { get; set; }
		public static ConfigEntry<float> AspectArmorStackArmorGain { get; set; }

		public static ConfigEntry<float> AspectBannerBaseDamageGain { get; set; }
		public static ConfigEntry<float> AspectBannerStackDamageGain { get; set; }
		public static ConfigEntry<bool> AspectBannerExtraJump { get; set; }
		public static ConfigEntry<bool> AspectBannerTweaks { get; set; }
		public static ConfigEntry<bool> AspectBannerBoth { get; set; }

		public static ConfigEntry<float> AspectImpaleDamageMult { get; set; }
		public static ConfigEntry<float> AspectImpaleBaseDotAmp { get; set; }
		public static ConfigEntry<float> AspectImpaleStackDotAmp { get; set; }
		public static ConfigEntry<bool> AspectImpaleTweaks { get; set; }

		public static ConfigEntry<float> AspectGoldenBaseRegenGain { get; set; }
		public static ConfigEntry<float> AspectGoldenStackRegenGain { get; set; }
		public static ConfigEntry<float> AspectGoldenItemScoreFactor { get; set; }
		public static ConfigEntry<float> AspectGoldenItemScoreExponent { get; set; }
		public static ConfigEntry<float> AspectGoldenItemScoreLevelScaling { get; set; }
		public static ConfigEntry<float> AspectGoldenBaseScoredRegenGain { get; set; }
		public static ConfigEntry<float> AspectGoldenStackScoredRegenGain { get; set; }

		public static ConfigEntry<float> AspectCycloneBaseMovementGain { get; set; }
		public static ConfigEntry<float> AspectCycloneStackMovementGain { get; set; }
		public static ConfigEntry<float> AspectCycloneBaseAtkSpdGain { get; set; }
		public static ConfigEntry<float> AspectCycloneStackAtkSpdGain { get; set; }
		public static ConfigEntry<float> AspectCycloneBaseDodgeGain { get; set; }
		public static ConfigEntry<float> AspectCycloneStackDodgeGain { get; set; }
		public static ConfigEntry<float> AspectCycloneBlindDodgeEffect { get; set; }
		public static ConfigEntry<bool> AspectCycloneTweaks { get; set; }
		public static ConfigEntry<float> AspectCycloneProc { get; set; }

		public static ConfigEntry<float> AspectTinkerBaseMinionDamageResistGain { get; set; }
		public static ConfigEntry<float> AspectTinkerStackMinionDamageResistGain { get; set; }
		public static ConfigEntry<float> AspectTinkerBaseMinionDamageGain { get; set; }
		public static ConfigEntry<float> AspectTinkerStackMinionDamageGain { get; set; }
		public static ConfigEntry<bool> AspectTinkerTweaks { get; set; }
		public static ConfigEntry<int> AspectTinkerMonsterLimit { get; set; }
		public static ConfigEntry<float> AspectTinkerMonsterDamageMult { get; set; }
		public static ConfigEntry<float> AspectTinkerMonsterHealthMult { get; set; }
		public static ConfigEntry<int> AspectTinkerPlayerLimit { get; set; }
		public static ConfigEntry<float> AspectTinkerPlayerDamageMult { get; set; }
		public static ConfigEntry<float> AspectTinkerPlayerHealthMult { get; set; }

		public static ConfigEntry<float> AspectMotivatorBaseDamageGain { get; set; }
		public static ConfigEntry<float> AspectMotivatorStackDamageGain { get; set; }
		public static ConfigEntry<bool> AspectMotivatorExtraJump { get; set; }
		public static ConfigEntry<bool> AspectMotivatorTweaks { get; set; }
		public static ConfigEntry<bool> AspectMotivatorEmpowerSelf { get; set; }

		public static ConfigEntry<float> AspectOsmiumBaseCooldownGain { get; set; }
		public static ConfigEntry<float> AspectOsmiumStackCooldownGain { get; set; }
		public static ConfigEntry<bool> AspectOsmiumPlayerNearbyNormal { get; set; }

		public static ConfigEntry<float> AspectEmpyreanBaseCooldownGain { get; set; }
		public static ConfigEntry<float> AspectEmpyreanStackCooldownGain { get; set; }
		public static ConfigEntry<float> AspectEmpyreanBaseHealthGain { get; set; }
		public static ConfigEntry<float> AspectEmpyreanStackHealthGain { get; set; }
		public static ConfigEntry<float> AspectEmpyreanBaseDamageGain { get; set; }
		public static ConfigEntry<float> AspectEmpyreanStackDamageGain { get; set; }



		public static bool ValidElusiveModifier = false;



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
			AspectCommandGroupItems = Config.Bind(
				"0a-General", "aspectCommandGroupItems", true,
				"If world unique is enabled, be able to choose any other itemized aspect in command."
			);
			AspectCommandGroupEquip = Config.Bind(
				"0a-General", "aspectCommandGroupEquip", true,
				"Be able to choose any other equipment aspect in command."
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

			LogbookHideItem = Config.Bind(
				"0a-General", "logbookHideItem", false,
				"Hide itemized aspects from the logbook."
			);
			LogbookHideEquipment = Config.Bind(
				"0a-General", "logbookHideEquipment", false,
				"Hide aspect equipment from the logbook."
			);



			AspectDropVerboseLogging = Config.Bind(
				"0b-DropChance", "aspectDropVerboseLogging", false,
				"Log the results of aspect drop chance for each elite killed."
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

				foreach (AspectDef aspectDef in Catalog.aspectDefs)
				{
					aspectDef.CreateDropWeightConfig(Config);
				}

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



			MonsterRegenExponent = Config.Bind(
				"0d-Tweaks", "monsterRegenExponent", 0.75f,
				"Health regeneration scaling exponent on monsters. Value is an exponent on monster level used to apply level regeneration scaling. Applies to flat health regeneration only."
			);
			TranscendenceRegen = Config.Bind(
				"0d-Tweaks", "transcendenceShieldRegen", 0.50f,
				"Health regeneration gained as shield regeneration. Set to 0 to disable."
			);
			ShieldRegenBreakDelay = Config.Bind(
				"0d-Tweaks", "shieldRegenBreakDelay", 3f,
				"Shield regeneration gained from health regeneration is disabled while shields are below 1. Set the time since last hit for shield regeneration to reactivate. Set to 0 so that shield regeneration is always active."
			);
			NearbyPlayerDodgeBypass = Config.Bind(
				"0d-Tweaks", "playerBypassDodge", 20f,
				"Monster dodge chance is ignored if player is near target. Value is distance to trigger. 0 to disable."
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
			UpdateInventoryFromBuff = Config.Bind(
				"21-Mod Compatibility", "updateInventoryFromBuff", 1,
				"Adds a hidden item whenever a buff expires. 2 = all buffs, 1 = only aspect buffs, 0 = none"
			);

			AetheriumHooks = Config.Bind(
				"21-Mod Compatibility", "aetheriumHooks", true,
				"Allows for the reading and modification of functions and values within the Aetherium mod."
			);
			EliteReworksHooks = Config.Bind(
				"21-Mod Compatibility", "eliteReworksHooks", true,
				"Allows for the reading and modification of functions and values within the EliteReworks mod."
			);
			SpikeStripHooks = Config.Bind(
				"21-Mod Compatibility", "spikeStripHooks", true,
				"Allows for the reading and modification of functions and values within the SpikeStrip mod."
			);
			WarWispHooks = Config.Bind(
				"21-Mod Compatibility", "warWispHooks", true,
				"Allows for the reading and modification of functions and values within the WispWarframe mod. Must be enabled to allow itemized aspect to function."
			);
			BlightedHooks = Config.Bind(
				"21-Mod Compatibility", "blightedHooks", true,
				"Allows for the reading and modification of functions and values within the BlightedElites mod. Should be enabled to allow better buff control."
			);
			GotceHooks = Config.Bind(
				"21-Mod Compatibility", "gotceHooks", true,
				"Allows for the reading and modification of functions and values within the GOTCE mod."
			);
			RisingTidesHooks = Config.Bind(
				"21-Mod Compatibility", "risingTidesHooks", true,
				"Allows for the reading and modification of functions and values within the RisingTides mod."
			);
			MoreElitesHooks = Config.Bind(
				"21-Mod Compatibility", "moreElitesHooks", true,
				"Allows for the reading and modification of functions and values within the MoreElites mod."
			);
			EliteVarietyHooks = Config.Bind(
				"21-Mod Compatibility", "eliteVarietyHooks", true,
				"Allows for the reading and modification of functions and values within the EliteVariety mod."
			);
			AugmentumHooks = Config.Bind(
				"21-Mod Compatibility", "augmentumHooks", true,
				"Allows for the reading and modification of functions and values within the Augmentum mod. Must be enabled to allow itemized aspect to function."
			);
			SandsweptHooks = Config.Bind(
				"21-Mod Compatibility", "sandsweptHooks", true,
				"Allows for the reading and modification of functions and values within the Sandswept mod."
			);
			StarstormHooks = Config.Bind(
				"21-Mod Compatibility", "starstormHooks", true,
				"Allows for the reading and modification of functions and values within the Starstorm mod."
			);

			RiskOfRainConfigs(Config);
			SpikeStripConfigs(Config);
			GoldenCoastPlusConfigs(Config);
			AetheriumConfigs(Config);
			BubbetConfigs(Config);
			WarWispConfigs(Config);
			BlightedConfigs(Config);
			GotceConfigs(Config);
			ThalassoConfigs(Config);
			RisingTidesConfigs(Config);
			NemRisingTidesConfigs(Config);
			MoreElitesConfigs(Config);
			EliteVarietyConfigs(Config);

			SandsweptConfigs(Config);
			StarstormConfigs(Config);
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
			AspectWhiteThornsProc = Config.Bind(
				"2aa-AspectWhite", "whiteThornsProc", true,
				"Use ProcType.Thorns on blades, helps prevent infinite chaining."
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
			AspectBlueEliteReworksDamage = Config.Bind(
				"2ab-AspectBlue", "blueEliteReworksDamage", true,
				"Override damage multiplier value in EliteReworks Overloading OnHit effects."
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
			AspectHauntedAllyDodgeGain = Config.Bind(
				"2ad-AspectHaunted", "hauntedAllyDodgeChance", 0f,
				"Dodge chance granted to nearby allies. Effect only applies to allies without aspect and does not stack. Set to 0 to disable."
			);
			AspectHauntedBaseArmorGain = Config.Bind(
				"2ad-AspectHaunted", "hauntedBaseArmor", 60f,
				"Armor gained. Set to 0 to disable."
			);
			AspectHauntedStackArmorGain = Config.Bind(
				"2ad-AspectHaunted", "hauntedAddedArmor", 30f,
				"Armor gained per stack."
			);
			AspectHauntedBaseDodgeGain = Config.Bind(
				"2ad-AspectHaunted", "hauntedBaseDodgeChance", 0f,
				"Dodge chance gained. Set to 0 to disable."
			);
			AspectHauntedStackDodgeGain = Config.Bind(
				"2ad-AspectHaunted", "hauntedAddedDodgeChance", 0f,
				"Dodge chance gained per stack."
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
				"Health regeneration gained as shield regeneration. Set to 0 to disable."
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
				"Percent health regeneration gained. Monsters need to be outOfCombat. Set to 0 to disable."
			);
			AspectEarthPoachedDuration = Config.Bind(
				"2ag-AspectEarth", "earthPoachedDuration", 4.0f,
				"Set poached duration in seconds. Set to 0 to disable."
			);
			AspectEarthPoachedAttackSpeed = Config.Bind(
				"2ag-AspectEarth", "earthPoachedAttackSpeed", 0.10f,
				"Attack speed reduction of poached. Set to 0 to disable."
			);
			AspectEarthLeechModifier = Config.Bind(
				"2ag-AspectEarth", "earthLeechModifier", 1,
				"Leech modifier. 0 = None, 1 = POW, 2 = LOG."
			);
			AspectEarthLeechModMult = Config.Bind(
				"2ag-AspectEarth", "earthLeechModMult", 1f,
				"Multiply leech value before modifier applied. Only applied when a modifier is enabled."
			);
			AspectEarthLeechParameter = Config.Bind(
				"2ag-AspectEarth", "earthLeechModParameter", 0.65f,
				"Leech modifier parameter."
			);
			AspectEarthLeechPostModMult = Config.Bind(
				"2ag-AspectEarth", "earthLeechPostModMult", 2f,
				"Multiply leech value after modifier applied. Only applied when a modifier is enabled."
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
				"Monster leech multiplier. Unaffected by Leech Modifier."
			);



			AspectVoidContagiousItem = Config.Bind(
				"2ah-AspectVoid", "voidContagiousItem", false,
				"Corrupt other aspect items into itself. Sets item ItemTier to respective void ItemTier."
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
			AspectPlatedStifleMult = Config.Bind(
				"2ba-AspectPlated", "platedStifleMult", 0.5f,
				"Multiplier applied to damage reduction of stiffle."
			);
			AspectPlatedStifleExponent = Config.Bind(
				"2ba-AspectPlated", "platedStifleExponent", 0.5f,
				"Exponent applied to buff count of stiffle for damage calculations."
			);
			AspectPlatedPlayerPlateCountMult = Config.Bind(
				"2ba-AspectPlated", "platedPlayerPlateCountMult", 1f,
				"Multiplier applied to number of health plates for players."
			);
			AspectPlatedMonsterPlateCountMult = Config.Bind(
				"2ba-AspectPlated", "platedMonsterPlateCountMult", 0.275f,
				"Multiplier applied to number of health plates for monsters."
			);
			AspectPlatedBaseArmorGain = Config.Bind(
				"2ba-AspectPlated", "platedBaseArmor", 30f,
				"Armor gained. Set to 0 to disable."
			);
			AspectPlatedStackArmorGain = Config.Bind(
				"2ba-AspectPlated", "platedAddedArmor", 15f,
				"Armor gained per stack."
			);
			AspectPlatedBasePlatingGain = Config.Bind(
				"2ba-AspectPlated", "platedBasePlating", 20f,
				"Incoming damage reduction gained. Set to 0 to disable."
			);
			AspectPlatedStackPlatingGain = Config.Bind(
				"2ba-AspectPlated", "platedAddedPlating", 10f,
				"Incoming damage reduction gained per stack."
			);
			AspectPlatedMonsterPlatingMult = Config.Bind(
				"2ba-AspectPlated", "platedMonsterPlatingMult", 0.5f,
				"Monster incoming damage reduction multiplier."
			);
			AspectPlatedPlayerHealthReduction = Config.Bind(
				"2ba-AspectPlated", "platedPlayerHealthReduction", true,
				"NemSpikeStrip - Apply health reduction to player team."
			);
			AspectPlatedAllowDefenceWithNem = Config.Bind(
				"2ba-AspectPlated", "platedAllowDefenceWithNem", false,
				"NemSpikeStrip - Apply armor and damage reduction with NemSpikeStrip installed."
			);



			AspectWarpedLiftForce = Config.Bind(
				"2bb-AspectWarped", "warpedLiftForce", 100f,
				"Lift force applied by anti-gravity bubble."
			);
			AspectWarpedAltDebuff = Config.Bind(
				"2bb-AspectWarped", "warpedAltDebuff", 0,
				"Change debuff into a buff that reduces jump power and movement. 0 = dont change, 1 = only change debuff applied to players, 2 = change debuff for everything."
			);
			AspectWarpedAltJumpMult = Config.Bind(
				"2bb-AspectWarped", "warpedAltJumpMult", 0.75f,
				"Jump power multiplier from alt debuff."
			);
			AspectWarpedAltSpeedMult = Config.Bind(
				"2bb-AspectWarped", "warpedAltSpeedMult", 0.75f,
				"Movement speed multiplier from alt debuff."
			);
			AspectWarpedAltAccelerationMult = Config.Bind(
				"2bb-AspectWarped", "warpedAltAccelerationMult", 0.5f,
				"Acceleration multiplier from alt debuff."
			);
			AspectWarpedBaseCooldownGain = Config.Bind(
				"2bb-AspectWarped", "warpedBaseCooldown", 0.2f,
				"Cooldown reduction gained. Set to 0 to disable."
			);
			AspectWarpedStackCooldownGain = Config.Bind(
				"2bb-AspectWarped", "warpedAddedCooldown", 0.1f,
				"Cooldown reduction gained per stack."
			);
			AspectWarpedBaseFallReductionGain = Config.Bind(
				"2bb-AspectWarped", "warpedBaseFallReduction", 0.5f,
				"Fall damage reduction gained. Set to 0 to disable. Set to 1000 to become immune."
			);
			AspectWarpedStackFallReductionGain = Config.Bind(
				"2bb-AspectWarped", "warpedAddedFallReduction", 0.25f,
				"Fall damage reduction gained per stack."
			);
			AspectWarpedBaseForceResistGain = Config.Bind(
				"2bb-AspectWarped", "warpedBaseForceResist", 1000f,
				"Knockback resistance gained. Set to 0 to disable. Set to 1000 to become immune."
			);
			AspectWarpedStackForceResistGain = Config.Bind(
				"2bb-AspectWarped", "warpedAddedForceResist", 0f,
				"Knockback resistance gained per stack."
			);



			AspectVeiledBaseDodgeGain = Config.Bind(
				"2bc-AspectVeiled", "veiledBaseDodgeChance", 25f,
				"Dodge chance gained. Set to 0 to disable."
			);
			AspectVeiledStackDodgeGain = Config.Bind(
				"2bc-AspectVeiled", "veiledAddedDodgeChance", 0f,
				"Dodge chance gained per stack."
			);
			AspectVeiledBaseMovementGain = Config.Bind(
				"2bc-AspectVeiled", "veiledBaseMovementGained", 0.20f,
				"Movement speed gained. Set to 0 to disable."
			);
			AspectVeiledStackMovementGain = Config.Bind(
				"2bc-AspectVeiled", "veiledAddedMovementGained", 0f,
				"Movement speed gained per stack."
			);
			AspectVeiledCloakOnSpawn = Config.Bind(
				"2bc-AspectVeiled", "veiledCloakOnSpawn", true,
				"Allow Veiled elites to cloak for 30 seconds whenever they spawn."
			);
			AspectVeiledCloakPassive = Config.Bind(
				"2bc-AspectVeiled", "veiledCloakPassive", true,
				"Allow Veiled elites to passively cloak."
			);
			AspectVeiledCloakOnly = Config.Bind(
				"2bc-AspectVeiled", "veiledCloakOnly", false,
				"Dont apply elusive and only cloak. Affected by duration and refresh configs."
			);
			AspectVeiledElusiveDuration = Config.Bind(
				"2bc-AspectVeiled", "veiledElusiveDuration", 4f,
				"Base duration of elusive. Increased elusive effect also applies to buff duration if effect decays. Set to 0 to disable."
			);
			AspectVeiledElusiveDecay = Config.Bind(
				"2bc-AspectVeiled", "veiledElusiveDecay", true,
				"Whether elusive effect decays over its duration."
			);
			AspectVeiledElusiveRefresh = Config.Bind(
				"2bc-AspectVeiled", "veiledElusiveRefresh", true,
				"Whether elusive can be refreshed while you are already elusive."
			);
			AspectVeiledElusiveEffectMultWithNem = Config.Bind(
				"2bc-AspectVeiled", "veiledElusiveEffectMultWithNem", 0.50f,
				"Multiplier on magnitude of elusive effects with NemSpikeStrip installed."
			);
			AspectVeiledElusiveMovementGain = Config.Bind(
				"2bc-AspectVeiled", "veiledElusiveMovementGained", 0.40f,
				"Movement speed gained from 100% elusive effect. Set to 0 to disable."
			);
			AspectVeiledElusiveMovementGain.SettingChanged += ValidateElusiveModifier;
			AspectVeiledElusiveDodgeGain = Config.Bind(
				"2bc-AspectVeiled", "vailedElusiveDodgeGained", 50f,
				"Dodge chance gained from 100% elusive effect. Set to 0 to disable."
			);
			AspectVeiledElusiveStackEffect = Config.Bind(
				"2bc-AspectVeiled", "vailedElusiveStackEffect", 0.25f,
				"Elusive effect gained per stack. Elusive effect is rounded into 5% intervals."
			);
			AspectVeiledElusiveMovementGain.SettingChanged += ValidateElusiveModifier;

			ValidateElusiveModifier();



			AspectAragoniteBaseMovementGain = Config.Bind(
				"2bd-AspectAragonite", "aragoniteBaseMovementGained", 0.30f,
				"Movement speed gained. Set to 0 to disable."
			);
			AspectAragoniteStackMovementGain = Config.Bind(
				"2bd-AspectAragonite", "aragoniteAddedMovementGained", 0.15f,
				"Movement speed gained per stack."
			);
			AspectAragoniteAllyMovementGain = Config.Bind(
				"2bd-AspectAragonite", "aragoniteAllyMovementGained", 0.20f,
				"Movement speed granted to nearby allies. Effect only applies to allies without aspect and does not stack. Set to 0 to disable."
			);
			AspectAragoniteMonsterMovementMult = Config.Bind(
				"2bd-AspectAragonite", "aragoniteMonsterMovementMult", 2f,
				"Monster movement speed gain multiplier."
			);
			AspectAragoniteBaseAtkSpdGain = Config.Bind(
				"2bd-AspectAragonite", "aragoniteBaseAtkSpdGained", 0.30f,
				"Attack speed gained. Set to 0 to disable."
			);
			AspectAragoniteStackAtkSpdGain = Config.Bind(
				"2bd-AspectAragonite", "aragoniteAddedAtkSpdGained", 0.15f,
				"Attack speed gained per stack."
			);
			AspectAragoniteAllyAtkSpdGain = Config.Bind(
				"2bd-AspectAragonite", "aragoniteAllyAtkSpdGained", 0.20f,
				"Attack speed granted to nearby allies. Effect only applies to allies without aspect and does not stack. Set to 0 to disable."
			);
			AspectAragoniteMonsterAtkSpdMult = Config.Bind(
				"2bd-AspectAragonite", "aragoniteMonsterAtkSpdMult", 1f,
				"Monster attack speed gain multiplier."
			);
			AspectAragoniteBaseCooldownGain = Config.Bind(
				"2bd-AspectAragonite", "aragoniteBaseCooldown", 0.30f,
				"Cooldown reduction gained. Set to 0 to disable."
			);
			AspectAragoniteStackCooldownGain = Config.Bind(
				"2bd-AspectAragonite", "aragoniteAddedCooldown", 0.15f,
				"Cooldown reduction gained per stack."
			);
			AspectAragoniteAllyCooldownGain = Config.Bind(
				"2bd-AspectAragonite", "aragoniteAllyCooldown", 0.20f,
				"Cooldown reduction granted to nearby allies. Effect only applies to allies without aspect and does not stack. Set to 0 to disable."
			);
			AspectAragoniteMonsterCooldownMult = Config.Bind(
				"2bd-AspectAragonite", "aragoniteMonsterCooldownMult", 1f,
				"Monster cooldown reduction gain multiplier."
			);
		}

		private static void GoldenCoastPlusConfigs(ConfigFile Config)
		{
			AspectGoldBaseRegenGain = Config.Bind(
				"2ca-Gold Aspect", "goldBaseRegen", 12f,
				"Health regeneration gained. Set to 0 to disable."
			);
			AspectGoldStackRegenGain = Config.Bind(
				"2ca-Gold Aspect", "goldAddedRegen", 6f,
				"Health regeneration gained per stack."
			);
			AspectGoldItemScoreFactor = Config.Bind(
				"2ca-Gold Aspect", "goldItemScoreFactor", 2.0f,
				"Multiply itemscore by value. Applies before itemscore exponent. white = 1x, green = 3x, other = 9x."
			);
			AspectGoldItemScoreExponent = Config.Bind(
				"2ca-Gold Aspect", "goldItemScoreExponent", 0.65f,
				"Itemscore exponent. Raise itemscore to the power of value."
			);
			AspectGoldItemScoreLevelScaling = Config.Bind(
				"2ca-Gold Aspect", "goldItemScoreLevelScaling", 0.1f,
				"Itemscore regeneration level scaling. Vanilla regeneration scaling is 0.2 = +100% every 5 levels."
			);
			AspectGoldBaseScoredRegenGain = Config.Bind(
				"2ca-Gold Aspect", "goldBaseScoredRegen", 1.0f,
				"ScoredRegen multiplier gained. Applies after itemscore exponent. Set to 0 to disable itemscore regen."
			);
			AspectGoldStackScoredRegenGain = Config.Bind(
				"2ca-Gold Aspect", "goldAddedScoredRegen", 0.5f,
				"ScoredRegen multiplier gained per stack."
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
				"2da-Sanguine Aspect", "sanguineBaseTotalDamage", 1.2f,
				"Base total damage of bleed over duration. Set to 0 to disable."
			);
			AspectSanguineStackDamage = Config.Bind(
				"2da-Sanguine Aspect", "sanguineAddedTotalDamage", 0.6f,
				"Added total damage of bleed per stack."
			);
			AspectSanguineMonsterDamageMult = Config.Bind(
				"2da-Sanguine Aspect", "sanguineMonsterDamageMult", 1f,
				"Monster bleed damage multiplier."
			);
		}

		private static void BubbetConfigs(ConfigFile Config)
		{
			AspectSepiaBaseCooldownGain = Config.Bind(
				"2ea-AspectSepia", "sepiaBaseCooldown", 0.2f,
				"Cooldown reduction gained. Set to 0 to disable."
			);
			AspectSepiaStackCooldownGain = Config.Bind(
				"2ea-AspectSepia", "sepiaAddedCooldown", 0.1f,
				"Cooldown reduction gained per stack."
			);
			AspectSepiaBaseRegenGain = Config.Bind(
				"2ea-AspectSepia", "sepiaBaseRegen", 12f,
				"Health regeneration gained. Set to 0 to disable."
			);
			AspectSepiaStackRegenGain = Config.Bind(
				"2ea-AspectSepia", "sepiaAddedRegen", 6f,
				"Health regeneration gained per stack."
			);
			AspectSepiaBaseDodgeGain = Config.Bind(
				"2ea-AspectSepia", "sepiaBaseDodgeChance", 35f,
				"Dodge chance gained. Set to 0 to disable."
			);
			AspectSepiaStackDodgeGain = Config.Bind(
				"2ea-AspectSepia", "sepiaAddedDodgeChance", 15f,
				"Dodge chance gained per stack."
			);
			AspectSepiaBlindDuration = Config.Bind(
				"2ea-AspectSepia", "sepiaBlindDuration", 4.0f,
				"Set blind duration in seconds. Set to 0 to disable."
			);
			AspectSepiaBlindDodgeEffect = Config.Bind(
				"2ea-AspectSepia", "sepiaBlindDodgeEffect", 25f,
				"Dodge chance effect from blind. Set to 0 to disable."
			);
		}

		private static void WarWispConfigs(ConfigFile Config)
		{
			AspectNullifierOverrideShield = Config.Bind(
				"2fa-AspectNullifier", "nullifierOverrideShield", true,
				"Override shield gained from affix, using these configs with recalcstats instead of the methods used in the WispWarframe mod."
			);
			AspectNullifierShieldRecharge = Config.Bind(
				"2fa-AspectNullifier", "nullifierShieldRecharge", true,
				"Override outOfCombatStopwatch method in WispWarframe mod. Re-enables shield recharge while out of danger."
			);
			AspectNullifierHealthConverted = Config.Bind(
				"2fa-AspectNullifier", "nullifierHealthConverted", 0.25f,
				"Health converted into shield. Set to 0 to disable."
			);
			AspectNullifierRegen = Config.Bind(
				"2fa-AspectNullifier", "nullifierShieldRegen", 0.50f,
				"Health regeneration gained as shield regeneration. This setting does not effect shield regeneration from the WispWarframe mod. Set to 0 to disable."
			);
			AspectNullifierBaseShieldGain = Config.Bind(
				"2fa-AspectNullifier", "nullifierBaseShieldGained", 0.20f,
				"Shield gained from health. Set to 0 to disable."
			);
			AspectNullifierStackShieldGain = Config.Bind(
				"2fa-AspectNullifier", "nullifierAddedShieldGained", 0.10f,
				"Shield gained from health per stack."
			);
			AspectNullifierOverrideAllyArmor = Config.Bind(
				"2fa-AspectNullifier", "nullifierOverrideAllyArmor", true,
				"Override armor granted to allies, Ally armor value from WispWarframe mod will be disabled."
			);
			AspectNullifierAllyArmorGain = Config.Bind(
				"2fa-AspectNullifier", "nullifierAllyArmor", 30f,
				"Armor granted to nearby allies. Must enable nullifierOverrideAllyArmor. Effect only applies to allies without aspect and does not stack. Set to 0 to disable."
			);
			AspectNullifierBaseArmorGain = Config.Bind(
				"2fa-AspectNullifier", "nullifierBaseArmor", 30f,
				"Armor gained. Set to 0 to disable."
			);
			AspectNullifierStackArmorGain = Config.Bind(
				"2fa-AspectNullifier", "nullifierAddedArmor", 15f,
				"Armor gained per stack."
			);
		}

		private static void BlightedConfigs(ConfigFile Config)
		{
			AspectBlightedBaseHealthGain = Config.Bind(
				"2ga-AspectBlighted", "blightedBaseHealthGained", 0.20f,
				"Health gained. Set to 0 to disable."
			);
			AspectBlightedStackHealthGain = Config.Bind(
				"2ga-AspectBlighted", "blightedAddedHealthGained", 0.10f,
				"Health gained per stack."
			);
			AspectBlightedBaseDamageGain = Config.Bind(
				"2ga-AspectBlighted", "blightedBaseDamageGained", 0.20f,
				"Damage gained. Set to 0 to disable."
			);
			AspectBlightedStackDamageGain = Config.Bind(
				"2ga-AspectBlighted", "blightedAddedDamageGained", 0.10f,
				"Damage gained per stack."
			);
			AspectBlightedBaseCooldownGain = Config.Bind(
				"2ga-AspectBlighted", "blightedBaseCooldown", 0.2f,
				"Cooldown reduction gained. Set to 0 to disable."
			);
			AspectBlightedStackCooldownGain = Config.Bind(
				"2ga-AspectBlighted", "blightedAddedCooldown", 0.1f,
				"Cooldown reduction gained per stack."
			);
		}

		private static void GotceConfigs(ConfigFile Config)
		{
			AspectBackupBaseChargesGain = Config.Bind(
				"2ha-AspectBackup", "backupBaseCharges", 2,
				"Secondary charges gained. Set to 0 to disable."
			);
			AspectBackupStackChargesGain = Config.Bind(
				"2ha-AspectBackup", "backupAddedCharges", 1,
				"Secondary charges gained per stack."
			);
			AspectBackupBaseCooldownGain = Config.Bind(
				"2ha-AspectBackup", "backupBaseCooldown", 0.5f,
				"Secondary cooldown reduction gained. Set to 0 to disable."
			);
			AspectBackupStackCooldownGain = Config.Bind(
				"2ha-AspectBackup", "backupAddedCooldown", 0.25f,
				"Secondary cooldown reduction gained per stack."
			);
		}

		private static void ThalassoConfigs(ConfigFile Config)
		{
			AspectPurityBaseHealthGain = Config.Bind(
				"2ia-AspectPurity", "purityBaseHealth", 200f,
				"Health gained. Set to 0 to disable."
			);
			AspectPurityStackHealthGain = Config.Bind(
				"2ia-AspectPurity", "purityAddedHealth", 100f,
				"Health gained per stack."
			);
			AspectPurityBaseRegenGain = Config.Bind(
				"2ia-AspectPurity", "purityBaseRegen", 12f,
				"Health regeneration gained. Set to 0 to disable."
			);
			AspectPurityStackRegenGain = Config.Bind(
				"2ia-AspectPurity", "purityAddedRegen", 6f,
				"Health regeneration gained per stack."
			);
		}

		private static void RisingTidesConfigs(ConfigFile Config)
		{
			AspectBarrierPlayerHealthReduction = Config.Bind(
				"2ja-AspectBarrier", "barrierPlayerHealthReduction", false,
				"Apply health reduction to player team."
			);
			AspectBarrierBaseDamageReductionGain = Config.Bind(
				"2ja-AspectBarrier", "barrierBaseDamageReduction", 0.3f,
				"Damage reduction gained. Set to 0 to disable."
			);
			AspectBarrierStackDamageReductionGain = Config.Bind(
				"2ja-AspectBarrier", "barrierStackDamageReduction", 0.15f,
				"Damage reduction gained per stack."
			);
			AspectBarrierBaseBarrierDamageReductionGain = Config.Bind(
				"2ja-AspectBarrier", "barrierBaseBarrierDamageReduction", 0f,
				"Damage reduction gained while barrier is active. Set to 0 to disable."
			);
			AspectBarrierStackBarrierDamageReductionGain = Config.Bind(
				"2ja-AspectBarrier", "barrierStackBarrierDamageReduction", 0f,
				"Damage reduction gained per stack while barrier is active."
			);



			AspectBlackHoleBaseDamageGain = Config.Bind(
				"2jb-AspectBlackHole", "blackHoleBaseDamageGained", 0.20f,
				"Damage gained. Set to 0 to disable."
			);
			AspectBlackHoleStackDamageGain = Config.Bind(
				"2jb-AspectBlackHole", "blackHoleAddedDamageGained", 0.10f,
				"Damage gained per stack."
			);



			AspectMoneyBaseRegenGain = Config.Bind(
				"2jc-AspectMoney", "moneyBaseRegen", 12f,
				"Health regeneration gained. Set to 0 to disable."
			);
			AspectMoneyStackRegenGain = Config.Bind(
				"2jc-AspectMoney", "moneyAddedRegen", 6f,
				"Health regeneration gained per stack."
			);
			AspectMoneyBaseGoldMult = Config.Bind(
				"2jc-AspectMoney", "moneyBaseGoldMult", 0.2f,
				"Gold from kills increase. Set to 0 to disable."
			);
			AspectMoneyStackGoldMult = Config.Bind(
				"2jc-AspectMoney", "moneyAddedGoldMult", 0.1f,
				"Gold from kills increase per stack."
			);



			AspectNightBlindDodgeEffect = Config.Bind(
				"2jd-AspectNight", "nightBlindDodgeEffect", 35f,
				"Dodge chance effect from blind. Set to 0 to disable."
			);
			AspectNightBaseMovementGain = Config.Bind(
				"2jd-AspectNight", "nightBaseMovementGained", 0.20f,
				"Movement speed gained. Set to 0 to disable."
			);
			AspectNightStackMovementGain = Config.Bind(
				"2jd-AspectNight", "nightAddedMovementGained", 0.10f,
				"Movement speed gained per stack."
			);
			AspectNightMonsterMovementMult = Config.Bind(
				"2jd-AspectNight", "nightMonsterMovementMult", 1f,
				"Monster movement speed gain multiplier."
			);
			AspectNightBaseAtkSpdGain = Config.Bind(
				"2jd-AspectNight", "nightBaseAtkSpdGained", 0.10f,
				"Attack speed gained. Set to 0 to disable."
			);
			AspectNightStackAtkSpdGain = Config.Bind(
				"2jd-AspectNight", "nightAddedAtkSpdGained", 0.1f,
				"Attack speed gained per stack."
			);
			AspectNightMonsterAtkSpdMult = Config.Bind(
				"2jd-AspectNight", "nightMonsterAtkSpdMult", 1f,
				"Monster attack speed gain multiplier."
			);
			AspectNightBaseSafeMovementGain = Config.Bind(
				"2jd-AspectNight", "nightBaseSafeMovementGained", 0.40f,
				"Movement speed gained while out of danger. Set to 0 to disable."
			);
			AspectNightStackSafeMovementGain = Config.Bind(
				"2jd-AspectNight", "nightAddedSafeMovementGained", 0f,
				"Movement speed gained while out of danger per stack."
			);
			AspectNightMonsterSafeMovementMult = Config.Bind(
				"2jd-AspectNight", "nightMonsterSafeMovementMult", 2f,
				"Monster movement speed gain while out of danger multiplier."
			);
			AspectNightBaseSafeAtkSpdGain = Config.Bind(
				"2jd-AspectNight", "nightBaseSafeAtkSpdGained", 0.20f,
				"Attack speed gained while out of danger. Set to 0 to disable."
			);
			AspectNightStackSafeAtkSpdGain = Config.Bind(
				"2jd-AspectNight", "nightAddedSafeAtkSpdGained", 0f,
				"Attack speed gained while out of danger per stack."
			);
			AspectNightMonsterSafeAtkSpdMult = Config.Bind(
				"2jd-AspectNight", "nightMonsterSafeAtkSpdMult", 1.5f,
				"Monster attack speed gain while out of danger multiplier."
			);



			AspectWaterBaseCooldownGain = Config.Bind(
				"2je-AspectWater", "waterBaseCooldown", 0.2f,
				"Cooldown reduction gained. Set to 0 to disable."
			);
			AspectWaterStackCooldownGain = Config.Bind(
				"2je-AspectWater", "waterAddedCooldown", 0.1f,
				"Cooldown reduction gained per stack."
			);



			AspectRealgarBaseDotAmp = Config.Bind(
				"2jf-AspectRealgar", "realgarBaseDotAmp", 0.20f,
				"DOT damage multiplier gained. Set to 0 to disable."
			);
			AspectRealgarStackDotAmp = Config.Bind(
				"2jf-AspectRealgar", "realgarAddedDotAmp", 0.10f,
				"DOT damage multiplier gained per stack."
			);
		}

		private static void NemRisingTidesConfigs(ConfigFile Config)
		{
			AspectOppressiveExtraJump = Config.Bind(
				"2ka-AspectOppressive", "oppressiveExtraJump", true,
				"Extra jump. Player Only"
			);
			AspectOppressiveBaseMovementGain = Config.Bind(
				"2ka-AspectOppressive", "oppressiveBaseMovementGained", 0.20f,
				"Movement speed gained. Set to 0 to disable."
			);
			AspectOppressiveStackMovementGain = Config.Bind(
				"2ka-AspectOppressive", "oppressiveAddedMovementGained", 0.10f,
				"Movement speed gained per stack."
			);

			AspectBufferedBaseDamageReductionGain = Config.Bind(
				"2kb-AspectBuffered", "bufferedBaseDamageReduction", 0f,
				"Damage reduction gained. Set to 0 to disable."
			);
			AspectBufferedStackDamageReductionGain = Config.Bind(
				"2kb-AspectBuffered", "bufferedStackDamageReduction", 0f,
				"Damage reduction gained per stack."
			);
			AspectBufferedBaseBarrierDamageReductionGain = Config.Bind(
				"2kb-AspectBuffered", "bufferedBaseBarrierDamageReduction", 0.3f,
				"Damage reduction gained while barrier is active. Set to 0 to disable."
			);
			AspectBufferedStackBarrierDamageReductionGain = Config.Bind(
				"2kb-AspectBuffered", "bufferedStackBarrierDamageReduction", 0.15f,
				"Damage reduction gained per stack while barrier is active."
			);
		}

		private static void MoreElitesConfigs(ConfigFile Config)
		{
			AspectEmpoweringExtraJump = Config.Bind(
				"2la-AspectEmpowering", "empoweringExtraJump", false,
				"Extra jump. Player Only"
			);
			AspectEmpoweringBaseDamageGain = Config.Bind(
				"2la-AspectEmpowering", "empoweringBaseDamageGained", 0.20f,
				"Damage gained. Set to 0 to disable."
			);
			AspectEmpoweringStackDamageGain = Config.Bind(
				"2la-AspectEmpowering", "empoweringAddedDamageGained", 0.10f,
				"Damage gained per stack."
			);

			AspectFrenziedBaseMovementGain = Config.Bind(
				"2lb-AspectFrenzied", "frenziedBaseMovementGained", 0.20f,
				"Movement speed gained. Set to 0 to disable."
			);
			AspectFrenziedStackMovementGain = Config.Bind(
				"2lb-AspectFrenzied", "frenziedAddedMovementGained", 0.10f,
				"Movement speed gained per stack."
			);
			AspectFrenziedMonsterMovementMult = Config.Bind(
				"2lb-AspectFrenzied", "frenziedMonsterMovementMult", 2.5f,
				"Monster movement speed gain multiplier."
			);
			AspectFrenziedBaseAtkSpdGain = Config.Bind(
				"2lb-AspectFrenzied", "frenziedBaseAtkSpdGained", 0.20f,
				"Attack speed gained. Set to 0 to disable."
			);
			AspectFrenziedStackAtkSpdGain = Config.Bind(
				"2lb-AspectFrenzied", "frenziedAddedAtkSpdGained", 0.10f,
				"Attack speed gained per stack."
			);
			AspectFrenziedMonsterAtkSpdMult = Config.Bind(
				"2lb-AspectFrenzied", "frenziedMonsterAtkSpdMult", 1.75f,
				"Monster attack speed gain multiplier."
			);
			AspectFrenziedBaseCooldownGain = Config.Bind(
				"2lb-AspectFrenzied", "frenziedBaseCooldown", 0.20f,
				"Cooldown reduction gained. Set to 0 to disable."
			);
			AspectFrenziedStackCooldownGain = Config.Bind(
				"2lb-AspectFrenzied", "frenziedAddedCooldown", 0.10f,
				"Cooldown reduction gained per stack."
			);
			AspectFrenziedMonsterCooldownMult = Config.Bind(
				"2lb-AspectFrenzied", "frenziedMonsterCooldownMult", 1.25f,
				"Monster cooldown reduction gain multiplier."
			);

			AspectVolatileBaseDamage = Config.Bind(
				"2lc-AspectVolatile", "volatileBaseTotalDamage", 0.20f,
				"Base total damage of explosion. Set to 0 to disable."
			);
			AspectVolatileStackDamage = Config.Bind(
				"2lc-AspectVolatile", "volatileAddedTotalDamage", 0.10f,
				"Added total damage of explosion per stack."
			);
			AspectVolatileMonsterDamageMult = Config.Bind(
				"2lc-AspectVolatile", "volatileMonsterDamageMult", 1f,
				"Monster explosion damage multiplier."
			);

			AspectEchoBaseMinionDamageResistGain = Config.Bind(
				"2ld-AspectEcho", "echoBaseMinionDamageResistGain", 0.20f,
				"Minion damage taken reduction. Set to 0 to disable."
			);
			AspectEchoStackMinionDamageResistGain = Config.Bind(
				"2ld-AspectEcho", "echoAddedMinionDamageResistGain", 0.10f,
				"Minion damage taken reduction per stack."
			);
			AspectEchoBaseCooldownGain = Config.Bind(
				"2ld-AspectEcho", "echoBaseCooldown", 0.20f,
				"Cooldown reduction gained. Set to 0 to disable."
			);
			AspectEchoStackCooldownGain = Config.Bind(
				"2ld-AspectEcho", "echoAddedCooldown", 0.10f,
				"Cooldown reduction gained per stack."
			);
			AspectEchoMonsterCooldownMult = Config.Bind(
				"2ld-AspectEcho", "echoMonsterCooldownMult", 1f,
				"Monster cooldown reduction gain multiplier."
			);
		}

		private static void EliteVarietyConfigs(ConfigFile Config)
		{
			AspectArmorBaseArmorGain = Config.Bind(
				"2ma-Armor Aspect", "armorBaseArmor", 30f,
				"Armor gained. Set to 0 to disable."
			);
			AspectArmorStackArmorGain = Config.Bind(
				"2ma-Armor Aspect", "armorAddedArmor", 15f,
				"Armor gained per stack."
			);



			AspectBannerBaseDamageGain = Config.Bind(
				"2mb-Banner Aspect", "bannerBaseDamageGained", 0.20f,
				"Damage gained. Set to 0 to disable."
			);
			AspectBannerStackDamageGain = Config.Bind(
				"2mb-Banner Aspect", "bannerAddedDamageGained", 0.10f,
				"Damage gained per stack."
			);
			AspectBannerExtraJump = Config.Bind(
				"2mb-Banner Aspect", "bannerExtraJump", false,
				"Extra jump. Player Only"
			);
			AspectBannerTweaks = Config.Bind(
				"2mb-Banner Aspect", "bannerTweaks", false,
				"Prevents buffward type from changing. Will only provide warbanner buff."
			);
			AspectBannerBoth = Config.Bind(
				"2mb-Banner Aspect", "bannerBoth", false,
				"Allows both buffwards to be active simultaneously if buff changing not disabled."
			);



			AspectImpaleDamageMult = Config.Bind(
				"2mc-Impale Aspect", "impaleDotDamageMult", 0.75f,
				"Multiply impale DOT damage. This setting reduces impale damage to counteract DotAmp."
			);
			AspectImpaleBaseDotAmp = Config.Bind(
				"2mc-Impale Aspect", "impaleBaseDotAmp", 0.20f,
				"DOT damage multiplier gained. Set to 0 to disable."
			);
			AspectImpaleStackDotAmp = Config.Bind(
				"2mc-Impale Aspect", "impaleAddedDotAmp", 0.10f,
				"DOT damage multiplier gained per stack."
			);
			AspectImpaleTweaks = Config.Bind(
				"2mc-Impale Aspect", "impaleTweaks", false,
				"Scale impale damage and duration based on ambient level. No effect at lvl 90."
			);



			AspectGoldenBaseRegenGain = Config.Bind(
				"2md-Golden Aspect", "goldenBaseRegen", 12f,
				"Health regen gained. Set to 0 to disable."
			);
			AspectGoldenStackRegenGain = Config.Bind(
				"2md-Golden Aspect", "goldenAddedRegen", 6f,
				"Health regen gained per stack."
			);
			AspectGoldenItemScoreFactor = Config.Bind(
				"2md-Golden Aspect", "goldenItemScoreFactor", 4.0f,
				"Multiply itemscore by value. Applies before itemscore exponent. white = 1x, green = 3x, other = 9x."
			);
			AspectGoldenItemScoreExponent = Config.Bind(
				"2md-Golden Aspect", "goldenItemScoreExponent", 0.65f,
				"Itemscore exponent. Raise itemscore to the power of value."
			);
			AspectGoldenItemScoreLevelScaling = Config.Bind(
				"2md-Golden Aspect", "goldenItemScoreLevelScaling", 0.1f,
				"Itemscore regeneration level scaling. Vanilla regeneration scaling is 0.2 = +100% every 5 levels."
			);
			AspectGoldenBaseScoredRegenGain = Config.Bind(
				"2md-Golden Aspect", "goldenBaseScoredRegen", 1.0f,
				"ScoredRegen multiplier gained. Applies after itemscore exponent. Set to 0 to disable."
			);
			AspectGoldenStackScoredRegenGain = Config.Bind(
				"2md-Golden Aspect", "goldenStackScoredRegen", 0.5f,
				"ScoredRegen multiplier gained per stack."
			);



			AspectCycloneBaseMovementGain = Config.Bind(
				"2me-Cyclone Aspect", "cycloneBaseMovementGained", 0.20f,
				"Movement speed gained. Set to 0 to disable."
			);
			AspectCycloneStackMovementGain = Config.Bind(
				"2me-Cyclone Aspect", "cycloneAddedMovementGained", 0.10f,
				"Movement speed gained per stack."
			);
			AspectCycloneBaseAtkSpdGain = Config.Bind(
				"2me-Cyclone Aspect", "cycloneBaseAtkSpdGained", 0.20f,
				"Attack speed gained. Set to 0 to disable."
			);
			AspectCycloneStackAtkSpdGain = Config.Bind(
				"2me-Cyclone Aspect", "cycloneAddedAtkSpdGained", 0.10f,
				"Attack speed gained per stack."
			);
			AspectCycloneBaseDodgeGain = Config.Bind(
				"2me-Cyclone Aspect", "cycloneBaseDodgeGained", 10f,
				"Dodge chance gained. Set to 0 to disable."
			);
			AspectCycloneStackDodgeGain = Config.Bind(
				"2me-Cyclone Aspect", "cycloneAddedDodgeGained", 10f,
				"Dodge chance gained per stack."
			);
			AspectCycloneBlindDodgeEffect = Config.Bind(
				"2me-Cyclone Aspect", "cycloneBlindDodgeEffect", 25f,
				"Dodge chance effect from blind. Set to 0 to disable."
			);
			AspectCycloneTweaks = Config.Bind(
				"2me-Cyclone Aspect", "cycloneTweaks", false,
				"Visibility: 15m -> 240m, ProcRate: 0.1s -> 0.5s, Prevent Crit(remove constant luck sound effect). Allows changing ProcCoeff."
			);
			AspectCycloneProc = Config.Bind(
				"2me-Cyclone Aspect", "cycloneSandstormProc", 0f,
				"Sandstorm ProcCoefficient. Original value is 0.25"
			);



			AspectTinkerBaseMinionDamageResistGain = Config.Bind(
				"2mf-Tinker Aspect", "tinkerBaseDroneDamageResist", 0.20f,
				"Drone damage taken reduction. Hyperbolic. Set to 0 to disable."
			);
			AspectTinkerStackMinionDamageResistGain = Config.Bind(
				"2mf-Tinker Aspect", "tinkerAddedDroneDamageResist", 0.10f,
				"Drone damage taken reduction per stack. Hyperbolic."
			);
			AspectTinkerBaseMinionDamageGain = Config.Bind(
				"2mf-Tinker Aspect", "tinkerBaseDroneDamageGain", 0.20f,
				"Drone damage gained. Set to 0 to disable."
			);
			AspectTinkerStackMinionDamageGain = Config.Bind(
				"2mf-Tinker Aspect", "tinkerAddedDroneDamageGain", 0.10f,
				"Drone damage gained per stack."
			);
			AspectTinkerTweaks = Config.Bind(
				"2mf-Tinker Aspect", "tinkerTweaks", false,
				"Disable scrap stealing. Allow changing limits and stats."
			);
			AspectTinkerMonsterLimit = Config.Bind(
				"2mf-Tinker Aspect", "tinkerMonsterLimit", 1,
				"Maximum tinkerer drones for monsters."
			);
			AspectTinkerMonsterDamageMult = Config.Bind(
				"2mf-Tinker Aspect", "tinkerMonsterDamage", 1.5f,
				"Damage multiplier for monster tinkerer drones."
			);
			AspectTinkerMonsterHealthMult = Config.Bind(
				"2mf-Tinker Aspect", "tinkerMonsterHealth", 2f,
				"Health multiplier for monster tinkerer drones."
			);
			AspectTinkerPlayerLimit = Config.Bind(
				"2mf-Tinker Aspect", "tinkerPlayerLimit", 2,
				"Maximum tinkerer drones for players."
			);
			AspectTinkerPlayerDamageMult = Config.Bind(
				"2mf-Tinker Aspect", "tinkerPlayerDamage", 1.5f,
				"Damage multiplier for player tinkerer drones."
			);
			AspectTinkerPlayerHealthMult = Config.Bind(
				"2mf-Tinker Aspect", "tinkerPlayerHealth", 2f,
				"Health multiplier for player tinkerer drones."
			);
		}

		private static void SandsweptConfigs(ConfigFile Config)
		{
			AspectMotivatorBaseDamageGain = Config.Bind(
				"2oa-Motivator Aspect", "motivatorBaseDamageGained", 0.20f,
				"Damage gained. Set to 0 to disable."
			);
			AspectMotivatorStackDamageGain = Config.Bind(
				"2oa-Motivator Aspect", "motivatorAddedDamageGained", 0.10f,
				"Damage gained per stack."
			);
			AspectMotivatorExtraJump = Config.Bind(
				"2oa-Motivator Aspect", "motivatorExtraJump", false,
				"Extra jump. Player Only"
			);
			AspectMotivatorTweaks = Config.Bind(
				"2oa-Motivator Aspect", "motivatorTweaks", false,
				"Prevents extra empower on-hit."
			);
			AspectMotivatorEmpowerSelf = Config.Bind(
				"2oa-Motivator Aspect", "motivatorEmpowerSelf", false,
				"Apply empower to self if on-hit is enabled."
			);



			AspectOsmiumBaseCooldownGain = Config.Bind(
				"2ob-Osmium Aspect", "osmiumBaseCooldown", 0.2f,
				"Cooldown reduction gained. Set to 0 to disable."
			);
			AspectOsmiumStackCooldownGain = Config.Bind(
				"2ob-Osmium Aspect", "osmiumAddedCooldown", 0.1f,
				"Cooldown reduction gained per stack."
			);
			AspectOsmiumPlayerNearbyNormal = Config.Bind(
				"2ob-Osmium Aspect", "osmiumPlayerNearbyNormal", true,
				"Players don't take increased damage from enemies inside an aura."
			);
		}



		private static void StarstormConfigs(ConfigFile Config)
		{
			AspectEmpyreanBaseHealthGain = Config.Bind(
				"2pa-AspectEmpyrean", "empyreanBaseHealthGained", 0.20f,
				"Health gained. Set to 0 to disable."
			);
			AspectEmpyreanStackHealthGain = Config.Bind(
				"2pa-AspectEmpyrean", "empyreanAddedHealthGained", 0.10f,
				"Health gained per stack."
			);
			AspectEmpyreanBaseDamageGain = Config.Bind(
				"2pa-AspectEmpyrean", "empyreanBaseDamageGained", 0.20f,
				"Damage gained. Set to 0 to disable."
			);
			AspectEmpyreanStackDamageGain = Config.Bind(
				"2pa-AspectEmpyrean", "empyreanAddedDamageGained", 0.10f,
				"Damage gained per stack."
			);
			AspectEmpyreanBaseCooldownGain = Config.Bind(
				"2pa-AspectEmpyrean", "empyreanBaseCooldown", 0f,
				"Cooldown reduction gained. Set to 0 to disable."
			);
			AspectEmpyreanStackCooldownGain = Config.Bind(
				"2pa-AspectEmpyrean", "empyreanAddedCooldown", 0f,
				"Cooldown reduction gained per stack."
			);
		}



		private static void ValidateElusiveModifier(object sender, System.EventArgs e)
		{
			ValidateElusiveModifier();
		}

		private static void ValidateElusiveModifier()
		{
			ValidElusiveModifier = AspectVeiledElusiveMovementGain.Value > 0f || AspectVeiledElusiveDodgeGain.Value > 0f;
		}
	}
}
