using LookingGlass.ItemStatsNameSpace;
using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

using StatType = LookingGlass.ItemStatsNameSpace.ItemStatsDef.ValueType;
using Units = LookingGlass.ItemStatsNameSpace.ItemStatsDef.MeasurementUnits;

namespace TPDespair.ZetAspects.Compat
{
	/// <summary>
	/// Adds the stat lines LookingGlass shows under item descriptions.
	/// Aspect stats are the values recorded from each aspect description (Language.ScalingText), so they always match the text.
	/// Every method touching LookingGlass types is NoInlining, so nothing loads unless LookingGlass is installed.
	/// </summary>
	internal static class LookingGlassCompat
	{
		public const string GUID = "droppod.lookingglass";
		private static readonly string identifier = "[LookingGlassCompat]";

		public static bool Installed => Catalog.PluginLoaded(GUID);
		public static bool Active => Installed && Configuration.LookingGlassStats.Value;



		private struct Line
		{
			public string description;
			public StatType valueType;
			public Units units;
			public Func<float, float> value;
		}

		private static readonly Dictionary<string, string> labels = new Dictionary<string, string>()
		{
			{ "STAT_HEALTH", "Health" },
			{ "STAT_HEALTH_REDUCTION", "Health Reduction" },
			{ "STAT_HEALTH_EXTRA_SHIELD", "Extra Shield" },
			{ "STAT_EXTRA_JUMP", "Extra Jumps" },
			{ "STAT_MOVESPEED", "Movement Speed" },
			{ "STAT_ATTACKSPEED", "Attack Speed" },
			{ "STAT_BOTHSPEED", "Movement and Attack Speed" },
			{ "STAT_ARMOR", "Armor" },
			{ "STAT_REGENERATION", "Regeneration" },
			{ "STAT_DAMAGE", "Damage" },
			{ "STAT_DAMAGE_DRONE", "Drone Damage" },
			{ "STAT_COOLDOWN", "Cooldown Reduction" },
			{ "STAT_CRIT", "Crit Chance" },
			{ "STAT_CRITMULT", "Crit Damage" },
			{ "STAT_DAMAGE_TAKEN", "Damage Reduction" },
			{ "STAT_DAMAGE_TAKEN_DRONE", "Drone Damage Reduction" },
			{ "STAT_DAMAGE_TAKEN_MINION", "Minion Damage Reduction" },
			{ "STAT_DAMAGE_TAKEN_BARRIER", "Barrier Damage Reduction" },
			{ "STAT_SECONDARY_COOLDOWN", "Secondary Cooldown Reduction" },
			{ "STAT_SECONDARY_CHARGE", "Secondary Charges" },
			{ "CHILL_ON_HIT", "Chill" },
			{ "CHANCE_TO_FREEZE", "Freeze Chance" },
			{ "FROST_BLADE", "Frost Blade Damage" },
			{ "SAP_ON_HIT", "Sap" },
			{ "SCATTER_BOMB", "Scatter Bomb Damage" },
			{ "LIGHTNING_BOMB", "Lightning Bomb Damage" },
			{ "BURN_DOT", "Burn Damage" },
			{ "SHRED_ON_HIT", "Shred" },
			{ "GHOST_ARMOR", "Ghost Armor" },
			{ "GHOST_DODGE", "Ghost Dodge Chance" },
			{ "RUIN_ON_HIT_BASIC", "Ruin" },
			{ "RUIN_ON_HIT", "Ruin" },
			{ "WEAKEN_ON_HIT", "Weaken" },
			{ "CRIPPLE_ON_HIT", "Cripple" },
			{ "POACH_ON_HIT_BASIC", "Poach" },
			{ "POACH_ON_HIT", "Poach" },
			{ "POACH_DETAIL", "Poach Effect" },
			{ "HEAL_PERCENT_ON_HIT", "Healing On Hit" },
			{ "LEECH_MODIFIER_FORMULA", "Leech" },
			{ "NULLIFY_ON_HIT", "Nullify" },
			{ "COLLAPSE_DOT", "Collapse Damage" },
			{ "BEAD_TETHER", "Tether" },
			{ "DAMAGEREDUCTION_ON_HIT", "Stifle" },
			{ "LEVITATE_ON_HIT", "Levitate" },
			{ "WARPED_ON_HIT", "Warp" },
			{ "CLOAK_ON_HIT_TIMED", "Cloak" },
			{ "ELUSIVE_EFFECT_MOVE_DETAIL", "Elusive" },
			{ "ELUSIVE_EFFECT_DODGE_DETAIL", "Elusive" },
			{ "ELUSIVE_EFFECT_BOTH_DETAIL", "Elusive" },
			{ "ELUSIVE_DECAY_DETAIL", "Elusive Decay" },
			{ "ELUSIVE_EFFECT", "Elusive" },
			{ "ANGRY_ATKSPD", "Ally Attack Speed" },
			{ "ANGRY_MOVSPD", "Ally Movement Speed" },
			{ "ANGRY_BOTHSPD", "Ally Movement and Attack Speed" },
			{ "ANGRY_COOLDOWN", "Ally Cooldown Reduction" },
			{ "ITEMSCORE_REGEN_MULT", "Item Score Regeneration" },
			{ "SEPIABLIND_ON_HIT", "Blind" },
			{ "BLEED_DOT", "Bleed Damage" },
			{ "DOT_AMP", "DoT Damage" },
			{ "NULL_ON_HIT_SPD", "Nullify" },
			{ "BACKUPED_ON_HIT", "Backup" },
			{ "BLACKMARK_ON_HIT", "Black Mark" },
			{ "GOLD_FROM_KILL", "Gold From Kills" },
			{ "NIGHTBLIND_ON_HIT", "Blind" },
			{ "NIGHTBLIND_DETAIL", "Blind Effect" },
			{ "OOD_NIGHT_ATKSPD", "Out of Danger Attack Speed" },
			{ "OOD_NIGHT_MOVSPD", "Out of Danger Movement Speed" },
			{ "OOD_NIGHT_BOTHSAMESPD", "Out of Danger Speed" },
			{ "OOD_NIGHT_BOTHDIFFSPD", "Out of Danger Speed" },
			{ "AURA_WARBANNER", "Warbanner Aura" },
			{ "AURA_WARBANNER_ATKSPD", "Warbanner Attack Speed" },
			{ "AURA_WARBANNER_MOVSPD", "Warbanner Movement Speed" },
			{ "EXPLODE_ON_HIT", "Explosion Damage" },
			{ "SANDBLIND_ON_HIT", "Blind" },
			{ "PASSIVE_TINKERDRONE", "Drones" },
			{ "ADAPTIVE_DEFENSE", "Adaptive Armor" },
			{ "ADAPTIVE_REACT", "Adaptive Boost" },
			{ "ADAPTIVE_REACT_INVIS", "Invisibility" },
			{ "LACERATION_ON_HIT", "Laceration" },
			{ "OSMIUM_DAMAGE_DETAIL", "Osmium" },
			{ "NEARBY_ARMOR", "Nearby Ally Armor" },
			{ "BLOCK_CHANCE", "Block Chance" },
			{ "DODGE_CHANCE", "Dodge Chance" },
			{ "PLATING_EFFECT", "Plating" },
			{ "FALL_REDUCTION", "Fall Damage Reduction" },
			{ "FORCE_REDUCTION", "Knockback Reduction" }
		};



