using System.Collections.Generic;
using UnityEngine;

namespace TPDespair.ZetAspects
{
	public static class Language
	{
		public static Dictionary<string, Dictionary<string, string>> tokens = new Dictionary<string, Dictionary<string, string>>();

		public static string helperTarget = "default";



		public static void RegisterToken(string token, string text, string language = "default")
		{
			if (!tokens.ContainsKey(language)) tokens.Add(language, new Dictionary<string, string>());

			var langDict = tokens[language];

			if (!langDict.ContainsKey(token)) langDict.Add(token, text);
			else langDict[token] = text;
		}



		public static string SecondText(float sec, string modifier = "")
		{
			if (helperTarget == "tr")
			{
				if (modifier == "for") return sec + " saniyeliğine";
				if (modifier == "over") return sec + " saniyenin üzerinde";
				if (modifier == "after") return sec + " saniye sonra";
				return sec + " saniye";
			}
			else
			{
				if (sec == 1.0f) return "1 second";
				return sec + " seconds";
			}
		}

		public static string StackText(float value, string prefix = "", string suffix = "")
		{
			string sign = value >= 0f ? "+" : "-";

			if (helperTarget == "tr")
			{
				return "<style=cStack>(birikim başına " + sign + prefix + Mathf.Abs(value) + suffix + ")</style>";
			}
			else
			{
				return "<style=cStack>(" + sign + prefix + Mathf.Abs(value) + suffix + " per stack)</style>";
			}
		}

		public static string EquipmentStackText(float stack)
		{
			stack = Mathf.Max(1f, stack);

			if (helperTarget == "tr")
			{
				return "\n\n" + stack + " birikim olarak sayılır";
			}
			else
			{
				if (stack == 1.0f) return "\n\nCounts as 1 stack";
				return "\n\nCounts as " + stack + " stacks";
			}
		}

		public static string EquipmentConvertText()
		{
			return "\nClick bottom-right equipment icon to convert";
		}

		public static string EquipmentDescription(string baseDesc, string activeEffect)
		{
			string output = "";
			if (Catalog.aspectAbilities) output += activeEffect + "\n\n";
			output += baseDesc;
			output += EquipmentStackText(Configuration.AspectEquipmentEffect.Value);
			if (Configuration.AspectEquipmentConversion.Value) output += EquipmentConvertText();

			return output;
		}



		internal static void Override()
		{
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

				//Debug.LogWarning(language);

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
	}
}
