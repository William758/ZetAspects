using RoR2;
using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.Networking;
using System;
using System.Reflection;

namespace TPDespair.ZetAspects
{
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.TeamMoonstorm.Starstorm2", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [R2APISubmoduleDependency(nameof(BuffAPI), nameof(ItemAPI), nameof(ItemDropAPI), nameof(LanguageAPI), nameof(ResourcesAPI))]

    public class ZetAspectsPlugin : BaseUnityPlugin
    {
        public const string ModVer = "1.4.1";
        public const string ModName = "ZetAspects";
        public const string ModGuid = "com.TPDespair.ZetAspects";

        public static AssetBundle MainAssets;

        public static BuffIndex ZetHeadHunterBuff { get; private set; }
        public static BuffIndex ZetSappedDebuff { get; private set; }
        public static BuffIndex ZetShreddedDebuff { get; private set; }

        public static EquipmentIndex StarVoidEquipIndex = EquipmentIndex.None;
        public static EliteIndex StarVoidEliteIndex = EliteIndex.None;
        public static BuffIndex StarVoidAffixBuffIndex = BuffIndex.None;
        public static BuffIndex StarVoidSlowBuffIndex = BuffIndex.None;

        public static ItemIndex StarCritMultiItemIndex = ItemIndex.None;

        public static ConfigEntry<bool> ZetAspectEliteEquipmentCfg { get; set; }
        public static ConfigEntry<bool> ZetAspectEquipmentConversionCfg { get; set; }
        public static ConfigEntry<float> ZetAspectEquipmentEffectCfg { get; set; }
        public static ConfigEntry<bool> ZetAspectRedTierCfg { get; set; }
        public static ConfigEntry<float> ZetAspectDropChanceCfg { get; set; }
        public static ConfigEntry<bool> ZetAspectShowDropTextCfg { get; set; }
        public static ConfigEntry<string> ZetAspectDropTextCfg { get; set; }

        public static ConfigEntry<int> ZetAspectLeechSeedLgohCfg { get; set; }

        public static ConfigEntry<bool> ZetEnableSizeControllerCfg { get; set; }
        public static ConfigEntry<bool> ZetEnableCameraModifyCfg { get; set; }
        public static ConfigEntry<float> ZetSizeEffectBaseCfg { get; set; }
        public static ConfigEntry<float> ZetSizeEffectAspectCfg { get; set; }
        public static ConfigEntry<float> ZetSizeEffectAspectEquipmentCfg { get; set; }
        public static ConfigEntry<float> ZetSizeEffectHunterCfg { get; set; }
        public static ConfigEntry<float> ZetSizeEffectTonicCfg { get; set; }
        public static ConfigEntry<float> ZetSizeChangeRateCfg { get; set; }
        public static ConfigEntry<float> ZetSizeLimitCfg { get; set; }

        public static ConfigEntry<int> ZetHeadHunterCountExtraCfg { get; set; }
        public static ConfigEntry<float> ZetHeadHunterExtraEffectCfg { get; set; }
        public static ConfigEntry<float> ZetHeadHunterBaseDurationCfg { get; set; }
        public static ConfigEntry<float> ZetHeadHunterStackDurationCfg { get; set; }

        public static ConfigEntry<bool> ZetHeadHunterBuffEnableCfg { get; set; }
        public static ConfigEntry<float> ZetHeadHunterBuffHealthCfg { get; set; }
        public static ConfigEntry<float> ZetHeadHunterBuffMovementSpeedCfg { get; set; }
        public static ConfigEntry<float> ZetHeadHunterBuffDamageCfg { get; set; }
        public static ConfigEntry<float> ZetHeadHunterBuffAttackSpeedCfg { get; set; }
        public static ConfigEntry<float> ZetHeadHunterBuffCritChanceCfg { get; set; }

        public static ConfigEntry<float> ZetAspectEffectMonsterDamageMultCfg { get; set; }
        public static ConfigEntry<float> ZetAspectEffectPlayerDebuffMultCfg { get; set; }

        public static ConfigEntry<float> ZetAspectWhiteFreezeChanceCfg { get; set; }
        public static ConfigEntry<float> ZetAspectWhiteFreezeDurationCfg { get; set; }
        public static ConfigEntry<float> ZetAspectWhiteSlowDurationCfg { get; set; }
        public static ConfigEntry<float> ZetAspectWhiteBaseDamageCfg { get; set; }
        public static ConfigEntry<float> ZetAspectWhiteStackDamageCfg { get; set; }

        public static ConfigEntry<float> ZetAspectBlueSappedDurationCfg { get; set; }
        public static ConfigEntry<float> ZetAspectBlueSappedDamageCfg { get; set; }
        public static ConfigEntry<float> ZetAspectBlueHealthConvertedCfg { get; set; }
        public static ConfigEntry<float> ZetAspectBlueBaseShieldGainCfg { get; set; }
        public static ConfigEntry<float> ZetAspectBlueStackShieldGainCfg { get; set; }
        public static ConfigEntry<float> ZetAspectBlueBombDurationCfg { get; set; }
        public static ConfigEntry<float> ZetAspectBlueBaseDamageCfg { get; set; }
        public static ConfigEntry<float> ZetAspectBlueStackDamageCfg { get; set; }

        public static ConfigEntry<bool> ZetAspectRedExtraJumpCfg { get; set; }
        public static ConfigEntry<float> ZetAspectRedBaseMovementGainCfg { get; set; }
        public static ConfigEntry<float> ZetAspectRedStackMovementGainCfg { get; set; }
        public static ConfigEntry<float> ZetAspectRedMovementGainCapCfg { get; set; }
        public static ConfigEntry<float> ZetAspectRedBurnDurationCfg { get; set; }
        public static ConfigEntry<bool> ZetAspectRedTrailCfg { get; set; }
        public static ConfigEntry<bool> ZetAspectRedUseBaseCfg { get; set; }
        public static ConfigEntry<float> ZetAspectRedBaseDamageCfg { get; set; }
        public static ConfigEntry<float> ZetAspectRedStackDamageCfg { get; set; }