		/// <summary>Called after the tokens of an aspect are created, and again whenever they are rebuilt.</summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void RegisterAspect(AspectDef aspectDef)
		{
			try
			{
				if (aspectDef.invalid || aspectDef.recordedStats.Count == 0) return;

				List<Line> lines = BuildLines(aspectDef.recordedStats);
				if (lines.Count == 0) return;

				BuffDef buffDef = aspectDef.buffDef;

				ItemDef itemDef = aspectDef.itemDef;
				if (itemDef && itemDef.itemIndex != ItemIndex.None)
				{
					EnsureTokenRegistered(itemDef.descriptionToken);

					ItemStatsDef def = Create(lines);
					def.calculateValues = (master, count) => Values(lines, count + EquipmentStacks(master, buffDef));
					ItemDefinitions.allItemDefinitions[(int)itemDef.itemIndex] = def;
				}

				EquipmentDef equipDef = aspectDef.equipmentDef;
				if (equipDef && equipDef.equipmentIndex != EquipmentIndex.None)
				{
					EnsureTokenRegistered(equipDef.descriptionToken);

					ItemStatsDef def = Create(lines);
					// equipment has no master, use the local player for the aspect items they hold
					def.calculateValuesFlat = (count) => Values(lines, EquipmentEffect + LocalItemCount(itemDef));
					ItemDefinitions.allEquipmentDefinitions[(int)equipDef.equipmentIndex] = def;
				}
			}
			catch (Exception e)
			{
				Logger.Warn(identifier + " - Failed to register stats for : " + aspectDef.identifier);
				Logger.Warn(e);
			}
		}

