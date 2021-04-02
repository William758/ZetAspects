using BepInEx.Configuration;

namespace TPDespair.ZetAspects
{
    public static class Configuration
    {
        public static ConfigEntry<bool> AspectRedTier { get; set; }
        public static ConfigEntry<float> AspectDropChance { get; set; }
        public static ConfigEntry<bool> AspectShowDropText { get; set; }
        public static ConfigEntry<string> AspectDropText { get; set; }
        public static ConfigEntry<bool> AspectSkinApply { get; set; }


        public static ConfigEntry<bool> AspectEliteEquipment { get; set; }
        public static ConfigEntry<float> AspectEquipmentEffect { get; set; }
        public static ConfigEntry<bool> AspectEquipmentConversion { get; set; }
        public static ConfigEntry<bool> AspectSkinEquipmentPriority { get; set; }


        public static ConfigEntry<int> LeechSeedHeal { get; set; }
        //public static ConfigEntry<float> TranscendenceRegen { get; set; }


        public static ConfigEntry<int> HeadHunterCountExtra { get; set; }
        public static ConfigEntry<float> HeadHunterExtraEffect { get; set; }
        public static ConfigEntry<float> HeadHunterBaseDuration { get; set; }
        public static ConfigEntry<float> HeadHunterStackDuration { get; set; }


        public static ConfigEntry<bool> HeadHunterBuffEnable { get; set; }
        public static ConfigEntry<float> HeadHunterBuffHealth { get; set; }
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


        internal static void Init(ConfigFile Config)
        {
            AspectRedTier = Config.Bind(
                "0a-General", "aspectRedTier", true,
                "Make aspects red items, otherwise drop-only yellow."
            );
            AspectDropChance = Config.Bind(
                "0a-General", "aspectDropChance", 0.1f,
                "Percent chance that an elite drops its aspect."
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
            AspectEquipmentEffect = Config.Bind(
                "0b-Equipment", "eliteEquipmentEffect", 3f,
                "Count equipment as this much stacks of aspects for players."
            );
            AspectEquipmentConversion = Config.Bind(
                "0b-Equipment", "eliteEquipmentConversion", true,
                "Picking up the same elite equipment will convert into non-equipment item."
            );
            AspectSkinEquipmentPriority = Config.Bind(
                "0b-Equipment", "eliteEquipmentSkinPriority", true,
                "Whether elite equipment skin takes priority."
            );


            LeechSeedHeal = Config.Bind(
                "0c-Tweaks", "seedLifeGainOnHit", 2,
                "Health gained on hit from Leech Seed."
            );
            //TranscendenceRegen = Config.Bind(
            //    "0c-Tweaks", "TranscendanceShieldRegen", 0.25f,
            //    "Health regen converted into shield regen. Set to 0 to disable."
            //);


            HeadHunterCountExtra = Config.Bind(
                "1a-Headhunter", "hunterExtraCount", -1,
                "Count extra headhunters up to amount. -1 for no limit."
            );
            HeadHunterExtraEffect = Config.Bind(
                "1a-Headhunter", "hunterExtraEffect", 0.5f,
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
                "1b-Headhunter Buff", "buffEnable", true,
                "Killing elites also grants stat buffs."
            );
            HeadHunterBuffHealth = Config.Bind(
                "1b-Headhunter Buff", "increasedHealth", 0.05f,
                "Increased health per headhunter buff."
            );
            HeadHunterBuffMovementSpeed = Config.Bind(
                "1b-Headhunter Buff", "IncreasedMovementSpeed", 0.05f,
                "Increased movement speed per headhunter buff."
            );
            HeadHunterBuffDamage = Config.Bind(
                "1b-Headhunter Buff", "increasedDamage", 0.02f,
                "Increased damage per headhunter buff."
            );
            HeadHunterBuffAttackSpeed = Config.Bind(
                "1b-Headhunter Buff", "increasedAttackSpeed", 0.02f,
                "Increased attack speed per headhunter buff."
            );
            HeadHunterBuffCritChance = Config.Bind(
                "1b-Headhunter Buff", "increasedCritChance", 2f,
                "Increased crit chance per headhunter buff."
            );


            AspectEffectMonsterDamageMult = Config.Bind(
                "20-Aspect Effects", "effectMonsterDamageMult", 1f,
                "Multiply damage value of aspect effects from monsters."
            );
            AspectEffectPlayerDebuffMult = Config.Bind(
                "20-Aspect Effects", "effectPlayerDebuffMult", 1f,
                "Multiply debuff effect of shredded and nullified applied to players."
            );


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
                "Set health converted into shield."
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
                "2ac-Fire Aspect", "firePlayerTrail", false,
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
                "Armor granted to nearby allies. Effect only applies once to allies without aspect. Set to 0 to disable."
            );
            AspectGhostBaseArmorGain = Config.Bind(
                "2ad-Celestial Aspect", "celestialBaseArmor", 40f,
                "Armor gained. Set to 0 to disable."
            );
            AspectGhostStackArmorGain = Config.Bind(
                "2ad-Celestial Aspect", "celestialAddedArmor", 20f,
                "Armor gained per stack."
            );


            AspectPoisonFireSpikes = Config.Bind(
                "2ae-Malachite Aspect", "playerSpikes", false,
                "Set whether players throw spike balls."
            );
            AspectPoisonNullDuration = Config.Bind(
                "2ae-Malachite Aspect", "nullDuration", 4.0f,
                "Set nullification duration for players in seconds. Monsters is 8 seconds."
            );
            AspectPoisonNullDamageTaken = Config.Bind(
                "2ae-Malachite Aspect", "nullDamageTaken", 0.10f,
                "Damage taken increase from nullification. Set to 0 to disable."
            );
            AspectPoisonBaseHealthGain = Config.Bind(
                "2ae-Malachite Aspect", "baseHealth", 300f,
                "Health gained. Set to 0 to disable."
            );
            AspectPoisonStackHealthGain = Config.Bind(
                "2ae-Malachite Aspect", "addedHealth", 150f,
                "Health gained per stack."
            );
            AspectPoisonBaseHeal = Config.Bind(
                "2ae-Malachite Aspect", "baseLifeGainOnHit", 8,
                "Health gained on hit. Set to 0 to disable."
            );
            AspectPoisonStackHeal = Config.Bind(
                "2ae-Malachite Aspect", "stackLifeGainOnHit", 4,
                "Health gained on hit per stack."
            );
        }
    }
}