        public static ConfigEntry<bool> ZetAspectGhostSlowEffectCfg { get; set; }
        public static ConfigEntry<float> ZetAspectGhostShredDurationCfg { get; set; }
        public static ConfigEntry<float> ZetAspectGhostShredArmorCfg { get; set; }
        public static ConfigEntry<float> ZetAspectGhostAllyArmorGainCfg { get; set; }
        public static ConfigEntry<float> ZetAspectGhostBaseArmorGainCfg { get; set; }
        public static ConfigEntry<float> ZetAspectGhostStackArmorGainCfg { get; set; }

        public static ConfigEntry<float> ZetAspectPoisonNullDurationCfg { get; set; }
        public static ConfigEntry<float> ZetAspectPoisonNullDamageTakenCfg { get; set; }
        public static ConfigEntry<bool> ZetAspectPoisonFireSpikesCfg { get; set; }
        public static ConfigEntry<float> ZetAspectPoisonBaseHealthGainCfg { get; set; }
        public static ConfigEntry<float> ZetAspectPoisonStackHealthGainCfg { get; set; }
        public static ConfigEntry<int> ZetAspectPoisonBaseLgohCfg { get; set; }
        public static ConfigEntry<int> ZetAspectPoisonStackLgohCfg { get; set; }

        public static ConfigEntry<float> ZetAspectVoidBaseDamageGainCfg { get; set; }
        public static ConfigEntry<float> ZetAspectVoidStackDamageGainCfg { get; set; }
        public static ConfigEntry<float> ZetAspectVoidBaseDamageTakenCfg { get; set; }
        public static ConfigEntry<float> ZetAspectVoidStackDamageTakenCfg { get; set; }

        public static ConfigEntry<bool> ZetHypercritEnabledCfg { get; set; }
        public static ConfigEntry<int> ZetHypercritModeCfg { get; set; }
        public static ConfigEntry<float> ZetHypercritBaseCfg { get; set; }
        public static ConfigEntry<float> ZetHypercritMultCfg { get; set; }
        public static ConfigEntry<float> ZetHypercritDecayCfg { get; set; }



        private static void LoadAssets()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ZetAspects.zetaspectbundle"))
            {
                MainAssets = AssetBundle.LoadFromStream(stream);
                var provider = new AssetBundleResourcesProvider("@ZetAspects", MainAssets);
                ResourcesAPI.AddProvider(provider);
            }
        }