		/// <summary>ZetAspects changes Wake of Vultures to grant aspects (and stat buffs) for a configurable duration.</summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static void RegisterHeadHunter()
		{
			try
			{
				ItemDef itemDef = RoR2Content.Items.HeadHunter;
				if (!itemDef || itemDef.itemIndex == ItemIndex.None) return;

				List<Line> lines = new List<Line>()
				{
					new Line { description = "Aspect Duration: ", valueType = StatType.Damage, units = Units.Seconds,
						value = (count) => Configuration.HeadHunterBaseDuration.Value + Configuration.HeadHunterStackDuration.Value * (count - 1f) }
				};

				// per elite kill buff, does not scale with stacks
				if (Configuration.HeadHunterBuffEnable.Value)
				{
					AddConstant(lines, "Health per Kill: ", StatType.Healing, Units.Percentage, Configuration.HeadHunterBuffHealth.Value);
					AddConstant(lines, "Armor per Kill: ", StatType.Healing, Units.Number, Configuration.HeadHunterBuffArmor.Value);
					AddConstant(lines, "Movement Speed per Kill: ", StatType.Utility, Units.Percentage, Configuration.HeadHunterBuffMovementSpeed.Value);
					AddConstant(lines, "Damage per Kill: ", StatType.Damage, Units.Percentage, Configuration.HeadHunterBuffDamage.Value);
					AddConstant(lines, "Attack Speed per Kill: ", StatType.Damage, Units.Percentage, Configuration.HeadHunterBuffAttackSpeed.Value);
					AddConstant(lines, "Crit Chance per Kill: ", StatType.Damage, Units.Percentage, Configuration.HeadHunterBuffCritChance.Value * 0.01f);
				}

				ItemStatsDef def = Create(lines);
				def.calculateValuesFlat = (count) => Values(lines, count);
				ItemDefinitions.allItemDefinitions[(int)itemDef.itemIndex] = def;
			}
			catch (Exception e)
			{
				Logger.Warn(identifier + " - Failed to register stats for : HeadHunter");
				Logger.Warn(e);
			}
		}

		// LookingGlass skips descriptions with Language.IsTokenInvalid.
		// ZetAspects serves its text through hooks, so also place the tokens in the language tables.
		// The hooks still take priority over these values when the text is displayed.
		private static readonly HashSet<string> ensuredTokens = new HashSet<string>();
		private static bool languageHooked = false;

		private static void EnsureTokenRegistered(string token)
		{
			if (string.IsNullOrEmpty(token)) return;

			ensuredTokens.Add(token);

			if (!languageHooked)
			{
				languageHooked = true;
				RoR2.Language.onCurrentLanguageChanged += RefreshTokens;
			}

			SetToken(RoR2.Language.english, token);
			SetToken(RoR2.Language.currentLanguage, token);
		}

		private static void RefreshTokens()
		{
			foreach (string token in ensuredTokens)
			{
				SetToken(RoR2.Language.english, token);
				SetToken(RoR2.Language.currentLanguage, token);
			}
		}

		private static void SetToken(RoR2.Language language, string token)
		{
			if (language == null || language.stringsByToken == null || language.stringsByToken.ContainsKey(token)) return;

			string text = language.GetLocalizedStringByToken(token);
			if (!string.IsNullOrEmpty(text) && text != token) language.stringsByToken[token] = text;
		}

		private static void AddConstant(List<Line> lines, string description, StatType valueType, Units units, float value)
		{
			if (value == 0f) return;

			lines.Add(new Line { description = description, valueType = valueType, units = units, value = (count) => value });
		}



