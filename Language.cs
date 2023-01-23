﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class Language
	{
		public static string targetLanguage = "default";

		public static Dictionary<string, Dictionary<string, string>> tokens = new Dictionary<string, Dictionary<string, string>>();
		public static Dictionary<string, Dictionary<string, string>> fragments = new Dictionary<string, Dictionary<string, string>>();



		public static void RegisterToken(string token, string text, string language = "default")
		{
			if (targetLanguage != "" || targetLanguage != "default") language = targetLanguage;

			if (!tokens.ContainsKey(language)) tokens.Add(language, new Dictionary<string, string>());

			var langDict = tokens[language];

			if (!langDict.ContainsKey(token)) langDict.Add(token, text);
			else langDict[token] = text;
		}
		
		public static void RegisterFragment(string token, string text, string language = "default")
		{
			if (targetLanguage != "" || targetLanguage != "default") language = targetLanguage;

			if (!fragments.ContainsKey(language))
			{
				//Logger.Info("Creating (" + language + ") fragment language.");
				fragments.Add(language, new Dictionary<string, string>());
			}

			var langDict = fragments[language];

			if (!langDict.ContainsKey(token))
			{
				//Logger.Info("Creating fragment (" + token + ") in (" + language + ") fragment language.");
				langDict.Add(token, text);
			}
			else
			{
				Logger.Info("Replacing fragment (" + token + ") in (" + language + ") fragment language.");
				langDict[token] = text;
			}
		}
		

		
		internal static void SetupFragments()
		{
			targetLanguage = "default";

			RegisterFragment("EQUIPMENT_STACK_EFFECT", "\n\nCounts as {0} stacks");
			RegisterFragment("HOW_TO_CONVERT", "\nClick bottom-right equipment icon to convert");

			RegisterFragment("BASE_STACK_FORMAT", "{0} {1}");

			RegisterFragment("FLAT_VALUE", "{0}");
			RegisterFragment("PERCENT_VALUE", "{0}%");
			RegisterFragment("FLATREGEN_VALUE", "{0} hp/s");
			RegisterFragment("PERCENTREGEN_VALUE", "{0}% hp/s");
			RegisterFragment("DURATION_VALUE", "{0}s");
			RegisterFragment("METER_VALUE", "{0}m");

			RegisterFragment("FLAT_STACK_INC", "<style=cStack>(+{0} per stack)</style>");
			RegisterFragment("PERCENT_STACK_INC", "<style=cStack>(+{0}% per stack)</style>");
			RegisterFragment("FLATREGEN_STACK_INC", "<style=cStack>(+{0} hp/s per stack)</style>");
			RegisterFragment("PERCENTREGEN_STACK_INC", "<style=cStack>(+{0}% hp/s per stack)</style>");
			RegisterFragment("DURATION_STACK_INC", "<style=cStack>(+{0}s per stack)</style>");
			RegisterFragment("METER_STACK_INC", "<style=cStack>(+{0}m per stack)</style>");
			RegisterFragment("FLAT_STACK_DEC", "<style=cStack>(-{0} per stack)</style>");
			RegisterFragment("PERCENT_STACK_DEC", "<style=cStack>(-{0}% per stack)</style>");
			RegisterFragment("FLATREGEN_STACK_DEC", "<style=cStack>(-{0} hp/s per stack)</style>");
			RegisterFragment("PERCENTREGEN_STACK_DEC", "<style=cStack>(-{0}% hp/s per stack)</style>");
			RegisterFragment("DURATION_STACK_DEC", "<style=cStack>(-{0}s per stack)</style>");
			RegisterFragment("METER_STACK_DEC", "<style=cStack>(-{0}m per stack)</style>");

			RegisterFragment("BASE_DAMAGE", "base");
			RegisterFragment("TOTAL_DAMAGE", "TOTAL");

			RegisterFragment("FOR_SECOND", "for {0} second");
			RegisterFragment("FOR_SECONDS", "for {0} seconds");
			RegisterFragment("OVER_SECOND", "over {0} second");
			RegisterFragment("OVER_SECONDS", "over {0} seconds");
			RegisterFragment("AFTER_SECOND", "after {0} second");
			RegisterFragment("AFTER_SECONDS", "after {0} seconds");
			RegisterFragment("EVERY_SECOND", "every second");
			RegisterFragment("EVERY_SECONDS", "every {0} seconds");
			RegisterFragment("SECOND", "{0} second");
			RegisterFragment("SECONDS", "{0} seconds");



			RegisterFragment("AFFIX_WHITE_NAME", "Her Biting Embrace");
			RegisterFragment("AFFIX_WHITE_PICKUP", "Become an aspect of ice.");
			RegisterFragment("AFFIX_WHITE_ACTIVE", "Deploy an ice crystal that disables skills.");
			RegisterFragment("AFFIX_WHITE_ACTIVE_ALT", "Deploy an ice crystal that drains health.");
			RegisterFragment("ASPECT_OF_ICE", "<style=cDeath>Aspect of Ice</style> :");
			RegisterFragment("CHILL_ON_HIT", "\nAttacks <style=cIsUtility>chill</style> on hit {0}, reducing <style=cIsUtility>movement speed</style> by <style=cIsUtility>80%</style>.");
			RegisterFragment("CHANCE_TO_FREEZE", "\nAttacks have a {0} chance to <style=cIsUtility>freeze</style> {1}.");
			RegisterFragment("FROST_BLADE", "\nAttacks fire a <style=cIsDamage>blade</style> that deals {0} TOTAL damage.");

			RegisterFragment("AFFIX_BLUE_NAME", "Silence Between Two Strikes");
			RegisterFragment("AFFIX_BLUE_PICKUP", "Become an aspect of lightning.");
			RegisterFragment("AFFIX_BLUE_ACTIVE", "Teleport on use.");
			RegisterFragment("ASPECT_OF_LIGHTNING", "<style=cDeath>Aspect of Lightning</style> :");
			RegisterFragment("PASSIVE_SCATTER_BOMB", "\nOccasionally drop scatter bombs around you.");
			RegisterFragment("SAP_ON_HIT", "\nAttacks <style=cIsUtility>sap</style> on hit {0}, reducing <style=cIsUtility>damage</style> by {1}.");
			RegisterFragment("SCATTER_BOMB", "\nAttacks drop scatter bombs that explodes for {0} TOTAL damage.");
			RegisterFragment("LIGHTNING_BOMB", "\nAttacks attach a <style=cIsDamage>bomb</style> that explodes for {0} TOTAL damage {1}.");

			RegisterFragment("AFFIX_RED_NAME", "Ifrit's Distinction");
			RegisterFragment("AFFIX_RED_PICKUP", "Become an aspect of fire.");
			RegisterFragment("AFFIX_RED_ACTIVE", "Launch a seeking flame missile.");
			RegisterFragment("ASPECT_OF_FIRE", "<style=cDeath>Aspect of Fire</style> :");
			RegisterFragment("PASSIVE_FIRE_TRAIL", "\nLeave behind a fiery trail that damages enemies on contact.");
			RegisterFragment("BURN_DOT", "\nAttacks <style=cIsDamage>burn</style> on hit for {0} {1} damage {2}.");

			RegisterFragment("AFFIX_HAUNTED_NAME", "Spectral Circlet");
			RegisterFragment("AFFIX_HAUNTED_PICKUP", "Become an aspect of incorporeality.");
			RegisterFragment("AFFIX_HAUNTED_ACTIVE", "Temporarily grants nearby allies a chance to dodge hits.");
			RegisterFragment("ASPECT_OF_INCORPOREALITY", "<style=cDeath>Aspect of Incorporeality</style> :");
			RegisterFragment("PASSIVE_GHOST_AURA", "\nEmit an aura that cloaks nearby allies.");
			RegisterFragment("PASSIVE_POSSESS", "\nAttach to some nearby allies, possessing them.");
			RegisterFragment("SHRED_ON_HIT", "\nAttacks <style=cIsDamage>shred</style> on hit {0}, reducing <style=cIsDamage>armor</style> by {1}.");
			RegisterFragment("GHOST_ARMOR", "\nGrants nearby allies {0} additional <style=cIsHealing>armor</style>.");
			RegisterFragment("GHOST_DODGE", "\nGrants nearby allies {0} chance to <style=cIsHealing>dodge</style>.");

			RegisterFragment("AFFIX_POISON_NAME", "N'kuhana's Retort");
			RegisterFragment("AFFIX_POISON_PICKUP", "Become an aspect of corruption.");
			RegisterFragment("AFFIX_POISON_ACTIVE", "Summon an ally Malachite Urchin that inherits your items.");
			RegisterFragment("ASPECT_OF_CORRUPTION", "<style=cDeath>Aspect of Corruption</style> :");
			RegisterFragment("PASSIVE_SPIKEBALL", "\nPeriodically releases spiked balls that sprout spike pits from where they land.");
			RegisterFragment("PASSIVE_RUIN_AURA", "\nEmit an aura that applies <style=cIsDamage>ruin</style> to nearby enemies.");
			RegisterFragment("RUIN_ON_HIT_BASIC", "\nAttacks <style=cIsDamage>ruin</style> on hit {0}, preventing health recovery.");
			RegisterFragment("RUIN_ON_HIT", "\nAttacks <style=cIsDamage>ruin</style> on hit {0}, increasing <style=cIsDamage>damage taken</style> by {1}.");
			RegisterFragment("RUIN_DETAIL", "\n<style=cStack>(Ruin prevents health recovery)</style>");
			RegisterFragment("WEAKEN_ON_HIT", "\nAttacks <style=cIsDamage>weaken</style> on hit {0}, reducing <style=cIsDamage>armor</style> by <style=cIsDamage>30</style>, <style=cIsUtility>movement speed</style> by <style=cIsUtility>40%</style>, and <style=cIsDamage>damage</style> by <style=cIsDamage>40%</style>.");

			RegisterFragment("AFFIX_LUNAR_NAME", "Shared Design");
			RegisterFragment("AFFIX_LUNAR_PICKUP", "Become an aspect of perfection.");
			RegisterFragment("AFFIX_LUNAR_ACTIVE", "Gain temporary protection against powerful hits.");
			RegisterFragment("ASPECT_OF_PERFECTION", "<style=cDeath>Aspect of Perfection</style> :");
			RegisterFragment("PASSIVE_LUNAR_PROJ", "\nPeriodically fire projectiles while in combat.");
			RegisterFragment("CRIPPLE_ON_HIT", "\nAttacks <style=cIsDamage>cripple</style> on hit {0}, reducing <style=cIsDamage>armor</style> by <style=cIsDamage>20</style> and <style=cIsUtility>movement speed</style> by <style=cIsUtility>50%</style>.");

			RegisterFragment("AFFIX_EARTH_NAME", "His Reassurance");
			RegisterFragment("AFFIX_EARTH_PICKUP", "Become an aspect of earth.");
			RegisterFragment("AFFIX_EARTH_ACTIVE", "Grants a short burst of health regeneration.");
			RegisterFragment("ASPECT_OF_EARTH", "<style=cDeath>Aspect of Earth</style> :");
			RegisterFragment("PASSIVE_HEAL_ALLY", "\nHeal nearby allies.");
			RegisterFragment("POACH_ON_HIT_BASIC", "\nAttacks <style=cIsUtility>poach</style> on hit {0}, causing hits against them to <style=cIsHealing>heal</style> for {1} of <style=cIsDamage>damage</style> dealt.");
			RegisterFragment("POACH_ON_HIT", "\nAttacks <style=cIsUtility>poach</style> on hit {0}, reducing <style=cIsUtility>attack speed</style> by {1}.");
			RegisterFragment("POACH_DETAIL", "\n<style=cStack>(Hits against poached enemies heal for {0} of damage dealt)</style>");
			RegisterFragment("HEAL_PERCENT_ON_HIT", "\n<style=cIsHealing>Heal</style> for {0} of the <style=cIsDamage>damage</style> you deal.");
			RegisterFragment("LEECH_MODIFIER_FORMULA", "\n<style=cStack>Leech Modifier =>\n  {0}{1}( [dmg] * [bl]{2} , {3} ){4}</style>");

			RegisterFragment("AFFIX_VOID_NAME", "Entropic Fracture");
			RegisterFragment("AFFIX_VOID_PICKUP", "Become an aspect of void.");
			RegisterFragment("AFFIX_VOID_ACTIVE", "Resets all of your cooldowns.");
			RegisterFragment("ASPECT_OF_VOID", "<style=cDeath>Aspect of Void</style> :");
			RegisterFragment("PASSIVE_BLOCK", "\n<style=cIsHealing>Blocks</style> incoming damage once. Recharges after a delay.");
			RegisterFragment("NULLIFY_ON_HIT", "\nAttacks <style=cIsUtility>nullify</style> on hit {0}");
			RegisterFragment("NULLIFY_DETAIL", "\n<style=cStack>(Enemies with 3 nullify stacks are rooted for 3 seconds)</style>");
			RegisterFragment("COLLAPSE_DOT", "\nAttacks <style=cIsDamage>collapse</style> on hit for {0} {1} damage {2}.");
			RegisterFragment("COLLAPSE_DEFAULT", "\n<style=cIsDamage>100%</style> chance to <style=cIsDamage>collapse</style> an enemy for <style=cIsDamage>400%</style> base damage.");
			RegisterFragment("CORRUPT_ASPECT_ITEM", "\n<style=cIsVoid>Corrupts all Itemized Aspects</style>.");

			RegisterFragment("AFFIX_PLATED_NAME", "Alloy of Subservience");
			RegisterFragment("AFFIX_PLATED_PICKUP", "Become an aspect of endurance.");
			RegisterFragment("AFFIX_PLATED_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_ENDURANCE", "<style=cDeath>Aspect of Endurance</style> :");
			RegisterFragment("PASSIVE_DEFENSE_PLATING", "\nGain defensive plating that mitigates heavy damage.");
			RegisterFragment("DAMAGEREDUCTION_ON_HIT", "\nAttacks <style=cIsUtility>stifle</style> on hit {0}, reducing damage dealt.");

			RegisterFragment("AFFIX_WARPED_NAME", "Misplaced Faith");
			RegisterFragment("AFFIX_WARPED_PICKUP", "Become an aspect of gravity.");
			RegisterFragment("AFFIX_WARPED_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_GRAVITY", "<style=cDeath>Aspect of Gravity</style> :");
			RegisterFragment("PASSIVE_DEFLECT_PROJ", "\nOccasionally deflect nearby projectiles.");
			RegisterFragment("LEVITATE_ON_HIT", "\nAttacks <style=cIsUtility>levitate</style> on hit {0}.");

			RegisterFragment("AFFIX_VEILED_NAME", "Curse of Obscurity");
			RegisterFragment("AFFIX_VEILED_PICKUP", "Become an aspect of obfuscation.");
			RegisterFragment("AFFIX_VEILED_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_OBFUSCATION", "<style=cDeath>Aspect of Obfuscation</style> :");
			RegisterFragment("CLOAK_ON_HIT", "\nAttacks <style=cIsUtility>cloak</style> you on hit.");
			RegisterFragment("CLOAK_ON_HIT_TIMED", "\nAttacks <style=cIsUtility>cloak</style> you on hit {0}.");
			RegisterFragment("ELUSIVE_ON_HIT", "\nAttacks grant <style=cIsUtility>elusive</style> on hit.");
			RegisterFragment("ELUSIVE_EFFECT_MOVE_DETAIL", "\n<style=cStack>(Elusive grants {0} movement speed)</style>");
			RegisterFragment("ELUSIVE_EFFECT_DODGE_DETAIL", "\n<style=cStack>(Elusive grants {0} dodge chance)</style>");
			RegisterFragment("ELUSIVE_EFFECT_BOTH_DETAIL", "\n<style=cStack>(Elusive grants {0} movement speed and {1} dodge chance)</style>");
			RegisterFragment("ELUSIVE_DECAY_DETAIL", "\n<style=cStack>(Elusive effect decays by {0} every second)</style>");
			RegisterFragment("ELUSIVE_EFFECT", "\nElusive effect of {0}.");

			RegisterFragment("AFFIX_ARAGONITE_NAME", "Her Temper");
			RegisterFragment("AFFIX_ARAGONITE_PICKUP", "Become an aspect of fury.");
			RegisterFragment("AFFIX_ARAGONITE_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_FURY", "<style=cDeath>Aspect of Fury</style> :");
			RegisterFragment("PASSIVE_ANGRY_HIT", "\nOccasionally unleash a burst of anger on hit.");
			RegisterFragment("PASSIVE_ANGRY_AURA", "\nEmit an aura that empowers nearby allies.");
			RegisterFragment("ANGRY_ATKSPD", "\nGrants nearby allies {0} increased <style=cIsUtility>movement speed</style>.");
			RegisterFragment("ANGRY_MOVSPD", "\nGrants nearby allies {0} increased <style=cIsDamage>attack speed</style>.");
			RegisterFragment("ANGRY_BOTHSPD", "\nGrants nearby allies {0} increased <style=cIsUtility>movement speed</style> and <style=cIsDamage>attack speed</style>.");
			RegisterFragment("ANGRY_COOLDOWN", "\nGrants nearby allies {0} reduction to <style=cIsUtility>skill cooldowns</style>.");

			RegisterFragment("AFFIX_GOLD_NAME", "Coven of Gold");
			RegisterFragment("AFFIX_GOLD_PICKUP", "Become an aspect of fortune.");
			RegisterFragment("AFFIX_GOLD_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_FORTUNE", "<style=cDeath>Aspect of Fortune</style> :");
			RegisterFragment("GOLD_ON_HIT", "\nAttacks grant gold on hit.");
			RegisterFragment("ITEMSCORE_REGEN", "\nBonus <style=cIsHealing>health regeneration</style> based on quantity and tier of items owned.");
			RegisterFragment("ITEMSCORE_REGEN_MULT", "\nItem score regen multiplier of {0}.");

			RegisterFragment("AFFIX_SEPIA_NAME", "Fading Reflection");
			RegisterFragment("AFFIX_SEPIA_PICKUP", "Become an aspect of illusion.");
			RegisterFragment("AFFIX_SEPIA_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_ILLUSION", "<style=cDeath>Aspect of Illusion</style> :");
			RegisterFragment("SEPIABLIND_ON_HIT", "\nAttacks apply <style=cIsUtility>distorted vision</style> on hit {0}, reducing hit chance by {1}.");

			RegisterFragment("AFFIX_SANGUINE_NAME", "Bloody Fealty");
			RegisterFragment("AFFIX_SANGUINE_PICKUP", "Become an aspect of the red plane.");
			RegisterFragment("AFFIX_SANGUINE_ACTIVE", "Teleport dash and gain brief invulnurability.");
			RegisterFragment("ASPECT_OF_REDPLANE", "<style=cDeath>Aspect of the Red Plane</style> :");
			RegisterFragment("BLEED_DOT", "\nAttacks <style=cIsDamage>bleed</style> on hit for {0} base damage {1}.");
			RegisterFragment("DOT_AMP", "\nIncreases <style=cIsDamage>damage over time multiplier</style> by {0}.");

			RegisterFragment("AFFIX_NULLIFIER_NAME", "Blessing of Parvos");
			RegisterFragment("AFFIX_NULLIFIER_PICKUP", "Become an aspect of null.");
			RegisterFragment("AFFIX_NULLIFIER_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_NULL", "<style=cDeath>Aspect of Null</style> :");
			RegisterFragment("PASSIVE_NULL_AURA", "\nEmit an aura that protects nearby allies and debuffs nearby enemies.");
			RegisterFragment("NULL_ON_HIT", "\nAttacks <style=cIsUtility>null</style> on hit.");
			RegisterFragment("NULL_ON_HIT_SPD", "\nAttacks <style=cIsUtility>null</style> on hit, reducing <style=cIsUtility>movement speed</style> by {0}.");

			RegisterFragment("AFFIX_BLIGHTED_NAME", "Betrayal of the Bulwark");
			RegisterFragment("AFFIX_BLIGHTED_PICKUP", "Become an aspect of decay.");
			RegisterFragment("AFFIX_BLIGHTED_ACTIVE", "Reroll your randomized Elite Affixes.");
			RegisterFragment("ASPECT_OF_DECAY", "<style=cDeath>Aspect of Decay</style> :");
			RegisterFragment("PASSIVE_BLIGHT", "\nGain 2 random Elite Affixes.");



			RegisterFragment("NEARBY_ARMOR", "\nGrants nearby allies {0} additional <style=cIsHealing>armor</style>.");
			RegisterFragment("NEARBY_ARMOR_UNKNOWN", "\nGrants nearby allies additional <style=cIsHealing>armor</style>.");

			RegisterFragment("CONVERT_SHIELD", "\nConvert {0} of health into <style=cIsHealing>regenerating shield</style>.");
			RegisterFragment("EXTRA_SHIELD_CONVERT", "\nGain {0} extra <style=cIsHealing>shield</style> from conversion.");
			RegisterFragment("CONVERT_SHIELD_REGEN", "\nAt least {0} of <style=cIsHealing>health regeneration</style> applies to <style=cIsHealing>shield</style>.");
			RegisterFragment("DISABLE_OOD_SHIELD_RECOVERY", "\n<style=cStack>(Natural shield recovery is disabled)</style>");

			RegisterFragment("STAT_HEALTH_EXTRA_SHIELD", "\nGain {0} of health as extra <style=cIsHealing>shield</style>.");
			RegisterFragment("STAT_EXTRA_JUMP", "\nGain <style=cIsUtility>+1</style> maximum <style=cIsUtility>jump count</style>.");
			RegisterFragment("STAT_MOVESPEED", "\nIncreases <style=cIsUtility>movement speed</style> by {0}.");
			RegisterFragment("STAT_ATTACKSPEED", "\nIncreases <style=cIsDamage>attack speed</style> by {0}.");
			RegisterFragment("STAT_BOTHSPEED", "\nIncreases <style=cIsUtility>movement speed</style> and <style=cIsDamage>attack speed</style> by {0}.");
			RegisterFragment("STAT_ARMOR", "\nIncreases <style=cIsHealing>armor</style> by {0}.");
			RegisterFragment("STAT_HEALTH", "\nIncreases <style=cIsHealing>maximum health</style> by {0}.");
			RegisterFragment("STAT_REGENERATION", "\nIncreases <style=cIsHealing>health regeneration</style> by {0}.");
			RegisterFragment("STAT_DAMAGE", "\nIncreases <style=cIsDamage>damage</style> by {0}.");
			RegisterFragment("STAT_COOLDOWN", "\nReduces <style=cIsUtility>skill cooldowns</style> by {0}.");

			RegisterFragment("LARGE_SHIELD_UNKNOWN", "\nGain a large amount of <style=cIsHealing>regenerating shield</style>.");

			RegisterFragment("BLOCK_CHANCE", "\n{0} chance to <style=cIsHealing>block</style> incoming damage.");
			RegisterFragment("BLOCK_CHANCE_UNKNOWN", "\nChance to <style=cIsHealing>block</style> incoming damage.");
			RegisterFragment("BLOCK_DETAIL", "\n<style=cStack>(Block chance is unaffected by luck)</style>");

			RegisterFragment("DODGE_CHANCE", "\n{0} chance to <style=cIsHealing>dodge</style> incoming damage.");
			RegisterFragment("DODGE_CHANCE_UNKNOWN", "\nChance to <style=cIsHealing>dodge</style> incoming damage.");
			RegisterFragment("DODGE_DETAIL", "\n<style=cStack>(Dodge chance is unaffected by luck)</style>");

			RegisterFragment("PLATING_EFFECT", "\nReduce all <style=cIsDamage>incoming damage</style> by {0}.");
			RegisterFragment("PLATING_DETAIL", "\n<style=cStack>(Incoming damage cannot be reduced below 1)</style>");

			RegisterFragment("FALL_REDUCTION", "\nReduces fall damage by {0}.");
			RegisterFragment("FALL_IMMUNE", "\nImmune to fall damage.");

			RegisterFragment("FORCE_REDUCTION", "\nReduces knockback by {0}.");
			RegisterFragment("FORCE_IMMUNE", "\nImmune to knockback.");



			RegisterFragment("PASSIVE_UNKNOWN_AURA", "\nEmit an aura that <style=cStack>(???)</style>");

			RegisterFragment("HEADHUNTER", "Gain the <style=cIsDamage>power</style> of any killed elite monster for {0}.");





			targetLanguage = "pt-BR";

			RegisterFragment("EQUIPMENT_STACK_EFFECT", "\n\nConta como {0} acúmulos");
			RegisterFragment("HOW_TO_CONVERT", "\nClique no icone de equipamento no canto inferior direito para converter");

			RegisterFragment("BASE_STACK_FORMAT", "{0} {1}");

			RegisterFragment("FLAT_VALUE", "{0}");
			RegisterFragment("PERCENT_VALUE", "{0}%");
			RegisterFragment("FLATREGEN_VALUE", "{0} PV/s");
			RegisterFragment("PERCENTREGEN_VALUE", "{0}% PV/s");
			RegisterFragment("DURATION_VALUE", "{0} s");
			RegisterFragment("METER_VALUE", "{0} m");

			RegisterFragment("FLAT_STACK_INC", "<style=cStack>(+{0} por acúmulo)</style>");
			RegisterFragment("PERCENT_STACK_INC", "<style=cStack>(+{0}% por acúmulo)</style>");
			RegisterFragment("FLATREGEN_STACK_INC", "<style=cStack>(+{0} PV/s por acúmulo)</style>");
			RegisterFragment("PERCENTREGEN_STACK_INC", "<style=cStack>(+{0}% PV/s por acúmulo)</style>");
			RegisterFragment("DURATION_STACK_INC", "<style=cStack>(+{0} s por acúmulo)</style>");
			RegisterFragment("METER_STACK_INC", "<style=cStack>(+{0} m por acúmulo)</style>");
			RegisterFragment("FLAT_STACK_DEC", "<style=cStack>(-{0} por acúmulo)</style>");
			RegisterFragment("PERCENT_STACK_DEC", "<style=cStack>(-{0}% por acúmulo)</style>");
			RegisterFragment("FLATREGEN_STACK_DEC", "<style=cStack>(-{0} PV/s por acúmulo)</style>");
			RegisterFragment("PERCENTREGEN_STACK_DEC", "<style=cStack>(-{0}% PV/s por acúmulo)</style>");
			RegisterFragment("DURATION_STACK_DEC", "<style=cStack>(-{0} s por acúmulo)</style>");
			RegisterFragment("METER_STACK_DEC", "<style=cStack>(-{0} m por acúmulo)</style>");

			RegisterFragment("BASE_DAMAGE", "base");
			RegisterFragment("TOTAL_DAMAGE", "TOTAL");

			RegisterFragment("FOR_SECOND", "por {0} segundo");
			RegisterFragment("FOR_SECONDS", "por {0} segundos");
			RegisterFragment("OVER_SECOND", "ao longo de {0} segundo");
			RegisterFragment("OVER_SECONDS", "ao longo de {0} segundos");
			RegisterFragment("AFTER_SECOND", "após {0} segundo");
			RegisterFragment("AFTER_SECONDS", "após {0} segundos");
			RegisterFragment("EVERY_SECOND", "a cada segundo");
			RegisterFragment("EVERY_SECONDS", "a cada {0} segundos");
			RegisterFragment("SECOND", "{0} segundo");
			RegisterFragment("SECONDS", "{0} segundos");



			RegisterFragment("AFFIX_WHITE_NAME", "Abraço Congelante");
			RegisterFragment("AFFIX_WHITE_PICKUP", "Torne-se um elemento do gelo.");
			RegisterFragment("AFFIX_WHITE_ACTIVE", "Libere um cristal de gelo que desativa habilidades.");
			RegisterFragment("AFFIX_WHITE_ACTIVE_ALT", "Libere um cristal de gelo que drena saúde.");
			RegisterFragment("ASPECT_OF_ICE", "<style=cDeath>Elemento do Gelo</style> :");
			RegisterFragment("CHILL_ON_HIT", "\nAtaques <style=cIsUtility>arrepiam</style> ao golpear {0}, reduzindo a <style=cIsUtility>velocidade de movimento</style> em <style=cIsUtility>80%</style>.");
			RegisterFragment("CHANCE_TO_FREEZE", "\nAtaques tem {0} de chance de <style=cIsUtility>congelarem</style> {1}.");
			RegisterFragment("FROST_BLADE", "\nAtaques disparam uma <style=cIsDamage>lâmina</style> que causa {0} de dano TOTAL.");

			RegisterFragment("AFFIX_BLUE_NAME", "Silêncio Entre Golpes");
			RegisterFragment("AFFIX_BLUE_PICKUP", "Torne-se um elemento do relâmpago.");
			RegisterFragment("AFFIX_BLUE_ACTIVE", "Teletransporta ao usar.");
			RegisterFragment("ASPECT_OF_LIGHTNING", "<style=cDeath>Elemento do Relâmpago</style> :");
			RegisterFragment("PASSIVE_SCATTER_BOMB", "\nSolte bombas de dispersão periodicamente ao seu redor.");
			RegisterFragment("SAP_ON_HIT", "\nAtaques <style=cIsUtility>exauriam</style> ao golpear {0}, reduzindo o <style=cIsUtility>dano</style> em {1}.");
			RegisterFragment("SCATTER_BOMB", "\nAtaques soltam bombas de dispersão que explodem causando {0} de dano TOTAL.");
			RegisterFragment("LIGHTNING_BOMB", "\nAtaques grudam uma <style=cIsDamage>bomba</style> que explode causando {0} de dano TOTAL {1}.");

			RegisterFragment("AFFIX_RED_NAME", "Nobreza de Ifrit");
			RegisterFragment("AFFIX_RED_PICKUP", "Torne-se um elemento de fogo.");
			RegisterFragment("AFFIX_RED_ACTIVE", "Lança um míssil flamejante teleguiado.");
			RegisterFragment("ASPECT_OF_FIRE", "<style=cDeath>Elemento de Fogo</style> :");
			RegisterFragment("PASSIVE_FIRE_TRAIL", "\nDeixe para trás um rastro de fogo que causa dano a inimigos em contato.");
			RegisterFragment("BURN_DOT", "\nAtaques <style=cIsDamage>queimam</style> ao golpear causando {0} de dano {1} {2}.");

			RegisterFragment("AFFIX_HAUNTED_NAME", "Tiara Espectral");
			RegisterFragment("AFFIX_HAUNTED_PICKUP", "Torne-se um elemento incorpóreo.");
			RegisterFragment("AFFIX_HAUNTED_ACTIVE", "Concede temporariamente aos aliados próximos uma chance de desviar de golpes.");
			RegisterFragment("ASPECT_OF_INCORPOREALITY", "<style=cDeath>Elemento Incorpóreo</style> :");
			RegisterFragment("PASSIVE_GHOST_AURA", "\nEmite uma aura que encobre aliados próximos.");
			RegisterFragment("PASSIVE_POSSESS", "\nGrude em alguns aliados próximos, possuindo-os.");
			RegisterFragment("SHRED_ON_HIT", "\nAtaques <style=cIsDamage>fragmentam</style> ao golpear {0}, reduzindo a <style=cIsDamage>armadura</style> em {1}.");
			RegisterFragment("GHOST_ARMOR", "\nConcede aos aliados próximos {0} de <style=cIsHealing>armadura</style> adicional.");
			RegisterFragment("GHOST_DODGE", "\nConcede aos aliados próximos {0} de chance de <style=cIsHealing>desviar</style>.");

			RegisterFragment("AFFIX_POISON_NAME", "Retaliação de N'kuhana");
			RegisterFragment("AFFIX_POISON_PICKUP", "Torne-se um elemento da corrupção.");
			RegisterFragment("AFFIX_POISON_ACTIVE", "Gera um Ouriço Malaquita aliado que herda todos os seus itens.");
			RegisterFragment("ASPECT_OF_CORRUPTION", "<style=cDeath>Elemento da Corrupção</style> :");
			RegisterFragment("PASSIVE_SPIKEBALL", "\nLibera periodicamente bolas cravejadas que brotam poços de espinhos de onde pousam.");
			RegisterFragment("PASSIVE_RUIN_AURA", "\nEmite uma aura que aplica <style=cIsDamage>ruína</style> a inimigos próximos.");
			RegisterFragment("RUIN_ON_HIT_BASIC", "\nAtaques <style=cIsDamage>arruínam</style> ao golpear {0}, impedindo a recuperação da saúde.");
			RegisterFragment("RUIN_ON_HIT", "\nAtaques <style=cIsDamage>arruínam</style> ao golpear {0}, aumentado o <style=cIsDamage>dano recebido</style> em {1}.");
			RegisterFragment("RUIN_DETAIL", "\n<style=cStack>(Ruína impede a recuperação da saúde)</style>");
			RegisterFragment("WEAKEN_ON_HIT", "\nAtaques <style=cIsDamage>enfraquecem</style> ao golpear {0}, reduzindo a <style=cIsDamage>armadura</style> em <style=cIsDamage>30</style>, a <style=cIsUtility>velocidade de movimento</style> em <style=cIsUtility>40%</style>, e o <style=cIsDamage>dano</style> em <style=cIsDamage>40%</style>.");

			RegisterFragment("AFFIX_LUNAR_NAME", "Design Compartilhado");
			RegisterFragment("AFFIX_LUNAR_PICKUP", "Torne-se um aspecto da perfeição.");
			RegisterFragment("AFFIX_LUNAR_ACTIVE", "Ganhe proteção temporária contra golpes poderosos.");
			RegisterFragment("ASPECT_OF_PERFECTION", "<style=cDeath>Aspecto da Perfeição</style> :");
			RegisterFragment("PASSIVE_LUNAR_PROJ", "\nLance projéteis periodicamente durante o combate.");
			RegisterFragment("CRIPPLE_ON_HIT", "\nAtaques <style=cIsDamage>paralisam</style> ao golpear {0}, reduzindo a <style=cIsDamage>armadura</style> em <style=cIsDamage>20</style> e a <style=cIsUtility>velocidade de movimento</style> em <style=cIsUtility>50%</style>.");

			RegisterFragment("AFFIX_EARTH_NAME", "Vossa Reafirmação");
			RegisterFragment("AFFIX_EARTH_PICKUP", "Torne-se um aspecto da terra.");
			RegisterFragment("AFFIX_EARTH_ACTIVE", "Concede uma alta regeneração de saúde breve.");
			RegisterFragment("ASPECT_OF_EARTH", "<style=cDeath>Aspecto da Terra</style> :");
			RegisterFragment("PASSIVE_HEAL_ALLY", "\nCure aliados próximos.");
			RegisterFragment("POACH_ON_HIT_BASIC", "\nAtaques <style=cIsUtility>caçam</style> ao golpear {0}, fazendo com que os golpes contra eles <style=cIsHealing>curem</style> em {1} do <style=cIsDamage>dano</style> causado.");
			RegisterFragment("POACH_ON_HIT", "\nAtaques <style=cIsUtility>caçam</style> ao golpear {0}, reduzindo a <style=cIsUtility>velocidade de ataque</style> em {1}.");
			RegisterFragment("POACH_DETAIL", "\n<style=cStack>(Golpes contra inimigos caçados curam em {0} do dano causado)</style>");
			RegisterFragment("HEAL_PERCENT_ON_HIT", "\n<style=cIsHealing>Cura</style> em {0} do <style=cIsDamage>dano</style> que você causa.");
			RegisterFragment("LEECH_MODIFIER_FORMULA", "\n<style=cStack>Modificador Sanguessuga =>\n  {0}{1}( [dmg] * [bl]{2} , {3} ){4}</style>");

			RegisterFragment("AFFIX_VOID_NAME", "Fratura Entrópica");
			RegisterFragment("AFFIX_VOID_PICKUP", "Torne-se um aspecto da nulidade.");
			RegisterFragment("AFFIX_VOID_ACTIVE", "Redefine todos os seus tempos de recarga.");
			RegisterFragment("ASPECT_OF_VOID", "<style=cDeath>Aspecto da Nulidade</style> :");
			RegisterFragment("PASSIVE_BLOCK", "\n<style=cIsHealing>Bloqueia</style> dano recebido uma vez. Recarrega após um atraso.");
			RegisterFragment("NULLIFY_ON_HIT", "\nAtaques <style=cIsUtility>anulam</style> ao golpear {0}");
			RegisterFragment("NULLIFY_DETAIL", "\n<style=cStack>(Inimigos com 3 acúmulos de anulação ficam enraizados por 3 segundos)</style>");
			RegisterFragment("COLLAPSE_DOT", "\nAtaques <style=cIsDamage>derrubam</style> ao golpear em {0} {1} de dano {2}.");
			RegisterFragment("COLLAPSE_DEFAULT", "\n<style=cIsDamage>100%</style> de chance de <style=cIsDamage>derrubar</style> um inimigo em <style=cIsDamage>400%</style> de dano base.");
			RegisterFragment("CORRUPT_ASPECT_ITEM", "\n<style=cIsVoid>Corrompe todos os Aspectos Itemizados</style>.");

			RegisterFragment("AFFIX_PLATED_NAME", "Incorporação da Subserviência");
			RegisterFragment("AFFIX_PLATED_PICKUP", "Torne-se um aspecto da resistência.");
			RegisterFragment("AFFIX_PLATED_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_ENDURANCE", "<style=cDeath>Aspecto da Resistência</style> :");
			RegisterFragment("PASSIVE_DEFENSE_PLATING", "\nGanhe placas defensivas que mitigam o dano pesado.");
			RegisterFragment("DAMAGEREDUCTION_ON_HIT", "\nAtaques <style=cIsUtility>sufocam</style> ao golpear {0}, reduzindo o dano causado.");

			RegisterFragment("AFFIX_WARPED_NAME", "Fé Extraviada");
			RegisterFragment("AFFIX_WARPED_PICKUP", "Torne-se um aspecto da gravidade.");
			RegisterFragment("AFFIX_WARPED_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_GRAVITY", "<style=cDeath>Aspecto da Gravidade</style> :");
			RegisterFragment("PASSIVE_DEFLECT_PROJ", "\nReflete periodicamente projéteis próximos.");
			RegisterFragment("LEVITATE_ON_HIT", "\nAtaques <style=cIsUtility>levitam</style> ao golpear {0}.");

			RegisterFragment("AFFIX_VEILED_NAME", "Maldição da Obscuridade");
			RegisterFragment("AFFIX_VEILED_PICKUP", "Torne-se um aspecto da ofuscação.");
			RegisterFragment("AFFIX_VEILED_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_OBFUSCATION", "<style=cDeath>Aspecto da Ofuscação</style> :");
			RegisterFragment("CLOAK_ON_HIT", "\nAtaques <style=cIsUtility>encobrem</style> você ao golpear.");
			RegisterFragment("CLOAK_ON_HIT_TIMED", "\nAtaques <style=cIsUtility>encobrem</style> você ao golpear {0}.");
			RegisterFragment("ELUSIVE_ON_HIT", "\nAtaques concedem <style=cIsUtility>elusivo</style> ao golpear.");
			RegisterFragment("ELUSIVE_EFFECT_MOVE_DETAIL", "\n<style=cStack>(Elusivo concede {0} de velocidade de movimento)</style>");
			RegisterFragment("ELUSIVE_EFFECT_DODGE_DETAIL", "\n<style=cStack>(Elusivo concede {0} de chance de desviar)</style>");
			RegisterFragment("ELUSIVE_EFFECT_BOTH_DETAIL", "\n<style=cStack>(Elusivo concede {0} de velocidade de movimento e {1} de chance de desviar)</style>");
			RegisterFragment("ELUSIVE_DECAY_DETAIL", "\n<style=cStack>(O efeito Elusivo decai em {0} a cada segundo)</style>");
			RegisterFragment("ELUSIVE_EFFECT", "\nEfeito Elusivo de {0}.");

			RegisterFragment("AFFIX_ARAGONITE_NAME", "Seu Temperamento");
			RegisterFragment("AFFIX_ARAGONITE_PICKUP", "Torne-se um aspecto da fúria.");
			RegisterFragment("AFFIX_ARAGONITE_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_FURY", "<style=cDeath>Aspecto da Fúria</style> :");
			RegisterFragment("PASSIVE_ANGRY_HIT", "\nDesencadeie uma explosão de raiva periodicamente ao golpear.");
			RegisterFragment("PASSIVE_ANGRY_AURA", "\nEmite uma aura que fortalece aliados próximos.");
			RegisterFragment("ANGRY_ATKSPD", "\nConcede a aliados próximos {0} de <style=cIsUtility>velocidade de movimento</style> aumentada.");
			RegisterFragment("ANGRY_MOVSPD", "\nConcede a aliados próximos {0} de <style=cIsDamage>velocidade de ataque</style> aumentada.");
			RegisterFragment("ANGRY_BOTHSPD", "\nConcede a aliados próximos {0} de <style=cIsUtility>velocidade de movimento</style> e <style=cIsDamage>velocidade de ataque</style> aumentada.");
			RegisterFragment("ANGRY_COOLDOWN", "\nConcede a aliados próximos {0} de redução da <style=cIsUtility>recarga de habildades</style>.");

			RegisterFragment("AFFIX_GOLD_NAME", "Coven d'Ouro");
			RegisterFragment("AFFIX_GOLD_PICKUP", "Torne-se um aspecto da fortuna.");
			RegisterFragment("AFFIX_GOLD_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_FORTUNE", "<style=cDeath>Aspecto da Fortuna</style> :");
			RegisterFragment("GOLD_ON_HIT", "\nAtaques concedem ouro ao golpear.");
			RegisterFragment("ITEMSCORE_REGEN", "\n<style=cIsHealing>regeneração de saúde</style> bônus baseada na quantidade e nível dos itens possuídos.");
			RegisterFragment("ITEMSCORE_REGEN_MULT", "\nMultiplicador de regeneração de pontuação do item de {0}.");

			RegisterFragment("AFFIX_SEPIA_NAME", "Reflexão Desaparecendo");
			RegisterFragment("AFFIX_SEPIA_PICKUP", "Torne-se um aspecto da ilusão.");
			RegisterFragment("AFFIX_SEPIA_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_ILLUSION", "<style=cDeath>Aspecto da Ilusão</style> :");
			RegisterFragment("SEPIABLIND_ON_HIT", "\nAtaques aplicam <style=cIsUtility>visão distorcida</style> ao golpear {0}, reduzindo a chance de golpear em {1}.");

			RegisterFragment("AFFIX_SANGUINE_NAME", "Fidelidade Sangrenta");
			RegisterFragment("AFFIX_SANGUINE_PICKUP", "Torne-se um aspecto do plano vermelho.");
			RegisterFragment("AFFIX_SANGUINE_ACTIVE", "Teletransporte-se e ganhe breve invulnerabilidade.");
			RegisterFragment("ASPECT_OF_REDPLANE", "<style=cDeath>Aspecto do Plano Vermelho</style> :");
			RegisterFragment("BLEED_DOT", "\nAtaques <style=cIsDamage>sangram</style> ao golpear causando {0} de dano base {1}.");
			RegisterFragment("DOT_AMP", "\nAumenta o <style=cIsDamage>multiplicador do dano degenerativo</style> em {0}.");

			RegisterFragment("AFFIX_NULLIFIER_NAME", "Bênção dos Parvos");
			RegisterFragment("AFFIX_NULLIFIER_PICKUP", "Torne-se um aspecto do vazio.");
			RegisterFragment("AFFIX_NULLIFIER_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_NULL", "<style=cDeath>Aspecto do Vazio</style> :");
			RegisterFragment("PASSIVE_NULL_AURA", "\nEmite uma aura que protege aliados próximos e penaliza inimigos próximos.");
			RegisterFragment("NULL_ON_HIT", "\nAtaques <style=cIsUtility>nulificam</style> ao golpear.");
			RegisterFragment("NULL_ON_HIT_SPD", "\nAtaques <style=cIsUtility>nulificam</style> ao golpear, reduzindo a <style=cIsUtility>velocidade de movimento</style> em {0}.");



			RegisterFragment("NEARBY_ARMOR", "\nConcede a aliados próximos {0} de <style=cIsHealing>armadura</style> adicional.");
			RegisterFragment("NEARBY_ARMOR_UNKNOWN", "\nConcede a aliados próximos <style=cIsHealing>armadura</style> adicional.");

			RegisterFragment("CONVERT_SHIELD", "\nConverta {0} da sua saúde em <style=cIsHealing>escudo regenerador</style>.");
			RegisterFragment("EXTRA_SHIELD_CONVERT", "\nGanhe {0} de <style=cIsHealing>escudo</style> extra da conversão.");
			RegisterFragment("CONVERT_SHIELD_REGEN", "\nPelo menos {0} da <style=cIsHealing>regeneração da saúde</style> se aplica ao <style=cIsHealing>escudo</style>.");
			RegisterFragment("DISABLE_OOD_SHIELD_RECOVERY", "\n<style=cStack>(A recuperação de escudo natural é desativada)</style>");

			RegisterFragment("STAT_HEALTH_EXTRA_SHIELD", "\nGanhe {0} de saúde como <style=cIsHealing>escudo</style> extra.");
			RegisterFragment("STAT_EXTRA_JUMP", "\nGanhe <style=cIsUtility>+1</style> de <style=cIsUtility>quantidade de saltos</style> máximos.");
			RegisterFragment("STAT_MOVESPEED", "\nAumenta a <style=cIsUtility>velocidade de movimento</style> em {0}.");
			RegisterFragment("STAT_ATTACKSPEED", "\nAumenta a <style=cIsDamage>velocidade de ataque</style> em {0}.");
			RegisterFragment("STAT_BOTHSPEED", "\nAumenta a <style=cIsUtility>velocidade de movimento</style> e a <style=cIsDamage>velocidade de ataque</style> em {0}.");
			RegisterFragment("STAT_ARMOR", "\nAumenta a <style=cIsHealing>armadura</style> em {0}.");
			RegisterFragment("STAT_HEALTH", "\nAumenta a <style=cIsHealing>saúde máxima</style> em {0}.");
			RegisterFragment("STAT_REGENERATION", "\nAumenta a <style=cIsHealing>regeneração de saúde</style> em {0}.");
			RegisterFragment("STAT_DAMAGE", "\nAumenta o <style=cIsDamage>dano</style> em {0}.");
			RegisterFragment("STAT_COOLDOWN", "\nReduz os <style=cIsUtility>tempos de recarga das habilidades</style> em {0}.");

			RegisterFragment("LARGE_SHIELD_UNKNOWN", "\nGanhe uma grande quantidade de <style=cIsHealing>escudo regenerador</style>.");

			RegisterFragment("BLOCK_CHANCE", "\n{0} de chance de <style=cIsHealing>bloquear</style> o dano recebido.");
			RegisterFragment("BLOCK_CHANCE_UNKNOWN", "\nChance de <style=cIsHealing>bloquear</style> o dano recebido.");
			RegisterFragment("BLOCK_DETAIL", "\n<style=cStack>(A chance de bloquear não é afetada pela sorte)</style>");

			RegisterFragment("DODGE_CHANCE", "\n{0} de chance de <style=cIsHealing>desviar</style> o dano recebido.");
			RegisterFragment("DODGE_CHANCE_UNKNOWN", "\nChance de <style=cIsHealing>desviar</style> o dano recebido.");
			RegisterFragment("DODGE_DETAIL", "\n<style=cStack>(A chance de desviar não é afetada pela sorte)</style>");

			RegisterFragment("PLATING_EFFECT", "\nReduz todo o <style=cIsDamage>dano recebido</style> em {0}.");
			RegisterFragment("PLATING_DETAIL", "\n<style=cStack>(O dano recebido não pode ser reduzido para menos de 1)</style>");

			RegisterFragment("FALL_REDUCTION", "\nReduz o dano de queda em {0}.");
			RegisterFragment("FALL_IMMUNE", "\nImune a dano de queda.");

			RegisterFragment("FORCE_REDUCTION", "\nReduz o empurrão em {0}.");
			RegisterFragment("FORCE_IMMUNE", "\nImune ao empurrão.");



			RegisterFragment("PASSIVE_UNKNOWN_AURA", "\nEmite uma aura que <style=cStack>(???)</style>");

			RegisterFragment("HEADHUNTER", "Ganhe o <style=cIsDamage>poder</style> de qualquer monstro de elite abatido por {0}.");




			targetLanguage = "ko";

			RegisterFragment("EQUIPMENT_STACK_EFFECT", "\n\n{0} 중첩으로 계산됩니다.");
			RegisterFragment("HOW_TO_CONVERT", "\n우측 하단 장비 아이콘을 클릭하여 변환");

			RegisterFragment("BASE_STACK_FORMAT", "{0} {1}");

			RegisterFragment("FLAT_VALUE", "{0}");
			RegisterFragment("PERCENT_VALUE", "{0}%");
			RegisterFragment("FLATREGEN_VALUE", "{0} hp/s");
			RegisterFragment("PERCENTREGEN_VALUE", "{0}% hp/s");
			RegisterFragment("DURATION_VALUE", "{0}s");
			RegisterFragment("METER_VALUE", "{0}m");

			RegisterFragment("FLAT_STACK_INC", "<style=cStack>(중첩당 +{0})</style>");
			RegisterFragment("PERCENT_STACK_INC", "<style=cStack>(중첩당 +{0}%)</style>");
			RegisterFragment("FLATREGEN_STACK_INC", "<style=cStack>(중첩당 +{0} hp/s)</style>");
			RegisterFragment("PERCENTREGEN_STACK_INC", "<style=cStack>(중첩당 +{0}% hp/s)</style>");
			RegisterFragment("DURATION_STACK_INC", "<style=cStack>(중첩당 +{0}s)</style>");
			RegisterFragment("METER_STACK_INC", "<style=cStack>(중첩당 +{0}m)</style>");
			RegisterFragment("FLAT_STACK_DEC", "<style=cStack>(중첩당 -{0})</style>");
			RegisterFragment("PERCENT_STACK_DEC", "<style=cStack>(중첩당 -{0}%)</style>");
			RegisterFragment("FLATREGEN_STACK_DEC", "<style=cStack>(중첩당 -{0} hp/s)</style>");
			RegisterFragment("PERCENTREGEN_STACK_DEC", "<style=cStack>(중첩당 -{0}% hp/s)</style>");
			RegisterFragment("DURATION_STACK_DEC", "<style=cStack>(중첩당 -{0}s)</style>");
			RegisterFragment("METER_STACK_DEC", "<style=cStack>(중첩당 -{0}m)</style>");

			RegisterFragment("BASE_DAMAGE", "기본");
			RegisterFragment("TOTAL_DAMAGE", "총");

			RegisterFragment("FOR_SECOND", "{0} 초 동안");
			RegisterFragment("FOR_SECONDS", "{0} 초 동안");
			RegisterFragment("OVER_SECOND", "{0} 초 이상");
			RegisterFragment("OVER_SECONDS", "{0} 초에 걸쳐");
			RegisterFragment("AFTER_SECOND", "{0} 초 후");
			RegisterFragment("AFTER_SECONDS", "{0} 초 후");
			RegisterFragment("EVERY_SECOND", "매초");
			RegisterFragment("EVERY_SECONDS", "{0} 초마다");
			RegisterFragment("SECOND", "{0} 초");
			RegisterFragment("SECONDS", "{0} 초");



			RegisterFragment("AFFIX_WHITE_NAME", "살을 에는 그녀의 포옹");
			RegisterFragment("AFFIX_WHITE_PICKUP", "얼음의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_WHITE_ACTIVE", "스킬을 비활성화하는 얼음 결정을 배치합니다.");
			RegisterFragment("AFFIX_WHITE_ACTIVE_ALT", "체력을 흡수하는 얼음 결정을 배치합니다.");
			RegisterFragment("ASPECT_OF_ICE", "<style=cDeath>얼음의 형태</style> :");
			RegisterFragment("CHILL_ON_HIT", "\n공격 적중 시 <style=cIsUtility>냉기</style>를 부여해 {0}, <style=cIsUtility>이동 속도</style>를 <style=cIsUtility>80%</style> 감소시킵니다.");
			RegisterFragment("CHANCE_TO_FREEZE", "\n공격은 {0} 확률로 {1} <style=cIsUtility>동결</style> 시킵니다.");
			RegisterFragment("FROST_BLADE", "\n공격 시 {0} 피해를 입히는 <style=cIsDamage>날</style>을 발사합니다.");

			RegisterFragment("AFFIX_BLUE_NAME", "두 타격 사이의 고요");
			RegisterFragment("AFFIX_BLUE_PICKUP", "번개의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_BLUE_ACTIVE", "텔레포트 사용.");
			RegisterFragment("ASPECT_OF_LIGHTNING", "<style=cDeath>번개의 형태</style> :");
			RegisterFragment("PASSIVE_SCATTER_BOMB", "\n때때로 주변에 분산 폭탄을 떨어뜨립니다.");
			RegisterFragment("SAP_ON_HIT", "\n공격 적중 시 <style=cIsUtility>부하</style>를 부여해 {0}, <style=cIsUtility>피해량</style>을 {1} 감소시킵니다.");
			RegisterFragment("SCATTER_BOMB", "\n공격 시 {0} 의 피해를 주는 폭발하는 산란 폭탄을 투하합니다.");
			RegisterFragment("LIGHTNING_BOMB", "\n공격 적중 시 {1} 폭발하는 <style=cIsDamage>폭탄</style>을 부착하여 {0} 피해를 입힙니다.");

			RegisterFragment("AFFIX_RED_NAME", "이프리트의 차이");
			RegisterFragment("AFFIX_RED_PICKUP", "불의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_RED_ACTIVE", "유도 화염탄을 발사합니다.");
			RegisterFragment("ASPECT_OF_FIRE", "<style=cDeath>불의 형태</style> :");
			RegisterFragment("PASSIVE_FIRE_TRAIL", "\n적에게 닿으면 피해를 입히는 불타는 흔적을 남깁니다.");
			RegisterFragment("BURN_DOT", "\n공격 적중 시 <style=cIsDamage>화상</style>을 부여하고 {2}, {0} {1} 피해를 입힙니다.");

			RegisterFragment("AFFIX_HAUNTED_NAME", "유령 왕관");
			RegisterFragment("AFFIX_HAUNTED_PICKUP", "무형의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_HAUNTED_ACTIVE", "주변 아군에게 일시적으로 타격을 피할 수 있는 기회를 부여합니다.");
			RegisterFragment("ASPECT_OF_INCORPOREALITY", "<style=cDeath>무형의 형태</style> :");
			RegisterFragment("PASSIVE_GHOST_AURA", "\n주변 아군을 은폐하는 오라를 발산합니다.");
			RegisterFragment("PASSIVE_POSSESS", "\n주변 아군들에게 들러붙어 빙의합니다.");
			RegisterFragment("SHRED_ON_HIT", "\n공격 적중 시 <style=cIsDamage>파쇄</style>를 부여해 {0}, <style=cIsDamage>방어력</style>을 {1} 감소시킵니다.");
			RegisterFragment("GHOST_ARMOR", "\n주변 아군에게 {0} 추가 <style=cIsHealing>방어력</style>을 부여합니다.");
			RegisterFragment("GHOST_DODGE", "\n주변 아군에게 {0} 확률의 <style=cIsHealing>회피</style> 기회를 부여합니다.");

			RegisterFragment("AFFIX_POISON_NAME", "은쿠하나의 보고서");
			RegisterFragment("AFFIX_POISON_PICKUP", "부패의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_POISON_ACTIVE", "아이템을 물려받는 아군 말라카이트 성게를 소환합니다.");
			RegisterFragment("ASPECT_OF_CORRUPTION", "<style=cDeath>부패의 형태</style> :");
			RegisterFragment("PASSIVE_SPIKEBALL", "\n떨어지는 곳에서 가시 구덩이를 생성하는 스파이크 볼을 주기적으로 방출합니다.");
			RegisterFragment("PASSIVE_RUIN_AURA", "\n가까운 적에게 <style=cIsDamage>파멸</style>을 적용하는 오라를 방출합니다.");
			RegisterFragment("RUIN_ON_HIT_BASIC", "\n공격 적중 시 <style=cIsDamage>파멸</style>를 부여해 {0}, 체력 회복을 막습니다.");
			RegisterFragment("RUIN_ON_HIT", "\n공격 적중 시 <style=cIsDamage>파멸</style>을 부여해 {0}, <style=cIsDamage>받는 피해량</style>을 {1} 증가시킵니다.");
			RegisterFragment("RUIN_DETAIL", "\n<style=cStack>(파멸은 체력 회복을 방해합니다)</style>");
			RegisterFragment("WEAKEN_ON_HIT", "\n공격 적중 시 <style=cIsDamage>약화</style>를 부여해 {0}, <style=cIsDamage>방어력</style>을 <style=cIsDamage>30</style>, <style=cIsUtility>이동 속도</style>를 <style=cIsUtility>40%</style>, 그리고 <style=cIsDamage>피해량</style>을 <style=cIsDamage>40%</style> 감소시킵니다.");

			RegisterFragment("AFFIX_LUNAR_NAME", "공유된 설계");
			RegisterFragment("AFFIX_LUNAR_PICKUP", "완성의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_LUNAR_ACTIVE", "강력한 타격에 대해 일시적인 보호를 얻습니다.");
			RegisterFragment("ASPECT_OF_PERFECTION", "<style=cDeath>완성의 형태</style> :");
			RegisterFragment("PASSIVE_LUNAR_PROJ", "\n전투 중에 주기적으로 발사체를 발사합니다.");
			RegisterFragment("CRIPPLE_ON_HIT", "\n공격 적중 시 <style=cIsDamage>손상</style>을 부여해 {0}, <style=cIsDamage>방어력</style>을 <style=cIsDamage>20</style> 그리고 <style=cIsUtility>이동 속도</style>를 <style=cIsUtility>50%</style> 감소시킵니다.");

			RegisterFragment("AFFIX_EARTH_NAME", "그의 안도");
			RegisterFragment("AFFIX_EARTH_PICKUP", "땅의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_EARTH_ACTIVE", "짧은 시간 동안 체력 재생을 증가 시킵니다.");
			RegisterFragment("ASPECT_OF_EARTH", "<style=cDeath>땅의 형태</style> :");
			RegisterFragment("PASSIVE_HEAL_ALLY", "\n주변의 아군을 치료합니다.");
			RegisterFragment("POACH_ON_HIT_BASIC", "\n공격 적중 시 <style=cIsUtility>밀렵</style>을 부여해 {0}, 입힌 <style=cIsDamage>피해</style>의 {1}만큼 <style=cIsHealing>회복</style>합니다.");
			RegisterFragment("POACH_ON_HIT", "\n공격 적중 시 <style=cIsUtility>밀렵</style>을 부여해 {0}, <style=cIsUtility>공격 속도</style>를 {1} 감소시킵니다.");
			RegisterFragment("POACH_DETAIL", "\n<style=cStack>(밀렵된 적 타격 시 입힌 피해의 {0}만큼 회복)</style>");
			RegisterFragment("HEAL_PERCENT_ON_HIT", "\n입힌 <style=cIsDamage>피해</style>의 {0}만큼 <style=cIsHealing>회복</style>합니다.");
			RegisterFragment("LEECH_MODIFIER_FORMULA", "\n<style=cStack>거머리 수정자 =>\n  {0}{1}( [dmg] * [bl]{2} , {3} ){4}</style>");

			RegisterFragment("AFFIX_VOID_NAME", "엔트로피 파괴");
			RegisterFragment("AFFIX_VOID_PICKUP", "공허의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_VOID_ACTIVE", "모든 재사용 대기시간을 재설정합니다.");
			RegisterFragment("ASPECT_OF_VOID", "<style=cDeath>공허의 형태</style> :");
			RegisterFragment("PASSIVE_BLOCK", "\n피해를 한 번 <style=cIsHealing>무시</style>합니다. 지연 후 재충전됩니다.");
			RegisterFragment("NULLIFY_ON_HIT", "\n공격 적중 시 {0} <style=cIsUtility>무효</style>를 부여합니다.");
			RegisterFragment("NULLIFY_DETAIL", "\n<style=cStack>(무효 중첩이 3개인 적은 3초 동안 속박됩니다)</style>");
			RegisterFragment("COLLAPSE_DOT", "\n공격 적중 시 <style=cIsDamage>붕괴</style>를 부여하고 {2}, {0} {1} 피해를 입힙니다.");
			RegisterFragment("COLLAPSE_DEFAULT", "\n<style=cIsDamage>100%</style> 확률로 적에게 <style=cIsDamage>붕괴</style>를 부여하고 <style=cIsDamage>400%</style> 피해를 입힙니다.");
			RegisterFragment("CORRUPT_ASPECT_ITEM", "\n<style=cIsVoid>모든 형태를 오염시킵니다</style>.");

			RegisterFragment("AFFIX_PLATED_NAME", "복종의 합금");
			RegisterFragment("AFFIX_PLATED_PICKUP", "인내의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_PLATED_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_ENDURANCE", "<style=cDeath>인내의 형태</style> :");
			RegisterFragment("PASSIVE_DEFENSE_PLATING", "\n큰 피해를 완화하는 방어 장갑을 얻습니다.");
			RegisterFragment("DAMAGEREDUCTION_ON_HIT", "\n공격 적중 시 <style=cIsUtility>억압</style>을 부여해 {0}, 적에게서 받는 피해를 감소시킵니다.");

			RegisterFragment("AFFIX_WARPED_NAME", "잘못된 믿음");
			RegisterFragment("AFFIX_WARPED_PICKUP", "중력의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_WARPED_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_GRAVITY", "<style=cDeath>중력의 형태</style> :");
			RegisterFragment("PASSIVE_DEFLECT_PROJ", "\n근처의 투사체를 가끔 빗나가게 만듭니다.");
			RegisterFragment("LEVITATE_ON_HIT", "\n공격 적중 시 <style=cIsUtility>부양</style>을 부여해 {0} 공중에 떠오르게 만듭니다.");

			RegisterFragment("AFFIX_VEILED_NAME", "무명의 저주");
			RegisterFragment("AFFIX_VEILED_PICKUP", "맹의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_VEILED_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_OBFUSCATION", "<style=cDeath>맹의 형태</style> :");
			RegisterFragment("CLOAK_ON_HIT", "\n공격 적중 시 <style=cIsUtility>은신</style> 합니다.");
			RegisterFragment("CLOAK_ON_HIT_TIMED", "\n공격 적중 시 {0} <style=cIsUtility>은신</style> 합니다.");
			RegisterFragment("ELUSIVE_ON_HIT", "\n공격 적중 시 <style=cIsUtility>은신</style> 합니다.");
			RegisterFragment("ELUSIVE_EFFECT_MOVE_DETAIL", "\n<style=cStack>(은신은 {0} 이동 속도 부여)</style>");
			RegisterFragment("ELUSIVE_EFFECT_DODGE_DETAIL", "\n<style=cStack>(은신은 {0} 회피 확률 부여)</style>");
			RegisterFragment("ELUSIVE_EFFECT_BOTH_DETAIL", "\n<style=cStack>(은신은 {0} 이동 속도 증가와 {1} 회피 확률 증가)</style>");
			RegisterFragment("ELUSIVE_DECAY_DETAIL", "\n<style=cStack>(은신 효과 및 추가 능력은 매초마다 {0} 씩 사라집니다)</style>");
			RegisterFragment("ELUSIVE_EFFECT", "\n은신 시 효과 {0}.");

			RegisterFragment("AFFIX_ARAGONITE_NAME", "그녀의 성질");
			RegisterFragment("AFFIX_ARAGONITE_PICKUP", "분노의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_ARAGONITE_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_FURY", "<style=cDeath>분노의 형태</style> :");
			RegisterFragment("PASSIVE_ANGRY_HIT", "\n공격 적중 시 가끔 분노 폭발을 일으킵니다.");
			RegisterFragment("PASSIVE_ANGRY_AURA", "\n근처 아군을 강화하는 오라를 발산합니다.");
			RegisterFragment("ANGRY_ATKSPD", "\n주변 아군의 <style=cIsUtility>이동 속도</style>를 {0} 증가시킵니다.");
			RegisterFragment("ANGRY_MOVSPD", "\n주변 아군의 <style=cIsDamage>공격 속도</style>를 {0} 증가시킵니다.");
			RegisterFragment("ANGRY_BOTHSPD", "\n주변 아군의 <style=cIsUtility>이동 속도</style>와 <style=cIsDamage>공격 속도</style>를 {0} 증가시킵니다.");
			RegisterFragment("ANGRY_COOLDOWN", "\n주변 아군의 <style=cIsUtility>스킬 재사용 대기시간</style>을 {0} 감소시킵니다.");

			RegisterFragment("AFFIX_GOLD_NAME", "골드의 집회");
			RegisterFragment("AFFIX_GOLD_PICKUP", "금전의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_GOLD_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_FORTUNE", "<style=cDeath>금전의 형태</style> :");
			RegisterFragment("GOLD_ON_HIT", "\n공격 적중 시 골드를 획득합니다.");
			RegisterFragment("ITEMSCORE_REGEN", "\n소유한 아이템의 수량과 등급에 따라 보너스 <style=cIsHealing>체력 재생</style>이 주어집니다.");
			RegisterFragment("ITEMSCORE_REGEN_MULT", "\n{0}의 아이템 점수 재생 계수.");

			RegisterFragment("AFFIX_SEPIA_NAME", "흐려지는 반사");
			RegisterFragment("AFFIX_SEPIA_PICKUP", "환상의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_SEPIA_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_ILLUSION", "<style=cDeath>환상의 형태</style> :");
			RegisterFragment("SEPIABLIND_ON_HIT", "\n공격 적중 시 <style=cIsUtility>왜곡된 시야</style>를 부여해 {0}, 명중률을 {1} 감소시킵니다.");

			RegisterFragment("AFFIX_SANGUINE_NAME", "피투성이 서약");
			RegisterFragment("AFFIX_SANGUINE_PICKUP", "피의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_SANGUINE_ACTIVE", "순간이동으로 돌진하여 짧은 무적 판정을 얻습니다.");
			RegisterFragment("ASPECT_OF_REDPLANE", "<style=cDeath>피의 형태</style> :");
			RegisterFragment("BLEED_DOT", "\n공격 적중 시 <style=cIsDamage>출혈</style>을 부여해 {1}, {0} 피해를 입힙니다.");
			RegisterFragment("DOT_AMP", "\n<style=cIsDamage>지속 피해</style>를 {0} 증가시킵니다.");

			RegisterFragment("AFFIX_NULLIFIER_NAME", "파보스의 축복");
			RegisterFragment("AFFIX_NULLIFIER_PICKUP", "무효의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_NULLIFIER_ACTIVE", "<style=cStack>(???)</style>");
			RegisterFragment("ASPECT_OF_NULL", "<style=cDeath>무효의 형태</style> :");
			RegisterFragment("PASSIVE_NULL_AURA", "\n근처의 아군을 보호하고 적에게 디버프를 주는 오라를 발산합니다.");
			RegisterFragment("NULL_ON_HIT", "\n공격 적중 시 <style=cIsUtility>파기</style>를 부여합니다..");
			RegisterFragment("NULL_ON_HIT_SPD", "\n공격 적중 시<style=cIsUtility>파기</style>를 부여해, <style=cIsUtility>이동 속도</style>를 {0} 감소시킵니다.");

			RegisterFragment("AFFIX_BLIGHTED_NAME", "방벽의 배신");
			RegisterFragment("AFFIX_BLIGHTED_PICKUP", "쇠퇴의 한 형태가 됩니다.");
			RegisterFragment("AFFIX_BLIGHTED_ACTIVE", "무작위 엘리트 형태를 다시 굴립니다.");
			RegisterFragment("ASPECT_OF_DECAY", "<style=cDeath>쇠퇴의 형태</style> :");
			RegisterFragment("PASSIVE_BLIGHT", "\n임의의 엘리트 형태를 2개 획득합니다.");



			RegisterFragment("NEARBY_ARMOR", "\n주변 아군에게 {0} 추가 <style=cIsHealing>방어력</style>을 부여합니다.");
			RegisterFragment("NEARBY_ARMOR_UNKNOWN", "\n주변 아군에게 추가 <style=cIsHealing>방어력</style>을 부여합니다.");

			RegisterFragment("CONVERT_SHIELD", "\n체력의 {0} 를 <style=cIsHealing>재생되는 보호막</style>으로 전환합니다.");
			RegisterFragment("EXTRA_SHIELD_CONVERT", "\n전환으로 {0} 추가 <style=cIsHealing>보호막</style>을 얻습니다.");
			RegisterFragment("CONVERT_SHIELD_REGEN", "\n최소 {0} 의 <style=cIsHealing>체력 재생</style> 이 <style=cIsHealing>보호막</style>에 적용됩니다.");
			RegisterFragment("DISABLE_OOD_SHIELD_RECOVERY", "\n<style=cStack>(자연 보호막 복구 비활성화)</style>");

			RegisterFragment("STAT_HEALTH_EXTRA_SHIELD", "\n최대 체력의 {0} 해당하는 추가 <style=cIsHealing>보호막</style>을 얻습니다.");
			RegisterFragment("STAT_EXTRA_JUMP", "\n최대 <style=cIsUtility>점프 횟수</style>를 <style=cIsUtility>+1</style> 얻습니다.");
			RegisterFragment("STAT_MOVESPEED", "\n<style=cIsUtility>이동 속도</style>를 {0} 증가시킵니다.");
			RegisterFragment("STAT_ATTACKSPEED", "\n<style=cIsDamage>공격 속도</style>를 {0} 증가시킵니다.");
			RegisterFragment("STAT_BOTHSPEED", "\n<style=cIsUtility>이동 속도</style> 와 <style=cIsDamage>공격 속도</style>를 {0} 증가시킵니다.");
			RegisterFragment("STAT_ARMOR", "\n<style=cIsHealing>방어력</style>을 {0} 증가시킵니다.");
			RegisterFragment("STAT_HEALTH", "\n<style=cIsHealing>최대 체력</style>을 {0} 증가시킵니다.");
			RegisterFragment("STAT_REGENERATION", "\n<style=cIsHealing>체력 재생</style>을 {0} 증가시킵니다.");
			RegisterFragment("STAT_DAMAGE", "\n<style=cIsDamage>피해량</style>을 {0} 증가시킵니다.");
			RegisterFragment("STAT_COOLDOWN", "\n<style=cIsUtility>스킬 재사용 대기시간</style>을 {0} 감소시킵니다.");

			RegisterFragment("LARGE_SHIELD_UNKNOWN", "\n대량의 <style=cIsHealing>재생 보호막</style>을 얻습니다.");

			RegisterFragment("BLOCK_CHANCE", "\n{0} 확률로 받는 피해를 <style=cIsHealing>차단</style> 합니다.");
			RegisterFragment("BLOCK_CHANCE_UNKNOWN", "\n확률로 받는 피해를 <style=cIsHealing>차단</style> 합니다.");
			RegisterFragment("BLOCK_DETAIL", "\n<style=cStack>(차단확률은 운에 영향을 받지 않습니다)</style>");

			RegisterFragment("DODGE_CHANCE", "\n{0} 확률로 받는 피해를 <style=cIsHealing>회피</style> 합니다.");
			RegisterFragment("DODGE_CHANCE_UNKNOWN", "\n확률로 받는 피해를 <style=cIsHealing>회피</style> 합니다.");
			RegisterFragment("DODGE_DETAIL", "\n<style=cStack>(회피확률은 운에 영향을 받지 않습니다)</style>");

			RegisterFragment("PLATING_EFFECT", "\n모든 <style=cIsDamage>받는 피해</style>를 {0} 감소시킵니다.");
			RegisterFragment("PLATING_DETAIL", "\n<style=cStack>(받는 데미지는 1 미만으로 감소할 수 없습니다)</style>");

			RegisterFragment("FALL_REDUCTION", "\n낙하 피해를 {0} 감소시킵니다.");
			RegisterFragment("FALL_IMMUNE", "\n낙하 피해에 면역입니다.");

			RegisterFragment("FORCE_REDUCTION", "\n넉백을 {0} 감소시킵니다.");
			RegisterFragment("FORCE_IMMUNE", "\n넉백에 면역입니다.");



			RegisterFragment("PASSIVE_UNKNOWN_AURA", "\n오라를 내뿜다. <style=cStack>(???)</style>");

			RegisterFragment("HEADHUNTER", "{0} 동안 처치한 엘리트 몬스터의 <style=cIsDamage>힘</style> 을 얻습니다.");





			targetLanguage = "";
		}



		public static string TextFragment(string key, bool trim = false)
		{
			if (targetLanguage != "" || targetLanguage != "default")
			{
				if (fragments.ContainsKey(targetLanguage))
				{
					if (fragments[targetLanguage].ContainsKey(key))
					{
						//Logger.Info("Found fragment (" + key + ") in (" + targetFragmentLanguage + ") fragment language.");
						string output = fragments[targetLanguage][key];
						if (trim) output = output.Trim('\n');
						return output;
					}
				}
			}

			if (fragments.ContainsKey("default"))
			{
				if (fragments["default"].ContainsKey(key))
				{
					//Logger.Info("Found fragment (" + key + ") in (default) fragment language.");
					string output = fragments["default"][key];
					if (trim) output = output.Trim('\n');
					return output;
				}
			}

			Logger.Info("Failed to find fragment (" + key + ") in any fragment language.");
			return "[" + key + "]";
		}



		public static string ScalingText(float baseValue, float stackValue, string modifier = "", string style = "", bool combine = false)
		{
			if (stackValue == 0f)
			{
				return ScalingText(baseValue, modifier, style);
			}

			if (combine)
			{
				float stacks = Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);
				float value = baseValue + stackValue * (stacks - 1f);

				return ScalingText(value, modifier, style);
			}

			float mult = (modifier == "percent" || modifier == "percentregen") ? 100f : 1f;
			string sign = stackValue > 0f ? "INC" : "DEC";

			string baseString, stackString;
			if (modifier == "percent" || modifier == "chance")
			{
				baseString = TextFragment("PERCENT_VALUE");
				stackString = TextFragment("PERCENT_STACK_" + sign);
			}
			else if (modifier == "flatregen")
			{
				baseString = TextFragment("FLATREGEN_VALUE");
				stackString = TextFragment("FLATREGEN_STACK_" + sign);
			}
			else if (modifier == "percentregen")
			{
				baseString = TextFragment("PERCENTREGEN_VALUE");
				stackString = TextFragment("PERCENTREGEN_STACK_" + sign);
			}
			else if (modifier == "duration")
			{
				baseString = TextFragment("DURATION_VALUE");
				stackString = TextFragment("DURATION_STACK_" + sign);
			}
			else if (modifier == "meter")
			{
				baseString = TextFragment("METER_VALUE");
				stackString = TextFragment("METER_STACK_" + sign);
			}
			else
			{
				baseString = TextFragment("FLAT_VALUE");
				stackString = TextFragment("FLAT_STACK_" + sign);
			}

			string formatString = TextFragment("BASE_STACK_FORMAT");

			baseString = String.Format(baseString, baseValue * mult);
			stackString = String.Format(stackString, Mathf.Abs(stackValue * mult));

			if (style != "") baseString = "<style=" + style + ">" + baseString + "</style>";

			string output = String.Format(formatString, baseString, stackString);

			return output;
		}

		public static string ScalingText(float value, string modifier = "", string style = "")
		{
			if (modifier == "percent" || modifier == "percentregen") value *= 100f;

			string baseString;
			if (modifier == "percent" || modifier == "chance") baseString = TextFragment("PERCENT_VALUE");
			else if (modifier == "flatregen") baseString = TextFragment("FLATREGEN_VALUE");
			else if (modifier == "percentregen") baseString = TextFragment("PERCENTREGEN_VALUE");
			else if (modifier == "duration") baseString = TextFragment("DURATION_VALUE");
			else if (modifier == "meter") baseString = TextFragment("METER_VALUE");
			else baseString = TextFragment("FLAT_VALUE");

			string output = String.Format(baseString, value);

			if (style != "") output = "<style=" + style + ">" + output + "</style>";

			return output;
		}
		
		public static string SecondText(float sec, string modifier = "")
		{
			string targetString;
			if (modifier == "for") targetString = "FOR_SECOND";
			else if (modifier == "over") targetString = "OVER_SECOND";
			else if (modifier == "after") targetString = "AFTER_SECOND";
			else targetString = "SECOND";

			if (sec != 1f) targetString += "S";

			return String.Format(TextFragment(targetString), sec);
		}

		public static string EquipmentStackText(float stack)
		{
			stack = Mathf.Max(1f, stack);

			if (!DropHooks.CanObtainItem()) return "";
			if (stack == 1f) return "";

			return String.Format(TextFragment("EQUIPMENT_STACK_EFFECT"), stack);
		}

		public static string EquipmentDescription(string baseDesc, string activeEffect, bool activeAlwaysAvailable = false)
		{
			string output = "";
			if (Catalog.aspectAbilities || activeAlwaysAvailable) output += activeEffect + "\n\n";
			output += baseDesc;
			output += EquipmentStackText(Configuration.AspectEquipmentEffect.Value);
			if (Configuration.AspectEquipmentConversion.Value) output += TextFragment("HOW_TO_CONVERT");

			return output;
		}



		internal static void Init()
		{
			SetupFragments();

			RegisteredHook();
			StringHook();
		}

		private static void RegisteredHook()
		{
			On.RoR2.Language.TokenIsRegistered += (orig, self, token) =>
			{
				string language = self.name;

				if (token != null)
				{
					if (tokens.ContainsKey(language))
					{
						if (tokens[language].ContainsKey(token)) return true;
					}
					if (tokens.ContainsKey("default"))
					{
						if (tokens["default"].ContainsKey(token)) return true;
					}
				}

				return orig(self, token);
			};
		}

		private static void StringHook()
		{
			On.RoR2.Language.GetLocalizedStringByToken += (orig, self, token) =>
			{
				string language = self.name;

				if (token != null)
				{
					if (tokens.ContainsKey(language))
					{
						if (tokens[language].ContainsKey(token)) return tokens[language][token];
					}
					if (tokens.ContainsKey("default"))
					{
						if (tokens["default"].ContainsKey(token)) return tokens["default"][token];
					}
				}

				return orig(self, token);
			};
		}



		internal static void ChangeText()
		{
			string text;

			targetLanguage = "default";

			text = String.Format(
				TextFragment("HEADHUNTER"),
				ScalingText(Configuration.HeadHunterBaseDuration.Value, Configuration.HeadHunterStackDuration.Value, "duration", "cIsDamage")
			);
			RegisterToken("ITEM_HEADHUNTER_DESC", text);



			text = String.Format(
				TextFragment("CONVERT_SHIELD", true),
				ScalingText(1f, "percent", "cIsHealing")
			);
			text += String.Format(
				TextFragment("EXTRA_SHIELD_CONVERT"),
				ScalingText(0.5f, 0.25f, "percent", "cIsHealing")
			);
			if (Configuration.TranscendenceRegen.Value > 0f)
			{
				text += String.Format(
					TextFragment("CONVERT_SHIELD_REGEN"),
					ScalingText(Configuration.TranscendenceRegen.Value, "percent", "cIsHealing")
				);
			}
			if (Catalog.shieldJump)
			{
				text += TextFragment("STAT_EXTRA_JUMP");
			}
			RegisterToken("ITEM_SHIELDONLY_DESC", text);





			targetLanguage = "pt-BR";

			text = String.Format(
				TextFragment("HEADHUNTER"),
				ScalingText(Configuration.HeadHunterBaseDuration.Value, Configuration.HeadHunterStackDuration.Value, "duration", "cIsDamage")
			);
			RegisterToken("ITEM_HEADHUNTER_DESC", text);



			text = String.Format(
				TextFragment("CONVERT_SHIELD", true),
				ScalingText(1f, "percent", "cIsHealing")
			);
			text += String.Format(
				TextFragment("EXTRA_SHIELD_CONVERT"),
				ScalingText(0.5f, 0.25f, "percent", "cIsHealing")
			);
			if (Configuration.TranscendenceRegen.Value > 0f)
			{
				text += String.Format(
					TextFragment("CONVERT_SHIELD_REGEN"),
					ScalingText(Configuration.TranscendenceRegen.Value, "percent", "cIsHealing")
				);
			}
			if (Catalog.shieldJump)
			{
				text += TextFragment("STAT_EXTRA_JUMP");
			}
			RegisterToken("ITEM_SHIELDONLY_DESC", text);





			targetLanguage = "ko";

			text = String.Format(
				TextFragment("HEADHUNTER"),
				ScalingText(Configuration.HeadHunterBaseDuration.Value, Configuration.HeadHunterStackDuration.Value, "duration", "cIsDamage")
			);
			RegisterToken("ITEM_HEADHUNTER_DESC", text);



			text = String.Format(
				TextFragment("CONVERT_SHIELD", true),
				ScalingText(1f, "percent", "cIsHealing")
			);
			text += String.Format(
				TextFragment("EXTRA_SHIELD_CONVERT"),
				ScalingText(0.5f, 0.25f, "percent", "cIsHealing")
			);
			if (Configuration.TranscendenceRegen.Value > 0f)
			{
				text += String.Format(
					TextFragment("CONVERT_SHIELD_REGEN"),
					ScalingText(Configuration.TranscendenceRegen.Value, "percent", "cIsHealing")
				);
			}
			if (Catalog.shieldJump)
			{
				text += TextFragment("STAT_EXTRA_JUMP");
			}
			RegisterToken("ITEM_SHIELDONLY_DESC", text);





			targetLanguage = "";
		}
	}
}