        private void ConfigSetup()
        {
            ZetAspectEliteEquipmentCfg = Config.Bind<bool>(
                "0a-General",
                "eliteEquipment", false,
                "Elites drop equipment instead of new items."
            );
            ZetAspectEquipmentConversionCfg = Config.Bind<bool>(
                "0a-General",
                "convertEquipment", true,
                "Picking up the same elite equipment will convert into non-equipment item."
            );
            ZetAspectEquipmentEffectCfg = Config.Bind<float>(
                "0a-General",
                "equipmentEffect", 3f,
                "Count equipment as this much stacks of aspects for players."
            );
            ZetAspectRedTierCfg = Config.Bind<bool>(
                "0a-General",
                "dropAsRed", true,
                "Make aspects red items, otherwise drop-only yellow."
            );
            ZetAspectDropChanceCfg = Config.Bind<float>(
                "0a-General",
                "dropChance", 0.1f,
                "Percent chance that an elite drops its aspect."
            );
            ZetAspectShowDropTextCfg = Config.Bind<bool>(
                "0a-General",
                "showDropText", true,
                "Display a chat message when an elite drops an aspect."
            );
            ZetAspectDropTextCfg = Config.Bind<string>(
                "0a-General",
                "dropText", "A slain enemy leaves some of its power behind...",
                "Chat message for when an elite drops an aspect."
            );



            ZetAspectLeechSeedLgohCfg = Config.Bind<int>(
                "0b-Tweaks",
                "seedLifeGainOnHit", 2,
                "Health gained on hit from Leech Seed."
            );



            ZetEnableSizeControllerCfg = Config.Bind<bool>(
                "0c-SizeController",
                "enable", false,
                "Allow various items and effects to change character size."
            );
            ZetEnableCameraModifyCfg = Config.Bind<bool>(
                "0c-SizeController",
                "modifyCamera", true,
                "Modify camera to scale with size."
            );
            ZetSizeEffectBaseCfg = Config.Bind<float>(
                "0c-SizeController",
                "sizeEffectBase", 0.1f,
                "Increase size from other items."
            );
            ZetSizeEffectAspectCfg = Config.Bind<float>(
                "0c-SizeController",
                "sizeEffectAspect", 0.1f,
                "Increase size per aspect buff."
            );
            ZetSizeEffectAspectEquipmentCfg = Config.Bind<float>(
                "0c-SizeController",
                "sizeEffectAspectEquipment", 0.4f,
                "Increase size from aspect equipment. Player Only."
            );
            ZetSizeEffectHunterCfg = Config.Bind<float>(
                "0c-SizeController",
                "sizeEffectHunter", 0.05f,
                "Increase size per headhunter buff stack."
            );
            ZetSizeEffectTonicCfg = Config.Bind<float>(
                "0c-SizeController",
                "sizeEffectTonic", 1.5f,
                "Tonic multiplies final size value."
            );
            ZetSizeChangeRateCfg = Config.Bind<float>(
                "0c-SizeController",
                "sizeChangeRate", 0.2f,
                "Rate of size change per second."
            );
            ZetSizeLimitCfg = Config.Bind<float>(
                "0c-SizeController",
                "sizeLimit", 4f,
                "Maxiumum size."
            );



            ZetHeadHunterCountExtraCfg = Config.Bind<int>(
                "1a-Headhunter",
                "extraCount", -1,
                "Count extra headhunters up to amount. -1 for no limit."
            );
            ZetHeadHunterExtraEffectCfg = Config.Bind<float>(
                "1a-Headhunter",
                "extraEffect", 0.5f,
                "Count extra headhunters as this much extra stacks of aspects."
            );
            ZetHeadHunterBaseDurationCfg = Config.Bind<float>(
                "1a-Headhunter",
                "baseBuffDuration", 20f,
                "Base duration of buff effect."
            );
            ZetHeadHunterStackDurationCfg = Config.Bind<float>(
                "1a-Headhunter",
                "addedBuffDuration", 10f,
                "Added duration of buff effect per stack."
            );



            ZetHeadHunterBuffEnableCfg = Config.Bind<bool>(
                "1b-Headhunter Buff",
                "buffEnable", true,
                "Killing elites also grants stat buffs."
            );
            ZetHeadHunterBuffHealthCfg = Config.Bind<float>(
                "1b-Headhunter Buff",
                "increasedHealth", 0.05f,
                "Increased health per headhunter buff."
            );
            ZetHeadHunterBuffMovementSpeedCfg = Config.Bind<float>(
                "1b-Headhunter Buff",
                "IncreasedMovementSpeed", 0.05f,
                "Increased movement speed per headhunter buff."
            );
            ZetHeadHunterBuffDamageCfg = Config.Bind<float>(
                "1b-Headhunter Buff",
                "increasedDamage", 0.02f,
                "Increased damage per headhunter buff."
            );
            ZetHeadHunterBuffAttackSpeedCfg = Config.Bind<float>(
                "1b-Headhunter Buff",
                "increasedAttackSpeed", 0.02f,
                "Increased attack speed per headhunter buff."
            );
            ZetHeadHunterBuffCritChanceCfg = Config.Bind<float>(
                "1b-Headhunter Buff",
                "increasedCritChance", 2f,
                "Increased crit chance per headhunter buff."
            );



            ZetAspectEffectMonsterDamageMultCfg = Config.Bind<float>(
                "20-Aspect Effects",
                "effectMonsterDamageMult", 0.5f,
                "Multiply damage value of aspect effects from monsters."
            );
            ZetAspectEffectPlayerDebuffMultCfg = Config.Bind<float>(
                "20-Aspect Effects",
                "effectPlayerDebuffMult", 0.5f,
                "Multiply debuff effect of shredded and nullified applied to players."
            );



            ZetAspectWhiteFreezeChanceCfg = Config.Bind<float>(
                "2a-Ice Aspect",
                "freezeChance", 6.0f,
                "Set freeze chance. Hyperbolic. Player Only. Set to 0 to disable."
            );
            ZetAspectWhiteFreezeDurationCfg = Config.Bind<float>(
                "2a-Ice Aspect",
                "freezeDuration", 2.0f,
                "Set freeze duration in seconds."
            );
            ZetAspectWhiteSlowDurationCfg = Config.Bind<float>(
                "2a-Ice Aspect",
                "slowDuration", 4.0f,
                "Set slow duration in seconds."
            );
            ZetAspectWhiteBaseDamageCfg = Config.Bind<float>(
                "2a-Ice Aspect",
                "baseTotalDamage", 0.50f,
                "Base total damage of blades. Set to 0 to disable."
            );
            ZetAspectWhiteStackDamageCfg = Config.Bind<float>(
                "2a-Ice Aspect",
                "addedTotalDamage", 0.25f,
                "Added total damage of blades per stack."
            );



            ZetAspectBlueSappedDurationCfg = Config.Bind<float>(
                "2b-Lightning Aspect",
                "sappedDuration", 4.0f,
                "Set sapped duration in seconds. Set to 0 to disable."
            );
            ZetAspectBlueSappedDamageCfg = Config.Bind<float>(
                "2b-Lightning Aspect",
                "sappedDamage", 0.10f,
                "Base Damage reduction of sapped."
            );
            ZetAspectBlueHealthConvertedCfg = Config.Bind<float>(
                "2b-Lightning Aspect",
                "healthConverted", 0.20f,
                "Set health converted into shield."
            );
            ZetAspectBlueBaseShieldGainCfg = Config.Bind<float>(
                "2b-Lightning Aspect",
                "baseShieldGained", 0.20f,
                "Set shield gained from health. Set to 0 to disable."
            );
            ZetAspectBlueStackShieldGainCfg = Config.Bind<float>(
                "2b-Lightning Aspect",
                "addedShieldGained", 0.10f,
                "Set shield gained from health per stack."
            );
            ZetAspectBlueBombDurationCfg = Config.Bind<float>(
                "2b-Lightning Aspect",
                "bombDuration", 1.0f,
                "Set detonation timer for bomb."
            );
            ZetAspectBlueBaseDamageCfg = Config.Bind<float>(
                "2b-Lightning Aspect",
                "baseTotalDamage", 0.50f,
                "Base total damage of bombs. Set to 0 to disable."
            );
            ZetAspectBlueStackDamageCfg = Config.Bind<float>(
                "2b-Lightning Aspect",
                "addedTotalDamage", 0.25f,
                "Added total damage of bombs per stack."
            );



            ZetAspectRedExtraJumpCfg = Config.Bind<bool>(
                "2c-Fire Aspect",
                "extraJump", true,
                "Extra jump. Player Only"
            );
            ZetAspectRedBaseMovementGainCfg = Config.Bind<float>(
                "2c-Fire Aspect",
                "baseMovementGained", 0.20f,
                "Movement speed gained. Set to 0 to disable."
            );
            ZetAspectRedStackMovementGainCfg = Config.Bind<float>(
                "2c-Fire Aspect",
                "addedMovementGained", 0.10f,
                "Movement speed gained per stack."
            );
            ZetAspectRedMovementGainCapCfg = Config.Bind<float>(
                "2c-Fire Aspect",
                "movementGainCap", -1f,
                "Movement speed gained limit. -1 for no limit."
            );
            ZetAspectRedBurnDurationCfg = Config.Bind<float>(
                "2c-Fire Aspect",
                "burnDuration", 4.0f,
                "Set burn duration in seconds."
            );
            ZetAspectRedTrailCfg = Config.Bind<bool>(
                "2c-Fire Aspect",
                "playerTrail", false,
                "Set whether players leave fire trail."
            );
            ZetAspectRedUseBaseCfg = Config.Bind<bool>(
                "2c-Fire Aspect",
                "useBaseDamage", false,
                "Set whether burn damage is based on BASE or TOTAL damage. Vanilla behavior is 200% BASE damage over 4 seconds."
            );
            ZetAspectRedBaseDamageCfg = Config.Bind<float>(
                "2c-Fire Aspect",
                "baseTotalDamage", 0.50f,
                "Base total damage of burn over duration. Set to 0 to disable."
            );
            ZetAspectRedStackDamageCfg = Config.Bind<float>(
                "2c-Fire Aspect",
                "addedTotalDamage", 0.25f,
                "Added total damage of burn per stack."
            );



            ZetAspectGhostSlowEffectCfg = Config.Bind<bool>(
                "2d-Celestial Aspect",
                "slowEffect", false,
                "Set whether applies slow. Shares duration with ice aspect."
            );
            ZetAspectGhostShredDurationCfg = Config.Bind<float>(
                "2d-Celestial Aspect",
                "shredDuration", 4.0f,
                "Set shred duration in seconds. Set to 0 to disable."
            );
            ZetAspectGhostShredArmorCfg = Config.Bind<float>(
                "2d-Celestial Aspect",
                "shredArmor", 30f,
                "Armor reduction of shred."
            );
            ZetAspectGhostAllyArmorGainCfg = Config.Bind<float>(
                "2d-Celestial Aspect",
                "allyArmor", 30f,
                "Armor granted to nearby allies. Effect only applies once. Set to 0 to disable."
            );
            ZetAspectGhostBaseArmorGainCfg = Config.Bind<float>(
                "2d-Celestial Aspect",
                "baseArmor", 60f,
                "Armor gained. Set to 0 to disable."
            );
            ZetAspectGhostStackArmorGainCfg = Config.Bind<float>(
                "2d-Celestial Aspect",
                "addedArmor", 30f,
                "Armor gained per stack."
            );



            ZetAspectPoisonNullDurationCfg = Config.Bind<float>(
                "2e-Malachite Aspect",
                "nullDuration", 4.0f,
                "Set nullification duration for players in seconds. Monsters is 8 seconds."
            );
            ZetAspectPoisonNullDamageTakenCfg = Config.Bind<float>(
                "2e-Malachite Aspect",
                "nullDamageTaken", 0.20f,
                "Damage taken increase from nullification. Set to 0 to disable."
            );
            ZetAspectPoisonFireSpikesCfg = Config.Bind<bool>(
                "2e-Malachite Aspect",
                "playerSpikes", false,
                "Set whether players throw spike balls."
            );
            ZetAspectPoisonBaseHealthGainCfg = Config.Bind<float>(
                "2e-Malachite Aspect",
                "baseHealth", 400f,
                "Health gained. Set to 0 to disable."
            );
            ZetAspectPoisonStackHealthGainCfg = Config.Bind<float>(
                "2e-Malachite Aspect",
                "addedHealth", 200f,
                "Health gained per stack."
            );
            ZetAspectPoisonBaseLgohCfg = Config.Bind<int>(
                "2e-Malachite Aspect",
                "baseLifeGainOnHit", 8,
                "Health gained on hit. Set to 0 to disable."
            );
            ZetAspectPoisonStackLgohCfg = Config.Bind<int>(
                "2e-Malachite Aspect",
                "stackLifeGainOnHit", 4,
                "Health gained on hit per stack."
            );



            ZetAspectVoidBaseDamageGainCfg = Config.Bind<float>(
                "2f-Void Aspect",
                "baseDamageGain", 0.2f,
                "Base damage multiplier gained. Set to 0 to disable."
            );
            ZetAspectVoidStackDamageGainCfg = Config.Bind<float>(
                "2f-Void Aspect",
                "addedDamageGain", 0.1f,
                "Base damage multiplier gained per stack."
            );
            ZetAspectVoidBaseDamageTakenCfg = Config.Bind<float>(
                "2f-Void Aspect",
                "baseDamageTakenReduction", 0.2f,
                "Damage taken reduction. Hyperbolic. Set to 0 to disable."
            );
            ZetAspectVoidStackDamageTakenCfg = Config.Bind<float>(
                "2f-Void Aspect",
                "addedDamageTakenReduction", 0.1f,
                "Damage taken reduction per stack. Hyperbolic."
            );



            ZetHypercritEnabledCfg = Config.Bind<bool>(
                "3a-Hypercrit",
                "enable", false,
                "Hypercrit. Used to calculate TOTAL burn damage past 100% crit."
            );
            ZetHypercritModeCfg = Config.Bind<int>(
                "3a-Hypercrit",
                "critMode", 0,
                "0 = Linear, 1 = Exponential, 2 = Asymptotic."
            );
            ZetHypercritBaseCfg = Config.Bind<float>(
                "3a-Hypercrit",
                "critBase", 2.0f,
                "Base critical strike multiplier."
            );
            ZetHypercritMultCfg = Config.Bind<float>(
                "3a-Hypercrit",
                "critMult", 1.0f,
                "Extra critical strike multiplier factor."
            );
            ZetHypercritDecayCfg = Config.Bind<float>(
                "3a-Hypercrit",
                "critDecay", 1.0f,
                "Asymptotic crit mult decay factor."
            );
        }



