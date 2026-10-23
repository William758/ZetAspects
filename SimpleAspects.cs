using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public enum AspectStatType
	{
		Damage,
		Health,
		AttackSpeed,
		MoveSpeed,
		Armor,
		Regen,
		Crit,
		Cooldown
	}

	/// <summary>A configurable stat bonus that scales with aspect stacks, on top of the source mod's own elite effect.</summary>
	public class AspectStat
	{
		public AspectStatType type;
		public float baseValue;
		public float stackValue;

		public ConfigEntry<float> BaseEntry;
		public ConfigEntry<float> StackEntry;

		public float Base => BaseEntry != null ? BaseEntry.Value : baseValue;
		public float Stack => StackEntry != null ? StackEntry.Value : stackValue;

		public AspectStat(AspectStatType type, float baseValue, float stackValue)
		{
			this.type = type;
			this.baseValue = baseValue;
			this.stackValue = stackValue;
		}

		public float Value(float count)
		{
			if (Base <= 0f) return 0f;

			return Base + Stack * (count - 1f);
		}

		// fragment, value modifier, style
		internal string Describe(bool combine)
		{
			switch (type)
			{
				case AspectStatType.Damage: return Format("STAT_DAMAGE", "percent", "cIsDamage", combine);
				case AspectStatType.Health: return Format("STAT_HEALTH", "percent", "cIsHealing", combine);
				case AspectStatType.AttackSpeed: return Format("STAT_ATTACKSPEED", "percent", "cIsDamage", combine);
				case AspectStatType.MoveSpeed: return Format("STAT_MOVESPEED", "percent", "cIsUtility", combine);
				case AspectStatType.Armor: return Format("STAT_ARMOR", "flat", "cIsHealing", combine);
				case AspectStatType.Regen: return Format("STAT_REGENERATION", "flatregen", "cIsHealing", combine);
				case AspectStatType.Crit: return Format("STAT_CRIT", "chance", "cIsDamage", combine);
				case AspectStatType.Cooldown: return Format("STAT_COOLDOWN", "percent", "cIsUtility", combine);
			}

			return "";
		}

		private string Format(string fragment, string modifier, string style, bool combine)
		{
			return String.Format(Language.TextFragment(fragment), Language.ScalingText(Base, Stack, modifier, style, combine));
		}

		internal string ConfigName
		{
			get
			{
				switch (type)
				{
					case AspectStatType.Damage: return "Damage";
					case AspectStatType.Health: return "Health";
					case AspectStatType.AttackSpeed: return "AttackSpeed";
					case AspectStatType.MoveSpeed: return "MovementSpeed";
					case AspectStatType.Armor: return "Armor";
					case AspectStatType.Regen: return "Regeneration";
					case AspectStatType.Crit: return "CritChance";
					case AspectStatType.Cooldown: return "Cooldown";
				}

				return type.ToString();
			}
		}

		internal string ConfigUnit
		{
			get
			{
				switch (type)
				{
					case AspectStatType.Armor: return " (flat armor)";
					case AspectStatType.Regen: return " (flat health per second, scales with level)";
					case AspectStatType.Crit: return " (percentage points, 10 = +10%)";
				}

				return " (0.2 = +20%)";
			}
		}
	}

	/// <summary>
	/// An aspect that only describes the effect its source mod already gives to the elite buff.
	/// The item, icon and text are all created at runtime, so nothing needs to be added to the assetbundle.
	/// </summary>
	public class SimpleAspect
	{
		/// <summary>Item name, also used for the drop weight config.</summary>
		public string identifier = "";
		/// <summary>Fragment key : AFFIX_{affix}_NAME, AFFIX_{affix}_PICKUP, AFFIX_{affix}_ACTIVE, ASPECT_OF_{affix}, PASSIVE_{affix}</summary>
		public string affix = "";

		public string equipmentName = "";
		public Func<EquipmentDef> FindEquipmentDef;

		public int displayPriority = 1;
		public bool offense = true;
		public bool defence = false;
		public bool copyEquipmentPrefab = true;

		/// <summary>Sprite to recolor if the equipment does not have its own icon.</summary>
		public Func<Sprite> fallbackIcon;
		/// <summary>Recolor target. Uses the elite color if not set.</summary>
		public Color? iconTint;
		public Catalog.IconMode iconMode = Catalog.IconMode.OwnOrRecolor;
		/// <summary>Custom icon, overrides fallbackIcon, iconTint and iconMode.</summary>
		public Func<Sprite> icon;
		public Func<Sprite> outlineIcon;

		/// <summary>Default language text. If name or pickup is null, the english text of the equipment is used.</summary>
		public string name;
		public string pickup;
		public string aspectOf = "";
		/// <summary>Can contain format items that are filled by passiveArgs.</summary>
		public string passive = "";
		public string active = "<style=cStack>(???)</style>";
		public Func<object[]> passiveArgs;

		/// <summary>Stat bonuses granted while the aspect is active, scaling with stacks.</summary>
		public AspectStat[] stats = new AspectStat[0];

		/// <summary>Set any other AspectDef fields.</summary>
		public Action<AspectDef> configure;

		public AspectDef aspectDef;
		public ItemDef itemDef;

		private string Key => affix.ToUpperInvariant();



		public AspectDef Build()
		{
			aspectDef = new AspectDef()
			{
				identifier = identifier,
				equipmentName = equipmentName,
				FindEquipmentDef = FindEquipmentDef,
				itemName = identifier,
				displayPriority = displayPriority,
				copyEquipmentPrefab = copyEquipmentPrefab,
				BaseIcon = icon ?? (() => Catalog.OwnSpriteOrRecolor(fallbackIcon, iconTint, iconMode)),
				OutlineIcon = outlineIcon ?? (() => Catalog.Sprites.OutlineStandard),
				SetupTokens = SetupTokens
			};

			configure?.Invoke(aspectDef);

			All.Add(this);

			return aspectDef;
		}



		internal static readonly List<SimpleAspect> All = new List<SimpleAspect>();

		internal static void BindStatConfigs(ConfigFile config)
		{
			foreach (SimpleAspect aspect in All)
			{
				string name = aspect.identifier.Replace("ZetAspect", "");
				string key = Char.ToLowerInvariant(name[0]) + name.Substring(1);
				string section = "2za-Aspect" + name;

				foreach (AspectStat stat in aspect.stats)
				{
					stat.BaseEntry = config.Bind(
						section, key + "Base" + stat.ConfigName + "Gained", stat.baseValue,
						stat.ConfigName + " gained" + stat.ConfigUnit + ". Applies on top of the elite effect from its own mod. Set to 0 to disable."
					);
					stat.StackEntry = config.Bind(
						section, key + "Added" + stat.ConfigName + "Gained", stat.stackValue,
						stat.ConfigName + " gained per stack."
					);
				}
			}
		}

		/// <summary>Total bonus of the given stat from every active simple aspect on the body.</summary>
		public static float StatTotal(CharacterBody body, AspectStatType type)
		{
			float total = 0f;

			foreach (SimpleAspect aspect in All)
			{
				if (aspect.stats.Length == 0) continue;

				AspectDef def = aspect.aspectDef;
				if (def == null || def.invalid || !def.buffDef || !body.HasBuff(def.buffDef)) continue;

				float count = -1f;

				foreach (AspectStat stat in aspect.stats)
				{
					if (stat.type != type) continue;

					if (count < 0f) count = Catalog.GetStackMagnitude(body, def.buffDef);

					total += stat.Value(count);
				}
			}

			return total;
		}

		internal void ContentLoad()
		{
			if (name != null) Language.RegisterFragmentNoOverwrite("AFFIX_" + Key + "_NAME", name);
			if (pickup != null) Language.RegisterFragmentNoOverwrite("AFFIX_" + Key + "_PICKUP", pickup);
			Language.RegisterFragmentNoOverwrite("AFFIX_" + Key + "_ACTIVE", active);
			Language.RegisterFragmentNoOverwrite("ASPECT_OF_" + Key, aspectOf);
			Language.RegisterFragmentNoOverwrite("PASSIVE_" + Key, passive);

			itemDef = Catalog.CreateItemTemplate(identifier, offense, defence);

			ZetAspectsContent.itemDefs.Add(itemDef);
			Catalog.transformableAspectItemDefs.Add(itemDef);
		}

		private void SetupTokens()
		{
			ResolveEquipmentText();

			Catalog.SetupTokenBase(identifier, affix);
			Catalog.AspectDescriptionBuilder(Describe);
		}

		// must run before SetupTokenBase replaces the equipment tokens
		private void ResolveEquipmentText()
		{
			if (name != null && pickup != null) return;

			EquipmentDef equipDef = aspectDef != null ? aspectDef.equipmentDef : null;

			string lastLanguage = Language.targetLanguage;
			Language.targetLanguage = "default";

			if (name == null) Language.RegisterFragmentNoOverwrite("AFFIX_" + Key + "_NAME", EnglishText(equipDef ? equipDef.nameToken : null, identifier));
			if (pickup == null) Language.RegisterFragmentNoOverwrite("AFFIX_" + Key + "_PICKUP", EnglishText(equipDef ? equipDef.pickupToken : null, ""));

			Language.targetLanguage = lastLanguage;
		}

		private static string EnglishText(string token, string fallback)
		{
			if (string.IsNullOrEmpty(token) || RoR2.Language.english == null) return fallback;

			string text = RoR2.Language.english.GetLocalizedStringByToken(token);
			if (string.IsNullOrEmpty(text) || text == token) return fallback;

			return text;
		}

		private string Describe(bool combine)
		{
			string output = Language.TextFragment("ASPECT_OF_" + Key);

			string passiveText = Language.TextFragment("PASSIVE_" + Key);
			if (passiveArgs != null)
			{
				try
				{
					passiveText = String.Format(passiveText, passiveArgs());
				}
				catch (FormatException)
				{
					Logger.Warn("SimpleAspect - Bad format for PASSIVE_" + Key + " in language : " + Language.targetLanguage);
				}
			}

			output += passiveText;

			foreach (AspectStat stat in stats)
			{
				if (stat.Base > 0f) output += stat.Describe(combine);
			}

			return output;
		}
	}



	public static class SimpleAspectExtensions
	{
		/// <summary>Add simple aspects to a pack. Must be called before Register.</summary>
		public static AspectPack With(this AspectPack pack, params SimpleAspect[] aspects)
		{
			foreach (SimpleAspect aspect in aspects)
			{
				pack.aspectDefs.Add(aspect.Build());
			}

			Action previous = pack.ContentLoad;
			pack.ContentLoad = () =>
			{
				previous?.Invoke();

				foreach (SimpleAspect aspect in aspects)
				{
					aspect.ContentLoad();
				}
			};

			return pack;
		}

		/// <summary>Grant and sustain extra buffs while this aspect is active, for elites that normally get them from OnEquipmentObtained.</summary>
		public static void GrantsBuffs(this AspectDef aspectDef, params Func<BuffDef>[] buffs)
		{
			BuffIndex[] Indexes()
			{
				List<BuffIndex> list = new List<BuffIndex>();

				foreach (Func<BuffDef> buff in buffs)
				{
					BuffDef buffDef = buff();
					if (buffDef && buffDef.buffIndex != BuffIndex.None) list.Add(buffDef.buffIndex);
				}

				return list.ToArray();
			}

			aspectDef.Promoter = (body) => Indexes();

			aspectDef.OnApplied = (body) =>
			{
				foreach (BuffIndex buffIndex in Indexes())
				{
					// natural elites already have a permanent stack from their equipment
					if (!body.HasBuff(buffIndex)) body.AddTimedBuff(buffIndex, BuffHooks.BuffCycleDuration);
				}
			};

			aspectDef.OnExpire = (body) =>
			{
				foreach (BuffIndex buffIndex in Indexes())
				{
					EliteBuffManager.CheckSustain(body, buffIndex);
				}
			};
		}
	}
}
