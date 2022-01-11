# ZetAspects

Drop Rate from Killed Elites changed to 1 in 500.

Drop chance is halved every time one drops in a run (0.2% -> 0.1%).

Changes the Elite Aspects into scalable Drop Only Boss Tier Items.

Aspects can be changed to Red Tier or kept as Equipment.

Having Aspect Abilities installed will make Aspects drop as Equipment by default.

Clicking the Equipment icon at the bottom-right of the screen will Convert it into an Item.

## Wake of Vultures:

Now lasts for 20 (+10 per stack) seconds and also gives a buff to stats on killing elites.

## Aspect Effects:

![effects](https://i.imgur.com/UJejZyv.png)

## Elite Variety Aspect Effects:

Impale Dot Damage : Damage multiplied by x0.75 to counteract effect of aspect DotAmp.

bannerTweaks (disabled by default) : Aura will only provide warbanner buff and not warcry buff.

impaleTweaks (disabled by default) : Scale impale damage and duration based on ambient level. No effect at lvl 90.

cycloneTweaks (disabled by default) : Visibility: 15m -> 240m, ProcRate: 0.1s -> 0.5s, Prevent Crit(remove constant luck sound effect).

tinkerTweaks (disabled by default) : Disable scrap stealing. Reduce drone counts and increase health and damage(configurable).

![effects](https://i.imgur.com/g82PmCo.png)

## Lost In Transit Effects:

Effect of Guttural Whimpers and Primordial Rage for monsters is multiplied by x5.

Effect of Betrayal of the Bulwark for monsters is multiplied by x5 for damage and cooldowns. Health multiplier is x10.

![effects](https://i.imgur.com/7EHZtrm.png)

## Aetherium Aspect Effects:

![effects](https://i.imgur.com/RifeVeV.png)

## Installation:

Requires Bepinex and HookGenPatcher.

Use r2modman or place inside of Risk of Rain 2/Bepinex/Plugins/

## Changelog:

v2.6.4 - Added blind dodge effect value to Sandstorm Elite description. Configs to change when dodge effect is applicable.

v2.6.3 - Fix disabled red items breaking ItemDropList menu.

v2.6.2 - Adjust some EliteVariety effects. Added bannerTweaks. Added dodge effect to Sandstorm Elite. Change some aspect descriptions.

v2.6.1 - TinkerTweaks allows configuring drone count and stats. Change color of elite equipment droplets. Added server checks to blight manager. Change some health bar colors.

v2.6.0 - Added support for Blighted Elites from LostInTransit. Tweak how mod updates inventory behaviors.

v2.5.7 - Automatically display or hide item/equipment entries in LogBook, RuleBook, and PickupPicker menus. If configured to make item aspects unobtainable, equipment aspect descriptions show total effects only. Reduce Impale damage to negate DotAmp effect.

v2.5.6 - Fix NullRef preventing aspect drops and interfering with OnCharacterDeath. Only show LogBook entries if item/equipment is obtainable.

v2.5.5 - Added Volatile Elite support.

v2.5.4 - Added aspectDropChanceMultiplayerFactor config and allow negative aspectDropChanceMultiplayerExponent.

v2.5.3 - Added impaleTweaks. Config for freeze chance per stack. Added aspectDropChanceMultiplayerExponent config.

v2.5.2 - Fix RichTextChat preventing equipment conversion.

v2.5.1 - Configs to tweak some EliteVariety behavior.

v2.5.0 - Added Aetherium support.

v2.4.2 - Fix aspect icons being affected by the texture resolution setting. Added CDR config for Primordial Rage.

v2.4.1 - LostInTransit aspect stacking. Fix BORBO frost blades.

v2.4.0 - Added basic LostInTransit support. Added support for translations.

v2.3.6 - Prevent applying 0 duration chill.

v2.3.5 - Ice Aspect refreshes chill duration instead if chill can stack. Prevent chill from stacking past 10 with Artificer Extended installed.

v2.3.4 - Tweaked Golden Aspect regen scaling. Simplify drone detection for Tinker Aspect. Aspect drop chance increases over stages cleared vs drops. Remember drop count when loading a save.

v2.3.3 - Maybe fix equipment to item networking.

v2.3.2 - Replace equipment icons. Added Effects to equipment descriptions. Picking up Aspect Equipment discovers item in Logbook. Added Neural Link stacking behavior.

v2.3.1 - Fixed issue preventing Tinkerer item behavior from starting immediately.

v2.3.0 - Elite Variety Support. Changed Aspects to Boss Tier. Drop chance increased to 1 in 500 but halving every time one drops. Re-enabled some effects on the default aspects. Other config tweaks.

v2.2.0 - Prevent Ice Aspect freezing Mithrix. New Buff Icons. Config to reduce drop chance based on drops in run. Config to convert equipment to item by clicking bottom-right equipment icon. Alternate equipment applies elite skin.

v2.1.1 - Prevent other effects besides luck affecting drop chance.

v2.1.0 - Added Perfected Elite support. Tweaked burn calculations. Changed name of HeadHunter Buff configs to be more consistent.

v2.0.2 - Updated for latest patch. Now only requires HookGenPatcher.

v2.0.1 - Switched back to R2API. Changed name of N'kuhana's Retort configs to be more consistent.

v2.0.0 - Updated for Anniversary Update. Now uses EnigmaticThunder.

v1.5.1 - Fix elite skins preventing equipment displays.

v1.5.0 - Non-equipment aspects apply elite skin. Increase interaction range with size.

v1.4.2 - Fix compatibility with latest Starstorm.

v1.4.1 - Fix icons not showing in scrapper.

v1.4.0 - Icon borders reflect tier. Basic Starstorm Void Elite support.

v1.3.0 - Redo SizeController Camera. Use language tokens.

v1.2.1 - Fixed fall damage not making sound.

v1.2.0 - Load BuffAPI. Config allows disabling more effects. Item descriptions hide disabled effects.

v1.1.0 - Fixed stat hooks not working with TILER2 installed.

v1.0.1 - Picking up the same aspect equipment now grants the non-equipment version.

v1.0.0 - Initial Release.
