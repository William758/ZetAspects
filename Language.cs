using BepInEx;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class Language
	{
		public static string targetLanguage = "default";
		public static bool verboseSetup = false;

		public static Dictionary<string, Dictionary<string, string>> tokens = new Dictionary<string, Dictionary<string, string>>();
		public static Dictionary<string, Dictionary<string, string>> fragments = new Dictionary<string, Dictionary<string, string>>();



		public static void LoadLanguageFiles(Type plugin)
		{
			var assPath = Path.GetDirectoryName(Assembly.GetAssembly(plugin).Location);

			Logger.Info("Loading language files at: " + assPath);

			var languagePaths = Directory.GetFiles(assPath, "*.zetlang", SearchOption.AllDirectories);
			foreach (var path in languagePaths)
			{
				if (path != null)
				{
					var textFile = File.ReadAllText(path);
					if (textFile != null)
					{
						var langDict = LoadFile(textFile);
						if (langDict != null)
						{
							AddFromFileContent(langDict);
						}
					}
				}
			}
		}

		private static void AddFromFileContent(Dictionary<string, Dictionary<string, string>> langDict)
		{
			foreach (var lang in langDict)
			{
				targetLanguage = lang.Key;
				var tokenDict = lang.Value;

				Logger.Info(" - found language: " + targetLanguage);

				foreach (var item in tokenDict)
				{
					if (item.Value == null) continue;

					RegisterFragment(item.Key, item.Value);
				}
			}
		}

		private static Dictionary<string, Dictionary<string, string>> LoadFile(string fileContent)
		{
			Dictionary<string, Dictionary<string, string>> dict = new Dictionary<string, Dictionary<string, string>>();
			try
			{
				JSONNode jsonNode = JSON.Parse(fileContent);
				if (jsonNode == null) return null;

				var languages = jsonNode.Keys;
				foreach (var language in languages)
				{
					JSONNode languageTokens = jsonNode[language];
					if (languageTokens == null) continue;

					var languagename = language;
					if (languagename == "strings") languagename = "default";

					if (!dict.ContainsKey(languagename))
					{
						dict.Add(languagename, new Dictionary<string, string>());
					}
					var languagedict = dict[languagename];

					foreach (var key in languageTokens.Keys)
					{
						languagedict.Add(key, languageTokens[key].Value);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogFormat("Parsing error in zetlang file , Error: {0}", ex);
				return null;
			}
			if (dict.Count == 0)
			{
				return null;
			}
			return dict;
		}



		public static void RegisterToken(string token, string text)
		{
			string language = (targetLanguage != "") ? targetLanguage : "default";

			if (!tokens.ContainsKey(language))
			{
				if (verboseSetup) Logger.Info("Creating (" + language + ") token language.");
				tokens.Add(language, new Dictionary<string, string>());
			}

			var langDict = tokens[language];

			if (!langDict.ContainsKey(token))
			{
				if (verboseSetup) Logger.Info("Creating token (" + token + ") in (" + language + ") token language.");
				langDict.Add(token, text);
			}
			else
			{
				if (verboseSetup) Logger.Info("Replacing token (" + token + ") in (" + language + ") token language.");
				langDict[token] = text;
			}
		}
		
		public static void RegisterFragment(string token, string text)
		{
			string language = (targetLanguage != "") ? targetLanguage : "default";

			if (!fragments.ContainsKey(language))
			{
				if (verboseSetup) Logger.Info("Creating (" + language + ") fragment language.");
				fragments.Add(language, new Dictionary<string, string>());
			}

			var langDict = fragments[language];

			if (!langDict.ContainsKey(token))
			{
				if (verboseSetup) Logger.Info("Creating fragment (" + token + ") in (" + language + ") fragment language.");
				langDict.Add(token, text);
			}
			else
			{
				Logger.Info("Replacing fragment (" + token + ") in (" + language + ") fragment language.");
				langDict[token] = text;
			}
		}
		

		
		// moved to actual files - dont remember if this is hooked by another mod
		internal static void SetupFragments()
		{
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
						if (verboseSetup) Logger.Info("Found fragment (" + key + ") in (" + targetLanguage + ") fragment language.");
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
					if (verboseSetup) Logger.Info("Found fragment (" + key + ") in (default) fragment language.");
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
			else if (modifier == "second" || modifier == "duration" || modifier == "timer")
			{
				baseString = TextFragment("DURATION_VALUE");
				stackString = TextFragment("DURATION_STACK_" + sign);
			}
			else if (modifier == "distance" || modifier == "range" || modifier == "meter")
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

			if (style != "") baseString = ApplyStyle(baseString, style);

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
			else if (modifier == "second" || modifier == "duration" || modifier == "timer") baseString = TextFragment("DURATION_VALUE");
			else if (modifier == "distance" || modifier == "range" || modifier == "meter") baseString = TextFragment("METER_VALUE");
			else baseString = TextFragment("FLAT_VALUE");

			string output = String.Format(baseString, value);

			if (style != "") output = ApplyStyle(output, style);

			return output;
		}
		
		public static string SecondText(float sec, string modifier = "", string style = "")
		{
			string secText = sec.ToString();

			string targetString;
			if (modifier == "for") targetString = "FOR_SECOND";
			else if (modifier == "over") targetString = "OVER_SECOND";
			else if (modifier == "after") targetString = "AFTER_SECOND";
			else if (modifier == "every") targetString = "EVERY_SECOND";
			else targetString = "SECOND";

			if (sec != 1f) targetString += "S";

			if (style != "") secText = ApplyStyle(secText, style);

			return String.Format(TextFragment(targetString), secText);
		}

		public static string ApplyStyle(string text, string style)
		{
			if (style.Length > 1 && style.Substring(0, 1) == "#")
			{
				return "<color=" + style + ">" + text + "</color>";
			}
			else
			{
				return "<style=" + style + ">" + text + "</style>";
			}
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



		public static string HasteText(float baseMS, float stackMS, float baseAS, float stackAS, bool combine = false)
		{
			string output = "";

			if (baseMS == baseAS && stackMS == stackAS)
			{
				if (baseMS > 0f)
				{
					output += String.Format(
						TextFragment("STAT_BOTHSPEED"),
						ScalingText(baseMS, stackMS, "percent", "cIsDamage", combine)
					);
				}
			}
			else
			{
				if (baseMS > 0f)
				{
					output += String.Format(
						TextFragment("STAT_MOVESPEED"),
						ScalingText(baseMS, stackMS, "percent", "cIsUtility", combine)
					);
				}
				if (baseAS > 0f)
				{
					output += String.Format(
						TextFragment("STAT_ATTACKSPEED"),
						ScalingText(baseAS, stackAS, "percent", "cIsDamage", combine)
					);
				}
			}

			return output;
		}

		public static string HasteAuraText(float allyMS, float allyAS, bool self)
		{
			string output = "";

			if (!self)
			{
				if (allyMS == allyAS)
				{
					if (allyMS > 0f)
					{
						output += String.Format(
							TextFragment("ANGRY_BOTHSPD"),
							ScalingText(allyMS, "percent", "cIsDamage")
						);
					}
				}
				else
				{
					if (allyMS > 0f)
					{
						output += String.Format(
							TextFragment("ANGRY_MOVSPD"),
							ScalingText(allyMS, "percent", "cIsUtility")
						);
					}
					if (allyAS > 0f)
					{
						output += String.Format(
							TextFragment("ANGRY_ATKSPD"),
							ScalingText(allyAS, "percent", "cIsDamage")
						);
					}
				}
			}
			else
			{
				if (allyMS == allyAS)
				{
					if (allyMS > 0f)
					{
						output += String.Format(
							TextFragment("AURA_WARBANNER"),
							ScalingText(allyMS, "percent", "cIsDamage")
						);
					}
				}
				else
				{
					if (allyMS > 0f)
					{
						output += String.Format(
							TextFragment("AURA_WARBANNER_MOVSPD"),
							ScalingText(allyMS, "percent", "cIsUtility")
						);
					}
					if (allyAS > 0f)
					{
						output += String.Format(
							TextFragment("AURA_WARBANNER_ATKSPD"),
							ScalingText(allyAS, "percent", "cIsDamage")
						);
					}
				}
			}

			return output;
		}
		


		internal static void Init()
		{
			LoadLanguageFiles(typeof(ZetAspectsPlugin));
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

			foreach (string language in fragments.Keys)
			{
				targetLanguage = language;

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
			}

			targetLanguage = "";
		}
	}
}