		[MethodImpl(MethodImplOptions.NoInlining)]
		private static List<Line> BuildLines(List<Language.RecordedStat> stats)
		{
			List<Line> lines = new List<Line>();
			HashSet<string> used = new HashSet<string>();

			foreach (Language.RecordedStat stat in stats)
			{
				Units units = GetUnits(stat.modifier);
				float scale = GetScale(stat.modifier);
				float baseValue = stat.baseValue * scale;
				float stackValue = stat.stackValue * scale;

				string label = GetLabel(stat.fragmentKey);
				if (used.Contains(label)) label += " (" + UnitName(units) + ")";
				for (int i = 2; used.Contains(label); i++) label = GetLabel(stat.fragmentKey) + " " + i;
				used.Add(label);

				lines.Add(new Line
				{
					description = label + ": ",
					valueType = GetValueType(stat.style),
					units = units,
					value = (count) => baseValue + stackValue * (count - 1f)
				});
			}

			return lines;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static ItemStatsDef Create(List<Line> lines)
		{
			ItemStatsDef def = new ItemStatsDef();

			foreach (Line line in lines)
			{
				def.descriptions.Add(line.description);
				def.valueTypes.Add(line.valueType);
				def.measurementUnits.Add(line.units);
			}

			return def;
		}

		private static List<float> Values(List<Line> lines, float count)
		{
			count = Mathf.Max(1f, count);

			List<float> values = new List<float>(lines.Count);
			foreach (Line line in lines)
			{
				values.Add(line.value(count));
			}

			return values;
		}



		private static float EquipmentEffect => Mathf.Max(1f, Configuration.AspectEquipmentEffect.Value);

		// held aspect equipment adds to the stack count, same as Catalog.GetStackMagnitude
		private static float EquipmentStacks(CharacterMaster master, BuffDef buffDef)
		{
			if (!master || !buffDef) return 0f;

			Inventory inventory = master.inventory;
			if (!inventory) return 0f;

			int equipCount = Catalog.CountAspectEquipment(inventory, buffDef);
			if (equipCount == 0) return 0f;

			float stacks = equipCount;
			if (master.playerCharacterMasterController) stacks *= EquipmentEffect;

			return stacks;
		}

		private static float LocalItemCount(ItemDef itemDef)
		{
			if (!itemDef) return 0f;

			CharacterMaster master = LocalUserManager.GetFirstLocalUser()?.cachedMaster;
			if (!master || !master.inventory) return 0f;

			return master.inventory.GetItemCountEffective(itemDef);
		}



		private static string GetLabel(string key)
		{
			if (string.IsNullOrEmpty(key)) return "Effect";

			if (labels.TryGetValue(key, out string label)) return label;

			// SOME_FRAGMENT_KEY -> Some Fragment Key
			string[] words = key.ToLowerInvariant().Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < words.Length; i++)
			{
				words[i] = Char.ToUpperInvariant(words[i][0]) + words[i].Substring(1);
			}

			return words.Length > 0 ? String.Join(" ", words) : "Effect";
		}

		private static Units GetUnits(string modifier)
		{
			switch (modifier)
			{
				case "percent":
				case "chance":
					return Units.Percentage;
				case "percentregen":
					return Units.PercentHealing;
				case "flatregen":
					return Units.FlatHealing;
				case "second":
				case "duration":
				case "timer":
					return Units.Seconds;
				case "distance":
				case "range":
				case "meter":
					return Units.Meters;
			}

			return Units.Number;
		}

		// LookingGlass percentages are fractions, chance configs are stored as 0-100
		private static float GetScale(string modifier)
		{
			return modifier == "chance" ? 0.01f : 1f;
		}

		private static string UnitName(Units units)
		{
			switch (units)
			{
				case Units.Seconds: return "Duration";
				case Units.Meters: return "Radius";
				case Units.Percentage: return "%";
				case Units.FlatHealing: return "Healing";
				case Units.PercentHealing: return "Healing";
			}

			return "Amount";
		}

		private static StatType GetValueType(string style)
		{
			switch (style)
			{
				case "cIsDamage": return StatType.Damage;
				case "cIsHealing": return StatType.Healing;
				case "cIsHealth": return StatType.Health;
				case "cIsUtility": return StatType.Utility;
				case "cIsVoid": return StatType.Void;
				case "cDeath": return StatType.Death;
				case "cShrine": return StatType.Shrine;
			}

			return StatType.Utility;
		}
	}
}
