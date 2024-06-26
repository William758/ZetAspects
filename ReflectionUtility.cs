﻿using BepInEx;
using BepInEx.Bootstrap;
using System;
using System.Reflection;

namespace TPDespair.ZetAspects
{
	public static class ReflectionUtility
	{
		public class Reflector
		{
			public string GUID;
			public string identifier;

			public Assembly PluginAssembly;

			internal static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

			private Reflector()
			{

			}

			public Reflector(string GUID, string identifier)
			{
				this.GUID = GUID;
				this.identifier = identifier;
			}

			public bool FindPluginAssembly()
			{
				Logger.Warn(identifier + " - Find Assembly for : " + GUID);

				if (!Chainloader.PluginInfos.ContainsKey(GUID))
				{
					Logger.Warn(identifier + " - No plugin with that GUID found!");
					return false;
				}

				BaseUnityPlugin plugin = Chainloader.PluginInfos[GUID].Instance;
				PluginAssembly = Assembly.GetAssembly(plugin.GetType());

				if (PluginAssembly == null)
				{
					Logger.Warn(identifier + " - Could Not Get Assembly");

					return false;
				}

				return true;
			}
		}



		public static Type GetType(this Reflector reflector, string typeName)
		{
			Type type = Type.GetType(typeName + ", " + reflector.PluginAssembly.FullName, false);
			if (type != null)
			{
				return type;
			}
			else
			{
				Logger.Warn(reflector.identifier + " - Could Not Find Type : " + typeName);
				return null;
			}
		}

		public static Type GetType(this Reflector reflector, string typeName, string nestedTypeName)
		{
			Type type = reflector.GetType(typeName);
			if (type != null)
			{
				return reflector.GetType(type, nestedTypeName);
			}

			return null;
		}

		public static Type GetType(this Reflector reflector, Type type, string nestedTypeName)
		{
			type = type.GetNestedType(nestedTypeName, Reflector.Flags);
			if (type != null)
			{
				return type;
			}
			else
			{
				Logger.Warn(reflector.identifier + " - Could Not Find NestedType : " + type.Name + "." + nestedTypeName);
				return null;
			}
		}



		public static FieldInfo GetField(this Reflector reflector, string typeName, string fieldName)
		{
			Type type = reflector.GetType(typeName);
			if (type != null)
			{
				return reflector.GetField(type, fieldName);
			}

			return null;
		}

		public static FieldInfo GetField(this Reflector reflector, Type type, string fieldName)
		{
			FieldInfo fieldInfo = type.GetField(fieldName, Reflector.Flags);
			if (fieldInfo != null)
			{
				return fieldInfo;
			}
			else
			{
				Logger.Warn(reflector.identifier + " - Could Not Find Field : " + type.Name + "." + fieldName);
				return null;
			}
		}

		public static FieldInfo GetField(this Reflector reflector, string typeName, string nestedTypeName, string fieldName)
		{
			Type type = reflector.GetType(typeName, nestedTypeName);
			if (type != null)
			{
				return reflector.GetField(type, fieldName);
			}

			return null;
		}



		public static bool GetConfigValue(this Reflector reflector, FieldInfo fieldInfo, bool defaultValue)
		{
			object fieldValue = fieldInfo.GetValue(null);
			Type type = fieldValue.GetType();
			PropertyInfo propInfo = type.GetProperty("Value", Reflector.Flags);
			if (propInfo != null)
			{
				return (bool)propInfo.GetValue(fieldValue);
			}
			else
			{
				Logger.Warn(reflector.identifier + " - Could Not Get Value For : " + fieldInfo.Name);
				return defaultValue;
			}
		}

		public static bool GetConfigValue(this Reflector reflector, object instance, FieldInfo fieldInfo, bool defaultValue)
		{
			object fieldValue = fieldInfo.GetValue(instance);
			Type type = fieldValue.GetType();
			PropertyInfo propInfo = type.GetProperty("Value", Reflector.Flags);
			if (propInfo != null)
			{
				return (bool)propInfo.GetValue(fieldValue);
			}
			else
			{
				Logger.Warn(reflector.identifier + " - Could Not Get Value For : " + fieldInfo.Name);
				return defaultValue;
			}
		}

		public static int GetConfigValue(this Reflector reflector, FieldInfo fieldInfo, int defaultValue)
		{
			object fieldValue = fieldInfo.GetValue(null);
			Type type = fieldValue.GetType();
			PropertyInfo propInfo = type.GetProperty("Value", Reflector.Flags);
			if (propInfo != null)
			{
				return (int)propInfo.GetValue(fieldValue);
			}
			else
			{
				Logger.Warn(reflector.identifier + " - Could Not Get Value For : " + fieldInfo.Name);
				return defaultValue;
			}
		}

		public static int GetConfigValue(this Reflector reflector, object instance, FieldInfo fieldInfo, int defaultValue)
		{
			object fieldValue = fieldInfo.GetValue(instance);
			Type type = fieldValue.GetType();
			PropertyInfo propInfo = type.GetProperty("Value", Reflector.Flags);
			if (propInfo != null)
			{
				return (int)propInfo.GetValue(fieldValue);
			}
			else
			{
				Logger.Warn(reflector.identifier + " - Could Not Get Value For : " + fieldInfo.Name);
				return defaultValue;
			}
		}

		public static float GetConfigValue(this Reflector reflector, FieldInfo fieldInfo, float defaultValue)
		{
			object fieldValue = fieldInfo.GetValue(null);
			Type type = fieldValue.GetType();
			PropertyInfo propInfo = type.GetProperty("Value", Reflector.Flags);
			if (propInfo != null)
			{
				return (float)propInfo.GetValue(fieldValue);
			}
			else
			{
				Logger.Warn(reflector.identifier + " - Could Not Get Value For : " + fieldInfo.Name);
				return defaultValue;
			}
		}

		public static float GetConfigValue(this Reflector reflector, object instance, FieldInfo fieldInfo, float defaultValue)
		{
			object fieldValue = fieldInfo.GetValue(instance);
			Type type = fieldValue.GetType();
			PropertyInfo propInfo = type.GetProperty("Value", Reflector.Flags);
			if (propInfo != null)
			{
				return (float)propInfo.GetValue(fieldValue);
			}
			else
			{
				Logger.Warn(reflector.identifier + " - Could Not Get Value For : " + fieldInfo.Name);
				return defaultValue;
			}
		}
	}
}
