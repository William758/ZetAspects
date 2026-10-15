# ZetAspects

Drop Rate from Killed Elites changed to 1 in 500.

Configurable drop chance multiplier for each elite type.

Drop chance is halved every time one drops in a run (0.2% -> 0.1%).

Changes the Elite Aspects into scalable Drop Only Boss Tier Items.

Aspects can be configured to be Red Tier or kept as Equipment.

Clicking the Equipment icon at the bottom-right of the screen will Convert it into an Item.

## Item Changes:

Wake of Vultures lasts for 20 (+10 per stack) seconds and also gives a buff to stats on killing elites.

Transcendence grants 50% of life regeneration as shield regeneration.

Fixed VoidBearCooldown buff and made it stack.

## Aspect Effects:

![effects](https://i.imgur.com/szoURGs.png)

## Survivors of the Void:

![effects](https://i.imgur.com/TEmrOkh.png)

Monster leech multiplier : x5

## Seekers of the Storm:

![effects](https://i.imgur.com/0iYHPqp.png)

Monster Bead critical damage multiplier : x1.35 - Only applies if critical chance below 40%

## Alloyed Collective:

![effects](https://i.imgur.com/3xu3ekO.png)

## SpikeStrip:

![effects](https://i.imgur.com/c35yqIh.png)

Monster fury movement speed multiplier : x2

Monster endurance damage reduction multiplier : x0.5

Monster endurance plate layer count multiplier : x0.275

Monster endurance damage reduction and armor disabled with NemSpikeStrip

Stifle debuff changed from (-100% * buffCount) base damage to (-50% * SQRT(buffCount)) base damage

Elusive effect magnitude multiplier with NemSpikeStrip : x0.5

## GoldenCoastPlus:

![effects](https://i.imgur.com/HFBYaVV.png)

## Aetherium:

![effects](https://i.imgur.com/fOvxEoc.png)

## BubbetsItems:

![effects](https://i.imgur.com/k0z5dLx.png)

## WarframeWisp:

![effects](https://i.imgur.com/uao6nvi.png)

## BlightedElites:

![effects](https://i.imgur.com/kFos0O8.png)

## GOTCE:

![effects](https://i.imgur.com/7FxKgsu.png)

## Thalassophobia:

![effects](https://i.imgur.com/9ok4kSK.png)

## RisingTides:

![effects](https://i.imgur.com/qu6dbBd.png)

Monster Bismuth health multiplier : x0.5

Monster Nocturnal OOD movement speed multiplier : x2

Monster Nocturnal OOD attack speed multiplier : x1.5

## NemRisingTides:

![effects](https://i.imgur.com/g5uLb9X.png)

## MoreElites:

![effects](https://i.imgur.com/ySgSzGV.png)

Monster Frenzied movement speed multiplier : x2.5

Monster Frenzied attack speed multiplier : x1.75

Monster Frenzied cooldown reduction multiplier : x1.25

## Elite Variety Aspect Effects:

![effects](https://i.imgur.com/z5rHysk.png)

Impale Dot Damage : Damage multiplied by x0.75 to counteract effect of aspect DotAmp.

bannerTweaks (disabled by default) : Aura will only provide warbanner buff and not warcry buff.

impaleTweaks (disabled by default) : Scale impale damage and duration based on ambient level. Default effect at lvl 90.

cycloneTweaks (disabled by default) : Visibility: 15m -> 240m, ProcRate: 0.1s -> 0.5s, Prevent Crit(remove constant luck sound effect).

tinkerTweaks (disabled by default) : Disable scrap stealing. Reduce drone counts and increase health and damage(configurable).

## Augmentum:

![effects](https://i.imgur.com/88Z6eIM.png)

## Sandswept:

![effects](https://i.imgur.com/GVK7yBS.png)

## Starstorm:

![effects](https://i.imgur.com/1D5WESl.png)

## Installation:

Requires Bepinex and HookGenPatcher.

Use r2modman or place inside of Risk of Rain 2/Bepinex/Plugins/

## Credits:

Brazilian Portuguese translation by SpookyGabe#9476.

Korean translation by 깽비#8221 and Dice-001.

Chinese translation by BTP-G and MushroomEl.

Japanese translation by WakefulSpect.

## Changelog:

v2.10.9 - Fixed Item Models, Fixed bug related to DeathRewards.

v2.10.8 - Updated for latest game version. Aspect items dont have models...

v2.10.7 - Updated Korean translation.

v2.10.6 - Updated Japanese translation.

v2.10.5 - Alloyed Collective support.

v2.10.4 - Limit rate of GoldChunks produced by AffixAurelionite.

v2.10.3 - Fixed SpikeStrip AffixVeiled buff handling.

v2.10.2 - Fixed for latest patch.

v2.10.1 - Seekers of the Storm support.

v2.10.0 - Updated for latest game version. Added valdation check to Aspect Command Menu.

v2.9.10 - Fix Cyclic Dependency.

v2.9.9 - Starstorm2 Support. Rewrote buff management. Switch to language files.

v2.9.8 - Updated Japanese translation. Fixed Augmentum compatibility.

v2.9.7 - Fixed Fading Reflection from BubbetsItems not loading properly.

v2.9.6 - Added Japanese translation by WakefulSpect.

v2.9.5 - Fixed for latest patch.

v2.9.4 - Fix Headhunter count check.

v2.9.3 - Fix Transcendence shield gain. Rewrote Headhunter buff hook.

v2.9.2 - Fixed for latest patch.

v2.9.1 - Fixed content loader breaking whenever an elite is disabled.

v2.9.0 - Rewrote content loader. Updated Veiled compatibility. Fixed EquipmentConversion while using SS2.

v2.8.6 - Fixed lag related to Wayfarer from SS2.

v2.8.5 - Support for Augmentum and Sandswept.

v2.8.4 - EliteVariety Support.

v2.8.3 - Fixed for latest patch.

v2.8.2 - Support for GoldenCoastPlusRevived.

v2.8.1 - Made T2 Elites less spooky (Fixed audio issue).

v2.8.0 - Fixed for SotS.