        private static void HHBuffHealthHook()
        {
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);

                // Health increase
                bool found = c.TryGotoNext(
                    x => x.MatchLdloc(41),
                    x => x.MatchLdloc(42),
                    x => x.MatchMul(),
                    x => x.MatchStloc(41)
                );

                if (found)
                {
                    c.Emit(OpCodes.Ldarg, 0);
                    c.Emit(OpCodes.Ldloc, 42);
                    c.EmitDelegate<Func<CharacterBody, float, float>>((self, mult) =>
                    {
                        return mult + ZetHeadHunterBuffHealthCfg.Value * self.GetBuffCount(ZetHeadHunterBuff);
                    });
                    c.Emit(OpCodes.Stloc, 42);
                }
                else
                {
                    Debug.LogWarning("ZetAspect - HHBuff Health Hook Failed");
                }
            };
        }

        private static void HHBuffMovementSpeedHook()
        {
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);

                // Movement speed increase
                bool found = c.TryGotoNext(
                    x => x.MatchLdloc(53),
                    x => x.MatchLdloc(54),
                    x => x.MatchLdloc(55),
                    x => x.MatchDiv(),
                    x => x.MatchMul(),
                    x => x.MatchStloc(53)
                );

                if (found)
                {
                    c.Index += 1;

                    c.Emit(OpCodes.Pop);

                    c.Emit(OpCodes.Ldarg, 0);
                    c.Emit(OpCodes.Ldloc, 54);
                    c.EmitDelegate<Func<CharacterBody, float, float>>((self, mult) =>
                    {
                        return mult + ZetHeadHunterBuffMovementSpeedCfg.Value * self.GetBuffCount(ZetHeadHunterBuff);
                    });
                    c.Emit(OpCodes.Stloc, 54);

                    c.Emit(OpCodes.Ldloc, 53);
                }
                else
                {
                    Debug.LogWarning("ZetAspect - HHBuff Movespeed Hook Failed");
                }
            };
        }

        private static void HHBuffDamageHook()
        {
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);

                // Damage increase
                bool found = c.TryGotoNext(
                    x => x.MatchLdloc(57),
                    x => x.MatchLdloc(58),
                    x => x.MatchMul(),
                    x => x.MatchStloc(57)
                );

                if (found)
                {
                    c.Emit(OpCodes.Ldarg, 0);
                    c.Emit(OpCodes.Ldloc, 58);
                    c.EmitDelegate<Func<CharacterBody, float, float>>((self, mult) =>
                    {
                        return mult + ZetHeadHunterBuffDamageCfg.Value * self.GetBuffCount(ZetHeadHunterBuff);
                    });
                    c.Emit(OpCodes.Stloc, 58);
                }
                else
                {
                    Debug.LogWarning("ZetAspect - HHBuff Damage Hook Failed");
                }
            };
        }

        private static void HHBuffAttackSpeedHook()
        {
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);

                // Attack speed increase
                bool found = c.TryGotoNext(
                    x => x.MatchLdloc(60),
                    x => x.MatchLdloc(61),
                    x => x.MatchMul(),
                    x => x.MatchStloc(60)
                );

                if (found)
                {
                    c.Emit(OpCodes.Ldarg, 0);
                    c.Emit(OpCodes.Ldloc, 61);
                    c.EmitDelegate<Func<CharacterBody, float, float>>((self, mult) =>
                    {
                        return mult + ZetHeadHunterBuffAttackSpeedCfg.Value * self.GetBuffCount(ZetHeadHunterBuff);
                    });
                    c.Emit(OpCodes.Stloc, 61);
                }
                else
                {
                    Debug.LogWarning("ZetAspect - HHBuff AtkSpd Hook Failed");
                }
            };
        }

        private static void HHBuffCriticalHook()
        {
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);

                // Critical strike chance
                bool found = c.TryGotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchLdloc(62),
                    x => x.MatchCallvirt<CharacterBody>("set_crit")
                );

                if (found)
                {
                    c.Emit(OpCodes.Ldarg, 0);
                    c.Emit(OpCodes.Ldloc, 62);
                    c.EmitDelegate<Func<CharacterBody, float, float>>((self, mult) =>
                    {
                        return mult + ZetHeadHunterBuffCritChanceCfg.Value * self.GetBuffCount(ZetHeadHunterBuff);
                    });
                    c.Emit(OpCodes.Stloc, 62);
                }
                else
                {
                    Debug.LogWarning("ZetAspect - HHBuff Crit Hook Failed");
                }
            };
        }



        private static void DefineHeadHunterBuff()
        {
            var buff = new CustomBuff(new BuffDef
            {
                buffColor = Color.grey,
                canStack = true,
                isDebuff = false,
                name = "ZetHeadHunter",
                iconPath = "textures/bufficons/texBuffAttackSpeedOnCritIcon"
            });
            ZetHeadHunterBuff = BuffAPI.Add(buff);
        }

        private static void DefineSappedDebuff()
        {
            var buff = new CustomBuff(new BuffDef
            {
                buffColor = new Color(0.185f, 0.465f, 0.75f),
                canStack = false,
                isDebuff = true,
                name = "ZetSapped",
                iconPath = "textures/bufficons/texBuffNullifiedIcon"
            });
            ZetSappedDebuff = BuffAPI.Add(buff);
        }

        private static void DefineShreddedDebuff()
        {
            var buff = new CustomBuff(new BuffDef
            {
                buffColor = new Color(0.185f, 0.75f, 0.465f),
                canStack = false,
                isDebuff = true,
                name = "ZetShredded",
                iconPath = "textures/bufficons/texBuffPulverizeIcon"
            });
            ZetShreddedDebuff = BuffAPI.Add(buff);
        }



        private static void HeadHunterOnCharacterDeathHook()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);

                bool found = c.TryGotoNext(
                    x => x.MatchLdcR4(3f),
                    x => x.MatchLdcR4(5f),
                    x => x.MatchLdloc(51),
                    x => x.MatchConvR4(),
                    x => x.MatchMul(),
                    x => x.MatchAdd(),
                    x => x.MatchStloc(53)
                );

                if (found)
                {
                    c.Index += 6;

                    c.Emit(OpCodes.Pop);
                    c.Emit(OpCodes.Ldloc, 51);
                    c.EmitDelegate<Func<int, float>>((count) =>
                    {
                        return ZetHeadHunterBaseDurationCfg.Value + ZetHeadHunterStackDurationCfg.Value * (count - 1);
                    });
                    c.Emit(OpCodes.Dup);
                    c.Emit(OpCodes.Ldloc, 13);
                    c.EmitDelegate<Action<float, CharacterBody>>((duration, attacker) =>
                    {
                        if (ZetHeadHunterBuffEnableCfg.Value) attacker.AddTimedBuff(ZetHeadHunterBuff, duration);
                    });
                }
                else
                {
                    Debug.LogWarning("ZetAspect - HH Duration And Buff Hook Failed");
                }
            };
        }

        private static void RefreshAspectBuffsHook()
        {
            On.RoR2.CharacterBody.UpdateBuffs += (orig, self, deltaTime) =>
            {
                orig(self, deltaTime);

                if (self.inventory)
                {
                    if (StarVoidAffixBuffIndex != BuffIndex.None && !self.HasBuff(StarVoidAffixBuffIndex))
                    {
                        if (self.inventory.GetItemCount(ZetAspectVoid.itemIndex) > 0)
                        {
                            self.AddTimedBuff(StarVoidAffixBuffIndex, 5f);
                        }
                    }

                    if (!self.HasBuff(BuffIndex.AffixWhite))
                    {
                        if (self.inventory.GetItemCount(ZetAspectIce.itemIndex) > 0)
                        {
                            self.AddTimedBuff(BuffIndex.AffixWhite, 5f);
                        }
                    }
                    if (!self.HasBuff(BuffIndex.AffixBlue))
                    {
                        if (self.inventory.GetItemCount(ZetAspectLightning.itemIndex) > 0)
                        {
                            self.AddTimedBuff(BuffIndex.AffixBlue, 5f);
                        }
                    }
                    if (!self.HasBuff(BuffIndex.AffixRed))
                    {
                        if (self.inventory.GetItemCount(ZetAspectFire.itemIndex) > 0)
                        {
                            self.AddTimedBuff(BuffIndex.AffixRed, 5f);
                        }
                    }
                    if (!self.HasBuff(BuffIndex.AffixHaunted))
                    {
                        if (self.inventory.GetItemCount(ZetAspectCelestial.itemIndex) > 0)
                        {
                            self.AddTimedBuff(BuffIndex.AffixHaunted, 5f);
                        }
                    }
                    if (!self.HasBuff(BuffIndex.AffixPoison))
                    {
                        if (self.inventory.GetItemCount(ZetAspectMalachite.itemIndex) > 0)
                        {
                            self.AddTimedBuff(BuffIndex.AffixPoison, 5f);
                        }
                    }
                }
            };
        }

        private static void SetDropChanceHook()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);

                bool found = c.TryGotoNext(
                    x => x.MatchPop(),
                    x => x.MatchLdcR4(0.025f),
                    x => x.MatchLdloc(14)
                );

                if (found)
                {
                    c.Index += 2;

                    c.Emit(OpCodes.Pop);
                    c.Emit(OpCodes.Ldc_R4, ZetAspectDropChanceCfg.Value);

                    found = c.TryGotoNext(
                        x => x.MatchLdloc(2),
                        x => x.MatchCallOrCallvirt<CharacterBody>("get_isElite")
                    );

                    if (found)
                    {
                        c.Index += 2;

                        c.Emit(OpCodes.Ldloc, 10);
                        c.EmitDelegate<Func<bool, EquipmentIndex, bool>>((elite, index) =>
                        {
                            if (elite && index != EquipmentIndex.None)
                            {
                                if (ZetAspectShowDropTextCfg.Value && NetworkServer.active)
                                {
                                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                                    {
                                        baseToken = "<color=#DDDDA0>" + ZetAspectDropTextCfg.Value + "</color>"
                                    });
                                }
                            }
                            return elite;
                        });
                    }
                    else
                    {
                        Debug.LogWarning("ZetAspect - Elite Aspect Drop Message Hook Failed");
                    }
                }
                else
                {
                    Debug.LogWarning("ZetAspect - Elite Aspect Drop Chance And Message Hook Failed");
                }
            };
        }

        private static void InterceptAspectDropHook()
        {
            On.RoR2.PickupDropletController.CreatePickupDroplet += (orig, pickupIndex, position, velocity) =>
            {
                if (!ZetAspectEliteEquipmentCfg.Value)
                {
                    if (StarVoidEquipIndex != EquipmentIndex.None && pickupIndex == PickupCatalog.FindPickupIndex(StarVoidEquipIndex))
                    {
                        pickupIndex = PickupCatalog.FindPickupIndex(ZetAspectVoid.itemIndex);
                    }

                    if (pickupIndex == PickupCatalog.FindPickupIndex(EquipmentIndex.AffixWhite))
                    {
                        pickupIndex = PickupCatalog.FindPickupIndex(ZetAspectIce.itemIndex);
                    }
                    if (pickupIndex == PickupCatalog.FindPickupIndex(EquipmentIndex.AffixBlue))
                    {
                        pickupIndex = PickupCatalog.FindPickupIndex(ZetAspectLightning.itemIndex);
                    }
                    if (pickupIndex == PickupCatalog.FindPickupIndex(EquipmentIndex.AffixRed))
                    {
                        pickupIndex = PickupCatalog.FindPickupIndex(ZetAspectFire.itemIndex);
                    }
                    if (pickupIndex == PickupCatalog.FindPickupIndex(EquipmentIndex.AffixHaunted))
                    {
                        pickupIndex = PickupCatalog.FindPickupIndex(ZetAspectCelestial.itemIndex);
                    }
                    if (pickupIndex == PickupCatalog.FindPickupIndex(EquipmentIndex.AffixPoison))
                    {
                        pickupIndex = PickupCatalog.FindPickupIndex(ZetAspectMalachite.itemIndex);
                    }
                }

                orig(pickupIndex, position, velocity);
            };
        }

        private static void OnHitEnemyDebuffHook()
        {
            // Apply sapped and shreded debuffs
            On.RoR2.GlobalEventManager.OnHitEnemy += (orig, self, damageInfo, victimObject) =>
            {
                ApplyNewAspectDebuffs(damageInfo, victimObject);

                orig(self, damageInfo, victimObject);
            };
        }

        private static void ApplyNewAspectDebuffs(DamageInfo damageInfo, GameObject victimObject)
        {
            if (!NetworkServer.active) return;
            if (damageInfo.procCoefficient == 0f || damageInfo.rejected) return;

            if (damageInfo.attacker)
            {
                CharacterBody attacker = damageInfo.attacker.GetComponent<CharacterBody>();
                CharacterBody victim = victimObject ? victimObject.GetComponent<CharacterBody>() : null;

                if (attacker && victim)
                {
                    if (attacker.HasBuff(BuffIndex.AffixBlue))
                    {
                        float durationBlue = ZetAspectBlueSappedDurationCfg.Value;
                        if (attacker.teamComponent.teamIndex != TeamIndex.Player) durationBlue *= damageInfo.procCoefficient;
                        if (durationBlue > 0.1f) victim.AddTimedBuff(ZetSappedDebuff, durationBlue);
                    }

                    if (attacker.HasBuff(BuffIndex.AffixHaunted))
                    {
                        float durationGhost = ZetAspectGhostShredDurationCfg.Value;
                        if (attacker.teamComponent.teamIndex != TeamIndex.Player) durationGhost *= damageInfo.procCoefficient;
                        if (durationGhost > 0.1f) victim.AddTimedBuff(ZetShreddedDebuff, durationGhost);
                    }
                }
            }
        }

        private static void EquipmentConversionHook()
        {
            IL.RoR2.GenericPickupController.AttemptGrant += (il) =>
            {
                ILCursor c = new ILCursor(il);

                bool found = c.TryGotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchLdcI4(1),
                    x => x.MatchStfld<GenericPickupController>("consumed"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<GenericPickupController>("pickupIndex")
                );

                if (found)
                {
                    c.Index += 7;

                    c.Emit(OpCodes.Ldarg, 0);
                    c.Emit(OpCodes.Ldarg, 1);
                    c.Emit(OpCodes.Ldloc, 2);
                    c.EmitDelegate<Func<GenericPickupController, CharacterBody, PickupDef, PickupDef>>((gpc, body, pickupDef) =>
                    {
                        if (!ZetAspectEquipmentConversionCfg.Value) return pickupDef;
                        
                        if (pickupDef.equipmentIndex != EquipmentIndex.None)
                        {
                            EquipmentIndex equip = body.inventory.currentEquipmentIndex;

                            if (StarVoidEquipIndex != EquipmentIndex.None && pickupDef.equipmentIndex == StarVoidEquipIndex)
                            {
                                if (equip == StarVoidEquipIndex)
                                {
                                    gpc.pickupIndex = PickupCatalog.FindPickupIndex(ZetAspectVoid.itemIndex);
                                    return PickupCatalog.GetPickupDef(gpc.pickupIndex);
                                }
                            }

                            switch (pickupDef.equipmentIndex)
                            {
                                case EquipmentIndex.AffixWhite:
                                    if(equip == EquipmentIndex.AffixWhite)
                                    {
                                        gpc.pickupIndex = PickupCatalog.FindPickupIndex(ZetAspectIce.itemIndex);
                                        return PickupCatalog.GetPickupDef(gpc.pickupIndex);
                                    }
                                    break;
                                case EquipmentIndex.AffixBlue:
                                    if (equip == EquipmentIndex.AffixBlue)
                                    {
                                        gpc.pickupIndex = PickupCatalog.FindPickupIndex(ZetAspectLightning.itemIndex);
                                        return PickupCatalog.GetPickupDef(gpc.pickupIndex);
                                    }
                                    break;
                                case EquipmentIndex.AffixRed:
                                    if (equip == EquipmentIndex.AffixRed)
                                    {
                                        gpc.pickupIndex = PickupCatalog.FindPickupIndex(ZetAspectFire.itemIndex);
                                        return PickupCatalog.GetPickupDef(gpc.pickupIndex);
                                    }
                                    break;
                                case EquipmentIndex.AffixHaunted:
                                    if (equip == EquipmentIndex.AffixHaunted)
                                    {
                                        gpc.pickupIndex = PickupCatalog.FindPickupIndex(ZetAspectCelestial.itemIndex);
                                        return PickupCatalog.GetPickupDef(gpc.pickupIndex);
                                    }
                                    break;
                                case EquipmentIndex.AffixPoison:
                                    if (equip == EquipmentIndex.AffixPoison)
                                    {
                                        gpc.pickupIndex = PickupCatalog.FindPickupIndex(ZetAspectMalachite.itemIndex);
                                        return PickupCatalog.GetPickupDef(gpc.pickupIndex);
                                    }
                                    break;
                            }
                        }
                        
                        return pickupDef;
                    });
                    c.Emit(OpCodes.Stloc, 2);
                }
                else
                {
                    Debug.LogWarning("ZetAspect - Equipment Conversion Hook Failed");
                }
            };
        }

        private static void OnRunStartHook()
        {
            On.RoR2.Run.Start += (orig, self) =>
            {
                orig(self);

                if (StarCompat.enabled) StarCompat.GetIndexes();
            };
        }

        private static void ChangeText()
        {
            LanguageAPI.Add("ITEM_SEED_DESC", "Dealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>" + ZetAspectLeechSeedLgohCfg.Value + "</style> <style=cStack>(+" + ZetAspectLeechSeedLgohCfg.Value + " per stack)</style> <style=cIsHealing>health</style>.");
            LanguageAPI.Add("ITEM_HEADHUNTER_DESC", "Gain the <style=cIsDamage>power</style> of any killed elite monster for <style=cIsDamage>" + ZetHeadHunterBaseDurationCfg.Value + "s</style> <style=cStack>(+" + ZetHeadHunterStackDurationCfg.Value + "s per stack)</style>.");
        }



        public void Awake()
        {
            LoadAssets();

            ConfigSetup();

            DefineHeadHunterBuff();
            DefineSappedDebuff();
            DefineShreddedDebuff();

            HHBuffHealthHook();
            HHBuffMovementSpeedHook();
            HHBuffDamageHook();
            HHBuffAttackSpeedHook();
            HHBuffCriticalHook();

            ZetAspectIce.Init();
            ZetAspectLightning.Init();
            ZetAspectFire.Init();
            ZetAspectCelestial.Init();
            ZetAspectMalachite.Init();

            ZetAspectVoid.Init();

            if (ZetEnableSizeControllerCfg.Value) ZetSizeController.Init();

            HeadHunterOnCharacterDeathHook();
            RefreshAspectBuffsHook();
            SetDropChanceHook();
            InterceptAspectDropHook();
            OnHitEnemyDebuffHook();
            EquipmentConversionHook();

            OnRunStartHook();

            ChangeText();
        }
        /*
        public void Update()
        {
            DebugDrops();
        }
        
        
        private void DebugDrops()
        {
            bool debugDrops = false;

            if (debugDrops)
            {
                if (Input.GetKeyDown(KeyCode.F2))
                {
                    var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                    if (!ZetAspectEliteEquipmentCfg.Value)
                    {
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ZetAspectIce.itemIndex), transform.position, transform.forward * 30f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ZetAspectLightning.itemIndex), transform.position, transform.forward * 60f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ZetAspectFire.itemIndex), transform.position, transform.right * 30f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ZetAspectCelestial.itemIndex), transform.position, transform.forward * -30f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ZetAspectMalachite.itemIndex), transform.position, transform.forward * -60f);
                        if (StarCompat.enabled && StarCompat.EliteCoreEnabled())
                        {
                            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ZetAspectVoid.itemIndex), transform.position, transform.right * -30f);
                        }
                    }
                    else
                    {
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(EquipmentIndex.AffixWhite), transform.position, transform.forward * 30f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(EquipmentIndex.AffixBlue), transform.position, transform.forward * 60f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(EquipmentIndex.AffixRed), transform.position, transform.right * 30f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(EquipmentIndex.AffixHaunted), transform.position, transform.forward * -30f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(EquipmentIndex.AffixPoison), transform.position, transform.forward * -60f);
                        if (StarCompat.enabled && StarCompat.EliteCoreEnabled())
                        {
                            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(StarVoidEquipIndex), transform.position, transform.right * -30f);
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.F3))
                {
                    var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemIndex.CritGlasses), transform.position, transform.forward * 30f);
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemIndex.CritGlasses), transform.position, transform.forward * 30f);
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemIndex.CritGlasses), transform.position, transform.forward * 30f);
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemIndex.CritGlasses), transform.position, transform.forward * 30f);
                    if (StarCritMultiItemIndex != ItemIndex.None)
                    {
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(StarCritMultiItemIndex), transform.position, transform.forward * -30f);
                    }
                }

                if (Input.GetKeyDown(KeyCode.F4))
                {
                    var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemIndex.ShieldOnly), transform.position, transform.forward * 30f);
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemIndex.RepeatHeal), transform.position, transform.forward * -30f);
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemIndex.HeadHunter), transform.position, transform.right * 30f);
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemIndex.LunarDagger), transform.position, transform.right * -30f);
                }

                if (Input.GetKeyDown(KeyCode.F5))
                {
                    var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemIndex.Knurl), transform.position, transform.forward * 1f);
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ItemIndex.Pearl), transform.position, transform.forward * 30f);
                }
            }
        }
        */


        public static float GetStackMagnitude(CharacterBody self, ItemIndex aspectIndex)
        {
            if (self.inventory == null) return 1f;

            float aspect = self.inventory.GetItemCount(aspectIndex);
            float hunter = self.inventory.GetItemCount(ItemIndex.HeadHunter);

            if (self.teamComponent.teamIndex == TeamIndex.Player && HasAspectEquipment(self, aspectIndex))
            {
                aspect += ZetAspectEquipmentEffectCfg.Value;
            }

            if (aspect == 0f)
            {
                aspect = 1f;
                hunter = Mathf.Max(0f, hunter - 1f);
            }

            if (ZetHeadHunterCountExtraCfg.Value >= 0)
            {
                hunter = Mathf.Min(hunter, ZetHeadHunterCountExtraCfg.Value);
            }

            return aspect + (hunter * ZetHeadHunterExtraEffectCfg.Value);
        }

        public static bool HasAspectEquipment(CharacterBody self, ItemIndex aspectIndex)
        {
            EquipmentIndex equipIndex = self.inventory.currentEquipmentIndex;

            if (equipIndex == EquipmentIndex.None) return false;

            if (equipIndex == StarVoidEquipIndex)
            {
                if (aspectIndex == ZetAspectVoid.itemIndex) return true;
                else return false;
            }

            switch (equipIndex)
            {
                case EquipmentIndex.AffixWhite:
                    if (aspectIndex == ZetAspectIce.itemIndex) return true;
                    break;
                case EquipmentIndex.AffixBlue:
                    if (aspectIndex == ZetAspectLightning.itemIndex) return true;
                    break;
                case EquipmentIndex.AffixRed:
                    if (aspectIndex == ZetAspectFire.itemIndex) return true;
                    break;
                case EquipmentIndex.AffixHaunted:
                    if (aspectIndex == ZetAspectCelestial.itemIndex) return true;
                    break;
                case EquipmentIndex.AffixPoison:
                    if (aspectIndex == ZetAspectMalachite.itemIndex) return true;
                    break;
            }

            return false;
        }

        public static string FormatSeconds(float seconds)
        {
            string s = seconds == 1.0f ? "" : "s";
            return seconds + " second" + s;
        }

        public static bool HasValidBuff(CharacterBody self, BuffIndex buff)
        {
            return buff != BuffIndex.None && self.HasBuff(buff);
        }
    }
}
