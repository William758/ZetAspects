using System;
using System.Reflection;
using RoR2;

using static TPDespair.ZetAspects.ReflectionUtility;

namespace TPDespair.ZetAspects.Compat
{
	internal static class NemRisingTides
	{
		internal static Reflector Reflector;
		private static readonly string GUID = "prodzpod.NemesisRisingTides";
		private static readonly string identifier = "[NemRisingTidesCompat]";

		internal static Type BufferedEliteType;
		internal static object BufferedEliteInstance;
		internal static FieldInfo BufferedEliteDef;

		internal static Type OppressiveEliteType;
		internal static object OppressiveEliteInstance;
		internal static FieldInfo OppressiveEliteDef;

		private static bool Prepared = false;



		internal static void PrepareEquipmentCheck()
		{
			if (!Prepared)
			{
				Reflector = new Reflector(GUID, identifier);

				if (!Reflector.FindPluginAssembly()) return;

				GatherEquipmentInfos();

				Prepared = true;
			}
		}



		private static void GatherEquipmentInfos()
		{
			BufferedEliteType = Reflector.GetType("NemesisRisingTides.Contents.Buffered", "EliteBuffered");
			if (BufferedEliteType != null)
			{
				BufferedEliteInstance = Reflector.GetField(BufferedEliteType, "instance").GetValue(null);
				BufferedEliteDef = Reflector.GetField(BufferedEliteType, "eliteDef");
			}

			OppressiveEliteType = Reflector.GetType("NemesisRisingTides.Contents.Oppressive", "EliteOppressive");
			if (OppressiveEliteType != null)
			{
				OppressiveEliteInstance = Reflector.GetField(OppressiveEliteType, "instance").GetValue(null);
				OppressiveEliteDef = Reflector.GetField(OppressiveEliteType, "eliteDef");
			}
		}



		public static bool GetBufferedEnabled()
		{
			if (BufferedEliteInstance != null && BufferedEliteDef != null)
			{
				EliteDef eliteDef = (EliteDef)BufferedEliteDef.GetValue(BufferedEliteInstance);
				if (eliteDef != null)
				{
					if (eliteDef.eliteEquipmentDef != null)
					{
						Logger.Warn(identifier + " - Buffered Elite Is Enabled!");
						return true;
					}
					else
					{
						Logger.Warn(identifier + " - Buffered Elite Is Disabled!");
						return false;
					}
				}
			}

			Logger.Warn(identifier + " - Buffered Elite Enabled Status Is Unknown!");
			return false;
		}

		public static bool GetOppressiveEnabled()
		{
			if (OppressiveEliteInstance != null && OppressiveEliteDef != null)
			{
				EliteDef eliteDef = (EliteDef)OppressiveEliteDef.GetValue(OppressiveEliteInstance);
				if (eliteDef != null)
				{
					if (eliteDef.eliteEquipmentDef != null)
					{
						Logger.Warn(identifier + " - Oppressive Elite Is Enabled!");
						return true;
					}
					else
					{
						Logger.Warn(identifier + " - Oppressive Elite Is Disabled!");
						return false;
					}
				}
			}

			Logger.Warn(identifier + " - Oppressive Elite Enabled Status Is Unknown!");
			return false;
		}
	}
}
