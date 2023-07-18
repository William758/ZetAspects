using BepInEx.Logging;
using System.Reflection;

namespace TPDespair.ZetAspects
{
	internal static class Logger
	{
		internal static ManualLogSource logSource;

		internal static void Info(object data)
		{
			logSource.LogWarning(data);
		}

		internal static void Warn(object data)
		{
			logSource.LogWarning(data);
		}

		internal static void Error(object data)
		{
			logSource.LogError(data);
		}
	}
}
